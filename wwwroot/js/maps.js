/* UI */
const Elements =
{
	Maps: document.getElementById('js-map-select'),
	Versions: document.getElementById('js-version-select'),
	PrevMap: document.getElementById('js-version-prev'),
	NextMap: document.getElementById('js-version-next'),
	Sidebar: document.getElementById('js-sidebar'),
	Map: document.getElementById('js-map'),
	TechBox: document.getElementById('js-techbox'),
	Layers: document.getElementById('js-layers'),
	FlightLayer: document.getElementById('js-flightlayer'),
	POILayer: document.getElementById('js-poilayer'),
	ADTGrid: document.getElementById('js-adtgrid'),
	MNAM: document.getElementById('js-mnam')
};

const Current =
{
	BuildName: "",
	Map: false,
	InternalMap: false,
	InternalMapID: false,
	Version: 0,
	wdtFileDataID: 0
};

var d = function (text) { console.log(text); };

// Sidebar button
document.getElementById('js-sidebar-button').addEventListener('click', function () {
	Elements.Sidebar.classList.toggle('closed');
	document.getElementById('js-sidebar-button').classList.toggle('closed');
});

// Layer button
document.getElementById('js-layers-button').addEventListener('click', function () {
	Elements.Layers.classList.toggle('closed');
});

(async () => {
	d("initializing");
	var maps = await InitializeMapList();
	await InitializeMapOptions(maps);
	await InitializeEvents();
	render();
})();

async function InitializeMapList() {
	d("requesting map list");
	var response = await fetch('/map/list');
	var json = await response.json();
	d("got map list");
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
			Current.InternalMap = map.internal;
			Current.InternalMapID = map.internal_mapid;
			Current.wdtFileDataID = map.wdtFileDataID;
			Current.Version = '' + parseInt(url[3], 10);
			if (map.internal === decodeURIComponent(url[2])) {
				option.selected = true;
			}
		}
	});

	Elements.Maps.appendChild(fragment);

	d('Initialized map ' + Current.Map + ' on version ' + Current.Version);
}

