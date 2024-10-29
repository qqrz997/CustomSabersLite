using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Components
{
    internal class DefaultSaberColorer : MonoBehaviour
    {
        private SetSaberGlowColor[] setSaberGlowColors = null!;
        private SetSaberFakeGlowColor[] setSaberFakeGlowColors = null!;

        private void Awake()
        {
            setSaberGlowColors = GetComponentsInChildren<SetSaberGlowColor>();
            setSaberFakeGlowColors = GetComponentsInChildren<SetSaberFakeGlowColor>();
        }

        public void SetColor(Color color)
        {
            foreach (var setSaberGlowColor in setSaberGlowColors)
            {
                setSaberGlowColor.SetNewColor(color);
            }

            foreach (var setSaberFakeGlowColor in setSaberFakeGlowColors)
            {
                setSaberFakeGlowColor.SetNewColor(color);
            }
        }
    }
}
