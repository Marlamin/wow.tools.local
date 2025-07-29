document.addEventListener('mousemove', (e) => {
    if (!e.target.matches('[data-tooltip]')) {
        return;
    }

    tooltip2.call(this, e.target, e);
});

function tooltip2(el, event){
    if (document.getElementById("tooltipToggle")){
        if (!document.getElementById("tooltipToggle").checked){
            return;
        }
    }

    el.addEventListener("mouseout", hideTooltip2, el);
    el.addEventListener("click", hideTooltip2, el);

    const tooltipType = el.dataset.tooltip;
    const tooltipTargetValue = el.dataset.id;
    let tooltipDiv = document.getElementById("wtTooltip");
    let defaultTooltipHTML = "<div id='tooltip'><div class='tooltip-icon' style='display: none'></div><div class='tooltip-desc'>Generating tooltip..</div></div></div>";
    let needsRefresh = false;

    if (!tooltipDiv) {
        // Tooltip div does not exist yet, create!
        tooltipDiv = document.createElement("div");
        tooltipDiv.dataset.type = tooltipType;
        tooltipDiv.dataset.id = tooltipTargetValue;
        tooltipDiv.innerHTML = defaultTooltipHTML;
        tooltipDiv.style.position = "absolute";
        tooltipDiv.style.top = event.pageY + "px";

        tooltipDiv.style.left = event.pageX + "px";
        tooltipDiv.style.zIndex = 1100;
        tooltipDiv.style.display = "block";
        tooltipDiv.id = "wtTooltip";
        tooltipDiv.classList.add('wt-tooltip');

        //if (tooltipType == "spell" || tooltipType == "item"){
        //    tooltipDiv.querySelector(".tooltip-icon").style.display = 'block';
        //}
        needsRefresh = true;
        document.body.appendChild(tooltipDiv);
    } else {
        tooltipDiv.style.display = "block";
        tooltipDiv.style.top = (event.pageY + 5) + "px";
        tooltipDiv.style.left = (event.pageX + 5) + "px";

        if (tooltipTargetValue != tooltipDiv.dataset.id || tooltipType != tooltipDiv.dataset.type){
            tooltipDiv.innerHTML = defaultTooltipHTML;
            tooltipDiv.dataset.type = tooltipType;
            tooltipDiv.dataset.id = tooltipTargetValue;

            //if (tooltipType == "spell" || tooltipType == "item"){
            //    tooltipDiv.querySelector(".tooltip-icon").style.display = 'block';
            //}

            needsRefresh = true;
        }

        repositionTooltip(tooltipDiv);
    }

    if ((event.pageX + 400) > window.innerWidth) {
        tooltipDiv.style.left = ((event.pageX + 5) - 400) + "px";
    }

    if (needsRefresh){
        let localBuild = "";

        if ('build' in el.dataset){
            localBuild = el.dataset.build;
        } else if (build != undefined) {
            localBuild = build;
        } else {
            // TODO: Global site fallback build?
        }

        //if (tooltipType == 'spell'){
        //    generateSpellTooltip(tooltipTargetValue, tooltipDiv, localBuild);
        //} else if (tooltipType == 'item'){
        //    generateItemTooltip(tooltipTargetValue, tooltipDiv, localBuild);
        //} else if (tooltipType == 'creature'){
        //    generateCreatureTooltip(tooltipTargetValue, tooltipDiv);
        //} else if (tooltipType == 'quest'){
        //    generateQuestTooltip(tooltipTargetValue, tooltipDiv);
        if (tooltipType == 'fk') {
            if (el.dataset.fk == "FileData::ID") {
                generateFileTooltip(tooltipTargetValue, tooltipDiv);
            } else {
                generateFKTooltip(el.dataset.fk, tooltipTargetValue, tooltipDiv, localBuild);
            }
        } else if (tooltipType == 'file') {
            generateFileTooltip(tooltipTargetValue, tooltipDiv);
        } else if (tooltipType == 'wex') {
            generateWExTooltip(tooltipTargetValue, tooltipDiv);
        } else if (tooltipType == "flags") {
            if (el.dataset.overrideflag != null) {
                generateFlagsTooltip(el.dataset.table, el.dataset.col, el.dataset.value, tooltipDiv, el.dataset.overrideflag);
            } else {
                generateFlagsTooltip(el.dataset.table, el.dataset.col, el.dataset.value, tooltipDiv);
            }
        } else {
            console.log("Unsupported tooltip type " + tooltipType);
            return;
        }
    }
}

function hideTooltip2(){
    if (document.getElementById("keepTooltips")){
        if (document.getElementById("keepTooltips").checked){
            return;
        }
    }

    if (document.getElementById("wtTooltip")){
        document.getElementById("wtTooltip").style.display = "none";
    }

    return true;
}

