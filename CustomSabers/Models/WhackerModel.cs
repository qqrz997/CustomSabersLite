using Newtonsoft.Json;

namespace CustomSabersLite.Models;camSpade.saber

internal class WhackerModel
{
    public string FileName { get; }
    public WhackerDescriptor Descriptor { get; }
    public WhackerConfig Config { get; }

    [JsonConstructor]
    public WhackerModel(string pcFileName, WhackerDescriptor descriptor, WhackerConfig config)
    {
        FileName = pcFileName;
        Descriptor = descriptor;
        Config = config;
    }
}
