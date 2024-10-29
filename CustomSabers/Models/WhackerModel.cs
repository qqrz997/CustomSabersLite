using Newtonsoft.Json;

namespace CustomSabersLite.Models;

[method: JsonConstructor]
internal class WhackerModel(string androidFileName, string pcFileName, WhackerDescriptor descriptor, WhackerConfig config)
{
    public string androidFileName = androidFileName;
    public string pcFileName = pcFileName;
    public WhackerDescriptor descriptor = descriptor;
    public WhackerConfig config = config;
}
