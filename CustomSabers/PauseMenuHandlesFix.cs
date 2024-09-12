using Zenject;

namespace CustomSabersLite;

internal class PauseMenuHandlesFix : IInitializable
{
    [Inject] private readonly MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewController;

    public void Initialize() => 
        multiplayerLocalActivePlayerInGameMenuViewController._menuControllersGameObject.SetActive(false);
}
