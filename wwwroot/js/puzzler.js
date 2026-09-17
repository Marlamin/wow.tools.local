/* UI */
const CONSTANTS = {
	MAP_SIZE: 64,
	MAP_SIZE_SQ: 4096, // 64x64
	MAP_COORD_BASE: 51200 / 3, // 17066,66666666667
	TILE_SIZE: (51200 / 3) / 32, // 533,3333333333333
	MAP_OFFSET: 17066,
}

const Elements =
{
	Maps: document.getElementById('js-map-select'),
	Sidebar: document.getElementById('js-sidebar'),
	Map: document.getElementById('js-map'),
	Notifications: document.getElementById('js-notifs'),
	TechBox: document.getElementById('js-techbox'),
	Layers: document.getElementById('js-layers'),
	LayerSelect: document.getElementById('js-layer-select'),
	//ADTGrid: document.getElementById('js-adtgrid'),
};

const Current =
{
	Map: false,
	InternalMap: "",
	InternalMapID: -1,
	Version: 0,
	wdtFileDataID: -1
};


const state = {
	offsetX: 0,
	offsetY: 0,
	zoomFactor: 2,
	tileQueue: [],
	tilesLoading: 0,
	cache: new Array(CONSTANTS.MAP_SIZE_SQ),
	isPanning: false,
	isDragging: false,
    mousePos: { x: 0, y: 0 },
	tileSize: 512,
	mask: [],
    remoteMask: [],
	layer: 0,
	zoom: 25,
	hoverTileX: 0,
	hoverTileY: 0,
	showOverlay: false,
	map: -1, // None
};

const mapCanvas = document.getElementById('map-canvas');
state.canvas = mapCanvas;
state.ctx = state.canvas.getContext("2d", { willReadFrequently: true });

const techBoxCanvasPosX = document.getElementById('canvasPosX');
const techBoxCanvasPosY = document.getElementById('canvasPosY');
const techBoxMousePosX = document.getElementById('mousePosX');
const techBoxMousePosY = document.getElementById('mousePosY');
const techBoxWoWPosX = document.getElementById('wowPosX');
const techBoxWoWPosY = document.getElementById('wowPosY');
const techBoxWoWPosADTX = document.getElementById('adtPosX');
const techBoxWoWPosADTY = document.getElementById('adtPosY');
const techBoxZoom = document.getElementById('zoomLevel');

var d = function (text) { console.log(text); };

//Sidebar button
document.getElementById('js-sidebar-button').addEventListener('click', function () {
	Elements.Sidebar.classList.toggle('closed');
	Elements.TechBox.classList.toggle('closed');
	document.getElementById('js-sidebar-button').classList.toggle('closed');
});

// Layer button
document.getElementById('js-layers-button').addEventListener('click', function () {
	Elements.Layers.classList.toggle('closed');
	document.getElementById('js-layers-button').classList.toggle('closed');
});

(async () => {
	resizeWindow = function () {
		window.w = state.canvas.width = window.innerWidth;
		return window.h = state.canvas.height = window.innerHeight;
	};

	resizeWindow();

	var maps = await InitializeMapList();
	await InitializeMapOptions(maps);
	await InitializeEvents();

	if (Current.InternalMap != -1) {
		await loadMapMask(Current.Map, Current.InternalMap, Current.wdtFileDataID);
	}
	state.zoomFactor = 2;
	render();
	setDefaultPosition();
})();

async function InitializeMapList() {
	var response = await fetch('/map/list');
	var json = await response.json();
	return json;
}

