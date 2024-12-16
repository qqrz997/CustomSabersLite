using CustomSaber;
using CustomSabersLite.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Utilities;

// TODO: use polymorphism instead of the CustomSaberType to get trails from the saber
internal class CustomTrailUtils
{
    public static CustomTrailData[] GetTrailFromCustomSaber(GameObject saberObject, CustomSaberType customSaberType) => customSaberType switch
    {
        CustomSaberType.Saber => TrailsFromSaber(saberObject),
        CustomSaberType.Whacker => TrailsFromWhacker(saberObject),
        _ => []
    };

    private static CustomTrailData[] TrailsFromSaber(GameObject saberObject)
    {
        var customTrails = saberObject.GetComponentsInChildren<CustomTrail>();
        return customTrails.Length == 0 ? []
            : customTrails.Where(IsTrailValid)
            .Select(ct => new CustomTrailData(
                ct.PointEnd,
                ct.PointStart,
                ct.TrailMaterial,
                ct.colorType,
                ct.TrailColor,
                ct.MultiplierColor,
                TrailUtils.ConvertLegacyLength(ct.Length)))
            .ToArray();
    }

    private static bool IsTrailValid(CustomTrail ct) => ct.PointEnd && ct.PointStart;

    private static CustomTrailData[] TrailsFromWhacker(GameObject saberObject)
    {
        var texts = saberObject.GetComponentsInChildren<Text>();
        var transformData = texts
            .Where(text => text.text.Contains("\"isTop\":"))
            .Select(text => (
                Text: text, 
                Data: JsonConvert.DeserializeObject<WhackerTrailTransform>(text.text)))
            .ToList();
        var trailData = texts
            .Where(t => t.text.Contains("\"trailColor\":"))
            .Select(text => (
                Text: text,
                Data: JsonConvert.DeserializeObject<WhackerTrail>(text.text)))
            .Where(td => td.Data is not null);
        
        // search the transform data for each trail and find the matching transform data,
        // and take the transform from which that transform data originated from
        return trailData
            .Select(trail => new CustomTrailData(
                transformData.Where(transform => transform.Data.isTop)
                    .FirstOrDefault(transform => transform.Data.trailId == trail.Data!.trailId)
                    .Text.transform,
                transformData.Where(transform => !transform.Data.isTop)
                    .FirstOrDefault(transform => transform.Data.trailId == trail.Data!.trailId)
                    .Text.transform,
                trail.Text.GetComponent<MeshRenderer>().material,
                trail.Data!.colorType,
                trail.Data.trailColor,
                trail.Data.multiplierColor,
                TrailUtils.ConvertLegacyLength(trail.Data.length)))
            .ToArray();
    }
}
