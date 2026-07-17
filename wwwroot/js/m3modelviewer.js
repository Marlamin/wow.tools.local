import * as THREE from 'three';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
function showError(error) {
    const errorsDiv = document.getElementById("errors");
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error alert alert-danger';
    errorDiv.innerHTML = error;
    errorsDiv.appendChild(errorDiv);
}

window.onerror = function(message, source, lineno, colno, error) {
    showError("An error occured! You might have to force reload the page with <kbd>CTRL-F5</kbd>.<br>If it persists, you may be viewing a model not compatible with the modelviewer.");
}

var Elements =
{
    Sidebar: document.getElementById('js-sidebar'),
    Counter: document.getElementById('fpsLabel'),
    EventLabel: document.getElementById('eventLabel'),
    DownloadLabel: document.getElementById('downloadLabel'),
    ModelControl: document.getElementById('js-model-control'),
    MVContainer: document.getElementById('js-mv-container')
};


var Current =
{
    fileDataID: 6648661,
    type: "m3",
    embedded: false,
}
THREE.Object3D.DEFAULT_UP = new THREE.Vector3(1, 0, 0);
const scene = new THREE.Scene();
const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);

const renderer = new THREE.WebGLRenderer();
renderer.setSize(window.innerWidth, window.innerHeight);
Elements.MVContainer.appendChild(renderer.domElement);
const controls = new OrbitControls(camera, renderer.domElement);

