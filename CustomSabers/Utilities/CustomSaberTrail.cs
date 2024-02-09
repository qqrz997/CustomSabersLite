using CustomSaber;
using CustomSaber.Configuration;
using CustomSaber.Data;
using IPA.Utilities;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CustomSaber.Utilities
{
    internal class CustomSaberTrail : SaberTrail
    {
        /*public Transform _pointEnd;

        public Transform _pointStart;

        public Color _trailColor;

        public Color _trailColorMultiplier;

        public float WhiteSectionDuration;*/

        protected bool _inited;

        void Awake()
        {
            //i'm stupid and i don't know why i need this but i do so yer
        }

        public void Setup(SaberTrailRenderer defaultTrailRendererPrefab, SaberTrailRenderer defaultSaberTrailRenderer, MeshRenderer defaultMeshRenderer, TrailElementCollection defaultTrailElementCollection, IBladeMovementData defaultBladeMovementData, Transform pointStart, Transform pointEnd, Material material)
        {
            //Custom saber trails don't all work well with the regular trail values so we have to use their settings including width
            //Extra settings may be needed

            gameObject.layer = 12;

            _inited = true;
        }
    }
}