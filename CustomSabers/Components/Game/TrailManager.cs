using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using UnityEngine;
using CustomSabersLite.Data;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace CustomSabersLite.Components.Game
{
    /// <summary>
    /// Manages an instance of a <seealso cref="LiteSaberTrail"/>
    /// </summary>
    internal class TrailManager
    {
        private readonly CSLConfig config;
        private readonly float saberTrailIntensity;

        private TrailManager(CSLConfig config, GameplayCoreSceneSetupData gameplaySetupData)
        {
            this.config = config;
            saberTrailIntensity  = gameplaySetupData.playerSpecificSettings.saberTrailIntensity;
        }

        private SaberTrailRenderer defaultTrailRendererPrefab;
        private int defaultSamplingFrequency;
        private int defaultGranularity;
        private TrailElementCollection defaultTrailElementCollection;

        /// <summary>
        /// Sets up custom trails for a custom saber
        /// </summary>
        /// <returns>null if no trail is created</returns>
        public LiteSaberTrail[] CreateTrail(Saber defaultSaber, SaberTrail defaultTrail, Color saberTrailColor, LiteSaber customSaber)
        {
            if (config.TrailType == TrailType.None)
            {
                return new LiteSaberTrail[] { };
            }

            GameObject saberObject = customSaber.gameObject;
            defaultTrailRendererPrefab = defaultTrail._trailRendererPrefab;
            defaultSamplingFrequency = defaultTrail._samplingFrequency;
            defaultGranularity = defaultTrail._granularity;
            defaultTrailElementCollection = defaultTrail._trailElementCollection;

            if (config.TrailType == TrailType.Vanilla)
            {
                return SetupTrailUsingDefaultMaterial(saberObject, defaultSaber, saberTrailColor);
            }

            CustomTrailData[] customTrailData = null;

            switch (customSaber.Type)
            {
                case CustomSaberType.Saber:
                    customTrailData = TrailsFromSaber(saberObject, saberTrailColor); break;

                case CustomSaberType.Whacker:
                    customTrailData = TrailsFromWhacker(saberObject, saberTrailColor); break;
            }

            if (customTrailData is null)
            {
                return SetupTrailUsingDefaultMaterial(saberObject, defaultSaber, saberTrailColor);
            }

            List<LiteSaberTrail> trails = new List<LiteSaberTrail>();

            for (int i = 0; i < customTrailData.Length; i++)
            {
                trails.Add(i == 0 ? SetupTrail(saberObject, customTrailData[i]) 
                    : SetupSecondaryTrail(saberObject, customTrailData[i]));
            }

            return trails.ToArray();
        }

        private LiteSaberTrail[] SetupTrailUsingDefaultMaterial(GameObject saberObject, Saber defaultSaber, Color saberTrailColor) =>
            new LiteSaberTrail[] { SetupTrail(saberObject, TrailFromDefaultSaber(defaultSaber, saberTrailColor)) };

        private CustomTrailData TrailFromDefaultSaber(Saber defaultSaber, Color saberTrailColor)
        {
            // Make new transforms based on the default ones, because we cannot modify the default transforms
            Transform trailTop = GameObject.Instantiate(new GameObject()).transform;
            Transform trailBottom = GameObject.Instantiate(new GameObject()).transform;
            trailTop.SetPositionAndRotation(defaultSaber._saberBladeTopTransform.position, Quaternion.identity);
            trailBottom.SetPositionAndRotation(defaultSaber._saberBladeBottomTransform.position, Quaternion.identity);
            return new CustomTrailData(trailTop, trailBottom, new Material(defaultTrailRendererPrefab._meshRenderer.material), saberTrailColor);
        }

        private CustomTrailData[] TrailsFromWhacker(GameObject saberObject, Color saberTrailColor)
        {
            Text[] texts = saberObject.GetComponentsInChildren<Text>();
            Dictionary<Text, WhackerTrail> trailDatas = new Dictionary<Text, WhackerTrail>();
            Dictionary<Text, WhackerTrailTransform> transformDatas = new Dictionary<Text, WhackerTrailTransform>();
            
            foreach (Text trailDataText in texts.Where(t => t.text.Contains("\"trailColor\":")))
            {
                trailDatas.Add(trailDataText, JsonConvert.DeserializeObject<WhackerTrail>(trailDataText.text));
            }
            foreach (Text trailTransformText in texts.Where(t => t.text.Contains("\"isTop\":")))
            {
                transformDatas.Add(trailTransformText, JsonConvert.DeserializeObject<WhackerTrailTransform>(trailTransformText.text));
            }

            IList<CustomTrailData> customTrailData = new List<CustomTrailData>();

            foreach (KeyValuePair<Text, WhackerTrail> trailData in trailDatas) 
            {
                Transform trailTop = transformDatas.Where(kvp => kvp.Value.trailId == trailData.Value.trailId && kvp.Value.isTop).FirstOrDefault().Key.transform;
                Transform trailBottom = transformDatas.Where(kvp => kvp.Value.trailId == trailData.Value.trailId && !kvp.Value.isTop).FirstOrDefault().Key.transform;
                Material trailMaterial = trailData.Key.GetComponent<MeshRenderer>().material;

                customTrailData.Add(new CustomTrailData(trailTop, trailBottom, trailMaterial, saberTrailColor, trailData.Value.length));
            }

            return customTrailData.ToArray();
        }

        private CustomTrailData[] TrailsFromSaber(GameObject saberObject, Color saberTrailColor)
        {
            CustomTrail[] customTrails = saberObject.GetComponentsInChildren<CustomTrail>();

            if (customTrails.Length < 1)
            {
                return null;
            }

            IList<CustomTrailData> customTrailData = new List<CustomTrailData>();

            foreach (CustomTrail customTrail in customTrails)
            {
                customTrailData.Add(new CustomTrailData(customTrail.PointEnd, customTrail.PointStart, customTrail.TrailMaterial, saberTrailColor, customTrail.Length));
            }

            return customTrailData.ToArray();
        }

        private LiteSaberTrail SetupTrail(GameObject saberObject, CustomTrailData customTrailData)
        {
            LiteSaberTrail trail = saberObject.gameObject.AddComponent<LiteSaberTrail>();

            if (config.OverrideTrailWidth)
            {
                Vector3 trailTop = customTrailData.TrailTop.position;
                Vector3 trailBottom = customTrailData.TrailBottom.position;
                float distance = Vector3.Distance(trailTop, trailBottom);
                float width = distance > 0 ? config.TrailWidth / 100f / distance : 1f;

                customTrailData.TrailBottom.position = Vector3.LerpUnclamped(trailTop, trailBottom, width);
            }
            return InitTrail(customTrailData, trail);
        }

        private LiteSaberTrail SetupSecondaryTrail(GameObject saberObject, CustomTrailData customTrailData)
        {
            LiteSaberTrail trail = saberObject.gameObject.AddComponent<LiteSaberTrail>();
            return InitTrail(customTrailData, trail);
        }

        private LiteSaberTrail InitTrail(CustomTrailData customTrailData, LiteSaberTrail trail)
        {
            trail._trailDuration = TrailUtils.ConvertedDuration(customTrailData.Length);
            trail._samplingFrequency = defaultSamplingFrequency;
            trail._granularity = defaultGranularity;
            trail._color = customTrailData.Color;
            trail._trailRenderer = GameObject.Instantiate(defaultTrailRendererPrefab, Vector3.zero, Quaternion.identity);
            trail._trailRenderer._meshRenderer.material = customTrailData.Material;
            trail._trailRenderer._meshRenderer.material.color = customTrailData.Color.ColorWithAlpha(saberTrailIntensity);
            trail._trailElementCollection = defaultTrailElementCollection;

            trail.Setup(customTrailData.TrailTop, customTrailData.TrailBottom);
            trail.ConfigureTrail(config);

            return trail;
        }
    }
}
