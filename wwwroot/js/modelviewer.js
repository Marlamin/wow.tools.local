const NUM_TEXTURE_SLOTS = 27; // As the first configurable texture slot is #0, the value here is 1 more than the highest texture slot #

var Module = {
    onRuntimeInitialized: function() {
        createscene();
    },
    locateFile: function (path, prefix) {
        return prefix + path+"?v="+window.emscriptenBuildTime;
    }
};

function showError(error){
    $("#errors").append("<div class='error alert alert-danger'>" + error +"</div>")
}

window.onerror = function(message, source, lineno, colno, error) {
    showError("An error occured! You might have to force reload the page with <kbd>CTRL-F5</kbd>. <br>Let us know what the error is by opening the console (<kbd>CTRL-SHIFT-J</kbd> on Chrome) and posting a screenshot of the error in <a href='https://discord.gg/5bkAvXFkDF'>Discord</a>.");
}

var Elements =
{
    Sidebar: document.getElementById('js-sidebar'),
    Counter: document.getElementById('fpsLabel'),
    EventLabel: document.getElementById('eventLabel'),
    DownloadLabel: document.getElementById('downloadLabel'),
    ModelControl: document.getElementById('js-model-control'),
};

var Settings =
{
    showFPS: true,
    paused: false,
    clearColor: [0.117, 0.207, 0.392],
    farClip: 500,
    farClipCull: 250,
    speed: 1000.0,
    portalCulling: true,
    newDisplayInfo: true,
    buildName: ""
}

var Current =
{
    buildName: "",
    fileDataID: 397940,
    type: "m2",
    embedded: false,
    displayID: 0,
    availableGeosets: [],
    enabledGeosets: [],
    geosetsDone: false,
    availableTextures: new Array(NUM_TEXTURE_SLOTS),
    posX: 0,
    posY: 0,
    posZ: 0,
}

var DownloadQueue = [];
var isDownloading = false;
var numDownloading = 0;

var stats = new Stats();

const materialResourceMap = new Map();

const filenameMap = new Map();
filenameMap.set("0", "No texture");

function loadSettings(applyNow = false){
    /* Show/hide FPS counter */
    var storedShowFPS = localStorage.getItem('settings[showFPS]');
    if (storedShowFPS){
        if (storedShowFPS== "1"){
            Settings.showFPS = true;
            stats.showPanel(1);
            Elements.Counter.appendChild(stats.dom);
        } else {
            Settings.showFPS = false;
            Elements.Counter.innerHTML = "";
        }
    }

    document.getElementById("showFPS").checked = Settings.showFPS;

    /* Clear color */
    var storedCustomClearColor = localStorage.getItem('settings[customClearColor]');
    if (storedCustomClearColor){
        document.getElementById("customClearColor").value = storedCustomClearColor;
    } else {
        document.getElementById("customClearColor").value = '#1e3564';
    }

    var rawClearColor = document.getElementById("customClearColor").value.replace('#', '');
    var r = parseInt('0x' + rawClearColor.substring(0, 2)) / 255;
    var g = parseInt('0x' + rawClearColor.substring(2, 4)) / 255;
    var b = parseInt('0x' + rawClearColor.substring(4, 6)) / 255;
    Settings.clearColor = [r, g, b];

    /* Far clip */
    var storedFarClip = localStorage.getItem('settings[farClip]');
    if (storedFarClip){
        Settings.farClip = storedFarClip;
        document.getElementById('farClip').value = storedFarClip;
    } else {
        document.getElementById('farClip').value = Settings.farClip;
    }

    /* Far clip (model culling) */
    var storedFarClipCull = localStorage.getItem('settings[farClipCull]');
    if (storedFarClipCull){
        Settings.farClipCull = storedFarClipCull;
        document.getElementById('farClipCull').value = storedFarClipCull;
    } else {
        document.getElementById('farClipCull').value = Settings.farClipCull;
    }

    /* Portal culling */
    var storedPortalCulling = localStorage.getItem('settings[portalCulling]');
    if (storedPortalCulling){
        if (storedPortalCulling== "1"){
            Settings.portalCulling = true;
        } else {
            Settings.portalCulling = false;
        }
    }

    document.getElementById("portalCulling").checked = Settings.portalCulling;

    /* New Display Info */
    var newDisplayInfo = localStorage.getItem('settings[newDisplayInfo]');
    if (newDisplayInfo){
        if (newDisplayInfo== "1"){
            Settings.newDisplayInfo = true;
        } else {
            Settings.newDisplayInfo = false;
        }   
    }

    document.getElementById("newDisplayInfo").checked = Settings.newDisplayInfo;

    /* If settings should be applied now (don't do this on page load!) */
    if (applyNow){
        Module._setClearColor(Settings.clearColor[0], Settings.clearColor[1], Settings.clearColor[2]);
        Module._setFarPlane(Settings.farClip);
        Module._setFarPlaneForCulling(Settings.farClipCull);
        Module._enablePortalCulling(Settings.portalCulling);
    }
}

function saveSettings(){
    if (document.getElementById("showFPS").checked){
        localStorage.setItem('settings[showFPS]', '1');
    } else {
        localStorage.setItem('settings[showFPS]', '0');
    }

    localStorage.setItem('settings[customClearColor]', document.getElementById("customClearColor").value);
    localStorage.setItem('settings[farClip]', document.getElementById("farClip").value);
    localStorage.setItem('settings[farClipCull]', document.getElementById("farClipCull").value);

    if (document.getElementById("portalCulling").checked){
        localStorage.setItem('settings[portalCulling]', '1');
    } else {
        localStorage.setItem('settings[portalCulling]', '0');
    }

    if (document.getElementById("newDisplayInfo").checked){
        localStorage.setItem('settings[newDisplayInfo]', '1');
    } else {
        localStorage.setItem('settings[newDisplayInfo]', '0');
    }
    loadSettings(true);
}

// Sidebar button, might not exist in embedded mode
if (document.getElementById( 'js-sidebar-button' )){
    document.getElementById( 'js-sidebar-button' ).addEventListener( 'click', function( )
    {
        Elements.Sidebar.classList.toggle( 'closed' );
    } );
}

