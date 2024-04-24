> [!NOTE]
> Some sabers have shaders that are compiled for the old unity version used prior to Beat Saber 1.29.4, and may not render properly at all. However, a wide range of sabers are still completely functional and it is up to the saber creators to update the shaders they used if they are broken.

To read more about migrating sabers to the newer versions of Beat Saber, read [this wiki page](https://bsmg.wiki/models/shader-migration.html).

# Custom Sabers Lite
CustomSabersLite is a minimal-feature custom sabers mod, which aims to provide a user-friendly experience without overwhelming players with too many settings and menus. People who were used to the original CustomSabers may find this mod preferable over the alternatives.

The mod is in an incomplete state, so not all planned features are implemented yet. I also aim to make this a helpful tool for saber makers, by providing various warnings to help makers identify problems with their sabers and ensure that their sabers function properly across different mods and versions in the future. 

Credit to the original [Custom Sabers](https://github.com/Kylemc1413/CustomSaberPlugin) mod and its contributors, which was the basis for this mod's creation.

## Manual Installation
> [!IMPORTANT]
> In addition to BSIPA, you must have [AssetBundleLoadingTools](https://github.com/nicoco007/AssetBundleLoadingTools), [SiraUtil](https://github.com/Auros/SiraUtil), and [BeatSaberMarkupLanguage](https://github.com/monkeymanboy/BeatSaberMarkupLanguage) installed for this mod to load. Install them using your mod manager i.e. [ModAssistant](https://bsmg.wiki/pc-modding.html#mod-assistant).

### For Beat Saber v1.35.0

Place the contents of the unzipped folder from the latest [release](https://github.com/qqrz997/CustomSabersLite/releases/latest) into your Beat Saber installation folder. For help with installing mods join the [Beat Saber Modding Group](https://discord.gg/beatsabermods) discord server.

For Beat Saber v1.34.2, download [this release](https://github.com/qqrz997/CustomSabersLite/releases/tag/v0.7.0).

- After launching Beat Saber with the mod successfully installed, a `CustomSabers` folder will be created in your Beat Saber game folder. Saber files should be placed here.

## Configuration
Settings are set in-game from the left panel in the Custom Sabers menu. This is accessed by clicking the Custom Sabers button on the left of the main menu.

You can also adjust settings by clicking the Mods tab at the top of the gameplay settings menu to the left of the song list, then clicking the Custom Sabers tab.

### Todo
- Try to add trail intensity to settings
- Look into HMD only sabers compatibility
- Add a setting to adjust width of trail - dimension of trail mesh parallel to saber blade
- Check if a saber file was changed and reload it to update the cache
- Independent trail system
