document.getElementById("files")?.addEventListener('click', function(e){
    if (e.target.classList.contains('fileTableDL')){
        if (e.altKey || e.shiftKey){
            showButtons();
            addFileToDownloadQueue(e.target.href);
            e.preventDefault();
        }
    }
});

document.getElementById("multipleFileAddAll")?.addEventListener('click', function(e){
    queueAllFiles();
    e.preventDefault();
});

document.getElementById("multipleFileResetButton")?.addEventListener('click', function(e){
    resetQueue();
    e.preventDefault();
});

document.addEventListener("DOMContentLoaded", function(event) {
    if (localStorage.getItem('queue[fdids]')){
        updateButton();
        showButtons();
    }
});

function togglePreviewPane(){

    var visibility = document.getElementById("files_preview").style.display;
    if (document.getElementById("files_preview").style.display !== "none"){
        // Refresh table to rewrite the preview links
        $('#files').DataTable().draw(false);

        // Hide preview pane
        document.getElementById("files_preview").style.display = "none";

        // Resize table
        document.getElementById("files_wrapper").style.width = "100%";

        document.getElementById("files_wrapper").style.float = "none";
    } else {
        // Clear window to stop currently playing sound/models
        document.getElementById("files_preview").innerHTML = "";

        // Refresh table to rewrite the preview links
        $('#files').DataTable().draw(false);

        // Show preview pane
        document.getElementById("files_preview").style.display = "block";

        // Resize table
        if (document.getElementById("files_tree").style.display !== "none"){
            document.getElementById("files_wrapper").style.width = "45%";
        } else {
            document.getElementById("files_wrapper").style.width = "55%";
        }

        document.getElementById("files_wrapper").style.float = "left";
    }
}

function addFileToDownloadQueue(file){
    // Parse URL
    var url = new URL(file);
    const urlParams = new URLSearchParams(url.search);
    //var buildConfig  = urlParams.get('buildconfig');
    //var cdnConfig = urlParams.get('cdnconfig');
    var fileDataID = urlParams.get('fileDataID');

    // Update localstorage
    //var currentBuildConfig = localStorage.getItem('queue[buildconfig]');
    //if (!currentBuildConfig){
    //    localStorage.setItem('queue[buildconfig]', buildConfig);
    //} 

    //var currentCDNConfig = localStorage.getItem('queue[cdnconfig]');
    //if (!currentCDNConfig){
    //    localStorage.setItem('queue[cdnconfig]', cdnConfig);
    //}

    var fdids = localStorage.getItem('queue[fdids]');
    if (!fdids){
        localStorage.setItem('queue[fdids]', fileDataID);
    } else {
        var fdidArr = fdids.split(',');
        if (!fdidArr.includes(fileDataID)){
            fdidArr.push(fileDataID);
            localStorage.setItem('queue[fdids]', fdidArr.join(','));
        }
    }

    updateButton();
}

function showButtons(){
    document.getElementById("multipleFileDLButton").style.display = "block";
    document.getElementById("multipleFileAddAll").style.display = "block";
    document.getElementById("multipleFileResetButton").style.display = "block";
}

function updateButton(){
    var fdids = localStorage.getItem('queue[fdids]').split(',');
    document.getElementById("multipleFileDLButton").textContent = 'Download selected files (' + fdids.length + ')';
    document.getElementById("multipleFileDLButton").setAttribute('href', '/casc/zip/fdids?ids=' + localStorage.getItem('queue[fdids]') + '&filename=selection.zip');
}

function resetQueue(){
    localStorage.removeItem('queue[buildconfig]');
    localStorage.removeItem('queue[cdnconfig]');
    localStorage.removeItem('queue[fdids]');
    document.getElementById("multipleFileDLButton").style.display = "none";
    document.getElementById("multipleFileAddAll").style.display = "none";
    document.getElementById("multipleFileResetButton").style.display = "none";
}

