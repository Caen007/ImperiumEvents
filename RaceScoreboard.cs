using TMPro;
using UnityEngine;

namespace ImperiumEvents
{
    public class RaceScoreboard : MonoBehaviour
    {
        private TMP_Text scoreText;

        private void Awake()
        {
            Transform textTransform = transform.Find("ScoreText");
            if (textTransform != null)
            {
                scoreText = textTransform.GetComponent<TMP_Text>();
            }

            if (scoreText == null)
            {
                scoreText = GetComponentInChildren<TMP_Text>(true);
            }

            if (scoreText != null && string.IsNullOrWhiteSpace(scoreText.text))
            {
                scoreText.text = "IMPERIUM TRIATHLON\n\nWaiting for race...";
            }
        }

        private void OnEnable()
        {
            if (ImperiumTriathlonManager.Instance != null)
            {
                ImperiumTriathlonManager.Instance.RegisterLocalBoard(this);
            }
        }

        private void Start()
        {
            if (ImperiumTriathlonManager.Instance != null)
            {
                ImperiumTriathlonManager.Instance.RegisterLocalBoard(this);
            }
        }

        private void OnDisable()
        {
            if (ImperiumTriathlonManager.Instance != null)
            {
                ImperiumTriathlonManager.Instance.UnregisterLocalBoard(this);
            }
        }

        public void SetBoardText(string text)
        {
            if (scoreText != null)
            {
                scoreText.text = text;
            }
        }
    }
}