function generateFKTooltip(targetFK, value, tooltip, build)
{
    console.log("Generating foreign key tooltip for " + value);

    if (parseInt(build.split('.')[0]) > 6 && targetFK == "SoundEntries::ID")
        targetFK = "soundkit::ID";

    const collapsedFKs = ["playercondition::id", "holidays::id", "spellchaineffects::id", "spellvisual::id", "soundkitadvanced::id"];

    const tooltipIcon = tooltip.querySelector(".tooltip-icon img");
    const tooltipDesc = tooltip.querySelector(".tooltip-desc");

    const explodedTargetFK = targetFK.split("::");
    const table = explodedTargetFK[0].toLowerCase();
    let col = explodedTargetFK[1];

    if (col == "id")
        col = "ID";

    Promise.all([
        fetch("/dbc/peek/" + table + "?build=" + build + "&col=" + col + "&val=" + value),
    ])
        .then(function (responses) {
            return Promise.all(responses.map(function (response) {
                if (tooltipDesc == undefined){
                    console.log("Tooltip closed before rendering finished, nevermind");
                    return;
                }
                return response.json();
            })).catch(function (error) {
                console.log("An error occurred retrieving data to generate the tooltip: " + error);
                tooltipDesc.innerHTML = "An error occured generating the tooltip: " + error;
            });
        }).then(function (data) {
            if (tooltipDesc == undefined){
                console.log("Tooltip closed before rendering finished, nevermind");
                return;
            }

            const json = data[0];
            let tooltipTable = "<table class='tooltip-table'><tr><td colspan='2'><h2 class='q2'>" + targetFK + " value " + value +"</h2></td></tr>";

            if (!json || Object.keys(json.values).length == 0){
                if (table == "creature" && col == "ID"){
                    generateCreatureTooltip(value, tooltip);
                }

                tooltipTable += "<tr><td colspan='2'>Row not available in client DB</td><td>";
            }

            Object.keys(json.values).forEach(function (key) {
                const val = json.values[key];

                if (collapsedFKs.includes(targetFK.toLowerCase()) && (val == 0 || val == -1)){
                    return;
                }

                tooltipTable += "<tr><td>" + key + "</td><td>";

                if (key.startsWith("Flags") || flagMap.has(table + "." + key)){
                    tooltipTable += "0x" + dec2hex(val);
                } else if (targetFK == "PlayerCondition::ID" && key.endsWith("Logic")){
                    tooltipTable += val + " (" + parseLogic(val) + ")";
                } else {
                    tooltipTable += val;
                }
                if (enumMap.has(table + "." + key)){
                    var enumVal = getEnum(table, key, val);
                    if (val == '0' && enumVal == "Unk"){
                        // returnVar += full[meta.col];
                    } else {
                        tooltipTable += " <i>(" + enumVal + ")</i>";
                    }
                }

                tooltipTable += "</td></tr>"
            });

            tooltipTable += "</table>";

            if (collapsedFKs.includes(targetFK.toLowerCase())) {
                tooltipTable += "<p class='yellow'>(Empty/0 values hidden for this table)</p>";
            }

            tooltipDesc.innerHTML = tooltipTable;

            repositionTooltip(tooltip);
        }).catch(function (error) {
            console.log("An error occurred retrieving data to generate the tooltip: " + error);
            tooltipDesc.innerHTML = "An error occured generating the tooltip: " + error;
        });
}

function generateFileTooltip(id, tooltip)
{
    console.log("Generating file tooltip for " + id);

    //const tooltipIcon = tooltip.querySelector(".tooltip-icon img");
    const tooltipDesc = tooltip.querySelector(".tooltip-desc");

    Promise.all([
        fetch("/dbc/tooltip/file/" + id),
    ])
        .then(function (responses) {
            return Promise.all(responses.map(function (response) {
                if (tooltipDesc == undefined){
                    console.log("Tooltip closed before rendering finished, nevermind");
                    return;
                }
                return response.json();
            })).catch(function (error) {
                console.log("An error occurred retrieving data to generate the tooltip: " + error);
                tooltipDesc.innerHTML = "An error occured generating the tooltip: " + error;
            });
        }).then(function (data) {
            if (tooltipDesc == undefined){
                console.log("Tooltip closed before rendering finished, nevermind");
                return;
            }

            const calcData = data[0];

            let tooltipTable = "<table class='tooltip-table'><tr><td colspan='2'><h2 class='q2'>FileDataID " + calcData["fileDataID"] + "</h2></td></tr>";
            if (calcData["filename"] != null){
                tooltipTable += "<tr><td>Filename</td><td>" + calcData["filename"];
                //if (calcData["isOfficialFilename"] == true){
                //    tooltipTable += " <img src='/img/blizz.png'>";
                //}
                tooltipTable += "</td></tr>";
            } else {
                tooltipTable += "<tr><td>Filename</td><td>Unknown</td></tr>";
            }

            //if (calcData["type"] != null && calcData["type"] == "blp"){
            //    tooltipTable += "<tr><td colspan='2'><img class='tooltip-preview' src='https://wow.tools/casc/preview/fdid?buildconfig=" + SiteSettings.buildConfig + "&cdnconfig=" + SiteSettings.cdnConfig + "&filename=inlinepreview.blp&filedataid=" + calcData["fileDataID"] + "'></td></tr>";
            //}

            tooltipDesc.innerHTML = tooltipTable;
        }).catch(function (error) {
            console.log("An error occurred retrieving data to generate the tooltip: " + error);
            tooltipDesc.innerHTML = "An error occured generating the tooltip: " + error;
        });
}

