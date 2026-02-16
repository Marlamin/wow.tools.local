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
	//ADTGrid: document.getElementById('js-adtgrid'),
};

const Current =
{
	Map: false,
	InternalMap: "Azeroth",
	InternalMapID: 0,
	Version: 0,
	wdtFileDataID: 0
};


const state = {
	offsetX: 0,
	offsetY: 0,
	zoomFactor: 2,
	tileQueue: [],
	cache: new Array(CONSTANTS.MAP_SIZE_SQ),
	awaitingTile: false,
	isPanning: false,
	tileSize: 512,
	mask: [],
	zoom: 15,
	map: 0 // Azeroth?
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
//document.getElementById('js-layers-button').addEventListener('click', function () {
//	Elements.Layers.classList.toggle('closed');
//});

(async () => {
	resizeWindow = function () {
		window.w = state.canvas.width = window.innerWidth;
		return window.h = state.canvas.height = window.innerHeight;
	};

	resizeWindow();

	var maps = await InitializeMapList();
	await InitializeMapOptions(maps);
	await InitializeEvents();
	await loadMapMask(Current.Map, Current.InternalMap, Current.wdtFileDataID);
	render();
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
		option.dataset.imapid = map.id;
		option.dataset.wdtfiledataid = map.wdtFileDataID;
		option.value = map.id;
		option.textContent = map.displayName;

		fragment.appendChild(option);

		// Either first map, or specified map
		if (i === 0 || map.internal === decodeURIComponent(url[2])) {

			Current.Map = map.id;
			Current.InternalMap = map.internalName;
			Current.InternalMapID = map.id;
			Current.wdtFileDataID = map.wdtFileDataID;
			Current.Version = '' + parseInt(url[3], 10);
			if (map.internal === decodeURIComponent(url[2])) {
				option.selected = true;
			}
		}
	});

	Elements.Maps.appendChild(fragment);
}

async function InitializeEvents() {
	var select2El = $("#js-map-select").select2({ matcher: wowMapMatcher, disabled: false });
	Elements.MapSelect2 = select2El;
	$(Elements.Maps).on('change', function (event) {
		Current.Map = this.value;
		Current.InternalMap = this.options[this.selectedIndex].dataset.internal;
		Current.InternalMapID = this.options[this.selectedIndex].dataset.imapid;
		Current.wdtFileDataID = this.options[this.selectedIndex].dataset.wdtfiledataid;
		loadMapMask(Current.Map, Current.InternalMap, Current.wdtFileDataID);
	});

	Elements.Maps.disabled = false;
	return true;
}

function wowMapMatcher(params, data) {
	// If there are no search terms, return all of the data
	if ($.trim(params.term) === '') {
		return data;
	}

	// Do not display the item if there is no 'text' property
	if (typeof data.text === 'undefined') {
		return null;
	}

	if (data.text.toLowerCase().indexOf(params.term.toLowerCase()) > -1) {
		var modifiedData = $.extend({}, data, true);
		modifiedData.text += ' (text match)';
		return modifiedData;
	}

	if (data.element.dataset.internal.toLowerCase().indexOf(params.term.toLowerCase()) > -1) {
		var modifiedData = $.extend({}, data, true);
		modifiedData.text += ' (internal match)';
		return modifiedData;
	}

	if (data.element.dataset.internal.toLowerCase().indexOf(params.term.toLowerCase()) > -1) {
		var modifiedData = $.extend({}, data, true);
		modifiedData.text += ' (internal match)';
		return modifiedData;
	}

	if (data.element.dataset.imapid != null && data.element.dataset.imapid == params.term) {
		var modifiedData = $.extend({}, data, true);
		modifiedData.text += ' (mapid match)';
		return modifiedData;
	}

	return null;
}

/* VIEWER */
function updateTechCanvasPos() {
	techBoxCanvasPosX.innerHTML = Math.round(state.offsetX);
	techBoxCanvasPosY.innerHTML = Math.round(state.offsetY);
}

function updateTechMousePos(x, y) {
	techBoxMousePosX.innerHTML = Math.round(x);
	techBoxMousePosY.innerHTML = Math.round(y);
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

	const canvasPos = canvasPosToWoW(event.clientX, event.clientY);
	document.getElementById("clickedCoord").textContent = Math.floor(canvasPos.ingameX) + ' ' + Math.floor(canvasPos.ingameY) + ' ' + 200 + ' ' + Current.InternalMapID;
	document.getElementById("clickedADT").textContent = Current.InternalMap + '_' + canvasPos.tileX + '_' + canvasPos.tileY;
	document.getElementById("modelviewerLink").href = "/mv/?type=wdt&filedataid=" + Current.wdtFileDataID + "&x=" + Math.floor(canvasPos.ingameX) + "&y=" + Math.floor(canvasPos.ingameY) + "&z=200";
});