// Model control button, might not exist in embedded mode
if (document.getElementById( 'modelControlButton' )){
    document.getElementById( 'modelControlButton' ).addEventListener( 'click', function( )
    {
        Elements.ModelControl.classList.toggle( 'closed' );
    } );
}

try {
    if (typeof WebAssembly === "object" && typeof WebAssembly.instantiate === "function") {
        const module = new WebAssembly.Module(Uint8Array.of(0x0, 0x61, 0x73, 0x6d, 0x01, 0x00, 0x00, 0x00));
        if (module instanceof WebAssembly.Module)
            var testModule = new WebAssembly.Instance(module) instanceof WebAssembly.Instance;
        if (!testModule) showError("WebAssembly support is required but not supported by your browser.");
    }
} catch (e) {
    showError("WebAssembly support is required but not supported by your browser.");
}

var searchParams = new URLSearchParams(window.location.search);

var urlFileDataID = searchParams.get("filedataid");
if (urlFileDataID){
    Current.fileDataID = urlFileDataID;
}

var urlType = searchParams.get("type");
if (urlType){
    Current.type = urlType;
}

var urlEmbed = searchParams.get("embed");
if (urlEmbed){
    Current.embedded = true;
    $("#navbar").hide();
    $("#js-sidebar-button").hide();
    $("#fpsLabel").hide();
    console.log("Running modelviewer in embedded mode!");
}

var urlClearColor = searchParams.get("clearColor");
if (urlClearColor){
    var r = parseInt('0x' + urlClearColor.substring(0, 2)) / 255;
    var g = parseInt('0x' + urlClearColor.substring(2, 4)) / 255;
    var b = parseInt('0x' + urlClearColor.substring(4, 6)) / 255;
    Settings.clearColor = [r, g, b];
}

var posX = searchParams.get("x");
if (posX)
    Current.posX = posX;

var posY = searchParams.get("y");
if (posY)
    Current.posY = posY;

var posZ = searchParams.get("z");
if (posZ)
    Current.posZ = posZ;

var urlDisplayID = searchParams.get("displayID");
if (urlDisplayID){
    Current.displayID = urlDisplayID;
}

