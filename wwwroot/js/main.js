var theme = localStorage.getItem('theme');
if (!theme){
    localStorage.setItem('theme', 'dark');
    theme = 'dark';
    updateCSSVars(theme);
} else {
    if (theme == 'light'){
        updateCSSVars('light');
    } else {
        updateCSSVars('dark');
    }
}

function updateCSSVars(theme){
    if (theme == 'dark'){
        document.documentElement.style.setProperty('--background-color', '#343a40');
        document.documentElement.style.setProperty('--text-color', '#fff');
        document.documentElement.style.setProperty('--hover-color', '#fff');
        document.documentElement.style.setProperty('--diff-added-color', '#368a23');
        document.documentElement.style.setProperty('--diff-removed-color', '#9b0d0d');
        document.documentElement.style.setProperty('--table-header-color', '#272727');
    } else if (theme == 'light'){
        document.documentElement.style.setProperty('--background-color', '#f8f9fa');
        document.documentElement.style.setProperty('--text-color', '#000000');
        document.documentElement.style.setProperty('--hover-color', '#7e7e7e');
        document.documentElement.style.setProperty('--diff-added-color', '#e6ffe6');
        document.documentElement.style.setProperty('--diff-removed-color', '#ffe6e6');
        document.documentElement.style.setProperty('--table-header-color', '#dee2e6');
    }
}

/* multiple modal scroll fix */
$(function() {
    $('.modal').on("hidden.bs.modal", function (e) {
        if ($('.modal:visible').length) {
            $('body').addClass('modal-open');
        }
    });

    checkForUpdates();
});

async function checkForUpdates(force = false) {
    if (document.cookie && !force) {
        newUpdateAvailable(JSON.parse(document.cookie).updateAvailable);
        return;
    }

    const latestReleaseResponse = await fetch("https://api.github.com/repos/marlamin/wow.tools.local/releases/latest");
    const latestRelease = await latestReleaseResponse.json();
    const latestReleaseTag = latestRelease.tag_name + ".0";

    const currentVersionResponse = await fetch("/casc/getVersion");
    const currentVersion = await currentVersionResponse.text();
    
    if (latestReleaseTag !== currentVersion) {
        var cookieData = new Object();
        cookieData.updateAvailable = true;
        cookieData.latestVersion = latestReleaseTag;
        document.cookie = JSON.stringify(cookieData);
        newUpdateAvailable(true);
    } else {
        var cookieData = new Object();
        cookieData.updateAvailable = false;
        cookieData.latestVersion = latestReleaseTag;
        document.cookie = JSON.stringify(cookieData);
        newUpdateAvailable(false);
    }
}

function newUpdateAvailable(isUpdateAvailable) {
    var navBar = document.getElementsByTagName("nav");
    var updateDiv = document.createElement("div");
    updateDiv.id = 'updateDiv';
    if (isUpdateAvailable) {
        updateDiv.innerHTML = "<i class='fa fa-exclamation-circle' style='color: red'></i> <a href='https://github.com/marlamin/wow.tools.local/releases' target='_BLANK'>An update to version " + JSON.parse(document.cookie).latestVersion + " is available!</a> <a href='#' onClick='forceUpdateCheck()'><i class='fa fa-refresh'></i></a>";
    } else {
        updateDiv.innerHTML = "<i class='fa fa-check-circle' style='color: green'></i> Up to date. <a href='#' onClick='forceUpdateCheck()'><i class='fa fa-refresh'></i></a>";
    }
    navBar[0].appendChild(updateDiv);
}

function forceUpdateCheck() {
    var element = document.getElementById("updateDiv");
    element.parentNode.removeChild(element);
    checkForUpdates(true);
}

function renderBLPToIMGElement(url, elementID){
    fetch(url).then(function(response) {
        return response.arrayBuffer();
    }).then(function(arrayBuffer) {
        let data = new Bufo(arrayBuffer);
        let blp = new BLPFile(data);

        let canvas = document.createElement('canvas');
        canvas.width = blp.width;
        canvas.height = blp.height;

        let image = blp.getPixels(0, canvas);

        let img = document.getElementById(elementID);
        if (!img){
            console.log("Target image element does not exist: " + elementID);
            return;
        }
        img.src = canvas.toDataURL();
        img.setAttribute('data-loaded', true);
    });
}

function renderBLPToCanvasElement(url, elementID, canvasX, canvasY, resize = false) {
    return fetch(url)
        .then(function (response) {
            return response.arrayBuffer();
        })
        .then(function (arrayBuffer) {
            let data = new Bufo(arrayBuffer);
            let blp = new BLPFile(data);
            let canvas = document.getElementById(elementID);

            if (resize) {
                canvas.width = blp.width;
                canvas.height = blp.height;
            }
            let image = blp.getPixels(0, canvas, canvasX, canvasY);
        });
}

function renderBLPToCanvas(url, canvas, canvasX, canvasY) {
    return fetch(url)
        .then(function(response) {
            return response.arrayBuffer();
        })
        .then(function(arrayBuffer) {
            let data = new Bufo(arrayBuffer);
            let blp = new BLPFile(data);
            let image = blp.getPixels(0, canvas, canvasX, canvasY);
        });
}