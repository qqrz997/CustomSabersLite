using CustomSaber;
using CustomSabersLite.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Utilities;

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
        var trailDatas = new Dictionary<Text, WhackerTrailModel>();
        var transformDatas = new Dictionary<Text, WhackerTrailTransform>();

        foreach (var trailDataText in texts.Where(t => t.text.Contains("\"trailColor\":")))
        {
            var trailData = JsonConvert.DeserializeObject<WhackerTrailModel>(trailDataText.text);
            if (trailData is not null) trailDatas.Add(trailDataText, trailData);
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
                TrailUtils.ConvertLegacyLength(trailData.Value.length)));
        }

        return [.. customTrailData];
    }
}
