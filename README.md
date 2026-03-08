# WoW.tools but local (WTL)
This contains a very slimmed down version of the WoW.tools site meant for local use without relying on anything from the main WoW.tools website (or the internet in general, after external dependencies have been downloaded, see relevant section at the bottom of this README).

## Requirements
All dependencies should be included with the executable. If you have the included requirements already installed, feel free to compile it yourself for a smaller/cleaner runtime directory. As this loads quite a bit of data, you'll need at least ~6-8GB of free RAM. Please note that some features (e.g. DB2 global search) will take up far more RAM than that.

## Download 
The latest version can be downloaded [here](https://github.com/Marlamin/wow.tools.local/releases).

If you want to use the File History and File Links features, be sure to download WTL.db from the latest GitHub release that has it to the directory with the `wow.tools.local.exe` executable before starting WTL for the first time.

## Running
Make sure WoW is updated/closed and Battle.net is idle/closed before starting it and make sure to close it before starting WoW again to make sure you won't run into issues with files being locked and such.

Start the application by opening `wow.tools.local.exe` (or whatever executable is relevant for your OS). Startup can take some time depending on which product you are loading. Classic will be relatively fast, but Mainline builds will take a bit longer due to the amount of files it has. 

After startup is complete, it should show something along the lines of `Now listening on: http://localhost:5000`, go to whatever URL is mentioned there (including the port) in your favorite web browser to go to the site. The rest should work similarly to how wow.tools does/did.

If you want to modify which WoW folder it uses, which product it loads, or any of the other settings, be sure to check out the Settings page available through the button on the top right.

#### Command Line
Optionally, you may also start WTL from the command line, providing arguments to override options found in your `config.json` file. Any arguments that are not provided will be loaded from the `config.json` file either in the current working directory, or the executable directory as a fallback.
- `-wowFolder` - Path to your WoW installation
- `-wowProduct` - Product to load
- `-dbcFolder` - Path to your DBCs folder
- `-definitionDir` - Path to your WoWDBDefs folder
- `-listfileURL` - URL to fetch the Listfile
- `-tactKeyURL` - URL to fetch TactKeys
- `-region` - Region
- `-locale` - Locale
- ..for a full list of command line arguments, see [the Settings dictionary in SettingsManager.cs](https://github.com/Marlamin/wow.tools.local/blob/main/SettingsManager.cs#L19).

## WTL.db
WTL.db is a SQLite database that contains data for both the "File links" and "File history" features.
Getting one from the latest release that has it gives you a lot of data, but if you want to generate your own from your own cached WTL data on disk go to the following URLs (in order):
- Start WTL at least once
- Shut down WTL
- Delete WTL.db in wow.tools.local folder (it should exist after the first start)
- Start it again, then go to these URLs in order (wait until each is done, the console window has progress)
- /casc/generateFileHistory (skip if you only want file links)
- /casc/importAllFileHistory (skip if you only want file links)
- /casc/startLinking (skip if you only want file history)

Keep in mind that it uses cached manifests on disk to generate the file history. If you only have e.g. Midnight builds, file history would only show history going as far back as Midnight, with all files being added in the first Midnight build you loaded. 

## hotfixes.db
hotfixes.db is a SQLite database that contains specifically just data related to hotfixes. WTL will automatically generate it and fill it with available hotfix data. Every now and then I will also attached an uptodate hotfixes.db to the latest GitHub release in case you want a longer history.

## Build diffs
Build diffing is available through the builds/homepage. Only builds that have been previously loaded (or are currently loaded) can be diffed from/to. Basic file comparisons are available. Previews for added files on the diff page will show previews of the files in the currently loaded build, so make sure the build you want to diff TO is the currently active build. New files will not be available for preview until "Analyze unknown files" on the files page has been done (first time will take a while, progress can be checked in the console window). Files that remain unknown either have a unpreviewable/unknown type, are empty or are encrypted.

## DB2 diffs
DB2 diffs are available between DB2s that are either extracted on disk or from the currently loaded build. Extracting DB2s for the current build is available through a button on the builds page, which will also tell you if DB2s for a certain build are missing. Make sure to do this if you want to be able to diff DB2s with this build in the future.

## Hotfixes
Hotfixes are loaded from DBCache files that exist in the WoW directory (for all products) or manually placed `*.bin` files in the `caches` directory in the wow.tools.local directory. The latest DBCaches can also be downloaded from [Raidbots](https://www.raidbots.com/developers) through buttons on the hotfixes page. Keep in mind the "First detected" column on the hotfixes page will list times WTL first detected the hotfixes, not when the hotfixes were pushed to realms. After updating the DBCache files (either manually or by logging into/then quitting the game), a restart of WTL is currently required. There is a clear cache button, but restarting makes sure WTL properly reloads hotfixes fully.

## File links
File links (e.g. textures in M2 files, M2 doodads in WMO files, etc) are parsed (and then saved) when the "More Information" screen is loaded and will be available for that file from that point on. For a pre-compiled database with most file links (for retail WoW) already present, download "WTL.db" from the latest release that has one and replace the file on disk.

### Updating file links 
To update all file links, manually browse to `/casc/startLinking` and keep an eye on the console window for progress. After the page loads, it is done. 

## File history
File history for a build is only processed and saved when a build is loaded for the first time. For a pre-compiled database with a mostly complete history all the way back to 6.0.1, download "WTL.db" from the latest release that has one and replace the file on disk. When new builds are loaded for the first time (and the manifest is first generated) it should import file history for that build.

## External dependencies
### Definitions
WTL relies on updated database definitions from the [WoWDBDefs](https://github.com/wowdev/WoWDBDefs) repo. Definitions are downloaded during the first start of WTL and cached for 1 day. Updating definitions more often than every day can be done by pressing the "Update WoWDBDefs" button on the DBC browsing page. If you instead want to do this manually and supply definitions from disk, go [here](https://github.com/wowdev/WoWDBDefs) and download the ZIP by click Code -> Download ZIP, extract the ZIP somewhere and set `definitionDir` in config to the path to the `definitions` directory.

### Listfile
To have named files throughout the WTL, it downloads [the latest community-listfile.csv listfile](https://github.com/wowdev/wow-listfile/releases) on first start but this can go out of date in time as new files are added/named in future content, to prevent this the listfile is automatically updated if it has not been updated for over a day. If you want to force an update, click "Update listfile" on the files page. If you want to manually do this, remove `listfile.csv` to force a redownload on next startup or update it manually by placing your own listfile there. The listfile URL can also be changed in the config.

### TACT keys
WTL relies on updated TACT keys from the [TACTKeys](https://github.com/wowdev/TACTKeys) repo to be able to load encrypted files. This can go out of date in time as new things are encrypted and new keys become available, to prevent this the list of keys is automatically updated if it has not been updated for over a day. If you want to force an update, click "Update TACTKeys" on the files page. If you believe you have keys available in your local WoW's DBCache.bin that are not yet in the repository, make sure to go to the TACTKeys page and check the WTL console if it detected any new keys.

### Files from older builds
For downloading files from builds that are not currently loaded, WTL relies on downloads from [wago.tools](https://wago.tools).

## Maintaining/Credits
Outside of some glue and all the parts pretty much taken directly from WoW.tools projects, this project uses a few other projects to do the heavy lifting in the hopes that maintaining it in the future (if there need to be changes) should be relatively simple*, if the below projects are still being maintained. Many thanks to their authors!

**(compared to maintaining the main wow.tools site)*

- [BLPSharp](https://github.com/wowdev/BLPSharp) used to read convert WoW's BLP textures to PNGs.
- [CascLib](https://github.com/WoW-Tools/CascLib) or [TACTSharp](https://github.com/wowdev/TACTSharp) are used to retrieve files from WoW's file storage.
- [DBCD](https://github.com/wowdev/DBCD) used to read WoW's DB2 tables.
- [Deamon's WebWowViewerCpp](https://github.com/Deamon87/WebWowViewerCpp) (Emscripten version) which also powers the WoW.tools modelviewer. Does the actual model rendering.
- [WoWDBDefs](https://github.com/wowdev/WoWDBDefs) definitions required to read WoW's DB2 tables.
- [WoWFormatLib](https://github.com/Marlamin/WoWFormatLib) used to parse WoW files with WoW-specific formats.
- [wow.export](https://github.com/Kruithne/wow.export/) for expansion icons.
