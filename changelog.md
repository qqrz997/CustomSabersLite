# changelog

## 1.0.2
- Added a fix for TrickSaber throwing not working with CustomSabersLite enabled

## 1.0.1
- The preview will now show the default red and blue colours if you have override colours disabled
- Reordered the built-in trail options
- Fixed an issue where similar but not identical sabers would not load at the same time
- Fixed the trail length toggle not respecting the default trail length of the custom trail for the held saber preview
- Fixed the trail length setting being able to disable the trail in the held saber preview

## 1.0.0
- Features a fresh new menu with cleaner icons and interface elements
- Placing saber files within folders inside the `CustomSabers` folder will now cause those folders to show in the menu and can be viewed from the saber list
- Custom sabers' trails can be swapped out for custom trails from other sabers by selecting the 'trails' tab in the saber list
- Sabers can be favourited. When a saber is favourited, it can be found in the "favourites folder" in the saber list
- When switching between held and static previews the saber model will animate between the two positions

## 0.14.2
- Changed the way that saber objects are created so that GameObjects not parented to the left/right sabers such as events are included 

## 0.14.1
- Changed the saber width increments from 10% to 1%

## 0.14.0
### New
- Added a setting to change the length of the saber model
- Added a setting to change the width of the saber model
### Changes
- Added icons to each saber setting
- Added a % symbol to each saber setting slider
- Reduced the amount of vertical space that saber settings take u
### Fixes
- Fixed the preview not showing the selected custom saber on first menu activation
- Fixed the default saber's trail disappearing when disabling the "Mod Enabled" setting

## 0.13.2
- Fixed the saber list in the gameplay setup tab not being able to select any sabers
- Fixed the saber list in the main menu sometimes not being ordered as it is supposed to be

## 0.13.1
- Fix sabers potentially being mispositioned in the hands

## 0.13.0
### New
- Added saber list sorting; you can now sort by Saber Name, Author Name, and by the most recently added sabers
- Added a search bar which searches sabers by their names and authors
- Added the default saber to the main menu previews
- Added scroll buttons that will scroll to the top and bottom of the list
- Added a button to scroll to the selected saber
- Trail type options (Custom, Vanilla, None) now apply to the stationary saber preview
### Changes
- Changed the default sabers' image
- Changed the layout of the saber list UI to make it more compact
- Changed the position of the stationary preview so that it is closer, and so that it is much easier to see 
### Fixes
- Deleting a saber with a very long file name should now look correct
- Deleting a saber will now select the saber above the deleted saber in the list
- It should no longer be possible to experience unintentional invisible sabers when playing a level
- Tweaked the multiplayer pause menu pointers patch to be slightly more reliable

## 0.12.5
- Updated to 1.37.5

## 0.12.4
- Fixed the correct saber not being selected after deleting a saber
- Fixed an error caused by deleting the last saber in the list

## 0.12.3
- Fixed sabers not showing/hiding correctly according to Camera2's saber visibility setting

## 0.12.2
- Removed a preventitive measure to stop certain sabers from loading which would crash the game on Unity 2021.3.16f1; this is no longer a problem from Beat Saber 1.37.2 onwards

## 0.12.1
- Fixed incompatibility with other saber mods

## 0.12.0
- Added the ability to hold sabers in the menu
- Removed the saber color scheme setting, however, this feature will be moved into its own mod
- Game and menu saber trails now respect color types and color multipliers as they were originally intended to be used
- Fixed the conversion of legacy custom trail length to new trail duration
- Fixed the saber list selecting the default sabers upon opening the menu
- Fixed some edge case errors related to loading sabers and trails

## 0.11.1
- A large optimization which makes it possible to have large quantities of saber files in the CustomSabers folder
- It is no longer necessary to wait until all sabers are loaded before being able to access the custom sabers settings menu
- Added a progress bar in the custom sabers settings menu
- Made a blacklist for sabers that cause an internal unity crash when loaded due to the unity update
- If there was an error loading a saber, the saber list will display a brief description of the error
- Various code rewrite and clean-up

## 0.11.0
- Added sabers' custom trails to the saber preview

## 0.10.6
- Fixed a mistake that was causing regular crashing

## 0.10.5
- Now compatible with TrickSaber
- Fixed a very rare case where you wouldn't be able to use a certain saber anymore for the duration of the session
- Added some extra debugging to identify sabers that crash the game when they are loaded

## 0.10.4
- Cached saber icons are now compressed
- Fixed an error message when loading a level with default sabers
- Fixed an bug when trying to load a saber with a huge file name, causing a path too long error - note - this means that said sabers are no longer loaded, one must consider changing the file name for the saber to load

## 0.10.3
- Updated for Beat Saber v1.37.0
- Saber preview now shows your selected colour scheme
- Fixed an error caused when opening the saber menu after playing a level
- Possible fix for whackers sometimes being unable to be deleted from the menu

## 0.10.2
- Fixed saber model occasionally being off axis

## 0.10.1
- Saber preview should now be accurate when rapidly selecting new sabers
- Potential fix for sabers not showing up when clicking refresh