async function InitializeMapOptions(maps) {
	var url = window.location.pathname.split('/'),
		option,
		fragment = document.createDocumentFragment();

	maps.forEach(function (map, i) {
		option = document.createElement('option');
		option.dataset.internal = map.internalName;
		option.dataset.imapid = map.ID;
		option.dataset.wdtfiledataid = map.wdtFileDataID;
		option.setAttribute('data-custom-properties', JSON.stringify({
			internal: map.internalName,
			imapid: map.ID
		}));
		option.value = map.ID;
		option.textContent = map.displayName;

		fragment.appendChild(option);

		// // Either first map, or specified map
		// if (i === 0 || map.internal === decodeURIComponent(url[2])) {

		// 	Current.Map = map.ID;
		// 	Current.InternalMap = map.internalName;
		// 	Current.InternalMapID = map.ID;
		// 	Current.wdtFileDataID = map.wdtFileDataID;
		// 	Current.Version = '' + parseInt(url[3], 10);

		// 	document.getElementById("downloadLink").href = "/map/download?mapID=" + Current.Map + "&directory=" + Current.InternalMap + "&wdtFileDataID=" + Current.wdtFileDataID + "&layer=" + state.layer;

		// 	if (map.internal === decodeURIComponent(url[2])) {
		// 		option.selected = true;
		// 	}
		// }
	});

	Elements.Maps.appendChild(fragment);
}

async function InitializeEvents() {
	const choices = new Choices(Elements.Maps, {
		searchEnabled: true,
		searchChoices: true,
		searchFields: ['label', 'value', 'customProperties.internal', 'customProperties.imapid'],
		searchResultLimit: 50,
		shouldSort: false,
		itemSelectText: '',
	});

	Elements.ChoicesInstance = choices;

	Elements.Maps.addEventListener('change', async function (event) {
		Current.Map = this.value;
		Current.InternalMap = this.options[this.selectedIndex].dataset.internal;
		Current.InternalMapID = this.options[this.selectedIndex].dataset.imapid;
		Current.wdtFileDataID = this.options[this.selectedIndex].dataset.wdtfiledataid;

		await loadMapMask(Current.Map, Current.InternalMap, Current.wdtFileDataID);

		document.getElementById("downloadLink").href = "/map/download?mapID=" + Current.Map + "&directory=" + Current.InternalMap + "&wdtFileDataID=" + Current.wdtFileDataID + "&layer=" + state.layer;

		state.zoomFactor = 2;
		await render();
		setDefaultPosition();
	});

	Elements.LayerSelect.addEventListener('change', async function (event) {
		state.layer = this.value;

		//await loadMapMask(Current.Map, Current.InternalMap, Current.wdtFileDataID);
		state.cache = new Array(CONSTANTS.MAP_SIZE_SQ);

		document.getElementById("downloadLink").href = "/map/download?mapID=" + Current.Map + "&directory=" + Current.InternalMap + "&wdtFileDataID=" + Current.wdtFileDataID + "&layer=" + state.layer;

		await render();
	});

	choices.enable();
	return true;
}
/* VIEWER */
function updateTechCanvasPos() {
	techBoxCanvasPosX.innerHTML = Math.round(state.offsetX);
	techBoxCanvasPosY.innerHTML = Math.round(state.offsetY);
}

function updateTechMousePos(x, y) {
	techBoxMousePosX.innerHTML = Math.round(x);
	techBoxMousePosY.innerHTML = Math.round(y);

	if (state.mask.length == 0)
		return;

	const tileIndex = (state.hoverTileY * CONSTANTS.MAP_SIZE) + state.hoverTileX;
	const tileDetails = document.getElementById('js-tile-details');
	const tileState = state.mask[tileIndex];
	tileDetails.innerHTML = "Tile: " + state.hoverTileX + ", " + state.hoverTileY + " (index " + tileIndex + ")<br>ADT: " + tileState.rootADT + "<br>Minimap: " + tileState.minimapTexture + "<br>MapTexture: " + tileState.mapTexture + "<br>MapTextureN: " + tileState.mapTextureN + "<br>Layer: " + state.layer;
}

function updateTechWoWPos(x, y) {
	var wowPos = canvasPosToWoW(x, y);
	techBoxWoWPosX.innerHTML = Math.round(wowPos.ingameX, 3);
	techBoxWoWPosY.innerHTML = Math.round(wowPos.ingameY, 3);
	techBoxWoWPosADTX.innerHTML = Math.round(wowPos.tileX, 3);
	techBoxWoWPosADTY.innerHTML = Math.round(wowPos.tileY, 3);
}

function updateTechZoom() {
	techBoxZoom.innerHTML = state.zoomFactor;
}