window.createscene = async function () {
    const response = await fetch("/casc/buildname");
    Settings.buildName = await response.text();
    Current.buildName = Settings.buildName;

    Module["canvas"] = document.getElementById("wowcanvas");

    var url = "/casc/fname?filename=";
    let urlFileId = "/casc/fdid?filedataid=";

    var ptrUrl = allocateUTF8(url);
    var ptrUrlFileDataId = allocateUTF8(urlFileId);

    Module._createWebJsScene(document.body.clientWidth, document.body.clientHeight, ptrUrl, ptrUrlFileDataId);

    Module._setClearColor(Settings.clearColor[0], Settings.clearColor[1], Settings.clearColor[2]);

    if (Current.fileDataID == 397940 && Current.displayID != 0){
        Current.fileDataID = await getFileDataIDByDisplayID(Current.displayID);
        Settings.newDisplayInfo = true;
    }

    loadModel(Current.type, Current.fileDataID);

    _free(ptrUrl);
    _free(ptrUrlFileDataId);
    var lastTimeStamp = new Date().getTime();

    Module["canvas"].width = document.body.clientWidth;
    Module["canvas"].height = document.body.clientHeight;

    // Add input elements to texture form:
    const textureForm = document.getElementById("textureForm");
    const textureRow = document.getElementById("textureRow");
    const sscTextureForm = document.getElementById("sscTextureForm");
    const sscTextureListSelect = document.getElementById("sscTextureListSelect");

    for (let i = 0; i < 2; i++) {
        const textureCol = document.createElement('div');
        textureCol.id = "textureCol" + i;
        textureCol.classList.add("col-md-6");
        textureRow.appendChild(textureCol);
    }

    for (let i = 0; i < NUM_TEXTURE_SLOTS; i++)
    {
      const textureInputDiv = document.createElement('div');
      const textureInput = document.createElement('input');

        let targetCol = 0;
        if (i > (NUM_TEXTURE_SLOTS / 2)) {
            targetCol = 1;
        }

        const textureCol = document.getElementById("textureCol" + targetCol);
      textureInputDiv.classList.add("textureInputDiv");
      
      textureInput.type = "text";
      textureInput.id = "tex" + i;
      textureInput.setAttribute('name', "textures[" + i + "]");
      textureInput.classList.add("textureInput");
      
      var labelSuffix = "";
      switch (i)
      {
        case 2:
            labelSuffix = " (item #1)";
            break;
        case 3:
            labelSuffix = " (item #2)";
            break;
        case 4:
            labelSuffix = " (item #3)";
            break;
        case 5:
            labelSuffix = " (creature #4)";
            break;
        case 11:
            labelSuffix = " (creature #1)";
            break;
        case 12:
            labelSuffix = " (creature #2)";
            break;
        case 13:
            labelSuffix = " (creature #3)";
            break;
        case 24:
            labelSuffix = " (item #4)";
            break;
        }

        textureInput.placeholder = "Texture #" + i + labelSuffix;
      
      textureInputDiv.appendChild(textureInput);
      textureCol.appendChild(textureInputDiv);
      
      const sscTextureInputDiv = document.createElement('div');
      const sscTextureInput = document.createElement('input');
      const sscTextureInputLabel = document.createElement('label');
      sscTextureInputDiv.classList.add("sscInputDiv");
      sscTextureInputDiv.classList.add("sscTextureInputDiv");
      sscTextureInput.type = "text";
      sscTextureInput.id = "sscTex" + i;
      sscTextureInput.setAttribute('name', "sscTextures[" + i + "]");
      sscTextureInput.classList.add("sscInput");
      sscTextureInput.classList.add("sscTextureInput");
      sscTextureInputLabel.htmlFor = "sscTex" + i;
      sscTextureInputLabel.innerHTML = (i + ":");
      sscTextureInputLabel.classList.add("sscInputLabel");
      sscTextureInputLabel.classList.add("sscTextureInputLabel");
      sscTextureInputDiv.id = "sscTexDiv" + i;
      sscTextureInputDiv.appendChild(sscTextureInputLabel);
      sscTextureInputDiv.appendChild(sscTextureInput);
      sscTextureForm.appendChild(sscTextureInputDiv);
    }
    
    Module["animationArrayCallback"] = function(animIDArray) {
        const animSelect = document.getElementById("animationSelect");
        animSelect.length = 0;
        
        if (animIDArray.length > 0) {
          const animationOptions = [];
          
          animIDArray.forEach(function(a) {
            var opt = document.createElement('option');
            opt.value = a;
            if (a in animationNames) {
              opt.innerHTML = animationNames[a] + ' (' + a + ')';
            } else {
              console.log("Missing animation name for " + a + ", let a dev know!");
              opt.innerHTML = 'Animation ' + a;
            }
            animationOptions.push(opt);
          });
        
          animationOptions.sort(function(a,b) {
            if (a.innerHTML > b.innerHTML)
              return 1;
            return 0;
          });
        
          animationOptions.forEach(function(a) {
            animSelect.append(a);
          });
          
          animSelect.value = 0;
          animSelect.style.display = "block";
        }
    };

    Module["meshIdArrayCallback"] = function(meshIDArray) {
        Current.availableGeosets = Object.values(meshIDArray).map(function (x) { return parseInt(x, 10); }).sort();

        // Add in options for zero values, since they're sometimes valid but the model meshID data unfortunately aren't including them:
        const zeroGeosets = Current.availableGeosets.map(function (x) { return 100 * Math.floor(x / 100); });
        Current.availableGeosets = [...new Set(zeroGeosets.concat(Current.availableGeosets).sort())];
        
        const geosetControl = document.getElementById("geosets");
        geosetControl.innerHTML = "This functionality is WIP and might cause display issues. Use with caution. Sometimes a geoset value of 0 gives a valid appearance, other times it creates a hole in your model.";
        const sscGeoForm = document.getElementById("sscGeosetForm");
        sscGeoForm.innerHTML = "<strong>Geoset Groups:</strong>";
        for (let meshID of Current.availableGeosets){
            meshID = Number(meshID);

            const geosetGroup = Math.round(meshID / 100);
            const geosetIndex = meshID - (geosetGroup * 100);
            
            if (!document.getElementById("geosets-" + geosetGroup)){
                let geosetHolder = document.createElement('div');
                geosetHolder.innerHTML = "Geoset #" + geosetGroup;
                geosetHolder.id = "geosets-" + geosetGroup;
                geosetControl.appendChild(geosetHolder);
                
                let geosetSelect = document.createElement('select');
                geosetSelect.id = "geosetSelection-" + geosetGroup;
                geosetSelect.classList.add("geosetSelection");
                geosetSelect.dataset.geosetGroup = geosetGroup;
                geosetSelect.onchange = function () { Current.enabledGeosets[Number(this.dataset.geosetGroup)] = Number(this.value); updateEnabledGeosets(); };

                let opt = document.createElement('option');
                opt.value = -1;
                opt.innerHTML = "-1 (all)";
                geosetSelect.appendChild(opt);

                geosetHolder.appendChild(geosetSelect);
            }

            let select = document.getElementById("geosetSelection-" + geosetGroup);
            let opt = document.createElement('option');
            opt.value = geosetIndex;
            opt.innerHTML = geosetIndex;
            select.appendChild(opt);
            
            // prefill screenshot combo tab with (mostly) valid geoset options for this model...
            let geoInput = document.getElementById("sscGeo" + geosetGroup);
            if (!geoInput){
              const sscGeosetInputDiv = document.createElement('div');
              const sscGeosetInput = document.createElement('input');
              const sscGeosetInputLabel = document.createElement('label');
              sscGeosetInputDiv.classList.add("sscInputDiv");
              sscGeosetInputDiv.classList.add("sscGeosetInputDiv");
              sscGeosetInput.type = "text";
              sscGeosetInput.id = "sscGeo" + geosetGroup;
              sscGeosetInput.value = geosetIndex;
              sscGeosetInput.dataset.geosetGroup = geosetGroup;
              sscGeosetInput.classList.add("sscInput");
              sscGeosetInput.classList.add("sscGeosetInput");
              sscGeosetInputLabel.htmlFor = "sscGeo" + geosetGroup;
              sscGeosetInputLabel.innerHTML = geosetGroup + ":";
              sscGeosetInputLabel.classList.add("sscInputLabel");
              sscGeosetInputLabel.classList.add("sscGeosetInputLabel");
              sscGeosetInputDiv.appendChild(sscGeosetInputLabel);
              sscGeosetInputDiv.appendChild(sscGeosetInput);
              sscGeoForm.appendChild(sscGeosetInputDiv);
            }
            else{
              geoInput.value = geoInput.value + "," + geosetIndex;
            }
        }

        Current.geosetsDone = true;
    };

    var renderfunc = function(now){
        stats.begin();

        var timeDelta = 0;

        if (lastTimeStamp !== undefined) {
            timeDelta = now - lastTimeStamp;
        }

        lastTimeStamp = now;

        Module._gameloop(timeDelta / Settings.speed);

        // if (numDownloading > 0){
        //     Elements.DownloadLabel.innerText = "Downloading " + numDownloading + " files..";
        // }

        stats.end();
        window.requestAnimationFrame(renderfunc);
    };

    window.requestAnimationFrame(renderfunc);
}

window.addEventListener('resize', () => {
    var canvas = document.getElementById("wowcanvas");
    if (canvas){
        canvas.width = document.body.clientWidth;
        canvas.height = document.body.clientHeight;
        if (Module && Module._setSceneSize){
            window.Module._setSceneSize(document.body.clientWidth, document.body.clientHeight);
        }
    }
});

$('#mvfiles').on('click', 'tbody tr td:first-child', function() {
    var data = Elements.table.row($(this).parent()).data();

    $(".selected").removeClass("selected");
    $(this).parent().addClass('selected');
    loadModel(data[4], data[0]);
});

$('#js-sidebar').on('input', '.paginate_input', function(){
    if ($(".paginate_input")[0].value != ''){
        $("#mvfiles").DataTable().page($(".paginate_input")[0].value - 1).ajax.reload(null, false)
    }
});

