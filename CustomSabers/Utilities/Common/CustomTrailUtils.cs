using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using CustomSabersLite.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using static CustomSabersLite.Utilities.Common.TrailUtils;

namespace CustomSabersLite.Utilities.Common;

internal static class CustomTrailUtils
{
    /// <summary>
    /// Searches a GameObject's renderers for any materials which can be recolored.
    /// </summary>
    /// <param name="saberObject">The GameObject of the custom saber</param>
    /// <returns>An array of found <see cref="Material"/>s.</returns>
    public static Material[] GetColorableSaberMaterials(GameObject saberObject)
    {
        var colorableMaterials = new List<Material>();
        foreach (var renderer in saberObject.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null) continue;

            var materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].IsColorable())
                {
                    materials[i] = new(materials[i]);
                    renderer.sharedMaterials = materials;
                    colorableMaterials.Add(materials[i]);
                }
            }
        }
        return colorableMaterials.ToArray();
    }
    
    private static bool IsColorable(this Material material) =>
        material != null && material.HasProperty(MaterialProperties.Color) && material.HasColorableProperty();

    private static bool HasColorableProperty(this Material material) =>
        material.HasProperty(MaterialProperties.CustomColors) ? material.GetFloat(MaterialProperties.CustomColors) > 0
            : material.HasGlowOrBloom();

    private static bool HasGlowOrBloom(this Material material) =>
        material.HasProperty(MaterialProperties.Glow) && material.GetFloat(MaterialProperties.Glow) > 0
        || material.HasProperty(MaterialProperties.Bloom) && material.GetFloat(MaterialProperties.Bloom) > 0;
    
    /// <summary>
    /// Searches a CustomSaber's GameObject for any custom trails.
    /// </summary>
    /// <param name="saberObject">The GameObject of the custom saber</param>
    public static ITrailData[] GetTrailsFromCustomSaber(GameObject saberObject)
    {
        Logger.Notice("Getting trails from Saber");
        var customTrails = saberObject.GetComponentsInChildren<CustomTrail>();
        return customTrails.Length == 0 ? []
            : customTrails.Where(IsCustomTrailValid)
            .Select(ct => new CustomTrailData(
                material: ct.TrailMaterial,
                lengthSeconds: ConvertLegacyLength(ct.Length),
                colorType: ct.colorType,
                customColor: ct.TrailColor,
                colorMultiplier: ct.MultiplierColor,
                saberObjectRoot: saberObject,
                trailTop: ct.PointEnd,
                trailBottom: ct.PointStart))
            .Cast<ITrailData>()
            .ToArray();
        
        static bool IsCustomTrailValid(CustomTrail ct) => ct.PointEnd != null && ct.PointStart != null;
    }
    
    /// <summary>
    /// Searches a Whacker's GameObject for any custom trails.
    /// </summary>
    /// <param name="saberObject">The GameObject of the custom saber</param>
    public static ITrailData[] GetTrailsFromWhacker(GameObject saberObject)
    {
        var texts = saberObject.GetComponentsInChildren<Text>();
        var transformData = texts
            .Where(text => text.text.Contains("\"isTop\":"))
            .Select(text => (
                Transform: text.transform, 
                Data: JsonConvert.DeserializeObject<WhackerTrailTransform>(text.text)))
            .ToList();
        var trailData = texts
            .Where(t => t.text.Contains("\"trailColor\":"))
            .Select(text => (
                Material: text.GetComponent<MeshRenderer>().material,
                Data: JsonConvert.DeserializeObject<WhackerTrail>(text.text)))
            .Where(td => td.Data is not null);
        
        // search the transform data for each trail and find the matching transform data,
        // and take the transform from which that transform data originated from
        return trailData
            .Select(trail => new CustomTrailData(
                material: trail.Material,
                lengthSeconds: ConvertLegacyLength(trail.Data!.Length),
                colorType: trail.Data!.ColorType,
                customColor: trail.Data.TrailColor,
                colorMultiplier: trail.Data.MultiplierColor,
                saberObjectRoot: saberObject,
                trailTop: transformData.Where(transform => transform.Data.isTop)
                    .FirstOrDefault(transform => transform.Data.trailId == trail.Data!.TrailId)
                    .Transform,
                trailBottom: transformData.Where(transform => !transform.Data.isTop)
                    .FirstOrDefault(transform => transform.Data.trailId == trail.Data!.TrailId)
                    .Transform))
            .Cast<ITrailData>()
            .ToArray();
    }
}
