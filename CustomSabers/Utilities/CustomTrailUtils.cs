using CustomSaber;
using CustomSabersLite.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using static CustomSabersLite.Utilities.TrailUtils;

namespace CustomSabersLite.Utilities;

internal class CustomTrailUtils
{
    public static CustomTrailData[] GetTrailFromCustomSaber(GameObject saberObject, CustomSaberType customSaberType) => customSaberType switch
    {
        CustomSaberType.Saber => TrailsFromSaber(saberObject),
        CustomSaberType.Whacker => TrailsFromWhacker(saberObject),
        _ => null
    };

    private static CustomTrailData[] TrailsFromSaber(GameObject saberObject)
    {
        var customTrails = saberObject.GetComponentsInChildren<CustomTrail>();
        if (customTrails.Length == 0)
        {
            return null;
        }
        foreach (var invalidTrail in customTrails.Where(ct => !IsTrailValid(ct)))
        {
            Logger.Warn("!! WARNING !!\n" +
                "-------------\n" +
                $"{saberObject.name} has a CustomTrail that is invalid;\n" +
                "if you are the creator of this saber please fix this!\n" +
                $"Invalid trail is on object: {invalidTrail.gameObject.name}\n" +
                "-------------");
        }
        return customTrails
            .Where(IsTrailValid)
            .Select(ct => new CustomTrailData(
                ct.PointEnd,
                ct.PointStart,
                ct.TrailMaterial,
                ct.colorType,
                ct.TrailColor,
                ct.MultiplierColor,
                ConvertLegacyLength(ct.Length)))
            .ToArray();
    }

    private static bool IsTrailValid(CustomTrail ct) => ct.PointEnd && ct.PointStart;

    /*
    private static CustomTrailData[] TrailsFromSaber(GameObject saberObject) => saberObject.GetComponentsInChildren<CustomTrail>() switch
    {
        [] none => null,
        [..] customTrails => customTrails
            .Select(ct => new CustomTrailData(
                ct.PointEnd,
                ct.PointStart,
                ct.TrailMaterial,
                Color.white,
                ConvertLegacyLength(ct.Length)))
            .ToArray()
    };
    */

    private static CustomTrailData[] TrailsFromWhacker(GameObject saberObject)
    {
        var texts = saberObject.GetComponentsInChildren<Text>();
        var trailDatas = new Dictionary<Text, WhackerTrail>();
        var transformDatas = new Dictionary<Text, WhackerTrailTransform>();

        foreach (var trailDataText in texts.Where(t => t.text.Contains("\"trailColor\":")))
        {
            trailDatas.Add(trailDataText, JsonConvert.DeserializeObject<WhackerTrail>(trailDataText.text));
        }
        foreach (var trailTransformText in texts.Where(t => t.text.Contains("\"isTop\":")))
        {
            transformDatas.Add(trailTransformText, JsonConvert.DeserializeObject<WhackerTrailTransform>(trailTransformText.text));
        }

        var customTrailData = new List<CustomTrailData>();

        foreach (var trailData in trailDatas)
        {
            var trailTop = transformDatas.Where(kvp => kvp.Value.trailId == trailData.Value.trailId && kvp.Value.isTop).FirstOrDefault().Key.transform;
            var trailBottom = transformDatas.Where(kvp => kvp.Value.trailId == trailData.Value.trailId && !kvp.Value.isTop).FirstOrDefault().Key.transform;
            var trailMaterial = trailData.Key.GetComponent<MeshRenderer>().material;

            customTrailData.Add(new CustomTrailData(
                trailTop,
                trailBottom,
                trailMaterial,
                trailData.Value.colorType,
                trailData.Value.trailColor,
                trailData.Value.multiplierColor,
                ConvertLegacyLength(trailData.Value.length)));
        }

        return [.. customTrailData];
    }
}