function loadModel(filedataid) {
    clearScene();

    var modelJSON = fetch("/casc/json?fileDataID=" + filedataid);
    modelJSON.then(response => {
        return response.json();
    }).then(data => {
        console.log(data);

        const instances = data.Instances;

        const textureArray = [];
        for (let instanceIndex = 0; instanceIndex < instances.Instances.length; instanceIndex++) {
            const instance = instances.Instances[instanceIndex];
            const textureLoader = new THREE.TextureLoader();

            if (instance.shaderData.SamplerTextureFileIDs == undefined)
                continue;

            var textureFDID = instance.shaderData.SamplerTextureFileIDs[instance.shaderData.SamplerTextureFileIDs.length - 1];
            const texture = textureLoader.load('/casc/blp2png?filedataid=' + textureFDID);
            texture.flipY = false;
            texture.magFilter = THREE.LinearFilter;
            texture.minFilter = THREE.LinearFilter;
            texture.wrapT = THREE.RepeatWrapping;
            texture.wrapS = THREE.RepeatWrapping;
            texture.channel = 0;
            const material = new THREE.MeshBasicMaterial({
                map: texture,
                transparent: false, // Enable transparency if needed
                color: 0xffffff // Ensure no color multiplication
            });

            textureArray[instanceIndex] = material;
        }

        const mesh = data.Mesh;
        const renderBatches = mesh.RenderBatches;

        const geosetToMaterialMap = new Map();
        for (let batchIndex = 0; batchIndex < renderBatches.RenderBatches.length; batchIndex++) {
            var geoset = renderBatches.RenderBatches[batchIndex].GeosetIndex;
            var material = renderBatches.RenderBatches[batchIndex].MaterialIndex;
            geosetToMaterialMap.set(geoset, material);
        }

        const targetLOD = 0;
        for (let lodIndex = 0; lodIndex < mesh.LodLevels.LODCount + 1; lodIndex++) {
            if (lodIndex != targetLOD)
                continue;

            console.log("Rendering LOD " + lodIndex + " for FDID " + filedataid);
            for (let geosetIndex = mesh.LodLevels.GeosetCount * lodIndex; geosetIndex < (mesh.LodLevels.GeosetCount * (lodIndex + 1)); geosetIndex++) {
                const geoset = mesh.Geosets.Geosets[geosetIndex];
                const geosetName = mesh.Names.NameBlock.slice(geoset.NameCharStart, geoset.NameCharStart + geoset.NameCharCount);
                console.log("\tRendering geoset " + geosetIndex + " (" + geosetName + ")");

                var bufferGeo = new THREE.BufferGeometry();

                if (mesh.Vertices.Format == "3F32") {
                    var buffer = Uint8Array.from(atob(mesh.Vertices.Buffer), c => c.charCodeAt(0));
                    var asFloat32 = new Float32Array(buffer.buffer);
                    bufferGeo.setAttribute("position", new THREE.Float32BufferAttribute(asFloat32, 3));
                }

                if (mesh.Normals.Format == "3F32") {
                    var buffer = Uint8Array.from(atob(mesh.Normals.Buffer), c => c.charCodeAt(0));
                    var asFloat32 = new Float32Array(buffer.buffer);
                    bufferGeo.setAttribute("normal", new THREE.Float32BufferAttribute(asFloat32, 3));
                }

                if (mesh.UV0.Format == "2F32") {
                    var buffer = Uint8Array.from(atob(mesh.UV0.Buffer), c => c.charCodeAt(0));
                    var asFloat32 = new Float32Array(buffer.buffer);
                    bufferGeo.setAttribute("uv", new THREE.Float32BufferAttribute(asFloat32, 2));
                }

                if (mesh.UV1.Format == "2F32") {
                    var buffer = Uint8Array.from(atob(mesh.UV1.Buffer), c => c.charCodeAt(0));
                    var asFloat32 = new Float32Array(buffer.buffer);
                    bufferGeo.setAttribute("uv1", new THREE.Float32BufferAttribute(asFloat32, 2));
                }

                if (mesh.Tangents.Format == "4F32") {
                    var buffer = Uint8Array.from(atob(mesh.Tangents.Buffer), c => c.charCodeAt(0));
                    var asFloat32 = new Float32Array(buffer.buffer);
                    bufferGeo.setAttribute("tangent", new THREE.Float32BufferAttribute(asFloat32, 4));
                }

                if (mesh.Indices.Format == "1U16") {
                    var buffer = Uint8Array.from(atob(mesh.Indices.Buffer), c => c.charCodeAt(0));
                    var asUInt16 = new Uint16Array(buffer.buffer);
                    var lodFiltered = asUInt16.slice(geoset.IndexStart, geoset.IndexStart + geoset.IndexCount);
                    bufferGeo.setIndex(new THREE.Uint16BufferAttribute(lodFiltered, 1));
                }

                const materialIndex = geosetToMaterialMap.get(geosetIndex);
                console.log("\t\tUsing material index " + materialIndex + " for geoset " + geosetIndex);
                const model = new THREE.Mesh(bufferGeo, textureArray[materialIndex]);
                scene.add(model);
            }
        }
    });
}

function clearScene() {
    for (var i = scene.children.length - 1; i >= 0; i--) {
        var obj = scene.children[i];
        scene.remove(obj);
    }
}
//const geometry = new THREE.BoxGeometry(1, 1, 1);
//const material = new THREE.MeshBasicMaterial({ color: 0x00ff00 });
//const cube = new THREE.Mesh(geometry, material);
//scene.add(cube);

camera.position.x = 0;
camera.position.y = 0;
camera.position.z = 10;
controls.update();
renderer.setAnimationLoop(animate);

