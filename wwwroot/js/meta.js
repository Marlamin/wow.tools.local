window.flagMap = new Map();
window.enumMap = new Map();
window.colorFields = [];
window.dateFields = [];
window.conditionalFKs = new Map();
window.conditionalEnums = new Map();
window.conditionalFlags = new Map();

// Load the mappings if it hasn't been loaded yet
window.onload = _ => {
    fetch('/dbc/meta/getMappings')
        .then(res => res.json())
        .then(data => {
            data.forEach(entry => {
                let tableColumnKey = `${entry.tableName.toLowerCase()}.${entry.columnName}`;
                if (entry.arrIndex !== null) {
                    tableColumnKey += `[${entry.arrIndex}]`;
                }

                const isConditional = entry.conditionalTable && entry.conditionalColumn;
                if (entry.meta === 0) {         // Flags
                    if (isConditional) {
                        if (!window.conditionalFlags.has(tableColumnKey)) {
                            window.conditionalFlags.set(tableColumnKey, []);
                        }
                        const conditionKey = `${entry.conditionalTable.toLowerCase()}.${entry.conditionalColumn}=${entry.conditionalValue}`;
                        window.conditionalFlags.get(tableColumnKey).push([conditionKey, entry.entries]);
                    } else {
                        window.flagMap.set(tableColumnKey, entry.entries);
                    }

                } else if (entry.meta === 1) {  // Enum
                    if (isConditional) {
                        if (!window.conditionalEnums.has(tableColumnKey)) {
                            window.conditionalEnums.set(tableColumnKey, []);
                        }
                        const conditionKey = `${entry.conditionalTable.toLowerCase()}.${entry.conditionalColumn}=${entry.conditionalValue}`;
                        window.conditionalEnums.get(tableColumnKey).push([conditionKey, entry.entries]);
                    } else {
                        window.enumMap.set(tableColumnKey, entry.entries);
                    }
                } else if (entry.meta === 2) {  // Color
                    window.colorFields.push(tableColumnKey);
                } else if (entry.meta === 3) {  // Date
                    window.dateFields.push(tableColumnKey);
                }
            });
        });
};

function getEnum(db, field, value) {
    // eslint-disable-next-line no-undef
    let targetEnum = window.enumMap.get(db.toLowerCase() + '.' + field);
    return getEnumVal(targetEnum, value);
}

function getEnumVal(targetEnum, value) {
    if (targetEnum == null) {
        return "Unk";
    }

    const targetEnumEntry = targetEnum.find(item => item.value == value);
    if (targetEnumEntry != null) {
        if (targetEnumEntry.name == null) {
            return "Unk";
        } else {
            return `${targetEnumEntry.name}`;
        }

        // if (Array.isArray(targetEnum[value])){
        //     return targetEnum.at(value)[0].name;
        // } else {
        //     return targetEnum.at(value).name;
        // }
    } else {
        return "Unk";
    }
}