async function InitializeEvents() {
	d("initializing events");
	var select2El = $("#js-map-select").select2({ matcher: wowMapMatcher, disabled: false });
	Elements.MapSelect2 = select2El;
	Elements.MapSelect2.on('change', function (e) {
		Current.Map = this.value;
		Current.InternalMap = this.options[this.selectedIndex].dataset.internal;
		Current.InternalMapID = this.options[this.selectedIndex].dataset.imapid;
		Current.wdtFileDataID = this.options[this.selectedIndex].dataset.wdtfiledataid;
		loadMapMask(Current.Map, Current.InternalMap, Current.wdtFileDataID);
	});

	//Elements.FlightLayer.addEventListener('click', function () {
	//	if (Elements.FlightLayer.checked) {
	//		d('Enabled flight paths');
	//		if (Current.InternalMapID == undefined) {
	//			d("Unknown mapid, can't request fps");
	//			return;
	//		}
	//		fpxhr.open('GET', '/maps/api.php?type=flightpaths&build=' + Versions[Current.Map][Current.Version].fullbuild + '&mapid=' + Current.InternalMapID);
	//		fpxhr.responseType = 'json';
	//		fpxhr.send();
	//	} else {
	//		d('Disabled flight paths');
	//		LeafletMap.removeLayer(FlightPathLayer);
	//		FlightPathLayer = new L.LayerGroup();
	//	}
	//});

	//Elements.POILayer.addEventListener('click', function () {
	//	if (Elements.POILayer.checked) {
	//		d('Enabled POIs');
	//		if (Current.InternalMapID == undefined) {
	//			d("Unknown mapid, can't request POIs");
	//			return;
	//		}

	//		var poixhr = new XMLHttpRequest();
	//		poixhr.responseType = 'json';
	//		poixhr.onreadystatechange = function () {
	//			if (poixhr.readyState === 4) {
	//				ProcessPOIResult(poixhr.response);
	//			}
	//		}

	//		poixhr.open('GET', '/maps/api.php?type=pois&build=' + Versions[Current.Map][Current.Version].fullbuild + '&mapid=' + Current.InternalMapID, true);
	//		poixhr.send();
	//	} else {
	//		d('Disabled POIs')
	//		LeafletMap.removeLayer(POILayer);
	//		POILayer = new L.LayerGroup();
	//	}
	//});

	//Elements.ADTGrid.addEventListener('click', function () {
	//	if (Elements.ADTGrid.checked) {
	//		d('Enabled ADT grid');
	//		ADTGridLayer = new L.LayerGroup();
	//		ADTGridTextLayer = new L.LayerGroup();
	//		for (var x = 0; x < 64; x++) {
	//			for (var y = 0; y < 64; y++) {
	//				var fromlat = WoWtoLatLng(maxSize - (x * adtSize), -maxSize);
	//				var tolat = WoWtoLatLng(maxSize - (x * adtSize), maxSize);
	//				ADTGridLayer.addLayer(new L.polyline([fromlat, tolat], { weight: 0.1, color: 'red' }));

	//				var fromlat = WoWtoLatLng(maxSize, maxSize - (x * adtSize));
	//				var tolat = WoWtoLatLng(-maxSize, maxSize - (x * adtSize));
	//				ADTGridLayer.addLayer(new L.polyline([fromlat, tolat], { weight: 0.1, color: 'red' }));
	//			}
	//		}
	//		refreshADTGrid();
	//		ADTGridLayer.addTo(LeafletMap);
	//	} else {
	//		d('Disabled ADT grid')
	//		LeafletMap.removeLayer(ADTGridLayer);
	//		LeafletMap.removeLayer(ADTGridTextLayer);
	//	}
	//});

	//LeafletMap.on('moveend zoomend dragend', function () {
	//	SynchronizeTitleAndURL();
	//	if (Elements.ADTGrid.checked) {
	//		refreshADTGrid();
	//	}
	//});

	//LeafletMap.on('click', function (e) {
	//	ProcessOffsetClick(e, Versions[Current.Map][Current.Version].config.offset.min);
	//});

	Elements.Maps.disabled = false;
	d("initialized events");
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
const CONSTANTS = {
	MAP_SIZE: 64,
	MAP_SIZE_SQ: 4096, // 64x64
	MAP_COORD_BASE: 51200 / 3, // 17066,66666666667
	TILE_SIZE: (51200 / 3) / 32, // 533,3333333333333
	MAP_OFFSET: 17066,
}

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
	zoom: 7,
	ready: false,
	map: 0 // Azeroth?
};

const mapCanvas = document.getElementById('map-canvas');
state.canvas = mapCanvas;

const techBoxCanvasPosX = document.getElementById('canvasPosX');
const techBoxCanvasPosY = document.getElementById('canvasPosY');
const techBoxMousePosX = document.getElementById('mousePosX');
const techBoxMousePosY = document.getElementById('mousePosY');
const techBoxWoWPosX = document.getElementById('wowPosX');
const techBoxWoWPosY = document.getElementById('wowPosY');
const techBoxWoWPosADTX = document.getElementById('adtPosX');
const techBoxWoWPosADTY = document.getElementById('adtPosY');
const techBoxZoom = document.getElementById('zoomLevel');


function updateTechCanvasPos() {
	techBoxCanvasPosX.innerHTML = Math.round(state.offsetX);
	techBoxCanvasPosY.innerHTML = Math.round(state.offsetY);
}

function updateTechMousePos(x, y) {
	techBoxMousePosX.innerHTML = Math.round(x);
	techBoxMousePosY.innerHTML = Math.round(y);
}

function updateTechWoWPos(x, y) {
	var wowPos = mapPositionFromClientPoint(x, y);
	techBoxWoWPosX.innerHTML = Math.round(wowPos.posX, 3);
	techBoxWoWPosY.innerHTML = Math.round(wowPos.posY, 3);
	techBoxWoWPosADTX.innerHTML = Math.round(wowPos.tileX, 3);
	techBoxWoWPosADTY.innerHTML = Math.round(wowPos.tileY, 3);
}


function updateTechZoom() {
	techBoxZoom.innerHTML = state.zoomFactor;
}
mapCanvas.addEventListener('mouseup', function (e) {
	console.log('mouseup');
	if (state.isPanning)
		state.isPanning = false;
});