mapCanvas.addEventListener('mouseup', function (e) {
	if (state.isPanning)
		state.isPanning = false;

	if (state.isDragging) {
		state.isDragging = false;

		// todo: drop the tile on the current position, if there is already a tile here move that to the current tile's location
		const incomingTileImage = state.cache[state.selectedTile.index];
        const outgoingTileIndex = (state.hoverTileY * CONSTANTS.MAP_SIZE) + state.hoverTileX;
		const outgoingTileImage = state.cache[outgoingTileIndex];

		// check if we actually have to replace a tile here, we can just drop things on new tiles in theory
		if (outgoingTileImage !== undefined) {
            // put outgoing tile in the selected tile's previous position
			state.cache[outgoingTileIndex] = incomingTileImage;
		}

		state.cache[state.selectedTile.index] = outgoingTileImage;

		// also swap the mask values so that the tiles are actually swapped between zooms, todo: save this out
		const incomingMaskValue = getFDIDForTile(state.selectedTile.index);	
		const outgoingMaskValue = getFDIDForTile(outgoingTileIndex);

		updateTileInMask(state.selectedTile.index, state.layer, outgoingMaskValue);
        updateTileInMask(outgoingTileIndex, state.layer, incomingMaskValue);

		state.selectedTile = undefined;

		render();
	}

	const canvasPos = canvasPosToWoW(event.clientX, event.clientY);
	document.getElementById("clickedCoord").textContent = Math.floor(canvasPos.ingameX) + ' ' + Math.floor(canvasPos.ingameY) + ' ' + 200 + ' ' + Current.InternalMapID;
	document.getElementById("clickedADT").textContent = Current.InternalMap + '_' + canvasPos.tileX + '_' + canvasPos.tileY;
	document.getElementById("modelviewerLink").href = "/mv/?type=wdt&filedataid=" + Current.wdtFileDataID + "&x=" + Math.floor(canvasPos.ingameX) + "&y=" + Math.floor(canvasPos.ingameY) + "&z=200";
});

mapCanvas.addEventListener('mouseout', function (e) {
	if (state.isPanning)
		state.isPanning = false;
});

function canvasPosToWoW(x, y) {
	const tileSize = Math.floor(state.tileSize / state.zoomFactor);

	const canvasX = x - state.offsetX;
	const canvasY = y - state.offsetY;

	const pixelX = Math.floor(canvasX * state.zoomFactor);
	const pixelY = Math.floor(canvasY * state.zoomFactor);

	const tileY = Math.floor(canvasY / tileSize);
	const tileX = Math.floor(canvasX / tileSize);

	var adtsToCenterX = ((canvasY / tileSize)) - 32;
	var adtsToCenterY = ((canvasX / tileSize)) - 32;

	var ingameX = -(adtsToCenterX * CONSTANTS.TILE_SIZE); // (╯°□°）╯︵ ┻━┻
	var ingameY = -(adtsToCenterY * CONSTANTS.TILE_SIZE); // (╯°□°）╯︵ ┻━┻

	return { pixelX, pixelY, tileX, tileY, ingameX, ingameY };
}

mapCanvas.oncontextmenu = function (e) {
	e.preventDefault();
	e.stopPropagation();
}

mapCanvas.addEventListener('mousedown', function (event) {
	if (!state.isPanning && (event.button === 1 || event.button === 2)) {
		state.isPanning = true;

		this.mouseBaseX = event.clientX;
		this.mouseBaseY = event.clientY;

		this.panBaseX = state.offsetX;
		this.panBaseY = state.offsetY;

		updateTechCanvasPos();

		event.preventDefault();

		return;
	}

	if (event.button === 0) {
		// select tile on left click
		const canvasPos = canvasPosToWoW(event.clientX, event.clientY);
		state.selectedTile = { x: canvasPos.tileX, y: canvasPos.tileY, index: (canvasPos.tileY * CONSTANTS.MAP_SIZE) + canvasPos.tileX };
		state.isDragging = true;
		render();
	}
});

