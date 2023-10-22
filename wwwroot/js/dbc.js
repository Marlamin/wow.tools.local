function makeBuild(text){
    if (text == null){
        return "";
    }

    let rawdesc = text.replace("WOW-", "");
    const build  = rawdesc.substring(0, 5);

    rawdesc = rawdesc.replace(build, "").replace("patch", "");
    const descexpl = rawdesc.split("_");

    return descexpl[0] + "." + build;
}

function getFKCols(headers, fks){
    let fkCols = [];
    headers.forEach(function(header, index){
        Object.keys(fks).forEach(function(key) {
            if (key == header){
                fkCols[index] = fks[key];
            }
        });
    });
    return fkCols;
}

function openFKModal(value, location, build){
    const wowDBMap = new Map();
    wowDBMap.set("spell", "https://www.wowdb.com/spells/");
    wowDBMap.set("item", "https://www.wowdb.com/items/");
    wowDBMap.set("itemsparse", "https://www.wowdb.com/items/");
    wowDBMap.set("questv2", "https://www.wowdb.com/quests/");
    wowDBMap.set("creature", "https://www.wowdb.com/npcs/");
    wowDBMap.set("gameobjects", "https://www.wowdb.com/objects/");

    const wowheadMap = new Map();
    wowheadMap.set("spell", "https://www.wowhead.com/spell=");
    wowheadMap.set("item", "https://www.wowhead.com/item=");
    wowheadMap.set("itemsparse", "https://www.wowhead.com/item=");
    wowheadMap.set("questv2", "https://www.wowhead.com/quest=");
    wowheadMap.set("creature", "https://www.wowhead.com/npc=");
    wowheadMap.set("gameobjects", "https://www.wowhead.com/object=");

    const splitLocation = location.split("::");
    const db = splitLocation[0].toLowerCase();
    const col = splitLocation[1];
    const fkModal = document.getElementById("fkModalContent");

    fkModal.innerHTML = "<b>Lookup into table " + db + " on col '" + col + "' value '" + value + "'</b><br>";

    if (wowDBMap.has(db)){
        fkModal.innerHTML += " <a target='_BLANK' href='" + wowDBMap.get(db) + value + "' class='btn btn-warning btn-sm'>View on WoWDB</a>";
    }

    if (wowheadMap.has(db)){
        fkModal.innerHTML += " <a target='_BLANK' href='" + wowheadMap.get(db) + value + "' class='btn btn-warning btn-sm'>View on Wowhead</a>";
    }

    fkModal.innerHTML += "<table id='fktable' class='table table-condensed table-striped'><thead><tr><th style='width: 300px'>Column</th><th>Value</th></tr></thead></table>";

    const fkTable = document.getElementById("fktable");
    if (db == "spell" && col == "ID"){
        fetch("/dbc/peek/spellname?build=" + build + "&col=ID&val=" + value)
            .then(function (response) {
                return response.json();
            }).then(function (json) {
                if (json.values["Name_lang"] !== undefined){
                    fkTable.insertAdjacentHTML("beforeend", "<tr><td>Name <small>(from SpellName)</small></td><td>" + json.values["Name_lang"] + "</td></tr>");
                }
            });
    }

    Promise.all([
        fetch("/dbc/header/" + db + "?build=" + build),
        fetch("/dbc/peek/" + db + "?build=" + build + "&col=" + col + "&val=" + value)
    ])
        .then(async function (responses) {
            try {
                return Promise.all(responses.map(function (response) {
                    return response.json();
                }));
            } catch (error) {
                console.log(error);
                fkTable.insertAdjacentHTML("beforeend", "<tr><td colspan='2'>This row is not available in clients or an error occurred.</td></tr>");
            }
        }).then(function (data) {
            const headerjson = data[0];
            const json = data[1];

            if (!json || Object.keys(json.values).length == 0){
                fkTable.insertAdjacentHTML("beforeend", "<tr><td colspan='2'>This row is not available in clients, is a hotfix or is serverside-only.</td></tr>");
                return;
            }

            let fkTableHTML = "";
            Object.keys(json.values).forEach(function (key) {
                const val = json.values[key];
                if (key in headerjson.fks){
                    fkTableHTML += "<tr><td style='width: 300px;'>" + key + "</td>";

                    if (headerjson.fks[key] == "FileData::ID"){
                        fkTableHTML += "<td><a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' data-toggle='modal' data-target='#moreInfoModal' onclick='fillModal(" + val + ")'>" + val + "</a>";
                    } else if (headerjson.fks[key] == "SoundEntries::ID" && parseInt(build[0]) > 6){
                        fkTableHTML += "<td><a style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' onclick='openFKModal(" + val + ", \"SoundKit::ID\", \"" + build + "\")'>" + val + "</a>";
                    } else if (headerjson.fks[key] == "Item::ID" && val > 0){
                        fkTableHTML += "<td><a data-build='" + build + "' data-tooltip='item' data-id='" + val + "' style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' onclick='openFKModal(" + val + ", \"" + headerjson.fks[key] + "\", \"" + build + "\")'>" + val + "</a>";
                    } else if (headerjson.fks[key] == "Spell::ID" || headerjson.fks[key] == "SpellName::ID" && val > 0){
                        fkTableHTML += "<td><a data-build='" + build + "' data-tooltip='spell' data-id='" + val + "' style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' onclick='openFKModal(" + val + ", \"" + headerjson.fks[key] + "\", \"" + build + "\")'>" + val + "</a>";
                    } else {
                        fkTableHTML += "<td><a data-build='" + build + "' data-tooltip='fk' data-id='" + val + "' data-fk='" + headerjson.fks[key] + "' style='padding-top: 0px; padding-bottom: 0px; cursor: pointer; border-bottom: 1px dotted;' onclick='openFKModal(" + val + ", \"" + headerjson.fks[key] + "\", \"" + build + "\")'>" + val + "</a>";
                    }

                    var cleanDBname = headerjson.fks[key].split('::')[0].toLowerCase();

                    if (wowDBMap.has(cleanDBname) && val != 0){
                        fkTableHTML += " <a target='_BLANK' href='" + wowDBMap.get(cleanDBname) + val + "' class='btn btn-warning btn-sm'>View on WoWDB</a>";
                    }

                    if (wowheadMap.has(cleanDBname) && val != 0){
                        fkTableHTML += " <a target='_BLANK' href='" + wowheadMap.get(cleanDBname) + val + "' class='btn btn-warning btn-sm'>View on Wowhead</a>";
                    }
                } else {
                    fkTableHTML += "<tr><td style='width: 300px;'>" + key + "</td><td>" + val;
                }


                const columnWithTable = db.toLowerCase() + "." + key;

                if (enumMap.has(columnWithTable)) {
                    var enumVal = getEnum(db.toLowerCase(), key, val);
                    if (val == '0' && enumVal == "Unk") {
                        // returnVar += val;
                    } else {
                        fkTableHTML += " <i>(" + enumVal + ")</i>";
                    }
                }

                if (conditionalEnums.has(columnWithTable)) {
                    let conditionalEnum = conditionalEnums.get(columnWithTable);
                    conditionalEnum.forEach(function(conditionalEnumEntry) {
                        let condition = conditionalEnumEntry[0].split('=');
                        let conditionTarget = condition[0].split('.');
                        let conditionValue = condition[1];
                        let resultEnum = conditionalEnumEntry[1];

                        let colTarget = headerjson["headers"].indexOf(conditionTarget[1]);

                        // Col target found?
                        if (colTarget > -1) {
                            if (json.values[colTarget] == conditionValue) {
                                var enumVal = getEnumVal(resultEnum, val);
                                if (val == '0' && enumVal == "Unk") {
                                    //
                                } else {
                                    fkTableHTML +=" <i>(" + enumVal + ")</i>";
                                }
                            }
                        }
                    });
                }

                fkTableHTML += "</td></tr>";
            });

            fkTable.insertAdjacentHTML("beforeend", fkTableHTML);

            fkModal.insertAdjacentHTML("beforeend", " <a target=\"_BLANK\" href=\"/dbc/?dbc=" + db.replace(".db2", "") + "&build=" + build + "#page=1&colFilter[" + headerjson.headers.indexOf(col) + "]=exact:" + value + "\" class=\"btn btn-primary\">Go to record</a>");
        }).catch(function (error) {
            console.log(error);
            fkTable.insertAdjacentHTML("beforeend", "<tr><td colspan='2'>This row is not available in clients or an error occurred.</td></tr>");
        });

    //if (db == "soundkit" && col == "ID"){
    //    fkModal.insertAdjacentHTML("beforeend", "<div id='soundkitList'></div>");
    //    // TODO: Get rid of JQuery
    //    $( "#soundkitList" ).load( "/files/sounds.php?embed=1&skitid=" + value );
    //}
}

