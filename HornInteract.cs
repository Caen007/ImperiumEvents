using UnityEngine;

namespace ImperiumEvents
{
    public class HornInteract : MonoBehaviour, Hoverable, Interactable
    {
        private ZSFX m_sfx;
        private Piece m_piece;
        private ZNetView m_nview;

        private void Awake()
        {
            m_sfx = GetComponent<ZSFX>();
            m_piece = GetComponent<Piece>();
            m_nview = GetComponent<ZNetView>();

            if (m_nview != null)
            {
                m_nview.Register("ImperiumHornSound", RPC_PlayHorn);
            }
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

            if (!ImperiumTriathlonManager.Instance.IsConfiguredAdmin(player))
            {
                if (MessageHud.instance != null)
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "Only configured admin can start the race.");
                }

                return false;
            }

            if (m_nview != null)
            {
                m_nview.InvokeRPC(ZNetView.Everybody, "ImperiumHornSound");
            }

            ImperiumTriathlonManager.Instance.RequestStartRace(player);
            return true;
        }

        private void RPC_PlayHorn(long sender)
        {
            if (m_sfx != null)
            {
                m_sfx.Play();
            }
        }

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }

        public string GetHoverText()
        {
            string name = m_piece != null ? m_piece.m_name : "Horn";
            return name + "\n[<color=yellow><b>E</b></color>] Start Imperium Triathlon";
        }

        public string GetHoverName()
        {
            return m_piece != null ? m_piece.m_name : "Horn";
        }
    }
}