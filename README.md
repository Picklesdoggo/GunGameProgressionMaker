# GunGameProgressionMaker
A GUI Application for creating progressions for Kodeman's GunGame. To begin you will need to update `Config.json`to define where you want to save progressions and place the following files alongside the `GunGameProgressionMaker.exe`. :

1. `classdata.tpk`
2. `config.json`
3. `gameData.json`


Typical save location, note the double slash:
C:\\\Users\\\XXXX\\\AppData\\\Roaming\\\Thunderstore Mod Manager\\\DataFolder\\\H3VR\\profiles\\\XXXX\\\BepInEx\\\plugins\\\Kodeman-GunGame

By default game assets are located, if this is not where your game is installed edit `config.json` `GameResourcesPath` :
C:\\Program Files (x86)\\Steam\\steamapps\\common\\H3VR\\h3vr_Data\\resources.assets

By default game managed path is located, if this is not where your game is installed edit `config.json` `GameManagedPath` :

To have the application detect modded weapons edit `config.json` `ModsDirectory` to define the path to where modes are stored typically location, again note the double slash:
C:\\Users\\XXXX\\AppData\\Roaming\\Thunderstore Mod Manager\\DataFolder\\H3VR\\profiles\\john\\BepInEx\\plugins

Mods are currently detected by searching through provided mod path looking for files with late_ in the name, if you have a mod that does not contain a late file you may add it to `config.json` by editing `ManuallyLoadedMods` you will again need the double slash

![image](https://user-images.githubusercontent.com/114353253/230809663-47e4a693-d52c-42ba-b80f-6fd7005df8da.png)