mapCanvas.addEventListener('mouseout', function (e) {
	if (state.isPanning)
		state.isPanning = false;
});

mapCanvas.addEventListener('mousemove', function (event) {
	updateTechMousePos(event.clientX, event.clientY);
	updateTechWoWPos(event.clientX, event.clientY);
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

mapCanvas.addEventListener('mousedown', function (event) {
	if (!state.isPanning) {
		state.isPanning = true;

		this.mouseBaseX = event.clientX;
		this.mouseBaseY = event.clientY;

		this.panBaseX = state.offsetX;
		this.panBaseY = state.offsetY;

		updateTechCanvasPos();
	}
});

mapCanvas.addEventListener('mousemove', function (event) {
	if (state.isPanning) {
		const deltaX = this.mouseBaseX - event.clientX;
		const deltaY = this.mouseBaseY - event.clientY;

		state.offsetX = this.panBaseX - deltaX;
		state.offsetY = this.panBaseY - deltaY;

		updateTechCanvasPos();
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

async function initializeCache() {
	state.tileQueue = [];
	state.cache = new Array(CONSTANTS.MAP_SIZE_SQ);
}

async function loadMapMask(mapID, directory, wdtFileDataID) {
	const response = await fetch("/map/wdtMask?mapID=" + mapID + "&directory=" + directory + "&wdtFileDataID=" + wdtFileDataID);
	const tiles = await response.json();
	state.mask = tiles;
	if (tiles.every(fdid => fdid === 0)) {
		notify("No tiles found for this map");
	}
	state.cache = new Array(CONSTANTS.MAP_SIZE_SQ);
	state.zoomFactor = 2;

	setDefaultPosition();
	render();
	
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
	const tile = state.tileQueue.shift();
	if (tile)
		loadTile(tile);
	else
		state.awaitingTile = false;
}

function queueTile(x, y, index, tileSize) {
	const node = { x, y, index, tileSize };
	if (state.awaitingTile)
		state.tileQueue.push(node);
	else
		loadTile(node);
}

async function loadTile(tile) {
	state.awaitingTile = true;

	const cache = state.cache;

	const data = await loadMapTile(tile.x, tile.y, tile.tileSize, tile.index);
	cache[tile.index] = data;
	if (data !== false)
		render();

	checkTileQueue();
}

function setDefaultPosition() {
	let posX = 0, posY = 0;

	if (state.mask) {
		const center = Math.floor(CONSTANTS.MAP_COORD_BASE / CONSTANTS.TILE_SIZE)
		const centerIndex = state.mask[(center * CONSTANTS.MAP_SIZE) + center];

		if (centerIndex !== 0) {
			posX = ((center - 32) * CONSTANTS.TILE_SIZE) * -1;
			posY = ((center - 32) * CONSTANTS.TILE_SIZE) * -1;
		} else {
			const index = state.mask.findIndex(e => e > 0);
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
	if (state.map === null)
		return;

	if (state.mask.length === 0)
		return;

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

	for (let x = minTileY; x < maxTileY; x++) {
		for (let y = minTileX; y < maxTileX; y++) {
			const drawX = (y * tileSize) + state.offsetX;
			const drawY = (x * tileSize) + state.offsetY;

			const index = (x * CONSTANTS.MAP_SIZE) + y;
			const cached = state.cache[index];

			if (state.mask && state.mask[index] === 0)
				continue;

			if (cached === undefined) {
				state.cache[index] = true;
				queueTile(x, y, index, tileSize);
			} else if (cached instanceof ImageData) {
				ctx.putImageData(cached, drawX, drawY);
			}
		}
	}
}

function setMapPosition(x, y){
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

async function loadMapTile(x, y, size, index){
	try {
		const response = await fetch("/map/tile?fileDataID=" + state.mask[index] +"&targetSize="+ size);
		const arrayBuffer = await response.arrayBuffer();
		return new ImageData(new Uint8ClampedArray(arrayBuffer), size, size);
	} catch (e) {
		// Map tile does not exist or cannot be read.
		console.log(e);
		return false;
	}
}