mapCanvas.addEventListener('mousemove', function (event) {
	state.mousePos.x = event.clientX;
	state.mousePos.y = event.clientY;

	const tileSize = Math.floor(state.tileSize / state.zoomFactor);
	state.hoverTileX = Math.floor((state.mousePos.x - state.offsetX) / tileSize);
	state.hoverTileY = Math.floor((state.mousePos.y - state.offsetY) / tileSize);

	updateTechMousePos(event.clientX, event.clientY);
	updateTechWoWPos(event.clientX, event.clientY);

	if (state.isPanning) {
		const deltaX = this.mouseBaseX - event.clientX;
		const deltaY = this.mouseBaseY - event.clientY;

		state.offsetX = this.panBaseX - deltaX;
		state.offsetY = this.panBaseY - deltaY;

		updateTechCanvasPos();
		render();
	}

	if (state.isDragging) {
		render();
	}
});

mapCanvas.addEventListener('mousewheel', function (event) {
	const delta = event.deltaY > 0 ? 1 : -1;
	const newZoom = Math.max(1, Math.min(state.zoom, state.zoomFactor + delta));

	if (newZoom !== state.zoomFactor) {
		const zoomRatio = state.zoomFactor / newZoom;

		state.offsetX = (state.offsetX - event.clientX) * zoomRatio + event.clientX;
		state.offsetY = (state.offsetY - event.clientY) * zoomRatio + event.clientY;

		state.zoomFactor = newZoom;
		updateTechZoom();
		initializeCache();

		updateTechCanvasPos();
		render();
	}
	event.preventDefault();
	return false;
});

window.addEventListener('keydown', function (event) {
	if (document.activeElement.tagName === 'INPUT')
		return;
	
	if (event.key === 'l' || event.key === 'L') {
		const layerSelect = document.getElementById("js-layer-select");

		if (layerSelect.selectedIndex == 3) {
			layerSelect.selectedIndex = 0;
		} else {
            layerSelect.selectedIndex = layerSelect.selectedIndex + 1;
		}

        layerSelect.dispatchEvent(new Event('change'));
	}

    if (event.key == 'f' || event.key == 'F') {
		const manualFileInput = document.getElementById("js-manual-file-input");

		if (manualFileInput.style.display == 'block') {
			manualFileInput.style.display = 'none';
		} else {
			manualFileInput.style.display = 'block';
		}
	}

	if (event.key == 'd' || event.key == 'D') {
		const index = (state.hoverTileY * CONSTANTS.MAP_SIZE) + state.hoverTileX;
		updateTileInMask(index, state.layer, 0);
		render();
	}

	if (event.key == 'r' || event.key == 'R') {
		state.mask = JSON.parse(JSON.stringify(state.remoteMask));
		render();
	}

	if (event.key == 'o' || event.key == 'O') {
		state.showOverlay = !state.showOverlay;
		render();
	}
});

async function initializeCache() {
	state.tileQueue = [];
	state.cache = new Array(CONSTANTS.MAP_SIZE_SQ);
}

async function loadMapMask(mapID, directory, wdtFileDataID) {
	if (mapID == -1 || directory == "" || wdtFileDataID == -1)
		return;

	const response = await fetch("/map/wdtMaskPuzzle?mapID=" + mapID + "&directory=" + directory + "&wdtFileDataID=" + wdtFileDataID);
	const tiles = await response.json();
	state.mask = tiles;

	// store a copy of the remote mask so we can reset to it later
	state.remoteMask = JSON.parse(JSON.stringify(tiles));

	state.cache = new Array(CONSTANTS.MAP_SIZE_SQ);

	document.getElementById("clickedCoord").textContent = "No click. :(";
	document.getElementById("clickedADT").textContent = "No click. :(";

	return tiles;
}

function notify(msg, level = "danger") {
	Elements.Notifications.innerHTML = "";
	let notif = document.createElement("div");
	notif.classList.add("alert");
	notif.classList.add("alert-" + level);
	notif.textContent = msg;
	Elements.Notifications.appendChild(notif);
	setTimeout(() => {
		notif.remove();
	}, 2000);
}

function checkTileQueue() {
	// load a max of 5 tiles at a time so backend doesnt implode
	while (state.tilesLoading < 5 && state.tileQueue.length > 0) {
		const tile = state.tileQueue.shift();
		state.tilesLoading++;
		loadTile(tile);
	}
}

