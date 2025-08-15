// Flags are currently retrieved from TrinityCore repo, in a best case scenario these would come from DBD.
const classMask = {
    0x1 : 'WARRIOR',
    0x2 : 'PALADIN',
    0x4 : 'HUNTER',
    0x8 : 'ROGUE',
    0x10 : 'PRIEST',
    0x20 : 'DEATH_KNIGHT',
    0x40 : 'SHAMAN',
    0x80 : 'MAGE',
    0x100 : 'WARLOCK',
    0x200 : 'MONK',
    0x400 : 'DRUID',
    0x800 : 'DEMON_HUNTER',
    0x1000 : 'EVOKER',
}

const achievementFlags = {
    0x1 : 'COUNTER',
    0x2 : 'HIDDEN',
    0x4 : 'PLAY_NO_VISUAL',
    0x8 : 'SUM',
    0x10 : 'MAX_USED',
    0x20 : 'REQ_COUNT',
    0x40 : 'AVERAGE',
    0x80 : 'PROGRESS_BAR',
    0x100 : 'REALM_FIRST_REACH',
    0x200 : 'REALM_FIRST_KILL',
    0x400 : 'UNK3',
    0x800 : 'HIDE_INCOMPLETE',
    0x1000 : 'SHOW_IN_GUILD_NEWS',
    0x2000 : 'SHOW_IN_GUILD_HEADER',
    0x4000 : 'GUILD',
    0x8000 : 'SHOW_GUILD_MEMBERS',
    0x10000 : 'SHOW_CRITERIA_MEMBERS',
    0x20000 : 'ACCOUNT_WIDE',
    0x40000 : 'UNK5',
    0x80000 : 'HIDE_ZERO_COUNTER',
    0x100000 : 'TRACKING_FLAG',
}

const charSectionFlags = {
    0x1 : 'CHAR',
    0x2 : 'BARBERSHOP',
    0x4 : 'DEATHKNIGHT',
    0x8 : 'NPCSKIN',
    0x10 : 'SKIN',
    0x20 : 'DEMONHUNTER',
    0x40 : 'DEMONHUNTERFACE',
    0x80 : 'DHBLINDFOLDS',
    0x100 : 'SILHOUETTE',
    0x200 : 'VOIDELF',
    0x400 : 'HAS_CONDITION?'
}

const taxiNodeFlags = {
    0x1 : 'Show on alliance map',
    0x2 : 'Show on horde map',
    0x4: 'Show on map border',
    0x8: 'Show if client passes condition',
    0x10 : 'Use player favorite mount',
    0x20: 'End point only',
    0x40: 'Ignore for find nearest',
    0x80: 'Do not show in World Map UI'
}

const difficultyFlags = {
    0x1: 'Heroic-style Lockouts',
    0x2: 'Is Default for instance type',
    0x4: 'Is User Selectable',
    0x8: 'Deprecated',
    0x10: 'LFG Only',
    0x20: 'Legacy',
    0x40: 'Display Heroic Banner in the UI',
    0x80: 'Display Mythic Banner in the UI'
}

const mapFlags = {
    0x1: 'Optimize',
    0x2: 'Development Map',
    0x4: 'Weighted Blend',
    0x8: 'Vertex Coloring',
    0x10: 'Sort Objects',
    0x20: 'Limit to players from one realm',
    0x40: 'Enable Lighting',
    0x80: 'Inverted Terrain',
    0x100: 'Dynamic Difficulty',
    0x200: 'Object File',
    0x400: 'Texture File',
    0x800: 'Generate Normals',
    0x1000: 'Fix Border Shadow Seams',
    0x2000: 'Infinite Ocean',
    0x4000: 'Underwater Map',
    0x8000: 'Flexible Raid Locking',
    0x10000: 'Limit Farclip',
    0x20000: 'Use Parent Map Flight Bounds',
    0x40000: 'No race change on this map',
    0x80000: 'Disabled for non-GMs',
    0x100000: 'Weight Normals 1',
    0x200000: 'Disable low detail terrain',
    0x400000: 'Enable Org Arena Blink Rule',
    0x800000: 'Weighted Height Blend',
    0x1000000: 'Coalescing Area Sharing',
    0x2000000: 'Proving Grounds',
    0x4000000: 'Garrison',
    0x8000000: 'Enable AI Need System',
    0x10000000: 'Single VServer', 
    0x20000000: 'Use Instance Pool', 
    0x40000000: 'Map uses Raid Graphics', 
    0x80000000: 'Force custom UI Map', 
}

const mapFlags2 = {
    0x1: 'Dont Activate/Show Map',
    0x2: 'No Vote Kicks',
    0x4: 'No Incoming Transfers',
    0x8: 'Dont Voxelize Path Data',
    0x10: 'Terrain LOD',
    0x20: 'Unclamped Point Lights',
    0x40: 'PVP',
    0x80: 'Ignore Instance Farm Limit',
    0x100: 'Dont Inherit Area Lights From Parent',
    0x200: 'Force Light Buffer On',
    0x400: 'WMO Liquid Scale',
    0x800: 'Spell Clutter On',
    0x1000: 'Spell Clutter Off',
    0x2000: 'Reduced Path Map Height Validation',
    0x4000: 'NEW_MINIMAP_GENERATION',
    0x8000: 'AI_BOTS_DETECTED_LIKE_PLAYERS',
    0x10000: 'LINEARLY_LIT_TERRAIN',
    0x20000: 'FOG_OF_WAR',
    0x40000: 'DISABLE_SHARED_WEATHER_SYSTEMS',
    0x80000: 'HONOR_SPELL_ATTRIBUTE_EX_K_LOS_HITS_NOCAMCOLLIDE',
    0x100000: 'BELONGS_TO_LAYER',
    //0x200000: '',
    //0x400000: '',
    //0x800000: '',
    //0x1000000: '',
    //0x2000000: '',
    //0x4000000: '',
    //0x8000000: '',
};

const mapFlags3 = {
    //0x1: '',
    //0x2: '',
    //0x4: '',
    //0x8: '',
    //0x10: '',
    //0x20: '',
    //0x40: '',
    //0x80: '',
    0x800: 'Warband Scene (maybe)'
}

const soundkitFlags = {
    0x0001: 'UNK1',
    0x0020: 'NO_DUPLICATES',
    0x0200: 'LOOPING',
    0x0400: 'VARY_PITCH',
    0x0800: 'VARY_VOLUME'
}

const globalstringsFlags ={
    0x1: 'FRAMEXML',
    0x2: 'GLUEXML'
}

const inventoryTypeMask = {
    0x2: 'Head',
    0x4: 'Neck',
    0x8: 'Shoulder',
    0x10: 'Body',
    0x20: 'Chest',
    0x40: 'Waist',
    0x80: 'Legs',
    0x100: 'Feet',
    0x200: 'Wrist',
    0x400: 'Hand',
    0x800: 'Finger',
    0x1000: 'Trinket',
    0x2000: 'Main Hand',
    0x4000: 'Off Hand',
    0x8000: 'Ranged',
    0x10000: 'Cloak',
    0x20000: '2H Weapon',
    0x40000: 'Bag',
    0x80000: 'Tabard',
    0x100000: 'Robe',
    0x200000: 'Weapon Main Hand',
    0x400000: 'Weapon Off Hand',
    0x800000: 'Holdable',
    0x1000000: 'Ammo',
    0x2000000: 'Thrown',
    0x4000000: 'Ranged Right',
    0x8000000: 'Quiver',
    0x10000000: 'Relic'
}

const uiMapFlags = {
    0x1: 'Dont display any highlight',
    0x2: 'Show Overlays (puzzle pieces)',
    0x4: 'Show Taxi Nodes',
    0x8: 'Show Garrison Buildings',
    0x10: 'Fallback to Parent Map (e.g. if we dont want a map for this terrain phase)',
    0x20: 'Dont Display Highlight Texture',
    0x40: 'Show Task Objective POIs on Map',
    0x80: 'No World Positions',
    0x100: 'Hide Archaeology Digs',
    0x200: 'DEPRECATED (clear first)',
    0x400: 'Hide Map Icons',
    0x800: 'Hide Vignettes',
    0x1000: 'Force all Overlays Explored (puzzle pieces)',
    0x2000: 'FlightMapShowZoomOut',
    0x4000: 'FlightMapAutoZoom',
    0x8000: 'ForceOnNavbar',
    0x10000: 'AlwaysAllowUserWaypoints',
    0x20000: 'AlwaysAllowTaxiPathing',
    0x40000: 'ForceAllowMapLinks',
    0x80000: 'DoNotShowOnNavbar'
}

const garrAbilityFlags =
{
    0x1: 'Is Trait',
    0x2: 'No Random Selection',
    0x4: 'Horde Only',
    0x8: 'Alliance Only',
    0x10: 'Not Re-Rollable',
    0x20: 'First Slot Only',
    0x40: 'Single Mission Duration',
    0x80: 'Zone Supported Missions Only',
    0x100: 'Apply Only to First Mission of the Day',
    0x200: 'Is Specialization',
    0x400: 'Treat As Equipment Empty Slot'
}

const garrMissionFlags = {
    0x1: 'Is Rare Mission',
    0x2: 'Is Elite Mission',
    0x4: 'Applies Fatigue',
    0x8: 'Always Fail',
    0x10: 'Is Zone Support Mission',
    0x20: 'Requires 100 % Success Chance to Start',
    0x40: 'Do not re-offer',
    0x80: 'Always Succeed',
    0x100: 'DO_NOT_UPDATE_PHASESHIFT_ON_START',
    0x200: 'DO_NOT_UPDATE_PHASESHIFT_ON_COMPLETE',
    0x400: 'DONT_CLEAN_UP_WHILE_OFFERED',
}

const charShipmentFlags = {
    0x1: 'Can Be Started On Mobile App',
    0x2: 'Is Artifact Knowledge'
}

// 196
const currencyFlags = {
    0x1: 'Tradable', // CURRENCY_TRADABLE
    0x2: 'Appears in loot window', // CURRENCY_APPEARS_IN_LOOT_WINDOW
    0x4: 'Computed weekly maximum', // CURRENCY_COMPUTED_WEEKLY_MAXIMUM
    0x8: 'Uses 1/100 scaler for display', // CURRENCY_100_SCALER
    0x10: 'No Low Level Loot', // CURRENCY_NO_LOW_LEVEL_DROP
    0x20: 'Ignore max quantity on load', // CURRENCY_IGNORE_MAX_QTY_ON_LOAD
    0x40: 'Log quantity when changing worlds', // CURRENCY_LOG_ON_WORLD_CHANGE
    0x80: 'Track quantity', // CURRENCY_TRACK_QUANTITY
    0x100: 'Reset tracked quantity when updating version', // CURRENCY_RESET_TRACKED_QUANTITY
    0x200: 'Gains from updating version ignores maximum quantity', // CURRENCY_UPDATE_VERSION_IGNORE_MAX
    0x400: 'Suppress gain message when updating version', // CURRENCY_SUPPRESS_CHAT_MESSAGE_ON_VERSION_CHANGE
    0x800: 'Single Drop from loot', // CURRENCY_SINGLE_DROP_IN_LOOT
    0x1000: 'Has Weekly CatchUp', // CURRENCY_HAS_WEEKLY_CATCHUP
    0x2000: 'Do not compress chat messages', // CURRENCY_DO_NOT_COMPRESS_CHAT
    0x4000: 'Do not log acquisitions to BI (default is now to log)', // CURRENCY_DO_NOT_LOG_ACQUISITION_TO_BI
    0x8000: 'No Raid Drop', // CURRENCY_NO_RAID_DROP
    0x10000: 'Not saved', // CURRENCY_NOT_PERSISTENT
    0x20000: 'Deprecated', // CURRENCY_DEPRECATED
    0x40000: 'Dynamic Maximum', // CURRENCY_DYNAMIC_MAXIMUM
    0x80000: 'Supress all chat messages', // CURRENCY_SUPPRESS_CHAT_MESSAGES
    0x100000: 'Do Not Toast', // CURRENCY_DO_NOT_TOAST
    0x200000: 'Destroy extra when looting while at cap', // CURRENCY_DESTROY_EXTRA_ON_LOOT
    0x400000: 'CURRENCY_DONT_SHOW_TOTAL_IN_TOOLTIP',
    0x800000: 'CURRENCY_DONT_COALESCE_IN_LOOT_WINDOW',
    0x1000000: 'CURRENCY_ACCOUNT_WIDE',
    0x2000000: 'CURRENCY_ALLOW_OVERFLOW_MAILER',
    0x4000000: 'CURRENCY_HIDE_AS_REWARD',
    0x8000000: 'CURRENCY_HAS_WARMODE_BONUS',
    0x10000000: 'CURRENCY_IS_ALLIANCE_ONLY',
    0x20000000: 'CURRENCY_IS_HORDE_ONLY',
    0x40000000: 'CURRENCY_LIMIT_WARMODE_BONUS_ONCE_PER_TOOLTIP',
    0x80000000: 'deprecated_currency_flag',
}

const currencyFlagsB = {
    0x1: 'CURRENCY_B_USE_TOTAL_EARNED_FOR_MAX_QTY',
    0x2: 'CURRENCY_B_SHOW_QUEST_XP_GAIN_IN_TOOLTIP',
    0x4: 'CURRENCY_B_NO_NOTIFICATION_MAIL_ON_OFFLINE_PROGRESS',
    0x8: 'CURRENCY_B_BATTLENET_VIRTUAL_CURRENCY',
    // 0x10: 'future_currency_flag',
    0x20: 'CURRENCY_B_DONT_DISPLAY_IF_ZERO'
}

const garrAutoSpellEffectFlags = {
    0x1: 'USE_ATTACK_FOR_POINTS',
    0x2: 'EXTRA_INITIAL_PERIOD'
}

const garrClassSpecFlags = {
    0x1: 'IS_LIMITED_USE_FOLLOWER',
    0x2: 'NO_XP_GAIN',
    0x4: 'INCREASE_QUALITY_ON_MISSION_SUCCESS',
    0x8: 'AUTO_TROOP',
}

const garrEncounterFlags = {
    0x1: 'CUSTOM_PORTRAIT',
    0x2: 'ELITE'
}

const garrFollowerTypeFlags = {
    0x1: 'ALLOW_FOLLOWER_RENAME',
    0x2: 'ALLOW_MISSION_START_ABOVE_SOFT_CAP',
    0x4: 'ALLOW_MISSION_SALVAGE',
    0x8: 'ALLOW_FOLLOWER_DELETE',
    0x10: 'ILEVEL_INCREASE_REQUIRES_EPIC',
}

const holidayFlags = {
    0x1: 'IS_REGION_WIDE',
    0x2: 'DONT_SHOW_IN_CALENDAR',
    0x4: 'DONT_DISPLAY_END',
    0x8: 'DONT_DISPLAY_BANNER',
    0x10: 'NOT_AVAILABLE_CLIENT_SIDE',
    0x20: 'DURATION_USE_MINUTES',
    0x40: 'BEGIN_EVENT_ONLY_ON_STAGE_CHANGE'
}

const lfgFlags = {
    0x1: 'XREALM',
    0x2: 'GLOBAL',
    0x4: 'HOLIDAY',
    0x8: 'SPECIFIC_REWARD',
    0x10: 'WEEKLY',
    0x20: 'IGNORE_RAID_LOCK',
    0x40: 'SCALING_DUNGEON',
    0x80: 'AUTO_SPEC_FOR_ROLE',
    0x100: 'LEVEL_UP_TOAST',
    0x200: 'DELIVER_RANDOM_WHEN_QUEUED',
    0x400: 'ONLY_SERVER_INITIATED_QUEUE',
    0x800: 'NO_USER_TELEPORT',
    0x1000: 'REQUIRES_PREMADE_GROUP',
    0x2000: 'FORCE_THIS_REWARD',
    0x4000: 'LOOT_METHOD_PERSONAL',
    0x8000: 'HIDE_IF_REQ_UNMET',
    0x10000: 'MULTIFACTION',
    0x20000: 'ALLOW_EMPTY_TEAMS',
    0x40000: 'SKIP_PROPOSAL',
    0x80000: 'FACTION_BALANCE',
    0x100000: 'AUTO_SUMMON_PARTY',
    0x200000: 'ALLOW_SAME_FACTION',
    0x400000: 'SERVER_CONTROLLED_BACKFILL',
    0x800000: 'SEAMLESS_ENTRANCE_QUEUE',
    0x1000000: 'NO_IGNORES',
    0x2000000: 'BACKFILL_LARGE_FIRST',
    0x4000000: 'LEAVE_RIDE_TICKET_ON_TRANSFER',
    0x8000000: 'ALLOW_DESERTERS',
    0x10000000: 'LEAVE_RIDE_TICKET_ON_LOGOUT',
    0x20000000: 'DISABLE_BACKFILL',
    0x40000000: 'DISABLE_SOCIAL_QUEUING',
    0x80000000: 'NO_ROLES'
}

const lfgFlagsB = {
    0x1: 'HIDE_NAME_FROM_UI',
    0x2: 'BAILING_ACCEPTABLE',
    0x4: 'DISABLE_TELEPORT_TO_LEADER',
    0x8: 'KICK_ON_AFK',
    0x10: 'ROLE_CHANGE_ON_ENTER',
    0x20: 'SOLO_LFR_RULES',
    0x40: 'NO_PARTY_INVITES',
    0x80: 'EXTENDED_INSTANCE_SHUTDOWN',
    0x100: 'AVAILABLE_IN_NPE',
    0x200: 'CHROMIE_TIME_RANDOM_DUNGEON',
    0x400: 'CHROMIE_TIME_DUNGEON_POOL',
    0x800: 'DISABLE_CALENDAR_EVENT',
    0x1000: 'INSTANT_SHUTDOWN',
    0x2000: 'ALLOW_CROSS_FACTION_PARTY_QUEUE',
    0x4000: 'HIDE_IF_PLAYER_CONDITION_UNMET',
    0x8000: 'IGNORE_PARTIES',
}

const questTagModifierFlags = {
    0x1: 'RARE',
    0x2: 'ELITE',
    0x4: 'EPIC',
    0x8: 'NO_SPELL_COMPLETE_EFFECTS',
    0x10: 'HIDDEN_IN_QUEST_LOG',
    0x20: 'ALLOW_DISPLAY_PAST_CRITICAL',
    0x40: 'SUPPRESS_EXPIRATION_WARNING',
    0x80: 'SHOW_AS_WORLD_QUEST'
}

const campaignFlags = {
    0x1: 'DONT_USE_JOURNEY_QUEST_BANG',
    0x2: 'IS_CONTAINER'
}

// CreatureType.db2
const targetCreatureType = {
    0x1: 'Beast',
    0x2: 'Dragonkin',
    0x4: 'Demon',
    0x8: 'Elemental',
    0x10: 'Giant',
    0x20: 'Undead',
    0x40: 'Humanoid',
    0x80: 'Critter',
    0x100: 'Mechanical',
    0x200: 'Not specified',
    0x400: 'Totem',
    0x800: 'Non-combat pet',
    0x1000: 'Gas Cloud',
    0x2000: 'Wild Pet',
    0x4000: 'Aberration'
}