function queueAllFiles(){
    document.querySelectorAll(".fileTableDL").forEach(function(element){
        addFileToDownloadQueue(element.href);
    });
}
function fillModal(fileDataID){
    document.getElementById("moreInfoModalContent").innerHTML = "";
    fetch("/casc/moreInfo?filedataid=" + fileDataID)
        .then(response => response.text())
        .then(html => {
            document.getElementById("moreInfoModalContent").innerHTML = html;
            // document.getElementById('editableFilename').addEventListener("dblclick", makeEditable);
            // document.getElementById('editableFilename').addEventListener("blur", finishEditing);
        });
}

function relinkFile(fileDataID) {
    var button = document.getElementById('fileRelinkButton');
    button.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Recrawling, please wait...';
    button.disabled = true;
    button.classList.add('disabled');
    fetch("/casc/relinkFile?fileDataID=" + fileDataID)
        .then(response => response.text())
        .then(result => {
            fillModal(fileDataID);
            button.disabled = false;
            button.classList.remove('disabled');
        });
}

function makeEditable(){
    this.contentEditable = true;
    this.focus();
}

function finishEditing(){
    this.contentEditable = false;
    var currentSuggestions = localStorage.getItem('suggestionQueue');
    if (!currentSuggestions) {
        localStorage.setItem('suggestionQueue', this.dataset.id + ";" + this.innerText);
    }else{
        const currentItems = currentSuggestions.split("\n");
        let suggestionArray = new Array();
        for(const currentItem of currentItems){
            const splitName = currentItem.split(";");
            if(splitName[0] != this.dataset.id){
                suggestionArray.push(currentItem);
            }
        }

        suggestionArray.push(this.dataset.id + ";" + this.innerText);
        
        localStorage.setItem('suggestionQueue', suggestionArray.join('\n'));
    }
    console.log(this.innerText);
}