window.addEventListener('keydown', function(event){
    if (document.activeElement.tagName == "SELECT"){
        return;
    }

    if ($(".selected").length == 1){
        if (event.key == "ArrowDown"){
            if ($(".selected")[0].rowIndex == 20) return;
            if (document.getElementById('mvfiles').rows.length > 1){
                $(document.getElementById('mvfiles').rows[$(".selected")[0].rowIndex + 1].firstChild).trigger("click");
            }
        } else if (event.key == "ArrowUp"){
            if ($(".selected")[0].rowIndex == 1) return;
            if (document.getElementById('mvfiles').rows.length > 1){
                $(document.getElementById('mvfiles').rows[$(".selected")[0].rowIndex - 1].firstChild).trigger("click");
            }
        }
    }

    if (document.activeElement.tagName == "INPUT" || document.activeElement.tagName == "SELECT"){
        event.stopImmediatePropagation();
    } else {
        if (event.key == " "){
            if (Settings.paused){
                Settings.paused = false;
                Elements.EventLabel.textContent = "";
            } else {
                Settings.paused = true;
                Elements.EventLabel.innerHTML = "<i class='fa fa-pause'></i> Paused";
            }

            if (Settings.paused){
                Settings.speed = 1000000000000.0;
            } else {
                Settings.speed = 1000.0;
            }
        }
    }
}, true);

window.addEventListener('keyup', function(event){
    if (event.key == "PrintScreen" && !event.shiftKey && !event.ctrlKey && !event.altKey) Module._createScreenshot();
    if (document.activeElement.tagName == "INPUT" || document.activeElement.tagName == "SELECT"){
        event.stopImmediatePropagation();
    }
}, true);

window.addEventListener('keypress', function(event){
    if (document.activeElement.tagName == "INPUT" || document.activeElement.tagName == "SELECT"){
        event.stopImmediatePropagation();
    }

    if (event.key == "Z" && event.shiftKey){
        toggleUI();
    }
}, true);

$("#animationSelect").change(function () {
    var display = this.options[this.selectedIndex].value;
    Module._setAnimationId(display);
});

$("#skinSelect").change(function() {
    if (this.options[this.selectedIndex].dataset.displayid == undefined){
        // Backwards compat
        var display = this.options[this.selectedIndex].value.split(',');
        if (display.length == 3 || display.length == 4){
            // Creature
            if (display.length == 3){
                Module._resetReplaceParticleColor();
            }
            setModelTexture(display, 11);
        } else {
            // Item
            setModelTexture(display, 2);
        }
    } else {
        setModelDisplay(this.options[this.selectedIndex].dataset.displayid, this.options[this.selectedIndex].dataset.type);
    }
});

function toggleUI(){
    $(".navbar").toggle();
    $("#js-sidebar-button").toggle();
    $("#js-controls").toggle();
}

function loadModel(type, filedataid){
    Current.fileDataID = filedataid;
    Current.type = type;

    Module._setClearColor(Settings.clearColor[0], Settings.clearColor[1], Settings.clearColor[2]);
    Module._setFarPlane(Settings.farClip);
    Module._setFarPlaneForCulling(Settings.farClipCull);
    Module._enablePortalCulling(Settings.portalCulling);

    DownloadQueue = [];
    isDownloading = false;
    numDownloading = 0;
    $.ajax({
        url: "/listfile/info?filename=1&filedataid=" + Current.fileDataID
    })
        .done(function( filename ) {
            Current.filename = filename;

            updateURLs();

            if (!embeddedMode){
                history.pushState({id: 'modelviewer'}, 'Model Viewer', '/mv/?filedataid=' + Current.fileDataID);
            }

            $("#animationSelect").hide();
            $("#skinSelect").hide();

            var alwaysLoadByFDID = true;

            if (Current.type == "adt"){
                alwaysLoadByFDID = false;
            }

            if (Current.type == "m2"){
                $("#exportButton").prop('disabled', false);
            } else {
                $("#exportButton").prop('disabled', true);
            }
            
            if (Current.filename != "" && !alwaysLoadByFDID) {
                console.log("Loading " + Current.filename + " " + Current.fileDataID + " (" + Current.type + ")");
                var ptrName = allocateUTF8(Current.filename);
                if (Current.type == "adt") {
                    Module._setScene(2, ptrName, -1);
                    $("#js-controls").hide();
                } else if (Current.type == "wmo") {
                    Module._setScene(1, ptrName, -1);
                    $("#js-controls").hide();
                } else if (Current.type == "m2") {
                    Current.geosetsDone = false;
                    Module._setScene(0, ptrName, -1);
                    $("#js-controls").show();
                    if (!Settings.newDisplayInfo){
                        loadModelTextures();
                    } else {
                        loadModelDisplays();
                    }
                } else {
                    console.log("Unsupported type: " + Current.type);
                }
            } else {
                console.log("Loading " + Current.fileDataID + " (" + Current.type + ")");
                if (Current.type == "adt") {
                    Module._setSceneFileDataId(2, Current.fileDataID, -1);
                    $("#js-controls").hide();
                } else if (Current.type == "wmo") {
                    Module._setSceneFileDataId(1, Current.fileDataID, -1);
                    $("#js-controls").hide();
                } else if (Current.type == "wdt") {
                    Module._setMap(0, Current.fileDataID, Current.posX, Current.posY, Current.posZ)
                    $("#js-controls").hide();
                } else if (Current.type == "m2") {
                    Module._setSceneFileDataId(0, Current.fileDataID, -1);
                    $("#js-controls").show();
                    if (!Settings.newDisplayInfo){
                        loadModelTextures();
                    } else {
                        loadModelDisplays();
                    }
                } else {
                    console.log("Unsupported type: " + Current.type);
                }
            }
        });
}

async function setBuildNameByConfig(config){
    const buildResponse = await fetch("/api.php?type=namebybc&hash=" + config);
    const buildName = await buildResponse.text();
    Current.buildName = buildName;
}

