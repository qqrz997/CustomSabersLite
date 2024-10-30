using CustomSaber;
using CustomSabersLite.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class CustomLiteSaber : ILiteSaber
{
    private Material[] ColorableMaterials { get; }

    public GameObject GameObject { get; }
    public Transform Transform { get; }
    public EventManager? EventManager { get; }
    public CustomTrailData[] TrailData { get; }

    public CustomLiteSaber(GameObject gameObject, CustomSaberType customSaberType)
    {
        GameObject = gameObject;
        GameObject.SetLayerRecursively(12);
        Transform = gameObject.transform;
        EventManager = gameObject.TryGetComponentOrAdd<EventManager>();
        TrailData = CustomTrailUtils.GetTrailFromCustomSaber(gameObject, customSaberType);
        ColorableMaterials = GetColorableMaterials(gameObject);
    }

    public void SetColor(Color color)
    {
        foreach (var colorableMaterial in ColorableMaterials)
        {
            colorableMaterial.SetColor(MaterialProperties.Color, color);
        }
    }

    public void SetParent(Transform parent)
    {
        Transform.SetParent(parent, false);
        Transform.position = parent.position;
        Transform.rotation = parent.rotation;
    }

    public void Destroy()
    {
        if (GameObject != null)
        {
            Object.Destroy(GameObject);
        }
    }

    private static Material[] GetColorableMaterials(GameObject gameObject)
    {
        var colorableMaterials = new List<Material>();
        foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null) continue;

            var materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                if (IsColorable(materials[i]))
                {
                    materials[i] = new(materials[i]);
                    renderer.sharedMaterials = materials;
                    colorableMaterials.Add(materials[i]);
                }
            }
        }
        return colorableMaterials.ToArray();
    }

    private static bool IsColorable(Material material) =>
        material != null && material.HasProperty(MaterialProperties.Color) && HasColorableProperty(material);

    private static bool HasColorableProperty(Material material) =>
        material.HasProperty(MaterialProperties.CustomColors) ? material.GetFloat(MaterialProperties.CustomColors) > 0
        : HasGlowOrBloom(material);

    private static bool HasGlowOrBloom(Material material) =>
        material.HasProperty(MaterialProperties.Glow) && material.GetFloat(MaterialProperties.Glow) > 0
        || material.HasProperty(MaterialProperties.Bloom) && material.GetFloat(MaterialProperties.Bloom) > 0;
}
