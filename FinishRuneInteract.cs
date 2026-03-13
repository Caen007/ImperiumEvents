using UnityEngine;

namespace ImperiumEvents
{
    public class FinishRuneInteract : MonoBehaviour, Hoverable, Interactable
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

            ImperiumTriathlonManager.Instance.RequestFinish(player);
            return true;
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }

        public string GetHoverText()
        {
            string name = m_piece != null ? m_piece.m_name : "Finish Rune";
            return name + "\n[<color=yellow><b>E</b></color>] Finish Race";
        }

        public string GetHoverName()
        {
            return m_piece != null ? m_piece.m_name : "Finish Rune";
        }
    }
}