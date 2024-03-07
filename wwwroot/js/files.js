$("#files").on('click', '.fileTableDL', function(e){
    if (e.altKey || e.shiftKey){
        showButtons();
        addFileToDownloadQueue(this.href);
        event.preventDefault();
    }
});

$("#multipleFileAddAll").on('click', function(e){
    queueAllFiles();
    event.preventDefault();
});

$("#multipleFileResetButton").on('click', function(e){
    resetQueue();
    event.preventDefault();
});

document.addEventListener("DOMContentLoaded", function(event) {
    if (localStorage.getItem('queue[fdids]')){
        updateButton();
        showButtons();
    }
});

function togglePreviewPane(){

    var visibility = document.getElementById("files_preview").style.display;
    if ($("#files_preview").is(":visible")){
        // Refresh table to rewrite the preview links
        $('#files').DataTable().draw(false);

        // Hide preview pane
        document.getElementById("files_preview").style.display = "none";

        // Resize table
        document.getElementById("files_wrapper").style.width = "100%";

        document.getElementById("files_wrapper").style.float = "none";

        // Show footer
        $("footer").show();
    } else {
        // Clear window to stop currently playing sound/models
        document.getElementById("files_preview").innerHTML = "";

        // Refresh table to rewrite the preview links
        $('#files').DataTable().draw(false);

        // Show preview pane
        document.getElementById("files_preview").style.display = "block";

        // Resize table
        if ($("#files_tree").is(":visible")){
            document.getElementById("files_wrapper").style.width = "45%";
        } else {
            document.getElementById("files_wrapper").style.width = "55%";
        }

        document.getElementById("files_wrapper").style.float = "left";

        // Hide footer
        $("footer").hide();
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
    $("#multipleFileDLButton").show();
    $("#multipleFileAddAll").show();
    $("#multipleFileResetButton").show();
}

function updateButton(){
    var fdids = localStorage.getItem('queue[fdids]').split(',');
    $("#multipleFileDLButton").text('Download selected files (' + fdids.length + ')');
    $("#multipleFileDLButton").attr('href', '/casc/zip/fdids?ids=' + localStorage.getItem('queue[fdids]') + '&filename=selection.zip');
}

function resetQueue(){
    localStorage.removeItem('queue[buildconfig]');
    localStorage.removeItem('queue[cdnconfig]');
    localStorage.removeItem('queue[fdids]');
    $("#multipleFileDLButton").popover('hide');
    $("#multipleFileDLButton").hide();
    $("#multipleFileAddAll").hide();
    $("#multipleFileResetButton").hide();
}

function queueAllFiles(){
    $(".fileTableDL").each(function(){
        addFileToDownloadQueue(this.href);
    });
}

function showDifferentBuildWarning(){
    $("#multipleFileDLButton").popover({
        title: 'Warning',
        placement: 'bottom',
        content: '<p>You have files from different builds selected, this is currently not supported. Files will be retrieved from only the first build you selected.</p>',
        html: true
    });
    $("#multipleFileDLButton").popover('show');
}

function applyBuildFilter(build){
    if (build == undefined){
        build = "";
    }
    $.ajax("/files/scripts/api.php?switchbuild=" + build)
        .always(function() {
            $('#files').DataTable().ajax.reload();
        });

    updateBuildFilterButton();
}

function buildFilterClick(){
    if (window.rootFiltering){
        window.rootFiltering = false;
        applyBuildFilter();
    } else {
        window.rootFiltering = true;
        applyBuildFilter($("#buildFilter").val());
    }
}

function updateBuildFilterButton(){
    if (window.rootFiltering){
        $("#buildFilterButton").hide();
        $("#clearBuildFilterButton").show();
    } else {
        $("#buildFilterButton").show();
        $("#clearBuildFilterButton").hide();
    }
}

function fillModal(fileDataID){
    $("#moreInfoModalContent").load("/casc/moreInfo?filedataid=" + fileDataID, function () {
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
    } else if (type == "m2" || type == "wmo") {
        html += "<iframe style=\"border:0px;width:100%;min-height: 75vh\" src=\"/mv/?embed=true&filedataid=" + filedataid + "&type=" + type + "\"></iframe>";
        if (type == "m2") {
            html += "<div class='modal-mvlink' style='text-align:right;'><a href='/mv/?filedataid=" + filedataid + "' target='_blank'>Open in modelviewer</a></div>";
        }else if(type == "wmo"){
            html += "<div class='modal-mvlink' style='text-align:right;'><a href='/mv/?filedataid=" + filedataid + "&type=wmo' target='_blank'>Open in modelviewer</a></div>";
        }
    } else if (type == "xml" || type == "lua" || type == "txt" || type == "srt" || type == "toc") {
        fetch(url).then((response) => response.text()).then((text) => {
            document.getElementById('codeHolder').innerHTML = text;
            });
        html += "<pre style='max-height: 80vh'><code id='codeHolder'></code></pre>";
    }

    if ($("#files_preview").is(":visible")){
        $("#files_preview").html(html);
        return false;
    } else {
        $( "#previewModalContent" ).html(html);
    }
}

function fillPreviewModalByContenthash(buildconfig, filedataid, contenthash){
    if ($("#files_preview").is(":visible")){
        $( "#files_preview" ).load( "/files/scripts/preview_api.php?buildconfig=" + buildconfig + "&filedataid=" + filedataid + "&contenthash=" + contenthash);
    } else {
        $( "#previewModalContent" ).load( "/files/scripts/preview_api.php?buildconfig=" + buildconfig + "&filedataid=" + filedataid + "&contenthash=" + contenthash);
    }
}

function fillDiffModal(from, to, filedataid){
    $( "#previewModalContent" ).load( "/casc/diffFile?from=" + from + "&to=" + to + "&filedataid=" + filedataid);
}

function fillDiffModalRaw(from, to, filedataid){
    $( "#previewModalContent" ).load( "/files/diff.php?from=" + from + "&to=" + to + "&filedataid=" + filedataid + "&raw=1");
}

function fillDBCDiffModal(from, to, dbc) {
    $("#previewModalContent" ).html( "<iframe src=\"/dbc/diff.html?embed=1&dbc=" + dbc + "&old=" + from + "&new=" + to + "\" style='width: 100%; height: 80vh'></iframe>");
}

function fillChashModal(contenthash){
    $( "#chashModalContent" ).load( "/casc/samehashes?chash=" + contenthash);
}

function fillSkitModal(skitid){
    $( "#moreInfoModalContent" ).load( "/files/sounds.php?embed=1&skitid=" + skitid );
}

$("html").on('hidden.bs.modal', '#moreInfoModal', function(e) {
    console.log("Clearing modal");
    $( "#moreInfoModalContent" ).html( '<i class="fa fa-refresh fa-spin" style="font-size:24px"></i>' );
})

$("html").on('hidden.bs.modal', '#previewModal', function(e) {
    console.log("Clearing modal");
    $( "#previewModalContent" ).html( '<i class="fa fa-refresh fa-spin" style="font-size:24px"></i>' );
})

$("html").on('hidden.bs.modal', '#chashModal', function(e) {
    console.log("Clearing modal");
    $( "#chashModalContent" ).html( '<i class="fa fa-refresh fa-spin" style="font-size:24px"></i>' );
})

$(function () {
    $('[data-toggle="popover"]').popover();

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
            $("#moreInfoModal").modal('show');
        }
    }
})

function toggleTree(forceHide = false){
    if ($("#files_tree").is(":visible") || forceHide){
        $("#files_tree").hide();
        $("#files_treeFilter").hide();
        document.getElementById('files_treetoggle').classList.add("collapsed");
        document.getElementById('files_buttons').classList.remove("tree");
        document.getElementById('files_buttons').classList.add("notree");
        $("#files_tree").html("<div id='tree'></div>");
        $("#files_treetoggle").html("&gt;");
    } else {
        treeClick(document.getElementById("tree"));
        document.getElementById('files_treetoggle').classList.remove("collapsed");
        document.getElementById('files_buttons').classList.remove("notree");
        document.getElementById('files_buttons').classList.add("tree");
        $("#files_treeFilter").show();
        $("#files_tree").show();
        $("#files_treetoggle").html("&lt;");
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