// SpellCastTargetFlags from TrinityCore
const targetFlags = {
    0x1: 'TARGET_FLAG_UNUSED_1',
    0x2: 'TARGET_FLAG_UNIT',
    0x4: 'TARGET_FLAG_UNIT_RAID',
    0x8: 'TARGET_FLAG_UNIT_PARTY',
    0x10: 'TARGET_FLAG_ITEM',
    0x20: 'TARGET_FLAG_SOURCE_LOCATION',
    0x40: 'TARGET_FLAG_DEST_LOCATION',
    0x80: 'TARGET_FLAG_UNIT_ENEMY',
    0x100: 'TARGET_FLAG_UNIT_ALLY',
    0x200: 'TARGET_FLAG_CORPSE_ENEMY',
    0x400: 'TARGET_FLAG_UNIT_DEAD',
    0x800: 'TARGET_FLAG_GAMEOBJECT',
    0x1000: 'TARGET_FLAG_TRADE_ITEM',
    0x2000: 'TARGET_FLAG_STRING',
    0x4000: 'TARGET_FLAG_GAMEOBJECT_ITEM',
    0x8000: 'TARGET_FLAG_CORPSE_ALLY',
    0x10000: 'TARGET_FLAG_UNIT_MINIPET',
    0x20000: 'TARGET_FLAG_GLYPH_SLOT',
    0x40000: 'TARGET_FLAG_DEST_TARGET',
    0x80000: 'TARGET_FLAG_EXTRA_TARGETS',
    0x100000: 'TARGET_FLAG_UNIT_PASSENGER',
    // 0x200000: 'TARGET_FLAG_UNK200000',
    // 0X400000: 'TARGET_FLAG_UNK400000',
    // 0X800000: 'TARGET_FLAG_UNK800000',
    // 0X1000000: 'TARGET_FLAG_UNK1000000',
    // 0X2000000: 'TARGET_FLAG_UNK2000000',
    // 0X4000000: 'TARGET_FLAG_UNK4000000',
    // 0X8000000: 'TARGET_FLAG_UNK8000000',
    // 0X10000000: 'TARGET_FLAG_UNK10000000',
    // 0X20000000: 'TARGET_FLAG_UNK20000000',
    // 0X40000000: 'TARGET_FLAG_UNK40000000'
}

const facingCasterFlags = {
    0x1: 'Facing',
    // 0x2: '',
    // 0x4: '',
    0x8: 'Behind',
};

const areaTableFlags = {
    0x00000001: 'Emit Breath Particles',
    0x00000002: 'Breath Particles Override Parent',
    0x00000004: 'On Map Dungeon',
    0x00000008: 'Allow Trade Channel',
    0x00000010: 'Enemies PvP Flagged',
    0x00000020: 'Allow Resting',
    0x00000040: 'Allow Dueling',
    0x00000080: 'Free For All PvP',
    0x00000100: 'Linked Chat (Set in cities)',
    0x00000200: 'Linked Chat Special Area',
    0x00000400: 'Force this area when on a Dynamic Transport',
    0x00000800: 'No PvP',
    0x00001000: 'No Ghost on Release',
    0x00002000: 'Sub-zone Ambient Multiplier',
    0x00004000: 'Enable Flight Bounds on Map',
    0x00008000: 'PVP POI',
    0x00010000: 'No chat channel',
    0x00020000: 'Area not in use',
    0x00040000: 'Contested',
    0x00080000: 'No Player Summoning',
    0x00100000: 'No Dueling if Tournament Realm',
    0x00200000: 'Players Call Guards',
    0x00400000: 'Horde Resting',
    0x00800000: 'Alliance Restubg',
    0x01000000: 'Combat Zone',
    0x02000000: 'Force Indoors',
    0x04000000: 'Force Outdoors',
    0x08000000: 'Allow Hearth-and-Resurrect from Area',
    0x10000000: 'No Local Defense Channel',
    0x20000000: 'Only Evaluate Ghost Bind Once',
    0x40000000: 'Is Subzone'
};

const areaTableFlags2 = {
    0x1: 'Dont Evaluate Graveyard From Client',
    0x2: 'Force Micro-Dungeon Art Map (ask Programmer)',
    0x4: 'Use subzone player loot',
    0x8: 'Allow Pet Battle Dueling even if no Dueling Allowed',
    0x10: 'Use Map Transfer Locs for Cemeteries',
    0x20: 'Is Garrison',
    0x40: 'Use subzone for chat channel',
    0x80: 'Dont realm-coalesce chat channel',
    0x100: 'Not explorable (dont assign area bit)',
    0x200: 'Dont use parent map cemeteries',
    0x400: 'Dont show Sanctuary text',
    0x800: 'Cross-faction zone chat',
    0x1000: 'Force No Resting',
    0x2000: 'Allow War Mode Toggle',
    0x4000: 'Is New Player Experience' // Speculation
}

const criteriaTreeFlags = {
    0x1: 'Progress Bar',
    0x2: 'Do Not Display',
    0x4: 'Is a Date',
    0x8: 'Is Money',
    0x10: 'Toast on Complete',
    0x20: 'Use Objects Description',
    0x40: 'Show faction specific child',
    0x80: 'Display all children',
    0x100: 'Award Bonus Rep (Hack!!)',
    0x200: 'Treat this criteria or block as Alliance',
    0x400: 'Treat this criteria or block as Horde',
    0x800: 'Display as Fraction',
    0x1000: 'Is For Quest',
    // 0x2000: '',
    // 0x4000: '',
    // 0x8000: '',
}

const areaPOIFlags = {
    0x1: 'Show On Minimap',
    0x2: 'Show Minimap Icon',
    0x4: 'Worldmap Zone Zoom',
    0x8: 'Worldmap Continent Zoom',
    0x10: 'Worldmap World Zoom',
    0x20: 'Worldmap City Zoom',
    0x40: 'Hidden (Quest/Gossip Only)',
    0x80: 'Always Draw Icon (world map)',
    0x100: 'Show When Indoors',
    0x200: 'Show in BattleMap',
    0x400: 'Only show in current area',
    0x800: 'Cemetery can be selected',
    0x1000: 'Show on Worldmap Zone in unexplored areas',
    0x2000: 'Battle Pet Tamer',
    0x4000: 'Only show in direct area ancestry',
    0x8000: 'Hide Text',
    0x10000: 'Display as Banner',
    0x20000: 'Show on flightmap',
    0x40000: 'Worldmap Micro-Dungeon Zoom',
    0x80000: 'Assault/SEND_TO_MOBILE',
    0x100000: 'ShouldGlow',
    0x200000: 'Always visible on flightmap',
    0x400000: 'HIDE_TIMER_IN_TOOLTIP',
    0x800000: 'NO_PADDING_ABOVE_TOOLTIP_WIDGETS',
    0x1000000: 'HIGHLIGHT_WORLD_QUESTS_ON_HOVER',
    0x2000000: 'HIGHLIGHT_VIGNETTES_ON_HOVER',
}

const summonPropertiesFlags = {
    0x1: 'Attack Summoner',
    0x2: 'Help when Summoned in combat',
    0x4: 'Use Level Offset',
    0x8: 'Despawn on Summoner Death',
    0x10: 'Only Visible to Summoner',
    0x20: 'Cannot Dismiss Pet',
    0x40: 'Use Demon Timeout',
    0x80: 'Unlimited Summons',
    0x100: 'Use Creature Level',
    0x200: 'Join Summoners Spawn Group',
    0x400: 'Do Not Toggle',
    0x800: 'Despawn When Expired',
    0x1000: 'Use Summoner Faction',
    0x2000: 'Do Not Follow Mounted Summoner',
    0x4000: 'Save Pet Autocast',
    0x8000: 'Ignore Summoners Phase (Wild Only)',
    0x10000: 'Only Visible to Summoners Group',
    0x20000: 'Despawn on Summoner Logout',
    0x40000: 'Cast Ride Vehicle Spell on Summoner',
    0x80000: 'Guardian Acts Like a Pet',
    0x100000: 'Dont Snap Sessile To Ground',
    0x200000: 'Summons from Battle Pet Journal',
    0x400000: 'Unit Clutter',
    0x800000: 'Default Name Color',
    0x1000000: 'Use Own Invisibility Detection (Ignore Owners Invisibility Detection)',
    0x2000000: 'Despawn When Replaced (Totem Slots Only)',
    0x4000000: 'Despawn When Teleporting Out of Range',
    0x8000000: 'Summoned at Group Formation Position',
    0x10000000: 'Dont Despawn On Summoners Death',
    0x20000000: 'Use Title As Creature Name',
    0x40000000: 'Attackable By Summoner',
    0x80000000: 'Dont dismiss when an encounter is aborted'
}

const questFlags0 = {
    0x1: 'STAY_ALIVE',
    0x2: 'PARTY_ACCEPT',
    0x4: 'EXPLORATION',
    0x8: 'SHARABLE',
    0x10: 'HAS_CONDITION',
    0x20: 'HIDE_REWARD_POI',
    0x40: 'RAID',
    0x80: 'WAR_MODE_REWARDS_OPT_IN',
    0x100: 'NO_MONEY_FROM_XP',
    0x200: 'HIDDEN_REWARDS',
    0x400: 'TRACKING',
    0x800: 'DEPRECATE_REPUTATION',
    0x1000: 'DAILY',
    0x2000: 'FLAGS_PVP',
    0x4000: 'UNAVAILABLE',
    0x8000: 'WEEKLY',
    0x10000: 'AUTOCOMPLETE',
    0x20000: 'DISPLAY_ITEM_IN_TRACKER',
    0x40000: 'OBJ_TEXT',
    0x80000: 'AUTO_ACCEPT',
    0x100000: 'PLAYER_CAST_ON_ACCEPT',
    0x200000: 'PLAYER_CAST_ON_COMPLETE',
    0x400000: 'UPDATE_PHASE_SHIFT',
    0x800000: 'SOR_WHITELIST',
    0x1000000: 'LAUNCH_GOSSIP_COMPLETE',
    0x2000000: 'REMOVE_EXTRA_GET_ITEMS',
    0x4000000: 'HIDE_UNTIL_DISCOVERED',
    0x8000000: 'PORTRAIT_IN_QUEST_LOG',
    0x10000000: 'SHOW_ITEM_WHEN_COMPLETED',
    0x20000000: 'LAUNCH_GOSSIP_ACCEPT',
    0x40000000: 'ITEMS_GLOW_WHEN_DONE',
    0x80000000: 'FAIL_ON_LOGOUT'
}

const questFlags1 = {
    0x1: 'KEEP_ADDITIONAL_ITEMS',
    0x2: 'SUPPRESS_GOSSIP_COMPLETE',
    0x4: 'SUPPRESS_GOSSIP_ACCEPT',
    0x8: 'DISALLOW_PLAYER_AS_QUESTGIVER',
    0x10: 'DISPLAY_CLASS_CHOICE_REWARDS',
    0x20: 'DISPLAY_SPEC_CHOICE_REWARDS',
    0x40: 'REMOVE_FROM_LOG_ON_PERIDOIC_RESET',
    0x80: 'ACCOUNT_LEVEL_QUEST',
    0x100: 'LEGENDARY_QUEST',
    0x200: 'NO_GUILD_XP',
    0x400: 'RESET_CACHE_ON_ACCEPT',
    0x800: 'NO_ABANDON_ONCE_ANY_OBJECTIVE_COMPLETE',
    0x1000: 'RECAST_ACCEPT_SPELL_ON_LOGIN',
    0x2000: 'UPDATE_ZONE_AURAS',
    0x4000: 'NO_CREDIT_FOR_PROXY',
    0x8000: 'DISPLAY_AS_DAILY_QUEST',
    0x10000: 'PART_OF_QUEST_LINE',
    0x20000: 'QUEST_FOR_INTERNAL_BUILDS_ONLY',
    0x40000: 'SUPPRESS_SPELL_LEARN_TEXT_LINE',
    0x80000: 'DISPLAY_HEADER_AS_OBJECTIVE_FOR_TASKS',
    0x100000: 'GARRISON_NON_OWNERS_ALLOWED',
    0x200000: 'REMOVE_QUEST_ON_WEEKLY_RESET',
    0x400000: 'SUPPRESS_FAREWELL_AUDIO_AFTER_QUEST_ACCEPT',
    0x800000: 'REWARDS_BYPASS_WEEKLY_CAPS_AND_SEASON_TOTAL',
    0x1000000: 'IS_WORLD_QUEST',
    0x2000000: 'NOT_IGNORABLE',
    0x4000000: 'AUTO_PUSH',
    0x8000000: 'NO_SPELL_COMPLETE_EFFECTS',
    0x10000000: 'DO_NOT_TOAST_HONOR_REWARD',
    0x20000000: 'KEEP_REPEATABLE_QUEST_ON_FACTION_CHANGE',
    0x40000000: 'KEEP_PROGRESS_ON_FACTION_CHANGE',
    0x80000000: 'PUSH_TEAM_QUEST_USING_MAP_CONTROLLER'
}

const questFlags2 = {
    0x1: 'RESET_ON_GAME_MILESTONE',
    0x2: 'NO_WAR_MODE_BONUS',
    0x4: 'AWARD_HIGHEST_PROFESSION',
    0x8: 'NOT_REPLAYABLE',
    0x10: 'NO_REPLAY_REWARDS',
    0x20: 'DISABLE_WAYPOINT_PATHING',
    0x40: 'RESET_ON_MYTHIC_PLUS_SEASON',
    0x80: 'RESET_ON_PVP_SEASON',
    0x100: 'ENABLE_OVERRIDE_SORT_ORDER',
    0x200: 'FORCE_STARTING_LOC_ON_ZONE_MAP',
    0x400: 'BONUS_LOOT_NEVER',
    0x0800: 'BONUS_LOOT_ALWAYS',
    0x1000: 'HIDE_TASK_ON_MAIN_MAP',
    0x2000: 'HIDE_TASK_IN_TRACKER',
    0x4000: 'SKIP_DISABLED_CHECK',
    0x8000: 'ENFORCE_MAXIMUM_QUEST_LEVEL',
}

const auraInterruptFlags0 = {
    0x00000001: 'Hostile Action Received Cancels', // HITBYSPELL
    0x00000002: 'Damage Cancels', // TAKE_DAMAGE
    0x00000004: 'Action Cancels', // CAST
    0x00000008: 'Moving Cancels', // MOVE
    0x00000010: 'Turning Cancels', // TURNING
    0x00000020: 'Anim Cancels', // JUMP
    0x00000040: 'Dismount Cancels', // NOT_MOUNTED
    0x00000080: 'Under Water Cancels', // NOT_ABOVEWATER
    0x00000100: 'Above Water Cancels', // NOT_UNDERWATER
    0x00000200: 'Sheathing Cancels', // NOT_SHEATHED
    0x00000400: 'Interacting Cancels', // TALK
    0x00000800: 'Looting Cancels', // USE
    0x00001000: 'Attacking Cancels', // MEELEE_ATTACK
    0x00002000: 'Item Use Cancels', // SPELL_ATTACK
    0x00004000: 'Damage Channel Duration',
    0x00008000: 'Shapeshifting Cancels', // TRANSFORM
    0x00010000: 'Action Cancels - Late',
    0x00020000: 'Mount Cancels', // MOUNT
    0x00040000: 'Standing Cancels', // NOT_SEATED
    0x00080000: 'Leave World Cancels', // CHANGE_MAP
    0x00100000: 'Stealth/Invis Cancels', // IMMUNE_OR_LOST_SELECTION
    0x00200000: 'Invulnerability Buff Cancels',
    0x00400000: 'Enter World Cancels', // TELEPORTED
    0x00800000: 'PvP Active Cancels', // ENTER_PVP_COMBAT
    0x01000000: 'Non-Periodic Damage Cancels', // DIRECT_DAMAGE
    0x02000000: 'Landing Cancels', // LANDING
    0x04000000: 'Release Cancels',
    0x08000000: 'Damage Cancels Script',
    0x10000000: 'Entering Combat Cancels',
    0x20000000: 'Login Cancels',
    0x40000000: 'Summon Cancels',
    0x80000000: 'Leaving Combat Cancels',
};

const auraInterruptFlags1 = {
    0x00000001: 'Falling Cancels',
    0x00000002: 'Swimming Cancels',
    0x00000004: 'Not Moving Cancels',
    0x00000008: 'Ground Cancels',
    0x00000010: 'Transform Cancels',
    0x00000020: 'Jumping Cancels',
    0x00000040: 'Change Spec Cancels',
    0x00000080: 'Abandon Vehicle Cancels',
    0x00000100: 'Start of Encounter Cancels',
    0x00000200: 'End of Encounter Cancels',
    0x00000400: 'Disconnect Cancels',
    0x00000800: 'Entering Instance Cancels',
    0x00001000: 'Duel Ends Cancels',
    0x00002000: 'Leave Arena or Battleground Cancels',
    0x00004000: 'Change Talent Cancels',
    0x00008000: 'Change Glyph Cancels',
    0x00010000: 'Seamless Transfer Cancels',
};

const cdiFlags = {};

const contentTuningFlags = {
    0x4: 'DisabledForItem',
    0x8: 'Horde',
    0x10: 'Alliance'
}

const socketColors = {
    0x00001: 'META',
    0x00002: 'RED',
    0x00004: 'YELLOW',
    0x00008: 'BLUE',
    0x00010: 'HYDRAULIC',
    0x00020: 'COGWHEEL',
    0x0000E: 'PRISMATIC',
    0x00040: 'RELIC_IRON',
    0x00080: 'RELIC_BLOOD',
    0x00100: 'RELIC_SHADOW',
    0x00200: 'RELIC_FEL',
    0x00400: 'RELIC_ARCANE',
    0x00800: 'RELIC_FROST',
    0x01000: 'RELIC_FIRE',
    0x02000: 'RELIC_WATER',
    0x04000: 'RELIC_LIFE',
    0x08000: 'RELIC_WIND',
    0x10000: 'RELIC_HOLY',
    0x20000: 'PUNCHCARD_RED',
    0x40000: 'PUNCHCARD_YELLOW',
    0x80000: 'PUNCHCARD_BLUE',
    0x100000: 'DOMINATION_BLOOD',
    0x200000: 'DOMINAITON_FROST',
    0x400000: 'DOMINATION_UNHOLY',
    0x800000: 'CYPHER',
}

// 134
const spellAttributes0 = {
    0x00000001: 'Proc Failure Burns Charge',
    0x00000002: 'Uses Ranged Slot', // REQ_AMMO -- on next ranged
    0x00000004: 'On Next Swing (No Damage)', // ON_NEXT_SWING
    0x00000008: 'Do Not Log Immune Misses', // IS_REPLENISHMENT -- not set in 3.0.3
    0x00000010: 'Is Ability', // ABILITY -- client puts 'ability' instead of 'spell' in game strings for these spells
    0x00000020: 'Is Tradeskill', // TRADESPELL -- trade spells (recipes), will be added by client to a sublist of profession spell
    0x00000040: 'Passive', // PASSIVE -- Passive spell
    0x00000080: 'Do Not Display (Spellbook, Aura Icon, Combat Log)', // HIDDEN_CLIENTSIDE -- Spells with this attribute are not visible in spellbook or aura bar
    0x00000100: 'Do Not Log', // HIDE_IN_COMBAT_LOG -- This attribite controls whether spell appears in combat logs
    0x00000200: 'Held Item Only', // TARGET_MAINHAND_ITEM -- Client automatically selects item from mainhand slot as a cast target
    0x00000400: 'On Next Swing', // ON_NEXT_SWING_2
    0x00000800: 'Wearer Casts Proc Trigger',
    0x00001000: 'Server Only', // DAYTIME_ONLY -- only useable at daytime, not set in 2.4.2
    0x00002000: 'Allow Item Spell In PvP', // NIGHT_ONLY -- only useable at night, not set in 2.4.2
    0x00004000: 'Only Indoors', // INDOORS_ONLY -- only useable indoors, not set in 2.4.2
    0x00008000: 'Only Outdoors', // OUTDOORS_ONLY -- Only useable outdoors.
    0x00010000: 'Not Shapeshifted', // NOT_SHAPESHIFT -- Not while shapeshifted
    0x00020000: 'Only Stealthed', // ONLY_STEALTHED -- Must be in stealth
    0x00040000: 'Do Not Sheath', // DONT_AFFECT_SHEATH_STATE -- client won't hide unit weapons in sheath on cast/channel
    0x00080000: 'Scales w/ Creature Level', // LEVEL_DAMAGE_CALCULATION -- spelldamage depends on caster level
    0x00100000: 'Cancels Auto Attack Combat', // STOP_ATTACK_TARGET -- Stop attack after use this spell (and not begin attack if use)
    0x00200000: 'No Active Defense', // IMPOSSIBLE_DODGE_PARRY_BLOCK -- Cannot be dodged/parried/blocked
    0x00400000: 'Track Target in Cast (Player Only)', // CAST_TRACK_TARGET -- Client automatically forces player to face target when casting
    0x00800000: 'Allow Cast While Dead', // CASTABLE_WHILE_DEAD -- castable while dead?
    0x01000000: 'Allow While Mounted', // CASTABLE_WHILE_MOUNTED -- castable while mounted
    0x02000000: 'Cooldown On Event', // DISABLED_WHILE_ACTIVE -- Activate and start cooldown after aura fade or remove summoned creature or go
    0x04000000: 'Aura Is Debuff', // NEGATIVE_1 -- Many negative spells have this attr
    0x08000000: 'Allow While Sitting', // CASTABLE_WHILE_SITTING -- castable while sitting
    0x10000000: 'Not In Combat (Only Peaceful)', // CANT_USED_IN_COMBAT -- Cannot be used in combat
    0x20000000: 'No Immunities', // UNAFFECTED_BY_INVULNERABILITY -- unaffected by invulnerability (hmm possible not...)
    0x40000000: 'Heartbeat Resist', // HEARTBEAT_RESIST_CHECK -- random chance the effect will end TODO: implement core support
    0x80000000: 'No Aura Cancel', // CANT_CANCEL -- positive aura can't be canceled
};

