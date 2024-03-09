using UnityEngine;

namespace CustomSaber.Components
{
    internal class CustomSaber : MonoBehaviour
    {
        public SaberType SaberType { get; private set; }

        public Vector3 customSaberTopPos { get; private set; }

        public Vector3 customSaberBottomPos { get; private set; }

        private Transform customSaberTopTransform;

        private Transform customSaberBottomTransform;

        public GameObject customSaberObject;

        public void Init(GameObject saber)
        {
            customSaberObject = saber;
        }
    }
}
