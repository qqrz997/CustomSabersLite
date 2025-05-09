using System.Collections;
using Zenject;

namespace CustomSabersLite.Misc;

// TODO: this patch has been disabled; as of Beat Saber 1.40.4 the problem appears to be fixed. monitoring.

internal class PauseMenuHandlesFix : IInitializable
{
    private readonly MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewController;
    private readonly ICoroutineStarter coroutineStarter;

    public PauseMenuHandlesFix(MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewControllerAndThisIsAReallyLongNameThatMakesMyWordWrappingLookFunny, ICoroutineStarter coroutineStarter)
    {
        multiplayerLocalActivePlayerInGameMenuViewController = multiplayerLocalActivePlayerInGameMenuViewControllerAndThisIsAReallyLongNameThatMakesMyWordWrappingLookFunny;
        this.coroutineStarter = coroutineStarter;
    }

    public void Initialize() => coroutineStarter.StartCoroutine(DisableMenuControllersAfterFrames(5));
        
    private IEnumerator DisableMenuControllersAfterFrames(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        if (multiplayerLocalActivePlayerInGameMenuViewController._menuControllersGameObject != null)
        {
            multiplayerLocalActivePlayerInGameMenuViewController._menuControllersGameObject.SetActive(false);
        }
    }
}
