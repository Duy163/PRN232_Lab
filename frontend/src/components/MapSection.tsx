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

const getValue = (marker: MapMarkerData, view: string) => {
  switch (view) {
    case 'confirmed': return marker.confirmed;
    case 'active': return marker.active;
    case 'recovered': return marker.recovered;
    case 'deaths': return marker.deaths;
    case 'daily': return Math.max(marker.confirmed - marker.deaths - marker.recovered, 0);
    default: return marker.confirmed;
  }
};

const getColor = (value: number, max: number) => {
  const ratio = max > 0 ? value / max : 0;
  if (ratio > 0.6) return '#ef4444';      // Red for high severity
  if (ratio > 0.25) return '#f97316';     // Orange for medium-high
  if (ratio > 0.08) return '#eab308';     // Yellow for medium
  if (ratio > 0.02) return '#3b82f6';     // Blue for medium-low
  return '#10b981';                      // Green for low severity
};

function MapSection({ 
  markers, 
  view = 'confirmed', 
  onCountrySelect 
}: { 
  markers: MapMarkerData[]; 
  view?: string; 
  onCountrySelect?: (country: string) => void;
}) {
  const mapCenter: [number, number] = [20, 0];

  // Calculate maximum value for the active metric among filtered markers
  const maxVal = Math.max(...markers.map(m => getValue(m, view)), 1);

  return (
    <article id="world-map" className="panel map-panel" style={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <div className="section-head"><h2>World Wide Cases</h2><span>Real map</span></div>
      <div className="map-wrap" style={{ flex: 1, minHeight: '320px', position: 'relative' }}>
        <MapContainer center={mapCenter} zoom={2} className="map-canvas" scrollWheelZoom={false} style={{ height: '360px', width: '100%', borderRadius: '8px' }}>
          <TileLayer attribution='&copy; OpenStreetMap contributors' url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
          {markers.filter(m => m.lat != null && m.lng != null).map((marker, idx) => {
            const val = getValue(marker, view);
            const color = getColor(val, maxVal);
            // Dynamic radius based on relative cases (square root scale to prevent oversized bubbles)
            const ratio = maxVal > 0 ? val / maxVal : 0;
            const radius = Math.min(Math.max(5 + Math.sqrt(ratio) * 20, 5), 25);

            return (
              <CircleMarker
                key={`${marker.country}-${marker.name}-${idx}`}
                center={[marker.lat!, marker.lng!]}
                radius={radius}
                pathOptions={{ color: color, fillColor: color, fillOpacity: 0.6 }}
              >
                <Popup>
                  <div style={{ color: '#0f172a', fontFamily: 'sans-serif' }}>
                    <strong style={{ fontSize: '14px', display: 'block', marginBottom: '6px' }}>{marker.name}, {marker.country}</strong>
                    <div style={{ margin: '2px 0' }}>Confirmed: <strong>{marker.confirmed.toLocaleString()}</strong></div>
                    <div style={{ margin: '2px 0' }}>Active: <strong>{marker.active.toLocaleString()}</strong></div>
                    <div style={{ margin: '2px 0' }}>Deaths: <strong>{marker.deaths.toLocaleString()}</strong></div>
                    <div style={{ margin: '2px 0' }}>Recovered: <strong>{marker.recovered.toLocaleString()}</strong></div>
                    {onCountrySelect && (
                      <button
                        onClick={() => onCountrySelect(marker.country)}
                        style={{
                          marginTop: '8px',
                          background: '#3b82f6',
                          color: '#fff',
                          border: 'none',
                          padding: '6px 10px',
                          borderRadius: '4px',
                          cursor: 'pointer',
                          width: '100%',
                          fontSize: '11px',
                          fontWeight: 'bold',
                          transition: 'background 0.2s'
                        }}
                        onMouseOver={(e) => (e.currentTarget.style.background = '#2563eb')}
                        onMouseOut={(e) => (e.currentTarget.style.background = '#3b82f6')}
                      >
                        Filter by {marker.country}
                      </button>
                    )}
                  </div>
                </Popup>
              </CircleMarker>
            );
          })}
        </MapContainer>
      </div>
    </article>
  );
}

export default memo(MapSection);
