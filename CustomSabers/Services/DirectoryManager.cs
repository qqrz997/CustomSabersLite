using System.IO;
using IPA.Utilities;
using Zenject;

namespace CustomSabersLite.Services;

internal class DirectoryManager : IInitializable
{
    private readonly string customSabersPath = Path.Combine(UnityGame.InstallPath, "CustomSabers");
    private readonly string userDataPath = Path.Combine(UnityGame.UserDataPath, "Custom Sabers Lite");

    public DirectoryManager()
    {
        CustomSabers = new(customSabersPath);
        UserData = new(userDataPath);
        DeletedSabers = UserData.CreateSubdirectory("Deleted Sabers");
    }
    
    public DirectoryInfo CustomSabers { get; }
    public DirectoryInfo UserData { get; }
    public DirectoryInfo DeletedSabers { get; }

    public void Initialize()
    {
        if (!CustomSabers.Exists)
        {
            CustomSabers.Create();
        }

        if (!UserData.Exists)
        {
            UserData.Create();
        }

        if (!DeletedSabers.Exists)
        {
            DeletedSabers.Create();
        }
    }
}
