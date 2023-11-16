// Enums are currently retrieved from TrinityCore repo and game data, in a best case scenario these would come from DBD..

const reputationLevels = {
    0: 'None/Hated',
    1: 'Hostile',
    2: 'Unfriendly',
    3: 'Neutral',
    4: 'Friendly',
    5: 'Honored',
    6: 'Revered',
    7: 'Exalted',
}

const expansionLevels = {
    0: 'Vanilla',
    1: ['TBC', 'The Burning Crusade'],
    2: ['WotLK', 'Wrath of the Lich King'],
    3: ['Cata', 'Cataclysm'],
    4: ['MoP', 'Mists of Pandaria'],
    5: ['WoD', 'Warlords of Draenor'],
    6: 'Legion',
    7: ['BfA', 'Battle for Azeroth'],
    8: 'Shadowlands',
    9: 'Dragonflight',
    10: ['TWW', 'The War Within'],
    11: 'Midnight',
    12: ['TLT', 'The Last Titan'],
}

const mapTypes = {
    0: 'Not Instanced',
    1: 'Party Dungeon',
    2: 'Raid Dungeon',
    3: 'PVP Battlefield',
    4: 'Arena Battlefield',
    5: 'Scenario',
}

// 585
const itemBonusTypes = {
    0: 'No Bonus',
    1: 'Increase iLevel by {#Item Level}',
    2: 'Increase Bonus Stat {@g_bonusStatFields} by {#Percentage}% of budget',
    3: 'Set Item Quality to {@ITEM_QUALITY}',
    4: 'Add Custom Tooltip "{ItemNameDescription}" in slot {#Slot}',
    5: 'Set Name Suffix "{ItemNameDescription}" at priority {#Priority}',
    6: 'Add {#Num Sockets} socket(s) of type {$Item Socket Type}',
    7: 'Set Item Appearance Modifier "{ItemAppearanceModifier}" at priority {#Priority}',
    8: 'Increase required level by {#Required Level}',
    9: 'When received, toast item with "{$Item Toast}" at priority {#Priority}',
    10: 'Multiply repair cost by {#Repair Cost Multiplier}%',
    11: 'Scaling Stat Distribution "{ScalingStatDistribution}" at priority {#Priority}',
    12: 'Set Disenchant Loot to "{Treasure}" at priority {#Priority}',
    13: 'Fixed Stat Distribution "{ScalingStatDistribution}" at priority {#Priority} + ContentTuning (optional) "{ContentTuning}"',
    14: 'Preview as Item Pyramid',
    15: 'Is Client Preview',
    16: 'Set Bind Type to {@ITEM_BIND}',
    17: 'Secondary Relic Power Label{}',
    18: 'Set Required Level to {#Required Level}',
    19: 'AzeriteTierUnlockSetID',
    21: 'CanDisenchant',
    22: 'CanScrap',
    23: 'ItemEffectID',
    25: 'ModifiedCraftingStat',
    27: 'RequiredLevelCurve',
    30: 'ItemDescription',
    31: 'LegendaryName',
    34: 'ItemBonusListGroupID',
    35: 'ItemLimitCategoryID',
    37: 'ItemConversionID',
    42: 'Base Item Level',
};

const criteriaTreeOperator = {
    0: 'SINGLE',
    1: 'SINGLE_NOT_COMPLETED',
    4: 'ALL',
    5: 'SUM_CHILDREN',
    6: 'MAX_CHILD',
    7: 'COUNT_DIRECT_CHILDREN',
    8: 'ANY',
    9: 'SUM_CHILDREN_WEIGHT'
}

const criteriaTreeOperatorFriendly = {
    0: 'Requires just one of:',
    1: 'SINGLE_NOT_COMPLETED',
    4: 'Requires all of:',
    5: 'Sum of:',
    6: 'MAX_CHILD',
    7: 'COUNT_DIRECT_CHILDREN',
    8: 'Requires any of:',
    9: 'SUM_CHILDREN_WEIGHT'
}

const modifierTreeOperator = {
    2: 'SingleTrue',
    3: 'SingleFalse',
    4: 'All',
    8: 'Some'
};

const criteriaAdditionalCondition = {
    0:   ['NONE', 'No modifier'],
    1:   ['SOURCE_DRUNK_VALUE', 'Player inebriation level is {#Drunkenness} or more'],
    2:   ['SOURCE_PLAYER_CONDITION', 'Player meets condition "{PlayerCondition}"'],
    3:   ['ITEM_LEVEL', 'Minimum item level is {#Item Level}'],
    4:   ['TARGET_CREATURE_ENTRY', 'Target is NPC "{Creature}"'],
    5:   ['TARGET_MUST_BE_PLAYER', 'Target is player'],
    6:   ['TARGET_MUST_BE_DEAD', 'Target is dead'],
    7:   ['TARGET_MUST_BE_ENEMY', 'Target is opposite faction'],
    8:   ['SOURCE_HAS_AURA', 'Player has aura "{Spell}"'],
    9:   ['SOURCE_HAS_AURA_TYPE', 'Player has aura effect "{SpellAuraNames.EnumID}"'],
    10:  ['TARGET_HAS_AURA', 'Target has aura "{Spell}"'],
    11:  ['TARGET_HAS_AURA_TYPE', 'Target has aura effect "{SpellAuraNames.EnumID}"'],
    12:  ['SOURCE_AURA_STATE', 'Target has aura state "{$Aura State}"'],
    13:  ['TARGET_AURA_STATE', 'Player has aura state "{$Aura State}"'],
    14:  ['ITEM_QUALITY_MIN', 'Item quality is at least {$Item Quality}'],
    15:  ['ITEM_QUALITY_EQUALS', 'Item quality is exactly {$Item Quality}'],
    16:  ['SOURCE_IS_ALIVE', 'Player is alive'],
    17:  ['SOURCE_AREA_OR_ZONE', 'Player is in area "{AreaTable}"'],
    18:  ['TARGET_AREA_OR_ZONE', 'Target is in area "{AreaTable}"'],
    19:  ['ITEM_IS_ITEMID', 'Item is "{Item}"'],
    20:  ['MAP_DIFFICULTY_OLD', 'Legacy dungeon difficulty is "{$Dungeon Difficulty}"'],
    21:  ['TARGET_CREATURE_YIELDS_XP', 'Exceeds the target\'s level by {#Level Delta} levels'],
    22:  ['SOURCE_LEVEL_ABOVE_TARGET', 'Target exceeds your level by {#Level Delta} levels'],
    23:  ['SOURCE_LEVEL_EQUAL_TARGET', 'You and the target are equal level'],
    24:  ['ARENA_TYPE', 'Player is in an arena with team size {#Team Size}'],
    25:  ['SOURCE_RACE', 'Player race is "{ChrRaces}"'],
    26:  ['SOURCE_CLASS', 'Player class is "{ChrClasses}"'],
    27:  ['TARGET_RACE', 'Target race is "{ChrRaces}"'],
    28:  ['TARGET_CLASS', 'Target class is "{ChrClasses}"'],
    29:  ['MAX_GROUP_MEMBERS', 'Less than {#Tappers} tappers'],
    30:  ['TARGET_CREATURE_TYPE', 'Creature is type "{CreatureType}"'],
    31:  ['TARGET_CREATURE_FAMILY', 'Creature is family "{CreatureFamily}"'],
    32:  ['SOURCE_MAP', 'Player is on map "{Map}"'],
    33:  ['CLIENT_VERSION', 'Milestone is at or before "{WowStaticSchemas}"'],
    34:  ['BATTLE_PET_TEAM_LEVEL', 'All three winning battle pets are at or above level {#Battle Pet Level}'],
    35:  ['NOT_IN_GROUP', 'Player is not in a party'],
    36:  ['IN_GROUP', 'Player is in a party'],
    37:  ['MIN_PERSONAL_RATING', 'Has a Personal Rating of at least {#Personal Rating}'],
    38:  ['TITLE_BIT_INDEX', 'Has title "{CharTitles.Mask_ID}"'],
    39:  ['SOURCE_LEVEL', 'Player is exactly level {#Level}'],
    40:  ['TARGET_LEVEL', 'Target is exactly level {#Level}'],
    41:  ['SOURCE_ZONE', 'Player is in top-level area "{AreaTable}"'],
    42:  ['TARGET_ZONE', 'Target is in top-level area "{AreaTable}"'],
    43:  ['SOURCE_HEALTH_PCT_LOWER', 'Player health below {#Percent}%'],
    44:  ['SOURCE_HEALTH_PCT_GREATER', 'Player health above {#Percent}%'],
    45:  ['SOURCE_HEALTH_PCT_EQUAL', 'Player health equals {#Percent}%'],
    46:  ['TARGET_HEALTH_PCT_LOWER', 'Target health below {#Percent}%'],
    47:  ['TARGET_HEALTH_PCT_GREATER', 'Target health above {#Percent}%'],
    48:  ['TARGET_HEALTH_PCT_EQUAL', 'Target health equals {#Percent}%'],
    49:  ['SOURCE_HEALTH_LOWER', 'Player health below {#Hit Points} HP'],
    50:  ['SOURCE_HEALTH_GREATER', 'Player health above {#Hit Points} HP'],
    51:  ['SOURCE_HEALTH_EQUAL', 'Player health equals {#Hit Points} HP'],
    52:  ['TARGET_HEALTH_LOWER', 'Target health below {#Hit Points} HP'],
    53:  ['TARGET_HEALTH_GREATER', 'Target health above {#Hit Points} HP'],
    54:  ['TARGET_HEALTH_EQUAL', 'Target health equals {#Hit Points} HP'],
    55:  ['TARGET_PLAYER_CONDITION', 'Target is a player with condition "{PlayerCondition}"'],
    56:  ['MIN_ACHIEVEMENT_POINTS', 'Player has over {#Achievement Pts} achievement points'],
    57:  ['IN_LFG_DUNGEON', 'Player is in a LFG dungeon'],
    58:  ['IN_LFG_RANDOM_DUNGEON', 'Player is in a random LFG dungeon'],
    59:  ['IN_LFG_FIRST_RANDOM_DUNGEON', 'Player is in a first random LFG dungeon'],
    60:  ['IN_RANKED_ARENA_MATCH', 'Player is in a ranked arena match'],
    61:  ['REQUIRES_GUILD_GROUP', 'Player is in a guild party'],
    62:  ['GUILD_REPUTATION', 'Player has guild reputation of {#Guild Reputation} or more'],
    63:  ['RATED_BATTLEGROUND', 'Player is in rated battleground'],
    64:  ['RATED_BATTLEGROUND_RATING', 'Player has a battleground rating of {#Battleground Rating} or more'],
    65:  ['PROJECT_RARITY', 'Research project rarity is "{$Project Rarity}"'],
    66:  ['PROJECT_RACE', 'Research project is in branch "{ResearchBranch}"'],
    67:  ['WORLD_STATE_EXPRESSION', 'World state expression "{WorldStateExpression}" is true'],
    68:  ['MAP_DIFFICULTY', 'Dungeon difficulty is "{Difficulty}"'],
    69:  ['SOURCE_LEVEL_GREATER', 'Player level is {#Level} or more'],
    70:  ['TARGET_LEVEL_GREATER', 'Target level is {#Level} or more'],
    71:  ['SOURCE_LEVEL_LOWER', 'Player level is {#Level} or less'],
    72:  ['TARGET_LEVEL_LOWER', 'Target level is {#Level} or less'],
    73:  ['MODIFIER_TREE', 'Modifier tree "{ModifierTree}" is also true'],
    74:  ['SCENARIO_ID', 'Player is on scenario "{Scenario}"'],
    75:  ['THE_TILLERS_REPUTATION', 'Reputation with Tillers is above {#Reputation}'],
    76:  ['PET_BATTLE_ACHIEVEMENT_POINTS', 'Battle pet achievement points are at least {#Achievement Pts}'],
    77:  ['UNIQUE_PETS_KNOWN', '(Account) At least {#Pets Known} unique pets known'],
    78:  ['BATTLE_PET_FAMILY', 'Battlepet is of type "{$Battle Pet Types}"'],
    79:  ['BATTLE_PET_HEALTH_PCT', '(Account) Battlepet\'s health is below {#Health Percent} percent'],
    80:  ['GUILD_GROUP_MEMBERS', 'Be in a group with at least {#Members} guild members'],
    81:  ['BATTLE_PET_ENTRY', 'Battle pet opponent is "{Creature}"'],
    82:  ['SCENARIO_STEP_INDEX', 'Player is on scenario step number {#Step Number}'],
    83:  ['CHALLENGE_MODE_MEDAL', 'Challenge mode medal earned is "{#Challenge Mode Medal(OBSOLETE)}" (OBSOLETE)'],
    84:  ['IS_ON_QUEST', 'Player is currently on the quest "{QuestV2}"'],
    85:  ['EXALTED_WITH_FACTION', 'Reach exalted with "{Faction}"'],
    86:  ['HAS_ACHIEVEMENT', 'Earned achievement "{Achievement}" on this account'],
    87:  ['HAS_ACHIEVEMENT_ON_CHARACTER', 'Earned achievement "{Achievement}" on this player'],
    88:  ['CLOUD_SERPENT_REPUTATION', 'Reputation with Order of the Cloud Serpent is above {#Reputation}'],
    89:  ['BATTLE_PET_BREED_QUALITY_ID', 'Battle pet is of quality "{BattlePetBreedQuality}"'],
    90:  ['PET_BATTLE_IS_PVP', 'Battle pet fight was PVP'],
    91:  ['BATTLE_PET_SPECIES', 'Battle pet is species type "{BattlePetSpecies}"'],
    92:  ['ACTIVE_EXPANSION', 'Server expansion level is "{$Expansion Level}" or higher'],
    93:  ['HAS_BATTLE_PET_JOURNAL_LOCK', 'Player has battle pet journal lock'],
    94:  ['FRIENDSHIP_REP_REACTION', 'Friendship rep reaction "{FriendshipRepReaction}" is met'],
    95:  ['FACTION_STANDING', 'Reputation with "{Faction}" is {#Reputation} or more'],
    96:  ['ITEM_CLASS_AND_SUBCLASS', 'Item is class "{ItemClass.ClassID}", subclass "{^ItemSubclass.SubclassID:ItemSubclass.ClassID = ?}"'],
    97:  ['SOURCE_SEX', 'Player\'s gender is "{$Gender}"'],
    98:  ['SOURCE_NATIVE_SEX', 'Player\'s native gender is "{$Gender}"'],
    99:  ['SKILL', 'Player skill "{SkillLine}" is level {#Skill Level} or higher'],
    100: ['LANGUAGE_SKILL', 'Player language "{Languages}" is level {#Language Level} or higher'],
    101: ['NORMAL_PHASE_SHIFT', 'Player is in normal phase'],
    102: ['IN_PHASE', 'Player is in phase "{Phase}"'],
    103: ['NOT_IN_PHASE', 'Player is in phase group "{PhaseGroup}"'],
    104: ['HAS_SPELL', 'Player knows spell "{Spell}"'],
    105: ['ITEM_COUNT', 'Player is carrying item "{Item}", quantity {#Quantity}'],
    106: ['ACCOUNT_EXPANSION', 'Player expansion level is "{$Expansion Level}" or higher'],
    107: ['SOURCE_HAS_AURA_LABEL', 'Player has aura with label {Label}'],
    108: ['WORLDSTATE_EQUALS', 'Player\'s realm state "{WorldState}" equals {#Value}'],
    109: ['TIME_IN_RANGE', 'Time is between "{/Begin Date}" and "{/End Date}"'],
    110: ['REWARDED_QUEST', 'Player has previously completed quest "{QuestV2}"'],
    111: ['COMPLETED_QUEST', 'Player is ready to turn in quest "{QuestV2}"'],
    112: ['COMPLETED_QUEST_OBJECTIVE', 'Player has completed Quest Objective "{QuestObjective}"'],
    113: ['EXPLORED_AREA', 'Player has explored area "{AreaTable}"'],
    114: ['ITEM_COUNT_INCLUDING_BANK', 'Player or bank has item "{Item}", quantity {#Quantity}'],
    115: ['WEATHER_IS_ID', 'Weather is "{Weather}"'],
    116: ['SOURCE_PVP_FACTION_INDEX', 'Player faction is {$Player Faction}'],
    117: ['LFG_VALUE_EQUAL', 'Looking-for-group status "{$LFG Status}" equals {#Value}'],
    118: ['LFG_VALUE_GREATER', 'Looking-for-group status "{$LFG Status}" is {#Value} or more'],
    119: ['CURRENCY_AMOUNT', 'Player has currency "{CurrencyTypes}" in amount {#Amount} or more'],
    120: ['CREATURE_KILL_NUM_THREAT', 'Player Killed creature with less than "{#Targets}" threat list targets'],
    121: ['CURRENCY_TRACKED_AMOUNT', 'Player has currency "{CurrencyTypes}" tracked (per season) in amount {#Amount} or more'],
    122: ['MAP_INSTANCE_TYPE', 'Player is on a map of type "{@INSTANCE_TYPE}"'],
    123: ['MENTOR', 'Player was in a Time Walker instance'],
    124: ['PVP_SEASON_ACTIVE', 'PVP season is active'],
    125: ['PVP_SEASON_CURRENT', 'Current PVP season is {#Season}'],
    126: ['GARRISON_LEVEL_ABOVE', 'Garrison is tier {#Tier} or higher for garrison type "{GarrType}"'],
    127: ['GARRISON_FOLLOWERS_ABOVE_LEVEL', 'At least {#Followers} followers of at least level {#Level} for follower type "{GarrFollowerType}"'],
    128: ['GARRISON_FOLLOWERS_ABOVE_QUALITY', 'At least {#Followers} followers at least quality "{@GARR_FOLLOWER_QUALITY}" for follower type "{GarrFollowerType}"'],
    129: ['GARRISON_FOLLOWER_ABOVE_LEVEL_WITH_ABILITY', 'Follower of at least level {#Level} has ability {GarrAbility} for follower type "{GarrFollowerType}"'],
    130: ['GARRISON_FOLLOWER_ABOVE_LEVEL_WITH_TRAIT', 'Follower of at least level {#Level} has trait {GarrAbility} for follower type "{GarrFollowerType}"'],
    131: ['GARRISON_FOLLOWER_WITH_ABILITY_IN_BUILDING', 'Follower with ability "{GarrAbility}" is assigned to building type "{@GARRISON_BUILDING_TYPE}" for garrison type "{GarrType}"'],
    132: ['GARRISON_FOLLOWER_WITH_TRAIT_IN_BUILDING', 'Follower with trait "{GarrAbility}" is assigned to building type "{@GARRISON_BUILDING_TYPE}" for garrison type "{GarrType}"'],
    133: ['GARRISON_FOLLOWER_ABOVE_LEVEL_IN_BUILDING', 'Follower at least level {#Level} is assigned to building type "{@GARRISON_BUILDING_TYPE}" for garrison type "GarrType}"'],
    134: ['GARRISON_BUILDING_ABOVE_LEVEL', 'Building "{@GARRISON_BUILDING_TYPE}" is at least level {#Level} for garrison type "{GarrType}"'],
    135: ['GARRISON_BLUEPRINT', 'Has blueprint for garrison building "{GarrBuilding}" of type "{GarrType}"'],
    136: ['GARRISON_SPECIALIZATION', 'Has garrison building specialization "{GarrSpecialization}"'],
    137: ['GARRISON_PLOTS_FULL', 'All garrison type "{GarrType}" plots are full'],
    138: ['OWN_GARRISON', 'Player is in their own garrison'],
    139: ['GARRISON_SHIPMENT_PENDING', 'Shipment of type "{CharShipmentContainer}" is pending'],
    140: ['GARRISON_BUILDING_INACTIVE', 'Garrison building "{GarrBuilding}" is under construction'],
    141: ['GARRISON_MISSION_COMPLETED', 'Garrison mission "{GarrMission}" has been completed'],
    142: ['GARRISON_BUILDING_EQUAL_LEVEL', 'Building {@GARRISON_BUILDING_TYPE} is exactly level {#Level} for garrison type "{GarrType}"'],
    143: ['GARRISON_FOLLOWER_WITH_ABILITY', 'This follower has ability "{GarrAbility}" for garrison type "{GarrType}"'],
    144: ['GARRISON_FOLLOWER_WITH_TRAIT', 'This follower has trait "{GarrAbility}" for garrison type "{GarrType}"'],
    145: ['GARRISON_FOLLOWER_ABOVE_QUALITY_WOD', 'This Garrison Follower is {@GARR_FOLLOWER_QUALITY} quality'],
    146: ['GARRISON_FOLLOWER_EQUAL_LEVEL', 'This Garrison Follower is level {#Level}'],
    147: ['GARRISON_RARE_MISSION', 'This Garrison Mission is Rare'],
    148: ['GARRISON_ELITE_MISSION', 'This Garrison Mission is Elite'],
    149: ['GARRISON_BUILDING_LEVEL', 'This Garrison Building is level {#Level}'],
    150: ['GARRISON_BUILDING_READY', 'Garrison plot instance "{GarrPlotInstance}" has building that is ready to activate'],
    151: ['BATTLE_PET_SPECIES_IN_TEAM', 'Battlepet: with at least {#Amount} "{BattlePetSpecies}"'],
    152: ['BATTLE_PET_FAMILY_IN_TEAM', 'Battlepet: with at least {#Amount} pets of type "{$Battle Pet Types}"'],
    153: ['BATTLE_PET_LAST_ABILITY', 'Battlepet: last ability was "{BattlePetAbility}"'],
    154: ['BATTLE_PET_LAST_ABILITY_TYPE', 'Battlepet: last ability was of type "{$Battle Pet Types}"'],
    155: ['BATTLE_PET_NUM_ALIVE', 'Battlepet: with at least {#Alive} alive'],
    156: ['GARRISION_SPECIALIZATION_ACTIVE', 'Has Garrison building active specialization "{GarrSpecialization}"'],
    157: ['GARRISON_FOLLOWER_ID', 'Has Garrison follower "{GarrFollower}"'],
    158: ['QUEST_OBJECTIVE_PROGRESS_EQUAL', 'Player\'s progress on Quest Objective "{QuestObjective}" is equal to {#Value}'],
    159: ['QUEST_OBJECTIVE_PROGRESS_ABOVE', 'Player\'s progress on Quest Objective "{QuestObjective}" is at least {#Value}'],
    160: ['IS_PTR_REALM', 'This is a PTR Realm'],
    161: ['IS_BETA_REALM', 'This is a Beta Realm'],
    162: ['IS_QA_REALM', 'This is a QA Realm'],
    163: ['SHIPMENT_CONTAINER_FULL', 'Shipment Container "{CharShipmentContainer}" is full'],
    164: ['GARRISON_INVASION_PLAYER_COUNT', 'Player count is valid to start garrison invasion'],
    165: ['INSTANCE_MAX_PLAYERS', 'Instance has at most {#Players} players'],
    166: ['GARRISON_PLOTLEVELTYPECHECK', 'All plots are full and at least level {#Level} for garrison type "{GarrType}"'],
    167: ['GARRISON_MISSION_TYPE', 'This mission is type "{GarrMissionType}"'],
    168: ['GARRISON_FOLLOWER_ABOVE_ITEM_LEVEL', 'This follower is at least item level {#Level}'],
    169: ['GARRISON_FOLLOWERS_ABOVE_ITEM_LEVEL', 'At least {#Followers} followers are at least item level {#Level} for follower type "{GarrFollowerType}"'],
    170: ['GARRISON_LEVEL_EQUAL', 'Garrison is exactly tier {#Tier} for garrison type "{GarrType}"'],
    171: ['GARRISON_GROUP_SIZE', 'Instance has exactly {#Players} players'],
    172: ['CURRENCY_IS_OF_ID', 'The currency is type "{CurrencyTypes}"'],
    173: ['TARGETING_CORPSE', 'Target is player corpse'],
    174: ['ELIGIBLE_FOR_QUEST', 'Player is currently eligible for quest "{QuestV2}"'],
    175: ['GARRISON_FOLLOWERS_LEVEL_EQUAL', 'At least {#Followers} followers exactly level {#Level} for follower type "{GarrFollowerType}"'],
    176: ['GARRISON_FOLLOWER_ID_IN_BUILDING', 'Garrison follower "{GarrFollower}" is in building "{GarrBuilding}"'],
    177: ['GARRISON_AVAIL_INPROGRESS_MISSIONS', 'Player has less than {#Available} available and {#In-Progress} in-progress missions of garrison type "{GarrType}"'],
    178: ['GARRISON_AVAILABLE_PLOTS', 'Player has at least {#Amount} instances of plot "{GarrPlot}" available'],
    179: ['WORLD_PVP_AREA', 'Currency source is {$Currency Source}'],
    180: ['NON_OWN_GARRISON', 'Player is in another garrison (not their own)'],
    181: ['GARRISON_ACTIVE_FOLLOWER', 'Has active Garrison follower "{GarrFollower}"'],
    182: ['DAILY_RANDOM_VALUE_MOD', 'Player daily random value mod {#Mod Value} equals {#Equals Value}'],
    183: ['HAS_MOUNT', 'Player has Mount "{Mount}"'],
    184: ['GARRISON_FOLLOWERS_ITEM_LEVEL_ABOVE', 'At least {#Followers} followers (including inactive) are at least item level {#Level} for follower type "{GarrFollowerType}"'],
    185: ['GARRISON_FOLLOWER_ON_MISSION', 'Garrison follower "{GarrFollower}" is on a mission'],
    186: ['GARRISON_MISSIONSET_INPROGRESSAVAIL', 'Player has less than {#Missions} available and in-progress missions of set "{GarrMissionSet}" in garrison type "{GarrType}"'],
    187: ['GARRISON_FOLLOWER_TYPE', 'This Garrison Follower is of type "{GarrFollowerType}"'],
    188: ['BOOST_TIME_REAL', 'Player has boosted and boost occurred < {#Hours} hours ago (real time)'],
    189: ['BOOST_TIME_GAME', 'Player has boosted and boost occurred < {#Hours} hours ago (in-game time)'],
    190: ['IS_MERCENARY', 'Player is currently Mercenary'],
    191: ['PLAYER_RACE_IS', 'Player effective race is "{ChrRaces}"'],
    192: ['TARGET_RACE_IS', 'Target effective race is "{ChrRaces}"'],
    193: ['HONOR_LEVEL', 'Honor level >= {#Level}'],
    194: ['PRESTIGE_LEVEL', 'Prestige level >= {#Level}'],
    195: ['GARRISON_MISSION_READY', 'Garrison mission "{GarrMission}" is ready to collect'],
    196: ['IS_INSTANCE_OWNER', 'Player is the instance owner (requires \'Lock Instance Owner\' LFGDungeon flag)'],
    197: ['HAS_HEIRLOOM', 'Player has heirloom "{Item}"'],
    198: ['HAS_TEAM_POINTS', 'Team has {#Points} Points'],
    199: ['HAS_TOY', 'Player has toy "{Item}"'],
    200: ['ITEM_MODIFIED_APPEARANCE', 'Player has transmog "{ItemModifiedAppearance}"'],
    201: ['GARRISON_SELECTED_TALENT', 'Garrison has talent "{GarrTalent}" selected'],
    202: ['GARRISON_RESEARCHED_TALENT', 'Garrison has talent "{GarrTalent}" researched'],
    203: ['HAS_CHARACTER_RESTRICTIONS', 'Player has restriction of type "{@CHARACTER_RESTRICTION_TYPE}"'],
    204: ['CREATED_TIME_REAL', 'Player has created their character < {#Hours} hours ago (real time)'],
    205: ['CREATED_TIME_GAME', 'Player has created their character < {#Hours} hours ago (in-game time)'],
    206: ['QUEST_INFO_ID', 'Quest has Quest Info "{QuestInfo}"'],
    207: ['GARRISON_RESEARCHING_TALENT', 'Garrison is researching talent "{GarrTalent}"'],
    208: ['ARTIFACT_APPEARANCE_SET_USED', 'Player has equipped Artifact Appearance Set "{ArtifactAppearanceSet}"'],
    209: ['CURRENCY_AMOUNT_EQUAL', 'Player has currency "{CurrencyTypes}" in amount {#Amount} exactly'],
    210: ['ITEM_MARK_FOR_SPEC', 'Minimum average item high water mark is {#Item High Water Mark} for "{$Item History Spec Match}")'],
    211: ['SCENARIO_TYPE', 'Player in scenario of type "{$Scenario Type}"'],
    212: ['ACCOUNT_EXPANSION_EQUAL', 'Player\'s auth expansion level is "{$Expansion Level}" or higher'],
    213: ['2V2_RATING', 'Player achieved at least a rating of {#Rating} in 2v2 last week player played'],
    214: ['3V3_RATING', 'Player achieved at least a rating of {#Rating} in 3v3 last week player played'],
    215: ['RBG_RATING', 'Player achieved at least a rating of {#Rating} in RBG last week player played'],
    216: ['NUM_CONNECTED_REALM_PLAYERS', 'At least {#Num Players} members of the group are from your connected realms'],
    217: ['ARTIFACT_NUM_TRAITS', 'At least {#Num Traits} traits have been unlocked in artifact "{Item}"'],
    218: ['PARAGON_LEVEL_EQUAL_OR_GREATER', 'Paragon level >= "{#Level}"'],
    219: ['CHARSHIPMENT_READY', 'Shipment in container type "{CharShipmentContainer}" ready'],
    220: ['IN_PVP_BRAWL', 'Player is in PvP Brawl'],
    221: ['PARAGON_LEVEL_WITH_FACTION_EQUAL_OR_GREATER', 'Paragon level >= "{#Level}" with faction "{Faction}"'],
    222: ['ITEM_BONUS_LIST_QUALITY', 'Player has an item with bonus list from tree "{ItemBonusTree}" and of quality "{$Item Quality}"'],
    223: ['EMPTY_INVENTORY_SLOTS', 'Player has at least "{#Number of empty slots}" empty inventory slots'],
    224: ['ITEM_HISTORY_EVENT', 'Player has item "{Item}" in the item history of progressive event "{ProgressiveEvent}"'],
    225: ['ARTIFACT_PURCHASED_POWER_RANKS', 'Player has at least {#Purchased Ranks} ranks of {ArtifactPower} on equipped artifact'],
    226: ['USED_LEVEL_BOOST', 'Player has boosted'],
    227: ['USED_RACE_CHANGE', 'Player has race changed'],
    228: ['USED_FACTION_CHANGE', 'Player has been granted levels from Recruit a Friend'],
    229: ['IS_TOURNAMENT_REALM', 'Is Tournament Realm'],
    230: ['CAN_ACCESS_ALLIED_RACE', 'Player can access allied races'],
    231: ['ACHIEVEMENT_GLOBALLY_INCOMPLETED', 'No More Than {#Group Members} With Achievement {Achievement} In Group (true if no group)'],
    232: ['MAIN_HAND_VISIBLE_SUBCLASS', 'Player has main hand weapon of type "{$Weapon Type}"'],
    233: ['OFF_HAND_VISIBLE_SUBCLASS', 'Player has off-hand weapon of type "{$Weapon Type}"'],
    234: ['PVP_TIER', 'Player is in PvP tier {PvpTier}'],
    235: ['AZERITE_ITEM_LEVEL', 'Players\' Azerite Item is at or above level "{#Azerite Level}" '],
    236: ['ON_QUESTLINE', 'Player is on quest in questline "{QuestLine}"'],
    237: ['ON_SWG_UNLOCK_QUEST', 'Player is on quest associated with current progressive unlock group "{ScheduledWorldStateGroup}"'],
    238: ['IN_RAID_GROUP', 'Player is in raid group'],
    239: ['PVP_TIER_GREATER', 'Player is at or above "{@PVP_TIER_ENUM}" for "{@PVP_BRACKET}"'],
    240: ['QUESTLINE_ELIGIBLE', 'Player is eligible for quest in questline "{Questline}"'],
    241: ['QUESTLINE_COMPLETE', 'Player has completed questline "{Questline}"'],
    242: ['QUESTLINE_QUEST_COMPLETE', 'Player has completed "{#Quests}" quests in questline "{Questline}"'],
    243: ['QUESTLINE_PCT_COMPLETE', 'Player has completed "{#Percentage}" % of quests in questline "{Questline}"'],
    244: ['IN_WARMODE', 'Player has WarMode Enabled (regardless of shard state)'],
    245: ['IN_WARMODE_SHARD', 'Player is on a WarMode Shard'],
    246: ['WARMODE_TOGGLE', 'Player is allowed to toggle WarMode in area'],
    247: ['KEYSTONE_LEVEL', 'Mythic Plus Keystone Level Atleast {#Level}'],
    248: ['KEYSTONE_COMPLETED_IN_TIME', 'Mythic Plus Completed In Time'],
    249: ['KEYSTONE_DUNGEON', 'Mythic Plus Map Challenge Mode {MapChallengeMode}'],
    250: ['MYTHIC_SEASON_DISPLAY', 'Mythic Plus Display Season {#Season}'],
    251: ['MYTHIC_SEASON_MILESOTNE', 'Mythic Plus Milestone Season {#Season}'],
    252: ['SOURCE_DISPLAY_RACE', 'Player visible race is "{ChrRaces}"'],
    253: ['TARGET_DISPLAY_RACE', 'Target visible race is "{ChrRaces}"'],
    254: ['FRIENDSHIP_REP_REACTION_EXACT', 'Friendship rep reaction is exactly "{FriendshipRepReaction}"'],
    255: ['SOURCE_AURA_COUNT_EQUAL', 'Player has exactly {#Stacks} stacks of aura "{Spell}"'],
    256: ['TARGET_AURA_COUNT_EQUAL', 'Target has exactly {#Stacks} stacks of aura "{Spell}"'],
    257: ['SOURCE_AURA_COUNT_GREATER', 'Player has at least {#Stacks} stacks of aura "{Spell}"'],
    258: ['TARGET_AURA_COUNT_GREATER', 'Target has at least {#Stacks} stacks of aura "{Spell}"'],
    259: ['UNLOCKED_AZERITE_ESSENCE_RANK_LOWER', 'Player has Azerite Essence {AzeriteEssence} at less than rank {#rank}'],
    260: ['UNLOCKED_AZERITE_ESSENCE_RANK_EQUAL', 'Player has Azerite Essence {AzeriteEssence} at rank {#rank}'],
    261: ['UNLOCKED_AZERITE_ESSENCE_RANK_GREATER', 'Player has Azerite Essence {AzeriteEssence} at greater than rank {#rank}'],
    262: ['SOURCE_HAS_AURA_EFFECT_INDEX', 'Player has Aura {Spell} with Effect Index {#index} active'],
    263: ['SOURCE_SPECIALIZATION_ROLE', 'Player loot specialization matches role {@LFG_ROLE}'],
    264: ['SOURCE_LEVEL_120', 'Player is at max expansion level'],
    265: ['TRANSMOG_SOURCE_IS_OF_ID', 'Transmog Source is "{@TRANSMOG_SOURCE}"'],
    266: ['SELECTED_AZERITE_ESSENCE_RANK_LOWER', 'Player has Azerite Essence in slot {@AZERITE_ESSENCE_SLOT} at less than rank {#rank}'],
    267: ['SELECTED_AZERITE_ESSENCE_RANK_GREATER', 'Player has Azerite Essence in slot {@AZERITE_ESSENCE_SLOT} at greater than rank {#rank}'],
    268: ['SOURCE_LEVEL_IN_RANGE_CT', 'Player has level within Content Tuning {ContentTuning}'],
    269: ['TARGET_LEVEL_IN_RANGE_CT', 'Target has level within Content Tuning {ContentTuning}'],
    270: ['IS_SCENARIO_INITIATOR', 'Player is Scenario Initiator'],
    271: ['QUEST_IS_ON_OR_HAS_COMPLETED', 'Player is currently on or previously completed quest "{QuestV2}"'],
    272: ['SOURCE_LEVEL_GREATER_CT', 'Player has level within or above Content Tuning {ContentTuning}'],
    273: ['TARGET_LEVEL_GREATER_CT', 'Target has level within or above Content Tuning {ContentTuning}'],
    274: ['SOURCE_LEVEL_GREATER', 'Player has level within or above Level Range {LevelRange}'],
    275: ['TARGET_LEVEL_GREATER', 'Target has level within or above Level Range {LevelRange}'],
    276: ['JAILER_MAX_TOWER', 'Max Jailers Tower Level Atleast {#Level}'],
    277: ['RAF_RECRUIT_IN_PARTY', 'Grouped With Recruit'],
    278: ['RAF_RECRUITER_IN_PARTY', 'Grouped with Recruiter'],
    279: ['IS_SPECIALIZATION', 'Specialization is "{ChrSpecialization}"'],
    280: ['MAP_OR_COSMETIC_MAP', 'Player is on map or cosmetic child map "{Map}"'],
    281: ['HAS_SL_PREPURCHASE', 'Player can access Shadowlands (9.0) prepurchase content'],
    282: ['HAS_ENTITLEMENT', 'Player has entitlement "{BattlePayDeliverable}"'],
    283: ['HAS_QUEST_SESSION', 'Player is in party sync group'],
    284: ['QUEST_PARTYSYNC_ELIGIBLE', 'Quest is eligible for party sync rewards'],
    285: ['SPECIAL_HONOR_GAIN', 'Player gained honor from source {@SPECIAL_MISC_HONOR_GAIN_SOURCE}'],
    286: ['ACTIVE_FLOOR_MIN_LEVEL', 'Active Floor Index Atleast {#Level}'],
    287: ['ACTIVE_FLOOR_DIFFICULTY', 'Active Floor Difficulty Atleast {#Level}'],
    288: ['COVENANT', 'Player is member of covenant "{Covenant}"'],
    289: ['TIME_EVENT_PASSED', 'Has time event "{TimeEvent}" passed'],
    290: ['PERMANENT_ANIMA_DIVERSION_TALENT', 'Garrison has permanent talent "{GarrTalent}"'],
    291: ['ACTIVE_SOULBIND', 'Has Active Soulbind "{Soulbind}"'],
    292: ['HAS_MEMORIZED_SPELL', 'Has memorized spell "{Spell}"'],
    293: ['HAS_APAC_SUB_REWARD', 'Player has APAC Subscription Reward 2020'],
    294: ['HAS_TBCC_WARPSTALKER_MOUNT', 'Player has TBCC:DE Warp Stalker Mount'],
    295: ['HAS_TBCC_DARKPORTAL_TOY', 'Player has TBCC:DE Dark Portal Toy'],
    296: ['HAS_TBCC_ILLIDAN_TOY', 'Player has TBCC:DE Path of Illidan Toy'],
    297: ['PLAYER_HAS_IMP_SUB_REWARD', 'Player has Imp in a Ball Toy Subscription Reward'],
    298: ['PLAYER_IN_AREA_GROUP', 'Player is in area group "{AreaGroup}"'],
    299: ['TARGET_IN_AREA_GROUP', 'Target is in area group "{AreaGroup}"'],
    300: ['SOURCE_IN_SPECIFIC_CHROMIE_TIME', 'Player has selected Chromie Time ID "{UiChromieTimeExpansionInfo}"'],
    301: ['SOURCE_IN_ANY_CHROMIE_TIME', 'Player has selected ANY Chromie Time ID'],
    302: ['ITEM_IS_AZERITE_ARMOR', 'Item is Azerite Armor'],
    303: ['RUNEFORGED_LEGENDARY_KNOWN', 'Player Has Runeforge Power "{RuneforgeLegendaryAbility}"'],
    304: ['IS_IN_CHROMIE_TIME', 'Player is Chromie Time for Scaling'],
    305: ['IS_RAF_RECRUIT', 'Is RAF recruit'],
    306: ['ALL_IN_GROUP_ACH', 'All Players In Group Have Achievement "{Achievement}"'],
    307: ['SOULBIND_CONDUIT_RANK', 'Player has Conduit "{SoulbindConduit}" at Rank {#Rank} or Higher'],
    308: ['SHAPESHIFT_FORM_CUSTOMIZATION_DISPLAY', 'Player has chosen {CreatureDisplayInfo} for shapeshift form {SpellShapeshiftForm}'],
    309: ['SOULBIND_MIN_CONDUITS_AT_RANK', 'Player has at least {#Level} Conduits at Rank {#Rank} or higher.'],
    310: ['IS_RESTRICTED_ACCOUNT', 'Player is a Restricted Account'],
    311: ['SOURCE_FLYING', 'Player is flying'],
    312: ['LAST_SCENARIO_STEP', 'Player is on the last step of a Scenario'],
    313: ['WEEKLY_REWARD_AVAIL', 'Player has weekly rewards available'],
    314: ['TARGET_MEMBER_OF_COVENANT', 'Target is member of covenant "{Covenant}"'],
    315: ['HAS_TBC_CE', 'Player has TBC Collector\'s Edition'],
    316: ['HAS_WOTLK_CE', 'Player has Wrath Collector\'s Edition'],
    317: ['GARRISON_TALENT_RESEARCHED_AND_ACTIVE', 'Garrison has talent "{GarrTalent}" researched and active at or above {#Rank}'],
    318: ['CURRENCY_SPENT_IN_GARRTALENT_TREE', 'Currency {CurrencyTypes} Spent on Garrison Talent Research in Tree {GarrTalentTree} is greater than or equal to {#Quantity}'],
    319: ['RENOWN_CATCHUP_ACTIVE', 'Renown Catchup Active'],
    320: ['RAPID_RENOWN_CATCHUP_ACTIVE', 'Rapid Renown Catchup Active'],
    321: ['MYTHIC_PLUS_RATING_EQ_OR_HIGHER', 'Player has Mythic+ Rating of at least "{#DungeonScore}"'],
    322: ['MYTHIC_PLUS_RUN_COUNT_EQ_OR_HIGHER', 'Player has completed at least "{#MythicKeystoneRuns}" Mythic+ runs in current expansion'],
    323: ['PLAYER_HAS_CUSTOMIZATION_CHOICE', 'Player has Customization Choice "{ChrCustomizationChoice}"'],
    324: ['PLAYER_HAS_WEEKLY_PVPTIER_WIN', 'Player has best weekly win in PVP tier {PvpTier}'],
    325: ['PLAYER_HAS_WEEKLY_PVPTIER_WIN_EQ_OR_HIGHER', 'Player has best weekly win at or above "{@PVP_TIER_ENUM}" for "{@PVP_BRACKET}"'],
    326: ['HAS_VANILLA_CE', 'Player has Vanilla Collector\'s Edition'],
    // 327: ['', ''], // Bag related
    329: ['DISPLAY_SEASON_UNK', 'Display Season (unk)'],
};

const itemStatType = {
    0: 'MANA',
    1: 'HEALTH',
    2: 'ENDURANCE',
    3: 'AGILITY',
    4: 'STRENGTH',
    5: 'INTELLECT',
    6: 'SPIRIT_UNUSED',        // Removed in 7.3.0
    7: 'STAMINA',
    8: 'ENERGY',
    9: 'RAGE',
    10: 'FOCUS',
    11: 'WEAPON_SKILL_RATING_OBSOLETE',
    12: 'DEFENSE_SKILL_RATING',
    13: 'DODGE_RATING',
    14: 'PARRY_RATING',
    15: 'BLOCK_RATING',
    16: 'HIT_MELEE_RATING',
    17: 'HIT_RANGED_RATING',
    18: 'HIT_SPELL_RATING',
    19: 'CRIT_MELEE_RATING',
    20: 'CRIT_RANGED_RATING',
    21: 'CRIT_SPELL_RATING',
    22: 'CORRUPTION',
    23: 'CORRUPTION_RESISTANCE',
    24: 'MODIFIED_CRAFTING_STAT_1',
    25: 'MODIFIED_CRAFTING_STAT_2',
    26: 'CRIT_TAKEN_RANGED_RATING', // Removed
    27: 'CRIT_TAKEN_SPELL_RATING', // Removed
    28: 'HASTE_MELEE_RATING',        // Removed
    29: 'HASTE_RANGED_RATING',        // Removed
    30: 'HASTE_SPELL_RATING',        // Removed
    31: 'HIT_RATING',
    32: 'CRIT_RATING',
    33: 'HIT_TAKEN_RATING', // Removed
    34: 'CRIT_TAKEN_RATING', // Removed
    35: 'RESILIENCE_RATING',
    36: 'HASTE_RATING',
    37: 'EXPERTISE_RATING',
    38: 'ATTACK_POWER',
    39: 'RANGED_ATTACK_POWER',
    40: 'VERSATILITY',
    41: 'SPELL_HEALING_DONE',
    42: 'SPELL_DAMAGE_DONE',
    43: 'MANA_REGENERATION', // Removed
    44: 'ARMOR_PENETRATION_RATING', // Removed
    45: 'SPELL_POWER',
    46: 'HEALTH_REGEN',
    47: 'SPELL_PENETRATION',
    48: 'BLOCK_VALUE', // Removed
    49: 'MASTERY_RATING',
    50: 'EXTRA_ARMOR',
    51: 'FIRE_RESISTANCE',
    52: 'FROST_RESISTANCE',
    53: 'HOLY_RESISTANCE',
    54: 'SHADOW_RESISTANCE',
    55: 'NATURE_RESISTANCE',
    56: 'ARCANE_RESISTANCE',
    57: 'PVP_POWER',
    58: 'CR_AMPLIFY',                // Deprecated
    59: 'CR_MULTISTRIKE',        // Deprecated
    60: 'CR_READINESS',                // Deprecated
    61: 'CR_SPEED',
    62: 'CR_LIFESTEAL',
    63: 'CR_AVOIDANCE',
    64: 'CR_STURDINESS',
    // 65: 'CR_UNUSED_7',
    // 66: 'CR_CLEAVE',
    // 67: 'CR_UNUSED_9',
    // 68: 'CR_UNUSED_10',
    // 69: 'CR_UNUSED_11',
    // 70: 'CR_UNUSED_12',
    71: 'AGI_STR_INT',
    72: 'AGI_STR',
    73: 'AGI_INT',
    74: 'STR_INT',
    75: 'PROFESSION_INSPIRATION',
    76: 'PROFESSION_RESOURCEFULNESS',
    77: 'PROFESSION_FINESSE',
    78: 'PROFESSION_DEFTNESS',
    79: 'PROFESSION_PERCEPTION',
    80: 'PROFESSION_CRAFTING_SPEED',
    81: 'PROFESSION_MULTICRAFT'
};

const itemPrettyStatType = {
    0: 'Mana',
    1: 'Health',
    3: 'Agility',
    4: 'Strength',
    5: 'Intellect',
    6: 'Spirit',
    7: 'Stamina',
    12: 'Defense',
    13: 'Dodge',
    14: 'Parry',
    15: 'Block',
    16: 'Hit (Melee)',
    17: 'Hit (Ranged)',
    18: 'Hit (Spell)',
    19: 'Crit (Melee)',
    20: 'Crit (Ranged)',
    21: 'Crit (Spell)',
    22: 'Corruption',
    23: 'Corruption Resistance',
    24: 'Random Stat 1',
    25: 'Random Stat 2',
    26: 'Critical Strike Avoidance (Ranged)',
    27: 'Critical Strike Avoidance (Spell)',
    28: 'Haste (Melee)',
    29: 'Haste (Ranged)',
    30: 'Haste (Spell)',
    31: 'Hit',
    32: 'Critical Strike',
    33: 'Hit Avoidance',
    34: 'Critical Strike Avoidance',
    35: 'Resilience',
    36: 'Haste',
    37: 'Expertise',
    38: 'Attack Power',
    39: 'Attack Power (Ranged)',
    40: 'Versatility',
    41: 'Bonus Healing',
    42: 'Bonus Damage',
    43: 'Mana Regeneration',
    44: 'Armor Penetration',
    45: 'Spell Power',
    46: 'Health Regen',
    47: 'Spell Penetration',
    48: 'Block',
    49: 'Mastery',
    50: 'Bonus Armor',
    51: 'Fire Resistance',
    52: 'Frost Resistance',
    53: 'Holy Resistance',
    54: 'Shadow Resistance',
    55: 'Nature Resistance',
    56: 'Arcane Resistance',
    57: 'PvP Power',
    58: 'Amplify',
    59: 'Multistrike',
    60: 'Readiness',
    61: 'Speed',
    62: 'Lifesteal',
    63: 'Avoidance',
    64: 'Sturdiness',
    65: 'Unused (7)',
    66: 'Cleave',
    67: 'Versatility',
    68: 'Unused (10)',
    69: 'Unused (11)',
    70: 'Unused (12)',
    71: 'Agility | Strength | Intellect',
    72: 'Agility | Strength',
    73: 'Agility | Intellect',
    74: 'Strength | Intellect'
};

const spellEffectName = {
    1: 'INSTAKILL',
    2: 'SCHOOL_DAMAGE',
    3: 'DUMMY',
    4: 'PORTAL_TELEPORT',
    5: 'UNK_ITEM_MOD',
    6: 'APPLY_AURA',
    7: 'ENVIRONMENTAL_DAMAGE',
    8: 'POWER_DRAIN',
    9: 'HEALTH_LEECH',
    10: 'HEAL',
    11: 'BIND',
    12: 'PORTAL',
    13: 'RITUAL_BASE',
    14: 'INCREASE_CURRENCY_CAP',
    15: 'RITUAL_ACTIVATE_PORTAL',
    16: 'QUEST_COMPLETE',
    17: 'WEAPON_DAMAGE_NOSCHOOL',
    18: 'RESURRECT',
    19: 'ADD_EXTRA_ATTACKS',
    20: 'DODGE',
    21: 'EVADE',
    22: 'PARRY',
    23: 'BLOCK',
    24: 'CREATE_ITEM',
    25: 'WEAPON',
    26: 'DEFENSE',
    27: 'PERSISTENT_AREA_AURA',
    28: 'SUMMON',
    29: 'LEAP',
    30: 'ENERGIZE',
    31: 'WEAPON_PERCENT_DAMAGE',
    32: 'TRIGGER_MISSILE',
    33: 'OPEN_LOCK',
    34: 'SUMMON_CHANGE_ITEM',
    35: 'APPLY_AREA_AURA_PARTY',
    36: 'LEARN_SPELL',
    37: 'SPELL_DEFENSE',
    38: 'DISPEL',
    39: 'LANGUAGE',
    40: 'DUAL_WIELD',
    41: 'JUMP',
    42: 'JUMP_DEST',
    43: 'TELEPORT_UNITS_FACE_CASTER',
    44: 'SKILL_STEP',
    45: 'PLAY_MOVIE',
    46: 'SPAWN',
    47: 'TRADE_SKILL',
    48: 'STEALTH',
    49: 'DETECT',
    50: 'TRANS_DOOR',
    51: 'FORCE_CRITICAL_HIT',
    52: 'SET_MAX_BATTLE_PET_COUNT',
    53: 'ENCHANT_ITEM',
    54: 'ENCHANT_ITEM_TEMPORARY',
    55: 'TAMECREATURE',
    56: 'SUMMON_PET',
    57: 'LEARN_PET_SPELL',
    58: 'WEAPON_DAMAGE',
    59: 'CREATE_RANDOM_ITEM',
    60: 'PROFICIENCY',
    61: 'SEND_EVENT',
    62: 'POWER_BURN',
    63: 'THREAT',
    64: 'TRIGGER_SPELL',
    65: 'APPLY_AREA_AURA_RAID',
    66: 'RECHARGE_ITEM',
    67: 'HEAL_MAX_HEALTH',
    68: 'INTERRUPT_CAST',
    69: 'DISTRACT',
    70: 'PULL',
    71: 'PICKPOCKET',
    72: 'ADD_FARSIGHT',
    73: 'UNTRAIN_TALENTS',
    74: 'APPLY_GLYPH',
    75: 'HEAL_MECHANICAL',
    76: 'SUMMON_OBJECT_WILD',
    77: 'SCRIPT_EFFECT',
    78: 'ATTACK',
    79: 'SANCTUARY',
    80: 'ADD_COMBO_POINTS',
    81: 'PUSH_ABILITY_TO_ACTION_BAR',
    82: 'BIND_SIGHT',
    83: 'DUEL',
    84: 'STUCK',
    85: 'SUMMON_PLAYER',
    86: 'ACTIVATE_OBJECT',
    87: 'GAMEOBJECT_DAMAGE',
    88: 'GAMEOBJECT_REPAIR',
    89: 'GAMEOBJECT_SET_DESTRUCTION_STATE',
    90: 'KILL_CREDIT',
    91: 'THREAT_ALL',
    92: 'ENCHANT_HELD_ITEM',
    93: 'FORCE_DESELECT',
    94: 'SELF_RESURRECT',
    95: 'SKINNING',
    96: 'CHARGE',
    97: 'CAST_BUTTON',
    98: 'KNOCK_BACK',
    99: 'DISENCHANT',
    100: 'INEBRIATE',
    101: 'FEED_PET',
    102: 'DISMISS_PET',
    103: 'REPUTATION',
    104: 'SUMMON_OBJECT_SLOT1',
    105: 'SURVEY',
    106: 'CHANGE_RAID_MARKER',
    107: 'SHOW_CORPSE_LOOT',
    108: 'DISPEL_MECHANIC',
    109: 'RESURRECT_PET',
    110: 'DESTROY_ALL_TOTEMS',
    111: 'DURABILITY_DAMAGE',
    114: 'ATTACK_ME',
    115: 'DURABILITY_DAMAGE_PCT',
    116: 'SKIN_PLAYER_CORPSE',
    117: 'SPIRIT_HEAL',
    118: 'SKILL',
    119: 'APPLY_AREA_AURA_PET',
    120: 'TELEPORT_GRAVEYARD',
    121: 'NORMALIZED_WEAPON_DMG',
    123: 'SEND_TAXI',
    124: 'PULL_TOWARDS',
    125: 'MODIFY_THREAT_PERCENT',
    126: 'STEAL_BENEFICIAL_BUFF',
    127: 'PROSPECTING',
    128: 'APPLY_AREA_AURA_FRIEND',
    129: 'APPLY_AREA_AURA_ENEMY',
    130: 'REDIRECT_THREAT',
    131: 'PLAY_SOUND',
    132: 'PLAY_MUSIC',
    133: 'UNLEARN_SPECIALIZATION',
    134: 'KILL_CREDIT2',
    135: 'CALL_PET',
    136: 'HEAL_PCT',
    137: 'ENERGIZE_PCT',
    138: 'LEAP_BACK',
    139: 'CLEAR_QUEST',
    140: 'FORCE_CAST',
    141: 'FORCE_CAST_WITH_VALUE',
    142: 'TRIGGER_SPELL_WITH_VALUE',
    143: 'APPLY_AREA_AURA_OWNER',
    144: 'KNOCK_BACK_DEST',
    145: 'PULL_TOWARDS_DEST',
    146: 'ACTIVATE_RUNE',
    147: 'QUEST_FAIL',
    148: 'TRIGGER_MISSILE_SPELL_WITH_VALUE',
    149: 'CHARGE_DEST',
    150: 'QUEST_START',
    151: 'TRIGGER_SPELL_2',
    152: 'SUMMON_RAF_FRIEND',
    153: 'CREATE_TAMED_PET',
    154: 'DISCOVER_TAXI',
    155: 'TITAN_GRIP',
    156: 'ENCHANT_ITEM_PRISMATIC',
    157: 'CREATE_LOOT',
    158: 'MILLING',
    159: 'ALLOW_RENAME_PET',
    160: 'FORCE_CAST_2',
    161: 'TALENT_SPEC_COUNT',
    162: 'TALENT_SPEC_SELECT',
    163: 'OBLITERATE_ITEM',
    164: 'REMOVE_AURA',
    165: 'DAMAGE_FROM_MAX_HEALTH_PCT',
    166: 'GIVE_CURRENCY',
    167: 'UPDATE_PLAYER_PHASE',
    168: 'ALLOW_CONTROL_PET',
    169: 'DESTROY_ITEM',
    170: 'UPDATE_ZONE_AURAS_AND_PHASES',
    171: 'SUMMON_PERSONAL_GAMEOBJECT',
    172: 'RESURRECT_WITH_AURA',
    173: 'UNLOCK_GUILD_VAULT_TAB',
    174: 'APPLY_AURA_ON_PET',
    176: 'SANCTUARY_2',
    179: 'CREATE_AREATRIGGER',
    180: 'UPDATE_AREATRIGGER',
    181: 'REMOVE_TALENT',
    182: 'DESPAWN_AREATRIGGER',
    184: 'REPUTATION_2',
    187: 'RANDOMIZE_ARCHAEOLOGY_DIGSITES',
    189: 'LOOT',
    191: 'TELEPORT_TO_DIGSITE',
    192: 'UNCAGE_BATTLEPET',
    193: 'START_PET_BATTLE',
    198: 'PLAY_SCENE',
    200: 'HEAL_BATTLEPET_PCT',
    201: 'ENABLE_BATTLE_PETS',
    202: 'APPLY_AURA_ON_?',
    204: 'CHANGE_BATTLEPET_QUALITY',
    205: 'LAUNCH_QUEST_CHOICE',
    206: 'ALTER_ITEM',
    207: 'LAUNCH_QUEST_TASK',
    210: 'LEARN_GARRISON_BUILDING',
    211: 'LEARN_GARRISON_SPECIALIZATION',
    214: 'CREATE_GARRISON',
    215: 'UPGRADE_CHARACTER_SPELLS',
    216: 'CREATE_SHIPMENT',
    217: 'UPGRADE_GARRISON',
    219: 'CREATE_CONVERSATION',
    220: 'ADD_GARRISON_FOLLOWER',
    222: 'CREATE_HEIRLOOM_ITEM',
    223: 'CHANGE_ITEM_BONUSES',
    224: 'ACTIVATE_GARRISON_BUILDING',
    225: 'GRANT_BATTLEPET_LEVEL',
    227: 'TELEPORT_TO_LFG_DUNGEON',
    229: 'SET_FOLLOWER_QUALITY',
    230: 'INCREASE_FOLLOWER_ITEM_LEVEL',
    231: 'INCREASE_FOLLOWER_EXPERIENCE',
    232: 'REMOVE_PHASE',
    233: 'RANDOMIZE_FOLLOWER_ABILITIES',
    236: 'GIVE_EXPERIENCE',
    237: 'GIVE_RESTED_EXPERIENCE_BONUS',
    238: 'INCREASE_SKILL',
    239: 'END_GARRISON_BUILDING_CONSTRUCTION',
    240: 'GIVE_ARTIFACT_POWER',
    242: 'GIVE_ARTIFACT_POWER_NO_BONUS',
    243: 'APPLY_ENCHANT_ILLUSION',
    244: 'LEARN_FOLLOWER_ABILITY',
    245: 'UPGRADE_HEIRLOOM',
    246: 'FINISH_GARRISON_MISSION',
    247: 'ADD_GARRISON_MISSION',
    248: 'FINISH_SHIPMENT',
    249: 'FORCE_EQUIP_ITEM',
    250: 'TAKE_SCREENSHOT',
    251: 'SET_GARRISON_CACHE_SIZE',
    252: 'TELEPORT_UNITS',
    253: 'GIVE_HONOR',
    255: 'LEARN_TRANSMOG_SET',
    258: 'MODIFY_KEYSTONE',
    259: 'RESPEC_AZERITE_EMPOWERED_ITEM',
    260: 'SUMMON_STABLED_PET',
    261: 'SCRAP_ITEM',
    263: 'REPAIR_ITEM',
    264: 'REMOVE_GEM',
    265: 'LEARN_AZERITE_ESSENCE_POWER',
    268: 'APPLY_MOUNT_EQUIPMENT',
    269: 'UPGRADE_ITEM',
    271: 'APPLY_AREA_AURA_PARTY_NONRANDOM',
    272: 'SET_COVENANT',
    273: 'CRAFT_RUNEFORGE_LEGENDARY',
    276: 'LEARN_TRANSMOG_ILLUSION',
    277: 'SET_CHROMIE_TIME',
    279: 'LEARN_GARR_TALENT',
    281: 'LEARN_SOULBIND_CONDUIT',
    282: 'CONVERT_ITEMS_TO_CURRENCY',
    // 283: '',
    // 284: '',
};

const charSectionType = {
    0: 'Skin',
    1: 'Face',
    2: 'FacialHair',
    3: 'Hair',
    4: 'Underwear',
    5: 'HDSkin',
    6: 'HDFace',
    7: 'HDFacialHair',
    8: 'HDHair',
    9: 'HDUnderwear',
    10: 'Custom1',
    11: 'HDCustom1',
    12: 'Custom2',
    13: 'HDCustom2',
    14: 'Custom3',
    15: 'HDCustom3'
}

const charSex = {
    0: 'Male',
    1: 'Female'
}

const uiMapType = {
    0: 'Cosmic',
    1: 'World',
    2: 'Continent',
    3: 'Zone',
    4: 'Dungeon',
    5: 'Micro-Dungeon',
    6: 'Orphan'
}

const componentSection = {
    0: 'ArmUpper',
    1: 'ArmLower',
    2: 'Hand',
    3: 'TorsoUpper',
    4: 'TorsoLower',
    5: 'LegUpper',
    6: 'LegLower',
    7: 'Foot',
    8: 'Accessory',
    9: 'ScalpUpper',
    10: 'ScalpLower',
    11: 'UNK0',
    12: 'Tattoo unk0',
    13: 'Tattoo unk1'
}

const geosetType = {
    0: 'Skin/Hair',
    1: 'Face 1',
    2: 'Face 2',
    3: 'Face 3',
    4: 'Gloves',
    5: 'Boots',
    6: 'Tail',
    7: 'Ears',
    8: 'Sleeves',
    9: 'Kneepads',
    10: 'Chest',
    11: 'Pants',
    12: 'Tabard',
    13: 'Trousers',
    14: 'DH Loincloth',
    15: 'Cloak',
    16: 'Mechagnome Chin',
    17: 'Eyeglows',
    18: 'Belt',
    19: 'Bone',
    20: 'Feet',
    22: 'Torso',
    23: 'Hand attachment',
    24: 'Head attachment',
    25: 'DH Blindfolds',
    29: 'Mechagnome Arms/Hands',
    30: 'Mechagnome Legs',
    31: 'Mechagnome Feet',
    32: 'Face',
    33: 'Eyes',
    34: 'Eyebrows',
    35: 'Earrings',
    36: 'Necklace',
    37: 'Headdress',
    38: 'Tails',
    39: 'Vines',
    40: 'Tusks',
    41: 'Noses',
    42: 'Hair decoration',
    43: 'Horn decoration'
}

const chrCustomizationReqType = {
    1: 'ClassReq',
    2: 'NPC',
    3: 'ChrCustomizationReqChoice',
    4: 'Transmog'
}

const uiCustomizationType = {
    0: 'Skin',
    1: 'Face',
    2: 'Hair',
    3: 'HairColor',
    4: 'FacialHair',
    5: 'CustomOptionTattoo',
    6: 'CustomOptionHorn',
    7: 'CustomOptionFacewear',
    8: 'CustomOptionTattooColor'
}

const effectAuraType = {
    0: 'NONE',
    1: 'BIND_SIGHT',
    2: 'MOD_POSSESS',
    3: 'PERIODIC_DAMAGE',
    4: 'DUMMY',
    5: 'MOD_CONFUSE',
    6: 'MOD_CHARM',
    7: 'MOD_FEAR',
    8: 'PERIODIC_HEAL',
    9: 'MOD_ATTACKSPEED',
    10: 'MOD_THREAT',
    11: 'MOD_TAUNT',
    12: 'MOD_STUN',
    13: 'MOD_DAMAGE_DONE',
    14: 'MOD_DAMAGE_TAKEN',
    15: 'DAMAGE_SHIELD',
    16: 'MOD_STEALTH',
    17: 'MOD_STEALTH_DETECT',
    18: 'MOD_INVISIBILITY',
    19: 'MOD_INVISIBILITY_DETECT',
    20: 'OBS_MOD_HEALTH',
    21: 'OBS_MOD_POWER',
    22: 'MOD_RESISTANCE',
    23: 'PERIODIC_TRIGGER_SPELL',
    24: 'PERIODIC_ENERGIZE',
    25: 'MOD_PACIFY',
    26: 'MOD_ROOT',
    27: 'MOD_SILENCE',
    28: 'REFLECT_SPELLS',
    29: 'MOD_STAT',
    30: 'MOD_SKILL',
    31: 'MOD_INCREASE_SPEED',
    32: 'MOD_INCREASE_MOUNTED_SPEED',
    33: 'MOD_DECREASE_SPEED',
    34: 'MOD_INCREASE_HEALTH',
    35: 'MOD_INCREASE_ENERGY',
    36: 'MOD_SHAPESHIFT',
    37: 'EFFECT_IMMUNITY',
    38: 'STATE_IMMUNITY',
    39: 'SCHOOL_IMMUNITY',
    40: 'DAMAGE_IMMUNITY',
    41: 'DISPEL_IMMUNITY',
    42: 'PROC_TRIGGER_SPELL',
    43: 'PROC_TRIGGER_DAMAGE',
    44: 'TRACK_CREATURES',
    45: 'TRACK_RESOURCES',
    // 46: '46',
    47: 'MOD_PARRY_PERCENT',
    // 48: '48',
    49: 'MOD_DODGE_PERCENT',
    50: 'MOD_CRITICAL_HEALING_AMOUNT',
    51: 'MOD_BLOCK_PERCENT',
    52: 'MOD_WEAPON_CRIT_PERCENT',
    53: 'PERIODIC_LEECH',
    54: 'MOD_HIT_CHANCE',
    55: 'MOD_SPELL_HIT_CHANCE',
    56: 'TRANSFORM',
    57: 'MOD_SPELL_CRIT_CHANCE',
    58: 'MOD_INCREASE_SWIM_SPEED',
    59: 'MOD_DAMAGE_DONE_CREATURE',
    60: 'MOD_PACIFY_SILENCE',
    61: 'MOD_SCALE',
    62: 'PERIODIC_HEALTH_FUNNEL',
    63: 'MOD_ADDITIONAL_POWER_COST',
    64: 'PERIODIC_MANA_LEECH',
    65: 'MOD_CASTING_SPEED_NOT_STACK',
    66: 'FEIGN_DEATH',
    67: 'MOD_DISARM',
    68: 'MOD_STALKED',
    69: 'SCHOOL_ABSORB',
    70: 'EXTRA_ATTACKS',
    71: 'MOD_SPELL_CRIT_CHANCE_SCHOOL',
    72: 'MOD_POWER_COST_SCHOOL_PCT',
    73: 'MOD_POWER_COST_SCHOOL',
    74: 'REFLECT_SPELLS_SCHOOL',
    75: 'MOD_LANGUAGE',
    76: 'FAR_SIGHT',
    77: 'MECHANIC_IMMUNITY',
    78: 'MOUNTED',
    79: 'MOD_DAMAGE_PERCENT_DONE',
    80: 'MOD_PERCENT_STAT',
    81: 'SPLIT_DAMAGE_PCT',
    82: 'WATER_BREATHING',
    83: 'MOD_BASE_RESISTANCE',
    84: 'MOD_REGEN',
    85: 'MOD_POWER_REGEN',
    86: 'CHANNEL_DEATH_ITEM',
    87: 'MOD_DAMAGE_PERCENT_TAKEN',
    88: 'MOD_HEALTH_REGEN_PERCENT',
    89: 'PERIODIC_DAMAGE_PERCENT',
    // 90: '90',
    91: 'MOD_DETECT_RANGE',
    92: 'PREVENTS_FLEEING',
    93: 'MOD_UNATTACKABLE',
    94: 'INTERRUPT_REGEN',
    95: 'GHOST',
    96: 'SPELL_MAGNET',
    97: 'MANA_SHIELD',
    98: 'MOD_SKILL_TALENT',
    99: 'MOD_ATTACK_POWER',
    100: 'AURAS_VISIBLE',
    101: 'MOD_RESISTANCE_PCT',
    102: 'MOD_MELEE_ATTACK_POWER_VERSUS',
    103: 'MOD_TOTAL_THREAT',
    104: 'WATER_WALK',
    105: 'FEATHER_FALL',
    106: 'HOVER',
    107: 'ADD_FLAT_MODIFIER',
    108: 'ADD_PCT_MODIFIER',
    109: 'ADD_TARGET_TRIGGER',
    110: 'MOD_POWER_REGEN_PERCENT',
    111: 'ADD_CASTER_HIT_TRIGGER',
    112: 'OVERRIDE_CLASS_SCRIPTS',
    113: 'MOD_RANGED_DAMAGE_TAKEN',
    114: 'MOD_RANGED_DAMAGE_TAKEN_PCT',
    115: 'MOD_HEALING',
    116: 'MOD_REGEN_DURING_COMBAT',
    117: 'MOD_MECHANIC_RESISTANCE',
    118: 'MOD_HEALING_PCT',
    119: 'PVP_TALENTS',
    120: 'UNTRACKABLE',
    121: 'EMPATHY',
    122: 'MOD_OFFHAND_DAMAGE_PCT',
    123: 'MOD_TARGET_RESISTANCE',
    124: 'MOD_RANGED_ATTACK_POWER',
    125: 'MOD_MELEE_DAMAGE_TAKEN',
    126: 'MOD_MELEE_DAMAGE_TAKEN_PCT',
    127: 'RANGED_ATTACK_POWER_ATTACKER_BONUS',
    128: 'MOD_POSSESS_PET',
    129: 'MOD_SPEED_ALWAYS',
    130: 'MOD_MOUNTED_SPEED_ALWAYS',
    131: 'MOD_RANGED_ATTACK_POWER_VERSUS',
    132: 'MOD_INCREASE_ENERGY_PERCENT',
    133: 'MOD_INCREASE_HEALTH_PERCENT',
    134: 'MOD_MANA_REGEN_INTERRUPT',
    135: 'MOD_HEALING_DONE',
    136: 'MOD_HEALING_DONE_PERCENT',
    137: 'MOD_TOTAL_STAT_PERCENTAGE',
    138: 'MOD_MELEE_HASTE',
    139: 'FORCE_REACTION',
    140: 'MOD_RANGED_HASTE',
    141: 'MOD_RANGED_HASTE_QUIVER', // Attack speed bonus from quiver/ammo pouch
    142: 'MOD_BASE_RESISTANCE_PCT',
    143: 'MOD_RESISTANCE_EXCLUSIVE',
    144: 'SAFE_FALL',
    145: 'MOD_PET_TALENT_POINTS',
    146: 'ALLOW_TAME_PET_TYPE',
    147: 'MECHANIC_IMMUNITY_MASK',
    148: 'RETAIN_COMBO_POINTS',
    149: 'REDUCE_PUSHBACK',
    150: 'MOD_SHIELD_BLOCKVALUE_PCT',
    151: 'TRACK_STEALTHED',
    152: 'MOD_DETECTED_RANGE',
    // 153: '153',
    154: 'MOD_STEALTH_LEVEL',
    155: 'MOD_WATER_BREATHING',
    156: 'MOD_REPUTATION_GAIN',
    157: 'PET_DAMAGE_MULTI',
    158: 'MOD_SHIELD_BLOCKVALUE',
    159: 'NO_PVP_CREDIT',
    // 160: '160',
    161: 'MOD_HEALTH_REGEN_IN_COMBAT',
    162: 'POWER_BURN',
    163: 'MOD_CRIT_DAMAGE_BONUS',
    // 164: '164',
    165: 'MELEE_ATTACK_POWER_ATTACKER_BONUS',
    166: 'MOD_ATTACK_POWER_PCT',
    167: 'MOD_RANGED_ATTACK_POWER_PCT',
    168: 'MOD_DAMAGE_DONE_VERSUS',
    // 169: '169',
    170: 'DETECT_AMORE',
    171: 'MOD_SPEED_NOT_STACK',
    172: 'MOD_MOUNTED_SPEED_NOT_STACK',
    // 173: '173',
    174: 'MOD_SPELL_DAMAGE_OF_STAT_PERCENT',
    175: 'MOD_SPELL_HEALING_OF_STAT_PERCENT',
    176: 'SPIRIT_OF_REDEMPTION',
    177: 'AOE_CHARM',
    // 178: '178',
    179: 'MOD_ATTACKER_SPELL_CRIT_CHANCE',
    180: 'MOD_FLAT_SPELL_DAMAGE_VERSUS',
    // 181: '181',
    182: 'MOD_RESISTANCE_OF_STAT_PERCENT',
    183: 'MOD_CRITICAL_THREAT',
    184: 'MOD_ATTACKER_MELEE_HIT_CHANCE',
    185: 'MOD_ATTACKER_RANGED_HIT_CHANCE',
    186: 'MOD_ATTACKER_SPELL_HIT_CHANCE',
    187: 'MOD_ATTACKER_MELEE_CRIT_CHANCE',
    188: 'MOD_ATTACKER_RANGED_CRIT_CHANCE',
    189: 'MOD_RATING',
    190: 'MOD_FACTION_REPUTATION_GAIN',
    191: 'USE_NORMAL_MOVEMENT_SPEED',
    192: 'MOD_MELEE_RANGED_HASTE',
    193: 'MELEE_SLOW',
    194: 'MOD_TARGET_ABSORB_SCHOOL',
    195: 'MOD_TARGET_ABILITY_ABSORB_SCHOOL',
    196: 'MOD_COOLDOWN',
    197: 'MOD_ATTACKER_SPELL_AND_WEAPON_CRIT_CHANCE',
    // 198: '198',
    // 199: '199',
    200: 'MOD_XP_PCT',
    201: 'FLY',
    202: 'IGNORE_COMBAT_RESULT',
    203: 'MOD_ATTACKER_MELEE_CRIT_DAMAGE',
    204: 'MOD_ATTACKER_RANGED_CRIT_DAMAGE',
    205: 'MOD_SCHOOL_CRIT_DMG_TAKEN',
    206: 'MOD_INCREASE_VEHICLE_FLIGHT_SPEED',
    207: 'MOD_INCREASE_MOUNTED_FLIGHT_SPEED',
    208: 'MOD_INCREASE_FLIGHT_SPEED',
    209: 'MOD_MOUNTED_FLIGHT_SPEED_ALWAYS',
    210: 'MOD_VEHICLE_SPEED_ALWAYS',
    211: 'MOD_FLIGHT_SPEED_NOT_STACK',
    212: 'MOD_HONOR_GAIN_PCT',
    213: 'MOD_RAGE_FROM_DAMAGE_DEALT',
    // 214: '214',
    215: 'ARENA_PREPARATION',
    216: 'HASTE_SPELLS',
    217: 'MOD_MELEE_HASTE_2',
    218: 'HASTE_RANGED',
    219: 'MOD_MANA_REGEN_FROM_STAT',
    220: 'MOD_ABILITY_SCHOOL_MASK',
    221: 'MOD_DETAUNT',
    222: 'REMOVE_TRANSMOG_COST',
    223: 'REMOVE_BARBER_SHOP_COST',
    224: 'GAIN_TALENT',
    225: 'MOD_VISIBILITY_RANGE',
    226: 'PERIODIC_DUMMY',
    227: 'PERIODIC_TRIGGER_SPELL_WITH_VALUE',
    228: 'DETECT_STEALTH',
    229: 'MOD_AOE_DAMAGE_AVOIDANCE',
    230: 'MOD_MAX_HEALTH',
    231: 'PROC_TRIGGER_SPELL_WITH_VALUE',
    232: 'MECHANIC_DURATION_MOD',
    233: 'CHANGE_MODEL_FOR_ALL_HUMANOIDS',
    234: 'MECHANIC_DURATION_MOD_NOT_STACK',
    235: 'MOD_DISPEL_RESIST',
    236: 'CONTROL_VEHICLE',
    237: 'MOD_SPELL_DAMAGE_OF_ATTACK_POWER',
    238: 'MOD_SPELL_HEALING_OF_ATTACK_POWER',
    239: 'MOD_SCALE_2',
    240: 'MOD_EXPERTISE',
    241: 'FORCE_MOVE_FORWARD',
    242: 'MOD_SPELL_DAMAGE_FROM_HEALING',
    243: 'MOD_FACTION',
    244: 'COMPREHEND_LANGUAGE',
    245: 'MOD_AURA_DURATION_BY_DISPEL',
    246: 'MOD_AURA_DURATION_BY_DISPEL_NOT_STACK',
    247: 'CLONE_CASTER',
    248: 'MOD_COMBAT_RESULT_CHANCE',
    249: 'CONVERT_RUNE',
    250: 'MOD_INCREASE_HEALTH_2',
    251: 'MOD_ENEMY_DODGE',
    252: 'MOD_SPEED_SLOW_ALL',
    253: 'MOD_BLOCK_CRIT_CHANCE',
    254: 'MOD_DISARM_OFFHAND',
    255: 'MOD_MECHANIC_DAMAGE_TAKEN_PERCENT',
    256: 'NO_REAGENT_USE',
    257: 'MOD_TARGET_RESIST_BY_SPELL_CLASS',
    258: 'OVERRIDE_SUMMONED_OBJECT',
    // 259: '259',
    260: 'SCREEN_EFFECT',
    261: 'PHASE',
    262: 'ABILITY_IGNORE_AURASTATE',
    263: 'ALLOW_ONLY_ABILITY',
    // 264: '264',
    // 265: '265',
    // 266: '266',
    267: 'MOD_IMMUNE_AURA_APPLY_SCHOOL',
    // 268: '268',
    269: 'MOD_IGNORE_TARGET_RESIST',
    270: 'SCHOOL_MASK_DAMAGE_FROM_CASTER',
    271: 'MOD_SPELL_DAMAGE_FROM_CASTER',
    272: 'IGNORE_MELEE_RESET',
    273: 'X_RAY',
    // 274: '274',
    275: 'MOD_IGNORE_SHAPESHIFT',
    276: 'MOD_DAMAGE_DONE_FOR_MECHANIC',
    // 277: '277',
    278: 'MOD_DISARM_RANGED',
    279: 'INITIALIZE_IMAGES',
    // 280: '280',
    281: 'PROVIDE_SPELL_FOCUS',
    282: 'MOD_BASE_HEALTH_PCT',
    283: 'MOD_HEALING_RECEIVED',
    284: 'LINKED',
    285: 'MOD_ATTACK_POWER_OF_ARMOR',
    286: 'ABILITY_PERIODIC_CRIT',
    287: 'DEFLECT_SPELLS',
    288: 'IGNORE_HIT_DIRECTION',
    289: 'PREVENT_DURABILITY_LOSS',
    290: 'MOD_CRIT_PCT',
    291: 'MOD_XP_QUEST_PCT',
    292: 'OPEN_STABLE',
    293: 'OVERRIDE_SPELLS',
    294: 'PREVENT_REGENERATE_POWER',
    // 295: '295',
    296: 'SET_VEHICLE_ID',
    297: 'BLOCK_SPELL_FAMILY',
    298: 'STRANGULATE',
    // 299: '299',
    300: 'SHARE_DAMAGE_PCT',
    301: 'SCHOOL_HEAL_ABSORB',
    // 302: '302',
    303: 'MOD_DAMAGE_DONE_VERSUS_AURASTATE',
    304: 'MOD_FAKE_INEBRIATE',
    305: 'MOD_MINIMUM_SPEED',
    // 306: '306',
    307: 'CAST_WHILE_WALKING_BY_SPELL_LABEL',
    308: 'MOD_CRIT_CHANCE_FOR_CASTER_WITH_ABILITIES',
    309: 'MOD_RESILIENCE',
    310: 'MOD_CREATURE_AOE_DAMAGE_AVOIDANCE',
    // 311: '311',
    312: 'ANIM_REPLACEMENT_SET',
    // 313: '313',
    314: 'PREVENT_RESURRECTION',
    315: 'UNDERWATER_WALKING',
    316: 'SCHOOL_ABSORB_OVERKILL',
    317: 'MOD_SPELL_POWER_PCT',
    318: 'MASTERY',
    319: 'MOD_MELEE_HASTE_3',
    320: 'MOD_RANGED_HASTE_2',
    321: 'MOD_NO_ACTIONS',
    322: 'INTERFERE_TARGETTING',
    // 323: '323',
    324: 'OVERRIDE_UNLOCKED_AZERITE_ESSENCE_RANK',
    325: 'LEARN_PVP_TALENT',
    326: 'PHASE_GROUP',
    327: 'PHASE_ALWAYS_VISIBLE',
    328: 'PROC_ON_POWER_AMOUNT',
    329: 'MOD_RUNE_REGEN_SPEED',
    330: 'CAST_WHILE_WALKING',
    331: 'FORCE_WEATHER',
    332: 'OVERRIDE_ACTIONBAR_SPELLS',
    333: 'OVERRIDE_ACTIONBAR_SPELLS_TRIGGERED',
    334: 'MOD_BLIND',
    // 335: '335',
    336: 'MOD_FLYING_RESTRICTIONS',
    337: 'MOD_VENDOR_ITEMS_PRICES',
    338: 'MOD_DURABILITY_LOSS',
    339: 'MOD_CRIT_CHANCE_FOR_CASTER',
    340: 'MOD_RESURRECTED_HEALTH_BY_GUILD_MEMBER',
    341: 'MOD_SPELL_CATEGORY_COOLDOWN',
    342: 'MOD_MELEE_RANGED_HASTE_2',
    343: 'MOD_MELEE_DAMAGE_FROM_CASTER',
    344: 'MOD_AUTOATTACK_DAMAGE',
    345: 'BYPASS_ARMOR_FOR_CASTER',
    346: 'ENABLE_ALT_POWER',
    347: 'MOD_SPELL_COOLDOWN_BY_HASTE',
    348: 'DEPOSIT_BONUS_MONEY_IN_GUILD_BANK_ON_LOOT',
    349: 'MOD_CURRENCY_GAIN',
    350: 'MOD_GATHERING_ITEMS_GAINED_PERCENT',
    // 351: '351',
    // 352: '352',
    353: 'MOD_CAMOUFLAGE',
    // 354: '354',
    355: 'MOD_CASTING_SPEED',
    356: 'PROVIDE_TOTEM_CATEGORY',
    357: 'ENABLE_BOSS1_UNIT_FRAME',
    358: 'WORGEN_ALTERED_FORM',
    359: 'MOD_HEALING_DONE_VERSUS_AURASTATE',
    360: 'PROC_TRIGGER_SPELL_COPY',
    361: 'OVERRIDE_AUTOATTACK_WITH_MELEE_SPELL',
    // 362: '362',
    363: 'MOD_NEXT_SPELL',
    // 364: '364',
    365: 'MAX_FAR_CLIP_PLANE',
    366: 'OVERRIDE_SPELL_POWER_BY_AP_PCT',
    367: 'OVERRIDE_AUTOATTACK_WITH_RANGED_SPELL',
    // 368: '368',
    369: 'ENABLE_POWER_BAR_TIMER',
    370: 'SPELL_OVERRIDE_NAME_GROUP',
    // 371: '371',
    // 372: '372',
    373: 'MOD_SPEED_NO_CONTROL',
    374: 'MOD_FALL_DAMAGE_PCT',
    // 375: '375',
    376: 'MOD_CURRENCY_GAIN_FROM_SOURCE',
    377: 'CAST_WHILE_WALKING_2',
    // 378: '378',
    379: 'MOD_MANA_REGEN_PCT',
    380: 'MOD_GLOBAL_COOLDOWN_BY_HASTE',
    381: 'MOD_DAMAGE_TAKEN_FROM_CASTER_PET',
    382: 'MOD_PET_STAT_PCT',
    383: 'IGNORE_SPELL_COOLDOWN',
    // 384: '384',
    385: 'CHANCE_OVERRIDE_AUTOATTACK_WITH_SPELL_ON_SELF',
    // 386: '386',
    // 387: '387',
    388: 'MOD_TAXI_FLIGHT_SPEED',
    // 389: '389',
    // 390: '390',
    // 391: '391',
    // 392: '392',
    // 393: '393',
    394: 'SHOW_CONFIRMATION_PROMPT',
    395: 'AREA_TRIGGER',
    396: 'PROC_ON_POWER_AMOUNT_2',
    // 397: '397',
    // 398: '398',
    399: 'MOD_TIME_RATE',
    400: 'MOD_SKILL_2',
    // 401: '401',
    402: 'MOD_POWER_DISPLAY',
    403: 'OVERRIDE_SPELL_VISUAL',
    404: 'OVERRIDE_ATTACK_POWER_BY_SP_PCT',
    405: 'MOD_RATING_PCT',
    406: 'KEYBOUND_OVERRIDE',
    407: 'MOD_FEAR_2',
    408: 'SET_ACTION_BUTTON_SPELL_COUNT',
    409: 'CAN_TURN_WHILE_FALLING',
    // 410: '410',
    411: 'MOD_MAX_CHARGES',
    // 412: '412',
    // 413: '413',
    // 414: '414',
    // 415: '415',
    416: 'MOD_COOLDOWN_BY_HASTE_REGEN',
    417: 'MOD_GLOBAL_COOLDOWN_BY_HASTE_REGEN',
    418: 'MOD_MAX_POWER',
    419: 'MOD_BASE_MANA_PCT',
    420: 'MOD_BATTLE_PET_XP_PCT',
    421: 'MOD_ABSORB_EFFECTS_DONE_PCT',
    422: 'MOD_ABSORB_EFFECTS_TAKEN_PCT',
    // 423: '423',
    424: 'CASTER_IGNORE_LOS',
    // 425: '425',
    // 426: '426',
    427: 'SCALE_PLAYER_LEVEL',
    428: 'LINKED_SUMMON',
    429: 'MOD_SUMMON_DAMAGE',
    430: 'PLAY_SCENE',
    431: 'MOD_OVERRIDE_ZONE_PVP_TYPE',
    // 432: '432', // UNUSED IN 9.0.1.34199
    // 433: '433', // UNUSED IN 9.0.1.34199
    // 434: '434', // Attacking nearby units (players/NPCs) of same faction?
    // 435: '435', // UNUSED IN 9.0.1.34199
    436: 'MOD_ENVIRONMENTAL_DAMAGE_TAKEN',
    437: 'MOD_MINIMUM_SPEED_RATE',
    438: 'PRELOAD_PHASE',
    // 439: '439', // UNUSED IN 9.0.1.34199
    440: 'MOD_MULTISTRIKE_DAMAGE',
    441: 'MOD_MULTISTRIKE_CHANCE',
    442: 'MOD_READINESS',
    443: 'MOD_LEECH',
    // 444: '444', // UNUSED IN 9.0.1.34199
    // 445: '445', // UNUSED IN 9.0.1.34199
    446: 'SPELL_AURA_ADVANCED_FLYING',
    447: 'MOD_XP_FROM_CREATURE_TYPE',
    // 448: '448', // Related to PvP rules
    // 449: '449', // UNUSED IN 9.0.1.34199
    // 450: '450', // Only used in Character Upgrade Spell Tier (156747)
    451: 'OVERRIDE_PET_SPECS',
    // 452: '452', // UNUSED IN 9.0.1.34199
    453: 'CHARGE_RECOVERY_MOD',
    454: 'CHARGE_RECOVERY_MULTIPLIER',
    455: 'MOD_ROOT_2', // Related to being immobilized/rooted
    456: 'CHARGE_RECOVERY_AFFECTED_BY_HASTE',
    457: 'CHARGE_RECOVERY_AFFECTED_BY_HASTE_REGEN',
    458: 'IGNORE_DUAL_WIELD_HIT_PENALTY',
    459: 'IGNORE_MOVEMENT_FORCES',
    460: 'RESET_COOLDOWNS_ON_DUEL_START',
    // 461: '461', // UNUSED IN 9.0.1.34199
    462: 'MOD_HEALING_AND_ABSORB_FROM_CASTER',
    463: 'CONVERT_CRIT_RATING_PCT_TO_PARRY_RATING',
    464: 'MOD_ATTACK_POWER_OF_BONUS_ARMOR',
    465: 'MOD_BONUS_ARMOR',
    466: 'MOD_BONUS_ARMOR_PCT',
    467: 'MOD_STAT_BONUS_PCT',
    468: 'TRIGGER_SPELL_ON_HEALTH_BELOW_PCT',
    469: 'SHOW_CONFIRMATION_PROMPT_WITH_DIFFICULTY',
    470: 'MOD_AURA_TIME_RATE_BY_SPELL_LABEL', // Used in spell        209618 (Expedite), EffectBasePointsF of 100, EffectMiscValue[0] of 174 or 182
    471: 'MOD_VERSATILITY',
    // 472: '472', // FIXATE?
    473: 'PREVENT_DURABILITY_LOSS_FROM_COMBAT',
    474: 'REPLACE_ITEM_BONUS_TREE', // "Upgrades", some of these removed in 8.3.0 => 9.0.1 for spell 170733? Needs ID mapping.
    475: 'ALLOW_USING_GAMEOBJECTS_WHILE_MOUNTED',
    476: 'MOD_CURRENCY_GAIN_LOOTED_PCT',
    // 477: '477', // Only set on scaling for "testing purposes" spells
    // 478: '478', // UNUSED IN 9.0.1.34199
    // 479: '479', // Set to nothing, 1 or 31
    480: 'MOD_ARTIFACT_ITEM_LEVEL', // UNUSED IN 9.0.1.34199
    481: 'CONVERT_CONSUMED_RUNE',
    // 482: '482', // Only used in S.E.L.F.I.E spells, always set to 120
    483: 'SUPPRESS_TRANSFORMS',
    // 484: '484', // INTERRUPTABLE_BY_SPELL
    485: 'MOD_MOVEMENT_FORCE_MAGNITUDE',
    // 486: '486', // OBSCURED?
    // 487: '487', // 12 spells, possibly SpellVisual* related?
    // 488: '488', // Frozen effect? Paused anim?? (195289 + movement spells)
    489: 'MOD_ALTERNATIVE_DEFAULT_LANGUAGE', // DISABLE_LANGUAGE? Only used in mercenary spells
    // 490: '490', // Only used in mercenary spells (193863/193864)
    // 491: '491', // SET_REPUTATION?
    // 492: '492', // UNUSED IN 9.0.1.34199
    // 493: '493', // SUMMON_ADDITIONAL_PET?
    494: 'SET_POWER_POINT_CHARGE',
    495: 'TRIGGER_SPELL_ON_EXPIRE',
    496: 'ALLOW_CHANGING_EQUIPMENT_IN_TORGHAST',
    497: 'MOD_ANIMA_GAIN',
    498: 'CURRENCY_LOSS_PCT_ON_DEATH',
    499: 'MOD_RESTED_XP_CONSUMPTION',
    500: 'IGNORE_SPELL_CHARGE_COOLDOWN',
    501: 'MOD_CRITICAL_DAMAGE_TAKEN_FROM_CASTER',
    502: 'MOD_VERSATILITY_DAMAGE_DONE_BENEFIT',
    503: 'MOD_VERSATILITY_HEALING_DONE_BENEFIT',
    // 504: '',
    // 505: '',
}

const spellVisualKitEffectType = {
    1: 'SpellProceduralEffectID',
    2: 'SpellVisualKitModelAttachID',
    3: 'CameraEffectID',
    4: 'CameraEffectID2',
    5: 'SoundKitID',
    6: 'SpellVisualAnimID',
    7: 'ShadowyEffectID',
    8: 'SpellEffectEmissionID',
    9: 'OutlineEffectID',
    10: 'UnitSoundType', // NOT soundkitSoundType!!!
    11: 'DissolveEffectID',
    12: 'EdgeGlowEffectID',
    13: 'BeamEffectID',
    14: 'ClientSceneEffectID',
    15: 'CloneEffectID', // Unused
    16: 'GradientEffectID',
    17: 'BarrageEffectID',
    18: 'RopeEffectID',
    19: 'SpellVisualScreenEffectID',
}

const spellLabelName = {
    // 12: '12',
    16: 'Player (???)',
    17: 'Mage',
    18: 'Priest',
    19: 'Warlock',
    20: 'Rogue',
    21: 'Druid',
    22: 'Monk',
    23: 'Hunter',
    24: 'Shaman',
    25: 'Warrior',
    26: 'Paladin',
    27: 'Death Knight',
    // 28: '28',
    // 29: '29',
    // 30: '30',
    // 31: '31',
    // 38: '38',
    // 40: '40',
    // 42: '42',
    // 52: '52',
    // 59: '59',
    // 60: '60',
    // 61: '61',
    // 62: '62',
    // 63: '63',
    // 64: '64',
    66: 'Demon Hunter',
    // 67: '67',
    // 68: '68',
    // 69: '69',
    // 71: '71',
    // 72: '72',
    // 73: '73',
    // 74: '74',
    // 75: '75',
    // 76: '76',
    // 77: '77',
    // 78: '78',
    // 79: '79',
    // 80: '80',
    // 81: '81',
    // 82: '82',
    // 83: '83',
    // 84: '84',
    // 85: '85',
    // 86: '86',
    // 87: '87',
    // 88: '88',
    // 89: '89',
    // 90: '90',
    // 91: '91',
    // 92: '92',
    // 93: '93',
    // 94: '94',
    // 95: '95',
    // 96: '96',
    // 97: '97',
    // 98: '98',
    // 99: '99',
    // 100: '100',
    // 101: '101',
    // 102: '102',
    // 103: '103',
    // 104: '104',
    // 105: '105',
    // 106: '106',
    // 107: '107',
    // 108: '108',
    // 109: '109',
    // 110: '110',
    // 111: '111',
    // 112: '112',
    // 113: '113',
    // 114: '114',
    // 115: '115',
    // 117: '117',
    // 118: '118',
    // 119: '119',
    // 120: '120',
    // 122: '122',
    // 123: '123',
    // 124: '124',
    // 125: '125',
    // 127: '127',
    // 128: '128',
    // 129: '129',
    // 130: '130',
    // 132: '132',
    // 134: '134',
    // 136: '136',
    // 137: '137',
    // 139: '139',
    // 140: '140',
    // 141: '141',
    // 142: '142',
    // 143: '143',
    // 144: '144',
    // 145: '145',
    // 146: '146',
    // 147: '147',
    // 151: '151',
    // 152: '152',
    // 153: '153',
    // 154: '154',
    // 155: '155',
    // 156: '156',
    // 157: '157',
    // 158: '158',
    // 159: '159',
    // 160: '160',
    // 161: '161',
    // 162: '162',
    // 163: '163',
    // 164: '164',
    // 165: '165',
    // 166: '166',
    // 167: '167',
    // 168: '168',
    // 170: '170',
    // 171: '171',
    // 172: '172',
    // 173: '173',
    // 174: '174',
    // 175: '175',
    // 176: '176',
    // 177: '177',
    // 178: '178',
    // 179: '179',
    // 180: '180',
    // 181: '181',
    // 182: '182',
    // 183: '183',
    // 184: '184',
    // 186: '186',
    // 187: '187',
    // 188: '188',
    // 189: '189',
    // 192: '192',
    // 193: '193',
    // 194: '194',
    // 197: '197',
    // 199: '199',
    // 201: '201',
    // 202: '202',
    // 203: '203',
    // 205: '205',
    // 207: '207',
    // 208: '208',
    // 209: '209',
    // 219: '219',
    // 228: '228',
    // 229: '229',
    // 230: '230',
    // 231: '231',
    // 232: '232',
    // 237: '237',
    // 242: '242',
    // 243: '243',
    // 247: '247',
    // 248: '248',
    // 249: '249',
    // 265: '265',
    // 275: '275',
    // 276: '276',
    // 277: '277',
    // 281: '281',
    // 282: '282',
    // 283: '283',
    // 284: '284',
    // 285: '285',
    // 286: '286',
    // 287: '287',
    // 288: '288',
    // 289: '289',
    // 291: '291',
    // 292: '292',
    // 293: '293',
    // 294: '294',
    // 295: '295',
    // 296: '296',
    // 297: '297',
    // 298: '298',
    // 299: '299',
    // 300: '300',
    // 301: '301',
    // 302: '302',
    // 303: '303',
    // 308: '308',
    // 309: '309',
    // 311: '311',
    // 312: '312',
    // 313: '313',
    // 319: '319',
    // 320: '320',
    // 322: '322',
    // 323: '323',
    // 325: '325',
    // 326: '326',
    // 327: '327',
    // 328: '328',
    // 329: '329',
    // 330: '330',
    // 331: '331',
    // 332: '332',
    // 333: '333',
    // 354: '354',
    // 359: '359',
    // 363: '363',
    // 364: '364',
    // 372: '372',
    // 373: '373',
    // 374: '374',
    // 375: '375',
    // 376: '376',
    // 378: '378',
    // 382: '382',
    // 383: '383',
    // 384: '384',
    // 385: '385',
    // 386: '386',
    // 387: '387',
    // 389: '389',
    // 391: '391',
    // 394: '394',
    // 396: '396',
    // 397: '397',
    // 399: '399',
    // 410: '410',
    // 411: '411',
    // 412: '412',
    // 413: '413',
    // 414: '414',
    // 415: '415',
    // 416: '416',
    // 417: '417',
    // 418: '418',
    // 420: '420',
    // 421: '421',
    // 422: '422',
    // 423: '423',
    // 424: '424',
    // 428: '428',
    // 429: '429',
    // 549: '549',
    // 563: '563',
    // 564: '564',
    // 565: '565',
    // 566: '566',
    // 569: '569',
    // 575: '575',
    // 577: '577',
    // 579: '579',
    // 580: '580',
    // 581: '581',
    585: 'Testimony spells',
    // 586: '586',
    // 587: '587',
    // 588: '588',
    // 590: '590',
    // 592: '592',
    // 599: '599',
    // 600: '600',
    // 602: '602',
    // 609: '609',
    // 611: '611',
    // 612: '612',
    // 613: '613',
    // 614: '614',
    615: 'Timewarp, Heroism, Drums etc',
    // 616: '616', // Hardcoded check
    // 617: '617',
    // 621: '621',
    // 623: '623',
    627: 'Torghast',
    // 629: '629',
    630: 'Anima Power',
    // 634: '634',
    // 638: '638',
    // 640: '640',
    // 641: '641',
    // 643: '643',
    // 644: '644',
    // 646: '646',
    // 647: '647',
    648: 'Hardened Azerite',
    // 649: '649',
    // 650: '650',
    // 651: '651',
    // 652: '652',
    // 653: '653',
    // 654: '654',
    // 655: '655',
    // 656: '656',
    // 657: '657',
    // 658: '658',
    // 659: '659',
    // 660: '660',
    // 661: '661',
    // 662: '662',
    // 663: '663',
    // 664: '664',
    // 665: '665',
    // 666: '666',
    // 667: '667', // PvP Hardcoded
    // 668: '668', // PvP Hardcoded
    // 669: '669', // PvP Hardcoded
    // 670: '670', // PvP Hardcoded
    // 671: '671',
    672: 'Empowered Null Barrier',
    673: 'Null Barrier',
    674: 'Null Barriers',
    675: 'Engine of X modifiers',
    676: 'Various conversation/phase spells',
    677: 'Azerite Spike',
    678: 'Unwavering Wards',
    679: 'Unwavering Ward',
    680: 'Guardian Shells',
    681: 'The Ever-Rising Tide',
    682: 'Overcharge Mana',
    683: 'Quickening',
    // 685: '685',
    // 686: '686',
    // 687: '687',
    // 688: '688',
    // 689: '689',
    // 690: '690',
    // 691: '691',
    // 692: '692',
    // 693: '693',
    // 694: '694',
    // 695: '695',
    // 698: '698',
    // 699: '699',
    // 700: '700',
    // 701: '701',
    // 702: '702',
    // 704: '704',
    // 709: '709',
    // 710: '710',
    // 711: '711',
    // 712: '712',
    // 713: '713',
    // 714: '714',
    // 715: '715',
    // 716: '716',
    // 717: '717',
    // 718: '718',
    // 719: '719',
    // 720: '720',
    // 721: '721',
    // 722: '722',
    // 723: '723',
    // 726: '726',
    // 731: '731',
    // 732: '732',
    // 733: '733',
    // 734: '734',
    // 735: '735',
    // 737: '737',
    // 738: '738',
    // 739: '739',
    // 740: '740',
    // 741: '741',
    // 742: '742',
    // 743: '743',
    // 744: '744',
    // 745: '745',
    // 746: '746', // Hardcoded check
    // 747: '747',
    // 748: '748',
    // 749: '749',
    // 750: '750',
    // 751: '751',
    // 752: '752',
    // 753: '753',
    // 754: '754',
    // 755: '755',
    756: 'Purification Protocol',
    // 757: '757',
    // 758: '758',
    // 759: '759',
    // 760: '760',
    // 768: '768',
    // 769: '769',
    // 770: '770',
    // 771: '771',
    // 773: '773',
    // 774: '774',
    // 777: '777',
    // 778: '778',
    // 779: '779',
    // 780: '780',
    // 781: '781',
    // 782: '782',
    // 783: '783',
    // 784: '784',
    // 785: '785',
    // 786: '786',
    // 795: '795',
    // 796: '796',
    // 797: '797',
    // 802: '802',
    // 803: '803',
    // 804: '804',
    // 805: '805',
    // 806: '806',
    // 807: '807',
    // 810: '810',
    // 811: '811',
    // 813: '813',
    // 814: '814',
    // 817: '817',
    // 818: '818',
    // 819: '819',
    // 820: '820',
    // 822: '822',
    // 823: '823',
    // 824: '824',
    825: 'Shroud of Resolve Rank auras',
    826: 'Azerite Essence - Worldvein Resonance',
    827: 'Gift/Servant of N\'Zoth',
    828: 'Covenant PH Abilities?',
    829: 'Receive Covenant Ability',
    830: 'Kyrian (Generic)',
    831: 'Kyrian (Deathknight)',
    832: 'Kyrian (Hunter)',
    833: 'Kyrian (Mage)',
    834: 'Kyrian (Paladin)',
    835: 'Covenant (Rogue)',
    837: 'Kyrian (Warlock)',
    838: 'Kyrian (Priest)',
    839: 'Kyrian (Warrior)',
    840: 'Kyrian (Warrior 2?)',
    841: 'Kyrian (Demon Hunter)',
    842: 'Kyrian (Rogue)',
    844: 'Kyrian (Monk)',
    845: 'Monk related',
    846: 'Kyrian (Hunter 2?)',
    847: 'Vision Madnesses',
    851: 'Vision Sanity Restoration',
    853: 'Kyrian (Priest)',
    854: 'Servant of N\'Zoth 2',
    856: 'Cyst related (dungeon/raid mechanic?)',
    858: 'Shroud of Resolve',
    859: 'Shroud of Resolve, again',
    860: 'Shroud of Resolve, but again',
    861: 'Venthyr (Warrior)',
    862: 'Muffinus messing around',
    863: 'High Noon (Druid Azerite)',
    869: 'Venthyr? (Rogue poisons?)',
    870: 'Crippling Poison (Rogue)',
    871: '[DNT] Immune To Bolster (Affix)',
    874: 'Find Weakness (?) (Rogue)',
    877: 'Eye of the Jailer Tiers',
    // 881: '881', // Many spells
    884: 'Torghast Chests',
    885: 'Felstorm/Beast Cleave (?)',
    887: 'Searing Bolt (?)',
    888: 'Venthyr (Shaman)',
    889: 'Fast Heal (?)',
    890: 'Night Fae (Generic/PH?)',
    891: 'Arcanic Pulse Detector (Torghast)',
    892: 'Alter Time',
    893: 'Alter Time 2',
    895: 'Clearcasting (Mage)',
    // 897: '897',
    // 905: '905',
    // 907: '907',
    // 908: '908',
    // 909: '909',
    // 910: '910',
    // 911: '911',
    // 912: '912',
    913: 'Kevin\'s Keyring (Soulbind)',
    914: 'Volatile Solvents',
    915: 'Ardenweald Garden',
    918: 'Hearth Kidneystone',
    919: 'Souls related (Torghast)',
    // 922: '922',
    // 923: '923',
    924: 'Ambient Sound States (All)',
    925: 'Ambient Sound States (Overrides)',
    926: 'Ambient Sound States (Nearby Threat)',
    927: 'Steward abilities/states',
    928: 'Covenant (Mage)',
    // 930: '930',
    931: 'Runecarver Legendary Abilities',
    932: 'Mage barriers?',
    933: 'Heroism (etc) exhaustions',
    934: 'Kyrian (Priest 2)',
    935: 'Disciplinary Command (Mage)',
    936: 'Ascended Nova (Mage)',
    937: 'Ascended Nova (Mage 2)',
    938: 'Brain Freeze (Mage)',
    939: 'Clearcasting 2 (Mage)',
    940: 'Hex',
    948: 'Warrior shouts',
    // 951: '951',
    952: 'Warrior cooldowns',
    954: 'Priest heals',
    958: 'Flasks',
    959: 'Well Fed(s)',
    960: 'Sinful Revelations', // (Priest? Pala?)
    961: 'Runes (DK)', // Frost?
    962: 'Runes 2 (DK)', // Unholy?
    963: 'Runes 3 (DK)', // Blood???
    965: 'Temple of Kotmogu Holding Artifact',
    966: 'Necrolord (Warlock)',
    967: 'Necrolord (Warlock 2)',
    968: 'Night Fae (Warlock)',
    969: '9.0 crafting related',
    970: 'Venthyr (Warlock)',
    971: '9.0 cooking',
    972: '9.0 crafting related (2)',
    973: 'Hold your ground 9.0',
    974: '9.0 enchanting',
    975: '9.0 inscription',
    976: '9.0 covenant',
    977: 'Wasteland Propriety soulbind',
    978: 'Kyrian Shaman',
    979: 'Denathrius abilities',
    980: 'Necrolord Shaman',
    981: 'Night Fae Shaman',
    982: 'Pelagos abilities',
    983: 'Chain Harvest (Venthyr)',
    984: 'Ancient Aftershock (Kyrian)',
    // 991: '',
    // 992: '',
    993: 'Path of Wisdom gifts',
    999: '9.0 soulbind conduits',
    1003: 'Warrior (unk 9.0)',
    1025: 'Maw 9.0',
    1027: 'Necrolord Hunter',
    1032: 'Necrolord Hunter 2',
    1033: 'Night Fae Hunter',
    1034: 'Night Fae Hunter 2',
    1035: 'Venthyr Hunter',
    1036: 'Kleia skills',
    1038: 'Windfury Totem',
    1043: 'Blizzard',
    1044: '"Pick up item x" spells',
    1305: 'Blood Shards of Domination',
    1306: 'Frost Shards of Domination',
    1307: 'Unholy Shards of Domination',
    1379: 'covenant signature abilities and other things they want to block in mage tower'
}

const unitConditionOperator = {
    1: 'EQUAL TO',
    2: 'NOT EQUAL TO',
    3: 'LESS THAN',
    4: 'LESS THAN OR EQUAL TO',
    5: 'GREATER THAN',
    6: 'GREATER THAN OR EQUAL TO',
}

const spellClassSet = {
    3: 'Mage',
    4: 'Warrior',
    5: 'Warlock',
    6: 'Priest',
    7: 'Druid',
    8: 'Rogue',
    9: 'Hunter',
    10: 'Paladin',
    11: 'Shaman',
    15: 'Death Knight',
    53: 'Monk',
    107: 'Demon Hunter',
}


// ChrModelID is already an enum that will end up at a race/gender but this is just a quick way
const tempChrModelIDEnum = {
    1: 'Human Male',
    2: 'Human Female',
    3: 'Orc Male',
    4: 'Orc Female',
    5: 'Dwarf Male',
    6: 'Dwarf Female',
    7: 'Night Elf Male',
    8: 'Night Elf Female',
    9: 'Scourge Male',
    10: 'Scourge Female',
    11: 'Tauren Male',
    12: 'Tauren Female',
    13: 'Gnome Male',
    14: 'Gnome Female',
    15: 'Troll Male',
    16: 'Troll Female',
    17: 'Goblin Male',
    18: 'Goblin Female',
    19: 'Blood Elf Male',
    20: 'Blood Elf Female',
    21: 'Draenei Male',
    22: 'Draenei Female',
    23: 'Fel Orc Male',
    24: 'Fel Orc Female',
    25: 'Naga Male',
    26: 'Naga Female',
    27: 'Broken Male',
    28: 'Broken Female',
    29: 'Skeleton Male',
    30: 'Skeleton (Fe)male',
    31: 'Vrykul Male',
    32: 'Vrykul (Fe)male',
    33: 'Tuskarr Male',
    34: 'Tuskarr Fe(male)',
    35: 'Forest Troll Male',
    36: 'Forest Troll (Fe)male',
    37: 'Taunka Male',
    38: 'Taunka (Fe)male',
    39: 'Northrend Skeleton Male',
    40: 'Northrend Skeleton (Fe)male',
    41: 'Ice Troll Male',
    42: 'Ice Troll (Fe)male',
    43: 'Worgen Male',
    44: 'Worgen Female',
    45: 'Gilnean Male',
    46: 'Gilnean Female',
    47: 'Pandaren Male',
    48: 'Pandaren Female',
    53: 'Nightborne Male',
    54: 'Nightborne Female',
    55: 'Highmountain Tauren Male',
    56: 'Highmountain Tauren Female',
    57: 'Void Elf Male',
    58: 'Void Elf Female',
    59: 'Lightforged Draenei Male',
    60: 'Lightforged Draenei Female',
    61: 'Zandalari Male',
    62: 'Zandalari Female',
    63: 'Kul Tiran Male',
    64: 'Kul Tiran Female',
    65: 'Thin Human Male',
    66: 'Thin Human (Fe)male',
    67: 'Dark Iron Dwarf Male',
    68: 'Dark Iron Dwarf Female',
    69: 'Vulpera Male',
    70: 'Vulpera Female',
    71: 'Mag\'har Orc Male',
    72: 'Mag\'har Orc Female',
    73: 'Mechagnome Male',
    74: 'Mechagnome Female',
    89: 'Dracthyr (Dragon)',
    123: 'Companion Drake',
    124: 'Companion Protodragon',
    125: 'Companion Serpent',
    126: 'Companion Wyvern',
    127: 'Dracthyr Visage Male',
    128: 'Dracthyr Visage Female',
    129: 'Companion Pterrodax',
}

const tempChrRaceIDEnum = {
    1: 'Human',
    2: 'Orc',
    3: 'Dwarf',
    4: 'Night Elf',
    5: 'Scourge',
    6: 'Tauren',
    7: 'Gnome',
    8: 'Troll',
    9: 'Goblin',
    10: 'Blood Elf',
    11: 'Draenei',
    12: 'Fel Orc',
    13: 'Naga',
    14: 'Broken',
    15: 'Skeleton',
    16: 'Vrykul',
    17: 'Tuskarr',
    18: 'Forest Troll',
    19: 'Taunka',
    20: 'Northrend Skeleton',
    21: 'Ice Troll',
    22: 'Worgen',
    23: 'Gilnean',
    24: 'Pandaren (Neutral)',
    25: 'Pandaren (Alliance)',
    26: 'Pandaren (Horde)',
    27: 'Nightborne',
    28: 'Highmountain Tauren',
    29: 'Void Elf',
    30: 'Lightforged Draenei',
    31: 'Zandalari Troll',
    32: 'Kul Tiran',
    33: 'Thin Human',
    34: 'Dark Iron Dwarf',
    35: 'Vulpera',
    36: 'Mag\'har Orc',
    37: 'Mechagnome',
    52: 'Dracthyr (Alliance)',
    70: 'Dracthyr (Horde)',
    71: 'Companion Drake',
    72: 'Companion Proto Dragon',
    73: 'Companion Serpent',
    74: 'Companion Wyvern',
    75: 'Dracthyr (Visage) (Alliance)',
    76: 'Dracthyr (Visage) (Horde)',
    77: 'Companion Pterrodax',
}

const challengeModeItemBonusOverrideType = {
    0: 'Mythic+',
    1: 'PvP'
}

const textureType = {
    0: 'InFile',
    1: 'Skin',
    2: 'Object Skin',
    3: 'Weapon Blade',
    4: 'Weapon Handle',
    5: '(OBSOLETE) Environment',
    6: 'Hair',
    7: '(OBSOLETE) Facial Hair',
    8: 'Skin Extra',
    9: 'UI Skin',
    10: 'Tauren Mane',
    11: 'Monster Skin 1',
    12: 'Monster Skin 2',
    13: 'Monster Skin 3',
    14: 'Item Icon',
    15: 'Guild BG Color',
    16: 'Guild Emblem Color',
    17: 'Guild Border Color',
    18: 'Guild Emblem',
    19: 'Eyes',
    20: 'Accessory',
    21: 'Secondary Skin',
    22: 'Secondary Hair',
}

const chrModelMaterialSkinType = {
    0: 'Primary Skin',
    1: 'Secondary Skin',
}

const inventoryTypeEnum = {
    0: 'Non-equippable',
    1: 'Head',
    2: 'Neck',
    3: 'Shoulder',
    4: 'Shirt',
    5: 'Chest',
    6: 'Waist',
    7: 'Legs',
    8: 'Feet',
    9: 'Wrist',
    10: 'Hands',
    11: 'Finger',
    12: 'Trinket',
    13: 'One-Hand',
    14: 'Off Hand',
    15: 'Ranged',
    16: 'Back',
    17: 'Two-Hand',
    18: 'Bag',
    19: 'Tabard',
    20: 'Chest',
    21: 'Main Hand',
    22: 'Off Hand',
    23: 'Held in Off-hand',
    24: 'Ammo',
    25: 'Thrown',
    26: 'Ranged',
    27: 'Quiver',
    28: 'Relic'
}

// From ItemClass table -- by ClassID (not ID)
const itemClassEnum = {
    0: 'Consumable',
    1: 'Container',
    2: 'Weapon',
    3: 'Gem',
    4: 'Armor',
    5: 'Reagent',
    6: 'Projectile',
    7: 'Tradeskill',
    8: 'Item Enhancement',
    9: 'Recipe',
    10: 'Money (OBSOLETE)',
    11: 'Quiver',
    12: 'Quest',
    13: 'Key',
    14: 'Permanent (OBSOLETE)',
    15: 'Miscellaneous',
    16: 'Glyph',
    17: 'Battle Pets',
    18: 'WoW Token'
}

let itemSubClass = [];
itemSubClass[0] = {
    0: 'Explosives and Devices',
    1: 'Potion',
    2: 'Elixir',
    3: 'Flask',
    4: 'Scroll (OBSOLETE)',
    5: 'Food & Drink',
    6: 'Item Enhancement (OBSOLETE)',
    7: 'Bandage',
    8: 'Other',
    9: 'Vantus Rune'
}

itemSubClass[1] = {
    0: 'Bag',
    1: 'Soul Bag',
    2: 'Herb Bag',
    3: 'Enchanting Bag',
    4: 'Engineering Bag',
    5: 'Gem Bag',
    6: 'Mining Bag',
    7: 'Leatherworking Bag',
    8: 'Inscription Bag',
    9: 'Tackle Box',
    10: 'Cooking Bag'
}

// 953
itemSubClass[2] = {
    0: 'Axe',
    1: 'Axe', //2H
    2: 'Bow',
    3: 'Gun',
    4: 'Mace',
    5: 'Mace', //2H
    6: 'Polearm',
    7: 'Sword',
    8: 'Sword', //2H
    9: 'Warglaives',
    10: 'Staff',
    11: 'Bear Claws',
    12: 'Cat Claws',
    13: 'Fist Weapon',
    14: 'Miscellaneous',
    15: 'Dagger',
    16: 'Thrown',
    17: 'Spear',
    18: 'Crossbow',
    19: 'Wand',
    20: 'Fishing Pole'
}

itemSubClass[3] = {
    0: 'Intellect',
    1: 'Agility',
    2: 'Strength',
    3: 'Stamina',
    4: 'Spirit',
    5: 'Critical Strike',
    6: 'Mastery',
    7: 'Haste',
    8: 'Versatility',
    9: 'Other',
    10: 'Multiple Stats',
    11: 'Artifact Relic'
}

itemSubClass[4] = {
    0: 'Miscellaneous',
    1: 'Cloth',
    2: 'Leather',
    3: 'Mail',
    4: 'Plate',
    5: 'Cosmetic',
    6: 'Shield',
    7: 'Libram',
    8: 'Idol',
    9: 'Totem',
    10: 'Sigil',
    11: 'Relic'
}

itemSubClass[5] = {
    0: 'Reagent',
    1: 'Keystone',
    2: 'Context Token'
}

itemSubClass[6] = {
    0: 'Wand(OBSOLETE)',
    1: 'Bolt(OBSOLETE)',
    2: 'Arrow',
    3: 'Bullet',
    4: 'Thrown(OBSOLETE)'
}

itemSubClass[7] = {
    0: 'Trade Goods (OBSOLETE)',
    1: 'Parts',
    2: 'Explosives (OBSOLETE)',
    3: 'Devices (OBSOLETE)',
    4: 'Jewelcrafting',
    5: 'Cloth',
    6: 'Leather',
    7: 'Metal & Stone',
    8: 'Cooking',
    9: 'Herb',
    10: 'Elemental',
    11: 'Other',
    12: 'Enchanting',
    13: 'Materials (OBSOLETE)',
    14: 'Item Enchantment (OBSOLETE)',
    15: 'Weapon Enchantment - Obsolete',
    16: 'Inscription',
    17: 'Explosives and Devices (OBSOLETE)',
    18: 'Optional Reagents'
}

itemSubClass[8] = {
    0: 'Head',
    1: 'Neck',
    2: 'Shoulder',
    3: 'Cloak',
    4: 'Chest',
    5: 'Wrist',
    6: 'Hands',
    7: 'Waist',
    8: 'Legs',
    9: 'Feet',
    10: 'Finger',
    11: 'Weapon',
    12: 'Two-Handed Weapon',
    13: 'Shield/Off-hand',
    14: 'Misc'
}

itemSubClass[9] = {
    0: 'Book',
    1: 'Leatherworking',
    2: 'Tailoring',
    3: 'Engineering',
    4: 'Blacksmithing',
    5: 'Cooking',
    6: 'Alchemy',
    7: 'First Aid',
    8: 'Enchanting',
    9: 'Fishing',
    10: 'Jewelcrafting',
    11: 'Inscription'
}

itemSubClass[10] = {
    0: 'Money(OBSOLETE)',
}

itemSubClass[11] = {
    0: 'Quiver(OBSOLETE)',
    1: 'Bolt(OBSOLETE)',
    2: 'Quiver',
    3: 'Ammo Pouch'
}

itemSubClass[12] = {
    0: 'Quest'
}

itemSubClass[13] = {
    0: 'Key',
    1: 'Lockpick'
}

itemSubClass[14] = {
    0: 'Permanent'
}

itemSubClass[15] = {
    0: 'Junk',
    1: 'Reagent',
    2: 'Companion Pets',
    3: 'Holiday',
    4: 'Other',
    5: 'Mount',
    6: 'Mount Equipment'
}

itemSubClass[16] = {
    1: 'Warrior',
    2: 'Paladin',
    3: 'Hunter',
    4: 'Rogue',
    5: 'Priest',
    6: 'Death Knight',
    7: 'Shaman',
    8: 'Mage',
    9: 'Warlock',
    10: 'Monk',
    11: 'Druid',
    12: 'Demon Hunter'
}

itemSubClass[17] = {
    0: 'BattlePet'
}

itemSubClass[18] = {
    0: 'WoW Token'
}

const uiMapSystem = {
    0: 'World',
    1: '[DEPRECATED] Legacy Taxi',
    2: 'Taxi and Adventure',
    3: 'Minimap'
}

const garrAbilityAction = {
    0: 'COUNTER_MECHANIC',
    1: 'SOLO_MISSION',
    2: 'MOD_SUCCESS_CHANCE',
    3: 'MOD_TRAVEL_TIME',
    4: 'MOD_XP',
    5: 'FRIENDLY_RACE',
    6: 'LONG_MISSION',
    7: 'SHORT_MISSION',
    8: 'MOD_CURRENCY',
    9: 'LONG_TRAVEL',
    10: 'SHORT_TRAVEL',
    11: 'MOD_BIAS',
    12: 'PROFESSION',
    13: 'MOD_BRONZE_LOOT_CHANCE',
    14: 'MOD_SILVER_LOOT_CHANCE',
    15: 'MOD_GOLD_LOOT_CHANCE',
    16: 'MOD_ALL_LOOT_CHANCE',
    17: 'MOD_MISSION_TIME',
    18: 'MENTORING',
    19: 'MOD_GOLD',
    20: 'PREVENT_DEATH',
    21: 'TREASURE_ON_MISSION_SUCCESS',
    22: 'FRIENDLY_CLASS',
    23: 'ADVANTAGE_MECHANIC',
    24: 'MOD_SUCCESS_PER_DURABILITY',
    25: 'MOD_SUCCESS_DURABILITY_IN_RANGE',
    26: 'FRIENDLY_FOLLOWER',
    27: 'KILL_TROOPS',
    28: 'MOD_DURABILITY_COST',
    29: 'MOD_BONUS_LOOT_CHANCE',
    30: 'MOD_XP_FLAT',
    31: 'MOD_ITEMLEVEL',
    32: 'MOD_STARTING_DURABILITY',
    33: 'UNIQUE_TROOPS',
    34: 'MOD_CLASSSPEC_LIMIT',
    35: 'TROOP_RESURRECTION',
    36: 'MOD_COST_BY_RACE',
    37: 'REWARD_ON_WORLD_QUEST_COMPLETE',
    38: 'MOD_SUCCESS_BY_MISSIONS_IN_PROGRESS',
    39: 'MOD_MISSION_COST',
    40: 'MOD_SUCCESS_IF_RARE_MISSION',
    41: 'SOLO_CHAMPION',
}

const mawPowerRarity = {
    1: 'Common',
    2: 'Uncommon',
    3: 'Rare',
    4: 'Epic'
}

const spellVisualEffectNames = {
    0: "FileDataID",            // Use value from SpellVisualEffectName::ModelFileDataID
    1: "Item",                  // Item::ID
    2: "CreatureDisplayInfo",   // CreatureDisplayInfo::ID
    // 3: "",
    // 4: "",
    // 5: "",
    // 6: "",
    // 7: "",
    // 8: "",
    // 9: "",
    // 10: ""
}

// 488
const itemQuality = {
    0: 'Poor',
    1: 'Common',
    2: 'Uncommon',
    3: 'Rare',
    4: 'Epic',
    5: 'Legendary',
    6: 'Artifact',
    7: 'Heirloom',
    8: 'WoW Token'
}

const spellItemEnchantmentEffect = {
    1: 'Proc',
    2: 'Damage',
    3: 'Buff',
    4: 'Armor',
    5: 'Stat',
    6: 'Totem',
    7: 'Use Spell',
    8: 'Prismatic socket'
}

const environmentalDamageType = {
    0: 'FATIGUE',
    1: 'DROWNING',
    2: 'FALLING',
    3: 'LAVA',
    4: 'SLIME',
    5: 'FIRE',
}

const garrBuildingType = {
    0: 'NONE',
    1: 'MINE',
    2: 'FARM',
    3: 'BARN',
    4: 'LUMBER_MILL',
    5: 'INN',
    6: 'TRADING_POST',
    7: 'PET_MENAGERIE',
    8: 'BARRACKS',
    9: 'SHIPYARD',
    10: 'ARMORY',
    11: 'STABLE',
    12: 'ACADEMY',
    13: 'MAGE_TOWER',
    14: 'SALAVAGE_YARD',
    15: 'STOREHOUSE',
    16: 'ALCHEMY',
    17: 'BLACKSMITH',
    18: 'ENCHANTING',
    19: 'ENGINEERING',
    20: 'INSCRIPTION',
    21: 'JEWELCRAFTING',
    22: 'LEATHERWORKING',
    23: 'TAILORING',
    24: 'FISHING',
    25: 'SPARRING_ARENA',
    26: 'WORKSHOP',
}

const garrFollowerItemSlot = {
    0: 'MAINHAND',
    1: 'OFFHAND',
    2: 'ARMOR'
}

const garrFollowerQuality = {
    1: 'COMMON',
    2: 'UNCOMMON',
    3: 'RARE',
    4: 'EPIC',
    5: 'LEGENDARY',
    6: 'TITLE',
}

const garrMechanicCategory = {
    0: 'ENVIRONMENT',
    1: 'ENEMY_RACE',
    2: 'ENCOUNTER'
}


const garrTalentCostType = {
    0: 'INITIAL',
    1: 'RESPEC',
    2: 'MAKE_PERMANENT',
    3: 'TREE_RESET',
}

const itemSlot = {
    0: 'HEAD',
    1: 'SHOULDER',
    2: 'SHIRT',
    3: 'ARMOR',
    4: 'WAIST',
    5: 'LEGS',
    6: 'FEET',
    7: 'WRIST',
    8: 'HAND',
    9: 'TABARD',
    10: 'CAPE',
    11: 'QUIVER'
}

// 1085
const uiWidgetScale = {
    0: '100',
    1: '90',
    2: '80',
    3: '70',
    4: '60',
    5: '50'
}

const questTagType = {
    0: 'TAG',
    1: 'PROFESSION',
    2: 'NORMAL',
    3: 'PVP',
    4: 'PET_BATTLE',
    5: 'BOUNTY',
    6: 'DUNGEON',
    7: 'INVASION',
    8: 'RAID',
    9: 'CONTRIBUTION',
    10: 'RATED_REWARD',
    11: 'INVASION_WRAPPER',
    12: 'FACTION_ASSAULT',
    13: 'ISLANDS',
    14: 'THREAT',
    15: 'COVENANT_CALLING',
    16: 'DRAGON_RIDER_RADING',
}

const questObjectiveType = {
    0: 'KILL',
    1: 'COLLECT',
    2: 'INTERACT_DOODAD',
    3: 'INTERACT_UNIT',
    4: 'GET_CURRENCY',
    5: 'LEARN_SPELL',
    6: 'FACTION_MIN',
    7: 'FACTION_MAX',
    8: 'PAY_MONEY',
    9: 'KILL_PLAYERS',
    10: 'AREA_TRIGGER_DEPRECATED',
    11: 'DEFEAT_BATTLEPET_NPC',
    12: 'DEFEAT_BATTLEPET',
    13: 'DEFEAT_BATTLEPET_PVP',
    14: 'CRITERIA_TREE',
    15: 'PROGRESS_BAR',
    16: 'REACH_CURRENCY',
    17: 'INCREASE_CURRENCY',
    18: 'AREA_TRIGGER_ENTER',
    19: 'AREA_TRIGGER_EXIT',
    20: 'KILL_WITH_LABEL',
}

const itemModification = {
    0: 'TRANSMOGRIFY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_ALL',
    1: 'TRANSMOGRIFY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_0',
    2: 'INCREMENT_LEVEL_OBSOLETE',
    3: 'BATTLE_PET_SPECIES',
    4: 'BATTLE_PET_BREED',
    5: 'BATTLE_PET_LEVEL',
    6: 'BATTLE_PET_CREATUREDISPLAYID',
    7: 'TRANSMOGRIFY_OVERRIDE_ENCHANT_VISUAL_ID_SPEC_ALL',
    8: 'ARTIFACT_APPEARANCE_ID',
    9: 'TIMEWALKER_LEVEL',
    10: 'TRANSMOGRIFY_OVERRIDE_ENCHANT_VISUAL_ID_SPEC_0',
    11: 'TRANSMOGRIFY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_1',
    12: 'TRANSMOGRIFY_OVERRIDE_ENCHANT_VISUAL_ID_SPEC_1',
    13: 'TRANSMOGRIFY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_2',
    14: 'TRANSMOGRIFY_OVERRIDE_ENCHANT_VISUAL_ID_SPEC_2',
    15: 'TRANSMOGRIFY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_3',
    16: 'TRANSMOGRIFY_OVERRIDE_ENCHANT_VISUAL_ID_SPEC_3',
    17: 'KEYSTONE_MAP_CHALLENGE_MODE_ID',
    18: 'KEYSTONE_POWER_LEVEL',
    19: 'KEYSTONE_AFFIX0',
    20: 'KEYSTONE_AFFIX01',
    21: 'KEYSTONE_AFFIX02',
    22: 'KEYSTONE_AFFIX03',
    23: 'LEGION_ARTIFACT_KNOWLEDGE_OBSOLETE',
    24: 'ARTIFACT_TIER',
    25: 'TRANSMOGRIFY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_4',
    26: 'PVP_RATING',
    27: 'TRANSMOGRIFY_OVERRIDE_ENCHANT_VISUAL_ID_SPEC_4',
    28: 'CONTENT_TUNING_ID',
    29: 'CHANGE_MODIFIED_CRAFTING_STAT_1',
    30: 'CHANGE_MODIFIED_CRAFTING_STAT_2',
    31: 'TRANSMOGRIFY_SECONDARY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_ALL',
    32: 'TRANSMOGRIFY_SECONDARY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_0',
    33: 'TRANSMOGRIFY_SECONDARY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_1',
    34: 'TRANSMOGRIFY_SECONDARY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_2',
    35: 'TRANSMOGRIFY_SECONDARY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_3',
    36: 'TRANSMOGRIFY_SECONDARY_ITEM_MODIFIED_APPEARANCE_ID_SPEC_4',
    37: 'SOULBIND_CONDUIT_RANK',
    38: 'CRAFTING_QUALITY_ID',
    39: 'CRAFTING_SKILL_LINE_ABILITY_ID',
    40: 'CRAFTING_DATA_ID',
    41: 'CRAFTING_SKILL_REAGENTS',
    42: 'CRAFTING_SKILL_WATERMARK',
    43: 'CRAFTING_REAGENT_SLOT_0',
    44: 'CRAFTING_REAGENT_SLOT_1',
    45: 'CRAFTING_REAGENT_SLOT_2',
    46: 'CRAFTING_REAGENT_SLOT_3',
    47: 'CRAFTING_REAGENT_SLOT_4',
    48: 'CRAFTING_REAGENT_SLOT_5',
    49: 'CRAFTING_REAGENT_SLOT_6',
    50: 'CRAFTING_REAGENT_SLOT_7',
    51: 'CRAFTING_REAGENT_SLOT_8',
    52: 'CRAFTING_REAGENT_SLOT_9',
    53: 'CRAFTING_REAGENT_SLOT_10',
    54: 'CRAFTING_REAGENT_SLOT_11',
    55: 'CRAFTING_REAGENT_SLOT_12',
    56: 'CRAFTING_REAGENT_SLOT_13',
    57: 'CRAFTING_REAGENT_SLOT_14',
}

const globalCurveType = {
    0: 'CR Bonus for CRIT_MELEE, CRIT_RANGED, CRIT_SPELL',
    1: 'CR Bonus for MASTERY',
    2: 'CR Bonus for HASTE_SPELL',
    3: 'CR Bonus for SPEED',
    4: 'CR Bonus for AVOIDANCE',
    5: 'CR Bonus for UNKNOWN (30), VERSATILITY_DAMAGE_DONE',
    6: 'CR Bonus for LIFESTEAL',
    7: 'CR Bonus for DODGE',
    8: 'CR Bonus for BLOCK',
    9: 'CR Bonus for PARRY',
    11: 'CR Bonus for VERSATILITY_DAMAGE_TAKEN'
}

const socketColorEnum = {
    1: 'META',
    2: 'RED',
    3: 'YELLOW',
    4: 'BLUE',
    5: 'HYDRAULIC',
    6: 'COGWHEEL',
    7: 'PRISMATIC',
    8: 'RELIC_IRON',
    9: 'RELIC_BLOOD',
    10: 'RELIC_SHADOW',
    11: 'RELIC_FEL',
    12: 'RELIC_ARCANE',
    13: 'RELIC_FROST',
    14: 'RELIC_FIRE',
    15: 'RELIC_WATER',
    16: 'RELIC_LIFE',
    17: 'RELIC_WIND',
    18: 'RELIC_HOLY',
    19: 'PUNCHCARD_RED',
    20: 'PUNCHCARD_YELLOW',
    21: 'PUNCHCARD_BLUE',
    22: 'CYPHER',
    23: 'TINKER',
    24: 'PRIMORDIAL'
}

// 176
const highlightColorType = {
    0: 'None',
    1: 'Hostile',
    2: 'Unfriendly',
    3: 'Neutral',
    4: 'Friendly',
    5: 'Player Simple',
    6: 'Player Extended',
    7: 'Party',
    8: 'Party PvP',
    9: 'Friend',
    10: 'Dead',
    11: 'Quest',
    12: 'Tracking',
    13: 'Game Object',
    14: 'Treasure',
    15: 'Quest Bright',
    16: 'Quest Dim',
    17: 'Spectral Vision: Hostile',
    18: 'Spectral Vision: Target',
    19: 'Spectral Vision: Treasure',
    20: 'Commentator Team 1',
    21: 'Commentator Team 2',
    22: 'Player Self Highlight',
    23: 'Important NPC',
    24: 'Important Quest NPC',
    25: 'Spectral Vision: Hostile - Fel Glyph',
    26: 'Spectral Vision: Target - Fel Glyph',
    27: 'Spectral Vision: Hostile - Shadow Glyph',
    28: 'Spectral Vision: Target - Shadow Glyph',
    29: 'Arena Team - Gold',
    30: 'Arena Team - Green',
    31: 'Party PvP in BG'
}

// 189
const groupFinderActivityDisplayType = {
    0: 'Role Counts',
    1: 'Role Enumeration',
    2: 'Class Enumeration',
    3: 'Hide All',
    4: 'Player Count'
}

// 237
const summonPropertiesSlot = {
    0: '- NONE -',
    1: 'Totem 1',
    2: 'Totem 2',
    3: 'Totem 3',
    4: 'Totem 4',
    5: 'Critter',
    6: 'Quest (Players Only)',
    // -1: 'Any Available Totem',
};

// 238
const summonPropertiesControl = {
    0: '- NONE -',
    1: 'Guardian',
    2: 'Pet',
    3: 'Possessed',
    4: 'Possessed Vehicle',
    5: 'Vehicle (Wild, but Ride Spell will be cast)',
};

// 272
const soundkitSoundType = {
    0: 'Unused/Miscellaneous',
    1: 'Spells',
    2: 'UI',
    3: 'Footsteps',
    4: 'Combat Impacts',
    6: 'Combat Swings',
    9: 'Item Use Sounds',
    10: 'Monster Sounds',
    12: 'VocalUISounds',
    13: 'Point Sound Emitters',
    14: 'Doodad Sounds',
    16: 'Death Thud Sounds',
    17: 'NPC Sounds',
    19: 'Foley Sounds (NOT EDITABLE)',
    20: 'Footsteps(Splashes)',
    21: 'CharacterSplashSounds',
    22: 'WaterVolume Sounds',
    23: 'Tradeskill Sounds',
    24: 'Terrain Emitter Sounds',
    25: 'Game Object Sounds',
    26: 'SpellFizzles',
    27: 'CreatureLoops',
    28: 'Zone Music Files',
    29: 'Character Macro Lines',
    30: 'Cinematic Music',
    31: 'Cinematic Voice',
    50: 'Zone Ambience',
    52: 'Sound Emitters',
    53: 'Vehicle States',
};

// 328
const powerTypePowerTypeEnum = {
    0: 'Mana',
    1: 'Rage',
    2: 'Focus',
    3: 'Energy',
    4: 'Happiness',
    5: 'Runes',
    6: 'Runic Power',
    7: 'Soul Shards',
    8: 'Lunar Power',
    9: 'Holy Power',
    10: 'Alternate',
    11: 'Maelstrom',
    12: 'Chi',
    13: 'Insanity',
    14: 'Combo Points',
    16: 'Arcane Charges',
    17: 'Fury',
    18: 'Pain',
};

// 353
const scenarioType = {
    0: 'Default',
    1: 'Challenge Mode',
    2: 'Proving Grounds',
    3: 'Dungeon Display',
    4: 'Legion Invasion',
    5: 'Boost Tutorial',
    6: 'Warfront',
    7: 'MULTI_STEP'
};

// 367
const weaponSwingType = {
    0: 'Light',
    1: 'Medium',
    2: 'Heavy',
    3: 'Agile',
    4: 'Pierce',
    5: 'Large Monster'
}

// 372
const transmogSourceTypeEnum = {
    0: 'Unknown',
    1: 'Dungeon Journal Encounter',
    2: 'Quest',
    3: 'Vendor',
    4: 'World Drop',
    5: 'Hidden Until Collected',
    6: 'Can\'t Collect',
    7: 'Achievement',
    8: 'Profession',
    9: 'Not Valid for Transmog'
}

// 455
const itemContext = {
    1: "Dungeon: Normal",
    2: "Dungeon: Heroic",
    3: "Raid: Normal",
    4: "Raid: Raid Finder",
    5: "Raid: Heroic",
    6: "Raid: Mythic",
    7: "PVP: Unranked 1",
    8: "PVP: Ranked 1 (Unrated)",
    9: "Scenario: Normal",
    10: "Scenario: Heroic",
    11: "Quest Reward",
    12: "In-Game Store",
    13: "Trade Skill",
    14: "Vendor",
    15: "Black Market",
    16: "Mythic+ End of Run",
    17: "Dungeon: Lvl-Up 1",
    18: "Dungeon: Lvl-Up 2",
    19: "Dungeon: Lvl-Up 3",
    20: "Dungeon: Lvl-Up 4",
    21: "Force to NONE",
    22: "Timewalking",
    23: "Dungeon: Mythic",
    24: "Pvp Honor Reward",
    25: "World Quest 1",
    26: "World Quest 2",
    27: "World Quest 3",
    28: "World Quest 4",
    29: "World Quest 5",
    30: "World Quest 6",
    31: "Mission Reward 1",
    32: "Mission Reward 2",
    33: "Mythic+ End of Run: Time Chest",
    34: "Mythic+ Timewalking End of Run",
    35: "Mythic+ Jackpot",
    36: "World Quest 7",
    37: "World Quest 8",
    38: "PVP: Ranked 2 (Combatant I)",
    39: "PVP: Ranked 4 (Challenger I)",
    40: "PVP: Ranked 6 (Rival I)",
    41: "PVP: Unranked 2",
    42: "World Quest 9",
    43: "World Quest 10",
    44: "PVP: Ranked 8 (Duelist)",
    45: "PVP: Ranked 9 (Elite)",
    46: "PVP: Ranked 3 (Combatant II)",
    47: "PVP: Unranked 3",
    48: "PVP: Unranked 4",
    49: "PVP: Unranked 5",
    50: "PVP: Unranked 6",
    51: "PVP: Unranked 7",
    52: "PVP: Ranked 5 (Challenger II)",
    53: "World Quest 11",
    54: "World Quest 12",
    55: "World Quest 13",
    56: "PVP: Ranked Jackpot",
    57: "Tournament Realm",
    58: "Relinquished",
    59: "Legendary Forge",
    60: "Quest Bonus Loot",
    61: "Character Boost BFA",
    62: "Character Boost Shadowlands",
    63: "Legendary Crafting 1",
    64: "Legendary Crafting 2",
    65: "Legendary Crafting 3",
    66: "Legendary Crafting 4",
    67: "Legendary Crafting 5",
    68: "Legendary Crafting 6",
    69: "Legendary Crafting 7",
    70: "Legendary Crafting 8",
    71: "Legendary Crafting 9",
    72: "Weekly Rewards Additional",
    73: "Weekly Rewards Concession",
    74: "World Quest Jackpot",
    75: "New Character",
    76: "War Mode",
    77: "PvP Brawl 1",
    78: "PvP Brawl 2",
    79: "Torghast",
    80: "Corpse Recovery",
    81: "World Boss",
    82: "Raid: Normal (Extended)",
    83: "Raid: Raid Finder (Extended)",
    84: "Raid: Heroic (Extended)",
    85: "Raid: Mythic (Extended)",
    86: "9.1 Character Template",
    87: "Mythic+ Timewalking End of Run: Time Chest",
    88: "PVP: Ranked 7 (Rival II)",
    89: "Raid: Normal (Extended 2)",
    90: "Raid: Raid Finder (Extended 2)",
    91: "Raid: Heroic (Extended 2)",
    92: "Raid: Mythic (Extended 2)",
    93: "Raid: Normal (Extended 3)",
    94: "Raid: Raid Finder (Extended 3)",
    95: "Raid: Heroic (Extended 3)",
    96: "Raid: Mythic (Extended 3)",
    97: "Template Character 1",
    98: "Template Character 2",
    99: "Template Character 3",
    100: "Template Character 4",
    101: "Normal Dungeon Jackpot",
    102: "Heroic Dungeon Jackpot",
    103: "Mythic Dungeon Jackpot",
}

// 457
const mapDifficultyResetInterval = {
    0: '- NONE -',
    1: 'Daily',
    2: 'Weekly',
    3: '3 Day',
    4: '5 Day',
};

// 465
const emoteSpecProc = {
    0: 'One Shot Emote',
    1: 'Changes Stand State',
    2: 'Sets Emote State'
}

// 497
const ObjectEffectPackageElem_StateType = {
    0:   '- Invalid -',
    1:   'Always',
    2:   'Transport - Stopped',
    3:   'Transport - Accelerating',
    4:   'Transport - Moving',
    5:   'Transport - Decelerating',
    6:   'Movement - Ascending',
    7:   'Movement - Descending',
    8:   'Anim - AttackThrown',
    9:   'Anim - HoldThrown',
    10:  'Anim - LoadThrown',
    11:  'Anim - ReadyThrown',
    12:  'Anim - Run',
    13:  'Anim - Walk',
    14:  'Anim - CombatWound',
    15:  'Anim - Death',
    16:  'Anim - SpellCastDirected',
    17:  'Anim - StandWound',
    18:  'Anim - ReadyUnarmed',
    19:  'Anim - Stand',
    20:  'Anim - ShuffleLeft',
    21:  'Anim - ShuffleRight',
    22:  'Anim - WalkBackwards',
    23:  'Anim - JumpStart',
    24:  'Anim - JumpEnd',
    25:  'Anim - Fall',
    26:  'Anim - SwimIdle',
    27:  'Anim - Swim',
    28:  'Anim - SwimLeft',
    29:  'Anim - SwimRight',
    30:  'Anim - SwimBackwards',
    31:  'Anim - RunLeft',
    32:  'Anim - RunRight',
    33:  'Anim - Fly',
    34:  'Anim - Sprint',
    35:  'Anim - JumpLandRun',
    36:  'Anim - Jump',
    37:  'Movement - Moving',
    38:  'Movement - Not Moving',
    39:  'Anim - CombatCritical',
    40:  'Anim - AttackUnarmed',
    41:  'Anim - Stand Var 1',
    42:  'Anim - Swim Idle Var 1',
    43:  'Anim - Sit Ground',
    44:  'Anim - Sit Ground Up',
    45:  'Anim - Sit Ground Down',
    46:  'Anim - Hover',
    47:  'Movement - Turning',
    48:  'Movement - Not Turning',
    49:  'Movement - Running',
    50:  'Movement - Running Forward',
    51:  'Movement - Running Backward',
    52:  'Movement - Running Left',
    53:  'Movement - Running Right',
    54:  'Movement - Running Sideways',
    55:  'Movement - Walking',
    56:  'Movement - Walking Forward',
    57:  'Movement - Walking Backward',
    58:  'Movement - Walking Left',
    59:  'Movement - Walking Right',
    60:  'Movement - Walking Sideways',
    61:  'Movement - Flying',
    62:  'Movement - Flying Forward',
    63:  'Movement - Flying Backward',
    64:  'Movement - Flying Left',
    65:  'Movement - Flying Right',
    66:  'Movement - Flying Sideways',
    67:  'Anim - Grab',
    68:  'Anim - Ship Start',
    69:  'Anim - Ship Moving',
    70:  'Anim - Ship Stop',
    71:  'Anim - Open',
    72:  'Anim - Opened',
    73:  'Anim - Close',
    74:  'Anim - Closed',
    75:  'Anim - Destroy',
    76:  'Anim - Destroyed',
    77:  'Anim - Custom 0',
    78:  'Anim - Custom 1',
    79:  'Anim - Custom 2',
    80:  'Anim - Custom 3',
    81:  'Anim - Dead',
    82:  'Anim - MountFlightIdle',
    83:  'Anim - MountFlightSprint',
    84:  'Anim - MountFlightLeft',
    85:  'Anim - MountFlightRight',
    86:  'Anim - MountFlightBackwards',
    87:  'Anim - MountFlightRun',
    88:  'Anim - MountFlightWalk',
    89:  'Anim - MountFlightWalkBackwards',
    90:  'Anim - MountFlightStart',
    91:  'Anim - MountFlightLand',
    92:  'Anim - MountFlightLandRun',
    93:  'Anim - MountSwimStart',
    94:  'Anim - MountSwimLand',
    95:  'Anim - MountSwimLandRun',
    96:  'Anim - MountSwimIdle',
    97:  'Anim - MountSwimBackwards',
    98:  'Anim - MountSwimLeft',
    99:  'Anim - MountSwimRight',
    100: 'Anim - MountSwimRun',
    101: 'Anim - MountSwimSprint',
    102: 'Anim - MountSwimWalk',
    103: 'Anim - MountSwimWalkBackwards',
    104: 'Anim - Birth',
    105: 'Anim - Decay',
    106: 'Anim - SPELL',
    107: 'Anim - STOP',
    108: 'Anim - RISE',
    109: 'Anim - STUN',
    110: 'Anim - HANDSCLOSED',
    111: 'Anim - ATTACK1H',
    112: 'Anim - ATTACK2H',
    113: 'Anim - ATTACK2HL',
    114: 'Anim - PARRYUNARMED',
    115: 'Anim - PARRY1H',
    116: 'Anim - PARRY2H',
    117: 'Anim - PARRY2HL',
    118: 'Anim - SHIELDBLOCK',
    119: 'Anim - READY1H',
    120: 'Anim - READY2H',
    121: 'Anim - READY2HL',
    122: 'Anim - READYBOW',
    123: 'Anim - DODGE',
    124: 'Anim - SPELLPRECAST',
    125: 'Anim - SPELLCAST',
    126: 'Anim - SPELLCASTAREA',
    127: 'Anim - NPCWELCOME',
    128: 'Anim - NPCGOODBYE',
    129: 'Anim - BLOCK',
    130: 'Anim - ATTACKBOW',
    131: 'Anim - FIREBOW',
    132: 'Anim - READYRIFLE',
    133: 'Anim - ATTACKRIFLE',
    134: 'Anim - LOOT',
    135: 'Anim - READYSPELLDIRECTED',
    136: 'Anim - READYSPELLOMNI',
    137: 'Anim - SPELLCASTOMNI',
    138: 'Anim - BATTLEROAR',
    139: 'Anim - READYABILITY',
    140: 'Anim - SPECIAL1H',
    141: 'Anim - SPECIAL2H',
    142: 'Anim - SHIELDBASH',
    143: 'Anim - EMOTETALK',
    144: 'Anim - EMOTEEAT',
    145: 'Anim - EMOTEWORK',
    146: 'Anim - EMOTEUSESTANDING',
    147: 'Anim - EMOTETALKEXCLAMATION',
    148: 'Anim - EMOTETALKQUESTION',
    149: 'Anim - EMOTEBOW',
    150: 'Anim - EMOTEWAVE',
    151: 'Anim - EMOTECHEER',
    152: 'Anim - EMOTEDANCE',
    153: 'Anim - EMOTELAUGH',
    154: 'Anim - EMOTESLEEP',
    155: 'Anim - EMOTESITGROUND',
    156: 'Anim - EMOTERUDE',
    157: 'Anim - EMOTEROAR',
    158: 'Anim - EMOTEKNEEL',
    159: 'Anim - EMOTEKISS',
    160: 'Anim - EMOTECRY',
    161: 'Anim - EMOTECHICKEN',
    162: 'Anim - EMOTEBEG',
    163: 'Anim - EMOTEAPPLAUD',
    164: 'Anim - EMOTESHOUT',
    165: 'Anim - EMOTEFLEX',
    166: 'Anim - EMOTESHY',
    167: 'Anim - EMOTEPOINT',
    168: 'Anim - ATTACK1HPIERCE',
    169: 'Anim - ATTACK2HLOOSEPIERCE',
    170: 'Anim - ATTACKOFF',
    171: 'Anim - ATTACKOFFPIERCE',
    172: 'Anim - SHEATH',
    173: 'Anim - HIPSHEATH',
    174: 'Anim - MOUNT',
    175: 'Anim - MOUNTSPECIAL',
    176: 'Anim - KICK',
    177: 'Anim - SLEEPDOWN',
    178: 'Anim - SLEEP',
    179: 'Anim - SLEEPUP',
    180: 'Anim - SITCHAIRLOW',
    181: 'Anim - SITCHAIRMED',
    182: 'Anim - SITCHAIRHIGH',
    183: 'Anim - LOADBOW',
    184: 'Anim - LOADRIFLE',
    185: 'Anim - HOLDBOW',
    186: 'Anim - HOLDRIFLE',
    187: 'Anim - EMOTESALUTE',
    188: 'Anim - KNEELSTART',
    189: 'Anim - KNEELLOOP',
    190: 'Anim - KNEELEND',
    191: 'Anim - ATTACKUNARMEDOFF',
    192: 'Anim - SPECIALUNARMED',
    193: 'Anim - STEALTHWALK',
    194: 'Anim - STEALTHSTAND',
    195: 'Anim - KNOCKDOWN',
    196: 'Anim - EATINGLOOP',
    197: 'Anim - USESTANDINGLOOP',
    198: 'Anim - CHANNELCASTDIRECTED',
    199: 'Anim - CHANNELCASTOMNI',
    200: 'Anim - WHIRLWIND',
    201: 'Anim - USESTANDINGSTART',
    202: 'Anim - USESTANDINGEND',
    203: 'Anim - CREATURESPECIAL',
    204: 'Anim - DROWN',
    205: 'Anim - DROWNED',
    206: 'Anim - FISHINGCAST',
    207: 'Anim - FISHINGLOOP',
    208: 'Anim - EMOTEWORKNOSHEATHE',
    209: 'Anim - EMOTESTUNNOSHEATHE',
    210: 'Anim - EMOTEUSESTANDINGNOSHEATHE',
    211: 'Anim - SPELLSLEEPDOWN',
    212: 'Anim - SPELLKNEELSTART',
    213: 'Anim - SPELLKNEELLOOP',
    214: 'Anim - SPELLKNEELEND',
    215: 'Anim - INFLIGHT',
    216: 'Anim - SPAWN',
    217: 'Anim - REBUILD',
    218: 'Anim - DESPAWN',
    219: 'Anim - HOLD',
    220: 'Anim - BOWPULL',
    221: 'Anim - BOWRELEASE',
    222: 'Anim - GROUPARROW',
    223: 'Anim - ARROW',
    224: 'Anim - CORPSEARROW',
    225: 'Anim - GUIDEARROW',
    226: 'Anim - SWAY',
    227: 'Anim - DRUIDCATPOUNCE',
    228: 'Anim - DRUIDCATRIP',
    229: 'Anim - DRUIDCATRAKE',
    230: 'Anim - DRUIDCATRAVAGE',
    231: 'Anim - DRUIDCATCLAW',
    232: 'Anim - DRUIDCATCOWER',
    233: 'Anim - DRUIDBEARSWIPE',
    234: 'Anim - DRUIDBEARBITE',
    235: 'Anim - DRUIDBEARMAUL',
    236: 'Anim - DRUIDBEARBASH',
    237: 'Anim - DRAGONTAIL',
    238: 'Anim - DRAGONSTOMP',
    239: 'Anim - DRAGONSPIT',
    240: 'Anim - DRAGONSPITHOVER',
    241: 'Anim - DRAGONSPITFLY',
    242: 'Anim - EMOTEYES',
    243: 'Anim - EMOTENO',
    244: 'Anim - LOOTHOLD',
    245: 'Anim - LOOTUP',
    246: 'Anim - STANDHIGH',
    247: 'Anim - IMPACT',
    248: 'Anim - LIFTOFF',
    249: 'Anim - SUCCUBUSENTICE',
    250: 'Anim - EMOTETRAIN',
    251: 'Anim - EMOTEDEAD',
    252: 'Anim - EMOTEDANCEONCE',
    253: 'Anim - DEFLECT',
    254: 'Anim - EMOTEEATNOSHEATHE',
    255: 'Anim - LAND',
    256: 'Anim - SUBMERGE',
    257: 'Anim - SUBMERGED',
    258: 'Anim - CANNIBALIZE',
    259: 'Anim - ARROWBIRTH',
    260: 'Anim - GROUPARROWBIRTH',
    261: 'Anim - CORPSEARROWBIRTH',
    262: 'Anim - GUIDEARROWBIRTH',
    263: 'Anim - EMOTETALKNOSHEATHE',
    264: 'Anim - EMOTEPOINTNOSHEATHE',
    265: 'Anim - EMOTESALUTENOSHEATHE',
    266: 'Anim - EMOTEDANCESPECIAL',
    267: 'Anim - MUTILATE',
    268: 'Anim - CUSTOMSPELL01',
    269: 'Anim - CUSTOMSPELL02',
    270: 'Anim - CUSTOMSPELL03',
    271: 'Anim - CUSTOMSPELL04',
    272: 'Anim - CUSTOMSPELL05',
    273: 'Anim - CUSTOMSPELL06',
    274: 'Anim - CUSTOMSPELL07',
    275: 'Anim - CUSTOMSPELL08',
    276: 'Anim - CUSTOMSPELL09',
    277: 'Anim - CUSTOMSPELL10',
    278: 'Anim - STEALTHRUN',
    279: 'Anim - EMERGE',
    280: 'Anim - COWER',
    281: 'Anim - GRABCLOSED',
    282: 'Anim - GRABTHROWN',
    283: 'Anim - FLYSTAND',
    284: 'Anim - FLYDEATH',
    285: 'Anim - FLYSPELL',
    286: 'Anim - FLYSTOP',
    287: 'Anim - FLYWALK',
    288: 'Anim - FLYRUN',
    289: 'Anim - FLYDEAD',
    290: 'Anim - FLYRISE',
    291: 'Anim - FLYSTANDWOUND',
    292: 'Anim - FLYCOMBATWOUND',
    293: 'Anim - FLYCOMBATCRITICAL',
    294: 'Anim - FLYSHUFFLELEFT',
    295: 'Anim - FLYSHUFFLERIGHT',
    296: 'Anim - FLYWALKBACKWARDS',
    297: 'Anim - FLYSTUN',
    298: 'Anim - FLYHANDSCLOSED',
    299: 'Anim - FLYATTACKUNARMED',
    300: 'Anim - FLYATTACK1H',
    301: 'Anim - FLYATTACK2H',
    302: 'Anim - FLYATTACK2HL',
    303: 'Anim - FLYPARRYUNARMED',
    304: 'Anim - FLYPARRY1H',
    305: 'Anim - FLYPARRY2H',
    306: 'Anim - FLYPARRY2HL',
    307: 'Anim - FLYSHIELDBLOCK',
    308: 'Anim - FLYREADYUNARMED',
    309: 'Anim - FLYREADY1H',
    310: 'Anim - FLYREADY2H',
    311: 'Anim - FLYREADY2HL',
    312: 'Anim - FLYREADYBOW',
    313: 'Anim - FLYDODGE',
    314: 'Anim - FLYSPELLPRECAST',
    315: 'Anim - FLYSPELLCAST',
    316: 'Anim - FLYSPELLCASTAREA',
    317: 'Anim - FLYNPCWELCOME',
    318: 'Anim - FLYNPCGOODBYE',
    319: 'Anim - FLYBLOCK',
    320: 'Anim - FLYJUMPSTART',
    321: 'Anim - FLYJUMP',
    322: 'Anim - FLYJUMPEND',
    323: 'Anim - FLYFALL',
    324: 'Anim - FLYSWIMIDLE',
    325: 'Anim - FLYSWIM',
    326: 'Anim - FLYSWIMLEFT',
    327: 'Anim - FLYSWIMRIGHT',
    328: 'Anim - FLYSWIMBACKWARDS',
    329: 'Anim - FLYATTACKBOW',
    330: 'Anim - FLYFIREBOW',
    331: 'Anim - FLYREADYRIFLE',
    332: 'Anim - FLYATTACKRIFLE',
    333: 'Anim - FLYLOOT',
    334: 'Anim - FLYREADYSPELLDIRECTED',
    335: 'Anim - FLYREADYSPELLOMNI',
    336: 'Anim - FLYSPELLCASTDIRECTED',
    337: 'Anim - FLYSPELLCASTOMNI',
    338: 'Anim - FLYBATTLEROAR',
    339: 'Anim - FLYREADYABILITY',
    340: 'Anim - FLYSPECIAL1H',
    341: 'Anim - FLYSPECIAL2H',
    342: 'Anim - FLYSHIELDBASH',
    343: 'Anim - FLYEMOTETALK',
    344: 'Anim - FLYEMOTEEAT',
    345: 'Anim - FLYEMOTEWORK',
    346: 'Anim - FLYEMOTEUSESTANDING',
    347: 'Anim - FLYEMOTETALKEXCLAMATION',
    348: 'Anim - FLYEMOTETALKQUESTION',
    349: 'Anim - FLYEMOTEBOW',
    350: 'Anim - FLYEMOTEWAVE',
    351: 'Anim - FLYEMOTECHEER',
    352: 'Anim - FLYEMOTEDANCE',
    353: 'Anim - FLYEMOTELAUGH',
    354: 'Anim - FLYEMOTESLEEP',
    355: 'Anim - FLYEMOTESITGROUND',
    356: 'Anim - FLYEMOTERUDE',
    357: 'Anim - FLYEMOTEROAR',
    358: 'Anim - FLYEMOTEKNEEL',
    359: 'Anim - FLYEMOTEKISS',
    360: 'Anim - FLYEMOTECRY',
    361: 'Anim - FLYEMOTECHICKEN',
    362: 'Anim - FLYEMOTEBEG',
    363: 'Anim - FLYEMOTEAPPLAUD',
    364: 'Anim - FLYEMOTESHOUT',
    365: 'Anim - FLYEMOTEFLEX',
    366: 'Anim - FLYEMOTESHY',
    367: 'Anim - FLYEMOTEPOINT',
    368: 'Anim - FLYATTACK1HPIERCE',
    369: 'Anim - FLYATTACK2HLOOSEPIERCE',
    370: 'Anim - FLYATTACKOFF',
    371: 'Anim - FLYATTACKOFFPIERCE',
    372: 'Anim - FLYSHEATH',
    373: 'Anim - FLYHIPSHEATH',
    374: 'Anim - FLYMOUNT',
    375: 'Anim - FLYRUNRIGHT',
    376: 'Anim - FLYRUNLEFT',
    377: 'Anim - FLYMOUNTSPECIAL',
    378: 'Anim - FLYKICK',
    379: 'Anim - FLYSITGROUNDDOWN',
    380: 'Anim - FLYSITGROUND',
    381: 'Anim - FLYSITGROUNDUP',
    382: 'Anim - FLYSLEEPDOWN',
    383: 'Anim - FLYSLEEP',
    384: 'Anim - FLYSLEEPUP',
    385: 'Anim - FLYSITCHAIRLOW',
    386: 'Anim - FLYSITCHAIRMED',
    387: 'Anim - FLYSITCHAIRHIGH',
    388: 'Anim - FLYLOADBOW',
    389: 'Anim - FLYLOADRIFLE',
    390: 'Anim - FLYATTACKTHROWN',
    391: 'Anim - FLYREADYTHROWN',
    392: 'Anim - FLYHOLDBOW',
    393: 'Anim - FLYHOLDRIFLE',
    394: 'Anim - FLYHOLDTHROWN',
    395: 'Anim - FLYLOADTHROWN',
    396: 'Anim - FLYEMOTESALUTE',
    397: 'Anim - FLYKNEELSTART',
    398: 'Anim - FLYKNEELLOOP',
    399: 'Anim - FLYKNEELEND',
    400: 'Anim - FLYATTACKUNARMEDOFF',
    401: 'Anim - FLYSPECIALUNARMED',
    402: 'Anim - FLYSTEALTHWALK',
    403: 'Anim - FLYSTEALTHSTAND',
    404: 'Anim - FLYKNOCKDOWN',
    405: 'Anim - FLYEATINGLOOP',
    406: 'Anim - FLYUSESTANDINGLOOP',
    407: 'Anim - FLYCHANNELCASTDIRECTED',
    408: 'Anim - FLYCHANNELCASTOMNI',
    409: 'Anim - FLYWHIRLWIND',
    410: 'Anim - FLYBIRTH',
    411: 'Anim - FLYUSESTANDINGSTART',
    412: 'Anim - FLYUSESTANDINGEND',
    413: 'Anim - FLYCREATURESPECIAL',
    414: 'Anim - FLYDROWN',
    415: 'Anim - FLYDROWNED',
    416: 'Anim - FLYFISHINGCAST',
    417: 'Anim - FLYFISHINGLOOP',
    418: 'Anim - FLYFLY',
    419: 'Anim - FLYEMOTEWORKNOSHEATHE',
    420: 'Anim - FLYEMOTESTUNNOSHEATHE',
    421: 'Anim - FLYEMOTEUSESTANDINGNOSHEATHE',
    422: 'Anim - FLYSPELLSLEEPDOWN',
    423: 'Anim - FLYSPELLKNEELSTART',
    424: 'Anim - FLYSPELLKNEELLOOP',
    425: 'Anim - FLYSPELLKNEELEND',
    426: 'Anim - FLYSPRINT',
    427: 'Anim - FLYINFLIGHT',
    428: 'Anim - FLYSPAWN',
    429: 'Anim - FLYCLOSE',
    430: 'Anim - FLYCLOSED',
    431: 'Anim - FLYOPEN',
    432: 'Anim - FLYOPENED',
    433: 'Anim - FLYDESTROY',
    434: 'Anim - FLYDESTROYED',
    435: 'Anim - FLYREBUILD',
    436: 'Anim - FLYCUSTOM0',
    437: 'Anim - FLYCUSTOM1',
    438: 'Anim - FLYCUSTOM2',
    439: 'Anim - FLYCUSTOM3',
    440: 'Anim - FLYDESPAWN',
    441: 'Anim - FLYHOLD',
    442: 'Anim - FLYDECAY',
    443: 'Anim - FLYBOWPULL',
    444: 'Anim - FLYBOWRELEASE',
    445: 'Anim - FLYSHIPSTART',
    446: 'Anim - FLYSHIPMOVING',
    447: 'Anim - FLYSHIPSTOP',
    448: 'Anim - FLYGROUPARROW',
    449: 'Anim - FLYARROW',
    450: 'Anim - FLYCORPSEARROW',
    451: 'Anim - FLYGUIDEARROW',
    452: 'Anim - FLYSWAY',
    453: 'Anim - FLYDRUIDCATPOUNCE',
    454: 'Anim - FLYDRUIDCATRIP',
    455: 'Anim - FLYDRUIDCATRAKE',
    456: 'Anim - FLYDRUIDCATRAVAGE',
    457: 'Anim - FLYDRUIDCATCLAW',
    458: 'Anim - FLYDRUIDCATCOWER',
    459: 'Anim - FLYDRUIDBEARSWIPE',
    460: 'Anim - FLYDRUIDBEARBITE',
    461: 'Anim - FLYDRUIDBEARMAUL',
    462: 'Anim - FLYDRUIDBEARBASH',
    463: 'Anim - FLYDRAGONTAIL',
    464: 'Anim - FLYDRAGONSTOMP',
    465: 'Anim - FLYDRAGONSPIT',
    466: 'Anim - FLYDRAGONSPITHOVER',
    467: 'Anim - FLYDRAGONSPITFLY',
    468: 'Anim - FLYEMOTEYES',
    469: 'Anim - FLYEMOTENO',
    470: 'Anim - FLYJUMPLANDRUN',
    471: 'Anim - FLYLOOTHOLD',
    472: 'Anim - FLYLOOTUP',
    473: 'Anim - FLYSTANDHIGH',
    474: 'Anim - FLYIMPACT',
    475: 'Anim - FLYLIFTOFF',
    476: 'Anim - FLYHOVER',
    477: 'Anim - FLYSUCCUBUSENTICE',
    478: 'Anim - FLYEMOTETRAIN',
    479: 'Anim - FLYEMOTEDEAD',
    480: 'Anim - FLYEMOTEDANCEONCE',
    481: 'Anim - FLYDEFLECT',
    482: 'Anim - FLYEMOTEEATNOSHEATHE',
    483: 'Anim - FLYLAND',
    484: 'Anim - FLYSUBMERGE',
    485: 'Anim - FLYSUBMERGED',
    486: 'Anim - FLYCANNIBALIZE',
    487: 'Anim - FLYARROWBIRTH',
    488: 'Anim - FLYGROUPARROWBIRTH',
    489: 'Anim - FLYCORPSEARROWBIRTH',
    490: 'Anim - FLYGUIDEARROWBIRTH',
    491: 'Anim - FLYEMOTETALKNOSHEATHE',
    492: 'Anim - FLYEMOTEPOINTNOSHEATHE',
    493: 'Anim - FLYEMOTESALUTENOSHEATHE',
    494: 'Anim - FLYEMOTEDANCESPECIAL',
    495: 'Anim - FLYMUTILATE',
    496: 'Anim - FLYCUSTOMSPELL01',
    497: 'Anim - FLYCUSTOMSPELL02',
    498: 'Anim - FLYCUSTOMSPELL03',
    499: 'Anim - FLYCUSTOMSPELL04',
    500: 'Anim - FLYCUSTOMSPELL05',
    501: 'Anim - FLYCUSTOMSPELL06',
    502: 'Anim - FLYCUSTOMSPELL07',
    503: 'Anim - FLYCUSTOMSPELL08',
    504: 'Anim - FLYCUSTOMSPELL09',
    505: 'Anim - FLYCUSTOMSPELL10',
    506: 'Anim - FLYSTEALTHRUN',
    507: 'Anim - FLYEMERGE',
    508: 'Anim - FLYCOWER',
    509: 'Anim - FLYGRAB',
    510: 'Anim - FLYGRABCLOSED',
    511: 'Anim - FLYGRABTHROWN',
    512: 'Anim - TOFLY',
    513: 'Anim - TOHOVER',
    514: 'Anim - TOGROUND',
    515: 'Anim - FLYTOFLY',
    516: 'Anim - FLYTOHOVER',
    517: 'Anim - FLYTOGROUND',
    518: 'Anim - SETTLE',
    519: 'Anim - FLYSETTLE',
    520: 'Anim - DEATHSTART',
    521: 'Anim - DEATHLOOP',
    522: 'Anim - DEATHEND',
    523: 'Anim - FLYDEATHSTART',
    524: 'Anim - FLYDEATHLOOP',
    525: 'Anim - FLYDEATHEND',
    526: 'Anim - DEATHENDHOLD',
    527: 'Anim - FLYDEATHENDHOLD',
    528: 'Anim - STRANGULATE',
    529: 'Anim - FLYSTRANGULATE',
    530: 'Anim - READYJOUST',
    531: 'Anim - LOADJOUST',
    532: 'Anim - HOLDJOUST',
    533: 'Anim - FLYREADYJOUST',
    534: 'Anim - FLYLOADJOUST',
    535: 'Anim - FLYHOLDJOUST',
    536: 'Anim - ATTACKJOUST',
    537: 'Anim - FLYATTACKJOUST',
    538: 'Anim - RECLINEDMOUNT',
    539: 'Anim - FLYRECLINEDMOUNT',
    540: 'Anim - TOALTERED',
    541: 'Anim - FROMALTERED',
    542: 'Anim - FLYTOALTERED',
    543: 'Anim - FLYFROMALTERED',
    544: 'Anim - INSTOCKS',
    545: 'Anim - FLYINSTOCKS',
    546: 'Anim - VEHICLEGRAB',
    547: 'Anim - VEHICLETHROW',
    548: 'Anim - FLYVEHICLEGRAB',
    549: 'Anim - FLYVEHICLETHROW',
    550: 'Anim - TOALTEREDPOSTSWAP',
    551: 'Anim - FROMALTEREDPOSTSWAP',
    552: 'Anim - FLYTOALTEREDPOSTSWAP',
    553: 'Anim - FLYFROMALTEREDPOSTSWAP',
    554: 'Anim - RECLINEDMOUNTPASSENGER',
    555: 'Anim - FLYRECLINEDMOUNTPASSENGER',
    556: 'Anim - CARRY2H',
    557: 'Anim - CARRIED2H',
    558: 'Anim - FLYCARRY2H',
    559: 'Anim - FLYCARRIED2H',
    560: 'Anim - EMOTESNIFF',
    561: 'Anim - EMOTEFLYSNIFF',
    562: 'Anim - ATTACKFIST1H',
    563: 'Anim - FLYATTACKFIST1H',
    564: 'Anim - ATTACKFIST1HOFF',
    565: 'Anim - FLYATTACKFIST1HOFF',
    566: 'Anim - PARRYFIST1H',
    567: 'Anim - FLYPARRYFIST1H',
    568: 'Anim - READYFIST1H',
    569: 'Anim - FLYREADYFIST1H',
    570: 'Anim - SPECIALFIST1H',
    571: 'Anim - FLYSPECIALFIST1H',
    572: 'Anim - EMOTEREADSTART',
    573: 'Anim - FLYEMOTEREADSTART',
    574: 'Anim - EMOTEREADLOOP',
    575: 'Anim - FLYEMOTEREADLOOP',
    576: 'Anim - EMOTEREADEND',
    577: 'Anim - FLYEMOTEREADEND',
    578: 'Anim - SWIMRUN',
    579: 'Anim - FLYSWIMRUN',
    580: 'Anim - SWIMWALK',
    581: 'Anim - FLYSWIMWALK',
    582: 'Anim - SWIMWALKBACKWARDS',
    583: 'Anim - FLYSWIMWALKBACKWARDS',
    584: 'Anim - SWIMSPRINT',
    585: 'Anim - FLYSWIMSPRINT',
    586: 'Anim - FLYMOUNTSWIMIDLE',
    587: 'Anim - FLYMOUNTSWIMBACKWARDS',
    588: 'Anim - FLYMOUNTSWIMLEFT',
    589: 'Anim - FLYMOUNTSWIMRIGHT',
    590: 'Anim - FLYMOUNTSWIMRUN',
    591: 'Anim - FLYMOUNTSWIMSPRINT',
    592: 'Anim - FLYMOUNTSWIMWALK',
    593: 'Anim - FLYMOUNTSWIMWALKBACKWARDS',
    594: 'Anim - FLYMOUNTFLIGHTIDLE',
    595: 'Anim - FLYMOUNTFLIGHTBACKWARDS',
    596: 'Anim - FLYMOUNTFLIGHTLEFT',
    597: 'Anim - FLYMOUNTFLIGHTRIGHT',
    598: 'Anim - FLYMOUNTFLIGHTRUN',
    599: 'Anim - FLYMOUNTFLIGHTSPRINT',
    600: 'Anim - FLYMOUNTFLIGHTWALK',
    601: 'Anim - FLYMOUNTFLIGHTWALKBACKWARDS',
    602: 'Anim - FLYMOUNTFLIGHTSTART',
    603: 'Anim - FLYMOUNTSWIMSTART',
    604: 'Anim - FLYMOUNTSWIMLAND',
    605: 'Anim - FLYMOUNTSWIMLANDRUN',
    606: 'Anim - FLYMOUNTFLIGHTLAND',
    607: 'Anim - FLYMOUNTFLIGHTLANDRUN',
    608: 'Anim - READYBLOWDART',
    609: 'Anim - FLYREADYBLOWDART',
    610: 'Anim - LOADBLOWDART',
    611: 'Anim - FLYLOADBLOWDART',
    612: 'Anim - HOLDBLOWDART',
    613: 'Anim - FLYHOLDBLOWDART',
    614: 'Anim - ATTACKBLOWDART',
    615: 'Anim - FLYATTACKBLOWDART',
    616: 'Anim - CARRIAGEMOUNT',
    617: 'Anim - FLYCARRIAGEMOUNT',
    618: 'Anim - CARRIAGEPASSENGERMOUNT',
    619: 'Anim - FLYCARRIAGEPASSENGERMOUNT',
    620: 'Anim - CARRIAGEMOUNTATTACK',
    621: 'Anim - FLYCARRIAGEMOUNTATTACK',
    622: 'Anim - BARTENDSTAND',
    623: 'Anim - FLYBARTENDSTAND',
    624: 'Anim - BARSERVERWALK',
    625: 'Anim - FLYBARSERVERWALK',
    626: 'Anim - BARSERVERRUN',
    627: 'Anim - FLYBARSERVERRUN',
    628: 'Anim - BARSERVERSHUFFLELEFT',
    629: 'Anim - FLYBARSERVERSHUFFLELEFT',
    630: 'Anim - BARSERVERSHUFFLERIGHT',
    631: 'Anim - FLYBARSERVERSHUFFLERIGHT',
    632: 'Anim - BARTENDEMOTETALK',
    633: 'Anim - FLYBARTENDEMOTETALK',
    634: 'Anim - BARTENDEMOTEPOINT',
    635: 'Anim - FLYBARTENDEMOTEPOINT',
    636: 'Anim - BARSERVERSTAND',
    637: 'Anim - FLYBARSERVERSTAND',
    638: 'Anim - BARSWEEPWALK',
    639: 'Anim - FLYBARSWEEPWALK',
    640: 'Anim - BARSWEEPRUN',
    641: 'Anim - FLYBARSWEEPRUN',
    642: 'Anim - BARSWEEPSHUFFLELEFT',
    643: 'Anim - FLYBARSWEEPSHUFFLELEFT',
    644: 'Anim - BARSWEEPSHUFFLERIGHT',
    645: 'Anim - FLYBARSWEEPSHUFFLERIGHT',
    646: 'Anim - BARSWEEPEMOTETALK',
    647: 'Anim - FLYBARSWEEPEMOTETALK',
    648: 'Anim - BARPATRONSITEMOTEPOINT',
    649: 'Anim - FLYBARPATRONSITEMOTEPOINT',
    650: 'Anim - MOUNTSELFIDLE',
    651: 'Anim - FLYMOUNTSELFIDLE',
    652: 'Anim - MOUNTSELFWALK',
    653: 'Anim - FLYMOUNTSELFWALK',
    654: 'Anim - MOUNTSELFRUN',
    655: 'Anim - FLYMOUNTSELFRUN',
    656: 'Anim - MOUNTSELFSPRINT',
    657: 'Anim - FLYMOUNTSELFSPRINT',
    658: 'Anim - MOUNTSELFRUNLEFT',
    659: 'Anim - FLYMOUNTSELFRUNLEFT',
    660: 'Anim - MOUNTSELFRUNRIGHT',
    661: 'Anim - FLYMOUNTSELFRUNRIGHT',
    662: 'Anim - MOUNTSELFSHUFFLELEFT',
    663: 'Anim - FLYMOUNTSELFSHUFFLELEFT',
    664: 'Anim - MOUNTSELFSHUFFLERIGHT',
    665: 'Anim - FLYMOUNTSELFSHUFFLERIGHT',
    666: 'Anim - MOUNTSELFWALKBACKWARDS',
    667: 'Anim - FLYMOUNTSELFWALKBACKWARDS',
    668: 'Anim - MOUNTSELFSPECIAL',
    669: 'Anim - FLYMOUNTSELFSPECIAL',
    670: 'Anim - MOUNTSELFJUMP',
    671: 'Anim - FLYMOUNTSELFJUMP',
    672: 'Anim - MOUNTSELFJUMPSTART',
    673: 'Anim - FLYMOUNTSELFJUMPSTART',
    674: 'Anim - MOUNTSELFJUMPEND',
    675: 'Anim - FLYMOUNTSELFJUMPEND',
    676: 'Anim - MOUNTSELFJUMPLANDRUN',
    677: 'Anim - FLYMOUNTSELFJUMPLANDRUN',
    678: 'Anim - MOUNTSELFSTART',
    679: 'Anim - FLYMOUNTSELFSTART',
    680: 'Anim - MOUNTSELFFALL',
    681: 'Anim - FLYMOUNTSELFFALL',
    682: 'Anim - STORMSTRIKE',
    683: 'Anim - FLYSTORMSTRIKE',
    684: 'Anim - READYJOUSTNOSHEATHE',
    685: 'Anim - FLYREADYJOUSTNOSHEATHE',
    686: 'Anim - SLAM',
    687: 'Anim - FLYSLAM',
    688: 'Anim - DEATHSTRIKE',
    689: 'Anim - FLYDEATHSTRIKE',
    690: 'Anim - SWIMATTACKUNARMED',
    691: 'Anim - FLYSWIMATTACKUNARMED',
    692: 'Anim - SPINNINGKICK',
    693: 'Anim - FLYSPINNINGKICK',
    694: 'Anim - ROUNDHOUSEKICK',
    695: 'Anim - FLYROUNDHOUSEKICK',
    696: 'Anim - ROLLSTART',
    697: 'Anim - FLYROLLSTART',
    698: 'Anim - ROLL',
    699: 'Anim - FLYROLL',
    700: 'Anim - ROLLEND',
    701: 'Anim - FLYROLLEND',
    702: 'Anim - PALMSTRIKE',
    703: 'Anim - FLYPALMSTRIKE',
    704: 'Anim - MONKOFFENSEATTACKUNARMED',
    705: 'Anim - FLYMONKOFFENSEATTACKUNARMED',
    706: 'Anim - MONKOFFENSEATTACKUNARMEDOFF',
    707: 'Anim - FLYMONKOFFENSEATTACKUNARMEDOFF',
    708: 'Anim - MONKOFFENSEPARRYUNARMED',
    709: 'Anim - FLYMONKOFFENSEPARRYUNARMED',
    710: 'Anim - MONKOFFENSEREADYUNARMED',
    711: 'Anim - FLYMONKOFFENSEREADYUNARMED',
    712: 'Anim - MONKOFFENSESPECIALUNARMED',
    713: 'Anim - FLYMONKOFFENSESPECIALUNARMED',
    714: 'Anim - MONKDEFENSEATTACKUNARMED',
    715: 'Anim - FLYMONKDEFENSEATTACKUNARMED',
    716: 'Anim - MONKDEFENSEATTACKUNARMEDOFF',
    717: 'Anim - FLYMONKDEFENSEATTACKUNARMEDOFF',
    718: 'Anim - MONKDEFENSEPARRYUNARMED',
    719: 'Anim - FLYMONKDEFENSEPARRYUNARMED',
    720: 'Anim - MONKDEFENSEREADYUNARMED',
    721: 'Anim - FLYMONKDEFENSEREADYUNARMED',
    722: 'Anim - MONKDEFENSESPECIALUNARMED',
    723: 'Anim - FLYMONKDEFENSESPECIALUNARMED',
    724: 'Anim - MONKHEALATTACKUNARMED',
    725: 'Anim - FLYMONKHEALATTACKUNARMED',
    726: 'Anim - MONKHEALATTACKUNARMEDOFF',
    727: 'Anim - FLYMONKHEALATTACKUNARMEDOFF',
    728: 'Anim - MONKHEALPARRYUNARMED',
    729: 'Anim - FLYMONKHEALPARRYUNARMED',
    730: 'Anim - MONKHEALREADYUNARMED',
    731: 'Anim - FLYMONKHEALREADYUNARMED',
    732: 'Anim - MONKHEALSPECIALUNARMED',
    733: 'Anim - FLYMONKHEALSPECIALUNARMED',
    734: 'Anim - FLYINGKICK',
    735: 'Anim - FLYFLYINGKICK',
    736: 'Anim - FLYINGKICKSTART',
    737: 'Anim - FLYFLYINGKICKSTART',
    738: 'Anim - FLYINGKICKEND',
    739: 'Anim - FLYFLYINGKICKEND',
    740: 'Anim - CRANESTART',
    741: 'Anim - FLYCRANESTART',
    742: 'Anim - CRANELOOP',
    743: 'Anim - FLYCRANELOOP',
    744: 'Anim - CRANEEND',
    745: 'Anim - FLYCRANEEND',
    746: 'Anim - DESPAWNED',
    747: 'Anim - FLYDESPAWNED',
    748: 'Anim - THOUSANDFISTS',
    749: 'Anim - FLYTHOUSANDFISTS',
    750: 'Anim - MONKHEALREADYSPELLDIRECTED',
    751: 'Anim - FLYMONKHEALREADYSPELLDIRECTED',
    752: 'Anim - MONKHEALREADYSPELLOMNI',
    753: 'Anim - FLYMONKHEALREADYSPELLOMNI',
    754: 'Anim - MONKHEALSPELLCASTDIRECTED',
    755: 'Anim - FLYMONKHEALSPELLCASTDIRECTED',
    756: 'Anim - MONKHEALSPELLCASTOMNI',
    757: 'Anim - FLYMONKHEALSPELLCASTOMNI',
    758: 'Anim - MONKHEALCHANNELCASTDIRECTED',
    759: 'Anim - FLYMONKHEALCHANNELCASTDIRECTED',
    760: 'Anim - MONKHEALCHANNELCASTOMNI',
    761: 'Anim - FLYMONKHEALCHANNELCASTOMNI',
    762: 'Anim - TORPEDO',
    763: 'Anim - FLYTORPEDO',
    764: 'Anim - MEDITATE',
    765: 'Anim - FLYMEDITATE',
    766: 'Anim - BREATHOFFIRE',
    767: 'Anim - FLYBREATHOFFIRE',
    768: 'Anim - RISINGSUNKICK',
    769: 'Anim - FLYRISINGSUNKICK',
    770: 'Anim - GROUNDKICK',
    771: 'Anim - FLYGROUNDKICK',
    772: 'Anim - KICKBACK',
    773: 'Anim - FLYKICKBACK',
    774: 'Anim - PETBATTLESTAND',
    775: 'Anim - FLYPETBATTLESTAND',
    776: 'Anim - PETBATTLEDEATH',
    777: 'Anim - FLYPETBATTLEDEATH',
    778: 'Anim - PETBATTLERUN',
    779: 'Anim - FLYPETBATTLERUN',
    780: 'Anim - PETBATTLEWOUND',
    781: 'Anim - FLYPETBATTLEWOUND',
    782: 'Anim - PETBATTLEATTACK',
    783: 'Anim - FLYPETBATTLEATTACK',
    784: 'Anim - PETBATTLEREADYSPELL',
    785: 'Anim - FLYPETBATTLEREADYSPELL',
    786: 'Anim - PETBATTLESPELLCAST',
    787: 'Anim - FLYPETBATTLESPELLCAST',
    788: 'Anim - PETBATTLECUSTOM0',
    789: 'Anim - FLYPETBATTLECUSTOM0',
    790: 'Anim - PETBATTLECUSTOM1',
    791: 'Anim - FLYPETBATTLECUSTOM1',
    792: 'Anim - PETBATTLECUSTOM2',
    793: 'Anim - FLYPETBATTLECUSTOM2',
    794: 'Anim - PETBATTLECUSTOM3',
    795: 'Anim - FLYPETBATTLECUSTOM3',
    796: 'Anim - PETBATTLEVICTORY',
    797: 'Anim - FLYPETBATTLEVICTORY',
    798: 'Anim - PETBATTLELOSS',
    799: 'Anim - FLYPETBATTLELOSS',
    800: 'Anim - PETBATTLESTUN',
    801: 'Anim - FLYPETBATTLESTUN',
    802: 'Anim - PETBATTLEDEAD',
    803: 'Anim - FLYPETBATTLEDEAD',
    804: 'Anim - PETBATTLEFREEZE',
    805: 'Anim - FLYPETBATTLEFREEZE',
    806: 'Anim - MONKOFFENSEATTACKWEAPON',
    807: 'Anim - FLYMONKOFFENSEATTACKWEAPON',
    808: 'Anim - BARTENDEMOTEWAVE',
    809: 'Anim - FLYBARTENDEMOTEWAVE',
    810: 'Anim - BARSERVEREMOTETALK',
    811: 'Anim - FLYBARSERVEREMOTETALK',
    812: 'Anim - BARSERVEREMOTEWAVE',
    813: 'Anim - FLYBARSERVEREMOTEWAVE',
    814: 'Anim - BARSERVERPOURDRINKS',
    815: 'Anim - FLYBARSERVERPOURDRINKS',
    816: 'Anim - BARSERVERPICKUP',
    817: 'Anim - FLYBARSERVERPICKUP',
    818: 'Anim - BARSERVERPUTDOWN',
    819: 'Anim - FLYBARSERVERPUTDOWN',
    820: 'Anim - BARSWEEPSTAND',
    821: 'Anim - FLYBARSWEEPSTAND',
    822: 'Anim - BARPATRONSIT',
    823: 'Anim - FLYBARPATRONSIT',
    824: 'Anim - BARPATRONSITEMOTETALK',
    825: 'Anim - FLYBARPATRONSITEMOTETALK',
    826: 'Anim - BARPATRONSTAND',
    827: 'Anim - FLYBARPATRONSTAND',
    828: 'Anim - BARPATRONSTANDEMOTETALK',
    829: 'Anim - FLYBARPATRONSTANDEMOTETALK',
    830: 'Anim - BARPATRONSTANDEMOTEPOINT',
    831: 'Anim - FLYBARPATRONSTANDEMOTEPOINT',
    832: 'Anim - CARRIONSWARM',
    833: 'Anim - FLYCARRIONSWARM',
    834: 'Anim - STANDVAR2',
    835: 'Anim - STANDVAR3',
    836: 'Anim - STANDVAR4',
    837: 'Anim - STANDVAR5',
    838: 'Anim - INSTOCKSVAR1',
    839: 'Anim - INSTOCKSVAR2',
    840: 'Anim - INSTOCKSVAR3',
    841: 'Anim - FLYINSTOCKSVAR1',
    842: 'Anim - FLYINSTOCKSVAR2',
    843: 'Anim - FLYINSTOCKSVAR3',
    844: 'Anim - BIRTHVAR0',
    845: 'Anim - BIRTHVAR1',
    846: 'Anim - DEATHVAR0',
    847: 'Anim - DEATHVAR1',
    848: 'Anim - DEATHVAR2',
    849: 'Anim - ATTACKUNARMEDVAR1',
    850: 'Anim - ATTACKUNARMEDVAR2',
};

// 530
const curveType = {
    0: 'Linear',
    1: 'Cubic',
    2: 'Bezier',
    3: 'Cosine',
};

// 555
const uiModelSceneCameraCameraType = {
    0: 'Orbit',
};

// 567
const expectedStatExpansionID = {
    '-2': '- INVALID -',
    0: 'Classic',
    1: 'Burning Crusade',
    2: 'Wrath of the Lich King',
    3: 'Cataclysm',
    4: 'Mists of Pandaria',
    5: 'Warlords of Draenor',
    6: 'Legion',
    7: 'Battle for Azeroth',
    8: 'Shadowlands',
    9: 'Dragonflight',
    10: 'The War Within',
    11: 'Midnight',
    12: 'The Last Titan',
};

// 588
const scenarioEventEntryTriggerType = {
    0: 'When the step starts',
    1: 'When the step is completed',
    2: 'When criteria tree "{CriteriaTree}" is true',
    3: 'Every {#Seconds} seconds',
    4: 'After {#Seconds} seconds',
    5: 'When completion criteria is {#Percent Complete:1,100}% complete',
};

// 590
const unitConditionVariable = {
    0: '- NONE -',
    1: 'Race {$Is/Is Not} "{ChrRaces}"',
    2: 'Class {$Is/Is Not} "{ChrClasses}"',
    3: 'Level {$Relative Op} "{#Level}"',
    4: 'Is self? {$Yes/No}{=1}',
    5: 'Is my pet? {$Yes/No}{=1}',
    6: 'Is master? {$Yes/No}{=1}',
    7: 'Is target? {$Yes/No}{=1}',
    8: 'Can assist? {$Yes/No}{=1}', // 'CAN_ASSIST'
    9: 'Can attack? {$Yes/No}{=1}', // 'CAN_ATTACK'
    10: 'Has pet? {$Yes/No}{=1}',
    11: 'Has weapon? {$Yes/No}{=1}',
    12: 'Health {$Relative Op} {#Health %}%',
    13: 'Mana {$Relative Op} {#Mana %}%',
    14: 'Rage {$Relative Op} {#Rage %}%',
    15: 'Energy {$Relative Op} {#Energy %}%',
    16: 'Combo Points {$Relative Op} {#Points}', // 'COMBO_POINTS'
    17: 'Has helpful aura spell? {$Yes/No} "{Spell}"',
    18: 'Has helpful aura dispel type? {$Yes/No} "{SpellDispelType}"',
    19: 'Has helpful aura mechanic? {$Yes/No} "{SpellMechanic}"',
    20: 'Has harmful aura spell? {$Yes/No} "{Spell}"',
    21: 'Has harmful aura dispel type? {$Yes/No} "{SpellDispelType}"',
    22: 'Has harmful aura mechanic? {$Yes/No} "{SpellMechanic}"',
    23: 'Has harmful aura school? {$Yes/No} "{Resistances}"',
    24: 'Damage (Physical) {$Relative Op} {#Physical Damage %}%', // 'DAMAGE_SCHOOL0_PERCENT'
    25: 'Damage (Holy) {$Relative Op} {#Holy Damage %}%', // 'DAMAGE_SCHOOL1_PERCENT'
    26: 'Damage (Fire) {$Relative Op} {#Fire Damage %}%', // 'DAMAGE_SCHOOL2_PERCENT'
    27: 'Damage (Nature) {$Relative Op} {#Nature Damage %}%', // 'DAMAGE_SCHOOL3_PERCENT'
    28: 'Damage (Frost) {$Relative Op} {#Frost Damage %}%', // 'DAMAGE_SCHOOL4_PERCENT'
    29: 'Damage (Shadow) {$Relative Op} {#Shadow Damage %}%', // 'DAMAGE_SCHOOL5_PERCENT'
    30: 'Damage (Arcane) {$Relative Op} {#Arcane Damage %}%', // 'DAMAGE_SCHOOL6_PERCENT'
    31: 'In combat? {$Yes/No}{=1}',
    32: 'Is moving? {$Yes/No}{=1}',
    33: 'Is casting? {$Yes/No}{=1}',
    34: 'Is casting spell? {$Yes/No}{=1}',
    35: 'Is channeling? {$Yes/No}{=1}',
    36: 'Is channeling spell? {$Yes/No}{=1}',
    37: 'Number of melee attackers {$Relative Op} {#Attackers}', // 'NPC_NUM_MELEE_ATTACKERS'
    38: 'Is attacking me? {$Yes/No}{=1}',
    39: 'Range {$Relative Op} {#Yards}',
    40: 'In melee range? {$Yes/No}{=1}', // 'IS_IN_MELEE_RANGE'
    41: 'Pursuit time {$Relative Op} {#Seconds}', // 'PURSUIT_TIME'
    42: 'Has harmful aura canceled by damage? {$Yes/No}{=1}', // 'HARMFUL_AURA_CANCELLED_BY_DAMAGE'
    43: 'Has harmful aura with periodic damage? {$Yes/No}{=1}',
    44: 'Number of enemies {$Relative Op} {#Enemies}',
    45: 'Number of friends {$Relative Op} {#Friends}', // 'NUM_FRIENDS'
    46: 'Threat (Physical) {$Relative Op} {#Physical Threat %}%', // 'THREAT_SCHOOL0_PERCENT'
    47: 'Threat (Holy) {$Relative Op} {#Holy Threat %}%', // 'THREAT_SCHOOL1_PERCENT'
    48: 'Threat (Fire) {$Relative Op} {#Fire Threat %}%', // 'THREAT_SCHOOL2_PERCENT'
    49: 'Threat (Nature) {$Relative Op} {#Nature Threat %}%', // 'THREAT_SCHOOL3_PERCENT'
    50: 'Threat (Frost) {$Relative Op} {#Frost Threat %}%', // 'THREAT_SCHOOL4_PERCENT'
    51: 'Threat (Shadow) {$Relative Op} {#Shadow Threat %}%', // 'THREAT_SCHOOL5_PERCENT'
    52: 'Threat (Arcane) {$Relative Op} {#Arcane Threat %}%', // 'THREAT_SCHOOL6_PERCENT'
    53: 'Is interruptible? {$Yes/No}{=1}', // 'IS_INTERRUPTIBLE'
    54: 'Number of attackers {$Relative Op} {#Attackers}',
    55: 'Number of ranged attackers {$Relative Op} {#Ranged Attackers}', // 'NPC_NUM_RANGED_ATTACKERS'
    56: 'Creature type {$Is/Is Not} "{CreatureType}"', // 'CREATURE_TYPE'
    57: 'Is melee-attacking? {$Yes/No}{=1}', // 'IN_MELEE_RANGE'
    58: 'Is ranged-attacking? {$Yes/No}{=1}',
    59: 'Health {$Relative Op} {#HP} HP',
    60: 'Spell known? {$Yes/No} "{Spell}"', // 'SPELL_KNOWN'
    61: 'Has harmful aura effect? {$Yes/No} "{#Spell Aura}"',
    62: 'Is immune to area-of-effect? {$Yes/No}{=1}', // 'IS_AREA_IMMUNE'
    63: 'Is player? {$Yes/No}{=1}',
    64: 'Damage (Magic) {$Relative Op} {#Magic Damage %}%', // 'DAMAGE_MAGIC_PERCENT'
    65: 'Damage (Total) {$Relative Op} {#Damage %}%', // 'DAMAGE_PERCENT'
    66: 'Threat (Magic) {$Relative Op} {#Magic Threat %}%', // 'THREAT_MAGIC_PERCENT'
    67: 'Threat (Total) {$Relative Op} {#Threat %}%', // 'THREAT_PERCENT'
    68: 'Has critter? {$Yes/No}{=1}',
    69: 'Has totem in slot 1? {$Yes/No}{=1}', // 'HAS_TOTEM1'
    70: 'Has totem in slot 2? {$Yes/No}{=1}', // 'HAS_TOTEM2'
    71: 'Has totem in slot 3? {$Yes/No}{=1}', // 'HAS_TOTEM3'
    72: 'Has totem in slot 4? {$Yes/No}{=1}', // 'HAS_TOTEM4'
    73: 'Has totem in slot 5? {$Yes/No}{=1}', // 'HAS_TOTEM5'
    74: 'Creature {$Is/Is Not} "{Creature}"',
    75: 'String ID {$Is/Is Not} "{StringID}"', // 'HAS_STRING_ID'
    76: 'Has aura? {$Yes/No} {Spell}', // 'HAS_AURA'
    77: 'Is enemy? {$Yes/No}{=1}', // 'REACTION_HOSTILE'
    78: 'Is spec - melee? {$Yes/No}{=1}', // 'CHAR_SPECIALIZATION_???'
    79: 'Is spec - tank? {$Yes/No}{=1}', // 'ROLE_IS_TANK'
    80: 'Is spec - ranged? {$Yes/No}{=1}', // 'CHAR_SPECIALIZATION_???'
    81: 'Is spec - healer? {$Yes/No}{=1}', // 'ROLE_IS_HEALER'
    82: 'Is player controlled NPC? {$Yes/No}{=1}',
    83: 'Is dying? {$Yes/No}{=1}',
    84: 'Path fail count {$Relative Op} {#Path Fail Count}', // 'PATH_FAIL_COUNT'
    86: 'Label {$Is/Is Not} "{Label}"', // 'HAS_LABEL
};

// 592
const criteriaStartEvent = {
    0: '- NONE -',
    1: 'Reach level {#Level}',
    2: 'Complete daily quest "{QuestV2}"',
    3: 'Start battleground "{Map}"',
    4: 'Win a ranked arena match with team size {#Team Size}',
    5: 'Gain aura "{Spell}"',
    6: 'Gain aura effect "{SpellAuraNames.EnumID}"',
    7: 'Cast spell "{Spell}"',
    8: 'Have spell "{Spell}" cast on you',
    9: 'Accept quest "{QuestV2}"',
    10: 'Kill NPC "{Creature}"',
    11: 'Kill player',
    12: 'Use item "{Item}"',
    13: 'Send event "{GameEvents}" (player-sent/instance only)',
    14: 'Begin scenario step "{#Step}" (for use with "Player on Scenario" modifier only)',
};

// 593
const criteriaFailEvent = {
    0: '- NONE -',
    1: 'Death',
    2: '24 hours without completing a daily quest',
    3: 'Leave a battleground',
    4: 'Lose a ranked arena match with team size {#Team Size}',
    5: 'Lose aura "{Spell}"',
    6: 'Gain aura "{Spell}"',
    7: 'Gain aura effect "{SpellAuraNames.EnumID}"',
    8: 'Cast spell "{Spell}"',
    9: 'Have spell "{Spell}" cast on you',
    10: 'Modify your party status',
    11: 'Lose a pet battle',
    12: 'Battle pet dies',
    13: 'Daily quests cleared',
    14: 'Send event "{GameEvents}" (player-sent/instance only)',
};

// 594
const criteriaType = {
    0: 'Kill NPC "{Creature}"', // KILL_CREATURE -- creature::ID
    1: 'Win battleground "{Map}"', // WIN_BG -- map::ID
    2: 'Complete research project "{ResearchProject}"', // -- researchproject::ID
    3: 'Complete any research project', // COMPLETE_ARCHAEOLOGY_PROJECTS -- No FK
    4: 'Find research object "{GameObjects}"', // SURVEY_GAMEOBJECT -- gameobjects::ID
    5: 'Reach level', // REACH_LEVEL -- No FK
    6: 'Exhaust any research site', // CLEAR_DIGSITE -- No FK
    7: 'Skill "{SkillLine}" raised', // REACH_SKILL_LEVEL -- skillline::ID
    8: 'Earn achievement "{Achievement}"', // COMPLETE_ACHIEVEMENT -- achievement::ID
    9: 'Count of complete quests (quest count)', // COMPLETE_QUEST_COUNT -- No FK
    10: 'Complete any daily quest (per day)', // COMPLETE_DAILY_QUEST_DAILY -- No FK
    11: 'Complete quests in "{AreaTable}"', // COMPLETE_QUESTS_IN_ZONE -- areatable::ID
    12: 'Currency "{CurrencyTypes}" gained', // CURRENCY -- currencytypes::ID
    13: 'Damage dealt', // DAMAGE_DONE -- No FK
    14: 'Complete daily quest', // COMPLETE_DAILY_QUEST -- No FK
    15: 'Participate in battleground "{Map}"', // COMPLETE_BATTLEGROUND -- map::ID
    16: 'Die on map "{Map}"', // DEATH_AT_MAP -- map::ID
    17: 'Die anywhere', // DEATH -- No FK
    18: 'Die in an instance which handles at most {#Max Players} players', // DEATH_IN_DUNGEON
    19: 'Run an instance which handles at most {#Max Players} players', // COMPLETE_RAID
    20: 'Get killed by "{Creature}"', // KILLED_BY_CREATURE -- creature::ID
    21: 'Designer Value{`Uses Record ID}', // MANUAL_COMPLETE_CRITERIA -- criteria::ID
    22: 'Complete any challenge mode', // COMPLETE_CHALLENGE_MODE_GUILD -- No FK
    23: 'Die to a player', // KILLED_BY_PLAYER -- No FK
    24: 'Maximum distance fallen without dying', // FALL_WITHOUT_DYING -- No FK
    25: 'Earn a challenge mode medal of "{#Challenge Mode Medal (OBSOLETE)}" (OBSOLETE)',
    26: 'Die to "{$Env Damage}" environmental damage', // DEATHS_FROM
    27: 'Complete quest "{QuestV2}"', // COMPLETE_QUEST -- questv2:ID
    28: 'Have the spell "{Spell}" cast on you', // BE_SPELL_TARGET -- spell::ID
    29: 'Cast the spell "{Spell}"', // CAST_SPELL -- spell::ID
    30: 'Tracked WorldStateUI value "{WorldStateUI}" is modified', // BG_OBJECTIVE_CAPTURE -- pvpstat::ID
    31: 'Kill someone in PVP in "{AreaTable}"', // HONORABLE_KILL_AT_AREA -- areatable::ID
    32: 'Win arena "{Map}"', // WIN_ARENA -- map::ID
    33: 'Participate in arena "{Map}"', // PLAY_ARENA -- map::ID
    34: 'Learn or Know spell "{Spell}"', // LEARN_SPELL -- spell::ID
    35: 'Earn an honorable kill', // HONORABLE_KILL -- No FK
    36: 'Acquire item "{Item}"', // OWN_ITEM -- item::ID
    37: 'Win a ranked arena match (any arena)', // WIN_RATED_ARENA -- No FK
    38: 'Earn a team arena rating of {#Arena Rating}', // HIGHEST_TEAM_RATING -- No FK
    39: 'Earn a personal arena rating of {#Arena Rating}', // HIGHEST_PERSONAL_RATING -- No FK
    40: 'Achieve a skill step in "{SkillLine}"', // LEARN_SKILL_LEVEL -- skilline::ID
    41: 'Use item "{Item}"', // USE_ITEM -- item::ID
    42: 'Loot "{Item}" via corpse, pickpocket, fishing, disenchanting, etc.', // LOOT_ITEM -- item::ID
    43: 'Reveal world map overlay "{WorldMapOverlay}"', // EXPLORE_AREA -- areatable::ID
    44: 'Deprecated PVP Titles', // OWN_RANK -- No FK
    45: 'Bank slots purchased', // BUY_BANK_SLOT -- No FK
    46: 'Reputation gained with faction "{Faction}"', // GAIN_REPUTATION -- faction::ID
    47: 'Total exalted factions', // GAIN_EXALTED_REPUTATION -- No FK
    48: 'Got a haircut', // VISIT_BARBER_SHOP -- No FK
    49: 'Equip item in slot "{$Equip Slot}"', // EQUIP_EPIC_ITEM -- No FK
    50: 'Roll need and get {#Need Roll}', // ROLL_NEED_ON_LOOT -- No FK
    51: 'Roll greed and get {#Greed Roll}', // ROLL_GREED_ON_LOOT -- No FK
    52: 'Deliver a killing blow to a {ChrClasses}', // HK_CLASS -- chrclasses::ID
    53: 'Deliver a killing blow to a {ChrRaces}', // HK_RACE -- chrraces::ID
    54: 'Do a "{EmotesText}" emote', // DO_EMOTE -- emotes::ID
    55: 'Healing done', // HEALING_DONE -- No FK
    56: 'Delivered a killing blow', // GET_KILLING_BLOWS -- No FK
    57: 'Equip item "{Item}"', // EQUIP_ITEM -- item::ID
    58: 'Complete quests in "{QuestSort}"',
    59: 'Sell items to vendors', // MONEY_FROM_VENDORS -- No FK
    60: 'Money spent on respecs', // GOLD_SPENT_FOR_TALENTS -- No FK
    61: 'Total respecs', // NUMBER_OF_TALENT_RESETS -- No FK
    62: 'Money earned from questing', // MONEY_FROM_QUEST_REWARD -- No FK
    63: 'Money spent on taxis', // GOLD_SPENT_FOR_TRAVELLING -- No FK
    64: 'Killed all units in spawn region "{SpawnRegion}"', // DEFEAT_CREATURE_GROUP
    65: 'Money spent at the barber shop', // GOLD_SPENT_AT_BARBER -- No FK
    66: 'Money spent on postage', // GOLD_SPENT_FOR_MAIL -- No FK
    67: 'Money looted from creatures', // LOOT_MONEY -- No FK
    68: 'Use Game Object "{GameObjects}"', // USE_GAMEOBJECT -- gameobjects::ID
    69: 'Gain aura "{Spell}"', // BE_SPELL_TARGET2 -- spell::ID
    70: 'Kill a player (no honor check)', // SPECIAL_PVP_KILL -- No FK
    71: 'Complete a challenge mode on map "{Map}"', // COMPLETE_CHALLENGE_MODE -- map::ID
    72: 'Catch fish in the "{GameObjects}" fishing hole', // FISH_IN_GAMEOBJECT -- gameobjects::ID
    73: 'Player will Trigger game event "{GameEvents}"', // SEND_EVENT
    74: 'Login (USE SPARINGLY!)', // ON_LOGIN -- No FK
    75: 'Learn spell from the "{SkillLine}" skill line', // LEARN_SKILLLINE_SPELLS -- skillline::ID
    76: 'Win a duel', // WIN_DUEL -- No FK
    77: 'Lose a duel', // LOSE_DUEL -- No FK
    78: 'Kill any NPC', // KILL_CREATURE_TYPE -- No FK
    79: 'Created items by casting a spell (limit 1 per create...)', // COOK_RECIPES_GUILD -- No FK
    80: 'Money earned from auctions', // GOLD_EARNED_BY_AUCTIONS -- No FK
    81: 'Battle pet achievement points earned', // EARN_PET_BATTLE_ACHIEVEMENT_POINTS -- No FK
    82: 'Number of items posted at auction', // CREATE_AUCTION -- No FK
    83: 'Highest auction bid', // HIGHEST_AUCTION_BID -- No FK
    84: 'Auctions won', // WON_AUCTIONS -- No FK
    85: 'Highest coin value of item sold', // HIGHEST_AUCTION_SOLD -- No FK
    86: 'Most money owned', // HIGHEST_GOLD_VALUE_OWNED -- No FK
    87: 'Total revered factions', // GAIN_REVERED_REPUTATION -- No FK
    88: 'Total honored factions', // GAIN_HONORED_REPUTATION -- No FK
    89: 'Total factions encountered', // KNOWN_FACTIONS -- No FK
    90: 'Loot any item', // LOOT_EPIC_ITEM -- No FK
    91: 'Obtain any item', // RECEIVE_EPIC_ITEM -- No FK
    92: 'Anyone will Trigger game event "{GameEvents}" (Scenario Only)', // SEND_EVENT_SCENARIO
    93: 'Roll any number on need', // ROLL_NEED -- No FK
    94: 'Roll any number on greed', // ROLL_GREED -- No FK
    95: 'Released Spirit', // RELEASE_SPIRIT -- No FK
    96: 'Account knows pet "{Creature}" (Backtracked)', // OWN_PET -- creature::ID
    97: 'Defeat Encounter "{DungeonEncounter}" While Eligible For Loot', // GARRISON_COMPLETE_DUNGEON_ENCOUNTER -- dungeonencounter::ID
    98: 'UNUSED 18{}',
    99: 'UNUSED 19{}',
    100: 'UNUSED 20{}',
    101: 'Highest damage done in 1 single ability', // HIGHEST_HIT_DEALT -- No FK
    102: 'Most damage taken in 1 single hit', // HIGHEST_HIT_RECEIVED -- No FK
    103: 'Total damage taken', // TOTAL_DAMAGE_RECEIVED -- No FK
    104: 'Largest heal cast', // HIGHEST_HEAL_CAST -- No FK
    105: 'Total healing received', // TOTAL_HEALING_RECEIVED -- No FK
    106: 'Largest heal received', // HIGHEST_HEALING_RECEIVED -- No FK
    107: 'Abandon any quest', // QUEST_ABANDONED -- No FK
    108: 'Buy a taxi', // FLIGHT_PATHS_TAKEN -- No FK
    109: 'Get loot via "{$Loot Acquisition}"', // LOOT_TYPE -- No FK
    110: 'Land targeted spell "{Spell}" on a target', // CAST_SPELL2 -- spell::ID
    111: 'UNUSED 21{}',
    112: 'Learn tradeskill skill line "{SkillLine}"', // LEARN_SKILL_LINE -- skillline::ID
    113: 'Honorable kills (number in interface, won\'t update except for login)', // EARN_HONORABLE_KILL -- No FK
    114: 'Accept a summon', // ACCEPTED_SUMMONINGS -- No FK
    115: 'Earn achievement points', // EARN_ACHIEVEMENT_POINTS -- No FK
    116: 'Roll disenchant and get {#Disenchant Roll}',
    117: 'Roll any number on disenchant',
    118: 'Completed an LFG dungeon', // COMPLETE_LFG_DUNGEON -- No FK
    119: 'Completed an LFG dungeon with strangers', // USE_LFD_TO_GROUP_WITH_PLAYERS -- No FK
    120: 'Kicked in an LFG dungeon (initiator)', // LFG_VOTE_KICKS_INITIATED_BY_PLAYER -- No FK
    121: 'Kicked in an LFG dungeon (voter)', // LFG_VOTE_KICKS_NOT_INIT_BY_PLAYER -- No FK
    122: 'Kicked in an LFG dungeon (target)', // BE_KICKED_FROM_LFG -- No FK
    123: 'Abandoned an LFG dungeon', // LFG_LEAVES -- No FK
    124: 'Guild repair amount spent', // SPENT_GOLD_GUILD_REPAIRS -- No FK
    125: 'Guild attained level', // REACH_GUILD_LEVEL -- No FK
    126: 'Created items by casting a spell', // CRAFT_ITEMS_GUILD -- No FK
    127: 'Fish in any pool', // CATCH_FROM_POOL -- No FK
    128: 'Guild bank tabs purchased', // BUY_GUILD_BANK_SLOTS -- No FK
    129: 'Earn guild achievement points', // EARN_GUILD_ACHIEVEMENT_POINTS -- No FK
    130: 'Win any battleground', // WIN_RATED_BATTLEGROUND -- No FK
    131: 'Participate in any battleground',
    132: 'Earn a battleground rating', // REACH_BG_RATING -- No FK
    133: 'Guild tabard created', // BUY_GUILD_TABARD -- No FK
    134: 'Count of complete quests for guild (Quest count)', // COMPLETE_QUESTS_GUILD -- No FK
    135: 'Honorable kills for Guild', // HONORABLE_KILLS_GUILD -- No FK
    136: 'Kill any NPC for Guild', // KILL_CREATURE_TYPE_GUILD -- No FK
    137: 'Grouped tank left early in an LFG dungeon', // COUNT_OF_LFG_QUEUE_BOOSTS_BY_TANK -- No FK
    138: 'Complete a "{$Guild Challenge}" guild challenge', // COMPLETE_GUILD_CHALLENGE_TYPE
    139: 'Complete any guild challenge', // COMPLETE_GUILD_CHALLENGE -- No FK
    140: 'Marked AFK in a battleground',
    141: 'Removed for being AFK in a battleground',
    142: 'Start any battleground (AFK tracking)',
    143: 'Complete any battleground (AFK tracking)',
    144: 'Marked someone for being AFK in a battleground',
    145: 'Completed an LFR dungeon', // LFR_DUNGEONS_COMPLETED -- No FK
    146: 'Abandoned an LFR dungeon', // LFR_LEAVES -- No FK
    147: 'Kicked in an LFR dungeon (initiator)', // LFR_VOTE_KICKS_INITIATED_BY_PLAYER -- No FK
    148: 'Kicked in an LFR dungeon (voter)', // LFR_VOTE_KICKS_NOT_INIT_BY_PLAYER -- No FK
    149: 'Kicked in an LFR dungeon (target)', // BE_KICKED_FROM_LFR -- No FK
    150: 'Grouped tank left early in an LFR dungeon', // COUNT_OF_LFR_QUEUE_BOOSTS_BY_TANK -- No FK
    151: 'Complete a Scenario', // COMPLETE_SCENARIO_COUNT -- No FK
    152: 'Complete scenario "{Scenario}"', // COMPLETE_SCENARIO -- scenario::ID
    153: 'Enter area trigger "{AreaTriggerActionSet}"', // REACH_AREATRIGGER_WITH_ACTIONSET
    154: 'Leave area trigger "{AreaTriggerActionSet}"',
    155: '(Account Only) Learned a new pet', // OWN_BATTLE_PET -- No FK
    156: '(Account Only) Unique pets owned', // OWN_BATTLE_PET_COUNT -- No FK
    157: '(Account Only) Obtain a pet through battle', // CAPTURE_BATTLE_PET -- No FK
    158: 'Win a pet battle', // WIN_PET_BATTLE -- No FK
    159: 'Lose a pet battle',
    160: '(Account Only) Battle pet has reached level {#Level}', // LEVEL_BATTLE_PET -- No FK
    161: '(Player) Obtain a pet through battle', // CAPTURE_BATTLE_PET_CREDIT -- No FK
    162: '(Player) Actively earn level {#Level} with a pet by a player', // LEVEL_BATTLE_PET_CREDIT -- No FK
    163: 'Enter Map Area "{AreaTable}"', // ENTER_AREA -- areatable::ID
    164: 'Leave Map Area "{AreaTable}"', // LEAVE_AREA -- areatable::ID
    165: 'Defeat Encounter "{DungeonEncounter}"', // COMPLETE_DUNGEON_ENCOUNTER -- dungeonencounter::ID
    166: 'Garrison Building: Place any',
    167: 'Garrison Building: Place "{GarrBuilding}"', // PLACE_GARRISON_BUILDING -- garrbuilding::ID
    168: 'Garrison Building: Activate any', // UPGRADE_GARRISON_BUILDING -- No FK
    169: 'Garrison Building: Activate "{GarrBuilding}"', // CONSTRUCT_GARRISON_BUILDING -- garrbuilding::ID
    170: 'Garrison: Upgrade Garrison to Tier "{#Tier:2,3}"', // UPGRADE_GARRISON -- No FK (GarrLevel)
    171: 'Garrison Mission: Start any with FollowerType "{GarrFollowerType}"', // START_GARRISON_MISSION -- No FK
    172: 'Garrison Mission: Start "{GarrMission}"', // START_ORDER_HALL_MISSION -- garrmission::ID
    173: 'Garrison Mission: Succeed any with FollowerType "{GarrFollowerType}"', // COMPLETE_GARRISON_MISSION_COUNT -- No FK
    174: 'Garrison Mission: Succeed "{GarrMission}"', // COMPLETE_GARRISON_MISSION -- garrmission::ID
    175: 'Garrison Follower: Recruit any', // RECRUIT_GARRISON_FOLLOWER_COUNT -- No FK
    176: 'Garrison Follower: Recruit "{GarrFollower}"', // RECRUIT_GARRISON_FOLLOWER -- garrfollower::ID
    177: 'Garrison: Acquire a Garrison',
    178: 'Garrison Blueprint: Learn any', // LEARN_GARRISON_BLUEPRINT_COUNT -- No FK
    179: 'Garrison Blueprint: Learn "{GarrBuilding}"',
    180: 'Garrison Specialization: Learn any',
    181: 'Garrison Specialization: Learn "{GarrSpecialization}"',
    182: 'Garrison Shipment of type "{CharShipmentContainer}" collected', // COMPLETE_GARRISON_SHIPMENT -- No FK
    183: 'Garrison Follower: Item Level Changed', // RAISE_GARRISON_FOLLOWER_ITEM_LEVEL -- No FK
    184: 'Garrison Follower: Level Changed', // RAISE_GARRISON_FOLLOWER_LEVEL -- No FK
    185: 'Learn Toy "{Item}"', // OWN_TOY
    186: 'Learn Any Toy', // OWN_TOY_COUNT
    187: 'Garrison Follower: Quality Upgraded', // RECRUIT_GARRISON_FOLLOWER_WITH_QUALITY -- No FK
    188: 'Learn Heirloom "{Item}"',
    189: 'Learn Any Heirloom', // OWN_HEIRLOOMS -- No FK
    190: 'Earn Artifact XP', // ARTIFACT_POWER_EARNED -- No FK
    191: 'Artifact Power Ranks Purchased', // ARTIFACT_TRAITS_UNLOCKED -- No FK
    192: 'Learn Transmog "{ItemModifiedAppearance}"',
    193: 'Learn Any Transmog',
    194: '(Player) honor level increase', // HONOR_LEVEL_REACHED -- No FK
    195: '(Player) prestige level increase', // PRESTIGE_REACHED -- No FK
    196: 'Actively level to level {#Level}', // HERITAGE_AT_LEVEL -- No FK (Level Reached, points to Heritage achievements)
    197: 'Garrison Talent: Complete Research Any', // COVENANT_SANCTUM_RANK_REACHED -- No FK
    198: 'Garrison Talent: Complete Research "{GarrTalent}"', // ORDER_HALL_TALENT_LEARNED -- No FK
    199: 'Learn Any Transmog in Slot "{$Equip Slot}"', // APPEARANCE_UNLOCKED_BY_SLOT -- No FK (Slot)
    200: 'Recruit any Garrison Troop', // ORDER_HALL_RECRUIT_TROOP -- No FK
    201: 'Garrison Talent: Start Research Any',
    202: 'Garrison Talent: Start Research "{GarrTalent}"', // RESEARCHED_GARRISON_TALENT -- garrtalent::ID
    203: 'Complete Any Quest', // COMPLETE_WORLD_QUEST -- No FK
    204: 'Earn License "{BattlePayDeliverable}" (does NOT work for box level)', // TRANSMOG_SET_RELATED -- transmogset::ID
    205: '(Account Only) Collect a Transmog Set from Group "{TransmogSetGroup}"', // TRANSMOG_SET_UNLOCKED -- transmogset::ID (?)
    206: '(Player) paragon level increase with faction "{Faction}"', // GAIN_PARAGON_REPUTATION -- No FK
    207: 'Player has earned honor', // EARN_HONOR_XP -- No FK
    208: 'Kill NPC "{Creature}" (scenario criteria only, do not use for player)',
    209: 'Artifact Power Rank of "{ArtifactPower}" Purchased',
    210: 'Choose any Relic Talent',
    211: 'Choose Relic Talent "{ArtifactPower}"', // RELIC_TALENT_UNLOCKED -- artifactpower::ID (?)
    212: 'Earn Expansion Level "{$Expansion Level}"',
    // incomplete
    213: '(Account Only) honor level {#Level} reached', // No FK (Honor Level Reached)
    214: 'Earn Artifact experience for Azerite Item', // No FK
    215: 'Azerite Level {#Azerite Level} reached', // No FK (Neck Level Reached)
    216: 'Mythic Plus Completed', // No FK
    217: 'Scenario Group Completed',
    218: 'Complete Any Replay Quest', // No FK
    219: 'Buy items from vendors', // No FK
    220: 'Sell items to vendors', // No FK
    221: 'Reach Max Level',
    222: 'Memorize Spell "{Spell}"', // FK todo
    223: 'Learn Transmog Illusion',
    224: 'Learn Any Transmog Illusion',
    225: 'Enter Top Level Map Area "{AreaTable}"', // areatable::ID
    226: 'Leave Top Level Map Area "{AreaTable}"', // FK todo
    227: 'Socket Garrison Talent {GarrTalent}',
    228: 'Socket Any Soulbind Conduit', // No FK
    229: 'Obtain Any Item With Currency Value "{CurrencyTypes}"', // No FK
    230: '(Player) Mythic+ Rating "{#DungeonScore}" attained',
    231: '(Player) spent talent point'
};

// 601
const ItemBonding = {
    0: 'Not Bound',
    1: 'Bind On Acquire',
    2: 'Bind On Equip',
    3: 'Bind On Use',
    4: 'Quest Item',
    5: 'Quest Item (Multi)',
    6: 'Multi'
}

// 644
const spellVisualEventStartEvent = {
    0: 'None',
    1: 'Precast Start',
    2: 'Precast End',
    3: 'Cast',
    4: 'Travel Start',
    5: 'Travel End',
    6: 'Impact',
    7: 'Aura Start',
    8: 'Aura End',
    9: 'Area Trigger Start',
    10: 'Area Trigger End',
    11: 'Channel Start',
    12: 'Channel End',
    13: 'One-Shot',
};

// 646
const spellVisualEventTargetType = {
    0: 'None',
    1: 'Caster',
    2: 'Target',
    3: 'Area',
    4: 'Target (Not Caster)',
    5: 'Missile Destination',
};

// 651
const relicTalentType = {
    0: 'Item Level',
    1: 'Void Power',
    2: 'Light Power',
    3: 'Bonus Rank',
};

// 712
const azeriteTierID = {
    0: 'Tier Four',
    1: 'Tier Three',
    2: 'Tier Two',
    3: 'Tier One',
    4: 'Outer Ring',
};

// 721
const SpellScript_Arguments = {
    0:   '(caster, spellrec)',
    1:   '(caster, cast, ...)',
    2:   '(caster, castinfo, verifyOnly)',
    3:   '(caster, cast, value)',
    4:   '(unit, aura, value)',
    5:   '(unit, aura, slot)',
    6:   '*',
    7:   '(victim, attacker, spell, effectIndex, ...)',
    8:   '(unit, caster, spell)',
    9:   '(caster, castinfo, targetindex, value)',
    10:  '(caster, target, value)',
    11:  '(caster, castinfo, spellrec, effectindex, target, value)',
    12:  '(unit, aura, procType, procSubType, target, spellrec, value1, value2, value3, value4, value5, procTypeB, originalCastGUID)',
    13:  '(unit, aura, effectindex, attacker, spellrec, value, istest, isperiodic, isreflected, isredirected)',
    14:  '(caster, spell, unit, aura)',
    15:  '(unit, aura, item, slot, equipFlag)',
    16:  '(unit, spellRec, aura, flag, flagB, interruptedBySpellID, interruptedByGUID)',
    17:  '(caster, reason, value1, value2)',
    18:  '(unit, aura, target)',
    19:  '(caster, target, spellrec, castlevel, duration)',
    20:  '(caster, spellrec, spelllevel, castingtime)',
    21:  '(target, caster, dispeller, spell, value)',
    22:  '(unit, areaTrigger)',
    23:  '()',
    24:  '(elapsed)',
    25:  '(flag, event, unit, team)',
    26:  '(winningTeam)',
    27:  '(castinfo, objType)',
    28:  '(group, unit, actionGUID, targetGUID, param, position, stringID)',
    29:  '(caster, spell, position, pathContainer)',
    30:  '(capturePoint, event, unit, team)',
    31:  '(caster, spell, position, pointContainer)',
    32:  '(encounter, attempt)',
    33:  '(encounter, attempt, success)',
    34:  '(treasureID, childTreasureID, itemID, currencyID, input, looters)',
    35:  '(followerData, removed)',
    36:  '(scenario)',
    37:  '(eventID, triggeredByGuid, param)',
    38:  '(paramTable)',
    39:  '(player)',
    40:  '(unit, aura, value, removedReason)',
    41:  '(player, mawPowerID, weight)',
    42:  '(player, filterID, powers)',
    43:  '(player, filterID, quantity)',
    44:  '(player, contextIDs, contextOwnerID, tags)',
    45:  '(player, floorDifficulty, floorIndex)',
    46:  '(elapsedMs)',
    47:  '(caster, spellID)',
    48:  '(player, casterGUID, spellID)',
};

// 743
const weatherType = {
    1: 'Clear',
    2: 'Rain',
    3: 'Snow',
    4: 'Sandstorm',
    5: 'Miscellaneous',
};

// 747
const chrRacesPlayableRaceBit = {
    0: 'Human',
    1: 'Orc',
    2: 'Dwarf',
    3: 'Night Elf',
    4: 'Undead',
    5: 'Tauren',
    6: 'Gnome',
    7: 'Troll',
    8: 'Goblin',
    9: 'Blood Elf',
    10: 'Draenei',
    11: 'Dark Iron Dwarf',
    12: 'Vulpera',
    13: 'Orc Clans',
    14: 'Mechagnome',
    15: 'RE-USE ME',
    16: 'RE-USE ME',
    17: 'RE-USE ME',
    18: 'RE-USE ME',
    19: 'RE-USE ME',
    20: 'RE-USE ME',
    21: 'Worgen',
    22: 'RE-USE ME',
    23: 'Pandaren(N)',
    24: 'Pandaren(A)',
    25: 'Pandaren(H)',
    26: 'Nightborne',
    27: 'Highmountain Tauren',
    28: 'Void Elf',
    29: 'Lightforged Draenei',
    30: 'Zandalari Troll',
    31: 'Kul\'Tiran',
    63: 'This bit can be reused but not deleted',
};

// 863
const UiWidgetVisualization_VisType = {
    0:  'IconAndText',
    1:  'CaptureBar',
    2:  'StatusBar',
    3:  'DoubleStatusBar',
    4:  'IconTextAndBackground',
    5:  'DoubleIconAndText',
    6:  'StackedResourceTracker',
    7:  'IconTextAndCurrencies',
    8:  'TextWithState',
    9:  'HorizontalCurrencies',
    10: 'BulletTextList',
    11: 'ScenarioHeaderCurrenciesAndBackground',
    12: 'TextureAndText',
    13: 'SpellDisplay',
    14: 'DoubleStateIconRow',
    15: 'TextureAndTextRow',
    16: 'ZoneControl',
    17: 'CaptureZone',
    18: 'TextureWithAnimation',
    19: 'DiscreteProgressSteps',
    20: 'ScenarioHeaderTimer',
}

// 864
const UiWidgetDataSource_SourceType = {
    // -1: 'Use Constant',
    0: 'World State',
    1: 'World State Expression',
    2: 'Currency Current Value',
    3: 'Currency Max Value',
    4: 'Contribution Percentage',
    5: 'Contribution Time Remaining',
    6: 'Contribution Time Total',
    7: 'Contribution State',
    8: 'Player Aura Presence',
    9: 'Player Aura Stack Count',
    10: 'Player Aura Time Remaining',
    11: 'Player Aura Time Total',
    12: 'Player Aura Effect 1 Points',
    13: 'Player Aura Effect 2 Points',
    14: 'Player Aura Effect 3 Points',
    15: 'Quest Presence',
    16: 'Quest Presence Complete',
    17: 'Quest Presence Not Complete',
    18: 'Quest Current Percentage',
    19: 'Quest Objective Current Count',
    20: 'Quest Objective Max Count',
    21: 'Quest Objective Presence',
    22: 'Quest Objective Presence Complete',
    23: 'Quest Objective Presence Not Complete',
    24: 'Quest Completed',
    25: 'Faction Reaction',
    26: 'Faction Standing',
    27: 'Faction Reaction Min Threshold',
    28: 'Faction Reaction Max Threshold',
    29: 'Faction Reaction Is Min',
    30: 'Faction Reaction Is Neutral',
    31: 'Faction Reaction Is Max',
    32: 'Faction Reaction Is Not Max',
    33: 'Quest Remaining Percentage',
    34: 'Quest Objective Remaining Count',
    35: 'Inventory Item Count',
    36: 'Inventory Item Count Bags Only',
    37: 'Scene Script Package Is Active',
    38: 'Scene Script Var Value',
    39: 'Scenario Is Active',
    40: 'Scenario Num Steps',
    41: 'Scenario Remaining Steps',
    42: 'Scenario Current Step',
    43: 'Scenario Current Step Zero Based',
    44: 'Scenario Step Is Active',
    45: 'Jailers Tower - Current Floor Index',
    46: 'Jailers Tower - Current Floor Level',
    47: 'Player Power Current',
    48: 'Player Power Min',
    49: 'Player Power Max',
    50: 'Player Power Is Min',
    51: 'Player Power Is Max',
    52: 'Player Aura Effect 4 Points',
    53: 'Player Aura Effect 5 Points',
    54: 'Unit Aura Presence',
    55: 'Unit Aura Stack Count',
    56: 'Unit Aura Time Remaining',
    57: 'Unit Aura Time Total',
    58: 'Unit Aura Effect 1 Points',
    59: 'Unit Aura Effect 2 Points',
    60: 'Unit Aura Effect 3 Points',
    61: 'Unit Aura Effect 4 Points',
    62: 'Unit Aura Effect 5 Points',
};

// 865
const UiWidgetVisTypeDataReq_ValueType = {
    0: 'Integer',
    1: 'String',
}

// 890
const liquidTypeXTextureType = {
    '-1': 'Disabled',
    0: 'Ocean',
    1: 'River',
    2: 'WMO',
};

// 915
const waypointNodeField_8_2_0_30080_005 = {
    0: 'Checkpoint',
    1: 'Portal Entrance',
    2: 'Portal Exit',
};

// 982
const garrPlotType = {
    0: 'Small',
    1: 'Medium',
    2: 'Large',
    3: 'Farm',
    4: 'Mine',
    5: 'Fishing',
    6: 'Pet Menagerie',
    7: 'Shipyard',
}

// 989
const garrSpecType = {
    0: ['REDUCE_TRAVEL_TIME', 'Reduce travel time'],
    1: ['STABLE_EXTRA_MOUNTS', 'Stable extra mounts'],
    2: ['RECALL_FOLLOWERS', 'Recall followers'],
    3: ['GENERATE_ITEM_RECURRING', 'Generate item recurring'],
    4: ['RECOVER_FOLLOWER', 'Recover follower'],
    5: ['INCREASED_HEALTH', 'Increased health'],
    6: ['FOLLOWER_DISCOVERY_CHANCE_INCREASE', 'Follower discovery chance increase'],
    7: ['INCREASE_GATHERING_RATE', 'Increase gathering rate'],
    8: ['MENAGERIE_EXTRA_PETS', 'Menagerie extra pets'],
    9: ['COST_MULTIPLIER', 'Building/Spec Cost Modifier'],
}

// 993
const garrAbilityTargetType = {
    0: 'None',
    1: 'Self',
    2: 'Party',
    3: 'Race',
    4: 'Class',
    5: 'Gender',
    6: 'Profession',
    7: 'Not Self',
    8: 'Not Race',
    9: 'Not Class',
    10: 'Not Profession'
}

// 999
const garrTalentType = {
    0: 'Standard',
    1: 'Minor',
    2: 'Major',
    3: 'Socket',
}

// 1003
const garrTalentResearchCostSource = {
    0: 'Talent',
    1: 'Tree',
}

// 1004
const itemEffectTriggerType = {
    0: 'On Use',
    1: 'On Equip',
    2: 'On Proc',
    3: 'Summoned By Spell', // Only on 23442
    4: 'On Death',
    5: 'On Pickup',
    6: 'On Learn',
    7: 'On Looted',
    8: 'Teach Mount',
}

// 1033
const garrAutoSpellEffectType = {
    0: 'None', // NONE
    1: 'Damage', // DAMAGE
    2: 'Heal', // HEAL
    3: 'Damage %', // DAMAGE_PCT
    4: 'Heal %', // HEAL_PCT
    5: 'Damage Over Time', // DOT
    6: 'Heal Over Time', // HOT
    7: 'Damage % Over Time', // DOT_PCT
    8: 'Heal % Over Time', // HOT_PCT
    9: 'Taunt', // TAUNT
    10: 'Detaunt', // DETAUNT
    11: 'Mod Damage Done', // MOD_DAMAGE_DONE
    12: 'Mod Damage Done %', // MOD_DAMAGE_DONE_PCT (Always %, which is inconsitent with other *_PCT named values)
    13: 'Mod Damage Taken', // MOD_DAMAGE_TAKEN
    14: 'Mod Damage Taken %', // MOD_DAMAGE_TAKEN_PCT (Always %, which is inconsitent with other *_PCT named values)
    15: 'Deal Damage to Attacker', // DEAL_DAMAGE_TO_ATTACKER
    16: 'Deal Damage to Attacker %', // DEAL_DAMAGE_TO_ATTACKER_PCT
    17: 'Incease Max Health', // INCREASE_MAX_HEALTH
    18: 'Increase Max Health %', // INCREASE_MAX_HEALTH_PCT
    19: 'MOD_DAMAGE_DONE_PCT_OF_FLAT', // (Behaves as expected for MOD_DAMAGE_DONE_PCT, i.e. can be %, or % of attack)
    20: 'MOD_DAMAGE_TAKEN_PCT_OF_FLAT',  // (Behaves as expected for MOD_DAMAGE_TAKEN_PCT, i.e. can be %, or % of attack)
}

// 1034
const garrAutoSpellEffectTargetType = {
    0: 'None', // NONE
    1: 'Self', // SELF
    2: 'Adjacent Friendly', // ADJACENT_FRIENDLY
    3: 'Adjacent Hostile', // ADJACENT_HOSTILE
    4: 'Ranged Friendly', // RANGED_FRIENDLY
    5: 'Ranged Hostile', // RANGED_HOSTILE
    6: 'All Friendlies', // ALL_FRIENDLIES
    7: 'All Hostile', // ALL_HOSTILES
    8: 'All Adjacent Friendlies', // ALL_ADJACENT_FRIENDLIES
    9: 'All Adjacent Hostiles', // ALL_ADJACENT_HOSTILES
    10: 'Cone Friendlies', // CONE_FRIENDLIES
    11: 'Cone Hostiles', // CONE_HOSTILES
    12: 'Line Friendlies', // LINE_FRIENDLIES
    13: 'Line Hostiles', // LINE_HOSTILES
    14: 'All Front Row Friendlies', // ALL_FRONT_ROW_FRIENDLIES
    15: 'All Front Row Hostiles', // ALL_FRONT_ROW_HOSTILES
    16: 'All Back Row Friendlies', // ALL_BACK_ROW_FRIENDLIES
    17: 'All Back Row Hostiles', // ALL_BACK_ROW_HOSTILES
    18: 'All Targets', // ALL_TARGETS
    19: 'Random Target', // RANDOM_TARGET
    20: 'Random Ally', // RANDOM_ALLY (Actually targets random follower)
    21: 'Random Enemy', // RANDOM_ENEMY (Actually targets random encounter)
    22: 'ALL_FRIENDLIES_BUT_SELF',
    23: 'ALL_FOLLOWERS', // (Unofficial name)
    24: 'ALL_ENCOUNTERS', // (Unofficial name)
}

// 1049
const globalTable_PlayerConditionWhat = {
    0: 'LFD UI Available',
    1: 'LFR UI Available',
    2: 'Scenario Finder UI Available',
    3: 'PVP UI Available',
    4: 'Talent UI Available',
    5: 'Talent Spec UI Available',
    6: 'PVP Talent UI Available',
    7: 'Rated PVP UI Available',
    8: 'NPE Tutorial Complete',
    9: 'NPEv2 Tutorial Complete',
    10: 'Premade Group Finder Available',
    11: 'Group Finder UI Available',
}

// 1060
const uISplashScreenScreenType = {
    0: 'What\'s New',
    1: 'Season Rollover',
};

// 1063
const garrTalentTreeFeatureTypeIndex = {
    0: 'Generic',
    1: 'Sanctum - Anima Diversion',
    2: 'Sanctum - Travel Portals',
    3: 'Sanctum - Adventures',
    4: 'Sanctum - Reservoir Upgrades',
    5: 'Sanctum - Unique Feature',
    6: 'Sanctum - Soul Binds',
    7: 'Sanctum - Map - Anima Diversion',
    8: 'Zereth Mortis - Cyphers',
}

// 1064
const garrTalentTreeFeatureSubtypeIndex = {
    0: 'Generic',
    1: 'Sanctum - Bastion',
    2: 'Sanctum - Revendreth',
    3: 'Sanctum - Maldraxxus',
    4: 'Sanctum - Ardenweald',
}

// 1069
const chrCustomizationOptionType = {
    0: 'Selection Popout',
    1: 'Checkbox',
    2: 'Slider',
}

// 1083
const chrModelTextureLayerBlendMode = {
    0: 'None',
    1: 'Blit',
    2: 'Blit Alphamask',
    3: 'Add',
    4: 'Multiply',
    5: 'Mod2x',
    6: 'Overlay',
    7: 'Screen',
    8: 'Hardlight',
    9: 'Alpha Straight',
    10: 'Blend Black',
    11: 'Mask Greyscale',
    12: 'Mask Greyscale, Using Color As Alpha',
    13: 'Generate Greyscale',
    14: 'Colorize',
    15: 'Infer Alpha Blend',
}

// 1087
const uiWidgetSetLayoutDirection = {
    0: 'Vertical',
    1: 'Horizontal',
}

// 1094
const chrCustomizationReqOverrideArchive = {
    0: 'No CVar Restriction',
    1: 'CVar OverrideArchive=0',
    2: 'CVar OverrideArchive=1',
}

// 1095
const chatChannelsRuleset = {
    0: 'None',
    1: 'Mentor Chat',
}

// 1096
const garrTalentSocketType = {
    0: 'None',
    1: 'Spell',
    2: 'Soulbind Conduit',
}

// 1097
const soulbindConduitType = {
    0: 'Finesse',
    1: 'Potency',
    2: 'Endurance',
}

// 1099
const garrAutoCombatantRole = {
    0: 'None', // NONE
    1: 'Melee', // MELEE
    2: 'Ranged (Physical)', // RANGED_PHYSICAL
    3: 'Ranged (Magic)', // RANGED_MAGIC
    4: 'Heal/Support', // HEAL_SUPPORT
    5: 'Tank', // TANK
}

const enumNames = {
    2: 'In-game browser',
    3: 'Locales',
    // 5: 'OS-ish',
    105: 'Mount::Flags',
    119: 'Classes',
    120: 'Races',
    123: 'Region',
    130: 'Math operators',
    134: 'SpellMisc::Attributes',
    143: 'BattlePay Categories',
    150: 'BattlePay Products',
    151: 'ItemStaticFlags',
    155: 'Product/Branch',
    156: 'Expansions',
    169: 'ItemDisplayInfo::Flags',
    170: 'GarrBuilding::BuildingType',
    174: 'Real world currencies',
    176: 'HighlightColor::Type',
    178: 'SpellInterruptFlags',
    // 180: 'Chat Message Type',
    185: 'GroupFinderActivity::Flags',
    186: 'GroupFinderCategory::Flags',
    188: 'AreaPOI::Flags', // updated
    189: 'GroupFinderActivity::DisplayType',
    190: 'WorldMapArea::Flags',
    192: 'Map::Flags', // added
    193: 'ItemBonusList::Flags',
    194: 'Phase::Flags',
    195: 'SkillLine::Flags',
    196: 'CurrencyTypes::Flags',
    197: 'SummonProperties::Flags', // updated
    // 198: 'WMO Liquids',
    199: 'MountCapability::Flags', // added, needs double check as some dont make sense
    // 200: 'TaxiPath',
    201: 'EmoteFlags',
    203: 'WMOAreaTable::Flags',
    // 205: 'SpellVisual',
    220: 'ChatChannels::Flags',
    223: 'SpellShapeShiftForm::Flags',
    236: 'CreatureModelData::Flags',
    237: 'SummonProperties::Slot',
    238: 'SummonProperties::Control',
    // 244: 'Queue type',
    // 247: 'SpellVisualKit',
    251: 'BroadcastText::Flags',
    // 252: 'Consumable item types',
    255: 'CreatureImmunities::Flags',
    256: 'BattlemasterList::Flags',
    258: 'ArtifactPower::Flags',
    259: 'InventorySlotName',
    265: 'AnimationData::Flags',
    // 266: 'AnimationData::Flags2?',
    267: 'GarrFollowerType::Flags',
    // 268: 'DB col flags',
    // 269: 'DB col types',
    271: 'Criteria::Flags',
    272: 'SoundTypes',
    277: 'CreatureStaticFlags',
    282: 'QuestV2::Flags',
    291: 'LightSkybox::Flags',
    292: 'SpellInterrupts::InterruptFlags',
    // 299: 'CASC File flags?',
    303: 'BattlepayDisplayFlag',
    306: 'MovementFlags',
    318: 'LFGDungeons::Flags',
    326: 'FactionTemplate::Flags',
    327: 'PowerType::Flags',
    328: 'PowerType::PowerTypeEnum',
    // 330: 'visual swing type',
    332: 'ItemSubClass::Flags',
    336: 'InventoryTypeNames',
    338: 'Gender',
    345: 'SpellCategories::PreventionType',
    346: 'ChrClasses::Flags',
    351: 'SpellItemEnchantment::Flags',
    352: 'Scenario::Flags',
    353: 'Scenario::Type',
    // 357: 'Creature/m2 sound',
    // 358: 'Creature/m2 sound',
    360: 'ChrClasses::PrimaryStatPriority',
    365: 'Material::Flags',
    367: 'WeaponSwingSounds2::SwingType', // added
    372: 'TransmogSource',
    374: 'Sheating',
    // 377: 'Attachments',
    379: 'PrestigeLevelInfo::Flags',
    // 383: 'Numbers',
    384: 'GarrType::Flags',
    387: 'BattlePetSpecies::SourceTypeEnum',
    // 385: 'Light',
    392: 'GarrFollower::Quality',
    // 405: 'Letters',
    406: 'CharShipment::Flags',
    407: 'SpellAuraOptions::ProcTypeMask',
    423: 'GarrMission::Flags', // updated but conflicts, needs check
    424: 'GarrAbilityEffect.AbilityAction',
    436: 'SpellCategory::Flags',
    443: 'MapDifficulty::Flags',
    446: 'GarrAbility::Flags', // updated
    455: 'ItemContext', // Partial
    457: 'MapDifficulty::ResetInterval',
    458: 'GlobalStrings::Flags',
    465: 'Emotes::EmoteSpecProc',
    489: 'AreaTable::Flags', // updated
    497: 'ObjectEffectPackageElem::StateType', // copied
    // 522: 'Map/dungeon flags',
    530: 'CurveType',
    531: 'ItemSpec::Primary/SecondaryStat',
    // 538: 'Light data types',
    539: 'SPELL_PROC_TEST_RESULT', // Unknown ref?
    546: 'Stationery::Flags',
    555: 'UiModelSceneCamera::CameraType',
    556: 'UiModelSceneActor::Flags',
    559: 'UiModelScene::UiSystemType',
    558: 'Holidays::Flags',
    562: 'JournalInstance::Flags',
    564: 'GameObjects::TypeID',
    565: 'TransmogSet::Flags',
    566: 'TransmogSetItem::Flags',
    567: 'ExpectedStat::ExpansionID',
    573: 'CreatureDisplayInfo::Flags',
    585: 'ItemBonus::Type',
    586: 'SpellAuraNames::SpecialMiscValue',
    588: 'ScenarioEventEntry::TriggerType',
    590: 'UnitCondition::Variable',
    592: 'Criteria::Start_event',
    593: 'Criteria::Fail_event',
    594: 'Criteria::Type',
    // 597: 'Script related',
    // 599: 'SpellEffect::MiscValue[0] for Effect 86',
    600: 'ModifierTree::Type', // copied, fks mapped
    601: 'ItemSparse::Bonding',
    603: 'DungeonEncounter::Flags',
    604: 'SpellVisualEffectName::Type',
    // 605: 'Some M2 events',
    614: 'AI Formation Type',
    615: 'AI Formation Behavior',
    638: 'CriteriaTree::Operator',
    639: 'CriteriaTree::Flags', // updated
    651: 'RelicTalent::Type',
    659: 'PlayerChoice::Flags',
    671: 'ConfigurationWarning',
    673: 'CharComponentTextureSections:SectionType',
    675: 'CharBaseSection::VariationEnum',
    681: 'SpellEffect::EffectAttributes',
    // 689: 'the answer to the minimap question',
    692: 'Map::InstanceType', // updated
    693: 'ScenarioStep::Flags',
    697: 'BattlepayProductGroupFlag',
    706: 'Difficulty::Flags',
    709: 'CharacterServiceInfo::BoostType',
    712: 'AzeriteTier',
    713: 'CreatureClassifications',
    714: 'CreatureAges',
    715: 'CreatureRankings',
    717: 'UiMap::Flags', // updated
    718: 'UiMap::System', // updated
    719: 'UiMap::Type', // updated
    721: 'SpellScript::Arguments', // copied
    // 724: 'ScalingTypes for combat log',
    738: 'ChrRaces::Flags', // updated
    743: 'Weather::Type',
    747: 'ChrRaces::PlayableRaceBit',
    756: 'Campaign::Flags',
    758: 'ChallengeModeItemBonusOverride::Type',
    759: 'TaxiNodes::Flags',
    762: 'Prestige Name',
    863: 'UiWidgetVisualization::VisType', // copied and extended
    864: 'UiWidgetDataSource::SourceType', // copied
    865: 'UiWidgetVisTypeDataReq::ValueType', // copied
    875: 'Vignette::VignetteType',
    876: 'ContributionState',
    881: 'Item Reward Type',
    890: 'LiquidTypeXTexture::Type',
    891: 'GarrEncounter::Flags',
    // 901: 'SpellVisual categories',
    914: 'GeosetType',
    915: 'WaypointNode::Field_8_2_0_30080_005',
    // 916: 'Waypoint2',
    // 917: 'Waypoint3',
    934: 'FriendshipReputation::Flags',
    950: 'SceneFlags',
    953: 'ItemSubClass',
    956: 'BonusStatIndex',
    965: 'ItemEquipLoc',
    // 968: 'Account flags',
    // 969: 'MNAM',
    972: 'GarrisonBuildingDoodadSet::Type',
    973: 'GarrMissionSetFlags', // no fk?
    982: 'GarrPlot::PlotType',
    983: 'PlayerCondition::Flags',
    985: 'GarrSite::Flags',
    986: 'GarrPlot::Flags',
    989: 'GarrSpecialization::SpecType',
    991: 'GarrAbilityEffect::Flags',
    // 992: 'Editor stuff',
    993: 'GarrAbilityEffect::AbilityTargetType',
    994: 'GarrBuilding::Flags',
    995: 'GarrFollower::Flags',
    996: 'GarrFollowerRerollType', // no fk?
    997: 'CaptureBarWidgetFillDirectionType',
    999: 'GarrTalent::TalentType',
    1003: 'GarrTalent::ResearchCostSource',
    1004: 'ItemEffect::TriggerType', // updated
    1007: 'GossipIcon',
    1021: 'QuestObjective::Flags',
    1039: 'SchoolMask/DamageClass',
    // 1009: 'AreaPoi',
    1033: 'GarrAutoSpellEffect::Effect',
    1034: 'GarrAutoSpellEffect::TargetType',
    1035: 'GarrAutoSpell::Flags',
    1036: 'VehiclePOIType::Flags',
    1037: 'GarrAutoBoardIndexs', // no fk?
    1038: 'ChrModel::Flags', // copied
    1041: 'BattlePetState::Flags',
    1049: 'GlobalTable_PlayerCondition::What',
    1060: 'UISplashScreen::ScreenType',
    1063: 'GarrTalentTree::FeatureTypeIndex',
    1064: 'GarrTalentTree::FeatureSubtypeIndex',
    1067: 'ChrCustomizationCategoryFlag',
    1069: 'ChrCustomizationOptionType',
    1073: 'GarrTalentCost::CostType',
    1074: 'ScriptedAnimationBehavior',
    1075: 'ScriptedAnimationTrajectory',
    // 1078: 'ChrCustomizationMaterial::ChrModelTextureTargetID',
    1083: 'ChrModelTextureLayer::BlendMode',
    1085: 'UiWidgetVisualization::WidgetScale', // copied
    1086: 'StatusBarColorTintValue',
    1087: 'UiWidgetSet::LayoutDirection',
    1094: 'ChrCustomizationReq::OverrideArchive',
    1095: 'ChatChannels::Ruleset',
    1096: 'GarrTalentSocketType',
    1097: 'SoulbindConduit::ConduitType',
    1099: 'GarrAutoCombatant::Role',
    1103: 'OptionalReagentItemFlag',
    1129: 'BattlepayGroupDisplayType',
}

const official_enums = {
    ['AIBehaviorNodeCastSpell_FLAGS']: null,
    ['AIBehaviorNodeCombatCondition_FLAGS']: null,
    ['AIBehaviorNodeRepeat_FLAGS']: null,
    ['AIBehaviorNodeTimer_FLAGS']: null,
    ['AIBehaviorNodeUseResource_FLAGS']: null,
    ['AZERITE_ESSENCE_SLOT']: null,                    // enum 922: AzeriteEssence
    ['BEHAVIOR_ROOT_TYPE']: null,
    ['CHARACTER_RESTRICTION_TYPE']: null,
    ['g_bonusStatFields']: itemStatType,               // enum 956: BonusStatIndex
    ['GAMEOBJECT_ACTION']: null,
    ['GARR_FOLLOWER_QUALITY']: garrFollowerQuality,    // enum 392: GarrFollower::Quality
    ['GARRISON_BUILDING_TYPE']: garrBuildingType,      // enum 170: GarrBuilding::BuildingType
    ['INSTANCE_TYPE']: null,
    ['ITEM_BIND']: ItemBonding,                               // enum 601: ItemSparse::Bonding
    ['ITEM_QUALITY']: itemQuality,                     // enum 488: ItemQuality
    ['LFG_ROLE']: null,
    ['PVP_BRACKET']: null,
    ['PVP_TIER_ENUM']: null,
    ['SPECIAL_MISC_HONOR_GAIN_SOURCE']: null,
    ['TRANSMOG_SOURCE']: null,                         // enum 372: TransmogSource | ItemModifiedAppearance::TransmogSourceTypeEnum
};

const EventToastEventType = { // from uieventtoastmanagerdocumentation.lua 9.1.0.38312
    0: 'LevelUp',
    1: 'LevelUpSpell',
    2: 'LevelUpDungeon',
    3: 'LevelUpRaid',
    4: 'LevelUpPvP',
    5: 'PetBattleNewAbility',
    6: 'PetBattleFinalRound',
    7: 'PetBattleCapture',
    8: 'BattlePetLevelChanged',
    9: 'BattlePetLevelUpAbility',
    10: 'QuestBossEmote',
    11: 'MythicPlusWeeklyRecord',
    12: 'QuestTurnedIn',
    13: 'WorldStateChange',
    14: 'Scenario',
    15: 'LevelUpOther',
    16: 'PlayerAuraAdded',
    17: 'PlayerAuraRemoved',
    18: 'SpellScript',
};

const EventToastDisplayType = { // from uieventtoastmanagerdocumentation.lua 9.1.0.38312
    0: 'NormalSingleLine',
    1: 'NormalBlockText',
    2: 'NormalTitleAndSubTitle',
    3: 'NormalTextWithIcon',
    4: 'LargeTextWithIcon',
    5: 'NormalTextWithIconAndRarity',
    6: 'Scenario',
    7: 'ChallengeMode',
};

const BattlePetAbilityTurnType = {
    0: 'Normal',
    1: 'TriggeredEffect'
}

const BattlePetBreedQuality = {
    0: 'Poor',
    1: 'Common',
    2: 'Uncommon',
    3: 'Rare',
    4: 'Epic',
    5: 'Legendary',
}

const BattlePetEffectParamType = {
    0: 'Int',
    1: 'Ability'
}

const BattlePetEvent = {
    0: 'OnAuraApplied',
    1: 'OnDamageTaken',
    2: 'OnDamageDealt',
    3: 'OnHealTaken',
    4: 'OnHealDealt',
    5: 'OnAuraRemoved',
    6: 'OnRoundStart',
    7: 'OnRoundEnd',
    8: 'OnTurn',
    9: 'OnAbility',
    10: 'OnSwapIn',
    11: 'OnSwapOut',
    12: 'PostAuraTicks',
}

const BattlePetSources = {
    0: 'Drop',
    1: 'Quest',
    2: 'Vendor',
    3: 'Profession',
    4: 'WildPet',
    5: 'Achievement',
    6: 'WorldEvent',
    7: 'Promotion',
    8: 'Tcg',
    9: 'PetStore',
    10: 'Discovery',
}

const BattlePetTypes = {
    0: 'Humanoid',
    1: 'Dragonkin',
    2: 'Flying',
    3: 'Undead',
    4: 'Critter',
    5: 'Magic',
    6: 'Elemental',
    7: 'Beast',
    8: 'Aquatic',
    9: 'Mechanical',
}

const BattlePetVisualRange = {
    0: 'Melee',
    1: 'Ranged',
    2: 'InPlace',
    3: 'PointBlank',
    4: 'BehindMelee',
    5: 'BehindRanged',
}

const ChatProfanityLanguage = {
    0: 'English',
    1: 'Korean',
    2: 'French',
    3: 'German',
    4: 'Chinese',
    5: 'Unknown',
    6: 'Unknown',
    7: 'Unknown',
    8: 'Cyrillic',
    9: 'Portugese',
}

// Retrieved from TC SharedDefines: Targets
const target = {
    0 : "NONE",
    1 : "TARGET_UNIT_CASTER",
    2 : "TARGET_UNIT_NEARBY_ENEMY",
    3 : "TARGET_UNIT_NEARBY_PARTY",
    4 : "TARGET_UNIT_NEARBY_ALLY",
    5 : "TARGET_UNIT_PET",
    6 : "TARGET_UNIT_TARGET_ENEMY",
    7 : "TARGET_UNIT_SRC_AREA_ENTRY",
    8 : "TARGET_UNIT_DEST_AREA_ENTRY",
    9 : "TARGET_DEST_HOME",
    11 : "TARGET_UNIT_SRC_AREA_UNK_11",
    15 : "TARGET_UNIT_SRC_AREA_ENEMY",
    16 : "TARGET_UNIT_DEST_AREA_ENEMY",
    17 : "TARGET_DEST_DB",
    18 : "TARGET_DEST_CASTER",
    20 : "TARGET_UNIT_CASTER_AREA_PARTY",
    21 : "TARGET_UNIT_TARGET_ALLY",
    22 : "TARGET_SRC_CASTER",
    23 : "TARGET_GAMEOBJECT_TARGET",
    24 : "TARGET_UNIT_CONE_ENEMY_24",
    25 : "TARGET_UNIT_TARGET_ANY",
    26 : "TARGET_GAMEOBJECT_ITEM_TARGET",
    27 : "TARGET_UNIT_MASTER",
    28 : "TARGET_DEST_DYNOBJ_ENEMY",
    29 : "TARGET_DEST_DYNOBJ_ALLY",
    30 : "TARGET_UNIT_SRC_AREA_ALLY",
    31 : "TARGET_UNIT_DEST_AREA_ALLY",
    32 : "TARGET_DEST_CASTER_SUMMON",
    33 : "TARGET_UNIT_SRC_AREA_PARTY",
    34 : "TARGET_UNIT_DEST_AREA_PARTY",
    35 : "TARGET_UNIT_TARGET_PARTY",
    36 : "TARGET_DEST_CASTER_UNK_36",
    37 : "TARGET_UNIT_LASTTARGET_AREA_PARTY",
    38 : "TARGET_UNIT_NEARBY_ENTRY",
    39 : "TARGET_DEST_CASTER_FISHING",
    40 : "TARGET_GAMEOBJECT_NEARBY_ENTRY",
    41 : "TARGET_DEST_CASTER_FRONT_RIGHT",
    42 : "TARGET_DEST_CASTER_BACK_RIGHT",
    43 : "TARGET_DEST_CASTER_BACK_LEFT",
    44 : "TARGET_DEST_CASTER_FRONT_LEFT",
    45 : "TARGET_UNIT_TARGET_CHAINHEAL_ALLY",
    46 : "TARGET_DEST_NEARBY_ENTRY",
    47 : "TARGET_DEST_CASTER_FRONT",
    48 : "TARGET_DEST_CASTER_BACK",
    49 : "TARGET_DEST_CASTER_RIGHT",
    50 : "TARGET_DEST_CASTER_LEFT",
    51 : "TARGET_GAMEOBJECT_SRC_AREA",
    52 : "TARGET_GAMEOBJECT_DEST_AREA",
    53 : "TARGET_DEST_TARGET_ENEMY",
    54 : "TARGET_UNIT_CONE_180_DEG_ENEMY",
    55 : "TARGET_DEST_CASTER_FRONT_LEAP",
    56 : "TARGET_UNIT_CASTER_AREA_RAID",
    57 : "TARGET_UNIT_TARGET_RAID",
    58 : "TARGET_UNIT_NEARBY_RAID",
    59 : "TARGET_UNIT_CONE_ALLY",
    60 : "TARGET_UNIT_CONE_ENTRY",
    61 : "TARGET_UNIT_TARGET_AREA_RAID_CLASS",
    62 : "TARGET_DEST_CASTER_GROUND",
    63 : "TARGET_DEST_TARGET_ANY",
    64 : "TARGET_DEST_TARGET_FRONT",
    65 : "TARGET_DEST_TARGET_BACK",
    66 : "TARGET_DEST_TARGET_RIGHT",
    67 : "TARGET_DEST_TARGET_LEFT",
    68 : "TARGET_DEST_TARGET_FRONT_RIGHT",
    69 : "TARGET_DEST_TARGET_BACK_RIGHT",
    70 : "TARGET_DEST_TARGET_BACK_LEFT",
    71 : "TARGET_DEST_TARGET_FRONT_LEFT",
    72 : "TARGET_DEST_CASTER_RANDOM",
    73 : "TARGET_DEST_CASTER_RADIUS",
    74 : "TARGET_DEST_TARGET_RANDOM",
    75 : "TARGET_DEST_TARGET_RADIUS",
    76 : "TARGET_DEST_CHANNEL_TARGET",
    77 : "TARGET_UNIT_CHANNEL_TARGET",
    78 : "TARGET_DEST_DEST_FRONT",
    79 : "TARGET_DEST_DEST_BACK",
    80 : "TARGET_DEST_DEST_RIGHT",
    81 : "TARGET_DEST_DEST_LEFT",
    82 : "TARGET_DEST_DEST_FRONT_RIGHT",
    83 : "TARGET_DEST_DEST_BACK_RIGHT",
    84 : "TARGET_DEST_DEST_BACK_LEFT",
    85 : "TARGET_DEST_DEST_FRONT_LEFT",
    86 : "TARGET_DEST_DEST_RANDOM",
    87 : "TARGET_DEST_DEST",
    88 : "TARGET_DEST_DYNOBJ_NONE",
    89 : "TARGET_DEST_TRAJ",
    90 : "TARGET_UNIT_TARGET_MINIPET",
    91 : "TARGET_DEST_DEST_RADIUS",
    92 : "TARGET_UNIT_SUMMONER",
    93 : "TARGET_CORPSE_SRC_AREA_ENEMY",
    94 : "TARGET_UNIT_VEHICLE",
    95 : "TARGET_UNIT_TARGET_PASSENGER",
    96 : "TARGET_UNIT_PASSENGER_0",
    97 : "TARGET_UNIT_PASSENGER_1",
    98 : "TARGET_UNIT_PASSENGER_2",
    99 : "TARGET_UNIT_PASSENGER_3",
    100 : "TARGET_UNIT_PASSENGER_4",
    101 : "TARGET_UNIT_PASSENGER_5",
    102 : "TARGET_UNIT_PASSENGER_6",
    103 : "TARGET_UNIT_PASSENGER_7",
    104 : "TARGET_UNIT_CONE_CASTER_TO_DEST_ENEMY",
    105 : "TARGET_UNIT_CASTER_AND_PASSENGERS",
    106 : "TARGET_DEST_CHANNEL_CASTER",
    107 : "TARGET_DEST_NEARBY_ENTRY_2",
    108 : "TARGET_GAMEOBJECT_CONE_CASTER_TO_DEST_ENEMY",
    109 : "TARGET_GAMEOBJECT_CONE_CASTER_TO_DEST_ALLY",
    110 : "TARGET_UNIT_CONE_CASTER_TO_DEST_ENTRY",
    111 : "TARGET_UNK_111",
    112 : "TARGET_UNK_112",
    113 : "TARGET_UNK_113",
    114 : "TARGET_UNK_114",
    115 : "TARGET_UNIT_SRC_AREA_FURTHEST_ENEMY",
    116 : "TARGET_UNIT_AND_DEST_LAST_ENEMY",
    117 : "TARGET_UNK_117",
    118 : "TARGET_UNIT_TARGET_ALLY_OR_RAID",
    119 : "TARGET_CORPSE_SRC_AREA_RAID",
    120 : "TARGET_UNIT_CASTER_AND_SUMMONS",
    121 : "TARGET_CORPSE_TARGET_ALLY",
    122 : "TARGET_UNIT_AREA_THREAT_LIST",
    123 : "TARGET_UNIT_AREA_TAP_LIST",
    124 : "TARGET_UNIT_TARGET_TAP_LIST",
    125 : "TARGET_DEST_CASTER_GROUND_2",
    126 : "TARGET_UNIT_CASTER_AREA_ENEMY_CLUMP",
    127 : "TARGET_DEST_CASTER_ENEMY_CLUMP_CENTROID",
    128 : "TARGET_UNIT_RECT_CASTER_ALLY",
    129 : "TARGET_UNIT_RECT_CASTER_ENEMY",
    130 : "TARGET_UNIT_RECT_CASTER",
    131 : "TARGET_DEST_SUMMONER",
    132 : "TARGET_DEST_TARGET_ALLY",
    133 : "TARGET_UNIT_LINE_CASTER_TO_DEST_ALLY",
    134 : "TARGET_UNIT_LINE_CASTER_TO_DEST_ENEMY",
    135 : "TARGET_UNIT_LINE_CASTER_TO_DEST",
    136 : "TARGET_UNIT_CONE_CASTER_TO_DEST_ALLY",
    137 : "TARGET_DEST_CASTER_MOVEMENT_DIRECTION",
    138 : "TARGET_DEST_DEST_GROUND",
    139 : "TARGET_UNK_139",
    140 : "TARGET_DEST_CASTER_CLUMP_CENTROID",
    141 : "TARGET_UNK_141",
    142 : "TARGET_UNK_142",
    143 : "TARGET_UNK_143",
    144 : "TARGET_UNK_144",
    145 : "TARGET_UNK_145",
    146 : "TARGET_UNK_146",
    147 : "TARGET_UNK_147",
    148 : "TARGET_UNK_148",
    149 : "TARGET_UNK_149",
    150 : "TARGET_UNIT_OWN_CRITTER",
    151 : "TARGET_UNK_151"
}

// Non-ID enums
const traitNodeEntryType = {
    0: "SpendHex",
    1: "SpendSquare",
    2: "SpendCircle",
    3: "SpendSmallCircle",
    4: "DeprecatedSelect",
    5: "DragAndDrop",
    6: "SpendDiamond",
    7: "ProfPath",
    8: "ProfPerk",
    9: "ProfPathUnlock"
}

const traitNodeType = {
    0: "Single",
    1: "Tiered",
    2: "Selection"
}

const traitEdgeVisualStyle = {
    0: "None",
    1: "Straight"
}

const traitEdgeType = {
    0: "VisualOnly",
    1: "DeprecatedRankConnection",
    2: "SufficientForAvailability",
    3: "RequiredForAvailability",
    4: "MutuallyExclusive",
    5: "DeprecatedSelectionOption"
}

const traitCurrencyType = {
    0: "Gold",
    1: "CurrencyTypesBased",
    2: "TraitSourced"
}

const traitConditionType = {
    0: "Available",
    1: "Visible",
    2: "Granted",
    3: "Increased"
}

const traitCostDefinitionType = {
    0: "PrimaryCurrency",
    1: "SecondaryCurrency",
    2: "Item",
    3: "Gold"
}

const traitPointsOperationType = {
    "-1": "None",
    0: "Set",
    1: "Multiply"
}

const characterLoadoutPurpose = {
    0: "NEW_CHARACTER_PREVIEW",
    1: "BATTLEGROUND",
    2: "UPGRADE_WARLORDS",
    3: "UPGRADE_LEGION",
    4: "CLASS_SPEC_DEFAULT_LOADOUT",
    5: "NONE",
    6: "UPGRADE_BFA",
    7: "UPGRADE_SHADOWLANDS",
    8: "CHARACTERLOADOUT_NEW_CHARACTER_OUTFIT",
    9: "UPGRADE_TBC",
    10: "UPGRADE_DRAGONFLIGHT",
    11: "UPGRADE_WRATH",
    12: "RPE_RESET",
    13: "UPGRADE_DRAGONFLIGHT_70",
    14: "UPGRADE_LATEST_MAINLINE"
}

const chrModelTextureTarget = {
    0: "NONE",
    1: "SKIN_SLOT_0",
    2: "SKIN_SLOT_1",
    3: "SKIN_SLOT_2",
    4: "FACE_SLOT_0",
    5: "FACE_SLOT_1",
    6: "FACE_SLOT_2",
    7: "FACIALHAIR_SLOT_0",
    8: "FACIALHAIR_SLOT_1",
    9: "FACIALHAIR_SLOT_2",
    10: "HAIR_SLOT_0",
    11: "HAIR_SLOT_1",
    12: "HAIR_SLOT_2",
    13: "UNDERWEAR_SLOT_0",
    14: "UNDERWEAR_SLOT_1",
    15: "UNDERWEAR_SLOT_2",
    16: "CUSTOM_DISPLAY_OPTION_0_SLOT_0",
    17: "CUSTOM_DISPLAY_OPTION_0_SLOT_1",
    18: "CUSTOM_DISPLAY_OPTION_0_SLOT_2",
    19: "CUSTOM_DISPLAY_OPTION_1_SLOT_0",
    20: "CUSTOM_DISPLAY_OPTION_1_SLOT_1",
    21: "CUSTOM_DISPLAY_OPTION_1_SLOT_2",
    22: "CUSTOM_DISPLAY_OPTION_2_SLOT_0",
    23: "CUSTOM_DISPLAY_OPTION_2_SLOT_1",
    24: "CUSTOM_DISPLAY_OPTION_2_SLOT_2",
    25: "EYES",
    26: "MISC_JEWELRY",
    27: "PIERCINGS",
    28: "NECKLACES",
    29: "MAKEUP",
    30: "SKINTONE",
    31: "MISC_FEATURES",
    32: "SECONDARY_SKIN",
    33: "SECONDARY_UNDERWEAR_LOWER",
    34: "SECONDARY_UNDERWEAR_UPPER",
    35: "SECONDARY_HAIR",
    36: "EYE_GLOW_FACE_OVERLAY"
}

const contentRestrictionRuleTypes = {
    0:  "IN_PVP_MAP",
    1: "IN_TIMEWALKING",
    2: "IN_FULLY_EXPLORED_CONTINENT",
    3: "IN_LEGION_MAGE_TOWER",
    4: "IN_SOLO_SHUFFLE",
    5: "IN_MAP_FOR_EXPANSION",
    6: "IN_MYTHIC_PLUS_MAP",
    7: "IN_DUNGEON",
    8: "IN_RAID",
    9: "IN_ARENA",
    10: "IN_RANDOM_BATTLEGROUND",
    11: "IN_RATED_BATTLEGROUND",
    12: "IN_MAP_WITH_ACHIEVEMENT",
    13: "IN_AREA_WITH_ACHIEVEMENT",
    14: "IN_MAP",
    15: "IN_AREA_GROUP"
}

const craftingDataType = {
    0: "ITEM",
    1: "LOOT",
    2: "SCRAP",
    3: "ENCHANT"
}

const soundEmitterType = {
    0: 'SPHERE',
    1: 'SEGMENTED_PILL'
}

const lfgType = {
    0: 'NONE',
    1: 'DUNGEON',
    2: 'MANUAL RAID',
    3: 'QUEST',
    4: 'ZONE',
    5: 'HEROIC DUNGEON (deprecated)',
    6: 'RANDOM DUNGEON'
}

const lfgSubType = {
    0: 'NONE',
    1: 'DUNGEON',
    2: 'RAID',
    3: 'SCENARIO',
    4: 'FLEXRAID',
    5: 'WORLD PVP',
    6: 'BATTLEFIELD',
}

const profession = {
    0: 'First Aid',
    1: 'Blacksmithing',
    2: 'Leatherworking',
    3: 'Alchemy',
    4: 'Herbalism',
    5: 'Cooking',
    6: 'Mining',
    7: 'Tailoring',
    8: 'Engineering',
    9: 'Enchanting',
    10: 'Fishing',
    11: 'Skinning',
    12: 'Jewelcrafting',
    13: 'Inscription',
    14: 'Archaeology',
}

// Regular enums
let enumMap = new Map();
enumMap.set("animationdata.ID", animationNames);
enumMap.set("azeritetierunlock.Tier", azeriteTierID);
enumMap.set("battlepetability.PetTypeEnum", BattlePetTypes);
enumMap.set("battlepetabilityturn.EventTypeEnum", BattlePetEvent);
enumMap.set("battlepetabilityturn.TurnTypeEnum", BattlePetAbilityTurnType);
enumMap.set("battlepetbreedquality.QualityEnum", BattlePetBreedQuality);
enumMap.set("battlepetspecies.SourceTypeEnum", BattlePetSources);
enumMap.set("battlepetspecies.PetTypeEnum", BattlePetTypes);
enumMap.set("battlepetvisual.RangeTypeEnum", BattlePetVisualRange);
enumMap.set("challengemodeitembonusoverride.Type", challengeModeItemBonusOverrideType);
enumMap.set("charcomponenttexturesections.SectionType", componentSection);
enumMap.set("charhairgeosets.GeosetType", geosetType);
enumMap.set("charsectioncondition.BaseSection", charSectionType);
enumMap.set("charsectioncondition.Sex", charSex);
enumMap.set("charsections.BaseSection", charSectionType);
enumMap.set("charsections.SexID", charSex);
enumMap.set("chatchannels.Ruleset", chatChannelsRuleset);
enumMap.set("chrcustomization.ComponentSection[0]", componentSection);
enumMap.set("chrcustomization.ComponentSection[1]", componentSection);
enumMap.set("chrcustomization.ComponentSection[2]", componentSection);
enumMap.set("chrcustomization.UiCustomizationType", uiCustomizationType);
enumMap.set("chrcustomizationgeoset.GeosetType", geosetType);
enumMap.set("chrcustomizationoption.ChrModelID", tempChrModelIDEnum);
enumMap.set("chrcustomizationoption.OptionType", chrCustomizationOptionType);
enumMap.set("chrcustomizationreq.OverrideArchive", chrCustomizationReqOverrideArchive);
enumMap.set("chrcustomizationreq.ReqType", chrCustomizationReqType);
enumMap.set("chrcustomizationskinnedmodel.GeosetType", geosetType);
enumMap.set("chrmodel.BaseRaceChrModelID", tempChrModelIDEnum);
enumMap.set("chrmodelmaterial.SkinType", chrModelMaterialSkinType);
enumMap.set("chrmodelmaterial.TextureType", textureType);
enumMap.set("chrmodeltexturelayer.BlendMode", chrModelTextureLayerBlendMode);
enumMap.set("chrmodeltexturelayer.TextureType", textureType);
enumMap.set("chrmodeltexturelayer.ChrModelTextureTargetID[0]", chrModelTextureTarget);
enumMap.set("chrraces.PlayableRaceBit", chrRacesPlayableRaceBit);
enumMap.set("chrracexchrmodel.ChrModelID", tempChrModelIDEnum);
enumMap.set("craftingdata.Type", craftingDataType);
enumMap.set("criteria.Fail_event", criteriaFailEvent);
enumMap.set("criteria.Start_event", criteriaStartEvent);
enumMap.set("criteria.Type", criteriaType);
enumMap.set("criteriatree.Operator", criteriaTreeOperator);
enumMap.set("contentrestrictionrule.RuleType", contentRestrictionRuleTypes);
enumMap.set("curve.Type", curveType);
enumMap.set("difficulty.InstanceType", mapTypes);
enumMap.set("emotes.EmoteSpecProc", emoteSpecProc);
enumMap.set("enumeratedstring.EnumID", enumNames);
enumMap.set("environmentaldamage.EnumID", environmentalDamageType);
enumMap.set("expectedstat.ExpansionID", expectedStatExpansionID);
enumMap.set("garrabilityeffect.AbilityAction", garrAbilityAction);
enumMap.set("garrabilityeffect.AbilityTargetType", garrAbilityTargetType);
enumMap.set("garrautocombatant.Role", garrAutoCombatantRole);
enumMap.set("garrautospelleffect.Effect", garrAutoSpellEffectType);
enumMap.set("garrautospelleffect.TargetType", garrAutoSpellEffectTargetType);
enumMap.set("garrbuilding.BuildingType", garrBuildingType);
enumMap.set("garrfollitemsetmember.ItemSlot", garrFollowerItemSlot);
enumMap.set("garrfollowerquality.Quality", garrFollowerQuality);
enumMap.set("garrmechanictype.Category", garrMechanicCategory);
enumMap.set("garrplot.PlotType", garrPlotType);
enumMap.set("garrspecialization.BuildingType", garrBuildingType);
enumMap.set("garrspecialization.SpecType", garrSpecType);
enumMap.set("garrtalent.ResearchCostSource", garrTalentResearchCostSource);
enumMap.set("garrtalent.TalentType", garrTalentType);
enumMap.set("garrtalentcost.CostType", garrTalentCostType);
enumMap.set("garrtalentsocketproperties.GarrTalentSocketType", garrTalentSocketType);
enumMap.set("garrtalenttree.FeatureSubtypeIndex", garrTalentTreeFeatureSubtypeIndex);
enumMap.set("garrtalenttree.FeatureTypeIndex", garrTalentTreeFeatureTypeIndex);
enumMap.set("globalcurve.Type", globalCurveType);
enumMap.set("globaltable_playercondition.What", globalTable_PlayerConditionWhat);
enumMap.set("item.ClassID", itemClassEnum);
enumMap.set("item.InventoryType", inventoryTypeEnum);
enumMap.set("itembonus.Type", itemBonusTypes);
enumMap.set("itembonustreenode.ItemContext", itemContext);
enumMap.set("itemdisplayinfomaterialres.ComponentSection", componentSection);
enumMap.set("itemeffect.TriggerType", itemEffectTriggerType);
enumMap.set("itemmodifiedappearance.TransmogSourceTypeEnum", transmogSourceTypeEnum);
enumMap.set("itemsparse.InventoryType", inventoryTypeEnum);
enumMap.set("itemsparse.OverallQualityID", itemQuality);
enumMap.set("itemsparse.SocketType[0]", socketColorEnum);
enumMap.set("itemsparse.SocketType[1]", socketColorEnum);
enumMap.set("itemsparse.SocketType[2]", socketColorEnum);
enumMap.set("itemsparse.Bonding", ItemBonding);
enumMap.set("liquidtypextexture.Type", liquidTypeXTextureType);
enumMap.set("map.ExpansionID", expansionLevels);
enumMap.set("map.InstanceType", mapTypes);
enumMap.set("mapdifficulty.ItemContext", itemContext);
enumMap.set("mapdifficulty.ResetInterval", mapDifficultyResetInterval);
enumMap.set("mawpower.MawPowerRarityID", mawPowerRarity);
enumMap.set("modifiertree.Operator", modifierTreeOperator);
enumMap.set("modifiertree.Type", criteriaAdditionalCondition);
enumMap.set("npcmodelitemslotdisplayinfo.ItemSlot", itemSlot);
enumMap.set("objecteffectpackageelem.StateType", ObjectEffectPackageElem_StateType);
enumMap.set("playercondition.MinReputation[0]", reputationLevels);
enumMap.set("powertype.PowerTypeEnum", powerTypePowerTypeEnum);
enumMap.set("questinfo.Type", questTagType);
enumMap.set("questobjective.Type", questObjectiveType);
enumMap.set("relictalent.Type", relicTalentType);
enumMap.set("scenario.Type", scenarioType);
enumMap.set("scenarioevententry.TriggerType", scenarioEventEntryTriggerType);
enumMap.set("soulbindconduit.ConduitType", soulbindConduitType);
enumMap.set("soundemitters.EmitterType", soundEmitterType);
enumMap.set("soundentries.SoundType", soundkitSoundType);
enumMap.set("soundkit.SoundType", soundkitSoundType);
enumMap.set("spellclassoptions.SpellClassSet", spellClassSet);
enumMap.set("spelleffect.Effect", spellEffectName);
enumMap.set("spelleffect.EffectAura", effectAuraType);
enumMap.set("spelleffect.ImplicitTarget[0]", target);
enumMap.set("spelleffect.ImplicitTarget[1]", target);
enumMap.set("spellitemenchantment.Effect[0]", spellItemEnchantmentEffect);
enumMap.set("spellitemenchantment.Effect[1]", spellItemEnchantmentEffect);
enumMap.set("spellitemenchantment.Effect[2]", spellItemEnchantmentEffect);
enumMap.set("spelllabel.LabelID", spellLabelName);
enumMap.set("spellscript.Arguments", SpellScript_Arguments);
enumMap.set("spellvisualeffectname.Type", spellVisualEffectNames);
enumMap.set("spellvisualevent.EndEvent", spellVisualEventStartEvent);
enumMap.set("spellvisualevent.StartEvent", spellVisualEventStartEvent);
enumMap.set("spellvisualevent.TargetType", spellVisualEventTargetType);
enumMap.set("spellvisualkiteffect.EffectType", spellVisualKitEffectType);
enumMap.set("traitcostdefinition.DefinitionType", traitCostDefinitionType);
enumMap.set("traitcond.CondType", traitConditionType);
enumMap.set("traitcurrency.Type", traitCurrencyType);
enumMap.set("traitdefinitioneffectpoints.OperationType", traitPointsOperationType);
enumMap.set("traitedge.Type", traitEdgeType);
enumMap.set("traitedge.VisualStyle", traitEdgeVisualStyle);
enumMap.set("traitnode.Type", traitNodeType);
enumMap.set("traitnodeentry.NodeEntryType", traitNodeEntryType);
enumMap.set("summonproperties.Control", summonPropertiesControl);
enumMap.set("summonproperties.Slot", summonPropertiesSlot);
enumMap.set("uieventtoast.DisplayType", EventToastDisplayType);
enumMap.set("uieventtoast.EventType", EventToastEventType);
enumMap.set("uimap.System", uiMapSystem);
enumMap.set("uimap.Type", uiMapType);
enumMap.set("uimodelscenecamera.CameraType", uiModelSceneCameraCameraType);
enumMap.set("uisplashscreen.ScreenType", uISplashScreenScreenType);
enumMap.set("uiwidgetdatasource.SourceType", UiWidgetDataSource_SourceType);
enumMap.set("uiwidgetset.LayoutDirection", uiWidgetSetLayoutDirection);
enumMap.set("uiwidgetvistypedatareq.ValueType", UiWidgetVisTypeDataReq_ValueType);
enumMap.set("uiwidgetvistypedatareq.VisType", UiWidgetVisualization_VisType);
enumMap.set("uiwidgetvisualization.VisType", UiWidgetVisualization_VisType);
enumMap.set("uiwidgetvisualization.WidgetScale", uiWidgetScale);
enumMap.set("waypointnode.Field_8_2_0_30080_005", waypointNodeField_8_2_0_30080_005);
enumMap.set("weaponswingsounds2.SwingType", weaponSwingType);
enumMap.set("weather.Type", weatherType);
enumMap.set("highlightcolor.Type", highlightColorType);
enumMap.set("groupfinderactivity.DisplayType", groupFinderActivityDisplayType);
enumMap.set("chatprofanity.Language", ChatProfanityLanguage);
enumMap.set("characterloadout.Purpose", characterLoadoutPurpose);
enumMap.set("lfgdungeons.TypeID", lfgType);
enumMap.set("lfgdungeons.Subtype", lfgSubType);
enumMap.set("profession.ProfessionEnumValue", profession);

/* Race IDs */
enumMap.set("chrracexchrmodel.ChrRacesID", tempChrRaceIDEnum);
enumMap.set("charvariations.RaceID", tempChrRaceIDEnum);
enumMap.set("gluescreenemote.RaceID", tempChrRaceIDEnum);
enumMap.set("chrraceracialability.ChrRacesID", tempChrRaceIDEnum);
enumMap.set("chrcustomizationconversion.ChrRacesID", tempChrRaceIDEnum);
enumMap.set("soundcharactermacrolines.Race", tempChrRaceIDEnum);
enumMap.set("charstartkit.ChrRacesID", tempChrRaceIDEnum);
enumMap.set("helmetgeosetdata.RaceID", tempChrRaceIDEnum);
enumMap.set("characterfacialhairstyles.RaceID", tempChrRaceIDEnum);
enumMap.set("charbaseinfo.RaceID", tempChrRaceIDEnum);
enumMap.set("helmetanimscaling.RaceID", tempChrRaceIDEnum);
enumMap.set("uicamfbacktransmogchrrace.ChrRaceID", tempChrRaceIDEnum);
enumMap.set("chrcustomization.RaceID", tempChrRaceIDEnum);
enumMap.set("namegen.RaceID", tempChrRaceIDEnum);
enumMap.set("charstartoutfit.RaceID", tempChrRaceIDEnum);
enumMap.set("alliedrace.RaceID", tempChrRaceIDEnum);
enumMap.set("emotestextsound.RaceID", tempChrRaceIDEnum);
enumMap.set("charsections.RaceID", tempChrRaceIDEnum);
enumMap.set("barbershopstyle.Race", tempChrRaceIDEnum);
enumMap.set("creaturedisplayinfoextra.DisplayRaceID", tempChrRaceIDEnum);
enumMap.set("charhairgeosets.RaceID", tempChrRaceIDEnum);
enumMap.set("chrraces.UnalteredVisualRaceID", tempChrRaceIDEnum);
enumMap.set("chrraces.NeutralRaceID", tempChrRaceIDEnum);

let conditionalFKs = new Map();

for (let i = 0; i < 5; i++){
    enumMap.set("battlepeteffectproperties.ParamTypeEnum[" + i + "]", BattlePetEffectParamType);
}

for (let i = 0; i < 8; i++){
    enumMap.set("unitcondition.Variable[" + i + "]", unitConditionVariable);
    enumMap.set("unitcondition.Op[" + i + "]", unitConditionOperator);

    conditionalFKs.set("unitcondition.Value[" + i + "]",
        [
            ["unitcondition.Variable[" + i + "]=1", 'chrraces::ID'],
            ["unitcondition.Variable[" + i + "]=2", 'chrclasses::ID'],
            //["unitcondition.Variable[" + i + "]=3", '#Level'],
            //["unitcondition.Variable[" + i + "]=12", '#Health %'],
            //["unitcondition.Variable[" + i + "]=13", '#Mana %'],
            //["unitcondition.Variable[" + i + "]=14", '#Rage %'],
            //["unitcondition.Variable[" + i + "]=15", '#Energy %'],
            //["unitcondition.Variable[" + i + "]=16", '#Points'],
            ["unitcondition.Variable[" + i + "]=17", 'spell::ID'],
            ["unitcondition.Variable[" + i + "]=18", 'spelldispeltype::ID'],
            ["unitcondition.Variable[" + i + "]=19", 'spellmechanic::ID'],
            ["unitcondition.Variable[" + i + "]=20", 'spell::ID'],
            ["unitcondition.Variable[" + i + "]=21", 'spelldispeltype::ID'],
            ["unitcondition.Variable[" + i + "]=22", 'spellmechanic::ID'],
            ["unitcondition.Variable[" + i + "]=23", 'resistances::ID'],
            //["unitcondition.Variable[" + i + "]=24", '#Physical Damage %'],
            //["unitcondition.Variable[" + i + "]=25", '#Holy Damage %'],
            //["unitcondition.Variable[" + i + "]=26", '#Fire Damage %'],
            //["unitcondition.Variable[" + i + "]=27", '#Nature Damage %'],
            //["unitcondition.Variable[" + i + "]=28", '#Frost Damage %'],
            //["unitcondition.Variable[" + i + "]=29", '#Shadow Damage %'],
            //["unitcondition.Variable[" + i + "]=30", '#Arcane Damage %'],
            //["unitcondition.Variable[" + i + "]=37", '#Attackers'],
            //["unitcondition.Variable[" + i + "]=39", '#Yards'],
            //["unitcondition.Variable[" + i + "]=41", '#Seconds'],
            //["unitcondition.Variable[" + i + "]=44", '#Enemies'],
            //["unitcondition.Variable[" + i + "]=45", '#Friends'],
            //["unitcondition.Variable[" + i + "]=46", '#Physical Threat %'],
            //["unitcondition.Variable[" + i + "]=47", '#Holy Threat %'],
            //["unitcondition.Variable[" + i + "]=48", '#Fire Threat %'],
            //["unitcondition.Variable[" + i + "]=49", '#Nature Threat %'],
            //["unitcondition.Variable[" + i + "]=50", '#Frost Threat %'],
            //["unitcondition.Variable[" + i + "]=51", '#Shadow Threat %'],
            //["unitcondition.Variable[" + i + "]=52", '#Arcane Threat %'],
            //["unitcondition.Variable[" + i + "]=54", '#Attackers'],
            //["unitcondition.Variable[" + i + "]=55", '#Ranged Attackers'],
            ["unitcondition.Variable[" + i + "]=56", 'creaturetype::ID'],
            //["unitcondition.Variable[" + i + "]=59", '#HP'],
            ["unitcondition.Variable[" + i + "]=60", 'spell::ID'],
            //["unitcondition.Variable[" + i + "]=61", '#Spell Aura'],
            //["unitcondition.Variable[" + i + "]=64", '#Magic Damage %'],
            //["unitcondition.Variable[" + i + "]=65", '#Damage %'],
            //["unitcondition.Variable[" + i + "]=66", '#Magic Threat %'],
            //["unitcondition.Variable[" + i + "]=67", '#Threat %'],
            ["unitcondition.Variable[" + i + "]=74", 'creature::ID'],
            ["unitcondition.Variable[" + i + "]=75", 'stringid::ID'],
            ["unitcondition.Variable[" + i + "]=76", 'spell::ID'],
            //["unitcondition.Variable[" + i + "]=84", '#Path Fail Count'],
            ["unitcondition.Variable[" + i + "]=86", 'label::ID'],

        ]
    );
}

// Conditional enums
let conditionalEnums = new Map();
//['itembonus.Type=1', '#Item Level'],
//['itembonus.Type=6', '#Num Sockets'],
//['itembonus.Type=8', '#Required Level'],
//['itembonus.Type=9', '$Item Toast'],
//['itembonus.Type=10', '#Repair Cost Multiplier'],
//['itembonus.Type=18', '#Required Level'],
conditionalEnums.set("itembonus.Value[0]",
    [
        ['itembonus.Type=2', itemStatType], // @g_bonusStatFields
        ['itembonus.Type=3', itemQuality], // @ITEM_QUALITY
        ['itembonus.Type=16', ItemBonding], // @ITEM_BIND
        ['itembonus.Type=25', itemPrettyStatType],
    ]
);

conditionalEnums.set("itembonus.Value[1]",
    [
        ['itembonus.Type=6', socketColorEnum] // $Item Socket Type
    ]
);

conditionalEnums.set("item.SubclassID",
    [
        ['item.ClassID=0',  itemSubClass[0]],
        ['item.ClassID=1',  itemSubClass[1]],
        ['item.ClassID=2',  itemSubClass[2]],
        ['item.ClassID=3',  itemSubClass[3]],
        ['item.ClassID=4',  itemSubClass[4]],
        ['item.ClassID=5',  itemSubClass[5]],
        ['item.ClassID=6',  itemSubClass[6]],
        ['item.ClassID=7',  itemSubClass[7]],
        ['item.ClassID=8',  itemSubClass[8]],
        ['item.ClassID=9',  itemSubClass[9]],
        ['item.ClassID=10', itemSubClass[10]],
        ['item.ClassID=11', itemSubClass[11]],
        ['item.ClassID=12', itemSubClass[12]],
        ['item.ClassID=13', itemSubClass[13]],
        ['item.ClassID=14', itemSubClass[14]],
        ['item.ClassID=15', itemSubClass[15]],
        ['item.ClassID=16', itemSubClass[16]],
        ['item.ClassID=17', itemSubClass[17]],
        ['item.ClassID=18', itemSubClass[18]]
    ]
);

conditionalEnums.set("modifiertree.Asset",
    [
        ['modifiertree.Type=14', itemQuality],
        ['modifiertree.Type=15', itemQuality]
    ]
);

for (let i = 0; i < 3; i++){
    conditionalEnums.set("spellitemenchantment.EffectArg[" + i + "]",
        [
            ['spellitemenchantment.Effect[' + i + ']=5', itemStatType]
        ]
    );
}

// Conditional FKs (move to sep file?)
conditionalFKs.set("itembonus.Value[0]",
    [
        ['itembonus.Type=4','itemnamedescription::ID'],
        ['itembonus.Type=5','itemnamedescription::ID'],
        ['itembonus.Type=7','itemappearancemodifier::ID'],
        ['itembonus.Type=11','scalingstatdistribution::ID'],
        ['itembonus.Type=12','treasure::ID'],
        ['itembonus.Type=13','scalingstatdistribution::ID'],


        ['itembonus.Type=19','azeritetierunlockset::ID'],
        ['itembonus.Type=23','itemeffect::ID'],
        ['itembonus.Type=30','itemnamedescription::ID'],
        ['itembonus.Type=31','itemnamedescription::ID'],
        ['itembonus.Type=34','itembonuslistgroup::ID'],
        ['itembonus.Type=35','itemlimitcategory::ID'],
    ]
);

conditionalFKs.set("itembonus.Value[2]",
    [
        ['itembonus.Type=13','contenttuning::ID'],
    ]
);

conditionalFKs.set("itembonus.Value[3]",
    [
        ['itembonus.Type=13','curve::ID']
    ]
);

conditionalFKs.set("spelleffect.EffectMiscValue[0]",
    [
        ['spelleffect.EffectAura=56','creature::ID'],
        ['spelleffect.EffectAura=78','creature::ID'],
        ['spelleffect.EffectAura=260','screeneffect::ID'],
        ['spelleffect.EffectAura=307','spelllabel::LabelID'],
        ['spelleffect.Effect=16','questv2::ID'],
        ['spelleffect.Effect=28','creature::ID'],
        ['spelleffect.Effect=90','creature::ID'],
        ['spelleffect.Effect=131','soundkit::ID'],
        ['spelleffect.Effect=132','soundkit::ID'],
        ['spelleffect.Effect=134','creature::ID'],
        ['spelleffect.Effect=269','itembonuslistgroup::ID'],
        ['spelleffect.Effect=279','garrtalent::ID'],
    ]
);

conditionalFKs.set("spelleffect.EffectMiscValue[1]",
    [
        ['spelleffect.Effect=28','summonproperties::ID'],
    ]
);

conditionalFKs.set("criteria.Asset",
    [
        ['criteria.Type=0', 'creature::ID'],
        ['criteria.Type=1', 'map::ID'],
        ['criteria.Type=2', 'researchproject::ID'],
        ['criteria.Type=4', 'gameobjects::ID'],
        ['criteria.Type=7', 'skillline::ID'],
        ['criteria.Type=8', 'achievement::ID'],
        ['criteria.Type=11', 'areatable::ID'],
        ['criteria.Type=12', 'currencytypes::ID'],
        ['criteria.Type=15', 'map::ID'],
        ['criteria.Type=16', 'map::ID'],
        //['criteria.Type=18', '#Max Players'],
        //['criteria.Type=19', '#Max Players'],
        ['criteria.Type=20', 'creature::ID'],
        ['criteria.Type=21', 'criteria::ID'],
        //['criteria.Type=25', '#Challenge Mode Medal (OBSOLETE)'],
        //['criteria.Type=//26', '$Env Damage'],
        ['criteria.Type=27', 'questv2::ID'],
        ['criteria.Type=28', 'spell::ID'],
        ['criteria.Type=29', 'spell::ID'],
        ['criteria.Type=30', 'worldstateui::ID'],
        ['criteria.Type=31', 'areatable::ID'],
        ['criteria.Type=32', 'map::ID'],
        ['criteria.Type=33', 'map::ID'],
        ['criteria.Type=34', 'spell::ID'],
        ['criteria.Type=36', 'item::ID'],
        //['criteria.Type=38', '#Arena Rating'],
        //['criteria.Type=39', '#Arena Rating'],
        ['criteria.Type=40', 'skilline::ID'],
        ['criteria.Type=41', 'item::ID'],
        ['criteria.Type=42', 'item::ID'],
        ['criteria.Type=43', 'worldmapoverlay::ID'],
        ['criteria.Type=46', 'faction::ID'],
        //['criteria.Type=49': '$Equip Slot'],
        //['criteria.Type=50': '#Need Roll'],
        //['criteria.Type=51': '#Greed Roll'],
        ['criteria.Type=52', 'chrclasses::ID'],
        ['criteria.Type=53', 'chrraces::ID'],
        ['criteria.Type=54', 'emotestext::ID'],
        ['criteria.Type=57', 'item::ID'],
        ['criteria.Type=58', 'questsort::ID'],
        ['criteria.Type=64', 'spawnregion::ID'],
        ['criteria.Type=68', 'gameobjects::ID'],
        ['criteria.Type=69', 'spell::ID'],
        ['criteria.Type=71', 'map::ID'],
        ['criteria.Type=72', 'gameobjects::ID'],
        ['criteria.Type=73', 'gameevents::ID'],
        ['criteria.Type=75', 'skillline::ID'],
        ['criteria.Type=92', 'gameevents::ID'],
        ['criteria.Type=96', 'creature::ID'],
        ['criteria.Type=97', 'dungeonencounter::ID'],
        //['criteria.Type=109': '$Loot Acquisition'],
        ['criteria.Type=110', 'spell::ID'],
        ['criteria.Type=112', 'skillline::ID'],
        //['criteria.Type=116': '#Disenchant Roll'],
        ['criteria.Type=138', '{guild::IDChallenge}'],
        ['criteria.Type=152', 'scenario::ID'],
        ['criteria.Type=153', 'areatriggeractionset::ID'],
        ['criteria.Type=154', 'areatriggeractionset::ID'],
        //['criteria.Type=160': '#Level'],
        //['criteria.Type=162': '#Level}'],
        ['criteria.Type=163', 'areatable::ID'],
        ['criteria.Type=164', 'areatable::ID'],
        ['criteria.Type=165', 'dungeonencounter::ID'],
        ['criteria.Type=167', 'garrbuilding::ID'],
        ['criteria.Type=169', 'garrbuilding::ID'],
        //['criteria.Type=170': '#Tier:2,3'],
        ['criteria.Type=171', 'garrfollowertype::ID'],
        ['criteria.Type=172', 'garrmission::ID'],
        ['criteria.Type=173', 'garrfollowertype::ID'],
        ['criteria.Type=174', 'garrmission::ID'],
        ['criteria.Type=176', 'garrfollower::ID'],
        ['criteria.Type=179', 'garrbuilding::ID'],
        ['criteria.Type=181', 'garrspecialization::ID'],
        ['criteria.Type=182', 'charshipmentcontainer::ID'],
        ['criteria.Type=185', 'item::ID'],
        ['criteria.Type=188', 'item::ID'],
        ['criteria.Type=192', 'itemmodifiedappearance::ID'],
        //['criteria.Type=196': '#Level'],
        ['criteria.Type=198', 'garrtalent::ID'],
        //['criteria.Type=199': '$Equip Slot'],
        ['criteria.Type=202', 'garrtalent::ID'],
        ['criteria.Type=204', 'battlepaydeliverable::ID'],
        ['criteria.Type=205', 'transmogsetgroup::ID'],
        ['criteria.Type=206', 'faction::ID'],
        ['criteria.Type=208', 'creature::ID'],
        ['criteria.Type=209', 'artifactpower::ID'],
        ['criteria.Type=211', 'artifactpower::ID'],
        //['criteria.Type=212': '$Expansion Level'],
        ['criteria.Type=225', 'areatable::ID'],
    ]
);

conditionalFKs.set("criteria.Start_asset",
    [
        //['criteria.Start_event=1', '#Level'],
        ['criteria.Start_event=2', 'questv2::ID'],
        ['criteria.Start_event=3', 'map::ID'],
        //['criteria.Start_event=4', '#Team Size'],
        ['criteria.Start_event=5', 'spell::ID'],
        ['criteria.Start_event=6', 'spellauranames::EnumID'],
        ['criteria.Start_event=7', 'spell::ID'],
        ['criteria.Start_event=8', 'spell::ID'],
        ['criteria.Start_event=9', 'questv2::ID'],
        ['criteria.Start_event=10', 'creature::ID'],
        ['criteria.Start_event=12', 'item::ID'],
        ['criteria.Start_event=13', 'gameevents::ID'],
        //['criteria.Start_event=14', '#Step'],
    ]
);

conditionalFKs.set("criteria.Fail_asset",
    [
        //['criteria.Fail_event=4', '#Team Size'],
        ['criteria.Fail_event=5', 'spell::ID'],
        ['criteria.Fail_event=6', 'spell::ID'],
        ['criteria.Fail_event=7', 'spellauranames::EnumID'],
        ['criteria.Fail_event=8', 'spell::ID'],
        ['criteria.Fail_event=9', 'spell::ID'],
        ['criteria.Fail_event=14', 'gameevents::ID'],
    ]
);

conditionalFKs.set("spellvisualkiteffect.Effect",
    [
        ['spellvisualkiteffect.EffectType=1','spellproceduraleffect::ID'],
        ['spellvisualkiteffect.EffectType=2','spellvisualkitmodelattach::ID'],
        ['spellvisualkiteffect.EffectType=3','cameraeffect::ID'],
        ['spellvisualkiteffect.EffectType=4','cameraeffect::ID'],
        ['spellvisualkiteffect.EffectType=5','soundkit::ID'],
        ['spellvisualkiteffect.EffectType=6','spellvisualanim::ID'],
        ['spellvisualkiteffect.EffectType=7','shadowyeffect::ID'],
        ['spellvisualkiteffect.EffectType=8','spelleffectemission::ID'],
        ['spellvisualkiteffect.EffectType=9','outlineeffect::ID'],
        ['spellvisualkiteffect.EffectType=11','dissolveeffect::ID'],
        ['spellvisualkiteffect.EffectType=12','edgegloweffect::ID'],
        ['spellvisualkiteffect.EffectType=13','beameffect::ID'],
        ['spellvisualkiteffect.EffectType=14','clientsceneeffect::ID'],
        ['spellvisualkiteffect.EffectType=15','cloneeffect::ID'],
        ['spellvisualkiteffect.EffectType=16','gradienteffect::ID'],
        ['spellvisualkiteffect.EffectType=17','barrageeffect::ID'],
        ['spellvisualkiteffect.EffectType=18','ropeeffect::ID'],
        ['spellvisualkiteffect.EffectType=19','spellvisualscreeneffect::ID'],
    ]
);


conditionalFKs.set("modifiertree.TertiaryAsset",
    [
        ['modifiertree.Type=127','garrfollowertype::ID'],
        ['modifiertree.Type=128','garrfollowertype::ID'],
        ['modifiertree.Type=129','garrfollowertype::ID'],
        ['modifiertree.Type=130','garrfollowertype::ID'],
        ['modifiertree.Type=131','garrtype::ID'],
        ['modifiertree.Type=132','garrtype::ID'],
        ['modifiertree.Type=133','garrtype::ID'],
        ['modifiertree.Type=134','garrtype::ID'],
        ['modifiertree.Type=142','garrtype::ID'],
        ['modifiertree.Type=169','garrfollowertype::ID'],
        ['modifiertree.Type=175','garrfollowertype::ID'],
        ['modifiertree.Type=177','garrtype::ID'],
        ['modifiertree.Type=184','garrfollowertype::ID'],
        ['modifiertree.Type=186','garrtype::ID'],
    ]
);

conditionalFKs.set("modifiertree.SecondaryAsset",
    [
        //['modifiertree.Type=95', '#Reputation'],
        ['modifiertree.Type=96','itemsubclass::SubClassID'],
        //['modifiertree.Type=99', '#Skill Level'],
        //['modifiertree.Type=100','#Language Level'],
        //['modifiertree.Type=105','#Quantity'],
        //['modifiertree.Type=108','#Value'],
        //['modifiertree.Type=109','/End Date'],
        //['modifiertree.Type=114','#Quantity'],
        //['modifiertree.Type=117','#Value'],
        //['modifiertree.Type=118','#Value'],
        //['modifiertree.Type=119','#Amount'],
        //['modifiertree.Type=121','#Amount'],
        ['modifiertree.Type=126','garrtype::ID'],
        //['modifiertree.Type=127','#Level'],
        //['modifiertree.Type=128','@GARR_FOLLOWER_QUALITY'],
        ['modifiertree.Type=129','garrability::ID'],
        ['modifiertree.Type=130','garrability::ID'],
        // ['modifiertree.Type=131','@GARRISON_BUILDING_TYPE'],
        // ['modifiertree.Type=132','@GARRISON_BUILDING_TYPE'],
        // ['modifiertree.Type=133','@GARRISON_BUILDING_TYPE'],
        // ['modifiertree.Type=134','#Level'],
        ['modifiertree.Type=135','garrtype::ID'],
        //['modifiertree.Type=142','#Level'],
        ['modifiertree.Type=143','garrtype::ID'],
        ['modifiertree.Type=144','garrtype::ID'],
        ['modifiertree.Type=151','battlepetspecies::ID'],
        // ['modifiertree.Type=152','$Battle Pet Types'],
        // ['modifiertree.Type=158','#Value'],
        // ['modifiertree.Type=159','#Value'],
        ['modifiertree.Type=166','garrtype::ID'],
        // ['modifiertree.Type=169','#Level'],
        ['modifiertree.Type=170','garrtype::ID'],
        // ['modifiertree.Type=175','#Level'],
        ['modifiertree.Type=176','garrbuilding::ID'],
        // ['modifiertree.Type=177','#In-Progress'],
        ['modifiertree.Type=178','garrplot::ID'],
        // ['modifiertree.Type=184','#Level'],
        ['modifiertree.Type=186','garrmissionset::ID'],
        // ['modifiertree.Type=209','#Amount'],
        // ['modifiertree.Type=210','$Item History Spec Match'],
        ['modifiertree.Type=217','item::ID'],
        // ['modifiertree.Type=221','faction::ID'],
        // ['modifiertree.Type=222','$Item Quality'],
        ['modifiertree.Type=224','progressiveevent::ID'],
        ['modifiertree.Type=225','artifactpower::ID'],
        ['modifiertree.Type=231','achievement::ID'],
        // ['modifiertree.Type=239','@PVP_BRACKET'],
        ['modifiertree.Type=242','questline::ID'],
        ['modifiertree.Type=243','questline::ID'],
        ['modifiertree.Type=255','spell::ID'],
        ['modifiertree.Type=256','spell::ID'],
        ['modifiertree.Type=257','spell::ID'],
        ['modifiertree.Type=258','spell::ID'],
        // ['modifiertree.Type=259','{#rank}'],
        // ['modifiertree.Type=260','{#rank}'],
        // ['modifiertree.Type=261','{#rank}'],
        // ['modifiertree.Type=262','{#index}'],
        // ['modifiertree.Type=266','{#rank}'],
        // ['modifiertree.Type=267','{#rank}'],
        // ['modifiertree.Type=307','{#Rank}'],
        ['modifiertree.Type=308','spellshapeshiftform::ID'],
        // ['modifiertree.Type=309','{#Rank}'],
        ['modifiertree.Type=318','garrtalenttree::ID'],
        ['modifiertree.Type=329','displayseason::ID'],
    ]
);

conditionalFKs.set("modifiertree.Asset",
    [
        //['modifiertree.Type=1','#Drunkenness'],
        ['modifiertree.Type=2','playercondition::ID'],
        //['modifiertree.Type=3','#Item Level'],
        ['modifiertree.Type=4','creature::ID'],
        ['modifiertree.Type=8','spell::ID'],
        ['modifiertree.Type=9','spellauranames::EnumID'],
        ['modifiertree.Type=10','spell::ID'],
        ['modifiertree.Type=11','spellauranames::EnumID'],
        // ['modifiertree.Type=12', '$Aura State'],
        // ['modifiertree.Type=13', '$Aura State'],
        // ['modifiertree.Type=14', '$Item Quality'],
        // ['modifiertree.Type=15', '$Item Quality'],
        ['modifiertree.Type=17','areatable::ID'],
        ['modifiertree.Type=18','areatable::ID'],
        ['modifiertree.Type=19','item::ID'],
        //['modifiertree.Type=20', '$Dungeon Difficulty'],
        // ['modifiertree.Type=21', '#Level Delta'],
        // ['modifiertree.Type=22', '#Level Delta'],
        // ['modifiertree.Type=23', ''], // "Character restrictions" 
        // ['modifiertree.Type=24', '#Team Size'],
        ['modifiertree.Type=25','chrraces::ID'],
        ['modifiertree.Type=26','chrclasses::ID'],
        ['modifiertree.Type=27','chrraces::ID'],
        ['modifiertree.Type=28','chrclasses::ID'],
        //['modifiertree.Type=29', '#Tappers'],
        ['modifiertree.Type=30','creaturetype::ID'],
        ['modifiertree.Type=31','creaturefamily::ID'],
        ['modifiertree.Type=32','map::ID'],
        ['modifiertree.Type=33', 'wowstaticschemas::ID'],
        // ['modifiertree.Type=34', '#Battle Pet Level'],
        // ['modifiertree.Type=37', '#Personal Rating'],
        ['modifiertree.Type=38','chartitles::Mask_ID'],
        // ['modifiertree.Type=39', '#Level'],
        // ['modifiertree.Type=40', '#Level'],
        ['modifiertree.Type=41','areatable::ID'],
        ['modifiertree.Type=42','areatable::ID'],
        // ['modifiertree.Type=43', '#Percent'],
        // ['modifiertree.Type=44', '#Percent'],
        // ['modifiertree.Type=45', '#Percent'],
        // ['modifiertree.Type=46', '#Percent'],
        // ['modifiertree.Type=47', '#Percent'],
        // ['modifiertree.Type=48', '#Percent'],
        // ['modifiertree.Type=49', '#Hit Points'],
        // ['modifiertree.Type=50', '#Hit Points'],
        // ['modifiertree.Type=51', '#Hit Points'],
        // ['modifiertree.Type=52', '#Hit Points'],
        // ['modifiertree.Type=53', '#Hit Points'],
        // ['modifiertree.Type=54', '#Hit Points'],
        ['modifiertree.Type=55','playercondition::ID'],
        // ['modifiertree.Type=56', '#Achievement Pts'],
        // ['modifiertree.Type=62', '#Guild Reputation'],
        // ['modifiertree.Type=64', '#Battleground Rating'],
        //['modifiertree.Type=65', '$Project Rarity'],
        ['modifiertree.Type=66','researchbranch::ID'],
        ['modifiertree.Type=67','worldstateexpression::ID'],
        ['modifiertree.Type=68','difficulty::ID'],
        // ['modifiertree.Type=69', '#Level'],
        // ['modifiertree.Type=70', '#Level'],
        // ['modifiertree.Type=71', '#Level'],
        // ['modifiertree.Type=72', '#Level'],
        ['modifiertree.Type=73','modifiertree::ID'],
        ['modifiertree.Type=74','scenario::ID'],
        // ['modifiertree.Type=75', '#Reputation'],
        //['modifiertree.Type=76', '#Achievement Pts'],
        // ['modifiertree.Type=77', '#Pets Known'],
        // ['modifiertree.Type=78', '$Battle Pet Types'],
        // ['modifiertree.Type=79', '#Health Percent'],
        // ['modifiertree.Type=80', '#Members'],
        ['modifiertree.Type=81','creature::ID'],
        // ['modifiertree.Type=82', '#Step Number'],
        // ['modifiertree.Type=83', '#Challenge Mode Medal(OBSOLETE)'],
        ['modifiertree.Type=84','questv2::ID'],
        ['modifiertree.Type=85','faction::ID'],
        ['modifiertree.Type=86','achievement::ID'],
        ['modifiertree.Type=87','achievement::ID'],
        //['modifiertree.Type=88', '#Reputation'],
        ['modifiertree.Type=89','battlepetbreedquality::ID'],
        ['modifiertree.Type=91','battlepetspecies::ID'],
        // ['modifiertree.Type=92', '$Expansion Level'],
        ['modifiertree.Type=94','friendshiprepreaction::ID'],
        ['modifiertree.Type=95','faction::ID'],
        ['modifiertree.Type=96','itemclass::ClassID'],
        // ['modifiertree.Type=97', '$Gender'],
        // ['modifiertree.Type=98', '$Gender'],
        ['modifiertree.Type=99','skillline::ID'],
        ['modifiertree.Type=100','languages::ID'],
        ['modifiertree.Type=102','phase::ID'],
        ['modifiertree.Type=103','phasegroup::ID'],
        ['modifiertree.Type=104','spell::ID'],
        ['modifiertree.Type=105','item::ID'],
        //['modifiertree.Type=106','$Expansion Level'],
        ['modifiertree.Type=107','spelllabel::LabelID'], // ['modifiertree.Type=107','label::ID'],
        ['modifiertree.Type=108','worldstate::ID'],
        //['modifiertree.Type=109','/Begin Date'],
        ['modifiertree.Type=110','questv2::ID'],
        ['modifiertree.Type=111','questv2::ID'],
        ['modifiertree.Type=112','questobjective::ID'],
        ['modifiertree.Type=113','areatable::ID'],
        ['modifiertree.Type=114','item::ID'],
        ['modifiertree.Type=115','weather::ID'],
        //['modifiertree.Type=116','$Faction'],
        //['modifiertree.Type=117','$LFG Status'],
        //['modifiertree.Type=118','$LFG Status'],
        ['modifiertree.Type=119','currencytypes::ID'],
        //['modifiertree.Type=120','#Targets'],
        ['modifiertree.Type=121','currencytypes::ID'],
        //['modifiertree.Type=122','@INSTANCE_TYPE'],
        //['modifiertree.Type=125','#Season'],
        //['modifiertree.Type=126','#Tier'],
        //['modifiertree.Type=127','#Followers'],
        //['modifiertree.Type=128','#Followers'],
        // ['modifiertree.Type=129','#Level'],
        // ['modifiertree.Type=130','#Level'],
        ['modifiertree.Type=131','garrability::ID'],
        ['modifiertree.Type=132','garrability::ID'],
        //['modifiertree.Type=133','#Level'],
        //['modifiertree.Type=134','@GARRISON_BUILDING_TYPE'],
        ['modifiertree.Type=135','garrbuilding::ID'],
        ['modifiertree.Type=136','garrspecialization::ID'],
        ['modifiertree.Type=137','garrtype::ID'],
        ['modifiertree.Type=139','charshipmentcontainer::ID'],
        ['modifiertree.Type=140','garrbuilding::ID'],
        ['modifiertree.Type=141','garrmission::ID'],
        //['modifiertree.Type=142','@GARRISON_BUILDING_TYPE'],
        ['modifiertree.Type=143','garrability::ID'],
        ['modifiertree.Type=144','garrability::ID'],
        //['modifiertree.Type=145','@GARR_FOLLOWER_QUALITY'],
        //['modifiertree.Type=146','#Level'],
        //['modifiertree.Type=149','#Level'],
        ['modifiertree.Type=150','garrplotinstance::ID'],
        //['modifiertree.Type=151','#Amount'],
        //['modifiertree.Type=152','#Amount'],
        ['modifiertree.Type=153','battlepetability::ID'],
        //['modifiertree.Type=154','$Battle Pet Types'],
        //['modifiertree.Type=155','#Alive'],
        ['modifiertree.Type=156','garrspecialization::ID'],
        ['modifiertree.Type=157','garrfollower::ID'],
        ['modifiertree.Type=158','questobjective::ID'],
        ['modifiertree.Type=159','questobjective::ID'],
        ['modifiertree.Type=163','charshipmentcontainer::ID'],
        //['modifiertree.Type=165','#Players'],
        //['modifiertree.Type=166','#Level'],
        ['modifiertree.Type=167','garrmissiontype::ID'],
        //['modifiertree.Type=168','#Level'],
        //['modifiertree.Type=169','#Followers'],
        //['modifiertree.Type=170','#Tier'],
        //['modifiertree.Type=171','#Players'],
        ['modifiertree.Type=172','currencytypes::ID'],
        ['modifiertree.Type=174','questv2::ID'],
        // ['modifiertree.Type=175','#Followers'],
        ['modifiertree.Type=176','garrfollower::ID'],
        // ['modifiertree.Type=177','#Available'],
        // ['modifiertree.Type=178','#Amount'],
        // ['modifiertree.Type=179','$Currency Source'],
        ['modifiertree.Type=181','garrfollower::ID'],
        // ['modifiertree.Type=182','#Mod Value'],
        // ['modifiertree.Type=182','#Equals Value'],
        ['modifiertree.Type=183','mount::ID'],
        // ['modifiertree.Type=184','#Followers'],
        ['modifiertree.Type=185','garrfollower::ID'],
        // ['modifiertree.Type=186','#Missions'],
        ['modifiertree.Type=187','garrfollowertype::ID'],
        // ['modifiertree.Type=188','#Hours'],
        // ['modifiertree.Type=189','#Hours'],
        ['modifiertree.Type=191','chrraces::ID'],
        ['modifiertree.Type=192','chrraces::ID'],
        // ['modifiertree.Type=193','#Level'],
        // ['modifiertree.Type=194','#Level'],
        ['modifiertree.Type=195','garrmission::ID'],
        ['modifiertree.Type=197','item::ID'],
        // ['modifiertree.Type=198','#Points'],
        ['modifiertree.Type=199','item::ID'],
        ['modifiertree.Type=200','itemmodifiedappearance::ID'],
        ['modifiertree.Type=201','garrtalent::ID'],
        ['modifiertree.Type=202','garrtalent::ID'],
        // ['modifiertree.Type=203','@CHARACTER_RESTRICTION_TYPE'],
        // ['modifiertree.Type=204','#Hours'],
        // ['modifiertree.Type=205','#Hours'],
        ['modifiertree.Type=206','questinfo::ID'],
        ['modifiertree.Type=207','garrtalent::ID'],
        ['modifiertree.Type=208','artifactappearanceset::ID'],
        ['modifiertree.Type=209','currencytypes::ID'],
        // ['modifiertree.Type=210','#Item High Water Mark'],
        // ['modifiertree.Type=211','$Scenario Type'],
        // ['modifiertree.Type=212','$Expansion Level'],
        // ['modifiertree.Type=213','#Rating'],
        // ['modifiertree.Type=214','#Rating'],
        // ['modifiertree.Type=215','#Rating'],
        // ['modifiertree.Type=216','#Num Players'],
        // ['modifiertree.Type=217','#Num Traits'],
        // ['modifiertree.Type=218','#Level'],
        ['modifiertree.Type=219','charshipmentcontainer::ID'],
        // ['modifiertree.Type=221','#Level'],
        ['modifiertree.Type=222','itembonustree::ID'],
        // ['modifiertree.Type=223','#Number of empty slots'],
        ['modifiertree.Type=224','item::ID'],
        // ['modifiertree.Type=225','#Purchased Ranks'],
        // ['modifiertree.Type=231','#Group Members'],
        // ['modifiertree.Type=232','$Weapon Type'],
        // ['modifiertree.Type=233','$Weapon Type'],
        ['modifiertree.Type=234','pvptier::ID'],
        // ['modifiertree.Type=235','#Azerite Level'],
        ['modifiertree.Type=236','questline::ID'],
        ['modifiertree.Type=237','scheduledworldstategroup::ID'],
        // ['modifiertree.Type=239','@PVP_TIER_ENUM'],
        ['modifiertree.Type=240','questline::ID'],
        ['modifiertree.Type=241','questline::ID'],
        // ['modifiertree.Type=242','#Quests'],
        // ['modifiertree.Type=243','#Percentage'],
        // ['modifiertree.Type=247','#Level'],
        ['modifiertree.Type=249','mapchallengemode::ID'],
        // ['modifiertree.Type=250',#Season],
        // ['modifiertree.Type=251',#Season],
        ['modifiertree.Type=252','chrraces::ID'],
        ['modifiertree.Type=253','chrraces::ID'],
        ['modifiertree.Type=254','friendshiprepreaction::ID'],
        // ['modifiertree.Type=255','#Stacks'],
        // ['modifiertree.Type=256','#Stacks'],
        // ['modifiertree.Type=257','#Stacks'],
        // ['modifiertree.Type=258','#Stacks'],
        ['modifiertree.Type=259','azeriteessence::ID'],
        ['modifiertree.Type=260','azeriteessence::ID'],
        ['modifiertree.Type=261','azeriteessence::ID'],
        ['modifiertree.Type=262','spell::ID'],
        // ['modifiertree.Type=263','@LFG_ROLE'],
        // ['modifiertree.Type=265','@TRANSMOG_SOURCE'],
        // ['modifiertree.Type=266','@AZERITE_ESSENCE_SLOT'],
        // ['modifiertree.Type=267','@AZERITE_ESSENCE_SLOT'],
        ['modifiertree.Type=268','contenttuning::ID'],
        ['modifiertree.Type=269','contenttuning::ID'],
        ['modifiertree.Type=271','questv2::ID'],
        ['modifiertree.Type=272','contenttuning::ID'],
        ['modifiertree.Type=273','contenttuning::ID'],
        ['modifiertree.Type=274','levelrange::ID'],
        ['modifiertree.Type=275','levelrange::ID'],
        // ['modifiertree.Type=276','#Level'],
        ['modifiertree.Type=279','chrspecialization::ID'],
        ['modifiertree.Type=280','map::ID'],
        ['modifiertree.Type=282','battlepaydeliverable::ID'],
        // ['modifiertree.Type=285','@SPECIAL_MISC_HONOR_GAIN_SOURCE'],
        // ['modifiertree.Type=286','#Level'],
        // ['modifiertree.Type=287','#Level'],
        ['modifiertree.Type=288','covenant::ID'],
        ['modifiertree.Type=289','timeevent::ID'],
        ['modifiertree.Type=290','garrtalent::ID'],
        ['modifiertree.Type=291','soulbind::ID'],
        ['modifiertree.Type=292','spell::ID'],
        ['modifiertree.Type=298','areagroup::ID'],
        ['modifiertree.Type=299','areagroup::ID'],
        ['modifiertree.Type=300','uichromietimeexpansioninfo::ID'],
        ['modifiertree.Type=303','runeforgelegendaryability::ID'],
        ['modifiertree.Type=306','achievement::ID'],
        ['modifiertree.Type=307','soulbindconduit::ID'],
        // ['modifiertree.Type=308','creaturedisplayinfo::ID'],
        // ['modifiertree.Type=309','#Level'],
        ['modifiertree.Type=314','covenant::ID'],
        ['modifiertree.Type=317','garrtalent::ID'],
        ['modifiertree.Type=318','currencytypes::ID'],
        ['modifiertree.Type=327','item::ID'], //
    ]
);


conditionalFKs.set("spellvisualeffectname.GenericID",
    [
        ['spellvisualeffectname.Type=1', 'item::ID'],
        ['spellvisualeffectname.Type=2', 'creaturedisplayinfo::ID']
    ]
);

conditionalFKs.set("questobjective.ObjectID",
    [
        ['questobjective.Type=0', 'creature::ID'],
        ['questobjective.Type=1', 'item::ID'],
        ['questobjective.Type=2', 'gameobjects::ID'],
        ['questobjective.Type=3', 'creature::ID'],
        ['questobjective.Type=4', 'gameobjects::ID'],
        ['questobjective.Type=11', 'creature::ID'],
        ['questobjective.Type=12', 'battlepetspecies::ID'],
        ['questobjective.Type=14', 'criteriatree::ID'],
        ['questobjective.Type=17', 'currencytypes::ID'],
        ['questobjective.Type=19', 'areatrigger::ID'],
        ['questobjective.Type=20', 'areatrigger::ID'],
    ]
);

conditionalFKs.set("spellproceduraleffect.Value[0]",
    [
        ['spellproceduraleffect.Type=9', 'spellvisualkitareamodel::ID'],
        ['spellproceduraleffect.Type=26', 'spellchaineffects::ID'],
        ['spellproceduraleffect.Type=30', 'spellvisualcoloreffect::ID'],
    ]
);  

conditionalFKs.set("scenarioevententry.TriggerAsset",
    [
        ['scenarioevententry.TriggerType=2', 'criteriatree::ID'],
    ]
);


conditionalFKs.set("uieventtoast.EventAsset",
    [
        ['uieventtoast.EventType=12', 'questv2::ID'], // QuestTurnedIn
        ['uieventtoast.EventType=16', 'spell::ID'], // PlayerAuraAdded
        ['uieventtoast.EventType=17', 'spell::ID'], // PlayerAuraRemoved
    ]
);

conditionalFKs.set("spellkeyboundoverride.Data",
    [
        ['spellkeyboundoverride.Type=1', 'spell::ID'],
    ]
);


/* Colors */
let colorFields = new Array();
colorFields.push("chrcustomizationchoice.Color");
colorFields.push("chrcustomizationchoice.SwatchColor[0]");
colorFields.push("chrcustomizationchoice.SwatchColor[1]");
colorFields.push("lightdata.DirectColor");
colorFields.push("lightdata.AmbientColor");
colorFields.push("lightdata.SkyTopColor");
colorFields.push("lightdata.SkyMiddleColor");
colorFields.push("lightdata.SkyBand1Color");
colorFields.push("lightdata.SkyBand2Color");
colorFields.push("lightdata.SkySmogColor");
colorFields.push("lightdata.SkyFogColor");
colorFields.push("lightdata.SunColor");
colorFields.push("lightdata.CloudSunColor");
colorFields.push("lightdata.CloudEmissiveColor");
colorFields.push("lightdata.CloudLayer1AmbientColor");
colorFields.push("lightdata.CloudLayer2AmbientColor");
colorFields.push("lightdata.OceanCloseColor");
colorFields.push("lightdata.OceanFarColor");
colorFields.push("lightdata.RiverCloseColor");
colorFields.push("lightdata.RiverFarColor");
colorFields.push("lightdata.ShadowOpacity");
colorFields.push("weather.OverrideColor");
colorFields.push("highlightcolor.StartColor");
colorFields.push("highlightcolor.MidColor");
colorFields.push("highlightcolor.EndColor");
colorFields.push("lightning.FlashColor");
colorFields.push("lightning.BoltColor");
colorFields.push("liquidtype.MinimapStaticCol");
colorFields.push("itemnamedescription.Color");
// colorFields.push("uidungeonscorerarity.ScoreColor");

/* Dates */
let dateFields = new Array();

for (let i = 0; i < 26; i++){
    dateFields.push("holidays.Date[" + i + "]");
}