function dec2hex(str, big = false){
    if (BigInt !== undefined && big){
        return (BigInt(str)).toString(16).replace('-', '');
    } else {
        return (parseInt(str) >>> 0).toString(16);
    }
}

function BGRA2RGBA(color){
    var hex = dec2hex(color).padStart(6, '0');

    for (var bytes = [], c = 0; c < hex.length; c += 2)
    {
        bytes.push(parseInt(hex.substr(c, 2), 16));
    }

    for (let i = 0; i < 4; i++){
        if (bytes[i] == undefined){
            bytes[i] = 0;
        }
    }
    console.log(color + " => #" + hex + " => " + bytes);

    let b = bytes[2];
    let g = bytes[1];
    let r = bytes[0];
    let a = 255;

    return "rgba(" + r + "," + g + "," + b + "," + a + ")";
}


function getFlagDescriptions(db, field, value, targetFlags = 0){
    let usedFlags = Array();
    if (targetFlags == 0){
        // eslint-disable-next-line no-undef
        targetFlags = flagMap.get(db.toLowerCase() + '.' + field);
    }

    if (BigInt === undefined){
        return [value];
    }

    if (value == "-1")
        return ["All"];

    for (let i = 0; i < 32; i++){
        let toCheck = BigInt(1) << BigInt(i);
        if (BigInt(value) & toCheck){
            if (targetFlags !== undefined && targetFlags[toCheck]){
                usedFlags.push(['0x' + "" + dec2hex(toCheck, true), targetFlags[toCheck]]);
            } else {
                usedFlags.push(['0x' + "" + dec2hex(toCheck, true), ""]);
            }
        }
    }

    return usedFlags;
}