mapCanvas.addEventListener('mouseout', function (e) {
	console.log('mouseout');
	if (state.isPanning)
		state.isPanning = false;
});

mapCanvas.addEventListener('mousemove', function (event) {
	updateTechMousePos(event.clientX, event.clientY);
	updateTechWoWPos(event.clientX, event.clientY);
});

mapCanvas.addEventListener('mousedown', function (event) {
	console.log('mousedown');
	if (!state.isPanning) {
		state.isPanning = true;

		// Store the X/Y of the mouse event to calculate drag deltas.
		this.mouseBaseX = event.clientX;
		this.mouseBaseY = event.clientY;

		// Store the current offsetX/offsetY used for relative panning
		// as the user drags the component.
		this.panBaseX = state.offsetX;
		this.panBaseY = state.offsetY;

		updateTechCanvasPos();
	}
});

mapCanvas.addEventListener('mousemove', function (event) {
	if (state.isPanning) {
		// Calculate the distance from our mousedown event.
		const deltaX = this.mouseBaseX - event.clientX;
		const deltaY = this.mouseBaseY - event.clientY;

		// Update the offset based on our pan base.
		state.offsetX = this.panBaseX - deltaX;
		state.offsetY = this.panBaseY - deltaY;
		// Offsets are not reactive, manually trigger an update.

		updateTechCanvasPos();
		render();
	}
});

mapCanvas.addEventListener('mousewheel', function (event) {
	const delta = event.deltaY > 0 ? 1 : -1;
	const newZoom = Math.max(1, Math.min(state.zoom, state.zoomFactor + delta));

	// Setting the new zoom factor even if it hasn't changed would have no effect due to
	// the zoomFactor watcher being reactive, but we still check it here so that we only
	// pan the map to the new zoom point if we're actually zooming.
	if (newZoom !== state.zoomFactor) {
		// Get the in-game position of the mouse cursor.
		const point = mapPositionFromClientPoint(event.clientX, event.clientY);

		// Set the new zoom factor. This will not trigger a re-render.
		setZoomFactor(newZoom);

		// Pan the map to the cursor position.
		setMapPosition(point.posX, point.posY);
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
		d("No tiles found for map " + mapID + " in directory " + directory + " with wdtFileDataID " + wdtFileDataID);
	}
	state.cache = new Array(CONSTANTS.MAP_SIZE_SQ);
	setDefaultPosition();
	render();
	
	return tiles;
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

		if (centerIndex === 0) {
			const index = state.mask.findIndex(e => e > 1);

			if (index > -1) {
				// Translate the index into chunk co-ordinates, expand those to in-game co-ordinates
				// and then offset by half a chunk so that we are centered on the chunk.
				const chunkX = index % CONSTANTS.MAP_SIZE;
				const chunkY = Math.floor(index / CONSTANTS.MAP_SIZE);
				posX = ((chunkX - 32) * CONSTANTS.TILE_SIZE) * -1;
				posY = ((chunkY - 32) * CONSTANTS.TILE_SIZE) * -1;
			}
		}
	}

	setMapPosition(posX, posY);
}

