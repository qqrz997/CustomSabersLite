﻿using IPA.Utilities;
using System.IO;

namespace CustomSabersLite;

internal class PluginDirs
{
    public DirectoryInfo CustomSabers { get; }

    public DirectoryInfo UserData { get; }

    public DirectoryInfo DeletedSabers { get; }

    public PluginDirs()
    {
        CustomSabers = new DirectoryInfo(UnityGame.InstallPath).CreateSubdirectory("CustomSabers");
        UserData = new DirectoryInfo(UnityGame.UserDataPath).CreateSubdirectory("Custom Sabers Lite");
        DeletedSabers = UserData.CreateSubdirectory("Deleted Sabers");
    }
}
