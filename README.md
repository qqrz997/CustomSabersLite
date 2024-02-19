# Custom Sabers Lite
A PC Beat Saber plugin to replace default sabers with custom sabers similar to how the original [Custom Sabers](https://github.com/Kylemc1413/CustomSaberPlugin) mod worked. This is essentially a stripped-down and re-assembled version that implements some of the basic functionality.

Unlike the original Custom Sabers mod - this mod doesn't allow you to preview sabers in the game menu, currently only has a few config options, and it also does not handle `EventManager`s. This means that not all custom sabers will work fully as intended. However, most custom sabers should look correct.

- Saber files are loaded from the `CustomSabers` folder in the Beat Saber game directory
- Saber selection and settings can be found in the left panel in the main menu

> [!WARNING]
> If you have several hundreds of MB of sabers, you may experience a much longer load time for your game. I plan on optimizing the loading of sabers in the future.

## Config
Settings are set in-game from the left panel in the Custom Sabers menu. The possible settable settings are as follows:
- `Disable White Trail` Disables the initial white section of trails 
- `Override Trail Length` Allows you to use `Trail Length` to adjust trail length
- `Trail Length` Change the length of trails from 0% to 100%
- `Trail Type` Change the type of trail to use between Custom, Vanilla, and no trails.

## Manual Installation
> [!IMPORTANT]
> In addition to BSIPA, you must have [AssetBundleLoadingTools](https://github.com/nicoco007/AssetBundleLoadingTools), [BS Utils](https://github.com/Kylemc1413/Beat-Saber-Utils), and [BeatSaberMarkupLanguage (BSML)](https://github.com/monkeymanboy/BeatSaberMarkupLanguage) installed for this mod to load. Install them using your mod manager i.e. [ModAssistant](https://bsmg.wiki/pc-modding.html#mod-assistant).

Place the contents of the unzipped folder from the latest [release](https://github.com/qqrz997/CustomSabersLite/releases/latest) into your Beat Saber installation folder. For help with installing mods join the [Beat Saber Modding Group](https://discord.gg/beatsabermods) discord server.

### Todo
- Cache saber metadata to circumvent loading all sabers on startup
- Event managers
- More settings and possibly better UI