async function loadModelDisplays() {
    let results = [];
    
    // Query and combine display info from both creature and item tables. Occasionally, a model will appear in both tables
    // (possibly erroneously) and if we just pick the first matching one then we risk missing out on the right data:
    
    itemResults = await loadItemDisplays();
    if (itemResults != undefined && itemResults.length > 0)
        results = results.concat(itemResults);     
            
    const cmdRows = await findCreatureModelDataRows();
    if (cmdRows.length > 0){
        creatureResults = await loadCreatureDisplays(cmdRows);
    if (creatureResults != undefined && creatureResults.length > 0)
        results = results.concat(creatureResults); 
    }

    if (results.length == 0)
        return;
    
    const skinSelect = document.getElementById("skinSelect");
    skinSelect.length = 0;

    for (let i = 0; i < NUM_TEXTURE_SLOTS; i++)
      Current.availableTextures[i] = new Set();
    
    // Filenames?
    for (const result of results){
        var opt = document.createElement('option');

        if (result.ResultType == "creature"){
            // Backwards compat with current model texture setting
            const tex0 = result['TextureVariationFileDataID[0]'];
            const tex1 = result['TextureVariationFileDataID[1]'];
            const tex2 = result['TextureVariationFileDataID[2]'];
            const tex3 = result['TextureVariationFileDataID[3]'];
            opt.value = tex0 + "," + tex1 + "," + tex2 + "," + tex3;
            opt.dataset.displayid = result.ID;
            opt.dataset.type = 'creature';
            
            if (tex0 != "") Current.availableTextures[11].add(tex0);
            if (tex1 != "") Current.availableTextures[12].add(tex1);
            if (tex2 != "") Current.availableTextures[13].add(tex2);
            if (tex3 != "") Current.availableTextures[5].add(tex3);

            if (!filenameMap.has(result['TextureVariationFileDataID[0]'])){
                const filenameResponse = await fetch("/listfile/info?filename=1&filedataid=" + result['TextureVariationFileDataID[0]']);
                const filenameContents = await filenameResponse.text();
                if (filenameContents != ""){
                    filenameMap.set(result['TextureVariationFileDataID[0]'], filenameContents.substring(filenameContents.lastIndexOf('/') + 1).replace(".blp", ""));
                } else {
                    filenameMap.set(result['TextureVariationFileDataID[0]'], "Unknown");
                }
            }

            opt.innerHTML = result.ID + " (" + filenameMap.get(result['TextureVariationFileDataID[0]']) + ")";
        } else if (result.ResultType == "item"){
            if (result['ModelMaterialResourcesID[0]'] != "0" && !materialResourceMap.has(result['ModelMaterialResourcesID[0]'])){
                const materialResponse = await fetch("/dbc/peek/TextureFileData/?build=" + Current.buildName + "&col=MaterialResourcesID&val=" + result['ModelMaterialResourcesID[0]']);
                const materialJson = await materialResponse.json();

                if (materialJson.values.FileDataID != undefined){
                    materialResourceMap.set(materialJson.values.MaterialResourcesID, materialJson.values.FileDataID);

                    if (!filenameMap.has(materialJson.values.FileDataID)){
                        const filenameResponse = await fetch("/listfile/info?filename=1&filedataid=" + materialJson.values.FileDataID);
                        const filenameContents = await filenameResponse.text();
                        if (filenameContents != ""){
                            filenameMap.set(materialJson.values.FileDataID, filenameContents.substring(filenameContents.lastIndexOf('/') + 1).replace(".blp", ""));
                        } else {
                            filenameMap.set(materialJson.values.FileDataID, "Unknown");
                        }
                    }
                }
            } else {
                opt.innerHTML = "DisplayID " + result.ID;
            }

            if (materialResourceMap.has(result['ModelMaterialResourcesID[0]'])){
                opt.value = materialResourceMap.get(result['ModelMaterialResourcesID[0]']);
                opt.innerHTML = result.ID + " (" +  filenameMap.get(materialResourceMap.get(result['ModelMaterialResourcesID[0]'])) + ")";
            } else {
                opt.value = 0;
                opt.innerHTML = result.ID + " (Unknown)";
            }
            
            opt.dataset.displayid = result.ID;
            opt.dataset.type = 'item';
        }

        // TODO: If display ID is given (through URL params??), set to selected otherwise select first
        if (Current.displayID == 0){
            if (skinSelect.children.length == 0){
                opt.selected = true;
                setModelDisplay(result.ID, result.ResultType);   
            }
        }
        else if (Current.displayID != 0 && result.ID == Current.displayID){
            opt.selected = true;
            setModelDisplay(result.ID, result.ResultType);
        }

        skinSelect.appendChild(opt);
    }

    if (skinSelect.children.length == 0){
        opt.selected = true;
        setModelDisplay(result.ID, result.ResultType);   
    }


    // prefill screenshot combo tab with valid texture options for this model. Hide fields that should be irrelevant.
    for (i = 0; i < Current.availableTextures.length; i++){
      if (Current.availableTextures[i].size == 0){
        document.getElementById("sscTexDiv" + i).hidden = true;
        continue;
      }
      document.getElementById("sscTexDiv" + i).hidden = false;
      document.getElementById("sscTex" + i).value = [...Current.availableTextures[i]].sort().join(',');
    }
    skinSelect.style.display = "block";
}

async function findCreatureModelDataRows(){
    const response = await fetch("/dbc/find/CreatureModelData/?build=" + Current.buildName + "&col=FileDataID&val=" + Current.fileDataID);
    const json = await response.json();
    return json;
}

async function loadCreatureDisplays(cmdRows){
    const cdiPromises = Array(cmdRows.length);
    let index = 0;
    for (const cmdRow of cmdRows) {
        cdiPromises[index++] = fetch("/dbc/find/CreatureDisplayInfo/?build=" + Current.buildName + "&col=ModelID&val=" + cmdRow.ID);
    }

    const cdiResult = await Promise.all(cdiPromises);
    const data = Array(cdiResult.length);
    index = 0;
    for (const response of cdiResult)
        data[index++] = await response.json();

    const result = [];
    index = 0
    for (const entry of data){
        for (const row of entry){
            // TODO: Generic result format?
            result[index] = row;
            result[index].ResultType = "creature";
            index++;
        }
    }

    return result;
}

