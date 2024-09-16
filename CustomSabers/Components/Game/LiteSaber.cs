using CustomSaber;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace CustomSabersLite.Components.Game;

internal class LiteSaber : MonoBehaviour
{
    private readonly List<Material> colorableMaterials = [];

    public EventManager EventManager { get; private set; } = null!;

    public CustomSaberType Type { get; private set; }

    public void Init(CustomSaberType type) => Type = type;

    public void SetParent(Transform parent, bool worldPositionStays = false)
    {
        transform.SetParent(parent, worldPositionStays);
        transform.position = parent.position;
        transform.rotation = parent.rotation;
    }

    public CustomTrailData[] InstanceTrails =>
        CustomTrailUtils.GetTrailFromCustomSaber(gameObject, Type);

    void Awake()
    {
        EventManager = gameObject.TryGetComponentOrAdd<EventManager>();
        SetLayer(gameObject, 12);
        foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
        {
            if (!renderer) continue;

            var materials = renderer.sharedMaterials;

            for (var i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
                if (material != null && IsColorable(material))
                {
                    materials[i] = new(materials[i]);
                    renderer.sharedMaterials = materials;
                    colorableMaterials.Add(materials[i]);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (EventManager)
        {
            Destroy(EventManager);
        }
    }

    public void SetColor(Color color) =>
        colorableMaterials.ForEach(mat => mat.SetColor(MaterialProperties.Color, color));

    private void SetLayer(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            SetLayer(gameObject.transform.GetChild(i).gameObject, layer);
        }
    }

    private static bool IsColorable(Material material) =>
        material.HasProperty(MaterialProperties.Color) && HasColorableProperty(material);

    private static bool HasColorableProperty(Material material) =>
        material.HasProperty(MaterialProperties.CustomColors) ? material.GetFloat(MaterialProperties.CustomColors) > 0
        : HasGlowOrBloom(material);

    private static bool HasGlowOrBloom(Material material) =>
        material.HasProperty(MaterialProperties.Glow) && material.GetFloat(MaterialProperties.Glow) > 0
        || material.HasProperty(MaterialProperties.Bloom) && material.GetFloat(MaterialProperties.Bloom) > 0;
}