function fillPreviewModal(buildconfig, filedataid, type) {
    var html = "";
    var url = "/casc/fdid?fileDataID=" + filedataid + "&filename=preview";
    if (type == "blp") {
        html += "<canvas id='mapCanvas' width='1' height='1'></canvas>";
        renderBLPToCanvasElement(url, "mapCanvas", 0, 0, true);
    } else if (type == "mp3" || type == "ogg") {
        var mimeType = "";
        if (type == "mp3") {
            mimeType = "audio/mpeg";
        } else if (type == "ogg") {
            mimeType = "audio/ogg";
        }
        html += "<audio autoplay=\"\" controls=\"\"><source src=\"" + url + "\" type=\"" + mimeType + "\"></audio>";
    } else if (type == "m2" || type == "wmo" || type == "bls" || type == "gfat" || type == "m3" || type == "adt") {
        html += "<ul class=\"nav nav-tabs\" role=\"tablist\">";
        if (type == "m2" || type == "wmo" || type == "m3") {
            if (type == "m2" || type == "wmo") {
                html += "<li class=\"nav-item\"><a class=\"nav-link active\" id=\"modelviewer-tab\" data-bs-toggle=\"tab\" href=\"#modelviewer\" role=\"tab\" aria-controls=\"modelviewer\" aria-selected=\"true\">Modelviewer</a></li>";
                html += "<li class=\"nav-item\"><a class=\"nav-link\" id=\"modelinfo-tab\" data-bs-toggle=\"tab\" href=\"#modelinfo\" role=\"tab\" aria-controls=\"modelinfo\" aria-selected=\"false\">Model info</a></li>";
                html += "<li class=\"nav-item\"><a class=\"nav-link\" id=\"json-tab\" data-bs-toggle=\"tab\" href=\"#json\" role=\"tab\" aria-controls=\"json\" aria-selected=\"false\">JSON</a></li>";
            } else if (type == "m3") {
                html += "<li class=\"nav-item\"><a class=\"nav-link active\" id=\"modelviewer-tab\" data-bs-toggle=\"tab\" href=\"#modelviewer\" role=\"tab\" aria-controls=\"modelviewer\" aria-selected=\"true\">Modelviewer</a></li>";
                html += "<li class=\"nav-item\"><a class=\"nav-link \" id=\"json-tab\" data-bs-toggle=\"tab\" href=\"#json\" role=\"tab\" aria-controls=\"json\" aria-selected=\"false\">JSON</a></li>";

            }
        } else if (type == "bls" || type == "gfat" || type == "adt") {
            html += "<li class=\"nav-item\"><a class=\"nav-link active\" id=\"json-tab\" data-bs-toggle=\"tab\" href=\"#json\" role=\"tab\" aria-controls=\"json\" aria-selected=\"false\">JSON</a></li>";
        }

        html += "</ul>";
        html += "<div class=\"tab-content\">";
        if (type == "m2" || type == "wmo") {
            html += "<div class=\"tab-pane show active\" id=\"modelviewer\" role=\"tabpanel\" aria-labelledby=\"modelviewer-tab\">";
            html += "<iframe style=\"border:0px;width:100%;min-height: 75vh\" src=\"/mv/?embed=true&filedataid=" + filedataid + "&type=" + type + "\"></iframe>";
            if (type == "m2") {
                html += "<div class='modal-mvlink' style='text-align:right;'><a href='/mv/?filedataid=" + filedataid + "' target='_blank'>Open in modelviewer</a></div>";
            } else if (type == "wmo") {
                html += "<div class='modal-mvlink' style='text-align:right;'><a href='/mv/?filedataid=" + filedataid + "&type=wmo' target='_blank'>Open in modelviewer</a></div>";
            }
            html += "</div>";
        } else if (type == "m3") {
            html += "<p>Note: The M3 modelviewer is a work in progress, any textures you see have been manually mapped.</p>";
            html += "<div class=\"tab-pane show active\" id=\"modelviewer\" role=\"tabpanel\" aria-labelledby=\"modelviewer-tab\">";
            html += "<iframe style=\"border:0px;width:100%;min-height: 75vh\" src=\"/mv/m3.html?embed=true&filedataid=" + filedataid + "&type=" + type + "\"></iframe>";
            html += "<div class='modal-mvlink' style='text-align:right;'><a href='/mv/m3.html?filedataid=" + filedataid + "' target='_blank'>Open in modelviewer</a></div>";
            html += "</div>";
        }

        if (type == "m2" || type == "wmo" || type == "m3") {
            html += "<div class=\"tab-pane\" id=\"json\" role=\"tabpanel\" aria-labelledby=\"json-tab\">";
            html += "<pre style='max-height: 80vh' id='jsonHolder'></pre>";
            html += "</div>";
        } else if (type == "bls" || type == "gfat" || type == "adt") {
            html += "<div class=\"tab-pane active\" id=\"json\" role=\"tabpanel\" aria-labelledby=\"json-tab\">";
            html += "<pre style='max-height: 80vh' id='jsonHolder'></pre>";
            html += "</div>";
        }
      
        if (type == "m2" || type == "wmo" || type == "m3") {
            html += "<div class=\"tab-pane\" id=\"modelinfo\" role=\"tabpanel\" aria-labelledby=\"modelinfo-tab\">";
            html += "<div id='modelinfoHolder'></div>";
            html += "</div>";
        }
        html += "</div>";

        fetch("/casc/json?fileDataID=" + filedataid).then((response) => response.text()).then((text) => {
            document.getElementById('jsonHolder').innerHTML = text;
        });

        if (type == "wmo" || type == "m2" || type == "m3") {
            fetch("/model/info?fileDataID=" + filedataid).then((response) => response.text()).then((text) => {
                document.getElementById('modelinfoHolder').innerHTML = text;
            });
        }
    } else if (type == "lua" || type == "txt" || type == "srt" || type == "toc" || type == "hlsl") {
        fetch(url).then((response) => response.text()).then((text) => {
            document.getElementById('codeHolder').innerHTML = text;
        });
        html += "<pre style='max-height: 80vh'><code id='codeHolder'></code></pre>";
    } else if (type == "xml") {
                fetch(url).then((response) => response.text()).then((text) => {
            document.getElementById('codeHolder').innerHTML = text;
        });
        html += "<pre style='max-height: 80vh'><script type='text/plain' style='display: block' id='codeHolder'></script></pre>";
    }

    if (document.getElementById("files_preview").style.display !== "none") {
        document.getElementById("files_preview").innerHTML = html;
        return false;
    } else {
        document.getElementById("previewModalContent").innerHTML = html;
    }
}
function fillDiffModal(from, to, filedataid){
    fetch("/casc/diffFile?from=" + from + "&to=" + to + "&filedataid=" + filedataid)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById("previewModalContent");
            container.innerHTML = "";
            const fragment = document.createRange().createContextualFragment(html);
            container.appendChild(fragment);
        });
}

