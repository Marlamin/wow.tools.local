0.9.3 (31-03-2026)
- Added support for flag/enum definitions from WoWDBDefs, removed built-in flags/enums (thanks @MaxtorCoder).
  Note: This may have caused some issues with DB2 reading that I missed, please let me know if you encounter any.
- Reworked file preview modal UI to be more consistent between file types.
- Added parsed tooltips back to spells, items and BLP file previews (very experimental, many tooltips will be incomplete).
- Added hex dumps in file previews for all files.
- Added JSON previews for WDT and WDL files.
- Added JSON diffing support for non-root ADT files.
- Added persistent settings to table page lengths between sessions for DB2 browsing, DB2 diffs and file browsing.
- Added retrieval of known FileDataIDs from the local database when ShowAllFiles is enabled.
- Added mass placeholder file naming mode to naming page for quicker naming of related files.
- Added `parentof:`/`childof:` files page search filters (only works when file linking has ran at least once).
- Added settings for Battle.net API credentials, only used for file naming.
- Added retrying of file download when hitting the "Invalid BLTE header" exception.
- Added file tooltips to FileDataIDs in more info/contenthash modals.
- Added support for adding/editing PresetSplit file tags.
- Fixed file previews being available for unavailable versions when ShowAllFiles is enabled.
- Fixed dark mode/light mode styling for choicesjs dropdowns.
- Fixed parented files not showing up under non-M2/WMO files in the more info modal.
- Fixed flag tooltips constantly refreshing when moving the mouse.
- Fixed previews breaking on unnamed ADT files.
- Fixed previews being broken in diffs.
- Fixed switcher button in BLP diffs not working.
- Fixed namer not picking up on obvious names for player housing models.
- Improved diffing speed/listfile caching a little.
- Temporarily disabled `skit:`/`skitid:` search filter due to still unresolved issues with it.
- Removed Ribbit v1 support in CascLib and dropped Ribbit.NET (removes a few dependencies/build output files).
- Removed unused dependencies.
- More JQuery removal and JS cleanup.
- Updated WoWFormatLib for improved WDT/WDL parsing/dependency cleanup.
- Updated TACT Key metadata.

0.9.2 (25-02-2026)
- Finished up v1 of the Maps feature, it is now included in the menu.
- Improved speed of file history inserting for new builds.
- Started work on getting rid of JQuery, cleaned up/reworked some ancient JS and dropped Select2 for Choices.
- Updated naming to output better player housing names.
- Updated file extension for formerly-known-as 'wwf' files to the now confirmed 'pvdata'.
- Updated hotfix loading logic for the namer to be a bit more robust.
- Updated unknown lookup exports to hopefully contain more/all unknown lookups.
- Fixed crash when loading too many DB2s too close to the first load of any DB2.
- Fixed skit:/skitid: files page search filter not working.
- Fixed tooltips not picking up on whether hotfixes were enabled for a table or not.
- Updated TACTSharp/CASCLib/WoWNamingLib/wow-filetags submodules.

0.9.1 (31-01-2026)
- Added support for wow-filetags
- Added automatic downloading of known lookups
- Added downloading of hotfix caches from Raidbots
- Added initial version of read-only mode for running WTL in hosted environments
- Added 12AL filename prefix for Army of Light
- Fixed screenshot combo texture field labels #69 (by @WainPetopia)
- Fixed localized file loading #73 (by @madcowfred)
- Fixed issue where additional TACTSharp build loads (e.g. for diffs) weren't using local WoW dirs (if set)
- Updated flags with several new flags around secrets (by @Ghostopheles)
- Updated TACTSharp (fixes issue with configs downloading even with local WoW dir set)
- Updated WoWFormatLib (fixes cache reading on newer versions)

0.9.0 (29-11-2025)
Major version bump due to the bump to .NET 10!

- Added new binary manifest format to save on disk space/parsing speed.
- Added better page titles, DB2 pages will also now have the name of the DB2 in the title.
- Fixed file variant selection sometimes preferring low violence files.
- Updated to .NET 10.
- Updated modelviewer texture inputs to be 2 rows so it fits on normal screens.
- Updated files page preview pane-mode to show last selected file/preview on click.
- Updated file naming to generate more player housing related names. More to come.
- Updated Quest/Creature WDB structures up to 11.2.7.64587/12.0.0.64611.
- Updated TACT Key metadata.
- Updated Bootstrap from 5.3.7 to 5.3.8.
- Updated DataTables from 2.3.2 to 2.3.5.
- Updated CASCLib/TACTSharp/DBCD/DBDiffer/WoWNamingLib/WoWFormatLib to .NET 10.

