using BeatSaberMarkupLanguage;
using BS_Utils.Gameplay;
using CustomSaber.Configuration;
using CustomSaber.Data;
using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomSaber.Utilities
{
    internal class SaberScript : MonoBehaviour
    {
        public static SaberScript Instance;

        private GameObject sabers;

        private GameObject leftSaber;

        private GameObject rightSaber;

        public static ColorScheme colorScheme;

        private EventManager leftEventMananger;

        private EventManager rightEventMananger;

        public static void Load()
        {
            if (Instance != null)
            {
                Destroy(Instance.leftSaber);
                Destroy(Instance.rightSaber);
                Destroy(Instance.sabers);
                Destroy(Instance.gameObject);
            }

            Plugin.Log.Debug("Game scene loaded, loading custom sabers.");

            GameObject loader = new GameObject("Saber Loaded");
            Instance = loader.AddComponent<SaberScript>();
        }

        private void Awake()
        {
            colorScheme = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.colorScheme;
            
            if (sabers)
            {
                Destroy(sabers);
                sabers = null;
            }

            CustomSaberData customSaberData = CustomSaberAssetLoader.CustomSaber[CustomSaberAssetLoader.SelectedSaber];

            if (customSaberData != null)
            {
                Plugin.Log.Debug($"Selected saber: #{CustomSaberAssetLoader.SelectedSaber + 1} {customSaberData.FileName}"); //DefaultSabers

                if (customSaberData.FileName != "DefaultSabers")
                {//
                    if (customSaberData.Sabers)
                    {
                        Plugin.Log.Debug($"Custom saber is selected, replacing sabers: {customSaberData.FileName}");
                        sabers = Instantiate(customSaberData.Sabers);
                        rightSaber = sabers.transform.Find("RightSaber").gameObject;
                        leftSaber = sabers.transform.Find("LeftSaber").gameObject;
                    }

                    StartCoroutine(WaitForSabers(customSaberData.Sabers));
                }
                else
                {
                    StartCoroutine(WaitForDefaultSabers());
                }
            }
        }

        private IEnumerator WaitForDefaultSabers()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());

            IEnumerable<Saber> defaultSabers = Resources.FindObjectsOfTypeAll<Saber>();

            //Hide the default trails if 'None' trail is selected
            //They stay disabled through switching levels so they need to be re-enabled if the setting is changed
            foreach (Saber saber in defaultSabers)
            {
                SaberTrail defaultTrail = GetVanillaTrail(saber);

                if (CustomSaberConfig.Instance.TrailType == TrailType.None)
                {
                    CustomSaberUtils.HideTrail(defaultTrail);
                }
                else
                {
                    CustomSaberUtils.SetTrailDuration(defaultTrail);
                    CustomSaberUtils.SetWhiteTrailDuration(defaultTrail);
                    defaultTrail.enabled = true;
                }
            }
        }

        private IEnumerator WaitForSabers(GameObject saberRoot)
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());

            IEnumerable<Saber> defaultSabers = Resources.FindObjectsOfTypeAll<Saber>();

            foreach (Saber defaultSaber in defaultSabers)
            {
                Plugin.Log.Info($"Hiding default saber model for {defaultSaber.saberType}");

                SaberTrail defaultTrail = GetVanillaTrail(defaultSaber);

                Color saberColor = GetSaberColorByType(defaultSaber.saberType);

                IEnumerable<MeshFilter> meshFilters = defaultSaber.transform.GetComponentsInChildren<MeshFilter>();

                foreach (MeshFilter meshFilter in meshFilters)
                {
                    meshFilter.gameObject.SetActive(!saberRoot);

                    MeshFilter filter = meshFilter.GetComponentInChildren<MeshFilter>();
                    filter.gameObject.SetActive(!saberRoot);
                }

                GameObject saber = GetCustomSaberByType(defaultSaber.saberType);

                AttachSaberToDefaultSaber(saber, defaultSaber);
                SetSaberColour(saber, saberColor);

                switch (CustomSaberConfig.Instance.TrailType)
                {
                    case TrailType.Custom:
                        AddCustomSaberTrails(saber, saberColor, defaultSaber, defaultTrail);
                        break;
                    case TrailType.Vanilla:
                        CustomSaberUtils.SetTrailDuration(defaultTrail);
                        CustomSaberUtils.SetWhiteTrailDuration(defaultTrail);
                        break;
                    default:
                        CustomSaberUtils.HideTrail(defaultTrail); 
                        break;
                }
            }
        }

        private Color GetSaberColorByType(SaberType type)
        {
            Color color = Color.black;
            switch (type)
            {
                case SaberType.SaberA:
                    color = colorScheme.saberAColor; break;
                case SaberType.SaberB:
                    color = colorScheme.saberBColor; break;
            }
            return color;
        }

        private void AttachSaberToDefaultSaber(GameObject saber, Saber defaultSaber)
        {
            if (saber)
            {
                saber.transform.SetParent(defaultSaber.transform);
                saber.transform.position = defaultSaber.transform.position;
                saber.transform.rotation = defaultSaber.transform.rotation;
            }
        }

        private void AddCustomSaberTrails(GameObject saber, Color saberColor, Saber defaultSaber, SaberTrail defaultTrail)
        {
            CustomTrail customTrail = GetCustomTrail(saber);

            if (customTrail == null)
            {
                Plugin.Log.Warn("No custom trails. Defaulting to existing saber trails.");
                CustomSaberUtils.SetTrailDuration(defaultTrail);
            }
            else
            {
                Plugin.Log.Info($"Initializing custom trail to {defaultTrail.name}");

                var handler = new CustomSaberTrailHandler(defaultSaber, customTrail);
                handler.CreateTrail(defaultTrail, saberColor);
            }
        }

        private void SetSaberColour(GameObject saber, Color color)
        {
            IEnumerable<Renderer> renderers = saber.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                foreach (Material rendererMaterial in renderer.sharedMaterials)
                {
                    if (rendererMaterial.HasProperty("_Color"))
                    {
                        if (rendererMaterial.HasProperty("_CustomColors"))
                        {
                            if (rendererMaterial.GetFloat("_CustomColors") > 0)
                                rendererMaterial.SetColor("_Color", color);
                        }
                        else if (rendererMaterial.HasProperty("_Glow") && rendererMaterial.GetFloat("_Glow") > 0
                            || rendererMaterial.HasProperty("_Bloom") && rendererMaterial.GetFloat("_Bloom") > 0)
                        {
                            rendererMaterial.SetColor("_Color", color);
                        }
                    }
                }
            }
        }

        

        private SaberTrail GetVanillaTrail(Saber defaultSaber)
        {
            SaberTrail trail;
            try
            {
                trail = defaultSaber.gameObject.GetComponentInChildren<SaberTrail>();
                Plugin.Log.Debug("Successfully got SaberTrail from default saber.");
            }
            catch
            {
                trail = null;
            }
            return trail;
        }

        private CustomTrail GetCustomTrail(GameObject saber)
        {
            CustomTrail trail;
            try
            {
                trail = saber.GetComponent<CustomTrail>();
                Plugin.Log.Debug("Successfully got CustomTrail from custom saber.");
            }
            catch
            {
                trail = null;
            }
            return trail;
        }

        private GameObject GetCustomSaberByType(SaberType saberType)
        {
            GameObject saber = null;
            if (saberType == SaberType.SaberA)
            {
                saber = leftSaber;
            }
            else if (saberType == SaberType.SaberB)
            {
                saber = rightSaber;
            }
            return saber;
        }
    }
}
