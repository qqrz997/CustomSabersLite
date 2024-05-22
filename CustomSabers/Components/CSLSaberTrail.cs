using CustomSabersLite.Data;
using UnityEngine;

namespace CustomSabersLite.Utilities
{
    internal class CSLSaberTrail : SaberTrail
    {
        private Transform customTrailTopTransform;
        private Transform customTrailBottomTransform;

        private Vector3 customTrailTopPos;
        private Vector3 customTrailBottomPos;

        public SaberMovementData CustomTrailMovementData { get; } = new SaberMovementData();

        void Awake()
        {
            // i'm stupid and i don't know why i need this but i do so yer
        }

        public void Setup(Transform topTransform, Transform bottomTransform)
        {
            customTrailTopTransform = topTransform;
            customTrailBottomTransform = bottomTransform;

            customTrailTopTransform.name = "Custom Top";
            customTrailBottomTransform.name = "Custom Bottom";

            customTrailTopTransform.SetParent(transform.parent.transform);
            customTrailBottomTransform.SetParent(transform.parent.transform);

            gameObject.layer = 12;
        }

        void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                customTrailTopPos = customTrailTopTransform.position;
                customTrailBottomPos = customTrailBottomTransform.position;
                CustomTrailMovementData.AddNewData(customTrailTopPos, customTrailBottomPos, TimeHelper.time);
            }
        }

        public void SetColor(Color color)
        {
            _color = color;

            foreach (Material rendererMaterial in _trailRenderer._meshRenderer.materials)
            {
                rendererMaterial.SetColor(MaterialProperties.Color, color);
            }
        }
    }
}