async function loadItemDisplays(){
    const response = await fetch("/dbc/peek/ModelFileData/?build=" + Current.buildName + "&col=FileDataID&val=" + Current.fileDataID);
    const modelFileData = await response.json();

    if (modelFileData.values['ModelResourcesID'] === undefined)
        return [];

    const idiPromises = [
        fetch("/dbc/find/ItemDisplayInfo/?build=" + Current.buildName + "&col=ModelResourcesID[0]&val=" + modelFileData.values['ModelResourcesID']),
        fetch("/dbc/find/ItemDisplayInfo/?build=" + Current.buildName + "&col=ModelResourcesID[1]&val=" + modelFileData.values['ModelResourcesID'])
    ];

    const idiResult = await Promise.all(idiPromises);
    const data = Array(idiResult.length);
    let index = 0;
    for (const response of idiResult)
        data[index++] = await response.json();

    const result = [];
    index = 0
    for (const entry of data){
        for (const row of entry){
            // TODO: Generic result format?
            result[index] = row;
            result[index].ResultType = "item";
            index++;
        }
    }

    return result;
}

async function getFileDataIDByDisplayID(displayID){
    const cdiResponse = await fetch("/dbc/peek/CreatureDisplayInfo/?build=" + Current.buildName + "&col=ID&val=" + displayID);
    const cdiJson = await cdiResponse.json();
    
    const cmdResponse = await fetch("/dbc/peek/CreatureModelData/?build=" + Current.buildName + "&col=ID&val=" + cdiJson.values['ModelID']);
    const cmdJson = await cmdResponse.json();

    if (cmdJson.values['FileDataID'] !== undefined){
        return cmdJson.values['FileDataID'];
    }
}

function loadModelTextures() {
    //TODO build, fix wrong skin showing up after initial load
    var loadedTextures = Array();
    var currentFDID = Current.fileDataID;
    $.ajax({url: "/dbc/texture/" + Current.fileDataID + "?build=" + Current.buildName}).done( function(data) {
        var forFDID = this.url.replace("/dbc/texture/", "").replace("?build=" + Current.buildName, "");
        if (Current.fileDataID != forFDID){
            console.log("This request is not for this filedataid, discarding..");
            return;
        }

        $("#skinSelect").empty();
        for (let displayId in data) {
            if (!data.hasOwnProperty(displayId)) continue;

            var intArray = data[displayId];
            if (intArray.every(fdid => fdid === 0)){
                continue;
            }

            // Open controls overlay
            $("#skinSelect").show();

            if (loadedTextures.includes(intArray.join(',')))
                continue;

            loadedTextures.push(intArray.join(','));

            $.ajax({
                type: 'GET',
                url: "/listfile/info",
                data: {
                    filename: 1,
                    filedataid : intArray.join(",")
                }
            })
                .done(function( filename ) {
                    var textureFileDataIDs = decodeURIComponent(this.url.replace("/listfile/info?filename=1&filedataid=", '')).split(',');
          
                    var textureFileDataID = textureFileDataIDs[0];

                    var optionHTML = '<option value="' + textureFileDataIDs + '"';

                    if ($('#skinSelect option').length == 0){
                        optionHTML += " SELECTED>";
                        if (textureFileDataIDs.length == 3 || textureFileDataIDs.length == 4){
                        // Creature
                            setModelTexture(textureFileDataIDs, 11);
                        } else {
                        // Item
                            setModelTexture(textureFileDataIDs, 2);
                        }
                    } else {
                        optionHTML += ">";
                    }

                    if (filename != ""){
                        var nopathname = filename.replace(/^.*[\\\/]/, '');
                        optionHTML += "(" + textureFileDataID + ") " + nopathname + "</option>";
                    } else {
                        optionHTML += textureFileDataID + "</option>";
                    }

                    $("#skinSelect").append(optionHTML);
                });
        }
    });
}

function queueDL(url){
    DownloadQueue.push(url);
    numDownloading++;

    if (!isDownloading){
        isDownloading = true;
        $("#downloadLabel").show();
    }
}

function unqueueDL(url){
    DownloadQueue = DownloadQueue.filter(function(queuedURL){ 
        return queuedURL != url; 
    });

    if (DownloadQueue.length == 0){
        isDownloading = false;
        $("#downloadLabel").hide();
    }

    numDownloading--;
}

function handleDownloadStarted(url){
    queueDL(url);
}

function handleDownloadFinished(url){
    unqueueDL(url);
}

// Called by texture model save button
function updateTextures(){
    const textureArray = new Int32Array(NUM_TEXTURE_SLOTS);
    for (let i = 0; i < NUM_TEXTURE_SLOTS; i++){
        if (document.getElementById('tex' + i)){
            textureArray[i] = document.getElementById('tex' + i).value;
        }
    }
    setModelTextures(textureArray);
}

function parseComboList(comboListIndex){
  if (!document.getElementById(comboListIndex))
    return [0];
  list = document.getElementById(comboListIndex).value;

  list = list.replace(/\s+/g, ''); // remove spaces
  if (list == "" || list == "0")
    return [0];
  return list.split(",");
}

function asyncTimeout() {
  return new Promise((resolve) => { setTimeout(resolve, 1000) });
}

function takescreenShotCombo()
{
  Module._createScreenshot();
}

async function screenShotComboGeo(geoNumIndex, allGeosets)
{
  console.log("screenShotComboGeo() called. GeoNumIndex = " + geoNumIndex);
  if (geoNumIndex >= allGeosets.geoNums.length){
    await asyncTimeout();
    updateEnabledGeosets();
    await asyncTimeout();
    await takescreenShotCombo();
    return;
  }
  var geoNum = allGeosets.geoNums[geoNumIndex];
  var geoVals = allGeosets.geoVals[geoNumIndex];
  for (let i = 0; i < geoVals.length; i++){
    Current.enabledGeosets[geoNum] = Number(geoVals[i]);
    await screenShotComboGeo(geoNumIndex+1, allGeosets);
  }
}