const spellAttributes1 = {
    0x00000001: 'Dismiss Pet First', // DISMISS_PET -- for spells without this flag client doesn't allow to summon pet if caster has a pet
    0x00000002: 'Use All Mana', // DRAIN_ALL_POWER -- use all power (Only paladin Lay of Hands and Bunyanize)
    0x00000004: 'Is Channelled', // CHANNELED_1 -- clientside checked? cancelable?
    0x00000008: 'No Redirection', // CANT_BE_REDIRECTED
    0x00000010: 'No Skill Increase',
    0x00000020: 'Allow While Stealthed', // NOT_BREAK_STEALTH -- Not break stealth
    0x00000040: 'Is Self Channelled', // CHANNELED_2
    0x00000080: 'No Reflection', // CANT_BE_REFLECTED
    0x00000100: 'Only Peaceful Targets', // CANT_TARGET_IN_COMBAT -- can target only out of combat units
    0x00000200: 'Initiates Combat (Enables Auto-Attack)', // MELEE_COMBAT_START -- player starts melee combat after this spell is cast
    0x00000400: 'No Threat', // NO_THREAT -- no generates threat on cast 100% (old NO_INITIAL_AGGRO)
    0x00000800: 'Aura Unique',
    0x00001000: 'Failure Breaks Stealth', // IS_PICKPOCKET -- Pickpocket
    0x00002000: 'Toggle Far Sight', // FARSIGHT -- Client removes farsight on aura loss
    0x00004000: 'Track Target in Channel', // CHANNEL_TRACK_TARGET -- Client automatically forces player to face target when channeling
    0x00008000: 'Immunity Purges Effect', // DISPEL_AURAS_ON_IMMUNITY -- remove auras on immunity
    0x00010000: 'Immunity to Hostile & Friendly Effects', // UNAFFECTED_BY_SCHOOL_IMMUNE -- on immuniy
    0x00020000: 'No AutoCast (AI)', // UNAUTOCASTABLE_BY_PET
    0x00040000: 'Prevents Anim',
    0x00080000: 'Exclude Caster', // CANT_TARGET_SELF
    0x00100000: 'Finishing Move - Damage', // REQ_COMBO_POINTS1 -- Req combo points on target
    0x00200000: 'Threat only on Miss',
    0x00400000: 'Finishing Move - Duration', // REQ_COMBO_POINTS2 -- Req combo points on target
    0x00800000: 'Ignore Owner\'s Death',
    0x01000000: 'Special Skillup', // IS_FISHING -- only fishing spells
    0x02000000: 'Aura Stays After Combat',
    0x04000000: 'Require All Targets',
    0x08000000: 'Discount Power On Miss',
    0x10000000: 'No Aura Icon', // DONT_DISPLAY_IN_AURA_BAR -- client doesn't display these spells in aura bar
    0x20000000: 'Name in Channel Bar', // CHANNEL_DISPLAY_SPELL_NAME -- spell name is displayed in cast bar instead of 'channeling' text
    0x40000000: 'Combo on Block (Mainline: Dispel All Stacks)', // ENABLE_AT_DODGE -- Overpower
    0x80000000: 'Cast When Learned',
};

const spellAttributes2 = {
    0x00000001: 'Allow Dead Target', // CAN_TARGET_DEAD -- can target dead unit or corpse
    0x00000002: 'No shapeshift UI',
    0x00000004: 'Ignore Line of Sight', // CAN_TARGET_NOT_IN_LOS -- 26368 4.0.1 dbc change
    0x00000008: 'Allow Low Level Buff',
    0x00000010: 'Use Shapeshift Bar', // DISPLAY_IN_STANCE_BAR -- client displays icon in stance bar when learned, even if not shapeshift
    0x00000020: 'Auto Repeat', // AUTOREPEAT_FLAG
    0x00000040: 'Cannot cast on tapped', // CANT_TARGET_TAPPED -- target must be tapped by caster
    0x00000080: 'Do Not Report Spell Failure',
    0x00000100: 'Include In Advanced Combat Log',
    0x00000200: 'Always Cast As Unit',
    0x00000400: 'Special Taming Flag',
    0x00000800: 'No Target Per-Second Costs', // HEALTH_FUNNEL
    0x00001000: 'Chain From Caster',
    0x00002000: 'Enchant own item only', // PRESERVE_ENCHANT_IN_ARENA -- Items enchanted by spells with this flag preserve the enchant to arenas
    0x00004000: 'Allow While Invisible',
    0x00008000: 'Do Not Consume if Gained During Cast',
    0x00010000: 'No Active Pets', // TAME_BEAST
    0x00020000: 'Do Not Reset Combat Timers', // NOT_RESET_AUTO_ACTIONS -- don't reset timers for melee autoattacks (swings) or ranged autoattacks (autoshoots)
    0x00040000: 'No Jump While Cast Pending', // REQ_DEAD_PET -- Only Revive pet and Heart of the Pheonix
    0x00080000: 'Allow While Not Shapeshifted (caster form)', // NOT_NEED_SHAPESHIFT -- does not necessarly need shapeshift
    0x00100000: 'Initiate Combat Post-Cast (Enables Auto-Attack)',
    0x00200000: 'Fail on all targets immune', // DAMAGE_REDUCED_SHIELD -- for ice blocks, pala immunity buffs, priest absorb shields, but used also for other spells -> not sure!
    0x00400000: 'No Initial Threat',
    0x00800000: 'Proc Cooldown On Failure', // IS_ARCANE_CONCENTRATION -- Only mage Arcane Concentration have this flag
    0x01000000: 'Item Cast With Owner Skill',
    0x02000000: 'Don\'t Block Mana Regen',
    0x04000000: 'No School Immunities', // UNAFFECTED_BY_AURA_SCHOOL_IMMUNE -- unaffected by school immunity
    0x08000000: 'Ignore Weaponskill',
    0x10000000: 'Not an Action', // IGNORE_ITEM_CHECK -- Spell is cast without checking item requirements (charges/reagents/totem)
    0x20000000: 'Can\'t Crit', // CANT_CRIT -- Spell can't crit
    0x40000000: 'Active Threat', // TRIGGERED_CAN_TRIGGER_PROC -- spell can trigger even if triggered
    0x80000000: 'Retain Item Cast', // FOOD_BUFF -- Food or Drink Buff (like Well Fed)
};

const spellAttributes3 = {
    0x00000001: 'PvP Enabling',
    0x00000002: 'No Proc Equip Requirement',
    0x00000004: 'No Casting Bar Text',
    0x00000008: 'Completely Blocked', // BLOCKABLE_SPELL -- Only dmg class melee in 3.1.3
    0x00000010: 'No Res Timer', // IGNORE_RESURRECTION_TIMER -- you don't have to wait to be resurrected with these spells
    0x00000020: 'No Durability Loss',
    0x00000040: 'No Avoidance',
    0x00000080: 'DoT Stacking Rule', // STACK_FOR_DIFF_CASTERS -- separate stack for every caster
    0x00000100: 'Only On Player', // ONLY_TARGET_PLAYERS -- can only target players
    0x00000200: 'Not a Proc', // TRIGGERED_CAN_TRIGGER_PROC_2 -- triggered from effect?
    0x00000400: 'Requires Main-Hand Weapon', // MAIN_HAND -- Main hand weapon required
    0x00000800: 'Only Battlegrounds', // BATTLEGROUND -- Can only be cast in battleground
    0x00001000: 'Only On Ghosts', // ONLY_TARGET_GHOSTS
    0x00002000: 'Hide Channel Bar', // DONT_DISPLAY_CHANNEL_BAR -- Clientside attribute - will not display channeling bar
    0x00004000: 'Hide In Raid Filter', // IS_HONORLESS_TARGET -- "Honorless Target" only this spells have this flag
    0x00008000: 'Normal Ranged Attack',
    0x00010000: 'Suppress Caster Procs', // CANT_TRIGGER_PROC -- confirmed with many patchnotes
    0x00020000: 'Suppress Target Procs', // NO_INITIAL_AGGRO -- Soothe Animal, 39758, Mind Soothe
    0x00040000: 'Always Hit', // IGNORE_HIT_RESULT -- Spell should always hit its target
    0x00080000: 'Instant Target Procs', // DISABLE_PROC -- during aura proc no spells can trigger (20178, 20375)
    0x00100000: 'Allow Aura While Dead', // DEATH_PERSISTENT -- Death persistent spells
    0x00200000: 'Only Proc Outdoors',
    0x00400000: 'Casting Cancels Autorepeat (Mainline: Do Not Trigger Target Stand)', // REQ_WAND -- Req wand
    0x00800000: 'No Damage History',
    0x01000000: 'Requires Off-Hand Weapon', // REQ_OFFHAND -- Req offhand weapon
    0x02000000: 'Treat As Periodic', // TREAT_AS_PERIODIC -- Makes the spell appear as periodic in client combat logs - used by spells that trigger another spell on each tick
    0x04000000: 'Can Proc From Procs', // CAN_PROC_WITH_TRIGGERED -- auras with this attribute can proc from triggered spell casts with SPELL_ATTR3_TRIGGERED_CAN_TRIGGER_PROC_2 (67736 + 52999)
    0x08000000: 'Only Proc on Caster', // DRAIN_SOUL -- only drain soul has this flag
    0x10000000: 'Ignore Caster & Target Restrictions',
    0x20000000: 'Ignore Caster Modifiers', // NO_DONE_BONUS -- Ignore caster spellpower and done damage mods?  client doesn't apply spellmods for those spells
    0x40000000: 'Do Not Display Range', // DONT_DISPLAY_RANGE -- client doesn't display range in tooltip for those spells
    0x80000000: 'Not On AOE Immune',
};

const spellAttributes4 = {
    0x00000001: 'No Cast Log', // IGNORE_RESISTANCES -- spells with this attribute will completely ignore the target's resistance (these spells can't be resisted)
    0x00000002: 'Class Trigger Only On Target', // PROC_ONLY_ON_CASTER -- proc only on effects with TARGET_UNIT_CASTER?
    0x00000004: 'Aura Expires Offline',
    0x00000008: 'No Helpful Threat',
    0x00000010: 'No Harmful Threat',
    0x00000020: 'Allow Client Targeting',
    0x00000040: 'Cannot Be Stolen', // NOT_STEALABLE -- although such auras might be dispellable, they cannot be stolen
    0x00000080: 'Allow Cast While Casting', // CAN_CAST_WHILE_CASTING -- Can be cast while another cast is in progress - see CanCastWhileCasting(SpellRec const*,CGUnit_C *,int &)
    0x00000100: 'Ignore Damage Taken Modifiers', // FIXED_DAMAGE -- Ignores resilience and any (except mechanic related) damage or % damage taken auras on target.
    0x00000200: 'Combat Feedback When Usable', // TRIGGER_ACTIVATE -- initially disabled / trigger activate from event (Execute, Riposte, Deep Freeze end other)
    0x00000400: 'Weapon Speed Cost Scaling', // SPELL_VS_EXTEND_COST -- Rogue Shiv have this flag
    0x00000800: 'No Partial Immunity',
    0x00001000: 'Aura Is Buff',
    0x00002000: 'Do Not Log Caster', // COMBAT_LOG_NO_CASTER -- No caster object is sent to client combat log
    0x00004000: 'Reactive Damage Proc', // DAMAGE_DOESNT_BREAK_AURAS -- doesn't break auras by damage from these spells
    0x00008000: 'Not In Spellbook',
    0x00010000: 'Not In Arena or Rated Battleground', // NOT_USABLE_IN_ARENA_OR_RATED_BG -- Cannot be used in both Arenas or Rated Battlegrounds
    0x00020000: 'Ignore Default Arena Restrictions', // USABLE_IN_ARENA
    0x00040000: 'Bouncy Chain Missiles', // AREA_TARGET_CHAIN -- (NYI)hits area targets one after another instead of all at once
    0x00080000: 'Allow Proc While Sitting',
    0x00100000: 'Aura Never Bounces', // NOT_CHECK_SELFCAST_POWER -- supersedes message "More powerful spell applied" for self casts.
    0x00200000: 'Allow Entering Arena',
    0x00400000: 'Proc Suppress Swing Anim',
    0x00800000: 'Suppress Weapon Procs',
    0x01000000: 'Auto Ranged Combat',
    0x02000000: 'Owner Power Scaling', // IS_PET_SCALING -- pet scaling auras
    0x04000000: 'Only Flying Areas', // CAST_ONLY_IN_OUTLAND -- Can only be used in Outland.
    0x08000000: 'Force Display Castbar',
    0x10000000: 'Ignore Combat Timer',
    0x20000000: 'Aura Bounce Fails Spell',
    0x40000000: 'Obsolete',
    0x80000000: 'Use Facing From Spell',
};

const spellAttributes5 = {
    0x00000001: 'Allow Actions During Channel', // CAN_CHANNEL_WHEN_MOVING -- available casting channel spell when moving
    0x00000002: 'No Reagent Cost With Aura', // NO_REAGENT_WHILE_PREP -- not need reagents if UNIT_FLAG_PREPARATION
    0x00000004: 'Remove Entering Arena',
    0x00000008: 'Allow While Stunned', // USABLE_WHILE_STUNNED -- usable while stunned
    0x00000010: 'Triggers Channeling',
    0x00000020: 'Limit N', // SINGLE_TARGET_SPELL -- Only one target can be apply at a time
    0x00000040: 'Ignore Area Effect PvP Check',
    0x00000080: 'Not On Player',
    0x00000100: 'Not On Player Controlled NPC',
    0x00000200: 'Extra Initial Period', // START_PERIODIC_AT_APPLY -- begin periodic tick at aura apply
    0x00000400: 'Do Not Display Duration', // HIDE_DURATION -- do not send duration to client
    0x00000800: 'Implied Targeting', // ALLOW_TARGET_OF_TARGET_AS_TARGET -- uses target's target as target if original target not valid (intervene for example)
    0x00001000: 'Melee Chain Targeting',
    0x00002000: 'Spell Haste Affects Periodic', // HASTE_AFFECT_DURATION -- haste effects decrease duration of this
    0x00004000: 'Not Available While Charmed', // NOT_USABLE_DURING_MIND_CONTROL
    0x00008000: 'Treat as Area Effect',
    0x00010000: 'Aura Affects Not Just Req. Equipped Item',
    0x00020000: 'Allow While Fleeing', // USABLE_WHILE_FEARED -- usable while feared
    0x00040000: 'Allow While Confused', // USABLE_WHILE_CONFUSED -- usable while confused
    0x00080000: 'AI Doesn\'t Face Target', // DONT_TURN_DURING_CAST -- Blocks caster's turning when casting (client does not automatically turn caster's model to face UNIT_FIELD_TARGET)
    0x00100000: 'Do Not Attempt a Pet Resummon When Dismounting',
    0x00200000: 'Ignore Target Requirements',
    0x00400000: 'Not On Trivial',
    0x00800000: 'No Partial Resists',
    0x01000000: 'Ignore Caster Requirements',
    0x02000000: 'Always Line of Sight',
    0x04000000: 'Always AOE Line of Sight',
    0x08000000: 'No Caster Aura Icon', // DONT_SHOW_AURA_IF_SELF_CAST -- Auras with this attribute are not visible on units that are the caster
    0x10000000: 'No Target Aura Icon', // DONT_SHOW_AURA_IF_NOT_SELF_CAST -- Auras with this attribute are not visible on units that are not the caster
    0x20000000: 'Aura Unique Per Caster',
    0x40000000: 'Always Show Ground Texture',
    0x80000000: 'Add Melee Hit Rating',
};

const spellAttributes6 = {
    0x00000001: 'No Cooldown On Tooltip', // DONT_DISPLAY_COOLDOWN -- client doesn't display cooldown in tooltip for these spells
    0x00000002: 'Do Not Reset Cooldown In Arena', // ONLY_IN_ARENA -- only usable in arena
    0x00000004: 'Not an Attack', // IGNORE_CASTER_AURAS
    0x00000008: 'Can Assist Immune PC', // ASSIST_IGNORE_IMMUNE_FLAG -- skips checking UNIT_FLAG_IMMUNE_TO_PC and UNIT_FLAG_IMMUNE_TO_NPC flags on assist
    0x00000010: 'Ignore For Mod Time Rate',
    0x00000020: 'Do Not Consume Resources',
    0x00000040: 'Floating Combat Text On Cast', // USE_SPELL_CAST_EVENT -- Auras with this attribute trigger SPELL_CAST combat log event instead of SPELL_AURA_START (clientside attribute)
    0x00000080: 'Aura Is Weapon Proc',
    0x00000100: 'Do Not Chain To Crowd-Controlled Targets', // CANT_TARGET_CROWD_CONTROLLED
    0x00000200: 'Allow On Charmed Targets',
    0x00000400: 'No Aura Log', // CAN_TARGET_POSSESSED_FRIENDS
    0x00000800: 'Not In Raid Instances', // NOT_IN_RAID_INSTANCE -- not usable in raid instance
    0x00001000: 'Allow While Riding Vehicle', // CASTABLE_WHILE_ON_VEHICLE -- castable while caster is on vehicle
    0x00002000: 'Ignore Phase Shift', // CAN_TARGET_INVISIBLE -- ignore visibility requirement for spell target (phases, invisibility, etc.)
    0x00004000: 'AI Primary Ranged Attack',
    0x00008000: 'No Pushback',
    0x00010000: 'No Jump Pathing',
    0x00020000: 'Allow Equip While Casting',
    0x00040000: 'Originate From Controller', // CAST_BY_CHARMER -- client won't allow to cast these spells when unit is not possessed && charmer of caster will be original caster
    0x00080000: 'Delay Combat Timer During Cast',
    0x00100000: 'Aura Icon Only For Caster (Limit 10)', // ONLY_VISIBLE_TO_CASTER -- Auras with this attribute are only visible to their caster (or pet's owner)
    0x00200000: 'Show Mechanic as Combat Text', // CLIENT_UI_TARGET_EFFECTS -- it's only client-side attribute
    0x00400000: 'Absorb Cannot Be Ignore',
    0x00800000: 'Taps immediately',
    0x01000000: 'Can Target Untargetable', // CAN_TARGET_UNTARGETABLE
    0x02000000: 'Doesn\'t Reset Swing Timer if Instant', // NOT_RESET_SWING_IF_INSTANT -- Exorcism, Flash of Light
    0x04000000: 'Vehicle Immunity Category',
    0x08000000: 'Ignore Healing Modifiers',
    0x10000000: 'Do Not Auto Select Target with Initiates Combat',
    0x20000000: 'Ignore Caster Damage Modifiers', // NO_DONE_PCT_DAMAGE_MODS -- ignores done percent damage mods?
    0x40000000: 'Disable Tied Effect Points',
    0x80000000: 'No Category Cooldown Mods', // IGNORE_CATEGORY_COOLDOWN_MODS -- Spells with this attribute skip applying modifiers to category cooldowns
};

