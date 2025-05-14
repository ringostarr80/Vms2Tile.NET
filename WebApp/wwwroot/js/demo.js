import 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.js';
import 'https://cdn.jsdelivr.net/npm/@locr-company/leaflet-gridlayer-vms2@1.0.0/src/Leaflet.GridLayer.VMS2.js';

const map = L.map('map', {
    minZoom: 0,
    maxZoom: 19
});
map.setView([37.45523781879053, 14.026794433593752], 9);

const vms2Layer = L.gridLayer.vms2({
    attribution: '&copy; <a href="https://maps.locr.com">locr</a>/<a href="https://osm.org/copyright">OpenStreetMap</a>/<a href="https://leafletjs.com/">Leaflet</a>',
    style: '4203',
    tileUrl: '/api/tile/{z}/{y}/{x}?k={key}&v={value}&t={type}'
});
vms2Layer.addTo(map);