async function screenShotComboTex(texNum, texArray, allTextures, allGeosets)
{
  console.log("screenShotComboTex() called. TexNum = " + texNum);
  if (texNum >= NUM_TEXTURE_SLOTS){
    await asyncTimeout();
    setModelTextures(texArray);
    await asyncTimeout();
    await screenShotComboGeo(0, allGeosets);
    return;
  }
  var texes = allTextures[texNum];
  for (let i = 0; i < texes.length; i++){
    texArray[texNum] = texes[i];
    await screenShotComboTex(texNum+1, texArray, allTextures, allGeosets);
  }
}

function screenShotCombos()
{
  // Take screenshots for multiple combos of textures and/or geoset variants for a model.
  // It does this slowly or it bugs out.
  var allTextures = new Array(NUM_TEXTURE_SLOTS);
  for (let i = 0; i < allTextures.length; i++){
    allTextures[i] = parseComboList('sscTex' + i);
  }

  let rawGeosetList = document.getElementsByClassName("sscGeosetInput");
  var allGeosets = {};
  allGeosets.geoNums = Array();
  allGeosets.geoVals = Array();
  if (rawGeosetList.length > 0){
    for (i = 0; i < rawGeosetList.length; i++){
      let geoNum = rawGeosetList[i].dataset.geosetGroup;
      let geoVals = parseComboList("sscGeo"+geoNum);
      if (geoVals.length > 1 || geoVals[0] != "0"){
        allGeosets.geoNums.push(geoNum);
        allGeosets.geoVals.push(geoVals);
      }
    }
  }
  screenShotComboTex(0, new Int32Array(NUM_TEXTURE_SLOTS), allTextures, allGeosets);
}

function getScenePos(){
    var data = new Float32Array(3);

    var nDataBytes = data.length * data.BYTES_PER_ELEMENT;
    var dataPtr = Module._malloc(nDataBytes);

    var dataHeap = new Uint8Array(Module.HEAPU8.buffer, dataPtr, nDataBytes);
    dataHeap.set(new Uint8Array(data.buffer));

    Module._getScenePos(dataHeap.byteOffset);

    var pos = new Float32Array(dataHeap.buffer, dataHeap.byteOffset, data.length);
    console.log(pos);

    Module._free(dataHeap.byteOffset);
}

async function setModelDisplay(displayID, type){
    console.log("Selected Display ID " + displayID);
    if (type == "creature"){
        const response = await fetch("/dbc/peek/CreatureDisplayInfo/?build=" + Current.buildName + "&col=ID&val=" + displayID);
        const cdiRow = await response.json();

        if (Object.keys(cdiRow.values).length == 0)
            return;

        // Textures
        setModelTexture([cdiRow.values['TextureVariationFileDataID[0]'], cdiRow.values['TextureVariationFileDataID[1]'], cdiRow.values['TextureVariationFileDataID[2]'], cdiRow.values['TextureVariationFileDataID[3]']], 11);

        // Particle colors
        if (cdiRow.values['ParticleColorID'] != 0){
            const particleResponse = await fetch("/dbc/peek/ParticleColor/?build=" + Current.buildName + "&col=ID&val=" + cdiRow.values['ParticleColorID']);
            const particleRow = await particleResponse.json();
            console.log(particleRow);
            Module._resetReplaceParticleColor();
            Module._setReplaceParticleColors(
                particleRow.values["Start[0]"], particleRow.values["Start[1]"], particleRow.values["Start[2]"],
                particleRow.values["MID[0]"], particleRow.values["MID[1]"], particleRow.values["MID[2]"],
                particleRow.values["End[0]"], particleRow.values["End[1]"], particleRow.values["End[2]"]
            );
        } else {
            Module._resetReplaceParticleColor();
        }

        const cmdResponse = await fetch("/dbc/peek/CreatureModelData/?build=" + Current.buildName + "&col=ID&val=" + cdiRow.values['ModelID']);
        const cmdRow = await cmdResponse.json();
        // TODO: Model scale? Anything else from CMD?

        // Geosets
        Current.enabledGeosets = [];

        if (cmdRow.values.CreatureGeosetDataID != "0"){
            const geosetResponse = await fetch("/dbc/find/CreatureDisplayInfoGeosetData/?build=" + Current.buildName + "&col=CreatureDisplayInfoID&val=" + cdiRow.values['ID']);
            const geosetResults = await geosetResponse.json();
            const geosetsToEnable = [];
            
            console.log("CreatureDisplayInfoGeosetData results", geosetResults);

            for (const geosetRow of geosetResults) {
                console.log("Setting " + (Number(geosetRow.GeosetIndex) + 1) + " to " + geosetRow.GeosetValue);
                geosetsToEnable[Number(geosetRow.GeosetIndex) + 1] = Number(geosetRow.GeosetValue);
            }

            // Wait for meshIdArrayCallback to set Current.availableGeosets
            if (Current.geosetsDone == false) {
                while (!Current.geosetsDone) {
                    console.log("Available geosets not loaded yet, waiting 10ms..");
                    await new Promise(resolve => setTimeout(resolve, 10));
                }
            }

            console.log("Geosets to enable: ", geosetsToEnable);

            // Make list of geoset groups first so we don't run through each geoset option for each dropdown.
            const availableGeosetGroups = [];
            for (const geoset of Current.availableGeosets) {
                const geosetGroup = Math.floor(Number(geoset / 100));

                if (!(geosetGroup in availableGeosetGroups)) {
                    availableGeosetGroups.push(geosetGroup);
                }
            }

            console.log("Available geoset groups: ", availableGeosetGroups);

            // Run through each group and make sure options are synced up with reality.
            for (const geosetGroup of availableGeosetGroups){
                var option = document.getElementById("geosetSelection-" + geosetGroup);

                if (!(geosetGroup in geosetsToEnable)) {
                    geosetsToEnable[geosetGroup] = 0;
                    option.value = 0;
                } else {
                    option.value = geosetsToEnable[geosetGroup];
                }
            }

            Current.enabledGeosets = geosetsToEnable;

            updateEnabledGeosets();
        }
    } else if (type == "item"){
        const response = await fetch("/dbc/peek/ItemDisplayInfo/?build=" + Current.buildName + "&col=ID&val=" + displayID);
        const idiRow = await response.json();
        const displayMatResponse = await fetch("/dbc/find/ItemDisplayInfoModelMatRes/?build=" + Current.buildName + "&col=ItemDisplayInfoID&val=" + displayID);
        const displayMatResults = await displayMatResponse.json();
        const typedArray = new Int32Array(NUM_TEXTURE_SLOTS);
        
        for (const displayMatRow of displayMatResults){
            if (displayMatRow.ModelIndex == "1")  // I think ModelIndex of 1 is for opposite shoulder appearances, etc. Could probably support it somehow, but ignoring it for now.
              continue;
          
            const materialResponse = await fetch("/dbc/peek/TextureFileData/?build=" + Current.buildName + "&col=MaterialResourcesID&val=" + displayMatRow.MaterialResourcesID);
            const materialJson = await materialResponse.json();
            if (materialJson.values.FileDataID != undefined){
               if (typedArray.length <= Number(displayMatRow.TextureType)) {
                 console.log("TextureType of " + displayMatRow.TextureType + " encountered, which is greater than the max texture array index ("+(typedArray.length-1)+").");
                 continue;
               }
              typedArray[Number(displayMatRow.TextureType)] = materialJson.values.FileDataID;
            }
        }
        setModelTextures(typedArray);
        
        // Particle colors
        if (idiRow.values['ParticleColorID'] != 0){
            const particleResponse = await fetch("/dbc/peek/ParticleColor/?build=" + Current.buildName + "&col=ID&val=" + idiRow.values['ParticleColorID']);
            const particleRow = await particleResponse.json();
            console.log(particleRow);
            Module._resetReplaceParticleColor();
            Module._setReplaceParticleColors(
                particleRow.values["Start[0]"], particleRow.values["Start[1]"], particleRow.values["Start[2]"],
                particleRow.values["MID[0]"], particleRow.values["MID[1]"], particleRow.values["MID[2]"],
                particleRow.values["End[0]"], particleRow.values["End[1]"], particleRow.values["End[2]"]
            );
        } else {
            Module._resetReplaceParticleColor();
        }
    }
}