const spellAttributes7 = {
    0x00000001: 'Allow Spell Reflection',
    0x00000002: 'No Target Duration Mod', // IGNORE_DURATION_MODS -- Duration is not affected by duration modifiers
    0x00000004: 'Disable Aura While Dead', // REACTIVATE_AT_RESURRECT -- Paladin's auras and 65607 only.
    0x00000008: 'Debug Spell', // IS_CHEAT_SPELL -- Cannot cast if caster doesn't have UnitFlag2 & UNIT_FLAG2_ALLOW_CHEAT_SPELLS
    0x00000010: 'Treat as Raid Buff',
    0x00000020: 'Can Be Multi Cast', // SUMMON_TOTEM -- Only Shaman totems.
    0x00000040: 'Don\'t Cause Spell Pushback', // NO_PUSHBACK_ON_DAMAGE -- Does not cause spell pushback on damage
    0x00000080: 'Prepare for Vehicle Control End',
    0x00000100: 'Horde Specific Spell', // HORDE_ONLY -- Teleports, mounts and other spells.
    0x00000200: 'Alliance Specific Spell', // ALLIANCE_ONLY -- Teleports, mounts and other spells.
    0x00000400: 'Dispel Removes Charges', // DISPEL_CHARGES -- Dispel and Spellsteal individual charges instead of whole aura.
    0x00000800: 'Can Cause Interrupt', // INTERRUPT_ONLY_NONPLAYER -- Only non-player casts interrupt, though Feral Charge - Bear has it.
    0x00001000: 'Can Cause Silence', // SILENCE_ONLY_NONPLAYER -- Not set in 3.2.2a.
    0x00002000: 'No UI Not Interruptible',
    0x00004000: 'Recast On Resummon',
    0x00008000: 'Reset Swing Timer at spell start',
    0x00010000: 'Only In Spellbook Until Learned', // CAN_RESTORE_SECONDARY_POWER -- These spells can replenish a powertype, which is not the current powertype.
    0x00020000: 'Do Not Log PvP Kill',
    0x00040000: 'Attack on Charge to Unit', // HAS_CHARGE_EFFECT -- Only spells that have Charge among effects.
    0x00080000: 'Report Spell failure to unit target', // ZONE_TELEPORT -- Teleports to specific zones.
    0x00100000: 'No Client Fail While Stunned, Fleeing, Confused',
    0x00200000: 'Retain Cooldown Through Load',
    0x00400000: 'Ignores Cold Weather Flying Requirement',
    0x00800000: 'No Attack Dodge',
    0x01000000: 'No Attack Parry',
    0x02000000: 'No Attack Miss',
    0x04000000: 'Treat as NPC AoE',
    0x08000000: 'Bypass No Resurrect Aura',
    0x10000000: 'Do Not Count For PvP Scoreboard', // CONSOLIDATED_RAID_BUFF -- May be collapsed in raid buff frame (clientside attribute)
    0x20000000: 'Reflection Only Defends',
    0x40000000: 'Can Proc From Suppressed Target Procs',
    0x80000000: 'Always Cast Log', // CLIENT_INDICATOR
};

const spellAttributes8 = {
    0x00000001: 'No Attack Block', // CANT_MISS
    0x00000002: 'Ignore Dynamic Object Caster',
    0x00000004: 'Remove Outside Dungeons and Raids',
    0x00000008: 'Only Target If Same Creator',
    0x00000010: 'Can Hit AOE Untargetable',
    0x00000020: 'Allow While Charmed',
    0x00000040: 'Aura Required by Client',
    0x00000080: 'Ignore Sanctuary',
    0x00000100: 'Use Target\'s Level for Spell Scaling', // AFFECT_PARTY_AND_RAID -- Nearly all spells have "all party and raid" in description
    0x00000200: 'Periodic Can Crit', // DONT_RESET_PERIODIC_TIMER -- Periodic auras with this flag keep old periodic timer when refreshing at close to one tick remaining (kind of anti DoT clipping)
    0x00000400: 'Mirror creature name', // NAME_CHANGED_DURING_TRANSFORM -- according to wowhead comments, name changes, title remains
    0x00000800: 'Only Players Can Cast This Spell',
    0x00001000: 'Aura Points On Client', // AURA_SEND_AMOUNT -- Aura must have flag AFLAG_ANY_EFFECT_AMOUNT_SENT to send amount
    0x00002000: 'Not In Spellbook Until Learned',
    0x00004000: 'Target Procs On Caster',
    0x00008000: 'Requires location to be on liquid surface', // WATER_MOUNT -- only one River Boat used in Thousand Needles
    0x00010000: 'Only Target Own Summons',
    0x00020000: 'Haste Affects Duration',
    0x00040000: 'Ignore Spellcast Override Cost', // REMEMBER_SPELLS -- at some point in time, these auras remember spells and allow to cast them later
    0x00080000: 'Allow Targets Hidden by Spawn Tracking', // USE_COMBO_POINTS_ON_ANY_TARGET -- allows to consume combo points from dead targets
    0x00100000: 'Requires Equipped Inv Types', // ARMOR_SPECIALIZATION
    0x00200000: 'No \'Summon + Dest from Client\' Targeting Pathing Requirement',
    0x00400000: 'Melee Haste Affects Periodic',
    0x00800000: 'Enforce In Combat Ressurection Limit', // BATTLE_RESURRECTION -- Used to limit the Amount of Resurrections in Boss Encounters
    0x01000000: 'Heal Prediction', // HEALING_SPELL
    0x02000000: 'No Level Up Toast',
    0x04000000: 'Skip Is Known Check', // RAID_MARKER -- probably spell no need learn to cast
    0x08000000: 'AI Face Target',
    0x10000000: 'Not in Battleground', // NOT_IN_BG_OR_ARENA -- not allow to cast or deactivate currently active effect, not sure about Fast Track
    0x20000000: 'Mastery Affects Points', // MASTERY_SPECIALIZATION
    0x40000000: 'Display Large Aura Icon On Unit Frames (Boss Aura)', // HIGH_PRIORITY -- Show on raid frames
    0x80000000: 'Can Attack ImmunePC', // ATTACK_IGNORE_IMMUNE_TO_PC_FLAG -- Do not check UNIT_FLAG_IMMUNE_TO_PC in IsValidAttackTarget
};

const spellAttributes9 = {
    0x00000001: 'Force Dest Location',
    0x00000002: 'Mod Invis Includes Party',
    0x00000004: 'Only When Illegally Mounted', // RESTRICTED_FLIGHT_AREA -- Dalaran and Wintergrasp flight area auras have it
    0x00000008: 'Do Not Log Aura Refresh',
    0x00000010: 'Missile Speed is Delay (in sec)', // SPECIAL_DELAY_CALCULATION
    0x00000020: 'Ignore Totem Requirements for Casting', // SUMMON_PLAYER_TOTEM
    0x00000040: 'Item Cast Grants Skill Gain',
    0x00000080: 'Do Not Add to Unlearn List',
    0x00000100: 'Cooldown Ignores Ranged Weapon', // AIMED_SHOT
    0x00000200: 'Not In Arena', // NOT_USABLE_IN_ARENA -- Cannot be used in arenas
    0x00000400: 'Target Must Be Grounded',
    0x00000800: 'Allow While Banished Aura State',
    0x00001000: 'Face unit target upon completion of jump charge',
    0x00002000: 'Haste Affects Melee Ability Casttime', // SLAM
    0x00004000: 'Ignore Default Rated Battleground Restrictions', // USABLE_IN_RATED_BATTLEGROUNDS -- Can be used in Rated Battlegrounds
    0x00008000: 'Do Not Display Power Cost',
    0x00010000: 'Next modal spell requires same unit target',
    0x00020000: 'AutoCast Off By Default',
    0x00040000: 'Ignore School Lockout',
    0x00080000: 'Allow Dark Simulacrum',
    0x00100000: 'Allow Cast While Channeling',
    0x00200000: 'Suppress Visual Kit Errors',
    0x00400000: 'Spellcast Override In Spellbook',
    0x00800000: 'JumpCharge - no facing control',
    0x01000000: 'Ignore Caster Healing Modifiers',
    0x02000000: '(Programmer Only) Don\'t consume charge if item deleted',
    0x04000000: 'Item Passive On Client',
    0x08000000: 'Force Corpse Target',
    0x10000000: 'Cannot Kill Target',
    0x20000000: 'Log Passive',
    0x40000000: 'No Movement Radius Bonus',
    0x80000000: 'Channel Persists on Pet Follow',
};

const spellAttributes10 = {
    0x00000001: 'Bypass Visibility Check',
    0x00000002: 'Ignore Positive Damage Taken Modifiers',
    0x00000004: 'Uses Ranged Slot (Cosmetic Only)',
    0x00000008: 'Do Not Log Full Overheal',
    0x00000010: 'NPC Knockback - ignore doors', // WATER_SPOUT
    0x00000020: 'Force Non-Binary Resistance',
    0x00000040: 'No Summon Log',
    0x00000080: 'Ignore instance lock and farm limit on teleport', // TELEPORT_PLAYER -- 4 Teleport Player spells
    0x00000100: 'Area Effects Use Target Radius',
    0x00000200: 'Charge/JumpCharge - Use Absolute Speed',
    0x00000400: 'Proc cooldown on a per target basis',
    0x00000800: 'Lock chest at precast', // HERB_GATHERING_MINING -- Only Herb Gathering and Mining
    0x00001000: 'Use Spell Base Level For Scaling', // USE_SPELL_BASE_LEVEL_FOR_SCALING
    0x00002000: 'Reset cooldown upon ending an encounter', // 'RESET_AFTER_ENCOUNTER_ENDS', -- 'RESET_AFTER_ENCOUNTER_ENDS',
    0x00004000: 'Rolling Periodic',
    0x00008000: 'Spellbook Hidden Until Overridden',
    0x00010000: 'Defend Against Friendly Cast',
    0x00020000: 'Allow Defense While Casting',
    0x00040000: 'Allow Defense While Channeling',
    0x00080000: 'Allow Fatal Duel Damage',
    0x00100000: 'Multi-Click Ground Targeting',
    0x00200000: 'AoE Can Hit Summoned Invis',
    0x00400000: 'Allow While Stunned By Horror Mechanic',
    0x00800000: 'Visible only to caster (conversations only)',
    0x01000000: 'Update Passives on Apply/Remove',
    0x02000000: 'Normal Melee Attack',
    0x04000000: 'Ignore Feign Death',
    0x08000000: 'Caster Death Cancels Persistent Area Auras',
    0x10000000: 'Do Not Log Absorb',
    0x20000000: 'This Mount is NOT at the account level', // MOUNT_IS_NOT_ACCOUNT_WIDE -- This mount is stored per-character
    0x40000000: 'Prevent Client Cast Cancel',
    0x80000000: 'Enforce Facing on Primary Target Only',
};

const spellAttributes11 = {
    0x00000001: 'Lock Caster Movement and Facing While Casting',
    0x00000002: 'Don\'t Cancel When All Effects are Disabled',
    0x00000004: 'Scales with Casting Item\'s Level', // SCALES_WITH_ITEM_LEVEL
    0x00000008: 'Do Not Log on Learn',
    0x00000010: 'Hide Shapeshift Requirements',
    0x00000020: 'Absorb Falling Damage', // SPELL_ATTR11_ABSORB_ENVIRONMENTAL_DAMAGE
    0x00000040: 'Unbreakable Channel',
    0x00000080: 'Ignore Caster\'s spell level', // SPELL_ATTR11_RANK_IGNORES_CASTER_LEVEL -- Spell_C_GetSpellRank returns SpellLevels->MaxLevel * 5 instead of std::min(SpellLevels->MaxLevel, caster->Level) * 5
    0x00000100: 'Transfer Mount Spell',
    0x00000200: 'Ignore Spellcast Override Shapeshift Requirements',
    0x00000400: 'Newest Exclusive Complete',
    0x00000800: 'Not in Instances',
    0x00001000: 'Obsolete',
    0x00002000: 'Ignore PvP Power',
    0x00004000: 'Can Assist Uninteractible',
    0x00008000: 'Cast When Initial Logging In',
    0x00010000: 'Not in Mythic+ Mode (Challenge Mode)', // NOT_USABLE_IN_CHALLENGE_MODE
    0x00020000: 'Cheaper NPC Knockback',
    0x00040000: 'Ignore Caster Absorb Modifiers',
    0x00080000: 'Ignore Target Absorb Modifiers',
    0x00100000: 'Hide Loss of Control UI',
    0x00200000: 'Allow Harmful on Friendly',
    0x00400000: 'Cheap Missile AOI',
    0x00800000: 'Expensive Missile AOI',
    0x01000000: 'No Client Fail on No Pet',
    0x02000000: 'AI Attempt Cast on Immune Player',
    0x04000000: 'Allow While Stunned by Stun Mechanic',
    0x08000000: 'Don\'t close loot window',
    0x10000000: 'Hide Damage Absorb UI',
    0x20000000: 'Do Not Treat As Area Effect',
    0x40000000: 'Check Required Target Aura By Caster',
    0x80000000: 'Apply Zone Aura Spell To Pets',
};

const spellAttributes12 = {
    0x00000001: 'Enable Procs from Suppressed Caster Procs',
    0x00000002: 'Can Proc from Suppressed Caster Procs',
    0x00000004: 'Show Cooldown As Charge Up',
    0x00000008: 'No PvP Battle Fatigue',
    0x00000010: 'Treat Self Cast As Reflect',
    0x00000020: 'Do Not Cancel Area Aura on Spec Switch',
    0x00000040: 'Cooldown on Aura Cancel Until Combat Ends',
    0x00000080: 'Do Not Re-apply Area Aura if it Persists Through Update',
    0x00000100: 'Display Toast Message',
    0x00000200: 'Active Passive',
    0x00000400: 'Ignore Damage Cancels Aura Interrupt',
    0x00000800: 'Face Destination',
    0x00001000: 'Immunity Purges Spell',
    0x00002000: 'Do Not Log Spell Miss',
    0x00004000: 'Ignore Distance Check On Charge/Jump Charge Done Trigger Spell',
    0x00008000: 'Disable known spells while charmed',
    0x00010000: 'Ignore Damage Absorb',
    0x00020000: 'Not In Proving Grounds',
    0x00040000: 'Override Default SpellClick Range',
    0x00080000: 'Is In-Game Store Effect',
    0x00100000: 'Allow during spell override',
    0x00200000: 'Use float values for scaling amounts',
    0x00400000: 'Suppress toasts on item push',
    0x00800000: 'Trigger Cooldown On Spell Start',
    0x01000000: 'Never Learn', // IS_GARRISON_BUFF
    0x02000000: 'No Deflect',
    0x04000000: '(Deprecated) Use Start-of-Cast Location for Spell Dest',
    0x08000000: 'Recompute Aura on Mercenary Mode', // IS_READINESS_SPELL
    0x10000000: 'Use Weighted Random For Flex Max Targets',
    0x20000000: 'Ignore Resilience',
    0x40000000: 'Apply Resilience To Self Damage',
    0x80000000: 'Only Proc From Class Abilities',
};

const spellAttributes13 = {
    0x00000001: 'Allow Class Ability Procs',
    0x00000002: 'Allow While Feared By Fear Mechanic',
    0x00000004: 'Cooldown Shared With AI Group',
    0x00000008: 'Interrupts Current Cast',
    0x00000010: 'Periodic Script Runs Late',
    0x00000020: 'Recipe Hidden Until Known',
    0x00000040: 'Can Proc From Lifesteal',
    0x00000080: 'Nameplate Personal Buffs/Debuffs',
    0x00000100: 'Cannot Lifesteal/Leech',
    0x00000200: 'Global Aura',
    0x00000400: 'Nameplate Enemy Debuffs',
    0x00000800: 'Always Allow PvP Flagged Target',
    0x00001000: 'Do Not Consume Aura Stack On Proc',
    0x00002000: 'Do Not PvP Flag Caster',
    0x00004000: 'Always Require PvP Target Match',
    0x00008000: 'Do Not Fail if No Target',
    0x00010000: 'Displayed Outside Of Spellbook',
    0x00020000: 'Check Phase on String ID Results',
    0x00040000: 'Do Not Enforce Shapeshift Requirements', // ACTIVATES_REQUIRED_SHAPESHIFT
    0x00080000: 'Aura Persists Through Tame Pet',
    0x00100000: 'Periodic Refresh Extends Duration',
    0x00200000: 'Use Skill Rank As Spell Level',
    0x00400000: 'Aura Always Shown', // IS_ALWAYS_SHOWN
    0x00800000: 'Use Spell Level For Item Squish Compensation',
    0x01000000: 'Chain by Most Hit',
    0x02000000: 'Do Not Display Cast Time',
    0x04000000: 'Always Allow Negative Healing Percent Modifiers',
    0x08000000: 'Do Not Allow "Disable Movement Interrupt"',
    0x10000000: 'Allow Aura On Level Scale',
    0x20000000: 'Remove Aura On Level Scale',
    0x40000000: 'Recompute Aura On Level Scale',
    0x80000000: 'Update Fall Speed After Aura Removal',
};

const spellAttributes14 = {
    0x00000001: 'Prevent Jumping During Precast',
    0x00100000: 'Private Aura',
};

const spellAttributes15 = {

}

// 151, part 0/4
const itemStaticFlags0 = {
    0x00000001: 'No Pickup',
    0x00000002: 'Conjured', // Conjured item
    0x00000004: 'Has Loot Table', // HAS_LOOT -- Item can be right clicked to open for loot
    0x00000008: 'Heroic Tooltip', // Makes green "Heroic" text appear on item
    0x00000010: 'Deprecated', // Cannot equip or use
    0x00000020: 'No User Destroy', // Item can not be destroyed, except by using spell (item can be reagent for spell)
    0x00000040: 'Player Cast', // Item's spells are castable by players
    0x00000080: 'No Equip Cooldown', // No default 30 seconds cooldown when equipped
    0x00000100: 'Multi-Loot Quest',
    0x00000200: 'Gift Wrap', // IS_WRAPPER -- Item can wrap other items
    0x00000400: 'Uses Resources',
    0x00000800: 'Multi Drop', // Looting this item does not remove it from available loot
    0x00001000: 'In-Game Refund', // ITEM_PURCHASE_RECORD -- Item can be returned to vendor for its original cost (extended cost)
    0x00002000: 'Petition', // Item is guild or arena charter
    0x00004000: 'Has Text', // Only readable items have this (but not all)
    0x00008000: 'No Disenchant', 
    0x00010000: 'Real Duration', 
    0x00020000: 'No Creator',
    0x00040000: 'Prospectable', // Item can be prospected
    0x00080000: 'Unique Equippable', // You can only equip one of these
    0x00100000: 'Disable Aura Quotas', // 'Disable Auto Qutoes' -- IGNORE_FOR_AURAS
    0x00200000: 'Ignore Default Arena Restrictions', // Item can be used during arena match
    0x00400000: 'No Durability Loss', // Some Thrown weapons have it (and only Thrown) but not all
    0x00800000: 'Useable While Shapeshifted', // USE_WHEN_SHAPESHIFTED -- Item can be used in shapeshift forms
    0x01000000: 'Has Quest Glow',
    0x02000000: 'Hide Unusable Recipe', // Profession recipes can only be looted if you meet requirements and don't already know it
    0x04000000: 'Not Usable In Arena', // Item cannot be used in arena
    0x08000000: 'Bound to Account (must be soulbound)', // IS_BOUND_TO_ACCOUNT -- Item binds to account and can be sent only to your own characters
    0x10000000: 'No Reagent Cost', // Spell is cast ignoring reagents
    0x20000000: 'Millable', // Item can be milled
    0x40000000: 'Report to Guild Chat',
    0x80000000: 'Don\'t use dynamic drop chance (Quest)', // NO_DYNAMIC_DROP_CHANCE
};

