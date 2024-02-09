using CustomSaber.Configuration;
using CustomSaber.Data;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSaber.Utilities
{
    internal class CustomSaberTrailHandler
    {
        public CustomSaberTrail TrailInstance { get; set; }

        private readonly CustomTrail _customTrail;

        public CustomSaberTrailHandler(Saber defaultSaber, CustomTrail customTrail)
        {
            _customTrail = customTrail;
            TrailInstance = defaultSaber.gameObject.AddComponent<CustomSaberTrail>();
        }

        private SaberTrailRenderer defaultTrailRendererPrefab;

        private MeshRenderer defaultMeshRenderer;

        private SaberTrailRenderer defaultSaberTrailRenderer;

        private TrailElementCollection defaultTrailElementCollection;

        private IBladeMovementData defaultBladeMovementData;

        public void CreateTrail(SaberTrail defaultTrail, Color saberColor)
        {
            Plugin.Log.Info($"Replacing trail");
            try
            {
                defaultTrailRendererPrefab = ReflectionUtil.GetField<SaberTrailRenderer, SaberTrail>(defaultTrail, "_trailRendererPrefab");
                defaultSaberTrailRenderer = ReflectionUtil.GetField<SaberTrailRenderer, SaberTrail>(defaultTrail, "_trailRenderer");
                defaultMeshRenderer = ReflectionUtil.GetField<MeshRenderer, SaberTrailRenderer>(defaultSaberTrailRenderer, "_meshRenderer");
                defaultTrailElementCollection = ReflectionUtil.GetField<TrailElementCollection, SaberTrail>(defaultTrail, "_trailElementCollection");
                defaultBladeMovementData = ReflectionUtil.GetField<IBladeMovementData, SaberTrail>(defaultTrail, "_movementData");
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex);
                throw;
            }

            TrailInstance.Setup(
                defaultTrailRendererPrefab,
                defaultSaberTrailRenderer,
                defaultMeshRenderer,
                defaultTrailElementCollection,
                defaultBladeMovementData,
                _customTrail.PointStart,
                _customTrail.PointEnd,
                _customTrail.TrailMaterial
            );

            //a later version should do this in a more elegant way if i can figure out a way to
            //Swap material
            MeshRenderer newMeshRenderer = defaultMeshRenderer;
            newMeshRenderer.material = _customTrail.TrailMaterial;

            //Adjusting the trail's meshrenderer before adding it to our trail
            ReflectionUtil.SetField(defaultSaberTrailRenderer, "_meshRenderer", newMeshRenderer);

            //Variables are null so set them
            ReflectionUtil.SetField<SaberTrail, SaberTrailRenderer>(TrailInstance, "_trailRendererPrefab", defaultTrailRendererPrefab);
            if (CustomSaberConfig.Instance.OverrideTrailDuration)
            {
                CustomSaberUtils.SetTrailDuration(TrailInstance);
            }
            if (CustomSaberConfig.Instance.DisableWhiteTrail)
            {
                CustomSaberUtils.SetWhiteTrailDuration(TrailInstance);
            }
            ReflectionUtil.SetField<SaberTrail, Color>(TrailInstance, "_color", saberColor);
            ReflectionUtil.SetField<SaberTrail, IBladeMovementData>(TrailInstance, "_movementData", defaultBladeMovementData);
            ReflectionUtil.SetField<SaberTrail, SaberTrailRenderer>(TrailInstance, "_trailRenderer", defaultSaberTrailRenderer);
            ReflectionUtil.SetField<SaberTrail, TrailElementCollection>(TrailInstance, "_trailElementCollection", defaultTrailElementCollection);
        }
    }
}
