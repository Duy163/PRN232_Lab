import { memo } from 'react';
import { CircleMarker, MapContainer, Popup, TileLayer } from 'react-leaflet';

export type MapMarkerData = {
  name: string;
  country: string;
  confirmed: number;
  deaths: number;
  recovered: number;
  active: number;
  lat?: number | null;
  lng?: number | null;
};

function MapSection({ markers }: { markers: MapMarkerData[] }) {
  const colors = ['#60a5fa', '#f87171', '#4ade80', '#facc15', '#a78bfa'];
  const mapCenter: [number, number] = [20, 0];

  return (
    <article id="world-map" className="panel map-panel" style={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <div className="section-head"><h2>World Wide Cases</h2><span>Real map</span></div>
      <div className="map-wrap" style={{ flex: 1, minHeight: '320px', position: 'relative' }}>
        <MapContainer center={mapCenter} zoom={2} className="map-canvas" scrollWheelZoom={false} style={{ height: '360px', width: '100%', borderRadius: '8px' }}>
          <TileLayer attribution='&copy; OpenStreetMap contributors' url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
          {markers.filter(m => m.lat != null && m.lng != null).map((marker, idx) => (
            <CircleMarker
              key={`${marker.country}-${marker.name}-${idx}`}
              center={[marker.lat!, marker.lng!]}
              radius={Math.min(Math.max(5 + (marker.confirmed / 2000000) * 3, 5), 25)}
              pathOptions={{ color: colors[idx % colors.length], fillColor: colors[idx % colors.length], fillOpacity: 0.6 }}
            >
              <Popup>
                <div style={{ color: '#0f172a' }}>
                  <strong style={{ fontSize: '14px', display: 'block', marginBottom: '6px' }}>{marker.name}, {marker.country}</strong>
                  <div style={{ margin: '2px 0' }}>Confirmed: <strong>{marker.confirmed.toLocaleString()}</strong></div>
                  <div style={{ margin: '2px 0' }}>Active: <strong>{marker.active.toLocaleString()}</strong></div>
                  <div style={{ margin: '2px 0' }}>Deaths: <strong>{marker.deaths.toLocaleString()}</strong></div>
                  <div style={{ margin: '2px 0' }}>Recovered: <strong>{marker.recovered.toLocaleString()}</strong></div>
                </div>
              </Popup>
            </CircleMarker>
          ))}
        </MapContainer>
      </div>
    </article>
  );
}

export default memo(MapSection);
