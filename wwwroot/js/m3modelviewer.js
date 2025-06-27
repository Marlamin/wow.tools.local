import * as THREE from 'three';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';
function showError(error) {
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
        const mesh = data.Mesh;

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

                const textureLoader = new THREE.TextureLoader();

                var textureFDID = 3025978;
                var textureUV = 0;
                if (filedataid == 6648661) {
                    if (geosetIndex == 0) {
                        textureFDID = 6055692;
                    } else if (geosetIndex == 1) {
                        textureFDID = 6055689;
                    } else if (geosetIndex == 2) {
                        textureFDID = 6076960;
                    }
                }
                else if (filedataid == 6655655) {
                    if (geosetIndex == 0) {
                        textureFDID = 7018222;
                    } else if (geosetIndex == 1) {
                        textureFDID = 7018220;
                    } else if (geosetIndex == 2) {
                        textureFDID = 7018222;
                    }
                }
                const texture = textureLoader.load('/casc/blp2png?filedataid=' + textureFDID);
                texture.flipY = false;
                texture.magFilter = THREE.LinearFilter;
                texture.minFilter = THREE.LinearFilter;
                texture.wrapT = THREE.RepeatWrapping;
                texture.wrapS = THREE.RepeatWrapping;
                texture.channel = textureUV;
                const material = new THREE.MeshBasicMaterial({
                    map: texture,
                    transparent: false, // Enable transparency if needed
                    color: 0xffffff // Ensure no color multiplication
                });

                const model = new THREE.Mesh(bufferGeo, material);
                console.log(model);
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
    $("#navbar").hide();
    $("#js-sidebar-button").hide();
    $("#fpsLabel").hide();
    console.log("Running modelviewer in embedded mode!");
}

window.addEventListener('resize', () => {
    var canvas = document.getElementById("wowcanvas");
    if (canvas){
        canvas.width = document.body.clientWidth;
        canvas.height = document.body.clientHeight;
    }
});

$('#mvfiles').on('click', 'tbody tr td:first-child', function() {
    var data = Elements.table.row($(this).parent()).data();

    $(".selected").removeClass("selected");
    $(this).parent().addClass('selected');
    loadModel(data[0]);
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

function toggleUI(){
    $(".navbar").toggle();
    $("#js-sidebar-button").toggle();
    $("#js-controls").toggle();
}

loadModel(Current.fileDataID);
(function() {
    $('#wowcanvas').bind('contextmenu', function(e){
        return false;
    });

    // Skip further initialization in embedded mode
    if (embeddedMode){
        return;
    }

    Elements.table = $('#mvfiles').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/listfile/datatables",
            "data": function ( d ) {
                return $.extend( {}, d, {
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