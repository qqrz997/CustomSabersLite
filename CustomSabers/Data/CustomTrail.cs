using CustomSaber.Data;
using UnityEngine;

namespace CustomSaber
{
    [AddComponentMenu("Custom Sabers/Custom Trail")]
    public class CustomTrail : MonoBehaviour
    {
        public Transform PointStart;

        public Transform PointEnd;

        public Material TrailMaterial;

        public ColorType colorType = ColorType.CustomColor;

        public Color TrailColor = new Color(1f, 1f, 1f, 1f);

        public Color MultiplierColor = new Color(1f, 1f, 1f, 1f);

        public int Length = 20;
    }
}
