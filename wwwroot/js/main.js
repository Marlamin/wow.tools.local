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

    var navbar = document.getElementById("navbar");

    if (navbar) {
        fetch("/header.html")
            .then(response => response.text())
            .then(html => {
                document.getElementById("navbar").innerHTML = html;
                updateTitle();
                checkForUpdates();
                setTheme(getPreferredTheme());
            });
    }
 

    document.addEventListener('init.dt', function (e) {
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
    setStoredTheme(theme);
    setTheme(theme);
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
            newUpdateAvailable(updateAvailable, currentVersion, json.latestVersion);
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
        newUpdateAvailable(true, currentVersion, latestRelease.tag_name);
    } else {
        newUpdateAvailable(false, currentVersion, latestRelease.tag_name);
    }
}

function newUpdateAvailable(isUpdateAvailable, currentVersion, latestVersion) {
    var navBar = document.getElementsByTagName("nav");
    var updateDiv = document.createElement("div");
    updateDiv.id = 'updateDiv';

    if (isUpdateAvailable) {
        updateDiv.innerHTML = "<button style='margin-left: 5px;' onclick='window.location.href=\"https://github.com/marlamin/wow.tools.local/releases\"' title='Update available' class='btn btn-danger active align-items-center'><i class='fa fa-download'></i> " + latestVersion + "</button>";
    } else {
        updateDiv.innerHTML = "<button style='margin-left: 5px' onClick='forceUpdateCheck()' title='Check for updates' class='btn active align-items-center'><i class='fa fa-refresh'></i> Check</button>";
    }
    navBar[0].appendChild(updateDiv);
}

function forceUpdateCheck() {
    var element = document.getElementById("updateDiv");
    element.parentNode.removeChild(element);
    checkForUpdates(true);
}

function renderBLPToCanvasElement(url, elementID, canvasX, canvasY, resize = false, discardAlpha = false) {
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
            let image = blp.getPixels(0, canvas, discardAlpha);
        });
}