function queueTile(x, y, index, tileSize) {
	const node = { x, y, index, tileSize };
	state.tileQueue.push(node);
	checkTileQueue();
}

async function loadTile(tile) {
	const data = await loadMapTile(tile.x, tile.y, tile.tileSize, tile.index);
	state.cache[tile.index] = data;
	if (data !== false)
		render();

	state.tilesLoading--;
	checkTileQueue();
}

function getFDIDForTile(index) {
	if (state.mask && state.mask[index] !== undefined) {
		if (state.layer == 0)
			return state.mask[index].minimapTexture; // minimap
		else if (state.layer == 1)
			return state.mask[index].mapTexture; // maptexture
		else if (state.layer == 2)
			return state.mask[index].mapTextureN; // maptexture normals
		else if (state.layer == 3 || state.layer == 4)
			return state.mask[index].rootADT; // adt vertex colors
		else if (state.layer == 5)
			return state.mask[index].liquidFlow; // liquid flow blp
	}
}

function setDefaultPosition() {
	let posX = 0, posY = 0;

	if (state.mask) {
		const center = Math.floor(CONSTANTS.MAP_COORD_BASE / CONSTANTS.TILE_SIZE)
		const centerIndex = getFDIDForTile((center * CONSTANTS.MAP_SIZE) + center);

		if (centerIndex !== 0) {
			posX = ((center - 32) * CONSTANTS.TILE_SIZE) * -1;
			posY = ((center - 32) * CONSTANTS.TILE_SIZE) * -1;
		} else {
			const index = state.mask.findIndex(e => e.minimapTexture > 0);
			if (index > -1) {
				const tileX = index % CONSTANTS.MAP_SIZE;
				const tileY = Math.floor(index / CONSTANTS.MAP_SIZE);
				posX = ((tileY - 32) * CONSTANTS.TILE_SIZE) * -1;
				posY = ((tileX - 32) * CONSTANTS.TILE_SIZE) * -1;
			}
		}
	}

	setMapPosition(posX, posY);
}