// 151, part 1/4
const itemStaticFlags1 = {
    0x00000001: 'Horde specific item', // FACTION_HORDE
    0x00000002: 'Alliance specific item', // FACTION_ALLIANCE
    0x00000004: '(deprecated) Don\'t ignore buy prices when extended cost is specified', // DONT_IGNORE_BUY_PRICE -- when item uses extended cost, gold is also required
    0x00000008: 'Only caster classes can roll need', // CLASSIFY_AS_CASTER
    0x00000010: 'Only non-caster classes can roll need', // CLASSIFY_AS_PHYSICAL
    0x00000020: 'Everyone can roll Need',
    0x00000040: 'Cannot Trade Bind on Pickup', // NO_TRADE_BIND_ON_ACQUIRE
    0x00000080: 'Can Trade Bind on Pickup', // CAN_TRADE_BIND_ON_ACQUIRE
    0x00000100: 'Players can only roll Greed (Need Before Greed only)', // CAN_ONLY_ROLL_GREED
    0x00000200: 'This is a caster weapon', // CASTER_WEAPON
    0x00000400: 'Delete On Login',
    0x00000800: 'Internal test item that players will never be able to see', // INTERNAL_ITEM
    0x00001000: 'Vendors will not purchase this item (has no vendor value)', // NO_VENDOR_VALUE
    0x00002000: 'Item is a well-known discovered item', // SHOW_BEFORE_DISCOVERED
    0x00004000: 'Override the automatic cost calculation', // OVERRIDE_GOLD_COST
    0x00008000: 'Ignore Default Rated Battleground Restrictions', // IGNORE_DEFAULT_RATED_BG_RESTRICTIONS
    0x00010000: 'Not Usable In Rated Battleground', // NOT_USABLE_IN_RATED_BG
    0x00020000: 'Bound to Battle.net Account (must be bound to account)', // BNET_ACCOUNT_TRADE_OK
    0x00040000: 'Ask for confirmation before use', // CONFIRM_BEFORE_USE
    0x00080000: 'Reevaluate binding when transforming to this item', // REEVALUATE_BONDING_ON_TRANSFORM
    0x00100000: 'Don\'t transform when all charges are consumed', // NO_TRANSFORM_ON_CHARGE_DEPLETION
    0x00200000: 'Cannot alter this item\'s visual appearance', // NO_ALTER_ITEM_VISUAL
    0x00400000: 'Cannot alter items to look like this item', // NO_SOURCE_FOR_ITEM_VISUAL
    0x00800000: 'Can be used as a source for item visual regardless of quality', // IGNORE_QUALITY_FOR_ITEM_VISUAL_SOURCE
    0x01000000: 'No Durability',
    0x02000000: '"Tank" role assignment required in order to get Need +Roll bonus in LFR', // ROLE_TANK
    0x04000000: '"Healer" role assignment required in order to get Need +Roll bonus in LFR', // ROLE_HEALER
    0x08000000: '"DPS" role assignment required in order to get Need +Roll bonus in LFR', // ROLE_DAMAGE
    0x10000000: 'Item can drop in Challenge Mode', // CAN_DROP_IN_CHALLENGE_MODE
    0x20000000: 'Don\'t stack this item in the loot window', // NEVER_STACK_IN_LOOT_UI
    0x40000000: 'Disenchant to Loot Table',
    0x80000000: 'Can be placed in reagent bank', // USED_IN_A_TRADESKILL
};

// 151, part 2/4
const itemStaticFlags2 = {
    0x00000001: 'Don\'t destroy on Quest Accept',
    0x00000002: 'Item Can Be Upgraded',
    0x00000004: 'Upgrade From Item Overrides Drop Upgrade',
    0x00000008: 'Always Free For All in Loot', // ALWAYS_FFA_IN_LOOT
    0x00000010: 'Hide Item Upgrades If Not Upgraded', // HIDE_UPGRADE_LEVELS_IF_NOT_UPGRADED
    0x00000020: 'Update NPC Interactions when picked up', // UPDATE_INTERACTIONS
    0x00000040: 'Doesn\'t leave progressive win history', // UPDATE_DOESNT_LEAVE_PROGRESSIVE_WIN_HISTORY
    0x00000080: 'Ignore Item History Tracker',
    0x00000100: 'Ignore Item Level Cap in PvP',
    0x00000200: 'Display As Heirloom', // Item appears as having heirloom quality ingame regardless of its real quality (does not affect stat calculation)
    0x00000400: 'Skip use check on pickup',
    0x00000800: 'No Loot Overflow Mail', // OBSOLETE
    0x00001000: 'Don\'t display in guild news', // Item is not included in the guild news panel
    0x00002000: 'Trial of the Gladiator Gear', // PVP_TOURNAMENT_GEAR
    0x00004000: 'Requires log line to be generated on stack count change', // REQUIRES_STACK_CHANGE_LOG
    0x00008000: 'Toy', // UNUSED_FLAG
    0x00010000: 'Suppress Name Suffixes', // HIDE_NAME_SUFFIX
    0x00020000: 'Push Loot',
    0x00040000: 'Don\'t report loot log to party/raid', // DONT_REPORT_LOOT_LOG_TO_PARTY
    0x00080000: 'Always Allow Dual Wield',
    0x00100000: 'Obliteratable',
    0x00200000: 'Acts as Transmog Hidden Visual Option',
    0x00400000: 'Expire On Weekly Reset',
    0x00800000: 'Doesn\'t show up in transmog UI until collected', // DOESNT_SHOW_UP_IN_TRANSMOG_UNTIL_COLLECTED
    0x01000000: 'Can store enchants',
    0x02000000: 'Hide Quest Item from Object Tooltip',
    0x04000000: 'Do Not Toast',
    0x08000000: 'Ignore item creation context for progressive win history', // IGNORE_CREATION_CONTEXT_FOR_PROGRESSIVE_WIN_HISTORY
    0x10000000: 'Force all specs for Item History', // FORCE_ALL_SPECS_FOR_ITEM_HISTORY
    0x20000000: 'Save After Consume', // SAVE_ON_CONSUME
    0x40000000: 'Loot Container Saves Player State', // CONTAINER_SAVES_PLAYER_DATA
    0x80000000: 'No Void Storage',
};

const itemStaticFlags3 = {
    0x00000001: 'Immediately Trigger On Use Binding Effects', // HANDLE_ON_USE_EFFECT_IMMEDIATELY
    0x00000002: 'Always Display Item Level in Tooltip', // ALWAYS_SHOW_ITEM_LEVEL_IN_TOOLTIP
    0x00000004: 'Display <Random additional stats> In Tooltip', // SHOWS_GENERATION_WITH_RANDOM_STATS
    // official names incomplete
    0x00000008: 'ACTIVATE_ON_EQUIP_EFFECTS_WHEN_TRANSMOGRIFIED',
    0x00000010: 'ENFORCE_TRANSMOG_WITH_CHILD_ITEM',
    0x00000020: 'SCRAPABLE',
    0x00000040: 'BYPASS_REP_REQUIREMENTS_FOR_TRANSMOG',
    0x00000080: 'DISPLAY_ONLY_ON_DEFINED_RACES',
    0x00000100: 'REGULATED_COMMODITY',
    0x00000200: 'CREATE_LOOT_IMMEDIATELY',
    0x00000400: 'GENERATE_LOOT_SPEC_ITEM',
    0x00000800: 'HIDDEN_IN_REWARD_SUMMARIES',
    0x00001000: 'DISALLOW_WHILE_LEVEL_LINKED',
    0x00002000: 'DISALLOW_ENCHANT',
    0x00004000: 'SQUISH_USING_ITEM_LEVEL_AS_PLAYER_LEVEL',
    0x00008000: 'ALWAYS_SHOW_SELL_PRICE_IN_TOOLTIP',
    0x00010000: 'COSMETIC_ITEM',
    0x00020000: 'NO_SPELL_EFFECT_TOOLTIP_PREFIXES',
    0x00040000: 'IGNORE_COSMETIC_COLLECTION_BEHAVIOR',
    0x00080000: 'NPC_ONLY',
    0x00100000: 'NOT_RESTORABLE',
    0x00200000: 'DONT_DISPLAY_AS_CRAFTING_REAGENT',
    0x00400000: 'DISPLAY_REAGENT_QUALITY_AS_CRAFTED_QUALITY',
    0x00800000: 'NO_SALVAGE',
    0x01000000: 'RECRAFTABLE',
    0x02000000: 'CC_TRINKET',
    0x04000000: 'KEEP_THROUGH_FACTION_CHANGE',
    0x08000000: 'NOT_MULTICRAFTABLE',
    0x10000000: 'DONT_REPORT_LOOT_LOG_TO_SELF',
    0x20000000: 'SEND_TELEMETRY_ON_USE'
};

const combatRatingFlags = {
    0x00000001: 'CR_AMPLIFY',
    0x00000002: 'CR_DEFENSE_SKILL',
    0x00000004: 'CR_DODGE',
    0x00000008: 'CR_PARRY',
    0x00000010: 'CR_BLOCK',
    0x00000020: 'CR_HIT_MELEE',
    0x00000040: 'CR_HIT_RANGED',
    0x00000080: 'CR_HIT_SPELL',
    0x00000100: 'CR_CRIT_MELEE',
    0x00000200: 'CR_CRIT_RANGED',
    0x00000400: 'CR_CRIT_SPELL',
    0x00000800: 'CR_CORRUPTION',
    0x00001000: 'CR_CORRUPTION_RESISTANCE',
    0x00002000: 'CR_SPEED',
    0x00004000: 'CR_RESILIENCE_CRIT_TAKEN',
    0x00008000: 'CR_RESILIENCE_PLAYER_DAMAGE',
    0x00010000: 'CR_LIFESTEAL',
    0x00020000: 'CR_HASTE_MELEE',
    0x00040000: 'CR_HASTE_RANGED',
    0x00080000: 'CR_HASTE_SPELL',
    0x00100000: 'CR_AVOIDANCE',
    0x00200000: 'CR_STURDINESS',
    0x00400000: 'CR_UNUSED_7',
    0x00800000: 'CR_EXPERTISE',
    0x01000000: 'CR_ARMOR_PENETRATION',
    0x02000000: 'CR_MASTERY',
    0x04000000: 'CR_PVP_POWER',
    0x08000000: 'CR_CLEAVE',
    0x10000000: 'CR_VERSATILITY_DAMAGE_DONE',
    0x20000000: 'CR_VERSATILITY_HEALING_DONE',
    0x40000000: 'CR_VERSATILITY_DAMAGE_TAKEN',
    0x80000000: 'CR_UNUSED_12'
}

// 2
const ingameBrowserFlags = {
    0x1: 'Deny',
    0x2: 'Navigate',
    0x4: 'Resource Load',
    0x8: 'XHeader',
    0x10: 'Referer',
    0x20: 'Set Cookie',
    0x40: 'Get Cookie',
    0x80: 'Persistent Cookie',
    0x100: 'Navigate External',
    0x200: 'Change Scheme HTTP',
    0x400: 'Change Scheme HTTPS',
    0x800: 'Ticket JS',
    0x1000: 'B.net Auth Challenge',
    0x2000: 'Social Callback',
    0x4000: 'Differentiate Browser',
    0x8000: 'Requires BattleNet Login',
}

// 169
const itemDisplayInfoFlags = {
    0x00000001: 'Emblazoned Tabard (Common)',
    0x00000002: 'No Sheathed Kit During Spell Combat Anims',
    0x00000004: 'Hide Pants and Belt',
    0x00000008: 'Emblazoned Tabard (Rare)',
    0x00000010: 'Emblazoned Tabard (Epic)',
    0x00000020: 'Use Spear Ranged Weapon Attachment',
    0x00000040: 'Inherit Character Animation',
    0x00000080: 'Mirror Animation from Right Shoulder to Left',
    0x00000100: 'Mirror Model When Equipped on Off-Hand',
    0x00000200: 'Disable Tabard Geo (waist only)',
    0x00000400: 'Mirror Model When Equipped on Main-Hand',
    0x00000800: 'Mirror Model When Sheathed (Warglaives)',
    0x00001000: 'Flip Model When Sheathed',
    0x00002000: 'Use Alternate Weapon Trail Endpoints',
    0x00004000: 'Force Sheathed if equipped as weapon',
    0x00008000: 'Don\'t Close Hands',
    0x00010000: 'Force Unsheathed for Spell Combat Anims',
    0x00020000: 'Brewmaster Unsheathe',
    0x00040000: 'Hide Belt Buckle',
    0x00080000: 'No Default Bow String',
    //0x00100000: 'Reserved for Future Patch',
    // incomplete
};

// 185
const groupFinderActivityFlags = {
    0x00000001: 'Show Proving Grounds',
    0x00000002: 'Show in PvE',
    0x00000004: 'Show in PvP',
    0x00000008: 'Segregate Time Zones',
    0x00000010: 'Use PvP Honor Level',
    0x00000020: 'Show Quick Join Toast',
    0x00000040: 'Use Dungeon Role Expectations',
    0x00000080: 'Fuzzy numeric matching',
}

// 186
const groupFinderCategoryFlags = {
    0x00000001: 'Separate Recommended',
    0x00000002: 'Auto Choose Activity',
    0x00000004: 'Prefer Current Area',
    0x00000008: 'Suppress full groups from search results',
    0x00000010: 'Auto-determine max players',
}

// 190
const worldMapAreaFlags = {
    0x00000001: 'Is Placeholder',
    0x00000002: 'Is a Phased World Map',
    0x00000004: 'Default Dungeon Floor is Terrain',
    0x00000008: 'Virtual Continent Map',
    0x00000010: 'Child of a Virtual Continent Map',
    0x00000020: 'Don\'t Display ANY Highlight',
    0x00000040: 'Force Map Overlays to be used',
    0x00000080: 'Don\'t Display Taxi Nodes',
    0x00000100: 'Don\'t Use Default Dungeon Floor (typically Floor 1)',
    0x00000200: 'Default Dungeon Floor is Micro Dungeon',
    0x00000400: 'Orphan Map',
    0x00000800: 'Show Garrison Buildings',
    0x00001000: 'Fallback to Parent Map (e.g. if we don\'t want a map for this terrain phase)',
    0x00002000: 'Don\'t Display Highlight Texture',
    0x00004000: 'Show Task Objective POIs on Map',
    0x00008000: 'HACK: micro dungeon map use parent WorldMapArea for filepath',
    0x00010000: 'No World Positions',
};

// 193
const itemBonusListFlags = {
    0x00000001: 'Disable In Challenge Modes',
    0x00000002: 'Disable in Proving Grounds',
    0x00000004: 'Disable in Arena or Rated Battleground',
    0x00000008: 'Hardcoded: Item Level Delta (DO NOT USE)',
    0x00000010: 'Deprecated Bonus List',
}

// 194
const phaseFlags = {
    0x00000001: 'Read Only',
    0x00000002: 'Internal Phase',
    0x00000004: 'Uses Player Conditions',
    0x00000008: 'Normal',
    0x00000010: 'Cosmetic',
    0x00000020: 'Personal',
    0x00000040: 'Expensive',
    0x00000080: 'Events are Observable',
    0x00000100: 'Uses Preload Conditions',
    0x00000200: 'Unshareable Personal',
    0x00000400: 'Objects are Visible',
};

// 195
const skillLineFlags = {
    0x00000001: 'Always shown in UI',
    0x00000002: 'Never shown in UI',
    0x00000004: 'First tier is self taught',
    0x00000008: 'Granted incrementally by character upgrade',
    0x00000010: 'Automatic rank',
    0x00000020: 'RESERVED for use in 8.0.1',
    0x00000040: '(DEPRECATED) Shows in spell tooltip (even if not in UI)',
    0x00000080: '(DEPRECATED) Appears in Misc Tab of Spellbook',
    0x00000100: '(UNUSED)',
    0x00000200: 'Ignore Category Mods',
    0x00000400: 'Displays as proficiency',
    0x00000800: 'Pets Only',
    0x00001000: 'Unique bitfield (Professions only)',
    0x00002000: 'Racial for the purpose of Paid Race/Faction change',
    0x00004000: 'Progressive skill up',
    0x00008000: 'Racial for the purpose of temporary race change',
};

// 198
const liquidTypeFlags = {
    0x00000001: 'Ripple',
    0x00000002: 'Splash',
    0x00000004: 'Water Walking',
    0x00000008: 'Particles',
    0x00000010: 'Particle Fog',
    0x00000020: 'WMO Fog Control',
    0x00000040: 'No Interior Fog Blend',
    0x00000080: 'Outdoor Ambience Only',
    0x00000100: 'WMO Fog Control (SLIME)',
    0x00000200: 'Force Exterior Lighting',
};

// 105
const mountFlags = {
    0x1: 'Server Only',
    0x2: 'Is Self Mount',
    0x4: 'Exclude from Journal if faction doesn\'t match',
    0x8: 'Allow mounted combat',
    0x10: 'Summon Random: Favor While Underwater',
    0x20: 'Summon Random: Favor While at Water Surface',
    0x40: 'Exclude from Journal if not learned',
    0x80: 'Summon Random: Do NOT Favor When Grounded',
    0x100: 'Show in Spellbook',
    0x200: 'Add to Action Bar on Learn',
    0x400: 'NOT for use as a taxi (non-standard mount anim)'
}

// 199
const mountCapabilityFlags = {
    0x1: 'Is Ground Mount',
    0x2: 'Is Flying Mount',
    0x4: 'Is Surface Swimming Mount',
    0x8: 'Is Underwater Swimming Mount',
    0x10: 'Mount Can Jump',
    0x20: 'Ignore All Area Restrictions'
}

// 201
const emotesFlags = {
    0x00000001: 'Only while standing',
    0x00000002: 'Emote applies to mount',
    0x00000004: 'Not while channeling',
    0x00000008: 'Talk anim talk',
    0x00000010: 'Talk anim question',
    0x00000020: 'Talk anim exclamation',
    0x00000040: 'Talk anim shout',
    0x00000080: 'Not while swimming',
    0x00000100: 'Talk anim laugh',
    0x00000200: 'Ok while sleeping or dead',
    0x00000400: 'Disallow from client',
    0x00000800: 'Not while casting',
    0x00001000: 'Movement ends',
    0x00002000: 'Interrupt on interact',
    0x00004000: 'Only while still',
    0x00008000: 'Not while flying',
    0x00010000: 'Not while mounted',
    0x00020000: 'Animation suppressed by movement',
    0x00040000: 'Combat ends (initiating an attack)',
    0x00080000: 'Don\'t allow animation to fallback',
    0x00100000: 'Don\'t log chat on server',
    0x00200000: 'Not while in vehicle',
    0x00400000: 'Not while in combat (has threat)',
    0x00800000: 'Do not throttle player emote frequency',
    0x01000000: 'Suppress all animation sounds',
};

