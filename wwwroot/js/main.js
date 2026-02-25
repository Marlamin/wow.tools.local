/*!
 * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
 * Copyright 2011-2024 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 */
const getStoredTheme = () => localStorage.getItem('theme')
const setStoredTheme = theme => localStorage.setItem('theme', theme)

const getPreferredTheme = () => {
    const storedTheme = getStoredTheme()
    if (storedTheme) {
        return storedTheme
    }

    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}

const setTheme = theme => {
    if (theme === 'auto') {
        document.documentElement.setAttribute('data-bs-theme', (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'))
    } else {
        document.documentElement.setAttribute('data-bs-theme', theme)
    }

    document.querySelectorAll('[data-bs-theme-value]').forEach(toggle => {
        const forTheme = toggle.getAttribute('data-bs-theme-value')
        if (forTheme == theme) {
            toggle.classList.add("active")
        } else {
            toggle.classList.remove("active")
        }
    }
    );
}

window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
    const storedTheme = getStoredTheme()
    if (storedTheme !== 'light' && storedTheme !== 'dark') {
        setTheme(getPreferredTheme())
    }
})

/* multiple modal scroll fix */
document.addEventListener('DOMContentLoaded', function() {
    document.addEventListener('hidden.bs.modal', function (e) {
        if (e.target.classList.contains('modal')) {
            const visibleModals = document.querySelectorAll('.modal.show');
            if (visibleModals.length > 0) {
                document.body.classList.add('modal-open');
            }
        }
    });

    fetch("/header.html")
        .then(response => response.text())
        .then(html => {
            document.getElementById("navbar").innerHTML = html;
            updateTitle();
            checkForUpdates();
            setTheme(getPreferredTheme());
        });

    $(document).on('init.dt', function (e, settings) {
        const pageInput = document.querySelector(".dt-paging-input input");
        if (pageInput) {
            pageInput.addEventListener("keydown", (event) => {
                let intValue = parseInt(pageInput.value);
                if (event.key == "ArrowRight") {
                    pageInput.value = intValue + 1;
                } else if (event.key == "ArrowLeft" && intValue > 1) {
                    pageInput.value = intValue - 1;
                }

                pageInput.dispatchEvent(new Event('input', { bubbles: true }));
            });
        }
    });
});

function themeClick(theme) {
    setStoredTheme(theme)
    setTheme(theme)
}

function shouldShowButts() {
    var max = 100;
    var min = 1;
    var luckyNumber = 42;
    return Math.floor(Math.random() * max) + min == luckyNumber;
}

async function updateTitle() {
    var title = "";

    if (shouldShowButts()) {
        title += "butt";
    } else {
        title += "but";
    }

    if (window.location.hostname === "localhost") {
        title += " local";
    } else {
        title += " not local";
    }

    document.getElementById("nocog").innerHTML = "<img src='/img/w.svg' alt='Logo W'><img src='/img/w.svg' alt='Logo W'><span>.tools <small><i>" + title + "</i></small></span>";
}

async function checkForUpdates(force = false) {
    const currentVersionResponse = await fetch("/casc/getVersion");
    const currentVersion = await currentVersionResponse.text();

    const lastUpdateCheck = localStorage.getItem("lastUpdate");
    if (lastUpdateCheck != null && !force) {
        const json = JSON.parse(lastUpdateCheck);
        if (json.lastCheck > Date.now() - 24 * 60 * 60 * 1000) {
            let updateAvailable = json.latestVersion != currentVersion;
            newUpdateAvailable(updateAvailable);
            return;
        }
    }

    const latestReleaseResponse = await fetch("https://api.github.com/repos/marlamin/wow.tools.local/releases/latest");
    const latestRelease = await latestReleaseResponse.json();
    const latestReleaseTag = latestRelease.tag_name + ".0";

    var updateData = new Object();
    updateData.updateAvailable = true;
    updateData.latestVersion = latestReleaseTag;
    updateData.lastCheck = Date.now();
    localStorage.setItem("lastUpdate", JSON.stringify(updateData));

    if (latestReleaseTag !== currentVersion) {
        newUpdateAvailable(true);
    } else {
        newUpdateAvailable(false);
    }
}

function newUpdateAvailable(isUpdateAvailable) {
    var navBar = document.getElementsByTagName("nav");
    var updateDiv = document.createElement("div");
    const lastUpdateCheck = localStorage.getItem("lastUpdate");

    updateDiv.id = 'updateDiv';
    if (isUpdateAvailable) {
        updateDiv.innerHTML = "<i class='fa fa-exclamation-circle' style='color: red'></i> <a href='https://github.com/marlamin/wow.tools.local/releases' target='_BLANK'>An update to version " + JSON.parse(lastUpdateCheck).latestVersion + " is available!</a> <a href='#' onClick='forceUpdateCheck()'><i class='fa fa-refresh'></i></a>";
    } else {
        updateDiv.innerHTML = "<i class='fa fa-check-circle' style='color: green;'></i> Up to date. <a style='cursor: pointer' onClick='forceUpdateCheck()'><i class='fa fa-refresh'></i></a>";
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