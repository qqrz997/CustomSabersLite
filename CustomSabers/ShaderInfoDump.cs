#if SHADER_DEBUG
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite;

internal class ShaderInfoDump
{
    private record MissingShaderInfo(string ModelName, string ShaderName);

    public static ShaderInfoDump Instance { get; } = new();

    private List<MissingShaderInfo> MissingShaderInfos { get; } = [];

    private IEnumerable<string> MissingShaderNames =>
        MissingShaderInfos.GroupBy(i => i.ShaderName).OrderBy(i => i.Count()).Select(i => i.Key).Distinct();

    private IEnumerable<(string shaderName, int appearances)> MissingShaderCounts =>
        MissingShaderInfos.GroupBy(i => i.ShaderName).Select(i => (i.Key, i.Count()));

    public void AddShader(string shaderName, string modelName)
    {
        var shaderInfo = new MissingShaderInfo(modelName, shaderName);
        if (!MissingShaderInfos.Contains(shaderInfo))
        {
            MissingShaderInfos.Add(shaderInfo);
        }
    }

    public void DumpTo(string dir)
    {
        try
        {
            var missingShaderCounts = MissingShaderCounts.Select(x => new { Name = x.shaderName, Count = x.appearances });
            var byCount = missingShaderCounts.OrderBy(i => i.Count);
            var byShaderName = missingShaderCounts.OrderBy(i => i.Name);
            File.WriteAllText(Path.Combine(dir, "byCount.json"), JsonConvert.SerializeObject(byCount, Formatting.Indented));
            File.WriteAllText(Path.Combine(dir, "byShaderName.json"), JsonConvert.SerializeObject(byShaderName, Formatting.Indented));


            var missingShaderNames = MissingShaderNames;
            File.WriteAllText(Path.Combine(dir, "names.json"), JsonConvert.SerializeObject(missingShaderNames, Formatting.Indented));

            var list = new List<(string Shader, int Count, string[] Models)>();
            var q = MissingShaderInfos
                .GroupBy(info => info.ShaderName)
                .Select((group, count) => new
                {
                    ShaderName = group.Key,
                    Count = group.Select(info => info.ModelName).Count(),
                    Models = group.Select(info => info.ModelName).ToArray()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            File.WriteAllText(Path.Combine(dir, "namesWithModels.json"), JsonConvert.SerializeObject(q, Formatting.Indented));
        }
        catch (Exception e)
        {
            Logger.Debug(e);
        }
    }
}
#endif