function updateEnabledGeosets(){
    var nDataBytes = Current.enabledGeosets.length;
    var dataPtr = Module._malloc(nDataBytes);

    var dataHeap = new Int8Array(Module.HEAP8.buffer, dataPtr, nDataBytes);
    dataHeap.set(new Int8Array(Current.enabledGeosets));

    Module._setMeshIdArray(dataHeap.byteOffset, Current.enabledGeosets.length);
}

function setModelTexture(textures, offset){
    //Create real texture replace array
    const typedArray = new Int32Array(NUM_TEXTURE_SLOTS);

    for (let i = 0; i < textures.length; i++){
        if (offset == 11 && i == 3)
            typedArray[5] = textures[i];
        else if (offset == 2 && i == 3)
            typedArray[24] = textures[i];
        else
            typedArray[offset + i] = textures[i];
    }
    
    setModelTextures(typedArray);
}

function setModelTextures(typedArray){
    // Takes an array with values for all texture slots

    for (let i = 0; i < typedArray.length; i++) {
        let texElement = document.getElementById('tex' + i);
        if (texElement) {
            if (typedArray[i] != 0)
                texElement.value = typedArray[i];
            else if (texElement.value != "")
                texElement.value = "";
        }
    }

    // Allocate some space in the heap for the data (making sure to use the appropriate memory size of the elements)
    let buffer = Module._malloc(typedArray.length * typedArray.BYTES_PER_ELEMENT);

    // Assign the data to the heap - Keep in mind bytes per element
    Module.HEAP32.set(typedArray, buffer >> 2);
    Module._setTextures(buffer, typedArray.length);
    Module._free(buffer);
}

function updateURLs(){
    var url = "/casc/fname?filename=";
    let urlFileId = "/casc/fdid?filedataid=";

    var ptrUrl = allocateUTF8(url);
    var ptrUrlFileDataId = allocateUTF8(urlFileId);

    Module._setNewUrls(ptrUrl, ptrUrlFileDataId);

    _free(ptrUrl);
    _free(ptrUrlFileDataId);
}

function exportScene(){
    if (Current.type == "m2"){
        Module._startExport();
    }
}

(function() {
    $('#wowcanvas').bind('contextmenu', function(e){
        return false;
    });


    // Skip further initialization in embedded mode
    if (embeddedMode){
        return;
    }

    loadSettings();

    Elements.table = $('#mvfiles').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/listfile/datatables",
            "data": function ( d ) {
                return $.extend( {}, d, {
                    "src": "mv",
                    //"showADT": $("#showADT").is(":checked"),
                    "showWMO": $("#showWMO").is(":checked"),
                    "showM2": $("#showM2").is(":checked")
                } );
            }
        },
        "pageLength": 30,
        "autoWidth": false,
        "orderMulti": false,
        "ordering": true,
        "order": [[0, 'asc']],
        layout: {
            topStart: null,
            topEnd: null,
            bottomStart: null,
            bottomEnd: 'inputPaging'
        },
        "searching": true,
        "columnDefs":
        [
            {
                "targets": 0,
                "orderable": false,
                "visible": false
            },
            {
                "targets": 1,
                "orderable": false,
                "createdCell": function (td, cellData, rowData, row, col) {
                    if (!cellData && !rowData[7]) {
                        $(td).css('background-color', '#ff5858');
                        $(td).css('color', 'white');
                    }
                },
                "render": function ( data, type, full, meta ) {
                    if (full[1]) {
                        var test = full[1].replace(/^.*[\\\/]/, '');
                    } else {
                        if (!full[4]){
                            full[4] = "unk";
                        }
                        if (full[7]){
                            var test = full[7].replace(/^.*[\\\/]/, '');
                        } else {
                            var test = "Unknown filename (Type: " + full[4] + ", ID " + full[0] + ")";
                        }
                    }

                    return test;
                }
            }
        ],
        "language": {
            search: "",
            searchPlaceholder: "Search"
        }
    });

    $(".filterBox").on('change', function(){
        Elements.table.ajax.reload();
    });

    $('#mvfiles_search').on('input', function(){
        Elements.table.search($(this).val()).draw();
    });
}());