function animate() {
    renderer.render(scene, camera);
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

var urlFileDataID = new URL(window.location).searchParams.get("filedataid");
if (urlFileDataID){
    Current.fileDataID = urlFileDataID;
}

var urlEmbed = new URL(window.location).searchParams.get("embed");
if (urlEmbed) {
    Current.embedded = true;
    document.getElementById("navbar").style.display = "none";
    document.getElementById("js-sidebar-button").style.display = "none";
    if (document.getElementById("fpsLabel"))
        document.getElementById("fpsLabel").style.display = "none";
    console.log("Running modelviewer in embedded mode!");
}

window.addEventListener('resize', () => {
    var canvas = document.getElementById("wowcanvas");
    if (canvas){
        canvas.width = document.body.clientWidth;
        canvas.height = document.body.clientHeight;
    }
});

document.getElementById('mvfiles').addEventListener('click', function(e) {
    const target = e.target;
    if (target.tagName === 'TD' && target.parentElement.tagName === 'TR' && target === target.parentElement.firstElementChild) {
        const tbody = target.closest('tbody');
        if (!tbody) return;

        const data = Elements.table.row(target.parentElement).data();

        const selected = document.querySelectorAll(".selected");
        selected.forEach(el => el.classList.remove("selected"));
        target.parentElement.classList.add('selected');
        loadModel(data[0]);
    }
});

document.getElementById('js-sidebar').addEventListener('input', function(e) {
    if (e.target.classList.contains('paginate_input')) {
        const paginateInput = document.querySelector(".paginate_input");
        if (paginateInput && paginateInput.value !== '') {
            const mvfilesTable = document.getElementById("mvfiles");
            if (mvfilesTable && Elements.table) {
                Elements.table.page(paginateInput.value - 1).ajax.reload(null, false);
            }
        }
    }
});

window.addEventListener('keydown', function(event){
    if (document.activeElement.tagName == "SELECT"){
        return;
    }

    const selected = document.querySelectorAll(".selected");
    if (selected.length == 1){
        if (event.key == "ArrowDown"){
            if (selected[0].rowIndex == 20) return;
            const mvfilesTable = document.getElementById('mvfiles');
            if (mvfilesTable && mvfilesTable.rows.length > 1){
                const nextRow = mvfilesTable.rows[selected[0].rowIndex + 1];
                if (nextRow && nextRow.firstChild) {
                    nextRow.firstChild.click();
                }
            }
        } else if (event.key == "ArrowUp"){
            if (selected[0].rowIndex == 1) return;
            const mvfilesTable = document.getElementById('mvfiles');
            if (mvfilesTable && mvfilesTable.rows.length > 1){
                const prevRow = mvfilesTable.rows[selected[0].rowIndex - 1];
                if (prevRow && prevRow.firstChild) {
                    prevRow.firstChild.click();
                }
            }
        }
    }

    if (document.activeElement.tagName == "INPUT" || document.activeElement.tagName == "SELECT"){
        event.stopImmediatePropagation();
    }
}, true);

window.addEventListener('keyup', function(event){
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

function toggleUI() {
    document.querySelector(".navbar").style.display = document.querySelector(".navbar").style.display === "none" ? "" : "none";
    document.getElementById("js-sidebar-button").style.display = document.getElementById("js-sidebar-button").style.display === "none" ? "" : "none";
    document.getElementById("js-controls").style.display = document.getElementById("js-controls").style.display === "none" ? "" : "none";
}

loadModel(Current.fileDataID);
(function() {
    const wowcanvas = document.getElementById('wowcanvas');
    if (wowcanvas) {
        wowcanvas.addEventListener('contextmenu', function(e){
            e.preventDefault();
            return false;
        });
    }

    // Skip further initialization in embedded mode
    if (typeof embeddedMode !== 'undefined' && embeddedMode){
        return;
    }

    const mvfilesElement = document.getElementById('mvfiles');
    if (mvfilesElement && typeof jQuery !== 'undefined' && jQuery.fn.DataTable) {
        Elements.table = jQuery(mvfilesElement).DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/listfile/datatables",
            "data": function ( d ) {
                return Object.assign( {}, d, {
                    "src": "mv",
                    "showWMO": false,
                    "showM2": false,
                    "showM3": true
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
                        td.style.backgroundColor = '#ff5858';
                        td.style.color = 'white';
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
    }

    document.querySelectorAll(".filterBox").forEach(filterBox => {
        filterBox.addEventListener('change', function(){
            if (Elements.table) {
                Elements.table.ajax.reload();
            }
        });
    });
 
    document.getElementById('mvfiles_search').addEventListener('input', function(){
        if (Elements.table) {
            Elements.table.search(this.value).draw();
        }
    });
}());