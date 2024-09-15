using System.Collections.Generic;
using System.Linq;

namespace CustomSabersLite.Models;

public class SaberModelFlags(bool incompatibleShaders, IEnumerable<string> incompatibleShaderNames)
{
    public bool IncompatibleShaders { get; } = incompatibleShaders;

    public string[] IncompatibleShaderNames { get; } = incompatibleShaderNames.ToArray();
}