function fancyFlagTable(flagArrs){
    if (flagArrs.length == 0){
        return "";
    }

    let tableHtml = "<table class=\"table table-sm table-striped\">";
    flagArrs.forEach((flagArr) => {
        tableHtml += "<tr><td>" + flagArr[0] + "</td><td>" + flagArr[1].replace("\"", "&quot;").replace("'", "&apos;") + "</td></tr>";
    });
    tableHtml += "</table>";

    return tableHtml;
}

function getEnum(db, field, value){
    // eslint-disable-next-line no-undef
    const targetEnum = enumMap.get(db.toLowerCase() + '.' + field);
    return getEnumVal(targetEnum, value);
}

function getEnumVal(targetEnum, value){
    if (targetEnum[value] !== undefined){
        if (Array.isArray(targetEnum[value])){
            return targetEnum[value][0];
        } else {
            return targetEnum[value];
        }
    } else {
        return "Unk";
    }
}

function parseLogic(l) { var i=0;var r = ""
    if (l & (1 << (16 + i))) r+='!'; r+='#'+i
    for (++i; i < 4; ++i) {
        let op = (l >> (2*(i-1))) & 3
        if (op == 1) r += ' | '; else if (op == 2) r+=' & '; else if (op == 0) continue
        if (l & (1 << (16 + i))) r+='!'; r+='#'+i
    }
    return r;
}

function parseDate(date){
    if (date == 0)
        return "";

    console.log("parsing " + date);

    let minute = date & 0x3F;
    if (minute == 63)
        minute = -1;

    console.log("minute", minute);
    
    let hour = (date >> 6) & 0x1F;
    if (hour == 31)
        hour = -1;

    console.log("hour", hour);

    let dotw = (date >> 11) & 0x7;
    if (dotw == 7)
        dotw = -1;
    
    console.log("day of the week", dotw);

    let dotm = (date >> 14) & 0x3F;
    if (dotm == 63){
        dotm -1;
    } else {
        dotm += 1;
    }
    
    console.log("day of the month", dotm);

    let month = (date >> 20) & 0xF;
    if (month == 15){
        month = -1;
    } else { 
        month += 1;
    }

    console.log("month", month);

    let year = (date >> 24) & 0x1F;
    if (year == 31){
        year = -1;
    } else {
        year += 2000;
    }

    console.log("year", year);

    let tz = (date >> 29) & 0x3;
    if (tz == 3)
        tz = -1;

    console.log("timezone", tz);

    if (dotm > 0 && month > 0 && year > 0){
        const utcDate = new Date(Date.UTC(year, month - 1, dotm, hour, minute, 0));
        return utcDate.toUTCString();
    }
}