## 0.10.0
- Added support for Qosmetics whacker files
- Sabers with more than one trail now show additional trails
- Custom trails now use their designated length when not overriding trail length
- Sabers are loaded in the background on startup, reducing load times considerably
- Fixed various errors pertaining to the saber preview
- Minor optimizations

## 0.9.2
- The game no longer freezes when loading the saber preview
- Various cleanup

## 0.9.1
- Fixed saber color scheme settings not updating in their respective settings menus

## 0.9.0
- Added a custom saber preview to the main menu
- Decreased load times by not loading the saber at the start of a map
- Fixed trail width setting linking to trail duration toggle
- Fixed trail width setting using local scale instead of world scale

## 0.8.3
- Added a setting to adjust the width of custom trails

## 0.8.2
- Fixed sabers not loading if any sabers were in subfolders of the CustomSabers folder

## 0.8.1
- Added a toggle to disable the mod during gameplay
- Fixed saber list in gameplay setup not refreshing
- Fixed saber list not scrolling to the selected saber correctly

## 0.8.0
- Added the ability to change saber colors separately from the selected color scheme
- Fixed default trail settings when using default trails with custom sabers
- Small UI tweaks

## 0.7.6
- Fixed custom sabers not loading when switching from default sabers to another saber

## 0.7.5
- Fix for BSML 1.9.2

## 0.7.4
- Fixed some compatibility issues when changing settings for default sabers

## 0.7.3
- Fixed note cut saber event firing for both sabers no matter which note was cut
- Tweaks and optimizations

## 0.7.2
- Re-enabled the mod in multiplayer

## 0.7.1
- Updated for beat saber v1.35.0

## 0.7.0
- Renamed the assembly to CustomSabersLite from CustomSaber
- Removed BS Utils dependency and added SiraUtil dependency
- Added Chroma coloured note compatibility; sabers will now change colour on maps that use Chroma's custom note colours
- Added compatibility for sabers which use dynamic bones
- Full rewrite using SiraUtil and Zenject
- Fixed missing data for Default Saber
- Fixed custom sabers being in the wrong hands

## 0.6.0
- Added a tab in the gameplay options menu (left of song list) so you can more conveniently swap and adjust sabers 

## 0.5.1
- Fixed missed case for trail custom colours where material colour would be set to the vertex colours

## 0.5.0
- Now handles custom events for sabers which have event managers
- Fixed trails that used their own custom colours

## 0.4.4
- Fixed a major gameplay breaking bug where custom trails would interfere with the hit-box of the sabers

## 0.4.3
- Cached metadata is now automatically replaced when the mod is updated to make sure the new flags are included
- Fixed an issue where the loader would get stuck when the saber name contained invalid file name characters
- Flag for sabers if their name contains invalid file name characters
- Fixed some cases where shader repair would happen while default sabers were selected

## 0.4.2
- Fixed shader repair not running on selected saber from startup whereas it only would run when the selected saber was changed
- Flag for sabers if they failed shader repair

## 0.4.1
- Patch to disable the script in multiplayer so that the default sabers aren't removed
note: multiplayer support is planned but is not of high priority

## 0.4.0
- Now caches data for sabers after the first time they're loaded, to reduce subsequent loading times
- Added a button to open the CustomSabers folder
- Added a button to delete sabers

## 0.3.7
- Most sabers, and their trails, should now be coloured correctly
- Fix for most saber trails not using Vertex Colors now have the correct colour 
- Fix for sabers with shared materials not applying the right colour to both sabers
- UI adjustments (WIP)
- Basic asynchronous loading

## 0.3.6
- Trail duration slider setting is now only interactable when Override Trail Duration is toggled on to make the link between the two settings immediately obvious

## 0.3.5
- Fix for trail duration setting now correctly applies to custom trails at all values

## 0.3.4
- Now uses correct trail width set by saber author (most sabers should now look relatively accurate)
- Fix for trail intensity which now controls the transparency of custom trails as well as default trails
- SaberScript cleanup

## 0.3.3
- Switch to AssetBundleLoadingTools to fix shaders compiled for the old unity version

## 0.3.2
- Fix for settings now apply to custom and default trails correctly
- Trail type setting now works as expected when using default sabers
- Fix for disabling trails now makes trails completely invisible
- Fixed an exception when trying to use custom trails while playing with the default sabers

## 0.3.1
- Settings UI is now more user friendly
- Trail duration slider is now a percentage rather than absolute value

## 0.3.0
- Now loads custom trails with basic settings (doesn't work well with most trails)

## 0.2.2
- Added setting to change trail type (Vanilla, Custom, No trail)
- Setup for custom trails
- Code cleanup

## 0.2.1
- Added settings UI which allows changing trail duration and toggling white trail gradient
- Setttings currently only apply to default trails

## 0.2.0
- Added UI list to select sabers
- Custom sabers now use player's colours
- Custom colours set in maps may not work

## 0.1.0
- Ability to replace default saber model with a custom saber model
- Basic asset bundle loading and reading from config file
