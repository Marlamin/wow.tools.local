# WoW.tools but local
This contains a very slimmed down version of the WoW.tools site (currently modelviewer for M2s only and DBC browsing/exporting) meant for local use without relying on anything from the main WoW.tools website (or the internet in general, after external dependencies have been downloaded, see relevant section at the bottom of this README).

## Requirements
All dependencies should be included with the executable. If you have the included requirements already installed, feel free to compile it yourself for a smaller/cleaner runtime directory. As this loads quite a bit of data, you'll need at least 4-5GB of free RAM.

## Known issues
- Some BLPs (e.g. interface BLPs) won't preview correctly/be empty. Download these from the files page instead and open them externally in a BLP viewer (e.g. XnView)/converter (e.g BLP2PNG).

## Download 
The latest version can be downloaded [here](https://github.com/Marlamin/wow.tools.local/releases).

## Configuration
1. Open `config.json` in your favorite text editor.
2. Set `wowFolder` to the directory your WoW is installed to using either \\ or / slashes. Using \ will break loading. Do NOT point it to a subdirectory like `_retail_` or anything like that, just the main "World of Warcraft" folder. You can also leave this empty, but this will stream all required files from the internet, which will be rather slow.
3. Set `wowProduct` to the product you wish to load. For example, Mainline Retail WoW (the default) would be `wow`, Mainline PTR `wowt`, Mainline Beta `wow_beta`, Classic Retail would be `wow_classic`, etc. You can view a full list of product [here](https://wowdev.wiki/TACT#Products). If you have multiple products installed, you can also switch between them on the "Builds" page after starting, but keep in mind the product set here is always loaded first.  
4. _**(Optional)**_ If you want to have DBCs/DB2s from other versions be available to the table browser/diffs, you can set `dbcFolder` to the path where these files can be found. The contents of this directory **MUST** be structured as follows: `<folder set in dbcFolder>/<x.y.z.build>/dbfilesclient/`, e.g. achievement.dbc for 3.2.0.10192 needs to be in this location: `<folder set in dbcFolder>/3.2.0.10192/dbfilesclient/achievement.dbc`. Depending on operating system, the DBC/DB2 names might need to be lowercase.
5. _**(Optional)**_ If you left wowFolder empty (online mode), you can set region to the region you are in (e.g. us, eu, tw, cn)
6. _**(Optional)**_ If you want to load a specific locale (e.g. deDE, zhCN, etc), set `locale` to the locale you want to load. Supported locales are listed [here](https://github.com/WoW-Tools/CascLib/blob/342211a799d6249ced1652ed285319a2aebaae38/CascLib/RootHandlers/WowRootHandler.cs#L14-L29).

## Running
Make sure WoW is updated/closed and Battle.net is idle/closed before starting it and make sure to close it before starting WoW again to make sure you won't run into issues with files being locked and such.

Start the application by opening wow.tools.local.exe (or whatever executable is relevant for your OS). Startup can take some time depending on which product you are loading. Classic will be relatively fast, but Mainline builds will take a bit longer due to the amount of files it has.  

After startup is complete, it should show something along the lines of `Now listening on: http://localhost:5000`, go to whatever URL is mentioned there (including the port) in your favorite web browser to go to the site. The rest should work similarly to how wow.tools does/did.

## Build diffs
Very rudimentary build diffing is available through the builds page. Only builds that have been previously loaded (or are currently loaded) can be diffed from/to. No file comparisons are available yet (with the exception of DB2 diffs, which are available, see below paragraph for notes) and previews for added files on the diff page will show previews of the files in the currently loaded build, so make sure the build you want to diff TO is the currently active build. New files will not be available for preview until "Analyze unknown files" on the files page has been done (first time will take a while, progress can be checked in the console window). Files that remain unknown either have a unpreviewable/unknown type, are empty or are encrypted.

## DB2 diffs
DB2 diffs are available between DB2s that are either extracted on disk (see step 4 of configuration) or from the currently loaded build. Extracting DB2s for the current build to the directory mentioned in step 4 is available through a button on the builds page, which will also tell you if DB2s for a certain build are missing. Make sure to do this if you want to be able to diff DB2s with this build in the future.

## External dependencies
### Definitions
wow.tools.local relies on updated database definitions from the [WoWDBDefs](https://github.com/wowdev/WoWDBDefs) repo. While definitions are included in releases, these are likely to go out of date quickly leading to errors such as "No definition found for this file" while attempting to load DB2s with recently changed structures. Updating definitions can be done by pressing the "Update WoWDBDefs" button on the DBC browsing page. If you instead want to do this manually, go [here](https://github.com/wowdev/WoWDBDefs) and download the ZIP by click Code -> Download ZIP, then overwrite the `WoWDBDefs/definitions` folder with the `definitions` folder from the ZIP.

### Listfile
To have named files throughout the wow.tools.local, it downloads [this listfile](https://github.com/wowdev/wow-listfile/blob/master/community-listfile.csv) on first start but this can go out of date in time as new files are added/named in future content, to prevent this the listfile is automatically updated if it has not been updated for over a day. If you want to force an update, click "Update listfile" on the files page. If you want to manually do this, remove `listfile.csv` to force a redownload on next startup or update it manually by placing your own listfile there. The listfile URL can also be changed in the config.

### TACT keys
wow.tools.local relies on updated TACT keys from the [TACTKeys](https://github.com/wowdev/TACTKeys) repo to be able to load encrypted files. This can go out of date in time as new things are encrypted and new keys become available, to prevent this the list of keys is automatically updated if it has not been updated for over a day. If you want to force an update, click "Update TACTKeys" on the files page.

## Maintaining/Credits
Outside of some glue and all the parts pretty much taken directly from WoW.tools projects, this project uses a few other projects to do the heavy lifting in the hopes that maintaining it in the future (if there need to be changes) should be relatively simple*, if the below projects are still being maintained. Many thanks to their authors!

**(compared to maintaining the main wow.tools site)*

- [Deamon's WebWowViewerCpp](https://github.com/Deamon87/WebWowViewerCpp) (Emscripten version) which also powers the WoW.tools modelviewer. Does the actual model rendering.
- [DBCD](https://github.com/wowdev/DBCD) used to read WoW's DB2 tables.
- [CascLib](https://github.com/WoW-Tools/CascLib) used to read information from WoW's game files.
- [WoWDBDefs](https://github.com/wowdev/WoWDBDefs) definitions required to read WoW's DB2 tables.
