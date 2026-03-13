using UnityEngine;

namespace ImperiumEvents
{
    public class StartRuneInteract : MonoBehaviour, Hoverable, Interactable
    {
        private Piece m_piece;

        private void Awake()
        {
            m_piece = GetComponent<Piece>();
        }

        public bool Interact(Humanoid user, bool hold, bool alt)
        {
            if (hold)
            {
                return false;
            }

            Player player = user as Player;
            if (player == null || ImperiumTriathlonManager.Instance == null)
            {
                return false;
            }

            if (ImperiumTriathlonManager.Instance.IsRaceEnded())
            {
                if (!ImperiumTriathlonManager.Instance.IsConfiguredAdmin(player))
                {
                    if (MessageHud.instance != null)
                    {
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Only configured admin can reset the event.");
                    }

                    return false;
                }

                ImperiumTriathlonManager.Instance.RequestAdminReset(player);
                return true;
            }

            if (!ImperiumTriathlonManager.Instance.IsRegistrationOpen())
            {
                if (MessageHud.instance != null)
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Registration is closed.");
                }

                return false;
            }

            ImperiumTriathlonManager.Instance.RequestRegister(player);
            return true;
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }

        public string GetHoverText()
        {
            string name = m_piece != null ? m_piece.m_name : "Start Rune";

            if (ImperiumTriathlonManager.Instance != null && ImperiumTriathlonManager.Instance.IsRaceEnded())
            {
                return name + "\n[<color=yellow><b>E</b></color>] Admin Reset Event";
            }

            return name + "\n[<color=yellow><b>E</b></color>] Register for Imperium Triathlon";
        }

        public string GetHoverName()
        {
            return m_piece != null ? m_piece.m_name : "Start Rune";
        }
    }
}