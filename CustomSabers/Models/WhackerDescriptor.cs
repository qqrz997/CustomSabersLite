using Newtonsoft.Json;

namespace CustomSabersLite.Models;

[method: JsonConstructor]
internal class WhackerDescriptor(string? objectName, string? author, string? description, string? coverImage)
{
    public string? objectName = objectName;
    public string? author = author;
    public string? description = description;
    public string? coverImage = coverImage;
}