0.8.5 (30-10-2025)
More fixes/additions for Midnight as well as some other stuff in this update!
- Added ! operator for inverse file page searches (by @bloerwald).
- Added cache for listfile searches speeding up various file-browsing related bits (by @bloerwald).
- Added useHotfixes checkbox back to DBC diffs.
- Added player housing naming mode to naming page.
- Updated TACTSharp to pick up on local archive bypass upon reading issues (e.g. when WoW is open).
- Updated WoWFormatLib to pick up on questcache.wdb parsing fixes.
- Updated some enums for Midnight.
- Updated/added some model naming prefixes.
- Updated some model naming page styling/behavior.
- Fixed crash when opening the more details window on newer WoW builds.

0.8.4 (03-10-2025)
Initial fixes/additions for Midnight. More to follow in another update soon, probably.
- Added race prefixes for Midnight model naming.
- Added various housing-related flags/enums (by @Luzifix)
- Updated TACTSharp/CASCLib to pick up on encrypted file reading fix.
- Updated executable icon.

0.8.3 (11-09-2025)
Mostly fixes this version, wanted to push this later on but V1 Ribbit being down for a few days now forced my hand a little.

- Added buildConfigFile and cdnConfigFile command line arguments (thanks @Ghostopheles).
- Added ADT JSON dumps (root ADTs only, doing this on other ADT types will cause issues).
- Updated new build logic to force-update DBDs upon detecting a new build.
- Updated some enums/flags for 11.2.5.
- Fixed reverse foreign key links for ID columns (thanks for reporting @WainPetopia).
- Fixed remote builds no longer working due to Ribbit V1 patch service having issues by switching to Ribbit V2.
- Fixed DB2 exports not working properly when no locale was set.
- Fixed DB2 color conversion (Thanks @Dorovon).
- Fixed some DB2 diffs not working.
- Fixed crash when trying to insert 2 different builds with a fakeBuildConfig.
- Updated TACTSharp to deal with Ribbit V1 issue.

