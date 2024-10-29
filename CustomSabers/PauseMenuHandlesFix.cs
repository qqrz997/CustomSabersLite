using System.Collections;
using Zenject;

namespace CustomSabersLite;

#pragma warning disable IDE0031 // Use null propagation

internal class PauseMenuHandlesFix(MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewControllerAndThisIsAReallyLongNameThatMakesMyWordWrappingLookFunny, ICoroutineStarter coroutineStarter) : IInitializable
{
    private readonly MultiplayerLocalActivePlayerInGameMenuViewController multiplayerLocalActivePlayerInGameMenuViewController = multiplayerLocalActivePlayerInGameMenuViewControllerAndThisIsAReallyLongNameThatMakesMyWordWrappingLookFunny;
    private readonly ICoroutineStarter coroutineStarter = coroutineStarter;

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
