using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ImperiumEvents
{
    public class ImperiumTriathlonManager : MonoBehaviour
    {
        public static ImperiumTriathlonManager Instance;

        private const string RpcRegister = "ImperiumTriathlon_Register";
        private const string RpcFinish = "ImperiumTriathlon_Finish";
        private const string RpcStartRace = "ImperiumTriathlon_StartRace";
        private const string RpcAdminReset = "ImperiumTriathlon_AdminReset";
        private const string RpcBoardText = "ImperiumTriathlon_BoardText";
        private const string RpcCenterMessage = "ImperiumTriathlon_CenterMessage";
        private const string RpcResultsPopup = "ImperiumTriathlon_ResultsPopup";

        private static ConfigEntry<string> AdminPlayerNames;
        private static ConfigEntry<string> ResultsFileName;
        private static ConfigEntry<float> CountdownSeconds;
        private static ConfigEntry<bool> RequireConfiguredAdminForStartAndReset;

        private readonly Dictionary<string, EntrantState> entrants = new Dictionary<string, EntrantState>(StringComparer.OrdinalIgnoreCase);
        private readonly List<RaceScoreboard> localBoards = new List<RaceScoreboard>();

        private bool registrationOpen = true;
        private bool raceStarted = false;
        private bool raceEnded = false;
        private double raceStartServerTime = 0d;
        private bool rpcsRegistered = false;

        public static void Initialize(ConfigFile config)
        {
            AdminPlayerNames = config.Bind(
                "Imperium Triathlon",
                "AdminPlayerNames",
                "Caenos",
                "Comma separated player names allowed to start/reset the event. Example: Caenos,James");

            ResultsFileName = config.Bind(
                "Imperium Triathlon",
                "ResultsFileName",
                "ImperiumTriathlonResults.json",
                "JSON file written into BepInEx/config.");

            CountdownSeconds = config.Bind(
                "Imperium Triathlon",
                "CountdownSeconds",
                5f,
                "Countdown before the race starts.");

            RequireConfiguredAdminForStartAndReset = config.Bind(
                "Imperium Triathlon",
                "RequireConfiguredAdminForStartAndReset",
                true,
                "If true, only names in AdminPlayerNames can start/reset the race.");
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(RegisterRpcsWhenReady());
        }

        private IEnumerator RegisterRpcsWhenReady()
        {
            while (ZRoutedRpc.instance == null || ZNet.instance == null)
            {
                yield return null;
            }

            if (!rpcsRegistered)
            {
                ZRoutedRpc.instance.Register<string>(RpcRegister, RPC_Register);
                ZRoutedRpc.instance.Register<string>(RpcFinish, RPC_Finish);
                ZRoutedRpc.instance.Register<string>(RpcStartRace, RPC_StartRace);
                ZRoutedRpc.instance.Register<string>(RpcAdminReset, RPC_AdminReset);
                ZRoutedRpc.instance.Register<string>(RpcBoardText, RPC_BoardText);
                ZRoutedRpc.instance.Register<string>(RpcCenterMessage, RPC_CenterMessage);
                ZRoutedRpc.instance.Register<string>(RpcResultsPopup, RPC_ResultsPopup);
                rpcsRegistered = true;
            }
        }

        public void ForceEndRace()
        {
            if (!ZNet.instance.IsServer())
                return;

            EndRace();
        }
        public void RegisterLocalBoard(RaceScoreboard board)
        {
            if (board == null)
            {
                return;
            }

            if (!localBoards.Contains(board))
            {
                localBoards.Add(board);
                board.SetBoardText(BuildBoardText());
            }
        }

        public void UnregisterLocalBoard(RaceScoreboard board)
        {
            if (board == null)
            {
                return;
            }

            localBoards.Remove(board);
        }

        public bool IsRaceStarted()
        {
            return raceStarted;
        }

        public bool IsRaceEnded()
        {
            return raceEnded;
        }

        public bool IsRegistrationOpen()
        {
            return registrationOpen && !raceStarted && !raceEnded;
        }

        public bool IsConfiguredAdmin(Player player)
        {
            return player != null && IsConfiguredAdmin(player.GetPlayerName());
        }

        public bool IsConfiguredAdmin(string playerName)
        {
            if (!RequireConfiguredAdminForStartAndReset.Value)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return false;
            }

            string[] admins = AdminPlayerNames.Value
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            return admins.Any(x => string.Equals(x, playerName, StringComparison.OrdinalIgnoreCase));
        }

        public void RequestRegister(Player player)
        {
            if (player == null || ZRoutedRpc.instance == null || ZNet.instance == null)
            {
                return;
            }

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, RpcRegister, player.GetPlayerName());
        }

        public void RequestFinish(Player player)
        {
            if (player == null || ZRoutedRpc.instance == null || ZNet.instance == null)
            {
                return;
            }

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, RpcFinish, player.GetPlayerName());
        }

        public void RequestStartRace(Player player)
        {
            if (player == null || ZRoutedRpc.instance == null || ZNet.instance == null)
            {
                return;
            }

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, RpcStartRace, player.GetPlayerName());
        }

        public void RequestAdminReset(Player player)
        {
            if (player == null || ZRoutedRpc.instance == null || ZNet.instance == null)
            {
                return;
            }

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, RpcAdminReset, player.GetPlayerName());
        }

        private void RPC_Register(long sender, string playerName)
        {
            if (!ZNet.instance.IsServer())
            {
                return;
            }

            if (raceStarted)
            {
                BroadcastCenterMessage("Race already started.");
                return;
            }

            if (raceEnded)
            {
                BroadcastCenterMessage("Race ended. Admin must reset the event.");
                return;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return;
            }

            if (!entrants.ContainsKey(playerName))
            {
                entrants.Add(playerName, new EntrantState
                {
                    PlayerName = playerName,
                    Registered = true,
                    Finished = false,
                    StartTime = 0d,
                    FinishTime = 0d
                });

                BroadcastCenterMessage(playerName + " joined the Imperium Triathlon.");
            }
            else
            {
                BroadcastCenterMessage(playerName + " is already registered.");
            }

            BroadcastBoardText(BuildBoardText());
        }

        private void RPC_Finish(long sender, string playerName)
        {
            if (!ZNet.instance.IsServer())
            {
                return;
            }

            if (!raceStarted || raceEnded)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return;
            }

            if (!entrants.TryGetValue(playerName, out EntrantState entrant))
            {
                BroadcastCenterMessage(playerName + " is not registered.");
                return;
            }

            if (entrant.Finished)
            {
                BroadcastCenterMessage(playerName + " already finished.");
                return;
            }

            entrant.Finished = true;
            entrant.FinishTime = Time.realtimeSinceStartupAsDouble - raceStartServerTime;
            entrants[playerName] = entrant;

            BroadcastCenterMessage(playerName + " finished in " + FormatTime(entrant.FinishTime));
            BroadcastBoardText(BuildBoardText());

            if (entrants.Count > 0 && entrants.Values.All(x => x.Finished))
            {
                EndRace();
            }
        }

        private void RPC_StartRace(long sender, string playerName)
        {
            if (!ZNet.instance.IsServer())
            {
                return;
            }

            if (raceStarted)
            {
                BroadcastCenterMessage("Race already started.");
                return;
            }

            if (raceEnded)
            {
                BroadcastCenterMessage("Race ended. Admin must reset the event.");
                return;
            }

            if (entrants.Count == 0)
            {
                BroadcastCenterMessage("No registered players.");
                return;
            }

            if (!IsConfiguredAdmin(playerName))
            {
                BroadcastCenterMessage("Only configured admin players can start the race.");
                return;
            }

            StartCoroutine(ServerStartRaceCountdown());
        }

        private void RPC_AdminReset(long sender, string playerName)
        {
            if (!ZNet.instance.IsServer())
            {
                return;
            }

            if (!raceEnded)
            {
                BroadcastCenterMessage("Race is not in ended state.");
                return;
            }

            if (!IsConfiguredAdmin(playerName))
            {
                BroadcastCenterMessage("Only configured admin players can reset the race.");
                return;
            }

            ResetRaceServer();
            BroadcastCenterMessage("Imperium Triathlon reset. Registration is open.");
            BroadcastBoardText(BuildBoardText());
        }

        private IEnumerator ServerStartRaceCountdown()
        {
            registrationOpen = false;
            BroadcastBoardText(BuildBoardText());

            int seconds = Mathf.Max(1, Mathf.RoundToInt(CountdownSeconds.Value));
            for (int i = seconds; i >= 1; i--)
            {
                BroadcastCenterMessage("Imperium Triathlon starts in " + i);
                yield return new WaitForSeconds(1f);
            }

            raceStarted = true;
            raceEnded = false;
            raceStartServerTime = Time.realtimeSinceStartupAsDouble;

            List<string> keys = entrants.Keys.ToList();
            foreach (string key in keys)
            {
                EntrantState state = entrants[key];
                state.StartTime = raceStartServerTime;
                state.FinishTime = 0d;
                state.Finished = false;
                entrants[key] = state;
            }

            BroadcastCenterMessage("GO!");
            BroadcastBoardText(BuildBoardText());
        }

        private void EndRace()
        {
            raceStarted = false;
            raceEnded = true;
            registrationOpen = false;

            string resultsText = BuildResultsPopupText();
            BroadcastBoardText(BuildBoardText());
            BroadcastResultsPopup(resultsText);
            SaveResultsJson();
        }

        private void ResetRaceServer()
        {
            entrants.Clear();
            registrationOpen = true;
            raceStarted = false;
            raceEnded = false;
            raceStartServerTime = 0d;
        }

        private void BroadcastBoardText(string text)
        {
            if (ZRoutedRpc.instance == null)
            {
                return;
            }

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, RpcBoardText, text ?? string.Empty);
        }

        private void BroadcastCenterMessage(string text)
        {
            if (ZRoutedRpc.instance == null)
            {
                return;
            }

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, RpcCenterMessage, text ?? string.Empty);
        }

        private void BroadcastResultsPopup(string text)
        {
            if (ZRoutedRpc.instance == null)
            {
                return;
            }

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, RpcResultsPopup, text ?? string.Empty);
        }

        private void RPC_BoardText(long sender, string text)
        {
            for (int i = localBoards.Count - 1; i >= 0; i--)
            {
                if (localBoards[i] == null)
                {
                    localBoards.RemoveAt(i);
                    continue;
                }

                localBoards[i].SetBoardText(text);
            }
        }

        private void RPC_CenterMessage(long sender, string text)
        {
            if (MessageHud.instance != null && !string.IsNullOrWhiteSpace(text))
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, text);
            }
        }

        private void RPC_ResultsPopup(long sender, string text)
        {
            if (MessageHud.instance != null && !string.IsNullOrWhiteSpace(text))
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, text);
            }
        }

        private string BuildBoardText()
        {
            if (raceEnded)
            {
                List<EntrantState> sortedFinished = entrants.Values
                    .OrderBy(x => x.FinishTime)
                    .ToList();

                string lines = "IMPERIUM TRIATHLON\n\nRESULTS";
                for (int i = 0; i < sortedFinished.Count; i++)
                {
                    lines += "\n" + (i + 1) + ". " + sortedFinished[i].PlayerName + "  " + FormatTime(sortedFinished[i].FinishTime);
                }

                lines += "\n\nAdmin touch Start Rune to reset";
                return lines;
            }

            if (raceStarted)
            {
                List<EntrantState> finished = entrants.Values
                    .Where(x => x.Finished)
                    .OrderBy(x => x.FinishTime)
                    .ToList();

                List<EntrantState> racing = entrants.Values
                    .Where(x => !x.Finished)
                    .OrderBy(x => x.PlayerName)
                    .ToList();

                string lines = "IMPERIUM TRIATHLON\n\nRACE LIVE";

                if (finished.Count > 0)
                {
                    lines += "\n\nFINISHED";
                    for (int i = 0; i < finished.Count; i++)
                    {
                        lines += "\n" + (i + 1) + ". " + finished[i].PlayerName + "  " + FormatTime(finished[i].FinishTime);
                    }
                }

                if (racing.Count > 0)
                {
                    lines += "\n\nRACING";
                    for (int i = 0; i < racing.Count; i++)
                    {
                        lines += "\n- " + racing[i].PlayerName;
                    }
                }

                return lines;
            }

            string text = "IMPERIUM TRIATHLON\n\nREGISTERED PLAYERS";
            if (entrants.Count == 0)
            {
                text += "\n\nWaiting for racers...";
            }
            else
            {
                int index = 1;
                foreach (EntrantState entrant in entrants.Values.OrderBy(x => x.PlayerName))
                {
                    text += "\n" + index + ". " + entrant.PlayerName;
                    index++;
                }
            }

            text += "\n\nUse Start Rune to register";
            text += "\nUse Horn to start";
            return text;
        }

        private string BuildResultsPopupText()
        {
            List<EntrantState> sortedFinished = entrants.Values
                .OrderBy(x => x.FinishTime)
                .ToList();

            string text = "IMPERIUM TRIATHLON RESULTS";
            for (int i = 0; i < sortedFinished.Count; i++)
            {
                text += "\n" + (i + 1) + ". " + sortedFinished[i].PlayerName + "  " + FormatTime(sortedFinished[i].FinishTime);
            }

            return text;
        }

        private void SaveResultsJson()
        {
            try
            {
                RaceResultsFile file = new RaceResultsFile
                {
                    EventName = "Imperium Triathlon",
                    SavedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Results = entrants.Values
                        .OrderBy(x => x.FinishTime)
                        .Select(x => new RaceResultEntry
                        {
                            PlayerName = x.PlayerName,
                            TimeSeconds = (float)x.FinishTime,
                            TimeFormatted = FormatTime(x.FinishTime)
                        })
                        .ToList()
                };

                string path = Path.Combine(Paths.ConfigPath, ResultsFileName.Value);
                string json = MiniJson.Serialize(file);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Debug.LogError("[Imperium Triathlon] Failed to save results JSON: " + ex);
            }
        }

        private string FormatTime(double seconds)
        {
            TimeSpan span = TimeSpan.FromSeconds(seconds);
            return span.ToString(@"mm\:ss\.ff");
        }

        [Serializable]
        private struct EntrantState
        {
            public string PlayerName;
            public bool Registered;
            public bool Finished;
            public double StartTime;
            public double FinishTime;
        }

        [Serializable]
        private class RaceResultsFile
        {
            public string EventName;
            public string SavedAt;
            public List<RaceResultEntry> Results = new List<RaceResultEntry>();
        }

        [Serializable]
        private class RaceResultEntry
        {
            public string PlayerName;
            public float TimeSeconds;
            public string TimeFormatted;
        }

        private static class MiniJson
        {
            public static string Serialize(RaceResultsFile file)
            {
                if (file == null)
                {
                    return "{}";
                }

                string json = "{\n";
                json += "  \"EventName\": " + Quote(file.EventName) + ",\n";
                json += "  \"SavedAt\": " + Quote(file.SavedAt) + ",\n";
                json += "  \"Results\": [\n";

                for (int i = 0; i < file.Results.Count; i++)
                {
                    RaceResultEntry entry = file.Results[i];
                    json += "    {\n";
                    json += "      \"PlayerName\": " + Quote(entry.PlayerName) + ",\n";
                    json += "      \"TimeSeconds\": " + entry.TimeSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",\n";
                    json += "      \"TimeFormatted\": " + Quote(entry.TimeFormatted) + "\n";
                    json += "    }";

                    if (i < file.Results.Count - 1)
                    {
                        json += ",";
                    }

                    json += "\n";
                }

                json += "  ]\n";
                json += "}";
                return json;
            }

            private static string Quote(string value)
            {
                if (value == null)
                {
                    return "\"\"";
                }

                return "\"" + value
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t") + "\"";
            }
        }
    }
}
