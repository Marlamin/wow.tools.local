# WoW.tools but local
This contains a very slimmed down version of the WoW.tools site (currently modelviewer for M2s only and partial DBC browsing features) meant for local use.

## Requirements
All dependencies should be included with the executable. If you have the included requirements already installed, feel free to compile it yourself for a smaller/cleaner runtime directory.

### Definitions
While definitions are included in releases, these are likely to go out of date quickly. Hopefully up-to-date definitions can be found on [here](https://github.com/wowdev/WoWDBDefs) (to download, click Code -> Download ZIP), to update definitions overwrite the `WoWDBDefs/definitions` folder with the `definitions` folder from the downloaded version of the WoWDBDefs repo.

### Listfile
The application downloads [this listfile](https://github.com/wowdev/wow-listfile/blob/master/community-listfile.csv) on first start, but this can go out of date in time as new files are added/named in future content. Simply remove `listfile.csv` to force a redownload on next startup or update it manually. The listfile URL can also be changed in the config.

## Download 
The latest version can be downloaded [here](https://github.com/Marlamin/wow.tools.local/releases).

## Configuration
1. Open `config.json` in your favorite text editor.
2. Set `wowFolder` to the directory your WoW is installed to. Do NOT point it to a subdirectory like `_retail_` or anything like that, just the main "World of Warcraft" folder. You can also leave this empty, but this will stream all required files from the internet, which will be rather slow.
2. Set `wowProduct` to the product you wish to load. For example, Mainline Retail WoW (the default) would be `wow`, Mainline PTR `wowt`, Mainline Beta `wow_beta`, Classic Retail would be `wow_classic`, etc. You can view a full list of product [here](https://wowdev.wiki/TACT#Products).

## Running
Start the application by opening wow.tools.local.exe (or whatever executable is relevant for your OS). Startup can take some time depending on which product you are loading. Classic will be relatively fast, but Mainline Retail will take a bit longer due to the amount of files it has.  

After startup is complete, it should show something along the lines of `Now listening on: http://localhost:5000`, go to whatever URL is mentioned there (including the port) in your favorite web browser to go to the site. The rest should work similarly to how wow.tools does/did.

## Maintaining/Credits
Outside of all the glue pretty much taken directly from WoW.tools projects, this project uses a few other projects to do the heavy lifting in the hopes that maintaining it in the future (if there need to be changes) should be relatively simple*, if the below projects are still being maintained. Many thanks to their authors!

**(compared to maintaining the main wow.tools site)*

- [Deamon's WebWowViewerCpp](https://github.com/Deamon87/WebWowViewerCpp) (Emscripten version) which also powers the WoW.tools modelviewer. Does the actual model rendering.
- [DBCD](https://github.com/wowdev/DBCD) used to read WoW's DB2 tables.
- [CascLib](https://github.com/WoW-Tools/CascLib) used to read information from WoW's game files.
- [WoWDBDefs](https://github.com/wowdev/WoWDBDefs) definitions required to read WoW's DB2 tables.
