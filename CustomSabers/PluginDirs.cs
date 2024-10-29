using IPA.Utilities;
using System.IO;

namespace CustomSabersLite;

internal class PluginDirs
{
    private static readonly DirectoryInfo customSabers = new DirectoryInfo(UnityGame.InstallPath).CreateSubdirectory("CustomSabers");
    private static readonly DirectoryInfo userData = new DirectoryInfo(UnityGame.UserDataPath).CreateSubdirectory("Custom Sabers Lite");
    private static readonly DirectoryInfo deletedSabers = userData.CreateSubdirectory("Deleted Sabers");

    public static DirectoryInfo CustomSabers => customSabers;
    public static DirectoryInfo UserData => userData;
    public static DirectoryInfo DeletedSabers => deletedSabers;
}