// 203
const wmoAreaTableFlags = {
    0x1: 'Render Minimap',
    0x2: 'Force Indoors',
    0x4: 'Force Outdoors',
    0x8: 'Generate Single Exterior Map',
    0x10: 'Stormwind',
    0x20: 'Chunk uses terrain for Minimap',
    0x40: 'Ignore for Minimap and Effects',
    0x80: 'Ignore Fatigue'
}

// 220
const chatChannelsFlags = {
    0x00000001: 'Auto-Join',
    0x00000002: 'Zone-Based',
    0x00000004: 'Read-Only',
    0x00000008: 'Allow Item Links',
    0x00000010: 'Only in Cities',
    0x00000020: 'Linked Channel',
    0x00010000: 'Zone Attack Alerts',
    0x00020000: 'Guild Recruitment',
    0x00040000: 'Looking for Group',
    0x00080000: 'Global for Tournament',
    0x00100000: 'Disable Raid Icons',
    // incomplete
};

// 223
const spellShapeshiftFormFlags = {
    0x00000001: 'Stance',
    0x00000002: 'Not Toggleable',
    0x00000004: 'Persist On Death',
    0x00000008: 'Can Interact NPC',
    0x00000010: 'Don\'t Use Weapon',
    0x00000020: 'Agility Attack Bonus',
    0x00000040: 'Can Use Equipped Items',
    0x00000080: 'Can Use Items',
    0x00000100: 'Don\'t Auto-Unshift',
    0x00000200: 'Considered Dead',
    0x00000400: 'Can Only Cast Shapeshift Spells',
    0x00000800: 'Stance Cancels At Flightmaster',
    0x00001000: 'No Emote Sounds',
    0x00002000: 'No Trigger Teleport',
    0x00004000: 'Cannot change equipped items',
    0x00008000: 'Resummon Pets on Unshift',
    0x00010000: 'Cannot Use Game Objects',
};

// 236
const creatureModelDataFlags = {
    0x00000001: 'No Footprint Particles',
    0x00000002: 'No Breath Particles',
    0x00000004: 'Is Player Model',
    0x00000010: 'No Attached Weapons',
    0x00000020: 'No Footprint Trail Textures',
    0x00000040: 'Disable Highlight',
    0x00000080: 'Can Mount while Transformed as this',
    0x00000100: 'Disable Scale Interpolation',
    0x00000200: 'Force Projected Tex. (EXPENSIVE)',
    0x00000400: 'Can Jump In Place As Mount',
    0x00000800: 'AI cannot use walk backwards anim',
    0x00001000: 'Ignore SpineLow for SplitBody',
    0x00002000: 'Ignore Head for SplitBody',
    0x00004000: 'Ignore SpineLow for SplitBody when Flying',
    0x00008000: 'Ignore Head for SplitBody when Flying',
    0x00010000: 'Use \'wheel\' animation on unit wheel bones',
    0x00020000: 'Is HD Model',
    0x00040000: 'Suppress Emitters on Low Settings',
};

// 251
const broadcastTextFlags = {
    0x00000001: 'Enable Boss Emote Warning Sound',
    0x00000002: 'Display as Subtitle in Letterbox',
    0x00000004: 'Available to Client-side Actors',
    0x00000008: 'Hide Sender in Letterbox Subtitle',
    0x00000010: 'Hide from Chat Log',
    // incomplete
};

// 255
const creatureImmunitiesFlags = {
    0x00000001: 'Area Effect Immune',
    0x00000002: 'Chain Effect Immune',
    0x00000004: 'Available on Client',
};

// 256
const battlemasterListFlags = {
    0x00000001: 'Internal Only',
    0x00000002: 'Rated Only',
    0x00000004: 'OBSOLETE - Do Not List',
    0x00000008: 'Show in War-Games',
    0x00000010: 'Show in PvP Battleground List',
    0x00000020: 'Is Brawl',
    // incomplete
};

// 258
const artifactPowerFlags = {
    0x00000001: 'Is Gold Medal',
    0x00000002: 'Is Start Power',
    0x00000004: 'Is Endgame Power',
    0x00000008: 'Is Meta Power',
    0x00000010: 'One Free Rank',
    0x00000020: 'Max Rank Variable',
    0x00000040: 'Single Rank Tooltip',
};

// 265
const animationDataFlags0 = {
    0x00000001: 'Animation used for Emotes',
    0x00000002: 'Animation used for Spells',
    0x00000008: 'Hide Weapons',
    0x00000010: 'Fallback Plays Backwards',
    0x00000020: 'Fallback Holds Last Frame',
    0x00000040: 'Load Only on Demand',
    0x00000080: 'Fallback to Variation Zero',
    0x00000100: 'Never allow spine/head twist',
    0x00000200: 'Force pitch and roll to track ground surface',
    0x00000400: 'Animation is Unapproved - See Gameplay Programmer',
    0x00000800: 'Move Weapons to Sheath',
    0x00001000: 'Move Melee Weapons to Hand',
    0x00002000: 'Scale to Ground',
    0x00004000: 'Scale to Ground Rev',
    0x00008000: 'Scale to Ground Always',
    0x00010000: 'Is Split Body Behavior',
    0x00020000: 'Is Bow Weapon Behavior',
    0x00040000: 'Is Rifle Weapon Behavior',
    0x00080000: 'Is Thrown Weapon Behavior',
    0x00100000: 'Is Death Behavior',
    0x00200000: 'Is Melee Combat Behavior',
    0x00400000: 'Is Special Combat Behavior',
    0x00800000: 'Is Wound Behavior',
    0x01000000: 'Is Unarmed Behavior',
    0x02000000: 'Use mounted name plate position',
    0x04000000: 'Flip Spear Weapons 180 degrees',
    0x08000000: 'Animation is required for a PetBattle pet',
    0x10000000: 'Animation is optional for a PetBattle pet',
    0x00000004: 'Is Pierce Anim',
    0x20000000: 'Spell Combat Behavior',
    0x40000000: 'Brewmaster Sheathe',
    0x80000000: 'Meta Animation',
};

// 266
const animKitConfigConfigFlags = {
    0x00000001: 'DEPRECATED: Force no animation blend in (use Blend Time Override : In Min&Max)',
    0x00000002: 'DEPRECATED: Force no animation blend out (use Blend Time Override: Out Min&Max)',
    0x00000004: 'Do not use: Advanced: Play on secondary track (disrupts blending)',
    0x00000008: 'Instead of playing a fallback animation, skip segment',
    0x00000010: 'Instead of playing a fallback animation, don\'t play kit at all',
    0x00000020: 'If model is missing a desired bone set, use parent or ancestor bone set',
    0x00000040: 'If model is missing a desired bone set, skip segment',
    0x00000080: 'If model is missing a desired bone set, don\'t play kit at all',
    0x00000100: 'Play on all children and descendant bone sets at same priority',
    0x00000200: 'If any bone set is suppressed by a higher priority, skip segment',
    0x00000400: 'If all bone sets are suppressed by a higher priority, skip segment',
    0x00000800: 'If any bone set is suppressed by a higher priority, stop kit',
    0x00001000: 'If all bone sets are suppressed by a higher priority, stop kit',
    0x00002000: 'When no longer suppressed by a higher priority, restart current segment',
    0x00004000: 'Advanced: Play segment even if bone set is missing (for animation mirroring)',
    0x00008000: 'Advanced: Blocks secondary animation when active',
    0x00010000: 'Advanced: Don\'t restart segment if the animation is unchanged following a set',
    0x00020000: 'If any bone set is suppressed by a higher priority, suppress segment',
    0x00040000: 'Instead of falling back to the STAND animation, skip segment',
    0x00080000: 'Instead of falling back to the STAND animation, don\'t play kit at all',
    0x00100000: 'When no longer suppressed by a higher priority, restart current kit',
    0x00200000: 'Suppress *all* sounds originating from animation events',
    0x00400000: 'Suppress sounds originating from *combat* animation events',
    0x00800000: 'Force Sheathed',
    0x01000000: 'Force Unsheathed',
    0x02000000: 'Maintain Initial Facing',
    0x04000000: 'Aim along unit\'s Smooth Facing',
    0x08000000: 'Use Mod Cast Speed',
    0x10000000: 'Aim Left Arm',
    0x20000000: 'Aim Right Arm',
    0x40000000: 'Disable Arm Aiming',
    0x80000000: 'Speed up based on Percentage Complete',
};

// 271
const criteriaFlags = {
    0x00000001: 'Fail Achievement',
    0x00000002: 'Reset on Start',
    0x00000004: 'Server Only',
    0x00000008: 'Always Save to DB (Use with Caution)',
    0x00000010: 'Allow criteria to be decremented',
    0x00000020: 'Is For Quest',
};

// 277
const creatureStaticFlags0 = {
    0x00000001: 'Mountable',
    0x00000002: 'No XP',
    0x00000004: 'No Loot',
    0x00000008: 'Unkillable',
    0x00000010: 'Tameable',
    0x00000020: 'ImmunePC',
    0x00000040: 'ImmuneNPC',
    0x00000080: 'CanWieldLoot',
    0x00000100: 'Sessile',
    0x00000200: 'Uninteractible',
    0x00000400: 'No Automatic Regen',
    0x00000800: 'Despawn Instantly',
    0x00001000: 'CorpseRaid',
    0x00002000: 'Creator Loot',
    0x00004000: 'No Defense',
    0x00008000: 'No Spell Defense',
    0x00010000: 'Raid Boss Mob',
    0x00020000: 'Combat Ping',
    0x00040000: 'Aquatic (aka Water Only)',
    0x00080000: 'Amphibious',
    0x00100000: 'No Melee (Flee)',
    0x00200000: 'Visible to Ghosts',
    0x00400000: 'PvP Enabling',
    0x00800000: 'Do Not Play Wound Anim',
    0x01000000: 'No Faction Tooltip',
    0x02000000: 'Ignore Combat',
    0x04000000: 'Only attack targets that are PvP enabling',
    0x08000000: 'Calls Guards',
    0x10000000: 'Can Swim',
    0x20000000: 'Floating <Dont Use>',
    0x40000000: 'More Audible: Caution, Expensive',
    0x80000000: 'Large (AOI): Caution, Expensive'
}

const creatureStaticFlags1 = {
    0x00000001: 'No Pet Scaling',
    0x00000002: 'Force Raid Combat',
    0x00000004: 'Lock Tappers To Raid On Death',
    0x00000008: 'No Harmful Vertex Coloring',
    0x00000010: 'No Crushing Blows',
    0x00000020: 'No Owner Threat',
    0x00000040: 'No Wounded Slowdown',
    0x00000080: 'Use Creator Bonuses',
    0x00000100: 'Ignore Feign Death',
    0x00000200: 'Ignore Sanctuary',
    0x00000400: 'Action Triggers While Charmed',
    0x00000800: 'Interact While Dead',
    0x00001000: 'No Interrupt School Cooldown',
    0x00002000: 'Return soul shard to master of pet',
    0x00004000: 'Skin With Herbalism',
    0x00008000: 'Skin With Mining',
    0x00010000: 'Alert Content Team on Death',
    0x00020000: 'Alert Content Team at 90% Health',
    0x00040000: 'Allow Mounted Combat',
    0x00080000: 'PvP Enabling OOC',
    0x00100000: 'No Death Message',
    0x00200000: 'Ignore Pathing Failure',
    0x00400000: 'Full Spell List',
    0x00800000: 'Doesnt Reduce Reputation for raids',
    0x01000000: 'Ignore Misdirection',
    0x02000000: 'Hide Body',
    0x04000000: 'Spawn Defensive',
    0x08000000: 'Server Only',
    0x10000000: 'No Collision',
    0x20000000: 'Player Can Heal/Buff',
    0x40000000: 'No Skill Gains',
    0x80000000: 'No Pet Bar',
}

const creatureStaticFlags2 = {
    0x00000001: 'No Damage History',
    0x00000002: 'Dont PvP Enable Owner',
    0x00000004: 'Dont Fade In',
    0x00000008: 'Non-Unique In Combat Log',
    0x00000010: 'Skin With Engineering',
    0x00000020: 'No Aggro On Leash',
    0x00000040: 'No Friendly Area Auras',
    0x00000080: 'Extended Corpse Duration',
    0x00000100: 'Cant Swim',
    0x00000200: 'Tameable (Exotic)',
    0x00000400: 'Gigantic (AOI): Caution, Expensive',
    0x00000800: 'Infinite (AOI): Caution, Expensive',
    0x00001000: 'Cannot Penetrate Water',
    0x00002000: 'No Name Plate',
    0x00004000: 'Checks Liquids: Caution, Expensive',
    0x00008000: 'No Threat Feedback',
    0x00010000: 'Use Model Collision Size (TALK TO A PROGRAMMER FIRST)',
    0x00020000: 'Attacker Ignores Facing',
    0x00040000: 'Allow Interaction While in Combat',
    0x00080000: 'Spell Click for Party Only',
    0x00100000: 'Faction Leader',
    0x00200000: 'Immune to Player Buffs',
    0x00400000: 'Collide With Missiles',
    0x00800000: 'Do Not Tap (Credit to threat list)',
    0x01000000: 'Disable Dodge, Parry and Block Animations',
    0x02000000: 'Cannot Turn',
    0x04000000: 'Enemy Check Ignores Line of Sight',
    0x08000000: 'Forever Corpse Duration (7 days)',
    0x10000000: 'Pets attack with 3d pathing (Kologarn)',
    0x20000000: 'LinkAll flag (TALK TO A PROGRAMMER FIRST)',
    0x40000000: 'AI Can Auto Takeoff in Combat',
    0x80000000: 'AI Can Auto Land in Combat',
}

const creatureStaticFlags3 = {
    0x00000001: 'No Birth Anim',
    0x00000002: 'Treat as Player for Diminishing Returns',
    0x00000004: 'Treat as Player for PvP Debuff Duration',
    0x00000008: 'Only Display Gossip for Summoner',
    0x00000010: 'No Death Scream',
    0x00000020: 'Can be Healed by Enemies',
    0x00000040: 'Deals triple damage to PC controlled pets',
    0x00000080: 'No NPC damage below 85%',
    0x00000100: 'Obeys Taunt Diminishing Returns',
    0x00000200: 'No Melee (Approach)',
    0x00000400: 'Update Creature Record when instance changes difficulty',
    0x00000800: 'Cannot Daze (Combat Stun)',
    0x00001000: 'Flat Honor Award',
    0x00002000: 'Other objects can ignore line of sight requirements when casting spells on me',
    0x00004000: 'Give quest kill credit while offline',
    0x00008000: 'Treat as Raid Unit For Helpful Spells (Instances ONLY)',
    0x00010000: 'Dont reposition because melee target is too close',
    0x00020000: 'Pet/Guardian AI Dont Go Behind Target',
    0x00040000: '5 Minute loot roll timer',
    0x00080000: 'Force Gossip',
    0x00100000: 'Dont reposition with friends in combat',
    0x00200000: 'Manual Sheathing control',
    0x00400000: 'Attacker Ignores Minimum Ranges',
    0x00800000: 'Suppress Instance Wide Release in Combat',
    0x01000000: 'AI will only swim if target swims',
    0x02000000: 'Dont generate combat log when engaged with NPCs',
    0x04000000: 'Allow NPC Combat while Uninteractible',
    0x08000000: 'Prefer NPCs When Searching For Enemies',
    0x10000000: 'Only Generate Initial Threat',
    0x20000000: 'Doesnt change target on right click',
    0x40000000: 'Hide name in world frame',
    0x80000000: 'Quest Boss'
}

const creatureStaticFlags4 = {
    0x00000001: 'Untargetable By Client',
    0x00000002: 'Force Self Mounting',
    0x00000004: 'Uninteractible If Hostile',
    0x00000008: 'Disables XP Award',
    0x00000010: 'Disable AI prediction',
    0x00000020: 'No LeaveCombat State Restore',
    0x00000040: 'Bypass Interact Interrupts',
    0x00000080: '240 Degree Back Arc',
    0x00000100: 'Interact while Hostile',
    0x00000200: 'Dont Dismiss On Flying Mount',
    0x00000400: 'Predictive Power Regen',
    0x00000800: 'Hide Level Info In Tooltip',
    0x00001000: 'Hide Health Bar Under Tooltip',
    0x00002000: 'Suppress highlight when targeted or moused over',
    0x00004000: 'AI Prefer pathable targets',
    0x00008000: 'Frequent Area Trigger Checks  (EXPENSIVE/TALK TO A PROGRAMMER FIRST)',
    0x00010000: 'Assign Kill Credit to Encounter List',
    0x00020000: 'Never Evade',
    0x00040000: 'AI Cant path on Steep Slopes',
    0x00080000: 'AI Ignore Los To Melee Target',
    0x00100000: 'Never display emote or chat text in a chat bubble',
    0x00200000: 'AI Pets close in on unpathable target',
    0x00400000: 'Pet/Guardian AI Dont Go Behind Me (use on target)',
    0x00800000: 'No Death Thud',
    0x01000000: 'Client Local Creature',
    0x02000000: 'Can drop loot while in a challenge mode instance',
    0x04000000: 'Has Safe Location',
    0x08000000: 'No Health Regen',
    0x10000000: 'No Power Regen',
    0x20000000: 'No Pet Unit Frame',
    0x40000000: 'No Interact On Left Click',
    0x80000000: 'Give criteria kill credit when charmed',
}

const creatureStaticFlags5 = {
    0x00000001: 'Do not auto-resummon this companion creature',
    0x00000002: 'Smooth Phasing: Replace visible unit if available (ASK PROGRAMMER FIRST)',
    0x00000004: 'Ignore the realm coalescing hiding code (always show)',
    0x00000008: 'Taps to Faction',
    0x00000010: 'Only QuestGiver for Summoner',
    0x00000020: 'AI Combat Return Precise',
    0x00000040: 'Home realm only loot',
    0x00000080: 'No Interact Response',
    0x00000100: 'No Initial Power',
    0x00000200: 'Dont Cancel Channel On Master Mounting',
    0x00000400: 'Can Toggle between Death and Personal Loot',
    0x00000800: 'Always, ALWAYS tries to stand right on top of his move to target. ALWAYS!!',
    0x00001000: 'Unconscious on Death',
    0x00002000: 'Dont report to local defense channel on death',
    0x00004000: 'Prefer unengaged monsters when picking a target',
    0x00008000: 'Use PVP power and resilience when players attack this creature',
    0x00010000: 'Dont clear debuffs on leave combat',
    0x00020000: 'Personal loot has full security (guaranteed push/mail delivery)',
    0x00040000: 'Triple Spell Visuals',
    0x00080000: 'Use Garrison Owner Level',
    0x00100000: 'Immediate AOI Update On Spawn',
    0x00200000: 'UI Can Get Position',
    0x00400000: 'Seamless Transfer Prohibited',
    0x00800000: 'Always use Group Loot Method',
    0x01000000: 'No Boss Kill Banner',
    0x02000000: 'Force Triggering Player Loot Only',
    0x04000000: 'Show Boss Frame While Uninteractable',
    0x08000000: 'Scales to Player Level',
    0x10000000: 'AI dont leave melee for ranged when target gets rooted',
    0x20000000: 'Dont Use Combat Reach For Chaining',
    0x40000000: 'Do not play procedural wound anim',
    0x80000000: 'Apply procedural wound anim to Base'
}