function fillDBCDiffModal(from, to, dbc) {
    document.getElementById("previewModalContent").innerHTML = "<iframe src=\"/dbc/diff.html?embed=1&dbc=" + dbc + "&old=" + from + "&new=" + to + "\" style='width: 100%; height: 80vh'></iframe>";
}

function fillChashModal(contenthash){
    fetch("/casc/samehashes?chash=" + contenthash)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById("chashModalContent");
            container.innerHTML = "";
            const fragment = document.createRange().createContextualFragment(html);
            container.appendChild(fragment);
        });

    document.getElementById('chashModalLabel').innerText = "Content hash lookup for hash " + contenthash;
}

$("html").on('hidden.bs.modal', '#moreInfoModal', function (e) {
    document.getElementById("moreInfoModalContent").innerHTML = '<i class="fa fa-refresh fa-spin" style="font-size:24px"></i>';
})

$("html").on('hidden.bs.modal', '#previewModal', function(e) {
    document.getElementById("previewModalContent").innerHTML = '<i class="fa fa-refresh fa-spin" style="font-size:24px"></i>';
})

$("html").on('hidden.bs.modal', '#chashModal', function(e) {
    document.getElementById("chashModalContent").innerHTML = '<i class="fa fa-refresh fa-spin" style="font-size:24px"></i>';
})

$(function () {
    let vars = {};
    window.location.hash.replace(/([^=&]+)=([^&]*)/gi, function(m, key, value) {
        if (key.includes('#')) {
            const splitString = key.split('#');
            vars[splitString[1]] = decodeURIComponent(value);
        } else {
            vars[key] = decodeURIComponent(value);
        }
    });

    if('search' in vars && 'fdidModal' in vars && vars['fdidModal'] == '1'){
        if(vars['search'].includes('fdid:')){
            let targetFDID = vars['search'].split(':')[1];
            fillModal(targetFDID);
            document.getElementById("moreInfoModal").style.display = "block";
        }
    }
})

function toggleTree(forceHide = false){
    if (document.getElementById("files_tree").style.display !== "none" || forceHide){
        document.getElementById("files_tree").style.display = "none";
        document.getElementById("files_treeFilter").style.display = "none";
        document.getElementById('files_treetoggle').classList.add("collapsed");
        document.getElementById('files_buttons').classList.remove("tree");
        document.getElementById('files_buttons').classList.add("notree");
        document.getElementById("files_tree").innerHTML = "<div id='tree'></div>";
        document.getElementById("files_treetoggle").innerHTML = "&gt;";
    } else {
        treeClick(document.getElementById("tree"));
        document.getElementById('files_treetoggle').classList.remove("collapsed");
        document.getElementById('files_buttons').classList.remove("notree");
        document.getElementById('files_buttons').classList.add("tree");
        document.getElementById("files_treeFilter").style.display = "block";
        document.getElementById("files_tree").style.display = "block";
        document.getElementById("files_treetoggle").innerHTML = "&lt;";
    }
}