async function render() {
	const canvas = state.canvas;
	if (!canvas)
		return;

	// We do this on purpose to clear the canvas, get rendering bugs on Chrome without
	canvas.width = canvas.offsetWidth;
	canvas.height = canvas.offsetHeight;

	const ctx = state.ctx;
	const tileSize = Math.floor(state.tileSize / state.zoomFactor);
	const viewportWidth = canvas.width;
	const viewportHeight = canvas.height;

	const minTileX = Math.max(0, Math.floor(-state.offsetX / tileSize));
	const minTileY = Math.max(0, Math.floor(-state.offsetY / tileSize));
	const maxTileX = Math.min(CONSTANTS.MAP_SIZE, Math.ceil((viewportWidth - state.offsetX) / tileSize) + 1);
	const maxTileY = Math.min(CONSTANTS.MAP_SIZE, Math.ceil((viewportHeight - state.offsetY) / tileSize) + 1);

	// first we draw a grid
	for (let x = minTileY; x < maxTileY; x++) {
		for (let y = minTileX; y < maxTileX; y++) {
			// draw rectangle on canvas
			const drawX = (y * tileSize) + state.offsetX;
			const drawY = (x * tileSize) + state.offsetY;
			ctx.strokeStyle = 'rgba(255, 255, 255, 0.1)';
			ctx.strokeRect(drawX, drawY, tileSize, tileSize);
		}
	}

	if (state.map === null)
		return;

	if (state.mask.length === 0)
		return;

	for (let x = minTileY; x < maxTileY; x++) {
		for (let y = minTileX; y < maxTileX; y++) {
			let drawX = (y * tileSize) + state.offsetX;
			let drawY = (x * tileSize) + state.offsetY;

			const index = (x * CONSTANTS.MAP_SIZE) + y;
			const cached = state.cache[index];

			if (!state.mask)
				continue;

			if (getFDIDForTile(index) === 0)
				continue;

			let tileSelected = false;
			if (state.selectedTile !== undefined && state.selectedTile.index === index)
				tileSelected = true;

			if (cached === undefined) {
				state.cache[index] = true;
				queueTile(x, y, index, tileSize);
			} else if (cached instanceof ImageData) {
				ctx.putImageData(cached, drawX, drawY);
			}

			if (tileSelected) {
				ctx.fillStyle = 'rgba(255, 0, 0, 0.3)';
				ctx.fillRect(drawX, drawY, tileSize, tileSize);
			}

			if (state.isDragging) {
				ctx.fillStyle = 'rgba(0, 255, 0, 0.3)';

				if (state.hoverTileX === y && state.hoverTileY === x) {
					ctx.fillRect(drawX, drawY, tileSize, tileSize);
				}
			}
		}
	}

	// draw dragged tile on top
	if (state.isDragging && state.selectedTile !== undefined) {
		const index = state.selectedTile.index;
		const cached = state.cache[index];
        if (cached instanceof ImageData) {
		    ctx.putImageData(cached, state.mousePos.x - (tileSize / 2), state.mousePos.y - (tileSize / 2));
        }
	}

	if (state.showOverlay) {
		for (let x = minTileY; x < maxTileY; x++) {
			for (let y = minTileX; y < maxTileX; y++) {
				const index = (x * CONSTANTS.MAP_SIZE) + y;
				if (IndexHasTile(index)) {
					const tileState = state.mask[index];
					const drawX = (y * tileSize) + state.offsetX;
					const drawY = (x * tileSize) + state.offsetY;

					ctx.fillStyle = 'rgba(255, 255, 255, 0.5)';
					ctx.fillRect(drawX, drawY, tileSize, tileSize);

                    ctx.fillStyle = 'rgba(0, 0, 0, 1)';
					ctx.font = "14px Arial";
					ctx.fillText("ADT: " + tileState.rootADT, drawX, drawY + 20);
					ctx.fillText("Minimap: " + tileState.minimapTexture, drawX, drawY + 40);
					ctx.fillText("MapTex: " + tileState.mapTexture, drawX, drawY + 60);
                    ctx.fillText("MapTexN: " + tileState.mapTextureN, drawX, drawY + 80);
				}
			}
		}
	}
}

function IndexHasTile(index) {
    const tileState = state.mask[index];
    return tileState.rootADT !== 0 || tileState.minimapTexture !== 0 || tileState.mapTexture !== 0 || tileState.mapTextureN !== 0;
}

function addManualFiles() {
	const fileInput = document.getElementById("manual-files");

	let startX = parseInt(document.getElementById("manual-start-x").value);
	if (startX == undefined || isNaN(startX) || startX < 0 || startX > 64)
		startX = 32;

	let startY = parseInt(document.getElementById("manual-start-y").value);
	if (startY == undefined || isNaN(startY) || startY < 0 || startY > 64)
		startY = 32;

	let perRow = parseInt(document.getElementById("manual-per-row").value);
	if (perRow == undefined || isNaN(perRow))
		perRow = 5;

	let startIndex = (startY * CONSTANTS.MAP_SIZE) + startX;
	let rowStartIndex = startIndex;
	let placedCols = 0;

	fileInput.value.split("\n").forEach(file => {
		let splitFile = file.split(";");
		if (splitFile.length != 2)
			return;

		let fileDataID = parseInt(splitFile[0]);
		let fileLower = splitFile[1].toLowerCase();

		if (state.layer == 0 && fileLower.includes("minimaps") && fileLower.endsWith(".blp")) {
			// Add as minimap
		} else if (state.layer == 1 && fileLower.includes("maptextures") && fileLower.endsWith(".blp") && !fileLower.endsWith("_n.blp")) {
			// Add as maptexture (non normals)
		} else if (state.layer == 2 && fileLower.includes("maptextures") && fileLower.endsWith("_n.blp")) {
			// Add as maptexture (normals)
		} else if (state.layer == 3 && fileLower.endsWith(".adt") && !fileLower.endsWith("_lod.adt") && !fileLower.endsWith("_obj0.adt") && !fileLower.endsWith("_obj1.adt") && !fileLower.endsWith("_tex0.adt")) {
			// Add as ADT (vertex colors/heightmap)
		} else {
			return;
		}

		console.log("Need to add fileDataID " + fileDataID + " for layer " + state.layer);

		var fileAdded = false;
		var index = rowStartIndex;

		while (!fileAdded) {
			if (getFDIDForTileLayer(index, state.layer) === 0) {
				updateTileInMask(index, state.layer, fileDataID);
				placedCols++;
				fileAdded = true;
			}

			if (placedCols >= perRow) {
				rowStartIndex += CONSTANTS.MAP_SIZE;
				placedCols = 0;
			}

			index++;
		}
	});

	render();
}

