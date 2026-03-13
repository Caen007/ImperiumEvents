using UnityEngine;

namespace ImperiumEvents
{
    public class ScoreboardEmissionNight : MonoBehaviour
    {
        private Renderer rend;
        private Material mat;
        private Color originalEmission;
        private float nextCheckTime;
        private bool lastNightState;

        private void Awake()
        {
            rend = GetComponent<Renderer>();

            if (rend != null)
            {
                mat = rend.material;

                if (mat.HasProperty("_EmissionColor"))
                {
                    originalEmission = mat.GetColor("_EmissionColor");
                }
            }
        }

        private void Update()
        {
            if (mat == null || EnvMan.instance == null)
                return;

            if (Time.time < nextCheckTime)
                return;

            nextCheckTime = Time.time + 2f;

            bool isNight = EnvMan.IsNight();

            if (isNight == lastNightState)
                return;

            lastNightState = isNight;

            if (isNight)
            {
                mat.SetColor("_EmissionColor", originalEmission);
                mat.EnableKeyword("_EMISSION");
            }
            else
            {
                mat.SetColor("_EmissionColor", Color.black);
                mat.DisableKeyword("_EMISSION");
            }
        }
    }
}