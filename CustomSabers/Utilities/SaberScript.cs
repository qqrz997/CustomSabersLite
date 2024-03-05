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

        public static ColorScheme colourScheme;

        //private EventManager leftEventMananger; todo - event managers

        //private EventManager rightEventMananger; todo - event managers

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
            colourScheme = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.colorScheme;
            
            if (sabers)
            {
                Destroy(sabers);
                sabers = null;
            }

            if (CustomSaberAssetLoader.SelectedSaber?.FileName != CustomSaberConfig.Instance.CurrentlySelectedSaber &&
                CustomSaberConfig.Instance.CurrentlySelectedSaber != "Default")
            {
                CustomSaberAssetLoader.SelectedSaber?.Destroy();
                CustomSaberAssetLoader.SelectedSaber = CustomSaberAssetLoader.LoadSaberWithRepair(CustomSaberConfig.Instance.CurrentlySelectedSaber);
            }
            if (CustomSaberConfig.Instance.CurrentlySelectedSaber == "Default")
            {
                CustomSaberAssetLoader.SelectedSaber = new CustomSaberData("DefaultSabers");
            }
            CustomSaberData customSaberData = CustomSaberAssetLoader.SelectedSaber;

            if (customSaberData != null)
            {
                if (customSaberData.FileName != "DefaultSabers")
                {
                    if (customSaberData.SabersObject)
                    {
                        Plugin.Log.Debug($"Custom saber is selected, replacing sabers: {customSaberData.FileName}");
                        sabers = Instantiate(customSaberData.SabersObject);
                        rightSaber = sabers.transform.Find("RightSaber").gameObject;
                        leftSaber = sabers.transform.Find("LeftSaber").gameObject;
                    }

                    StartCoroutine(WaitForSabers(customSaberData.SabersObject));
                }
                else
                {
                    StartCoroutine(WaitForDefaultSabers());
                }
            }
            else
            {
                Plugin.Log.Error("Current CustomSaberData is null");
            }
        }

        private IEnumerator WaitForDefaultSabers()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());

            if (CustomSaberUtils.CheckMultiplayer())
            {
                yield break;
            }

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

            if (CustomSaberUtils.CheckMultiplayer())
            { 
                DestroyImmediate(saberRoot);
                yield break; 
            }

            IEnumerable<Saber> defaultSabers = Resources.FindObjectsOfTypeAll<Saber>();

            foreach (Saber defaultSaber in defaultSabers)
            {
                Plugin.Log.Debug($"Hiding default saber model for {defaultSaber.saberType}");

                //Hide each saber mesh
                IEnumerable<MeshFilter> meshFilters = defaultSaber.transform.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    meshFilter.gameObject.SetActive(!saberRoot);

                    /*MeshFilter filter = meshFilter.GetComponentInChildren<MeshFilter>();
                    filter.gameObject.SetActive(!saberRoot);*/
                }

                Color saberColour = Color.white;
                GameObject customSaber = null;

                switch (defaultSaber.saberType)
                {
                    case SaberType.SaberA:
                        customSaber = leftSaber;
                        saberColour = colourScheme.saberAColor;
                        break;

                    case SaberType.SaberB:
                        customSaber = rightSaber;
                        saberColour = colourScheme.saberBColor;
                        break;
                }

                if (customSaber != null)
                {
                    customSaber.transform.SetParent(defaultSaber.transform);
                    customSaber.transform.position = defaultSaber.transform.position;
                    customSaber.transform.rotation = defaultSaber.transform.rotation;
                }
                else
                {
                    Plugin.Log.Error("Something went wrong when getting the custom saber instance"); yield break;
                }

                SetCustomSaberColour(customSaber, saberColour);

                SaberTrail defaultTrail = GetVanillaTrail(defaultSaber);

                switch (CustomSaberConfig.Instance.TrailType)
                {
                    case TrailType.Custom:
                        AddCustomSaberTrails(customSaber, saberColour, defaultSaber, defaultTrail);
                        break;

                    case TrailType.Vanilla:
                        CustomSaberUtils.SetTrailDuration(defaultTrail);
                        CustomSaberUtils.SetWhiteTrailDuration(defaultTrail);
                        break;

                    case TrailType.None:
                        CustomSaberUtils.HideTrail(defaultTrail); 
                        break;
                }
            }
        }

        private void AddCustomSaberTrails(GameObject customSaber, Color saberColour, Saber defaultSaber, SaberTrail defaultTrail)
        {
            CustomTrail customTrail;
            try
            {
                customTrail = customSaber.GetComponent<CustomTrail>();
                Plugin.Log.Debug("Successfully got CustomTrail from custom saber.");
            }
            catch
            {
                customTrail = null;
            }

            if (customTrail == null)
            {
                Plugin.Log.Warn("No custom trails. Defaulting to existing saber trails.");
                CustomSaberUtils.SetTrailDuration(defaultTrail);
                CustomSaberUtils.SetWhiteTrailDuration(defaultTrail);
            }
            else
            {
                Plugin.Log.Debug($"Initializing custom trail to {defaultTrail.name}");

                //Set trail transforms before initializing the trails
                /*ReflectionUtil.SetField(defaultSaber, "_saberBladeTopTransform", customTrail?.PointEnd);
                ReflectionUtil.SetField(defaultSaber, "_saberBladeBottomTransform", customTrail?.PointStart);*/

                var handler = new TrailHandler(customSaber, customTrail);
                handler.CreateTrail(defaultTrail, saberColour);
            }
        }

        //the default trail setup when there are no custom trails is incomplete
        private void SetCustomSaberColour(GameObject saber, Color colour)
        {
            IEnumerable<Renderer> renderers = saber.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;

                foreach (Material rendererMaterial in renderer.materials)
                {
                    if (rendererMaterial == null) continue;

                    if (rendererMaterial.HasProperty("_Color"))
                    {
                        if (rendererMaterial.HasProperty("_CustomColors"))
                        {
                            if (rendererMaterial.GetFloat("_CustomColors") > 0)
                            {
                                rendererMaterial.SetColor("_Color", colour);
                            }
                        }
                        else if (rendererMaterial.HasProperty("_Glow") && rendererMaterial.GetFloat("_Glow") > 0
                            || rendererMaterial.HasProperty("_Bloom") && rendererMaterial.GetFloat("_Bloom") > 0)
                        {
                            rendererMaterial.SetColor("_Color", colour);
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
    }
}
