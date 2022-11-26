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
});

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

function renderBLPToCanvasElement(url, elementID, canvasX, canvasY) {
    return fetch(url)
        .then(function(response) {
            return response.arrayBuffer();
        })
        .then(function(arrayBuffer) {
            let data = new Bufo(arrayBuffer);
            let blp = new BLPFile(data);
            let canvas = document.getElementById(elementID);
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