function getFDIDForTileLayer(index, layer) {
	if (state.mask && state.mask[index] !== undefined) {
		if (layer == 0)
			return state.mask[index].minimapTexture; // minimap
		else if (layer == 1)
			return state.mask[index].mapTexture; // maptexture
		else if (layer == 2)
			return state.mask[index].mapTextureN; // maptexture normals
		else if (layer == 3)
			return state.mask[index].rootADT; // adt vertex colors
	}
}

function updateTileInMask(index, layer, fileDataID) {
	if (layer == 0)
		state.mask[index].minimapTexture = fileDataID;
	else if (layer == 1)
		state.mask[index].mapTexture = fileDataID;
	else if (layer == 2)
		state.mask[index].mapTextureN = fileDataID;
	else if (layer == 3)
        state.mask[index].rootADT = fileDataID;
}

function generateStateDiffFiles() {
	// walk through state.remoteMask and state.mask and find any differences
	// store by fdid key
    var diffs = [];

	state.mask.forEach((tile, index) => {
		const remoteTile = state.remoteMask[index];
		const paddedX = tile.x.toString().padStart(2, "0");
		const paddedY = tile.y.toString().padStart(2, "0");

		if (tile.minimapTexture !== remoteTile.minimapTexture) {
			diffs.push(tile.minimapTexture + ";" + "World/Minimaps/" + Current.InternalMap + "/map" + paddedY + "_" + paddedX + ".blp");
		}

		if (tile.mapTexture !== remoteTile.mapTexture) {
			diffs.push(tile.mapTexture + ";" + "World/MapTextures/" + Current.InternalMap + "/" + Current.InternalMap + "_" + paddedY + "_" + paddedX + ".blp");
		}

		if (tile.mapTextureN !== remoteTile.mapTextureN) {
			diffs.push(tile.mapTextureN + ";" + "World/MapTextures/" + Current.InternalMap + "/" + Current.InternalMap + "_" + paddedY + "_" + paddedX + "_n.blp");
		}

		if (tile.rootADT !== remoteTile.rootADT) {
			diffs.push(tile.rootADT + ";" + "World/Maps/" + Current.InternalMap + "/" + Current.InternalMap + "_" + tile.y + "_" + tile.x + ".adt"); // adts arent padded
			// todo: also need to add other ADTs here
		}
	});

	diffs.sort();

    document.getElementById("state-output").value = diffs.join("\n");
}


function setMapPosition(x, y) {
	const tileSize = Math.floor(state.tileSize / state.zoomFactor);

	const ofsX = (((y - CONSTANTS.MAP_COORD_BASE) / CONSTANTS.TILE_SIZE) * tileSize);
	const ofsY = (((x - CONSTANTS.MAP_COORD_BASE) / CONSTANTS.TILE_SIZE) * tileSize);

	state.offsetX = ofsX + (state.canvas.clientWidth / 2);
	state.offsetY = ofsY + (state.canvas.clientHeight / 2);

	updateTechCanvasPos();
	updateTechZoom();
	render();
}

function setZoomFactor(factor) {
	state.zoomFactor = factor;
	updateTechZoom();
	initializeCache();
}

async function loadMapTile(x, y, size, index) {
	try {
		const response = await fetch("/map/tile?fileDataID=" + getFDIDForTile(index) + "&targetSize=" + size);
		const arrayBuffer = await response.arrayBuffer();
		return new ImageData(new Uint8ClampedArray(arrayBuffer), size, size);
	} catch (e) {
		// Map tile does not exist or cannot be read.
		console.log(e);
		return false;
	}
}