const creatureStaticFlags6 = {
    0x00000001: 'Important NPC',
    0x00000002: 'Important Quest NPC',
    0x00000004: 'Large Nameplate',
    0x00000008: 'Trivial Pet (Ignored by helpful AOEs)',
    0x00000010: 'AI Enemies Dont backup when I get rooted',
    0x00000020: 'No Automatic Combat Anchor',
    0x00000040: 'Only Targetable By Creator',
    0x00000080: '8.0.1 Flag - Treat as Player for IsPlayerControlled()',
    0x00000100: '8.0.1 Flag - Generate No Threat or Damage',
    0x00000200: '8.0.1 Flag - Interact Only on Quest',
    0x00000400: 'Disable Kill Credit for Offline Players',
    0x00000800: 'AI Additional Pathing',
    0x00001000: 'Force Close In On Path Fail Behavior',
    0x00002000: 'Use 2D Chasing Calculation',
    0x00004000: 'Use Fast Classic Heartbeat'
}

// 291
const lightSkyboxFlags = {
    0x00000001: 'Full day Skybox',
    0x00000002: 'Combine Procedural And Skybox',
    0x00000004: 'Procedural Fog Color Blend',
    0x00000008: 'Force Sun-shafts',
    0x00000010: 'Disable use Sun Fog Color',
};

// 292
const interruptFlags = {
    0x00000001: 'Movement',
    0x00000002: 'Dmg Pushback(Player)',
    0x00000004: 'Stun',
    0x00000008: 'Combat',
    0x00000010: 'Dmg Cancels(Player)',
    0x00000020: 'Melee Combat',
    0x00000040: 'Immunity',
    0x00000080: 'Damage Absorb',
    0x00000100: 'Zero Damage Cancels',
    0x00000200: 'Damage Pushback',
    0x00000400: 'Damage Cancels',
};

// 327
const powerTypeFlags = {
    0x00000001: 'Stop Regen While Casting',
    0x00000002: 'Use Regen Interrupt',
    0x00000004: 'Clear Fractional Power on Leave Combat',
    0x00000008: 'Fill Fractional Power on Energize',
    0x00000010: 'No Client Prediction',
    0x00000020: 'Units Use Default Power on Init',
    0x00000040: 'Not Set to Default on Resurrect',
    0x00000080: 'Is Used by NPCs',
    0x00000100: 'Cost Hidden on Tooltip',
    0x00000200: 'Continue Regen While Fatigued',
    0x00000400: 'Regen Affected by Haste',
    0x00000800: 'Set to Default After Regen Interrupt',
    0x00001000: 'Set to Max on level up',
    0x00002000: 'Set to Max on initial login',
    0x00004000: 'Allow Cost Mods For Players',
};

// 331
const itemSubClassFlags = {
    0x00000001: 'Weapon Can Parry',
    0x00000002: 'Weapon Set Finger Sequence (unused)',
    0x00000004: 'Weapon Is Unarmed (unused)',
    0x00000008: 'Weapon Is Rifle (unused)',
    0x00000010: 'Weapon Is Thrown (unused)',
    0x00000020: 'Weapon Is Right Hand Ranged (unused)',
    0x00000040: 'Quiver Not Required (unused)',
    0x00000080: 'Ranged (unused)',
    0x00000100: 'Ranged No Attack Power (unused)',
    0x00000200: 'Uses Inv Type',
};

// 332
const itemSubClassDisplayFlags = {
    0x1: 'Hide SubClass In Tooltips',
    0x2: 'Hide SubClass In Auction',
    0x3: 'Show Item Count',
}

// 346
const chrClassesFlags = {
    0x00000001: 'Use Loincloth',
    0x00000002: 'Player Class',
    0x00000004: 'Display Pet',
    0x00000010: 'Can Wear Scaling-Stat Mail',
    0x00000020: 'Can Wear Scaling-Stat Plate',
    0x00000040: 'Bind Starting Area',
    0x00000080: 'Pet Bar Initially Hidden',
    0x00000100: 'Send Stable at Login',
    0x00000200: 'Monk-Style Unarmed',
    0x00000400: 'Requires Stance',
    0x00000800: 'Disallow Boost',
    0x00001000: 'Creature Class',
    0x00002000: 'Early Faction Choice',
    0x00004000: 'Hero Class',
    0x00008000: 'Can Dual Wield',
    0x00010000: 'Disabled',
    0x00020000: 'Is Melee',
    0x00040000: 'Is Melee With Specialization',
    0x00080000: 'Hero Class Ignore Requirements',
    0x00100000: 'No Azerite Powers'
};

// 351
const spellItemEnchantmentFlags = {
    0x00000001: 'Soulbound',
    0x00000002: 'Do not log',
    0x00000004: 'Mainhand Only',
    0x00000008: 'Allow Entering Arena',
    0x00000010: 'Do Not Save To DB',
    0x00000020: 'Scale As A Gem',
    0x00000040: 'Disable In Challenge Modes',
    0x00000080: 'Disable in Proving Grounds',
    0x00000100: 'Allow Transmog',
    0x00000200: 'Hide Until Collected',
};

// 352
const scenarioFlags = {
    0x00000001: 'Use Challenge Mode Display (deprecated)',
    0x00000002: 'Suppress Scenario Stage Number Text',
    0x00000004: 'Use Proving Grounds Display (deprecated)',
    0x00000008: 'Use Dungeon Display (deprecated)',
    0x00000010: 'Grants Guild Dungeon Credit',
    0x00000020: 'IS_OUTDOOR'
};

// 365
const materialFlags = {
    0x00000001: 'Is Metal',
    0x00000002: 'Is Plate',
    0x00000004: 'Is Chain',
}

// 407
const procTypeMask0 = {
    0x00000001: 'Heartbeat',
    0x00000002: 'Kill',
    0x00000004: 'Deal Melee Swing',
    0x00000008: 'Take Melee Swing',
    0x00000010: 'Deal Melee Ability',
    0x00000020: 'Take Melee Ability',
    0x00000040: 'Deal Ranged Attack',
    0x00000080: 'Take Ranged Attack',
    0x00000100: 'Deal Ranged Ability',
    0x00000200: 'Take Ranged Ability',
    0x00000400: 'Deal Helpful Ability',
    0x00000800: 'Take Helpful Ability',
    0x00001000: 'Deal Harmful Ability',
    0x00002000: 'Take Harmful Ability',
    0x00004000: 'Deal Helpful Spell',
    0x00008000: 'Take Helpful Spell',
    0x00010000: 'Deal Harmful Spell',
    0x00020000: 'Take Harmful Spell',
    0x00040000: 'Deal Harmful Periodic',
    0x00080000: 'Take Harmful Periodic',
    0x00100000: 'Take Any Damage - DO NOT USE',
    0x00200000: 'Deal Helpful Periodic',
    0x00400000: 'Main Hand Weapon Swing',
    0x00800000: 'Off Hand Weapon Swing',
    0x01000000: 'Death',
    0x02000000: 'Jump',
    0x04000000: 'Proc Clone Spell',
    0x08000000: 'Enter Combat',
    0x10000000: 'Encounter Start',
    0x20000000: 'Cast Ended',
    0x40000000: 'Looted',
    0x80000000: 'Take Helpful Periodic',
}

const procTypeMask1 = {
    0x00000001: 'Target Dies',
    0x00000002: 'Knockback',
    0x00000004: 'Cast Successful',
}

// 436
const spellCategoryFlags = {
    0x00000001: 'Cooldown modifies item',
    0x00000002: 'Cooldown is global',
    0x00000004: 'Cooldown event on leave combat',
    0x00000008: 'Cooldown in days',
    0x00000010: 'Reset charges upon ending encounter',
    0x00000020: 'Reset cooldown upon ending encounter',
    0x00000040: 'Ignore for Mod Time Rate',
}

// 546
const stationeryFlags = {
    0x00000001: 'At Mailbox',
    0x00000002: 'Customer Support',
};

// 556
const uiModelSceneActorFlags = {
    0x00000001: 'DEPRECATED (was normalize scale)',
    0x00000002: 'Use Model\'s X center as X origin',
    0x00000004: 'Use Model\'s Y center as Y origin',
    0x00000008: 'Use Model\'s Z center as Z origin',
};

// 562
const journalInstanceFlags = {
    0x00000001: 'Timewalker Available',
    0x00000002: 'Hide User-Selectable Difficulty',
};

// 565
const transmogSetFlags = {
    0x00000001: 'Not in Transmog Set UI',
    0x00000002: 'Hidden Until Collected',
    0x00000004: 'Alliance Only',
    0x00000008: 'Horde Only',
    0x00000010: 'PVP Set',
};

// 566
const transmogSetItemFlags = {
    0x00000001: 'Primary in Slot',
    0x00000002: 'Auto-Fill Source',
};

// 573
const creatureDisplayInfoFlags = {
    0x00000001: 'No Shadow Blob',
    0x00000002: 'Permanent Visual Kit Persists When Dead',
    0x00000004: 'Don\'t change move anims based on scale',
    0x00000008: 'Override Combat Reach',
    0x00000010: 'Override Melee Range',
    0x00000020: 'No Fuzzy Hit',
};

// 603
const dungeonEncounterFlags = {
    0x00000001: 'Sticky News',
    0x00000002: 'Guild News',
    0x00000004: 'Lock players to raid upon successful completion',
    0x00000008: 'Auto end encounter on leave combat (in failure)',
    0x00000010: 'Cosmetic only',
    0x00000020: 'Debug Log',
    0x00000040: 'Hide until completed',
    0x00000080: 'Encounter must be manually started',
};

// 681
const spellEffectEffectAttributes = {
    0x00000001: 'No Immunity',
    0x00000002: 'Position is facing relative',
    0x00000004: 'Jump Charge Unit Melee Range',
    0x00000008: 'Jump Charge Unit Strict Path Check',
    0x00000010: 'Exclude Own Party',
    0x00000020: 'Always AOE Line of Sight',
    0x00000040: 'Suppress Points Stacking',
    0x00000080: 'Chain from Initial Target',
    0x00000100: 'Uncontrolled No Backwards',
    0x00000200: 'Aura Points Stack',
    0x00000400: 'No Copy Damage Interrupts or Procs',
    0x00000800: 'Add Target (Dest) Combat Reach to AOE',
    0x00001000: 'Is Harmful',
    0x00002000: 'Force Scale to Override Camera Min Height',
    0x00004000: 'Players Only',
    0x00008000: 'Compute Points Only At Cast Time',
    0x00010000: 'Enforce Line Of Sight To Chain Targets',
    0x00020000: 'Area Effects Use Target Radius',
    0x00040000: 'Teleport With Vehicle (during map transfer)',
    0x00080000: 'Scale Points By Challenge Mode Damage Scaler',
    0x00100000: 'Don\'t Fail Spell On Targeting Failure',
    // incomplete
};

// 693
const scenarioStepFlags = {
    0x00000001: 'Bonus Objective',
    0x00000002: 'Heroic Bonus Objective only',
    0x00000004: 'Criteria triggers only affect this step',
    0x00000008: 'FLAG_DEFAULT_MULTI_STEP'
};

// 738
const chrRacesFlags = {
    0x1: 'NPC Only',
    0x2: 'Do Not Component Feet',
    0x4: 'Can Mount',
    0x8: 'Has Bald',
    0x10: 'Bind to Starting Area',
    0x20: 'Alternate Form',
    0x40: 'Can Mount Self',
    0x80: 'Force to HD model if available',
    0x100: 'Exalted With All Vendors',
    0x200: 'Not Selectable',
    0x400: 'Reputation Bonus',
    0x800: 'Use Loincloth',
    0x1000: 'Rest Bonus',
    0x2000: 'No Start Kits',
    0x4000: 'No Starting Weapon',
    0x8000: 'Dont redeem account licenses',
    0x10000: 'Skin Variation Is Hair Color',
    0x20000: 'Use Pandaren Ring for componenting texture',
    0x40000: 'Ignore for asset manifest component info parsing',
    0x80000: 'Is Allied Race',
    0x100000: 'Void Vendor Discount (transmog/void storage)',
    0x200000: 'DAMM Component - No Male Generation (Tools Only)',
    0x400000: 'DAMM Component - No Female Generation (Tools Only)',
    0x800000: 'No Associated Faction Reputation in Race Change',
    0x1000000: 'Internal Only (in development)',
    0x2000000: 'Start in Alternate Form'
}

// 916
const waypointNodeFlags = {
    0x00000001: 'Inactive Node',
    0x00000002: 'No Autoconnect',
    0x00000004: 'No Cost Autoconnect',
    0x00000008: 'Player Location',
    0x00000010: 'Route on Same Continent',
};

// 917
const waypointEdgeFlags = {
    0x00000001: 'Inactive Edge',
    0x00000002: 'Bidirectional',
};

// 983
const playerConditionFlags = {
    0x00000001: 'Client executable',
    0x00000002: 'Check achievements on all chars',
    0x00000004: 'Compare power to max',
    0x00000008: 'Invert',
    0x00000010: 'Is at max expansion level',
    0x00000020: 'Within or above record',
    0x00000040: 'Use effective level',
    0x00000080: 'Invert Content Tuning',
    0x00000100: 'Disabled',
    0x00000200: 'Invert Modifier Tree',
    0x00000400: 'Not Recently Transferred',
}

// 934
const friendshipReputationFlags = {
    0x00000001: 'No FX on Reaction Change',
    0x00000002: 'No Log Text on Rep Gain',
    0x00000004: 'No Log Text on Reaction Change',
    0x00000008: 'Show Rep Gain and Reaction Change for Hidden Faction',
    0x00000010: 'No Rep Gain Modifiers',
    0x00000020: 'Reverse color'
};

// 985
const garrSiteFlags = {
    0: 'Top Level Only Site',
}

// 986
const garrPlotFlags = {
    0x1: 'Required for Garrison Upgrade'
}

// 991
const garrAbilityEffectFlags =
{
    0x1: 'Not Beneficial'
}

// 994
const garrBuildingFlags = {
    0x1: 'Requires Blueprint',
    0x2: 'Ignore Storehouse'
}

// 995
const garrFollowerFlags = {
    0x01: 'Unique Follower',
    0x02: 'No Automatic Spawning',
    0x04: 'Internal Only',
    0x08: 'Killed By Always Fail Missions',
    0x10: 'Hidden Unless Collected',
}

// 1021
const questObjectiveFlags = {
    0x01: 'Track on Minimap', // TRACK_ON_MINIMAP
    0x02: 'Sequenced', // SEQUENCED
    0x04: 'Optional', // OPTIONAL
    0x08: 'Hidden', // HIDDEN
    0x10: 'Hide Credit Msg', // HIDE_CREDIT_MSG
    0x20: 'Preserve Quest Items', // PRESERVE_QUEST_ITEMS
    0x40: 'Progress Bar Objective', // PROGRESS_BAR_SUB_TASK
    0x80: 'Kill Players (Same Faction)', // KILL_PLAYERS_SAME_FACTION
    0x100: 'NO_SHARE_PROGRESS',
    0x200: 'IGNORE_SOULBIND_ITEMS'
}

// 1035
const garrAutoSpellFlags = {
    0x1: 'No Initial Cast',
}

// 1036
const vehiclePOITypeFlags = {
    0x1: 'Minimap Icon Draws Below Player Blips',
}

// 1038
const chrModelFlags = {
    0x001: 'Do Not Component Feet',
    0x002: 'Has Bald',
    0x004: 'Use Loincloth',
    0x008: 'Skin Variation is Hair Color',
    0x010: 'Use Pandaren Ring for componenting texture',
    0x020: 'Is Creature Component Style',
    0x040: 'Use Item While Creature Style',
    0x080: 'Hide Back Objects'
}

// 1039
const damageClass = {
    0x01: 'Physical',
    0x02: 'Holy',
    0x04: 'Fire',
    0x08: 'Nature',
    0x10: 'Frost',
    0x20: 'Shadow',
    0x40: 'Arcane'
}

// 1041
const battlePetStateFlags = {
    0x00000001: 'Swap Out Lock',
    0x00000002: 'Turn Lock',
    0x00000004: 'Speed Bonus',
    0x00000008: 'Client',
    0x00000010: 'Max Health Bonus',
    0x00000020: 'Stamina',
    0x00000040: 'Quality does not effect',
    0x00000080: 'Dynamic Scaling(Level and BreedQuality)',
    0x00000100: 'Power',
    0x00000200: 'Pct Speed Multiplier',
    0x00000400: 'Swap In Lock',
    0x00000800: 'Server Only',
}

// 1067
const chrCustomizationCategoryFlags = {
    0x1: 'Undress Model',
}

// 1103
const optionalReagentItemFlag = {
    0x1: 'TooltipShowsAsStatModifications'
}

const TransmogIllisionFlags = { // sic; from transmogconstantsdocumentation.lua 9.1.0.38312
    0x00000001: 'HideUntilCollected',
    0x00000002: 'PlayerConditionGrantsOnLogin',
};
const TransmogIllusionFlags = TransmogIllisionFlags;

const ScriptedAnimationFlags = { // from scriptedanimationsdocumentation.lua 9.1.0.38312
    0x00000001: 'UseTargetAsSource',
};

const BattlePetAbilityFlag = {
    0x00000001: 'DisplayAsHostileDebuff',
    0x00000002: 'HideStrongWeakHints',
    0x00000004: 'Passive',
    0x00000008: 'ServerOnlyAura',
    0x00000010: 'ShowCast',
    0x00000020: 'StartOnCooldown',
}

const BattlePetSpeciesFlags = {
    0x1: 'NoRename',
    0x2: 'WellKnown',
    0x4: 'NotAcccountwide',
    0x8: 'Capturable',
    0x10: 'NotTradable',
    0x20: 'HideFromJournal',
    0x40: 'LegacyAccountUnique',
    0x80: 'CantBattle',
    0x100: 'HordeOnly',
    0x200: 'AllianceOnly',
    0x400: 'Boss',
    0x800: 'RandomDisplay',
    0x1000: 'NoLicenseRequired',
    0x2000: 'AddsAllowedWithBoss',
    0x4000: 'HideUntilLearned',
    0x8000: 'MatchPlayerHighPetLevel',
    0x10000: 'NoWildPetAddsAllowed'
}

const BattlePetVisualFlag = {
    0x1: 'Test1',
    0x2: 'Test2',
    0x4: 'Test3',
}

const itemSetSetFlags = {
    0x1: 'Legacy',
    0x2: 'UseItemHistorySetSlots',
    0x4: 'RequiresPvPTalentsActive'
}

const garrTalentTreeFlags = {
    0x1: 'IS_FACTION_BASED',
    0x2: 'TEMP_RESEARCH_DAILY_RESET',
    0x4: 'IGNORE_TIER_RESEARCH_REQUIREMENTS'   
}

const garrTypeFlags = {
    0x1: 'WITH_VAR_FOLLOWERS',
    0x2: 'USES_OVERMAX_TREAUSURE'
}

const languageFlags = {
    0x1: 'IsExotic',
    0x2: 'HiddenFromPlayer'
}

const sharedStringFlag = {
    0x1: 'InternalOnly'
}

const traitCondFlag = {
    0x1: 'IsGate',
    0x2: 'IsAlwaysMet',
    0x4: 'IsSufficient'
}

const traitNodeFlag = {
    0x1: 'ShowMultipleIcons',
    0x2: 'NeverPurchasable',
    0x4: 'TestPositionLocked',
    0x8: 'TestGridPositioned',
    0x10: 'ActiveAtFirstRank',
    0x20: 'ShowExpandedSelection',
    0x40: 'HideMaxRank'
}

const traitNodeGroupFlag = {
    0x1: 'AvailableByDefault'
}

const traitCurrencyFlag = {
    0x1: 'ShowQuantityAsSpent',
    0x2: 'TraitSourcedShowMax',
    0x4: 'UseClassIcon',
    0x8: 'UseSpecIcon'
}

// TODO: Map to field
const traitTreeFlag = {
    0x1: 'CannotRefund'
}