function render() {
	if (state.map === null)
		return;

	// No canvas reference? Component likely dismounting.
	const canvas = state.canvas;
	if (!canvas)
		return;

	if (state.mask.length === 0)
		return;
	// Update the internal canvas dimensions to match the element.
	canvas.width = canvas.offsetWidth;
	canvas.height = canvas.offsetHeight;
	// Viewport width/height defines what is visible to the user.
	const viewportWidth = canvas.clientWidth;
	const viewportHeight = canvas.clientHeight;

	// Calculate which tiles will appear within the viewer.
	const tileSize = Math.floor(state.tileSize / state.zoomFactor);

	// Get local reference to the canvas context.
	const ctx = canvas.getContext("2d");

	// We need to use a local reference to the cache so that async callbacks
	// for tile loading don't overwrite the most current cache if they resolve
	// after a new map has been selected.
	const cache = state.cache;
	// Iterate over all possible tiles in a map and render as needed.
	for (let x = 0; x < CONSTANTS.MAP_SIZE; x++) {
		for (let y = 0; y < CONSTANTS.MAP_SIZE; y++) {
			// drawX/drawY is the absolute position to draw this tile.
			const drawX = (y * tileSize) + state.offsetX;
			const drawY = (x * tileSize) + state.offsetY;

			// Cache is a one-dimensional array, calculate the index as such.
			const index = (x * CONSTANTS.MAP_SIZE) + y;
			const cached = cache[index];

			// This chunk is masked out, so skip rendering it.
			if (state.mask && state.mask[index] === 0) {
				continue;
			}

			// Skip tiles that are not in (or around) the viewport.
			if (drawX > (viewportWidth + tileSize) || drawY > (viewportHeight + tileSize) || drawX + tileSize < -tileSize || drawY + tileSize < -tileSize) {

				// Clear out cache entries for tiles no longer in viewport.
				//if (cached !== undefined) {
				//	ctx.clearRect(drawX, drawY, tileSize, tileSize);
				//	cache[index] = undefined;
				//}

				continue;
			}
			// No cache, request it (async) then skip.
			if (cached === undefined) {
				// Set the tile cache to 'true' so it is skipped while loading.
				cache[index] = true;
				// Add this tile to the loading queue.
				queueTile(x, y, index, tileSize);
			} else if (cached instanceof ImageData) {
				// If the tile is renderable, render it.
				ctx.putImageData(cached, drawX, drawY);
			}
		}
	}
}

function setMapPosition(x, y){
	// Translate to WoW co-ordinates.
	const posX = y;
	const posY = x;

	const tileSize = Math.floor(state.tileSize / state.zoomFactor);

	const ofsX = (((posX - CONSTANTS.MAP_COORD_BASE) / CONSTANTS.TILE_SIZE) * tileSize);
	const ofsY = (((posY - CONSTANTS.MAP_COORD_BASE) / CONSTANTS.TILE_SIZE) * tileSize);

	state.offsetX = ofsX + (state.canvas.clientWidth / 2);
	state.offsetY = ofsY + (state.canvas.clientHeight / 2);

	updateTechCanvasPos();
	console.log("Setting map pos to " + state.offsetX + ", " + state.offsetY);
	render();
}

function mapPositionFromClientPoint(x, y) {
	const viewOfsX = (x - state.canvas.clientWidth) - state.offsetX;
	const viewOfsY = (y - state.canvas.clientHeight) - state.offsetY;
	const tileSize = Math.floor(state.tileSize / state.zoomFactor);
	
	const tileX = viewOfsX / tileSize;
	const tileY = viewOfsY / tileSize;

	const posX = CONSTANTS.MAP_COORD_BASE - (CONSTANTS.TILE_SIZE * tileX);
	const posY = CONSTANTS.MAP_COORD_BASE - (CONSTANTS.TILE_SIZE * tileY);

	return { tileX: Math.floor(tileX), tileY: Math.floor(tileY), posX: posY, posY: posX };
}

function setZoomFactor(factor) {
	state.zoomFactor = factor;
	updateTechZoom();
	initializeCache();
}

async function loadMapTile(x, y, size, index){
	try {
		const response = await fetch("/casc/fdid?fileDataID=" + state.mask[index] + "&filename=map32_32.blp");
		const arrayBuffer = await response.arrayBuffer();
		let data = new Bufo(arrayBuffer);
		const blp = new BLPFile(data);
		const canvas = new OffscreenCanvas(blp.width, blp.height);
		blp.getPixels(0, canvas, 0, 0);

		// Scale the image down by copying the raw canvas onto a
		// scaled canvas, and then returning the scaled image data.
		const scale = size / blp.width;
		const scaled = document.createElement('canvas');
		scaled.width = size;
		scaled.height = size;

		const ctx = scaled.getContext('2d');
		if (ctx === null)
			throw new Error('Unable to get 2D context for canvas');

		ctx.scale(scale, scale);
		ctx.drawImage(canvas, 0, 0);
		return ctx.getImageData(0, 0, size, size);
	} catch (e) {
		// Map tile does not exist or cannot be read.
		console.log(e);
		return false;
	}
}
