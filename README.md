# Rogue Legacy Level Editor

This is the editor used to make the level files for Rogue Legacy 1.

The source code to the game can be found
[here](https://github.com/flibitijibibo/RogueLegacy1).

## License

Rogue Legacy's source code is released under a specialized, non-commercial-use
license. See
[LICENSE.md](https://github.com/flibitijibibo/RogueCastleEditor/blob/main/LICENSE.md)
for details.

## About This Repository

This repo is mostly a historical archive, as it is not compatible with the FNA
build of RL1. There are a couple reasons for this:

- The editor uses WPF, which makes use of the XNA WinForms HWND from the XNA
  version of the game. While it is possible to extract the HWND from an SDL
  window, this wouldn't fix multiplatform compatibility issues.
- The editor itself depends on XNA and the XNA content pipeline, so an updated
  build would need to replace the SpriteFont file and link to FNA instead.

A restored version of this editor would replace WPF with a multiplatform GUI
toolkit and remove the XNA linker references with FNA. There may be other
portability issues as well, this has not been thoroughly tested!

## Notes from the Developers

For it to run properly, in the editor under "Project" in the file menu, the
EditorSpritesheet directory needs to be set to wherever the
EditorSpritesheets.zip file is extracted to, and the Executable Directory needs
to be set to wherever the game's executable is.

And lastly, the EnemyList.xml file (also found in EditorSpritesheets.zip) needs
to be placed in the same folder as the executable.

To test levels directly in the editor press F5, but it will only work if the
executable directory points to a game executable built with the following EV file
settings:

```
LevelEV.RUN_TESTROOM = true;
LevelEV.CREATE_RETAIL_VERSION = false;
```
