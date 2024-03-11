> [!NOTE]
> Some sabers have shaders that are compiled for the old unity version used in Beat Saber 1.29.0, and may not render properly at all. However, a wide range of sabers are still completely functional and it is up to the saber creators to update the shaders they used if they are broken. Since this mod is in an unfinished state, there may be bugs especially with regards to sabers that were made for other mods that outdate the original Custom Saber plugin.

# Custom Sabers Lite
A PC Beat Saber plugin to replace default sabers with custom sabers similar to how the original [Custom Sabers](https://github.com/Kylemc1413/CustomSaberPlugin) mod worked. This is essentially a stripped-down and re-assembled version that implements some of the basic functionality.

Unlike the original Custom Sabers mod - this mod doesn't allow you to preview sabers in the game menu, and currently only has a few config options. This means that not all custom sabers will work fully as intended. However, most custom sabers should look correct.

- Saber files are loaded from the `CustomSabers` folder in the Beat Saber game directory
- Saber selection and settings can be found in the left panel in the main menu

## Manual Installation
> [!IMPORTANT]
> In addition to BSIPA, you must have [AssetBundleLoadingTools](https://github.com/nicoco007/AssetBundleLoadingTools), [BS Utils](https://github.com/Kylemc1413/Beat-Saber-Utils), and [BeatSaberMarkupLanguage (BSML)](https://github.com/monkeymanboy/BeatSaberMarkupLanguage) installed for this mod to load. Install them using your mod manager i.e. [ModAssistant](https://bsmg.wiki/pc-modding.html#mod-assistant).

### For Beat Saber v1.34.2
Place the contents of the unzipped folder from the latest [release](https://github.com/qqrz997/CustomSabersLite/releases/latest) into your Beat Saber installation folder. For help with installing mods join the [Beat Saber Modding Group](https://discord.gg/beatsabermods) discord server.

## Configuration
Settings are set in-game from the left panel in the Custom Sabers menu.

### Todo
- More settings and possibly better UI
  - Try to add trail intensity to settings
- Look into missing shaders for shader repair
- Independent trail system
- Make sabers appear in multiplayer
