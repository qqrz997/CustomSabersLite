namespace CustomSabersLite.Models;

internal class WhackerConfig
{
    public bool IsLegacy { get; }

    public WhackerConfig(bool isLegacy)
    {
        IsLegacy = isLegacy;
    }
}