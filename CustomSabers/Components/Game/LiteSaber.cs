using CustomSaber;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace CustomSabersLite.Components.Game;

internal class LiteSaber : MonoBehaviour
{
    private readonly List<Material> colorableMaterials = [];

    public EventManager EventManager { get; private set; }

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
        GetColorableMaterialsFromSaber();
        EventManager = gameObject.TryGetComponentOrDefault<EventManager>();
    }

    void OnDestroy()
    {
        if (EventManager)
        {
            Destroy(EventManager);
        }
    }

    public void SetColor(Color color)
    {
        foreach (var mat in colorableMaterials)
        {
            mat.SetColor(MaterialProperties.Color, color);
        }
    }

    private void GetColorableMaterialsFromSaber()
    {
        foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
        {
            if (!renderer) continue;

            var materials = renderer.sharedMaterials;

            for (var i = 0; i < materials.Length; i++) 
            {
                var material = materials[i];

                if (!material || !material.HasProperty(MaterialProperties.Color)) continue;

                if (material.HasProperty(MaterialProperties.CustomColors))
                {
                    if (material.GetFloat(MaterialProperties.CustomColors) > 0) AddColorableMaterial(renderer, materials, i);
                }
                else if (material.HasProperty(MaterialProperties.Glow))
                {
                    if (material.GetFloat(MaterialProperties.Glow) > 0) AddColorableMaterial(renderer, materials, i);
                }
                else if (material.HasProperty(MaterialProperties.Bloom))
                {
                    if (material.GetFloat(MaterialProperties.Bloom) > 0) AddColorableMaterial(renderer, materials, i);
                }
            }
        }
    }

    private void AddColorableMaterial(Renderer renderer, Material[] materials, int index)
    {
        materials[index] = new(materials[index]);
        renderer.sharedMaterials = materials;
        colorableMaterials.Add(materials[index]);
    }
}
