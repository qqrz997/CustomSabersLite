using CustomSaber;
using CustomSabersLite.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomSabersLite.Utilities;

internal class CustomTrailUtils
{
    public static CustomTrailData[] GetTrailFromCustomSaber(Color saberTrailColor, CustomSaberType customSaberType, GameObject saberObject) => customSaberType switch
    {
        CustomSaberType.Saber => TrailsFromSaber(saberObject, saberTrailColor),
        CustomSaberType.Whacker => TrailsFromWhacker(saberObject, saberTrailColor),
        _ => null
    };

    private static CustomTrailData[] TrailsFromSaber(GameObject saberObject, Color saberTrailColor)
    {
        var customTrails = saberObject.GetComponentsInChildren<CustomTrail>();

        return customTrails.Length > 0 ? customTrails
            .Select(ct => new CustomTrailData(ct.PointEnd, ct.PointStart, ct.TrailMaterial, saberTrailColor, ct.Length))
            .ToArray()
            : null;
    }

    private static CustomTrailData[] TrailsFromWhacker(GameObject saberObject, Color saberTrailColor)
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

            customTrailData.Add(new CustomTrailData(trailTop, trailBottom, trailMaterial, saberTrailColor, trailData.Value.length));
        }

        return [.. customTrailData];
    }
}
