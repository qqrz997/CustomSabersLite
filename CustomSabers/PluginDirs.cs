using IPA.Utilities;
using System.IO;

namespace CustomSaber
{
    internal class PluginDirs
    {
        public static DirectoryInfo CustomSabers;

        public static DirectoryInfo UserData;

        public static DirectoryInfo Cache;

        public static DirectoryInfo DeletedSabers;

        public static void Init()
        {
            DirectoryInfo installPath = new DirectoryInfo(UnityGame.InstallPath);
            DirectoryInfo userDataPath = new DirectoryInfo(UnityGame.UserDataPath);

            CustomSabers = installPath.CreateSubdirectory("CustomSabers");
            UserData = userDataPath.CreateSubdirectory("Custom Sabers Lite");
            Cache = UserData.CreateSubdirectory("Cache");
            DeletedSabers = UserData.CreateSubdirectory("Deleted Sabers");
        }
    }
}