0.8.2 (15-08-2025)
The Patreon is back! Read all about it [here](https://www.patreon.com/posts/hello-and-again-136127185) (Patreon) or [here](https://blog.marlam.in/patreon-v2) (blog, same post but mirrored).

0.8.2 reworks the settings system and adds a hopefully working settings page so you won't have to mess with `config.json` anymore. It also fixes some critical issues with certain DB2s not opening/extracting/etc correctly in 0.8.1 as well as a few other things. There might be some rough edges here and there but I deemed it important enough to ship for the DB2 fix. There might be a 0.8.3 soon if there's more issues that show up.

> If there are any new issues in this, open an issue on GitHub or reach out to me on BlueSky, by e-mail to marlamin@marlamin.com or via Discord or Patreon.

- Added settings page with validation and reworked settings system so users hopefully won't have to mess with `config.json` anymore.
- Added locale selection back to DB2s (experimental).
- Added automatic location of the WoW folder on Windows for new installations.
- Added local overriding of CDN config similar to how overriding for build config works (advanced users only).
- Fix DB2 file selection logic after an issue appeared in the 0.8.1. Should fix a bunch of DB2 crashes/extraction failures/etc.
- Fix crash when converting empty DB2s to CSV.
- Fix wrong help description for `added:` search option on files page.
- Updated GitHub update checking state to be saved in browser localstorage instead of a cookie.
- Updated WoWNamingLib with some additional contenthash based names.
- Updated TACT key metadata for 11.2.5.

0.8.1 (06-08-2025)
As some might know I've been going through a pretty bad time over the last month or so due to the passing of my mom after a short battle with bowel cancer, but I'm slowly getting back into working on stuff again and thought it might be smart to push out a new release with everything since 0.8.0 before I start working on new stuff, so here we are.
I've also been considering reactivating the Patreon to cover the small (but constant) costs of working on WoW things for the community and maybe expand some things I've had to cut down on if money permits. If that were to happen nothing will change in regards to WTL's availability nor am I planning on gating any functionality or whatever behind a Patreon, it would largely just be donations with maybe at best some influence on what I prioritize on my TODO list. But more updates on that to follow on my BlueSky.
If there are any new issues with this release, open an issue on this repo or reach out to me on BlueSky, by e-mail to marlamin@marlamin.com or via Discord.

- Added listing of multiple content hashes when available (e.g. for localized files)
- Added decal file namer.
- Updated DB2 flag tooltips to newer tooltip style.
- Updated DB2 diffs to have similar display functionality as normal DB2 pages (e.g. flags, foreign keys, tooltips, etc).
- Updated DB2 diffs to be considerably faster.
- Updated DB2 foreign keys to SoundEntries to point to SoundKit instead on 6.0+.
- Updated contenthash-based file namer.
- Updated DBD downloading to ensure that the cache directory exists when downloading for the first time by @Ovahlord in #51
- Updated WoWNamingLib for various naming-related updates.
- Updated TACT key metadata.
- (WIP) Updated TACT keys page with "More information" links showing various encryption-related data.
- Fixed DB2 cache not being cleared properly.
- Fixed linebreaks on WTL-suggestions downloads on Firefox-based browsers by @Ahava in #49
- Fixed DB2 case sensitivity issues for Linux, allowing dbc/db2 files to load regardless of casing by @Ovahlord in #52

0.8.0 (27-06-2025)
Major version bump as this version contains the start of the hotfix rework. Hotfix metadata is now tracked in a separate database. Due to this, hotfix metadata is now available while other products are loaded as well. Features like better hotfix diffs as well as searching/filtering on the hotfixes page are planned for the future. If you are having issues with hotfixes specifically, please stay on the previous WTL release and let me know via BlueSky, Discord (if you have me added/share a server) or via e-mail to marlamin@marlamin.com.

> [!IMPORTANT]
> If you want to backfill WTL with old hotfix metadata, make sure to download hotfixes.zip below and extract and drop hotfixes.db in your WTL dir. I've also included an updated WTL.db for new users, existing users shouldn't need to update (doing so could overwrite local VO/creature information).

- Added partial support for M3 models. Supports basic geometry previews through a separate model viewer as well as JSON dumps.
- Added search to the creatures page.
- Added skit:id search option to the files page to allow filtering files by their SoundKitID.
- Added chash:hash search option to the files page to allow filtering files by their MD5 contenthash.
- Added in:x.x.x.xxxxx search option to the files page to filter files by existing in a certain build
- Added neighboring files to more info file modal.
  Keep in mind that if you have showAllFiles set to false in config, the files on the files page are already filtered by the current build.
  Using the search option in that case would filter files twice, to always show all files in the listfile set showAllFiles to true.
- Improved speed of listfile loading on startup.
- Improved speed of encrypted file analysis on startup.
- Improved speed of contenthash loading when opening the more info file modal.
- Improved speed of various database queries.
- Decreased memory usage after initial startup.
- Fix a few concurrency issues, there are a few still left but hopefully it should behave a bit better and not load things multiple times/crash.
- Updated how WTL tracks builds. On first launch after updating, WTL will backlog build information from wago.tools.
- Updated how WTL parses and handles hotfix data.
- Updated WoWNamingLib, hopefully less error prone now.
- Updated TACTSharp, fixes loading older CDNConfigs that didn't have file-index yet.
- Updated CASCLib.
- Updated Bootstrap from 5.3.5 to 5.3.7.
- Updated DataTables from 2.1.5 to 2.3.2.

0.7.4 (20-06-2025)
> [!IMPORTANT]
> If you're having trouble with the config loading/startup after this, overwrite your current config with the included one and set it up again.
> If issues remain, poke me on Discord/BlueSky and we can investigate.

> [!NOTE]
> A rewrite of hotfix handling is coming up, I don't think I've shipped anything that's still WIP, but it's possible hotfixes could misbehave.

- Added additional CDNs setting to specify backup CDNs (comma separated) for TACTSharp to use if a file can not be found on Blizz CDNs.
- Added support for detecting XML/TOC/HLSL files.
- Added 11.2 naming presets for Ethereals/Brokers.
- Optimized BLP/raw hex diffs.
- Updated TACT key metadata.
- Updated WoWFormatLib for 11.2 WDB changes.
- Updated WoWNamingLib with various speed optimizations/fixes.
- Updated TACTSharp.
- Various cleanup/under the hood work that might cause random issues, let me know if any new issues arise.

0.7.3 (24-05-2025)
- Added raw hex diffs to diff page for all files (with somewhat sane limits against diffing files that are too large).
- Added cdnFolder setting to config.json for specifying a local CDN directory/cache if you have one.
- Added currently loaded build to the "remote builds" table on the builds page when loading custom configs.
- TACTSharp is now on by default in the config, please let me know if there's any issues.
- Fixed issue with install page not clearing files between build loads.
- Fixed crash on naming page if there were no more models left to name.
- Updated diffing UI with various improvements.
- Updated TACTSharp (fixes various issues and makes it more fault tolerant/less likely to download files when specifying a local WoW dir).
- Updated WoWNamingLib (now also runs map naming on maps only in listfile and not in Map.db2).

0.7.2 (02-05-2025)
- Add checkbox to include filename suggestions that change casing.
- Ignore filename suggestions for files with lookups (and the suggestion doesn't match).
- Rewrite TACTKey loading/updates, fixes loading TACTKeys when using TACTSharp.
- Official filename lookups loading is now quicker and uses lookup.csv now.
- Improve type detection for M3s/cinematics/XML files.
- Fix some M2 crashing during naming.
- Switch to BLPSharp for BLP loading.
- Update TACTSharp.

0.7.1 (23-04-2025)
> [!IMPORTANT]
> There is a known issue with TACTKeys not updating properly when using TACTSharp. This is fixed in the next version slated for release after the first 11.1.7 PTR build. For now you can manually update TACT keys by overwriting WoW.txt in the WTL dir with the latest one from here.

- Fixed name suggestions not being validated on Classic installations.
- Make table names in hotfixes use their proper uppercase versions.
- (TACTSharp only) Skip local install loading when loading custom configs.
- (TACTSharp only) Load local indices early so build-related files aren't always downloaded.
- (TACTSharp only) Fix encryption keys not being tied to files anymore.

0.7.0 (09-04-2024)
The switch from the included copy of WoWDBDefs to BDBD (Binary DBD) mentioned below is still experimental but should make for easier/faster updates and a far lower amount of times of manually pressing the update button as it should download a new BDBD file each day similar to the listfile.
Please let me know if there's any issues!

> [!IMPORTANT]
> When upgrading without overwriting the config, make sure to set `"definitionsDir"` to `""` to use BDBD as definition source instead as to not fall behind on WoWDBDefs updates. You can also removed the WoWDBDefs directory if it still exists. 
> If you have a local copy of the WoWDBDefs repository that you keep up-to-date, you can still use that like before instead of relying on BDBD.

> [!NOTE]
> If you downloaded 0.7.0 before this newer release was made (April 9 4PM UTC) please redownload it if you are running into issues with TACTSharp and local installs.

- Added remote builds to builds page (works with both CASCLib and TACTSharp).
- Added input fields on builds page to load specific build/cdnconfigs from CDN (requires WTL to be started with TACTSharp enabled in config).
- Added screenshot combo tab to modelviewer (by @WainPetopia in #41).
- Switched to BDBD (Binary DBD) as default WoWDBDefs definition source. A copy of the WoWDBDefs repo is no longer included with WTL.
- Updating WoWDBDefs from the table browser now grabs the latest BDBD file from the WoWDBDefs releases.
- Adjust background of hamburger menu button in modelviewer by @WainPetopia in #40
- Updated DumpInstall to use proper path separators on non-Windows (by @ChrisKader in #42).
- Updated modelviewer to fix geoset issues with certain models.
- Updated TACTSharp & improved TACTSharp integration, most of WTL should now work (some crashes/bugs still need tracking down).
- Updated TACT key metadata up to February 24th 2025.
- Updated Bootstrap from 5.3.3 to 5.3.5.
- Updated CascLib (new keys).
- Updated DBCD (for BDBD support).
- Updated WoWNamingLib (improved TACTSharp support).
- Updated WoWFormatLib (minor WMO parsing change).
- Updated various packages used by WTL.

0.6.5 (31-05-2025)
- Contenthash naming now also names empty minimaps/maptextures.
- Renamed 11GL to 11GO on naming page.
- Updated diffs to use file extension from file for deleted files.
- Fixed linked M2 textures not being named.
- Fixed crash if group index wasn't present.
- Fixed crash if file type was not set.
- Fixed switching builds always using CASCLib.
- Hopefully fix crash when exporting TACT keys.
- Updated enums.
- Updated TACTSharp.
- Updated CASCLib.
- Updated WoWDBDefs.
- Updated DBCD.

0.6.4 (08-01-2025)
- Updated creaturecache.wdb parsing for 11.1.
- Updated TACTSharp integration.
- Other locales are now supported, features like naming/crawling should no longer instantly crash but might still have issues.

0.6.3 (07-01-2025)
Now that 11.1 PTR is properly available, make sure to remember to remove or rename the fakebuildconfig file if you used the 11.1 loading workaround for 0.6.1/0.6.2.
- Added install files page.
- Added Goblin (11GL) to file naming page.
- Added spells an M2 uses to more information modal on files page (if file naming is initialized).
- Added contenthash to contenthash lookup modal title.
- Added checkbox to naming page to allow choosing whether or not to override existing VO names.
- Fixed crash in DB2 searching when encountering missing DB2s.
- Slightly improved file naming initialization speed.
- Improve spell model name generation.
- Removed quotes from NPC names generated for VO filenames.
- Added opt-in setting to use TACTSharp instead of CASCLib (useTACTSharp: true in config.json). Disabled by default.
- Non-enUS locales, naming, file crawling and model info features are NYI and will crash in this version.
- TACTSharp is meant to run with a lower memory footprint and faster startup than CASCLib does as well as behave in a more stable fashion in online/mixed online & local mode.
- Updated DB2 column filters to actually span their full width (how has it taken this long).
- Updated naming page layout.
- Updated SpellEffect type names enum.
- Updated CASCLib for proper 11.1 root fix implementation (fixes Classic not loading).
- Updated WoWDBDefs.

0.6.2 (20-12-2024)
- Updated CascLib to support V2 root (11.1+).

0.6.1 (20-12-2024)
- Fixed some bad filenames being mapped for Classic.
- Fixed crash when KeyRing field could not be found in .build.info.
- Slightly improved diffing speed.
- Updated WoWDBDefs.

0.6.0 (25-11-2024)
- Upgraded to .NET 9.
- Table browser now lists all DBCs/DB2s regardless of version/availability.
- Table browser now lists all known builds (from DBDs) regardless of availability.
- Missing DBC/DB2s throughout WTL will now be downloaded from wago.tools (when available).
- Note: Make sure the folder specified in dbcFolder in config.json exists (default dbcs), otherwise this functionality is not available.
- Improved automatic naming of item textures and files with lookups.
- Improved diff page layout (by @WainPetopia #33).
- Fixed issue where clicking in diff inputs on diff page would sort the column (by @WainPetopia #33).
- Fixed keyring loading by switching to Akamai CDN.
- Fixed XML diffs showing XML as parsed HTML in diffs.
- Updated TACT key metadata.
- Updated WoWDBDefs/CASCLib.

0.5.1 (01-11-2024)
- Improved various layouts.
- Updated TACT key metadata.
- Updated DB2 info page to support all DBC/DB2 versions.
- Updated WoWDBDefs/CASCLib.
- Fixed issue with TACT key page breaking on 11.0.7+.
- Fixed lines in text diffs not having the proper added/removed colors.
- Fixed crash loading DB2s that have since been removed.
- Fixed some tables using the wrong pagination method.
- Fixed arrow keys not working when using input pagination after 0.5.0.

0.5.0 (07-09-2024)
Modernizing some of the UI frameworks WTL uses here which meant a lot of changes to all pages. There might be some issues due to these upgrades remaining, but most things should work the same/better. Let me know if anything works less better than it did before the update.
- Added switcher between dark/light mode (dark is default).
- Added optional loading of a custom-listfile.csv if it exists that loads entries on top of the normal listfile.
- Updated DBCD to fix issue with deletion hotfixes not working.
- Update Bootstrap from 4.6.2 to 5.3.3.
- Update DataTables from 1.12.1 to 2.1.5.
- Update Select2 from 4.0.7 to 4.0.13.

0.4.9 (31-08-2024)
- Added force re-extraction of DB2s for a build by hitting the force update button on the builds page.
- Added keyring support.
- Added scenescript parsing for VO/Music naming.
- Added icons for sounds, creatures, models and textures back to the files table.
- Added WMO/M2 checkboxes back to the modelviewer page.
- Improved type detection for MP3 files.
  For full types updates you need to remove cachedUnknowns.txt before starting WTL and rerunning type detection.
- Now saves creature displayIDs in the SQLite DB (this will cause a slowdown during VO naming cache processing).
- Updated CascLib, DBCD, WoWFormatLib and WoWNamingLib.

0.4.8 (26-08-2024)
- Automated M2 namer now does newest M2s first so the oldest M2 gets filename priority.
- Added Dragonflight prefixes to naming page.
- Added "current" as valid target build for the added: search, e.g. added:10.2.7.54295|current.
- Added vo/clearCache endpoint to clear VO page cache.
- Improved default search on model page.
- Removed naming overrides for some pre-DF stuff.
- Naming a single file (e.g. m2) now removes it from placeholder names if named properly.
- Fixed bad result window count on naming page.
- Fixed crash during diffing.

0.4.7 (22-07-2024)
Extra release with some unfinished things before prepatch hits, new features might be buggy/WIP.
- Added VO naming page (WIP).
If you want to populate WTL.db with initial data for this page, run the VO namer on the naming page once, then manually browse to <WTL URL>/vo/startBackfill and let it do its thing.
- Added info tab for certain models (WIP).
- Added gameobjectcache.wdb based-naming (WIP).
- Fix crash if WoW folder has caches in unexpected places.
- Various naming page improvements.
- Various name crawler improvements.
- Now trims whitespace around search parameters on files page.
NOTE: A WTL.db with the latest 11.x/10.x builds and prepopulated VO data is included in the assets below, but keep in mind if you overwrite yours and stuff you locally have cached will be overwritten.

- 0.4.6 (21-06-2024)
- Add diffing for certain file types (e.g. models)
- Improved naming (voice over naming has gotten large upgrades)
- Improved file type detection
- Start saving certain things (creature names, broadcast texts) in a local database
- Update flags/enums
- Update animation names (likely for the last time)
- Updated WoWDBDefs/CASCLib
NOTE: There is a WTL.7z attached here, if you want an updated database extract this into/over your local WTL.db.

0.4.5 (25-05-2024)
- Added knownkey/unknownkey file search filters to filter by known/unknown TACT encryption keys
- Added added file search filter
  e.g. search for added:10.2.7.54295|11.0.0.54311 to get new files added between 10.2.7.54295 and 11.0.0.54311
  Note: This requires you to have loaded both builds so WTL has manifests for both on disk.
- Added support for loading listfile parts instead of downloading/reading listfile (
  To use this, set the listfile location in config to the parts dir of wowdev/wow-listfile repo.
  Note: This disables automatic listfile updates, so make sure to regularly pull the repo for updates.
- Added creature VO naming support to naming page.
  Note: This requires a data collection addon such as Datamine by @Ghostopheles to track what VO line belongs to which creatures as well having said voice lines still in your DBCache and the creature in creaturecache.wdb. Related files (Datamine_Data.lua, DBCache.bin and creaturecache.wdb) can also be dropped in a caches directory in the main WTL folder.
- Added support for naming WMO files on the manual model naming page.
- Added support for naming liquid textures in automatic naming.
- Added various contenthash-based file names to automatic naming.
- Fixed various issues/crashes with file naming. this feature is still very experimental.
- Updated flags/enums/FKs (thanks @QartemisT, @raethkcj and @Selenium)
- Updated CascLib/WoWDBDefs.

0.4.4 (11-04-2024)
This will likely be the last release before 11.0 testing begins. Keep in mind that if something breaks with 11.0, my first priority will NOT be wow.tools.local, so you might have to stick it out for a bit.
- Fix for CreatureDisplayInfo files not being picked up during model naming.
- Only show DB2s with actual changes in diffs (ignores schema changes).
- Updated enums.
- Updated CascLib/WoWDBDefs.

0.4.3 (21-03-2024)
- Added "Extract all" button on files page to extract all files in the current search result to disk (folder controlled by new extractionDir setting).
- Added preferHighResTextures setting to prefer loading higher-res BLPs over original ones (currently Cataclysm Beta-only).
- Updated CascLib/WoWDBDefs.

0.4.2 (17-03-2024)
Didn't want to write all this up for a not-likely-but-not-impossible emergency 10.2.6 release, so just a release to sync up.
- Added file-naming tool (work in progress).
- Added multi-file ZIP downloads on files page.
- Hold SHIFT or ALT while clicking "Download" on a file to queue a file/enable the UI.
- SHIFT-A to add all files on page to queue.
- Added link to modelviewer on modelviewer modals (by @WainPetopia).
- Added link to TACT key page in menu (by @WainPetopia).
- Added link to scenescripts page in menu (by @Ghostopheles).
- Added button to relink certain file types on request in file modal.
- Added WMOs back to modelviewer file list.
- Added optional showAllFiles config option to show all files, regardless if they're available.
- Added in-game browser flags (by @Luzifix).
- Added .toc as readable text.
- Updated multiple enums/flags (by @raethkcj).
- Updated wago.tools file downloads build filter to allow for all builds newer than 18378.
- Updated JQuery to 3.7.1.
- Updated animation list.
- Updated CascLib/DBCD/WoWDBDefs.
- Improved historic file downloading to download file when locally available.
- Improved skin selection to include results from both Creature/Item tables (by @WainPetopia).
- Improved texture slot UI on modelviewer (by @WainPetopia).
- Moved "Export keys" button to TACT key page.
- Cleaned up buttons on files page.
- Fixed DB2 info page not working for WDC5 DB2s.
- Fixed issue where files would not show up as decrypted after discovering a new key.

0.4.1 (14-01-2024)
- Fixed broken updater.

0.4.0 (14-01-2024)
- Added file links to the More Information file modal.
- Added file history to the More Information file modal.
- Added manifestFolder startup argument/config variable (by @Ghostopheles).
- Added WoWFormatLib and SereniaBLPLib dependencies.
- Added BLP comparing to build diffs.
- Added text format comparisons to build diffs (requires Git to be installed).
- Updated the modelviewer to support texture replacements up to slot 27 (by @WainPetopia).
- Updated the modelviewer to load additional item textures/particle colors (by @WainPetopia).
- Updated file typing to detect WDC5 DB2 files and to separate WMO (wmo) and group WMOs (gwmo).
- Updated CascLib/ImageSharp/WoWDBDefs.
- Updated TACT key metadata.
- Updated SereniaBLPLib for .NET 8.0 support (0.4.0a)

0.3.0 (08-12-2023)
- Added lookup support.
- Added filename verification icons for files with lookups.
- Added caching of lookups per file ID between branches.
- Updated to .NET 8.
- Updated TACT key metadata.
- Updated enums.
- Improved layout of encryption metadata.
- Improved TACTKey crawling.
- Updated WoWDBDefs/CascLib/DBCD.

0.2.2 (08-11-2023)
- Added basic WorldStateExpression tooltips.
- Updated TACT key metadata.
- Updated enums.
- Updated WoWDBDefs/CascLib.
- Fix files remaining seemingly encrypted after TACT key update.
- Fix DB2 info page not working for those with non-standard DB2 folders.
- Fix download links for files with quotes not working.
- Butts.

0.2.1 (07-10-2023)
- Added file size to more information modal.
- Added support for 3rd texture variation for creature models.
- Added DB2 encryption metadata to more information modals.
- Added searching by encryption key in DB2s (provided key is available), same encrypted:KEYLOOKUP format as files page.
- Added tracking of files that aren't encrypted but are supposed to be (white unlocked icon).
- Fixed unknown files in build diffs page showing up as "unknown/filedataid.unk", it is now back to red rows.
- Fixed deleted files never having names in build diffs.
- Fixed build diffs not being updated after listfile update.
- Fixed more information modal on hotfixes page.
- Updated TACT key metadata.
- Updated DBCD/WoWDBDefs/CascLib.

0.2.0 (10-09-2023)
- Added update checker (experimental).
- Added more information/content hash lookup modals to files/diff pages.
- Added some new file types to type detection.
- Added range:start-end search option for files page.
- Added file tooltips/modals to DBC pages.
- Added foreign key tooltips to DBC diffs.
- Improved listfile downloading.
- Updated DBC flags/enums.
- Updated TACT key metadata.
- Updated animation names.
- Updated WoWDBDefs/CascLib.

0.1.16 (22-08-2023)
- Fixes rare crash with listfile updates

0.1.15 (16-08-2023)
- Updated CascLib to deal with root format change
- Updated included database definitions

0.1.14 (03-08-2023)
- Fix inline DB2 diffs
- Update listfile URL

0.1.13 (21-07-2023)
- Added command line support (by @Ghostopheles).
- Added links to compare/view DBCs from build diffs (by @Ghostopheles).
- Added DBC/DB2 info page.
- Fixed various issues with capitalized listfile (some might remain).
- Fixed some interface textures not display correctly.
- Fixed exact filedataids not showing up in modelviewer search.
- Updated included database definitions.

0.1.12 (08-07-2023)
- Default listfile is now the listfile with capitals.
- Added compare button to DBC page (by @Ghostopheles).
- Updated modelviewer geoset handling (by @WainPetopia).
- Updated included database definitions.

0.1.11 (25-05-2023)
- Add detection for WWF files (thanks @bloerwald).
- Add "all" dropdown on files page (use at own risk).
- Updated DBCD to pick up on hopefully permanent fix for Collectable* DB2s.
- Updated included database definitions.

0.1.10 (12-05-2023)
- Add copy to clipboard button to files page to copy table rows to clipboard in listfile format.
- Fix DB2 cache not being cleared when reloading hotfixes.
- Fix file search being case sensitive.
- Updated DBCD to pick up on temporary crash fix for 10.1.5 Collectable* DB2s.
- Updated included database definitions up to first 10.1.5 build.

0.1.9 (09-05-2023)
- Add support for differently formatted .build.info files.
- Add no-caching rule to some DBC endpoints.
- Fix concurrency issues on hotfixes page by now loading hotfixes on startup.
- Fix hotfixes page showing hotfixes for other builds/products.

0.1.8 (09-05-2023)
- Add hotfixes page (Note: last detected column is based on when the pushID was first seen upon loading the page).
- Add very slow global DB2 search (exact matches only).
- Add TACTKey harvesting by going through all available DBCache files (in WoW dir and local caches directory, see README).
- Add force load button to builds page to force-(re)load builds.
- Add .srt files are previewable text file.
- Fix not being able to download partially encrypted DB2 files.
- Fix issue not being able to load different products that are on the same build as the currently loaded product.
- Updated included database definitions/CASCLib.

0.1.7 (05-04-2023)
- Add buttons for force updating listfile/TACTKeys to files page.
- Add button for updating WoWDBDefs to DBC page.
- Add private aura spell flag (thanks @QartemisT).
- Add scenescript browser for my own purposes (intentionally unlinked, go to /dbc/scenescript.html to access it).
- TACT keys are now loaded from hotfixes when available.
- Updated included database definitions/CASCLib.

0.1.6 (23-03-2023)
- Add new diff UI on build page that is hopefully more clear.
- Add encrypted search option for files page for listing all encrypted files.
- Add optional "caches" directory for loading additional hotfix caches.
- Add sorting for filename column on files page (slow).
- Add loading of cached unknown file types on startup.
- Updated animation names.
- Updated included database definitions/CASCLib.
- Fix some file types not being cached properly.
- Fix missing hotfixes when loading two hotfix caches for the same build if two products were on the same build.
- Reduce file type guesser output.

0.1.5 (12-03-2023)
- Add encrypted status back in build diffs and on files page.
- Red locks = encrypted (unknown keys)
- Yellow lock = partially encrypted (mixed known/unknown keys)
- Green lock = encrypted (known key)
- Add encrypted:KEYLOOKUP back to files page.
- Add buttons for exporting current listfile and tactkeys to a CSV file (placed in executable directory).
- Add note in preview modal about some BLPs displaying as transparent.
- Add file extensions in file download filenames.
- Improve filetype guessing speed by no longer guessing types for encrypted files with unknown keys.
- Improve type: search speed.
- Improve filetype guessing by adding data from DB2s as well.
- Fix issue where searching for partial numbers wouldn't show all IDs.
- Update included database definitions/CASCLib.

0.1.4 (10-03-2023)
- Add preview pane to files page
- Updated included DBDs up to 10.1.0.48480
- Update DBCD for (hackfixed) WDC4 support

0.1.3 (05-02-2023)
- Added experimental DB2 diff page. Please check the README for instructions. No really, please read, for the love of god.
- Home page is now /builds/
- Added manifest/DB2 status indicators to builds page to see if diffs are available and if DB2s are extracted
- Made DB2 diffs work in build diffs
- Updated included DBDs up to 3.4.1.47966

0.1.2 (01-02-2023)
- Rudimentary build diffing is now available. Please check the README for instructions.
- Updated included DBDs up to 10.0.7.47910.

0.1.1 (28-12-2022)
- Add locale config option (default is enUS)
- Add more file types to file analysis and show progress in the console
- Add unnamed and type: search options to files page
- Fix issue where application would start before TACTKeys/listfile update was done

0.1.0 (20-12-2022)
Bumping to 0.1.0 as I think enough features are working to warrant a major bump. :D
- Added basic file previews on files page
- Added filetype analysis for unknown files
- Added support for reading hotfix files from 10.0.5
- Improved error message when config is invalid
- Updated included DBDs to 10.0.5.47215
- Updated CASCLib (loads faster)

0.0.10 (16-12-2022)
- Allow searching by FileDataID (exact matches only)
- Add TACTKeys from GitHub
- Add auto-updating for listfile/tactkeys if older than 1 day
- Add button to reload DBD/DB2 cache (makes working on WoWDBDefs much easier!)
- Update WoWDBDefs for 10.0.5 (but many unnamed things still)

0.0.9 (16-12-2022)
- Fixes issues reading some 10.0.5 DB2s

0.0.8 (11-12-2022)
- Added builds page to switch between locally installed products
- Added files page with very basic file downloading/searching
- Enable hotfix support in DBC browser
- Don't cache static files
- Added "(disk)" behind DB2s loaded from local folder
- Files are now filtered on available files in the loaded build
- Default to current build for ZIP CSV exports when sending a request to /dbc/export/all/

0.0.7 (11-12-2022)
- Update dependencies
- Add DBC/DB2 table exporting
- Add tooltips/enums/flags in DBC browser

0.0.6 (28-11-2022)
- Allow users to supply their own DBCs/DB2s. See README for more on configuring this.

0.0.5 (27-11-2022)
- Hopefully fix an issue where binary files would have line endings/indentations modified on Windows builds causing shaders to not compile correctly.

0.0.4 (27-11-2022)
First automatically built release, includes:
- Modelviewer (M2 only)
- DB2 browser (no hotfixes, exporting or tooltips yet)