const chrCustomizationChoiceFlags = {
    0x1: 'REUSEME_WAS_INTERNAL_ONLY',
    0x2: 'HIDE_SCALP_TEXTURE',
    0x4: 'HIDE_BEARD_TEXTURE',
    0x8: 'EXCLUDE_FROM_RANDOMIZATION',
    0x10: 'DEFAULT_WHEN_OPTION_IS_NEW',
    0x20: 'DISPLAY_LOCKED_IF_REQUIREMENT_FAILED',
    0x40: 'REQUIREMENTS_ONLY_CONSIDER_UNLOCKS_FOR_AVAILABILITY'
}

const chrCustomizationOptionFlags = {
    0x1: 'UNDRESS_CHARACTER',
    0x2: 'IS_COLOR_CHOICE',
    0x4: 'REQUIRES_PLAYER_IDENTITY_CUSTOMIZATION_ENABLED',
    0x8: 'HIDE_IN_TRANSMOG',
    0x10: 'IS_PRIMARY_SKIN',
    0x20: 'EXCLUDE_FROM_INITIAL_RANDOMIZATION',
    0x40: 'IGNORE_REQ_CHOICES_IN_UI',
    0x80: 'INDICATE_BLOCKING_CHOICES_IN_UI',
    0x100: 'IS_SOUND_CHOICE',
    0x200: 'HAS_NO_DEFAULT'
}

const chrClassRaceSexFlags = {
    0x1: 'USES_CUSTOM_VOICE'
}

const chrCustomizationVisReqFlags = {
    0x1: 'FORCE_ON'
}

const contentRestrictionRuleFlags = {
    0x1: 'NOT'
}

const contentRestrictionRuleSetFlags = {
    0x1: 'INVERT_RESULT',
    0x2: 'DISABLED'
}

const itemBonusListGroupEntryFlags = {
    0x1: 'INACCESSIBLE_FOR_UPGRADES',
    0x2: 'IGNORE_FOR_PVP_ITEM_LEVEL',
    0x4: 'BARRIER'
}

const journalEncounterFlags = {
    0x1: 'OBSOLETE',
    0x2: 'LIMIT_DIFFICULTIES',
    0x4: 'ALLIANCE_ONLY',
    0x8: 'HORDE_ONLY',
    0x10: 'NO_MAP',
    0x20: 'INTERNAL_ONLY'
}

const journalEncounterItemFlags = {
    0x1: 'OBSOLETE',
    0x2: 'LIMIT_DIFFICULTIES',
    0x4: 'DISPLAY_AS_PER_PLAYER_LOOT',
    0x8: 'DISPLAY_AS_VERY_RARE',
    0x10: 'DISPLAY_AS_EXTREMELY_RARE'
}

const labelXContentRestrictRuleSetFlags = {
    0x1: 'HIDE_IN_SPELLBOOK',
    0x2: 'DISABLE_CLIENT_FEEDBACK'
}

const questPOIBlobFlags = {
    0x1: 'OBSOLETE',
    0x2: 'USER_GENERATED',
    0x4: 'OBSOLETE2',
    0x8: 'HIGHLIGHT_WORLD_QUESTS',
    0x10: 'HIGHLIGHT_DUNGEONS',
    0x20: 'HIGHLIGHT_TREASURES',
    0x40: 'HIGHLIGHT_WORLD_QUESTS_ELITE',
    0x80: 'TOOLS_ONLY_MISMATCHED_POINTS',
    0x100: 'TOOLS_ONLY_MULTIPLE_SPAWNGROUP',
    0x200: 'TOOLS_ONLY_NO_POINT_KEY',
    0x400: 'TOOLS_ONLY_NO_ACTION',
    0x800: 'TOOLS_ONLY_THRESHOLD_AVG',
    0x1000: 'ONLY_USED_TO_PICK_MAP',
    0x2000: 'RECALC_FROM_TERRAIN',
    0x4000: 'ZERO_HEIGHT_IS_VALID_FOR_INGAME_NAVIGATION',
    0x8000: 'USE_PHASES'
}

const renownRewardsFlags = {
    0x1: 'MILESTONE',
    0x2: 'CAPSTONE',
    0x4: 'HIDDEN',
    0x8: 'ACCOUNT_UNLOCK'
}

const soundAmbienceFlags = {
    0x1: 'WEATHER_BLENDFLAVORWITHZONESOUNDS',
    0x2: 'SCREEN_DONOTPLAYAMBIENCESOUNDS',
    0x4: 'WEATHER_BLENDAMBIENCEWITHZONESOUNDS',
    0x8: 'ENABLEALTITUDEFILTER'
}

const soundEmitterFlags = {
    0x1: 'OBSOLETE'
}

const chrModelMaterialFlags = {
    0x1: 'IS_SECONDARY_MATERIAL'
}

const questLineXQuestFlags = {
    0x1: 'IgnoreForCompletion'
}

window.flagMap = new Map();
window.flagMap.set("achievement.Flags", achievementFlags);
window.flagMap.set("animationdata.Flags[0]", animationDataFlags0);
window.flagMap.set("animkitconfig.ConfigFlags", animKitConfigConfigFlags);
window.flagMap.set("areapoi.Flags", areaPOIFlags);
window.flagMap.set("areatable.Flags[0]", areaTableFlags);
window.flagMap.set("areatable.Flags[1]", areaTableFlags2);
window.flagMap.set("artifactpower.Flags", artifactPowerFlags);
window.flagMap.set("battlemasterlist.Flags", battlemasterListFlags);
window.flagMap.set("battlepetability.Flags", BattlePetAbilityFlag);
window.flagMap.set("battlepetstate.Flags", battlePetStateFlags);
window.flagMap.set("battlepetspecies.Flags", BattlePetSpeciesFlags);
window.flagMap.set("battlepetvisual.Flags", BattlePetVisualFlag);
window.flagMap.set("broadcasttext.Flags", broadcastTextFlags);
window.flagMap.set("campaign.Flags", campaignFlags);
window.flagMap.set("charsections.Flags", charSectionFlags);
window.flagMap.set("charshipment.Flags", charShipmentFlags);
window.flagMap.set("chatchannels.Flags", chatChannelsFlags);
window.flagMap.set("chrclasses.Flags", chrClassesFlags);
window.flagMap.set("chrclassracesex.Flags", chrClassRaceSexFlags);
window.flagMap.set("chrcustomizationchoice.Flags", chrCustomizationChoiceFlags);
window.flagMap.set("chrcustomizationcategory.Flags", chrCustomizationCategoryFlags);
window.flagMap.set("chrcustomizationoption.Flags", chrCustomizationOptionFlags);
window.flagMap.set("chrcustomizationvisreq.Flags", chrCustomizationVisReqFlags);
window.flagMap.set("chrmodel.Flags", chrModelFlags);
window.flagMap.set("chrmodelmaterial.Flags", chrModelMaterialFlags);
window.flagMap.set("chrraces.Flags", chrRacesFlags);
window.flagMap.set("contentrestrictionrule.Flags", contentRestrictionRuleFlags);
window.flagMap.set("contentrestrictionruleset.Flags", contentRestrictionRuleSetFlags);
window.flagMap.set("contenttuning.Flags", contentTuningFlags);
window.flagMap.set("creaturedifficulty.Flags[0]", creatureStaticFlags0);
window.flagMap.set("creaturedifficulty.Flags[1]", creatureStaticFlags1);
window.flagMap.set("creaturedifficulty.Flags[2]", creatureStaticFlags2);
window.flagMap.set("creaturedifficulty.Flags[3]", creatureStaticFlags3);
window.flagMap.set("creaturedifficulty.Flags[4]", creatureStaticFlags4);
window.flagMap.set("creaturedifficulty.Flags[5]", creatureStaticFlags5);
window.flagMap.set("creaturedifficulty.Flags[6]", creatureStaticFlags6);
window.flagMap.set("creaturedisplayinfo.Flags", cdiFlags);
window.flagMap.set("creaturedisplayinfo.Flags", creatureDisplayInfoFlags);
window.flagMap.set("creaturemodeldata.Flags", creatureModelDataFlags);
window.flagMap.set("criteria.Flags", criteriaFlags);
window.flagMap.set("criteriatree.Flags", criteriaTreeFlags);
window.flagMap.set("currencytypes.Flags[0]", currencyFlags);
window.flagMap.set("currencytypes.Flags[1]", currencyFlagsB);
window.flagMap.set("difficulty.Flags", difficultyFlags);
window.flagMap.set("dungeonencounter.Flags", dungeonEncounterFlags);
window.flagMap.set("emotes.EmoteFlags", emotesFlags);
window.flagMap.set("friendshipreputation.Flags", friendshipReputationFlags);
window.flagMap.set("garrability.Flags", garrAbilityFlags);
window.flagMap.set("garrabilityeffect.Flags", garrAbilityEffectFlags);
window.flagMap.set("garrautospell.Flags", garrAutoSpellFlags);
window.flagMap.set("garrautospelleffect.Flags", garrAutoSpellEffectFlags);
window.flagMap.set("garrbuilding.Flags", garrBuildingFlags);
window.flagMap.set("garrclassspec.Flags", garrClassSpecFlags);
window.flagMap.set("garrencounter.Flags", garrEncounterFlags);
window.flagMap.set("garrfollower.Flags", garrFollowerFlags);
window.flagMap.set("garrfollowertype.Flags", garrFollowerTypeFlags);
window.flagMap.set("garrmision.Flags", garrMissionFlags);
window.flagMap.set("garrplot.Flags", garrPlotFlags);
window.flagMap.set("garrsite.Flags", garrSiteFlags);
window.flagMap.set("gemproperties.Type", socketColors);
window.flagMap.set("globalstrings.Flags", globalstringsFlags);
window.flagMap.set("holidays.Flags", holidayFlags);
window.flagMap.set("itembonuslistgroupentry.Flags", itemBonusListGroupEntryFlags);
window.flagMap.set("itemdisplayinfo.Flags", itemDisplayInfoFlags);
window.flagMap.set("itemset.SetFlags", itemSetSetFlags);
window.flagMap.set("itemsearchname.Flags[0]", itemStaticFlags0);
window.flagMap.set("itemsearchname.Flags[1]", itemStaticFlags1);
window.flagMap.set("itemsearchname.Flags[2]", itemStaticFlags2);
window.flagMap.set("itemsearchname.Flags[3]", itemStaticFlags3);
window.flagMap.set("itemsparse.AllowableClass", classMask);
window.flagMap.set("itemsearchname.AllowableClass", classMask);
window.flagMap.set("itemsparse.Flags[0]", itemStaticFlags0);
window.flagMap.set("itemsparse.Flags[1]", itemStaticFlags1);
window.flagMap.set("itemsparse.Flags[2]", itemStaticFlags2);
window.flagMap.set("itemsparse.Flags[3]", itemStaticFlags3);
window.flagMap.set("itemsubclass.DisplayFlags", itemSubClassDisplayFlags);
window.flagMap.set("itemsubclass.Flags", itemSubClassFlags);
window.flagMap.set("journalencounter.Flags", journalEncounterFlags);
window.flagMap.set("journalinstance.Flags", journalInstanceFlags);
window.flagMap.set("journalencounteritem.Flags", journalEncounterItemFlags);
window.flagMap.set("labelxcontentrestrictruleset.Flags", labelXContentRestrictRuleSetFlags);
window.flagMap.set("lfgdungeons.Flags[0]", lfgFlags);
window.flagMap.set("lfgdungeons.Flags[1]", lfgFlagsB);
window.flagMap.set("lightskybox.Flags", lightSkyboxFlags);
window.flagMap.set("liquidtype.Flags", liquidTypeFlags);
window.flagMap.set("material.Flags", materialFlags);
window.flagMap.set("map.Flags[0]", mapFlags);
window.flagMap.set("map.Flags[1]", mapFlags2);
window.flagMap.set("modifiedcraftingreagentitem.Flags", optionalReagentItemFlag);
window.flagMap.set("mount.Flags", mountFlags);
window.flagMap.set("mountcapability.Flags", mountCapabilityFlags);
window.flagMap.set("phase.Flags", phaseFlags);
window.flagMap.set("playercondition.ClassMask", classMask);
window.flagMap.set("playercondition.Flags", playerConditionFlags);
window.flagMap.set("powertype.Flags", powerTypeFlags);
window.flagMap.set("questinfo.Modifiers", questTagModifierFlags);
window.flagMap.set("questobjective.Flags", questObjectiveFlags);
window.flagMap.set("questv2clitask.Flags[0]", questFlags0);
window.flagMap.set("questv2clitask.Flags[1]", questFlags1);
window.flagMap.set("questv2clitask.Flags[2]", questFlags2);
window.flagMap.set("questpoiblob.Flags", questPOIBlobFlags);
window.flagMap.set("renownrewards.Flags", renownRewardsFlags);
window.flagMap.set("runeforgelegendaryability.InventoryTypeMask", inventoryTypeMask);
window.flagMap.set("scenario.Flags", scenarioFlags);
window.flagMap.set("scenariostep.Flags", scenarioStepFlags);
window.flagMap.set("sharedstring.Flags", sharedStringFlag);
window.flagMap.set("skillline.Flags", skillLineFlags);
window.flagMap.set("soundambience.Flags", soundAmbienceFlags);
window.flagMap.set("soundemitters.Flags", soundEmitterFlags);
window.flagMap.set("soundkit.Flags", soundkitFlags);
window.flagMap.set("spellauraoptions.ProcTypeMask[0]", procTypeMask0)
window.flagMap.set("spellauraoptions.ProcTypeMask[1]", procTypeMask1)
window.flagMap.set("spellcastingrequirements.FacingCasterFlags", facingCasterFlags);
window.flagMap.set("spelleffect.EffectAttributes", spellEffectEffectAttributes);
window.flagMap.set("spellinterrupts.AuraInterruptFlags[0]", auraInterruptFlags0);
window.flagMap.set("spellinterrupts.AuraInterruptFlags[1]", auraInterruptFlags1);
window.flagMap.set("spellinterrupts.ChannelInterruptFlags[0]", auraInterruptFlags0);
window.flagMap.set("spellinterrupts.ChannelInterruptFlags[1]", auraInterruptFlags1);
window.flagMap.set("spellinterrupts.InterruptFlags", interruptFlags);
window.flagMap.set("spellitemenchantment.Flags", spellItemEnchantmentFlags);
window.flagMap.set("spellmisc.Attributes[0]", spellAttributes0);
window.flagMap.set("spellmisc.Attributes[1]", spellAttributes1);
window.flagMap.set("spellmisc.Attributes[10]", spellAttributes10);
window.flagMap.set("spellmisc.Attributes[11]", spellAttributes11);
window.flagMap.set("spellmisc.Attributes[12]", spellAttributes12);
window.flagMap.set("spellmisc.Attributes[13]", spellAttributes13);
window.flagMap.set("spellmisc.Attributes[14]", spellAttributes14);
window.flagMap.set("spellmisc.Attributes[15]", spellAttributes15);
window.flagMap.set("spellmisc.Attributes[2]", spellAttributes2);
window.flagMap.set("spellmisc.Attributes[3]", spellAttributes3);
window.flagMap.set("spellmisc.Attributes[4]", spellAttributes4);
window.flagMap.set("spellmisc.Attributes[5]", spellAttributes5);
window.flagMap.set("spellmisc.Attributes[6]", spellAttributes6);
window.flagMap.set("spellmisc.Attributes[7]", spellAttributes7);
window.flagMap.set("spellmisc.Attributes[8]", spellAttributes8);
window.flagMap.set("spellmisc.Attributes[9]", spellAttributes9);
window.flagMap.set("spellmisc.SchoolMask", damageClass);
window.flagMap.set("spellshapeshiftform.Flags", spellShapeshiftFormFlags);
window.flagMap.set("spelltargetrestrictions.TargetCreatureType", targetCreatureType);
window.flagMap.set("spelltargetrestrictions.Targets", targetFlags);
window.flagMap.set("stationery.Flags", stationeryFlags);
window.flagMap.set("summonproperties.Flags", summonPropertiesFlags);
window.flagMap.set("summonproperties.Flags[0]", summonPropertiesFlags);
window.flagMap.set("taxinodes.Flags", taxiNodeFlags);
window.flagMap.set("traitcond.Flags", traitCondFlag);
window.flagMap.set("traitcurrency.Flags", traitCurrencyFlag);
window.flagMap.set("traitnode.Flags", traitNodeFlag);
window.flagMap.set("transmogillusion.Flags", TransmogIllusionFlags);
window.flagMap.set("transmogset.Flags", transmogSetFlags);
window.flagMap.set("transmogsetitem.Flags", transmogSetItemFlags);
window.flagMap.set("uimap.Flags", uiMapFlags);
window.flagMap.set("uimodelsceneactor.Flags", uiModelSceneActorFlags);
window.flagMap.set("uiscriptedanimationeffect.Flags", ScriptedAnimationFlags);
window.flagMap.set("vehiclepoitype.Flags", vehiclePOITypeFlags);
window.flagMap.set("waypointedge.Flags", waypointEdgeFlags);
window.flagMap.set("waypointnode.Flags", waypointNodeFlags);
window.flagMap.set("wmoareatable.Flags", wmoAreaTableFlags);
window.flagMap.set("worldmaparea.Flags", worldMapAreaFlags);
window.flagMap.set("groupfinderactivity.Flags", groupFinderActivityFlags);
window.flagMap.set("groupfindercategory.Flags", groupFinderCategoryFlags);
window.flagMap.set("itembonuslist.Flags", itemBonusListFlags);
window.flagMap.set("spellcategory.Flags", spellCategoryFlags);
window.flagMap.set("garrtalenttree.Flags", garrTalentTreeFlags);
window.flagMap.set("garrtype.Flags", garrTypeFlags);
window.flagMap.set("languages.Flags", languageFlags);
window.flagMap.set("creatureimmunities.Flags", creatureImmunitiesFlags);
window.flagMap.set("questlinexquest.Flags", questLineXQuestFlags);
window.flagMap.set("wbaccesscontrollist.GrantFlags", ingameBrowserFlags);
window.flagMap.set("wbaccesscontrollist.RevokeFlags", ingameBrowserFlags);

// Conditional flags
let conditionalFlags = new Map();
conditionalFlags.set("chrcustomizationreq.ReqValue",
    [
        ['chrcustomizationreq.ReqType=1', classMask],
    ]
);

conditionalFlags.set("spelleffect.EffectMiscValue[0]",
    [
        ['spelleffect.EffectAura=39', damageClass],
        ['spelleffect.EffectAura=69', damageClass],
        ['spelleffect.EffectAura=71', damageClass],
        ['spelleffect.EffectAura=72', damageClass],
        ['spelleffect.EffectAura=73', damageClass],
        ['spelleffect.EffectAura=74', damageClass],
        ['spelleffect.EffectAura=174', damageClass],
        ['spelleffect.EffectAura=189', combatRatingFlags],
        ['spelleffect.EffectAura=198', combatRatingFlags],
        ['spelleffect.EffectAura=194', damageClass],
        ['spelleffect.EffectAura=220', damageClass],
        ['spelleffect.EffectAura=267', damageClass],
        ['spelleffect.EffectAura=270', damageClass],
        ['spelleffect.EffectAura=301', damageClass],
        ['spelleffect.EffectAura=316', damageClass],
        ['spelleffect.EffectAura=405', combatRatingFlags],
        ['spelleffect.EffectAura=558', damageClass],
        ['spelleffect.EffectAura=574', damageClass],
        ['spelleffect.EffectAura=579', damageClass],
        ['spelleffect.EffectAura=583', combatRatingFlags],
    ]
);

conditionalFlags.set("spelleffect.EffectMiscValue[1]",
    [
        ['spelleffect.EffectAura=198', combatRatingFlags],
    ]
);
