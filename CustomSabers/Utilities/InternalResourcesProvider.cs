using BGLib.UnityExtension;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

using Random = UnityEngine.Random;

namespace CustomSabersLite.Utilities;


internal class InternalResourcesProvider : IInitializable
{
    public SaberTrailRenderer SaberTrailRenderer { get; private set; }

    private GameObject SaberModelPrefab { get; set; }

    private readonly List<UnityEngine.Object> loadedObjects = [];

    public void Initialize()
    {
        if (!SaberTrailRenderer)
        {
            var saberTrailRendererPrefab = TryLoadAsset<GameObject>("Assets/Prefabs/Effects/Sabers/SaberTrailRenderer.prefab");
            var saberTrailRendererComponent = saberTrailRendererPrefab.GetComponent<SaberTrailRenderer>();
            saberTrailRendererComponent._meshRenderer = saberTrailRendererPrefab.GetComponent<MeshRenderer>();
            saberTrailRendererComponent._meshFilter = saberTrailRendererPrefab.GetComponent<MeshFilter>();
            SaberTrailRenderer = saberTrailRendererComponent;
        }

        // this probably won't live very long
        var time = Utils.CanUseDateTimeNowSafely ? DateTime.Now : DateTime.UtcNow;
        if (time.Month == 4 && time.Day == 1)
        {
            SaberModelPrefab = TryLoadAsset<GameObject>("Assets/Prefabs/Sabers/BasicSaberModel.prefab");
            SaberModelPrefab.GetComponentsInChildren<MonoBehaviour>().ForEach(GameObject.Destroy);
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
    }

    private void OnSceneChanged(Scene prev, Scene curr)
    {
        if (prev.name == "EmptyTransition" && curr.name == "MainMenu")
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
            for (var i = 0; i < 100; i++)
            {
                var pos = new Vector3(
                    Random.Range(-20f, 20f),
                    Random.Range(0.4f, 10f),
                    Random.Range(-20f, 20f)
                );
                var rot = Random.rotation;
                var color = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f)
                );
                CreateNewSaber(pos, rot, color);
            }
        }
    }

    private void CreateNewSaber(Vector3 pos, Quaternion rot, Color color)
    {
        var saber = GameObject.Instantiate(SaberModelPrefab);
        foreach (var setSaberGlowColor in saber.GetComponentsInChildren<SetSaberGlowColor>())
        {
            var materialPropertyBlock = setSaberGlowColor._materialPropertyBlock ?? new MaterialPropertyBlock();
            foreach (var propertyTintColorPair in setSaberGlowColor._propertyTintColorPairs)
            {
                materialPropertyBlock.SetColor(propertyTintColorPair.property, color * propertyTintColorPair.tintColor);
            }
            setSaberGlowColor._meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
        saber.transform.position = pos;
        saber.transform.rotation = rot;
    }

    private T TryLoadAsset<T>(object label) where T : UnityEngine.Object
    {
        var asset = AddressablesExtensions.LoadContent<T>(label).FirstOrDefault();
        if (asset == null)
        {
            Logger.Error($"Couldn't load internal asset\n\t-{label}");
            return null;
        }
        loadedObjects.Add(asset);
        return asset;
    }
}