function treeFilterChange(event){
    treeClick(document.getElementById("tree"), false);
    return false;
}

function treeClick(event, returnAfterClear = true){
    let parentElement = event.parentElement;
    if (parentElement === undefined)
        parentElement = event.srcElement;

    // Check if loading icon already exists for this div, if so return to not load things twice
    if (parentElement.querySelector('#loading') != null)
        return;

    let depth = 1;
    let url = '/files/scripts/api.php?tree=1&depth=' + depth;
    let start = parentElement.dataset.name;
    if (start !== undefined){
        depth = Number(parentElement.dataset.depth) + 1;
        url = '/files/scripts/api.php?tree=1&start=' + start + '&depth=' + depth;
    }

    let filter = document.getElementById('treeFilter').value;
    if (filter !== undefined && filter != ''){
        url = url + "&filter=" + filter;
    }

    if (start !== undefined){
        if (filter != ''){
            $('#files').DataTable().search("^" + start + "%," + filter).draw();
        } else {
            $('#files').DataTable().search("^" + start + "%").draw();
        }
    }

    $(".selected").removeClass("selected");
    $(parentElement).addClass('selected');

    // If children exist, delete children and collapse folder.
    if (parentElement.querySelector('.treeEntry') != null){
        parentElement.querySelectorAll('.treeEntry').forEach(function(child){
            child.remove();
        });

        if (returnAfterClear){
            return;
        }
    }

    // Create loading icon
    var newElement = document.createElement('div');
    newElement.id = 'loading';
    newElement.style.float = 'right';
    newElement.innerHTML = '<i class="fa fa-spinner fa-spin"></i>';
    parentElement.appendChild(newElement);

    // Fire off API request
    fetch(url)
        .then(response => response.json())
        .then(result => {
            // Remove loading icon
            if (parentElement.querySelector('#loading') != null){
                parentElement.querySelector('#loading').remove();
            }

            // Check if filter value changed at this point, if so bail and restart
            if (filter != document.getElementById('treeFilter').value){
                return treeClick(event, returnAfterClear);
            }

            result.forEach(function(entry){
                // Ignore entry if entry is a file
                if (entry.entry == entry.filename)
                    return;

                let newElement = document.createElement('div');
                newElement.textContent = entry.entry.replace(start + '/', '');
                newElement.dataset.depth = depth;
                newElement.dataset.name = entry.entry;
                newElement.classList = ['treeEntry'];
                newElement.addEventListener('click', treeClick);

                parentElement.append(newElement);
            });

            $(".treeEntry").first().css("margin-top", "40px");
        });
}

async function exportTACTKeys() {
    var button = document.getElementById("exportTACTKeysButton");
    var beforeText = button.innerHTML;

    button.innerHTML = "<i class='fa fa-spin fa-refresh'></i> Exporting, please wait!";

    await fetch("/casc/exportTACTKeys");

    // Make it last long enough to show something happened, lol
    await new Promise(resolve => setTimeout(resolve, 100));

    button.innerHTML = beforeText;
}

async function updateTACTKeys() {
    var button = document.getElementById("updateTACTKeysButton");
    button.innerHTML = "<i class='fa fa-spin fa-refresh'></i> Updating, please wait!";

    await fetch("/casc/updateTACTKeys");
    window.location.reload();
}

function checkFiles() {
    var button = document.getElementById("checkFilesButton");
    button.innerHTML = "<i class='fa fa-spin fa-refresh'></i> Checking, please wait!";
    var filesToCheck = document.getElementById("filesToCheck").value;
    fetch("/casc/checkFiles", {
        method: "POST",
        body: filesToCheck
    })
    .then(response => response.text())
    .then(result => {
        document.getElementById("result").innerHTML = result;
        button.innerHTML = "Check files";
    });
}