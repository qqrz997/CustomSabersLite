using IPA.Utilities;
using System.IO;

namespace CustomSabersLite
{
    internal class PluginDirs
    {
        public DirectoryInfo CustomSabers;

        public DirectoryInfo UserData;

        public DirectoryInfo Cache;

        public DirectoryInfo DeletedSabers;

        /*public static void Init()
        {
            DirectoryInfo installPath = new DirectoryInfo(UnityGame.InstallPath);
            DirectoryInfo userDataPath = new DirectoryInfo(UnityGame.UserDataPath);

            CustomSabers = installPath.CreateSubdirectory("CustomSabers");
            UserData = userDataPath.CreateSubdirectory("Custom Sabers Lite");
            Cache = UserData.CreateSubdirectory("Cache");
            DeletedSabers = UserData.CreateSubdirectory("Deleted Sabers");
        }*/

        public PluginDirs()
        {
            Plugin.Log.Info("Setting directories");

            DirectoryInfo installDirectory = new DirectoryInfo(UnityGame.InstallPath);
            DirectoryInfo userDataDirectory = new DirectoryInfo(UnityGame.UserDataPath);
            CustomSabers = installDirectory.CreateSubdirectory("CustomSabers");
            UserData = userDataDirectory.CreateSubdirectory("Custom Sabers Lite");
            Cache = UserData.CreateSubdirectory("Cache");
            DeletedSabers = UserData.CreateSubdirectory("Deleted Sabers");
        }
    }
}