function generateWExTooltip(ex, tooltip) {

    const tooltipDesc = tooltip.querySelector(".tooltip-desc");

    Promise.all([
        fetch("/dbc/tooltip/wex/" + ex),
    ])
        .then(function (responses) {
            return Promise.all(responses.map(function (response) {
                if (tooltipDesc == undefined) {
                    console.log("Tooltip closed before rendering finished, nevermind");
                    return;
                }
                return response.json();
            })).catch(function (error) {
                console.log("An error occurred retrieving data to generate the tooltip: " + error);
                tooltipDesc.innerHTML = "An error occured generating the tooltip: " + error;
            });
        }).then(function (data) {
            if (tooltipDesc == undefined) {
                console.log("Tooltip closed before rendering finished, nevermind");
                return;
            }

            const calcData = data[0];

            tooltipDesc.innerHTML = data[0]["result"];
        }).catch(function (error) {
            console.log("An error occurred retrieving data to generate the tooltip: " + error);
            tooltipDesc.innerHTML = "An error occured generating the tooltip: " + error;
        });
}

function generateFlagsTooltip(table, col, value, tooltip, overrideflag) {
    const tooltipDesc = tooltip.querySelector(".tooltip-desc");
    if (BigInt === undefined) {
        tooltipDesc.innerHTML = "BigInt is not supported on this browser.";
        return;
    }

    if (value == 0) {
        tooltipDesc.innerHTML = "No flags set.";
        return;
    }

    let targetFlags = flagMap.get(table.toLowerCase() + '.' + col);
    if (overrideflag != null)
        targetFlags = JSON.parse(overrideflag);

    if (targetFlags == 0) {
        tooltipDesc.innerHTML = "No defined flags found for this field (" + table + "::" + col + ").";
        return;
    }

    if (value == "-1") {
        tooltipDesc.innerHTML = "-1 usually means all flags are enabled.";
        return;
    }

    const usedFlags = [];
    for (let i = 0; i < 32; i++) {
        let toCheck = BigInt(1) << BigInt(i);
        if (BigInt(value) & toCheck) {
            if (targetFlags !== undefined && targetFlags[toCheck]) {
                usedFlags.push(['0x' + "" + dec2hex(toCheck, true), targetFlags[toCheck]]);
            } else {
                usedFlags.push(['0x' + "" + dec2hex(toCheck, true), ""]);
            }
        }
    }

    let tooltipContents = "<table class='tooltip-table'>";

    for (let i = 0; i < usedFlags.length; i++) {
        tooltipContents += "<tr><td><b>" + usedFlags[i][0] + "</b></td>";
        if (usedFlags[i][1] == "") {
            tooltipContents += "<td style='opacity: 0.6;'><i>Unknown flag</i></td>";
        } else {
            tooltipContents += "<td>" + usedFlags[i][1] + "</td>";
        }
        tooltipContents += "</tr>";
    }

    tooltipContents += "</table>";

    tooltipDesc.innerHTML = tooltipContents;
}

function repositionTooltip(tooltip){
    const tooltipRect = tooltip.getBoundingClientRect();
    if (tooltipRect.bottom > window.innerHeight){
        tooltip.style.top =  (window.innerHeight - (tooltipRect.bottom - tooltipRect.top) - 5) + "px";

    }
}

function hideTooltip(el){
    return;
    if (document.getElementById("keepTooltips")){
        if (document.getElementById("keepTooltips").checked){
            return;
        }
    }

    if (el.children.length > 0){
        for (let i = 0; i < el.children.length; i++){
            if (el.children[i].classList.contains('wt-tooltip')){
                el.removeChild(el.children[i]);
            }
        }
    }
}
