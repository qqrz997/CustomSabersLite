#if SHADER_DEBUG
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetBundleLoadingTools.Utilities;
using CustomSaber;
using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite;

internal class ShaderInfoDump
{
    private record ShaderInfo(string ModelName, string ShaderName);
    private record ShaderCounts(string ShaderName, int Count);
    private record ShadersWithModels(string ShaderName, string[] ModelNames);
    
    public static ShaderInfoDump Instance { get; } = new();

    private HashSet<ShaderInfo> MissingShaderInfos { get; } = [];
    private HashSet<ShaderInfo> ShaderInfos { get; } = [];

    public async Task RegisterModelShaders(GameObject model, string modelName)
    { 
        var allMaterials = ShaderRepair.GetMaterialsFromGameObjectRenderers(model);
        allMaterials.AddRange(model.GetComponentsInChildren<CustomTrail>().Select(t => t.TrailMaterial)
            .Where(m => m != null && m.shader != null && !allMaterials.Contains(m)));
        allMaterials.ForEach(m => ShaderInfos.Add(new(modelName, m.shader.name)));

        (await ShaderRepairUtils.RepairSaberShadersAsync(model))
            .MissingShaderNames.ForEach(n => MissingShaderInfos.Add(new(modelName, n)));
    }
    
    public void DumpTo(string dir)
    {
        var shaderInfosDir = Directory.CreateDirectory(Path.Combine(dir, "ShaderInfos"));
        var missingShadersDir = Directory.CreateDirectory(Path.Combine(dir, "MissingShaders"));
        DumpShaderCollection(ShaderInfos, shaderInfosDir.FullName);
        DumpShaderCollection(MissingShaderInfos, missingShadersDir.FullName);
    }

    private static void DumpShaderCollection(IEnumerable<ShaderInfo> source, string dir)
    {
        var shaderInfos = source.ToList();

        try
        {
            var missingShaderCounts = shaderInfos
                .GroupBy(i => i.ShaderName)
                .Select(i => new ShaderCounts(i.Key, i.Count()))
                .ToList();
            
            var byCount = missingShaderCounts.OrderByDescending(i => i.Count);
            var byShaderName = missingShaderCounts.OrderBy(i => i.ShaderName);

            var allShaderNames = shaderInfos
                .GroupBy(i => i.ShaderName)
                .OrderByDescending(i => i.Count())
                .Select(i => i.Key)
                .Distinct();
            
            var shaderNamesWithModelNames = shaderInfos
                .GroupBy(info => info.ShaderName)
                .Select(group => new ShadersWithModels(group.Key, group.Select(i => i.ModelName).ToArray()))
                .OrderByDescending(x => x.ModelNames.Length)
                .ToList();

            File.WriteAllText(Path.Combine(dir, "byCount.json"), JsonConvert.SerializeObject(byCount, Formatting.Indented));
            File.WriteAllText(Path.Combine(dir, "byShaderName.json"), JsonConvert.SerializeObject(byShaderName, Formatting.Indented));
            File.WriteAllText(Path.Combine(dir, "allShaderNames.json"), JsonConvert.SerializeObject(allShaderNames, Formatting.Indented));
            File.WriteAllText(Path.Combine(dir, "shaderNamesWithModelNames.json"), JsonConvert.SerializeObject(shaderNamesWithModelNames, Formatting.Indented));
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}
#endif