for (let i = 0; i < 8; i++) {
    window.conditionalFKs.set("unitcondition.Value[" + i + "]",
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

for (let i = 0; i < 3; i++) {
    window.set("spellitemenchantment.EffectArg[" + i + "]",
        [
            ['spellitemenchantment.Effect[' + i + ']=1', 'spell::id'],
            ['spellitemenchantment.Effect[' + i + ']=3', 'spell::id'],
            ['spellitemenchantment.Effect[' + i + ']=4', 'resistances::id'],
            ['spellitemenchantment.Effect[' + i + ']=7', 'spell::id'],
            ['spellitemenchantment.Effect[' + i + ']=11', 'itembonuslist::id'],
        ]
    );
}

// Conditional FKs (move to sep file?)
window.conditionalFKs.set("itembonus.Value[0]",
    [
        ['itembonus.Type=4', 'itemnamedescription::ID'],
        ['itembonus.Type=5', 'itemnamedescription::ID'],
        ['itembonus.Type=7', 'itemappearancemodifier::ID'],
        ['itembonus.Type=11', 'scalingstatdistribution::ID'],
        ['itembonus.Type=12', 'treasure::ID'],
        ['itembonus.Type=13', 'scalingstatdistribution::ID'],


        ['itembonus.Type=19', 'azeritetierunlockset::ID'],
        ['itembonus.Type=23', 'itemeffect::ID'],
        ['itembonus.Type=30', 'itemnamedescription::ID'],
        ['itembonus.Type=31', 'itemnamedescription::ID'],
        ['itembonus.Type=34', 'itembonuslistgroup::ID'],
        ['itembonus.Type=35', 'itemlimitcategory::ID'],
    ]
);

window.conditionalFKs.set("itembonus.Value[2]",
    [
        ['itembonus.Type=13', 'contenttuning::ID'],
    ]
);

window.conditionalFKs.set("itembonus.Value[3]",
    [
        ['itembonus.Type=13', 'curve::ID']
    ]
);

window.conditionalFKs.set("spelleffect.EffectMiscValue[0]",
    [
        ['spelleffect.EffectAura=56', 'creature::ID'],
        ['spelleffect.EffectAura=78', 'creature::ID'],
        ['spelleffect.EffectAura=260', 'screeneffect::ID'],
        ['spelleffect.EffectAura=307', 'spelllabel::LabelID'],
        ['spelleffect.Effect=16', 'questv2::ID'],
        ['spelleffect.Effect=28', 'creature::ID'],
        ['spelleffect.Effect=90', 'creature::ID'],
        ['spelleffect.Effect=131', 'soundkit::ID'],
        ['spelleffect.Effect=132', 'soundkit::ID'],
        ['spelleffect.Effect=134', 'creature::ID'],
        ['spelleffect.Effect=195', 'spell::ID'],
        ['spelleffect.Effect=269', 'itembonuslistgroup::ID'],
        ['spelleffect.Effect=279', 'garrtalent::ID'],
        ["spelleffect.Effect=324", "housedecor::ID"]
    ]
);

window.conditionalFKs.set("spelleffect.EffectMiscValue[1]",
    [
        ['spelleffect.Effect=28', 'summonproperties::ID'],
    ]
);

window.conditionalFKs.set("criteria.Asset",
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

window.conditionalFKs.set("criteria.Start_asset",
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

window.conditionalFKs.set("criteria.Fail_asset",
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

window.conditionalFKs.set("spellvisualkiteffect.Effect",
    [
        ['spellvisualkiteffect.EffectType=1', 'spellproceduraleffect::ID'],
        ['spellvisualkiteffect.EffectType=2', 'spellvisualkitmodelattach::ID'],
        ['spellvisualkiteffect.EffectType=3', 'cameraeffect::ID'],
        ['spellvisualkiteffect.EffectType=4', 'cameraeffect::ID'],
        ['spellvisualkiteffect.EffectType=5', 'soundkit::ID'],
        ['spellvisualkiteffect.EffectType=6', 'spellvisualanim::ID'],
        ['spellvisualkiteffect.EffectType=7', 'shadowyeffect::ID'],
        ['spellvisualkiteffect.EffectType=8', 'spelleffectemission::ID'],
        ['spellvisualkiteffect.EffectType=9', 'outlineeffect::ID'],
        ['spellvisualkiteffect.EffectType=11', 'dissolveeffect::ID'],
        ['spellvisualkiteffect.EffectType=12', 'edgegloweffect::ID'],
        ['spellvisualkiteffect.EffectType=13', 'beameffect::ID'],
        ['spellvisualkiteffect.EffectType=14', 'clientsceneeffect::ID'],
        ['spellvisualkiteffect.EffectType=15', 'cloneeffect::ID'],
        ['spellvisualkiteffect.EffectType=16', 'gradienteffect::ID'],
        ['spellvisualkiteffect.EffectType=17', 'barrageeffect::ID'],
        ['spellvisualkiteffect.EffectType=18', 'ropeeffect::ID'],
        ['spellvisualkiteffect.EffectType=19', 'spellvisualscreeneffect::ID'],
    ]
);


window.conditionalFKs.set("modifiertree.TertiaryAsset",
    [
        ['modifiertree.Type=127', 'garrfollowertype::ID'],
        ['modifiertree.Type=128', 'garrfollowertype::ID'],
        ['modifiertree.Type=129', 'garrfollowertype::ID'],
        ['modifiertree.Type=130', 'garrfollowertype::ID'],
        ['modifiertree.Type=131', 'garrtype::ID'],
        ['modifiertree.Type=132', 'garrtype::ID'],
        ['modifiertree.Type=133', 'garrtype::ID'],
        ['modifiertree.Type=134', 'garrtype::ID'],
        ['modifiertree.Type=142', 'garrtype::ID'],
        ['modifiertree.Type=169', 'garrfollowertype::ID'],
        ['modifiertree.Type=175', 'garrfollowertype::ID'],
        ['modifiertree.Type=177', 'garrtype::ID'],
        ['modifiertree.Type=184', 'garrfollowertype::ID'],
        ['modifiertree.Type=186', 'garrtype::ID'],
    ]
);

window.conditionalFKs.set("modifiertree.SecondaryAsset",
    [
        //['modifiertree.Type=95', '#Reputation'],
        ['modifiertree.Type=96', 'itemsubclass::SubClassID'],
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
        ['modifiertree.Type=126', 'garrtype::ID'],
        //['modifiertree.Type=127','#Level'],
        //['modifiertree.Type=128','@GARR_FOLLOWER_QUALITY'],
        ['modifiertree.Type=129', 'garrability::ID'],
        ['modifiertree.Type=130', 'garrability::ID'],
        // ['modifiertree.Type=131','@GARRISON_BUILDING_TYPE'],
        // ['modifiertree.Type=132','@GARRISON_BUILDING_TYPE'],
        // ['modifiertree.Type=133','@GARRISON_BUILDING_TYPE'],
        // ['modifiertree.Type=134','#Level'],
        ['modifiertree.Type=135', 'garrtype::ID'],
        //['modifiertree.Type=142','#Level'],
        ['modifiertree.Type=143', 'garrtype::ID'],
        ['modifiertree.Type=144', 'garrtype::ID'],
        ['modifiertree.Type=151', 'battlepetspecies::ID'],
        // ['modifiertree.Type=152','$Battle Pet Types'],
        // ['modifiertree.Type=158','#Value'],
        // ['modifiertree.Type=159','#Value'],
        ['modifiertree.Type=166', 'garrtype::ID'],
        // ['modifiertree.Type=169','#Level'],
        ['modifiertree.Type=170', 'garrtype::ID'],
        // ['modifiertree.Type=175','#Level'],
        ['modifiertree.Type=176', 'garrbuilding::ID'],
        // ['modifiertree.Type=177','#In-Progress'],
        ['modifiertree.Type=178', 'garrplot::ID'],
        // ['modifiertree.Type=184','#Level'],
        ['modifiertree.Type=186', 'garrmissionset::ID'],
        // ['modifiertree.Type=209','#Amount'],
        // ['modifiertree.Type=210','$Item History Spec Match'],
        ['modifiertree.Type=217', 'item::ID'],
        // ['modifiertree.Type=221','faction::ID'],
        // ['modifiertree.Type=222','$Item Quality'],
        ['modifiertree.Type=224', 'progressiveevent::ID'],
        ['modifiertree.Type=225', 'artifactpower::ID'],
        ['modifiertree.Type=231', 'achievement::ID'],
        // ['modifiertree.Type=239','@PVP_BRACKET'],
        ['modifiertree.Type=242', 'questline::ID'],
        ['modifiertree.Type=243', 'questline::ID'],
        ['modifiertree.Type=255', 'spell::ID'],
        ['modifiertree.Type=256', 'spell::ID'],
        ['modifiertree.Type=257', 'spell::ID'],
        ['modifiertree.Type=258', 'spell::ID'],
        // ['modifiertree.Type=259','{#rank}'],
        // ['modifiertree.Type=260','{#rank}'],
        // ['modifiertree.Type=261','{#rank}'],
        // ['modifiertree.Type=262','{#index}'],
        // ['modifiertree.Type=266','{#rank}'],
        // ['modifiertree.Type=267','{#rank}'],
        // ['modifiertree.Type=307','{#Rank}'],
        ['modifiertree.Type=308', 'spellshapeshiftform::ID'],
        // ['modifiertree.Type=309','{#Rank}'],
        ['modifiertree.Type=318', 'garrtalenttree::ID'],
        ['modifiertree.Type=329', 'displayseason::ID'],
    ]
);

window.conditionalFKs.set("modifiertree.Asset",
    [
        //['modifiertree.Type=1','#Drunkenness'],
        ['modifiertree.Type=2', 'playercondition::ID'],
        //['modifiertree.Type=3','#Item Level'],
        ['modifiertree.Type=4', 'creature::ID'],
        ['modifiertree.Type=8', 'spell::ID'],
        ['modifiertree.Type=9', 'spellauranames::EnumID'],
        ['modifiertree.Type=10', 'spell::ID'],
        ['modifiertree.Type=11', 'spellauranames::EnumID'],
        // ['modifiertree.Type=12', '$Aura State'],
        // ['modifiertree.Type=13', '$Aura State'],
        // ['modifiertree.Type=14', '$Item Quality'],
        // ['modifiertree.Type=15', '$Item Quality'],
        ['modifiertree.Type=17', 'areatable::ID'],
        ['modifiertree.Type=18', 'areatable::ID'],
        ['modifiertree.Type=19', 'item::ID'],
        //['modifiertree.Type=20', '$Dungeon Difficulty'],
        // ['modifiertree.Type=21', '#Level Delta'],
        // ['modifiertree.Type=22', '#Level Delta'],
        // ['modifiertree.Type=23', ''], // "Character restrictions" 
        // ['modifiertree.Type=24', '#Team Size'],
        ['modifiertree.Type=25', 'chrraces::ID'],
        ['modifiertree.Type=26', 'chrclasses::ID'],
        ['modifiertree.Type=27', 'chrraces::ID'],
        ['modifiertree.Type=28', 'chrclasses::ID'],
        //['modifiertree.Type=29', '#Tappers'],
        ['modifiertree.Type=30', 'creaturetype::ID'],
        ['modifiertree.Type=31', 'creaturefamily::ID'],
        ['modifiertree.Type=32', 'map::ID'],
        ['modifiertree.Type=33', 'wowstaticschemas::ID'],
        // ['modifiertree.Type=34', '#Battle Pet Level'],
        // ['modifiertree.Type=37', '#Personal Rating'],
        ['modifiertree.Type=38', 'chartitles::Mask_ID'],
        // ['modifiertree.Type=39', '#Level'],
        // ['modifiertree.Type=40', '#Level'],
        ['modifiertree.Type=41', 'areatable::ID'],
        ['modifiertree.Type=42', 'areatable::ID'],
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
        ['modifiertree.Type=55', 'playercondition::ID'],
        // ['modifiertree.Type=56', '#Achievement Pts'],
        // ['modifiertree.Type=62', '#Guild Reputation'],
        // ['modifiertree.Type=64', '#Battleground Rating'],
        //['modifiertree.Type=65', '$Project Rarity'],
        ['modifiertree.Type=66', 'researchbranch::ID'],
        ['modifiertree.Type=67', 'worldstateexpression::ID'],
        ['modifiertree.Type=68', 'difficulty::ID'],
        // ['modifiertree.Type=69', '#Level'],
        // ['modifiertree.Type=70', '#Level'],
        // ['modifiertree.Type=71', '#Level'],
        // ['modifiertree.Type=72', '#Level'],
        ['modifiertree.Type=73', 'modifiertree::ID'],
        ['modifiertree.Type=74', 'scenario::ID'],
        // ['modifiertree.Type=75', '#Reputation'],
        //['modifiertree.Type=76', '#Achievement Pts'],
        // ['modifiertree.Type=77', '#Pets Known'],
        // ['modifiertree.Type=78', '$Battle Pet Types'],
        // ['modifiertree.Type=79', '#Health Percent'],
        // ['modifiertree.Type=80', '#Members'],
        ['modifiertree.Type=81', 'creature::ID'],
        // ['modifiertree.Type=82', '#Step Number'],
        // ['modifiertree.Type=83', '#Challenge Mode Medal(OBSOLETE)'],
        ['modifiertree.Type=84', 'questv2::ID'],
        ['modifiertree.Type=85', 'faction::ID'],
        ['modifiertree.Type=86', 'achievement::ID'],
        ['modifiertree.Type=87', 'achievement::ID'],
        //['modifiertree.Type=88', '#Reputation'],
        ['modifiertree.Type=89', 'battlepetbreedquality::ID'],
        ['modifiertree.Type=91', 'battlepetspecies::ID'],
        // ['modifiertree.Type=92', '$Expansion Level'],
        ['modifiertree.Type=94', 'friendshiprepreaction::ID'],
        ['modifiertree.Type=95', 'faction::ID'],
        ['modifiertree.Type=96', 'itemclass::ClassID'],
        // ['modifiertree.Type=97', '$Gender'],
        // ['modifiertree.Type=98', '$Gender'],
        ['modifiertree.Type=99', 'skillline::ID'],
        ['modifiertree.Type=100', 'languages::ID'],
        ['modifiertree.Type=102', 'phase::ID'],
        ['modifiertree.Type=103', 'phasegroup::ID'],
        ['modifiertree.Type=104', 'spell::ID'],
        ['modifiertree.Type=105', 'item::ID'],
        //['modifiertree.Type=106','$Expansion Level'],
        ['modifiertree.Type=107', 'spelllabel::LabelID'], // ['modifiertree.Type=107','label::ID'],
        ['modifiertree.Type=108', 'worldstate::ID'],
        //['modifiertree.Type=109','/Begin Date'],
        ['modifiertree.Type=110', 'questv2::ID'],
        ['modifiertree.Type=111', 'questv2::ID'],
        ['modifiertree.Type=112', 'questobjective::ID'],
        ['modifiertree.Type=113', 'areatable::ID'],
        ['modifiertree.Type=114', 'item::ID'],
        ['modifiertree.Type=115', 'weather::ID'],
        //['modifiertree.Type=116','$Faction'],
        //['modifiertree.Type=117','$LFG Status'],
        //['modifiertree.Type=118','$LFG Status'],
        ['modifiertree.Type=119', 'currencytypes::ID'],
        //['modifiertree.Type=120','#Targets'],
        ['modifiertree.Type=121', 'currencytypes::ID'],
        //['modifiertree.Type=122','@INSTANCE_TYPE'],
        //['modifiertree.Type=125','#Season'],
        //['modifiertree.Type=126','#Tier'],
        //['modifiertree.Type=127','#Followers'],
        //['modifiertree.Type=128','#Followers'],
        // ['modifiertree.Type=129','#Level'],
        // ['modifiertree.Type=130','#Level'],
        ['modifiertree.Type=131', 'garrability::ID'],
        ['modifiertree.Type=132', 'garrability::ID'],
        //['modifiertree.Type=133','#Level'],
        //['modifiertree.Type=134','@GARRISON_BUILDING_TYPE'],
        ['modifiertree.Type=135', 'garrbuilding::ID'],
        ['modifiertree.Type=136', 'garrspecialization::ID'],
        ['modifiertree.Type=137', 'garrtype::ID'],
        ['modifiertree.Type=139', 'charshipmentcontainer::ID'],
        ['modifiertree.Type=140', 'garrbuilding::ID'],
        ['modifiertree.Type=141', 'garrmission::ID'],
        //['modifiertree.Type=142','@GARRISON_BUILDING_TYPE'],
        ['modifiertree.Type=143', 'garrability::ID'],
        ['modifiertree.Type=144', 'garrability::ID'],
        //['modifiertree.Type=145','@GARR_FOLLOWER_QUALITY'],
        //['modifiertree.Type=146','#Level'],
        //['modifiertree.Type=149','#Level'],
        ['modifiertree.Type=150', 'garrplotinstance::ID'],
        //['modifiertree.Type=151','#Amount'],
        //['modifiertree.Type=152','#Amount'],
        ['modifiertree.Type=153', 'battlepetability::ID'],
        //['modifiertree.Type=154','$Battle Pet Types'],
        //['modifiertree.Type=155','#Alive'],
        ['modifiertree.Type=156', 'garrspecialization::ID'],
        ['modifiertree.Type=157', 'garrfollower::ID'],
        ['modifiertree.Type=158', 'questobjective::ID'],
        ['modifiertree.Type=159', 'questobjective::ID'],
        ['modifiertree.Type=163', 'charshipmentcontainer::ID'],
        //['modifiertree.Type=165','#Players'],
        //['modifiertree.Type=166','#Level'],
        ['modifiertree.Type=167', 'garrmissiontype::ID'],
        //['modifiertree.Type=168','#Level'],
        //['modifiertree.Type=169','#Followers'],
        //['modifiertree.Type=170','#Tier'],
        //['modifiertree.Type=171','#Players'],
        ['modifiertree.Type=172', 'currencytypes::ID'],
        ['modifiertree.Type=174', 'questv2::ID'],
        // ['modifiertree.Type=175','#Followers'],
        ['modifiertree.Type=176', 'garrfollower::ID'],
        // ['modifiertree.Type=177','#Available'],
        // ['modifiertree.Type=178','#Amount'],
        // ['modifiertree.Type=179','$Currency Source'],
        ['modifiertree.Type=181', 'garrfollower::ID'],
        // ['modifiertree.Type=182','#Mod Value'],
        // ['modifiertree.Type=182','#Equals Value'],
        ['modifiertree.Type=183', 'mount::ID'],
        // ['modifiertree.Type=184','#Followers'],
        ['modifiertree.Type=185', 'garrfollower::ID'],
        // ['modifiertree.Type=186','#Missions'],
        ['modifiertree.Type=187', 'garrfollowertype::ID'],
        // ['modifiertree.Type=188','#Hours'],
        // ['modifiertree.Type=189','#Hours'],
        ['modifiertree.Type=191', 'chrraces::ID'],
        ['modifiertree.Type=192', 'chrraces::ID'],
        // ['modifiertree.Type=193','#Level'],
        // ['modifiertree.Type=194','#Level'],
        ['modifiertree.Type=195', 'garrmission::ID'],
        ['modifiertree.Type=197', 'item::ID'],
        // ['modifiertree.Type=198','#Points'],
        ['modifiertree.Type=199', 'item::ID'],
        ['modifiertree.Type=200', 'itemmodifiedappearance::ID'],
        ['modifiertree.Type=201', 'garrtalent::ID'],
        ['modifiertree.Type=202', 'garrtalent::ID'],
        // ['modifiertree.Type=203','@CHARACTER_RESTRICTION_TYPE'],
        // ['modifiertree.Type=204','#Hours'],
        // ['modifiertree.Type=205','#Hours'],
        ['modifiertree.Type=206', 'questinfo::ID'],
        ['modifiertree.Type=207', 'garrtalent::ID'],
        ['modifiertree.Type=208', 'artifactappearanceset::ID'],
        ['modifiertree.Type=209', 'currencytypes::ID'],
        // ['modifiertree.Type=210','#Item High Water Mark'],
        // ['modifiertree.Type=211','$Scenario Type'],
        // ['modifiertree.Type=212','$Expansion Level'],
        // ['modifiertree.Type=213','#Rating'],
        // ['modifiertree.Type=214','#Rating'],
        // ['modifiertree.Type=215','#Rating'],
        // ['modifiertree.Type=216','#Num Players'],
        // ['modifiertree.Type=217','#Num Traits'],
        // ['modifiertree.Type=218','#Level'],
        ['modifiertree.Type=219', 'charshipmentcontainer::ID'],
        // ['modifiertree.Type=221','#Level'],
        ['modifiertree.Type=222', 'itembonustree::ID'],
        // ['modifiertree.Type=223','#Number of empty slots'],
        ['modifiertree.Type=224', 'item::ID'],
        // ['modifiertree.Type=225','#Purchased Ranks'],
        // ['modifiertree.Type=231','#Group Members'],
        // ['modifiertree.Type=232','$Weapon Type'],
        // ['modifiertree.Type=233','$Weapon Type'],
        ['modifiertree.Type=234', 'pvptier::ID'],
        // ['modifiertree.Type=235','#Azerite Level'],
        ['modifiertree.Type=236', 'questline::ID'],
        ['modifiertree.Type=237', 'scheduledworldstategroup::ID'],
        // ['modifiertree.Type=239','@PVP_TIER_ENUM'],
        ['modifiertree.Type=240', 'questline::ID'],
        ['modifiertree.Type=241', 'questline::ID'],
        // ['modifiertree.Type=242','#Quests'],
        // ['modifiertree.Type=243','#Percentage'],
        // ['modifiertree.Type=247','#Level'],
        ['modifiertree.Type=249', 'mapchallengemode::ID'],
        // ['modifiertree.Type=250',#Season],
        // ['modifiertree.Type=251',#Season],
        ['modifiertree.Type=252', 'chrraces::ID'],
        ['modifiertree.Type=253', 'chrraces::ID'],
        ['modifiertree.Type=254', 'friendshiprepreaction::ID'],
        // ['modifiertree.Type=255','#Stacks'],
        // ['modifiertree.Type=256','#Stacks'],
        // ['modifiertree.Type=257','#Stacks'],
        // ['modifiertree.Type=258','#Stacks'],
        ['modifiertree.Type=259', 'azeriteessence::ID'],
        ['modifiertree.Type=260', 'azeriteessence::ID'],
        ['modifiertree.Type=261', 'azeriteessence::ID'],
        ['modifiertree.Type=262', 'spell::ID'],
        // ['modifiertree.Type=263','@LFG_ROLE'],
        // ['modifiertree.Type=265','@TRANSMOG_SOURCE'],
        // ['modifiertree.Type=266','@AZERITE_ESSENCE_SLOT'],
        // ['modifiertree.Type=267','@AZERITE_ESSENCE_SLOT'],
        ['modifiertree.Type=268', 'contenttuning::ID'],
        ['modifiertree.Type=269', 'contenttuning::ID'],
        ['modifiertree.Type=271', 'questv2::ID'],
        ['modifiertree.Type=272', 'contenttuning::ID'],
        ['modifiertree.Type=273', 'contenttuning::ID'],
        ['modifiertree.Type=274', 'levelrange::ID'],
        ['modifiertree.Type=275', 'levelrange::ID'],
        // ['modifiertree.Type=276','#Level'],
        ['modifiertree.Type=279', 'chrspecialization::ID'],
        ['modifiertree.Type=280', 'map::ID'],
        ['modifiertree.Type=282', 'battlepaydeliverable::ID'],
        // ['modifiertree.Type=285','@SPECIAL_MISC_HONOR_GAIN_SOURCE'],
        // ['modifiertree.Type=286','#Level'],
        // ['modifiertree.Type=287','#Level'],
        ['modifiertree.Type=288', 'covenant::ID'],
        ['modifiertree.Type=289', 'timeevent::ID'],
        ['modifiertree.Type=290', 'garrtalent::ID'],
        ['modifiertree.Type=291', 'soulbind::ID'],
        ['modifiertree.Type=292', 'spell::ID'],
        ['modifiertree.Type=298', 'areagroup::ID'],
        ['modifiertree.Type=299', 'areagroup::ID'],
        ['modifiertree.Type=300', 'uichromietimeexpansioninfo::ID'],
        ['modifiertree.Type=303', 'runeforgelegendaryability::ID'],
        ['modifiertree.Type=306', 'achievement::ID'],
        ['modifiertree.Type=307', 'soulbindconduit::ID'],
        // ['modifiertree.Type=308','creaturedisplayinfo::ID'],
        // ['modifiertree.Type=309','#Level'],
        ['modifiertree.Type=314', 'covenant::ID'],
        ['modifiertree.Type=317', 'garrtalent::ID'],
        ['modifiertree.Type=318', 'currencytypes::ID'],
        ['modifiertree.Type=327', 'item::ID'], //
    ]
);


window.conditionalFKs.set("spellvisualeffectname.GenericID",
    [
        ['spellvisualeffectname.Type=1', 'item::ID'],
        ['spellvisualeffectname.Type=2', 'creaturedisplayinfo::ID']
    ]
);

window.conditionalFKs.set("questobjective.ObjectID",
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

window.conditionalFKs.set("spellproceduraleffect.Value[0]",
    [
        ['spellproceduraleffect.Type=9', 'spellvisualkitareamodel::ID'],
        ['spellproceduraleffect.Type=26', 'spellchaineffects::ID'],
        ['spellproceduraleffect.Type=30', 'spellvisualcoloreffect::ID'],
    ]
);

window.conditionalFKs.set("scenarioevententry.TriggerAsset",
    [
        ['scenarioevententry.TriggerType=2', 'criteriatree::ID'],
    ]
);


window.conditionalFKs.set("uieventtoast.EventAsset",
    [
        ['uieventtoast.EventType=12', 'questv2::ID'], // QuestTurnedIn
        ['uieventtoast.EventType=16', 'spell::ID'], // PlayerAuraAdded
        ['uieventtoast.EventType=17', 'spell::ID'], // PlayerAuraRemoved
    ]
);

window.conditionalFKs.set("spellkeyboundoverride.Data",
    [
        ['spellkeyboundoverride.Type=1', 'spell::ID'],
    ]
);

/* Dates */
for (let i = 0; i < 26; i++) {
    window.dateFields.push("holidays.Date[" + i + "]");
}
