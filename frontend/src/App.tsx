import { lazy, Suspense, useEffect, useMemo, useState } from 'react';
import 'leaflet/dist/leaflet.css';
import './styles.css';

type Country = { CountryRegionId?: number; Name?: string };
type Province = { ProvinceStateId?: number; Name?: string; CountryRegionId?: number; CountryRegion?: Country; countryRegion?: Country; Lat?: number | null; Long?: number | null };
type Confirmed = { CountryRegionId?: number | null; ProvinceStateId?: number | null; ReportDate?: string; ConfirmedCases?: number; CountryRegion?: Country; countryRegion?: Country; ProvinceState?: Province; provinceState?: Province };
type Death = { CountryRegionId?: number | null; ProvinceStateId?: number | null; ReportDate?: string; Deaths?: number; CountryRegion?: Country; countryRegion?: Country; ProvinceState?: Province; provinceState?: Province };
type Recovered = { CountryRegionId?: number | null; ProvinceStateId?: number | null; ReportDate?: string; RecoveredCases?: number; CountryRegion?: Country; countryRegion?: Country; ProvinceState?: Province; provinceState?: Province };
type DailyReport = { ProvinceStateId?: number | null; ReportDate?: string; Confirmed?: number; Deaths?: number; Recovered?: number; Active?: number; ProvinceState?: Province; provinceState?: Province; CountryRegionId?: number | null };

type CountryRow = { name: string; confirmed: number; deaths: number; recovered: number; active: number };
type ProvinceRow = { name: string; country: string; confirmed: number; deaths: number; recovered: number; active: number; lat?: number | null; lng?: number | null };
type Stats = { confirmed: number; deaths: number; recovered: number; active: number; dailyIncrease: number; latestDate: string; dailyReports: number };
type Filters = { country: string; date: string };

type DashboardData = {
  stats: Stats;
  topCountries: CountryRow[];
  provinceRows: ProvinceRow[];
  countries: string[];
  dates: string[];
  debug: {
    apiBase: string;
    fallbackApiBase: string;
    countries: number;
    provinces: number;
    confirmed: number;
    deaths: number;
    recovered: number;
    dailyReports: number;
    latestDate: string;
  };
};

const ENV_API_BASE = import.meta.env.VITE_API_BASE as string | undefined;
const DEFAULT_API_BASES = [
  ENV_API_BASE,
  'http://localhost:5284/odata',
  'https://localhost:7247/odata',
  'http://localhost:7247/odata',
  'https://localhost:5284/odata',
].filter(Boolean) as string[];

const ChartsSection = lazy(() => import('./components/ChartsSection'));
const MapSection = lazy(() => import('./components/MapSection'));

async function fetchJson<T>(baseUrl: string, path: string, query = ''): Promise<T[]> {
  const response = await fetch(`${baseUrl}/${path}${query}`);
  if (!response.ok) {
    throw new Error(`Failed to load ${path} from ${baseUrl} (${response.status})`);
  }
  const json = await response.json();
  if (Array.isArray(json)) return json as T[];
  if (Array.isArray(json.value)) return json.value as T[];
  throw new Error(`Unexpected response shape from ${path} at ${baseUrl}`);
}

async function fetchOData<T>(path: string, query = ''): Promise<{ data: T[]; source: string }> {
  const attempts = Array.from(new Set(DEFAULT_API_BASES));
  let lastError: unknown;

  for (const baseUrl of attempts) {
    try {
      const data = await fetchJson<T>(baseUrl, path, query);
      return { data, source: baseUrl };
    } catch (error) {
      lastError = error;
    }
  }

  throw lastError instanceof Error ? lastError : new Error(`Failed to load ${path}`);
}

const getCountryName = (item: { CountryRegion?: Country; countryRegion?: Country }) => item.CountryRegion?.Name || item.countryRegion?.Name || 'Unknown';
const toDateKey = (value?: string) => (value ? value.slice(0, 10) : '');
const safeDateLabel = (value?: string) => (value ? new Date(value).toLocaleDateString() : '');

async function loadDashboard(filters: Filters): Promise<DashboardData> {
  const allQuery = '?$top=5000&$expand=CountryRegion,ProvinceState';

  const [countriesRes, provincesRes, confirmedRes, deathsRes, recoveredRes, dailyReportsRes] = await Promise.all([
    fetchOData<Country>('CountryRegions', '?$top=5000'),
    fetchOData<Province>('ProvinceStates', '?$top=5000&$expand=CountryRegion'),
    fetchOData<Confirmed>('CovidGlobalConfirmeds', allQuery),
    fetchOData<Death>('CovidGlobalDeaths', allQuery),
    fetchOData<Recovered>('CovidGlobalRecovereds', allQuery),
    fetchOData<DailyReport>('DailyReport', allQuery),
  ]);

  const countries = countriesRes.data;
  const provinces = provincesRes.data;
  const confirmed = confirmedRes.data;
  const deaths = deathsRes.data;
  const recovered = recoveredRes.data;
  const dailyReports = dailyReportsRes.data;

  const allDates = Array.from(new Set([confirmed, deaths, recovered, dailyReports].flat().map((x) => toDateKey(x.ReportDate)).filter(Boolean))).sort().reverse();
  const latestDate = filters.date || allDates[0] || '';

  const latestConfirmed = filters.date ? confirmed.filter((x) => toDateKey(x.ReportDate) === toDateKey(filters.date)) : confirmed;
  const latestDeaths = filters.date ? deaths.filter((x) => toDateKey(x.ReportDate) === toDateKey(filters.date)) : deaths;
  const latestRecovered = filters.date ? recovered.filter((x) => toDateKey(x.ReportDate) === toDateKey(filters.date)) : recovered;
  const latestDaily = filters.date ? dailyReports.filter((x) => toDateKey(x.ReportDate) === toDateKey(filters.date)) : dailyReports;

  const filteredByCountry = <T extends { CountryRegion?: Country; countryRegion?: Country }>(items: T[]) => filters.country === 'All' ? items : items.filter((x) => getCountryName(x) === filters.country);

  const sum = <T,>(items: T[], getter: (x: T) => number | undefined) => items.reduce((acc, cur) => acc + (getter(cur) ?? 0), 0);
  const filteredConfirmed = filteredByCountry(latestConfirmed);
  const filteredDeaths = filteredByCountry(latestDeaths);
  const filteredRecovered = filteredByCountry(latestRecovered);

  const readNumber = (item: unknown, keys: string[]) => {
    if (!item || typeof item !== 'object') return 0;
    const record = item as Record<string, unknown>;
    for (const key of keys) {
      const value = record[key];
      if (typeof value === 'number') return value;
    }
    return 0;
  };

  const getRecoveredValue = (item?: Recovered) => readNumber(item, ['RecoveredCases', 'Recovered', 'countryRecovered']);

  const confirmedSum = sum(filteredConfirmed, (x) => x.ConfirmedCases);
  const deathsSum = sum(filteredDeaths, (x) => x.Deaths);
  const recoveredSum = sum(filteredRecovered, getRecoveredValue);
  const filteredDaily = filteredByCountry(latestDaily);
  const activeSum = sum(filteredDaily, (x) => x.Active);

  const byCountry = new Map<string, CountryRow>();
  [...latestConfirmed, ...latestDeaths, ...latestRecovered].forEach((item) => {
    const name = getCountryName(item);
    const current = byCountry.get(name) || { name, confirmed: 0, deaths: 0, recovered: 0, active: 0 };
    current.confirmed += item.ConfirmedCases ?? 0;
    current.deaths += item.Deaths ?? 0;
    current.recovered += getRecoveredValue(item as Recovered);
    current.active = Math.max(current.confirmed - current.deaths - current.recovered, 0);
    byCountry.set(name, current);
  });
  const topCountries = Array.from(byCountry.values()).sort((a, b) => b.confirmed - a.confirmed).slice(0, 8);

  const provinceRows = provinces
    .filter((p) => filters.country === 'All' || (p.CountryRegion?.Name || p.countryRegion?.Name || 'Unknown') === filters.country)
    .map((p) => {
      const relatedDaily = latestDaily.find((d) => d.ProvinceStateId === p.ProvinceStateId);
      return {
        name: p.Name || 'Unknown',
        country: p.CountryRegion?.Name || p.countryRegion?.Name || 'Unknown',
        confirmed: relatedDaily?.Confirmed ?? 0,
        deaths: relatedDaily?.Deaths ?? 0,
        recovered: relatedDaily?.Recovered ?? 0,
        active: relatedDaily?.Active ?? 0,
        lat: p.Lat,
        lng: p.Long,
      };
    })
    .filter((x) => x.confirmed > 0);

  return {
    stats: {
      confirmed: confirmedSum,
      deaths: deathsSum,
      recovered: recoveredSum,
      active: activeSum,
      dailyIncrease: Math.max(confirmedSum - deathsSum - recoveredSum, 0),
      latestDate,
      dailyReports: latestDaily.length,
    },
    topCountries,
    provinceRows,
    countries: ['All', ...countries.map((c) => c.Name || 'Unknown').filter(Boolean)],
    dates: allDates,
    debug: {
      apiBase: ENV_API_BASE || DEFAULT_API_BASES[0] || '',
      fallbackApiBase: DEFAULT_API_BASES.slice(1).join(', '),
      countries: countries.length,
      provinces: provinces.length,
      confirmed: confirmed.length,
      deaths: deaths.length,
      recovered: recovered.length,
      dailyReports: dailyReports.length,
      latestDate,
    },
  };
}

export default function App() {
  const [stats, setStats] = useState<Stats>({ confirmed: 0, deaths: 0, recovered: 0, active: 0, dailyIncrease: 0, latestDate: '', dailyReports: 0 });
  const [topCountries, setTopCountries] = useState<CountryRow[]>([]);
  const [provinceRows, setProvinceRows] = useState<ProvinceRow[]>([]);
  const [filters, setFilters] = useState<Filters>({ country: 'All', date: '' });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [countryOptions, setCountryOptions] = useState<string[]>(['All']);
  const [dateOptions, setDateOptions] = useState<string[]>([]);
  const [activeTab, setActiveTab] = useState<'overview' | 'map' | 'charts' | 'reports'>('overview');
  const [view, setView] = useState<'confirmed' | 'active' | 'recovered' | 'deaths' | 'daily'>('confirmed');
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 10;

  useEffect(() => {
    setLoading(true);
    setError(null);
    loadDashboard(filters)
      .then(({ stats, topCountries, provinceRows, countries, dates }) => {
        setStats(stats);
        setTopCountries(topCountries);
        setProvinceRows(provinceRows);
        setCountryOptions(countries);
        if (!filters.date) setDateOptions(dates);
        setCurrentPage(1);
      })
      .catch((err: unknown) => setError(err instanceof Error ? err.message : 'Unknown error'))
      .finally(() => setLoading(false));
  }, [filters]);

  const totalPages = Math.ceil(provinceRows.length / itemsPerPage);
  const paginatedRows = useMemo(() => {
    const start = (currentPage - 1) * itemsPerPage;
    return provinceRows.slice(start, start + itemsPerPage);
  }, [provinceRows, currentPage]);

  return (
    <div className="app-shell shell-mockup">
      <aside className="sidebar mock-sidebar">
        <div className="brand brand-mock">
          <div className="brand-icon">✦</div>
          <div>
            <strong>COVID-19 Data</strong>
            <span>Global Analysis</span>
          </div>
        </div>
        <nav className="nav-mock">
          <button type="button" className={view === 'confirmed' ? 'active' : ''} onClick={() => setView('confirmed')}>
            <span>Confirmed</span>
            <span style={{ fontSize: '13px', opacity: 0.9 }}>{stats.confirmed.toLocaleString()}</span>
          </button>
          <button type="button" className={view === 'active' ? 'active' : ''} onClick={() => setView('active')}>
            <span>Active</span>
            <span style={{ fontSize: '13px', opacity: 0.9 }}>{stats.active.toLocaleString()}</span>
          </button>
          <button type="button" className={view === 'recovered' ? 'active' : ''} onClick={() => setView('recovered')}>
            <span>Recovered</span>
            <span style={{ fontSize: '13px', opacity: 0.9 }}>{stats.recovered.toLocaleString()}</span>
          </button>
          <button type="button" className={view === 'deaths' ? 'active' : ''} onClick={() => setView('deaths')}>
            <span>Deaths</span>
            <span style={{ fontSize: '13px', opacity: 0.9 }}>{stats.deaths.toLocaleString()}</span>
          </button>
          <button type="button" className={view === 'daily' ? 'active' : ''} onClick={() => setView('daily')}>
            <span>Daily Increase</span>
            <span style={{ fontSize: '13px', opacity: 0.9 }}>{stats.dailyIncrease.toLocaleString()}</span>
          </button>
        </nav>
        <button type="button" className="detail-btn" onClick={() => setActiveTab('reports')}>
          Detailed Report
        </button>
      </aside>

      <main className="content mock-content">
        <header className="topbar topbar-mock">
          <div className="topbar-title">Global COVID-19 Tracker</div>
          <nav className="tabs-mock">
            <button type="button" className={activeTab === 'overview' ? 'active' : ''} onClick={() => setActiveTab('overview')}>
              Overview
            </button>
            <button type="button" className={activeTab === 'map' ? 'active' : ''} onClick={() => setActiveTab('map')}>
              Map
            </button>
            <button type="button" className={activeTab === 'charts' ? 'active' : ''} onClick={() => setActiveTab('charts')}>
              Charts
            </button>
            <button type="button" className={activeTab === 'reports' ? 'active' : ''} onClick={() => setActiveTab('reports')}>
              News
            </button>
          </nav>
          <div style={{ display: 'flex', gap: '14px', alignItems: 'center' }}>
            <div className="filters-group" style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
              <select 
                value={filters.country} 
                onChange={(e) => setFilters(prev => ({ ...prev, country: e.target.value }))}
                style={{ background: '#111b2e', color: '#cfe4ff', border: '1px solid rgba(255,255,255,0.12)', borderRadius: '6px', padding: '6px 10px', fontSize: '13px', cursor: 'pointer', outline: 'none' }}
              >
                {countryOptions.map(c => <option key={c} value={c}>{c === 'All' ? 'All Countries' : c}</option>)}
              </select>
              <select 
                value={filters.date} 
                onChange={(e) => setFilters(prev => ({ ...prev, date: e.target.value }))}
                style={{ background: '#111b2e', color: '#cfe4ff', border: '1px solid rgba(255,255,255,0.12)', borderRadius: '6px', padding: '6px 10px', fontSize: '13px', cursor: 'pointer', outline: 'none' }}
              >
                <option value="">All Dates</option>
                {dateOptions.map(d => <option key={d} value={d}>{safeDateLabel(d)}</option>)}
              </select>
            </div>
          </div>
        </header>

        {loading && <div className="panel">Loading dashboard data...</div>}
        {error && <div className="panel error">{error}</div>}

        {!loading && !error && (
          <>
            {activeTab === 'overview' && (
              <>
                <section className="hero panel" style={{ marginTop: '10px' }}>
                  <div className="hero-head">
                    <h1># of Cases World wide</h1>
                    <div className="metric-tabs">
                      {(['confirmed','active','recovered','deaths','daily'] as const).map((item) => (
                        <button key={item} className={view === item ? 'active' : ''} onClick={() => setView(item)}>
                          {item === 'daily' ? 'Daily Increase' : item.charAt(0).toUpperCase() + item.slice(1)}
                        </button>
                      ))}
                    </div>
                  </div>
                  <div className="hero-map panel-inner">
                    <Suspense fallback={<div className="mock-loading">Loading map...</div>}>
                      <MapSection markers={provinceRows} />
                    </Suspense>
                    <div className="hero-caption">
                      <strong>Interactive Choropleth Map View</strong>
                      <span>Real province coordinates and dynamics</span>
                    </div>
                  </div>
                </section>

                <section className="treemap panel" style={{ marginTop: '16px' }}>
                  <div className="treemap-head">
                    <h2>Treemap of Countries</h2>
                    <p>The Treemap shows the number of Cases in different countries and their percent of total cases worldwide</p>
                  </div>
                  <div className="mock-treemap">
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '12px' }}>
                      {topCountries.slice(0, 4).map((country, idx) => {
                        const totalMetric = view === 'confirmed' ? stats.confirmed
                                          : view === 'active' ? stats.active
                                          : view === 'recovered' ? stats.recovered
                                          : view === 'deaths' ? stats.deaths
                                          : stats.dailyIncrease;
                        const value = view === 'confirmed' ? country.confirmed
                                    : view === 'active' ? country.active
                                    : view === 'recovered' ? country.recovered
                                    : view === 'deaths' ? country.deaths
                                    : Math.max(country.confirmed - country.deaths - country.recovered, 0);
                        const percent = totalMetric > 0 ? Math.round((value / totalMetric) * 100) : 0;
                        return (
                          <div key={country.name} className={`tile tile-${idx}`} style={{ display: 'flex', flexDirection: 'column', justifyContent: 'space-between' }}>
                            <strong>{country.name}</strong>
                            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
                              <span style={{ fontSize: '20px', fontWeight: 'bold' }}>{value.toLocaleString()}</span>
                              {percent > 0 && <span style={{ fontSize: '12px', opacity: 0.8 }}>{percent}%</span>}
                            </div>
                          </div>
                        );
                      })}
                    </div>
                    <aside className="side-legend">
                      {([
                        { key: 'confirmed', label: 'Confirmed' },
                        { key: 'active', label: 'Active' },
                        { key: 'recovered', label: 'Recovered' },
                        { key: 'deaths', label: 'Deaths' }
                      ] as const).map((item) => (
                        <div 
                          key={item.key} 
                          className={view === item.key ? 'selected' : ''} 
                          style={{ cursor: 'pointer', transition: 'all 0.2s ease' }}
                          onClick={() => setView(item.key)}
                        >
                          {item.label}
                        </div>
                      ))}
                    </aside>
                  </div>
                </section>
              </>
            )}

            {activeTab === 'map' && (
              <section className="hero panel" style={{ marginTop: '10px', height: 'calc(100vh - 120px)', display: 'flex', flexDirection: 'column' }}>
                <div className="hero-head">
                  <h1># of Cases World wide</h1>
                  <div className="metric-tabs">
                    {(['confirmed','active','recovered','deaths','daily'] as const).map((item) => (
                      <button key={item} className={view === item ? 'active' : ''} onClick={() => setView(item)}>
                        {item === 'daily' ? 'Daily Increase' : item.charAt(0).toUpperCase() + item.slice(1)}
                      </button>
                    ))}
                  </div>
                </div>
                <div className="hero-map panel-inner" style={{ flex: 1, display: 'flex', flexDirection: 'column' }}>
                  <Suspense fallback={<div className="mock-loading">Loading map...</div>}>
                    <MapSection markers={provinceRows} />
                  </Suspense>
                </div>
              </section>
            )}

            {activeTab === 'charts' && (
              <section className="grid dashboard-grid mock-grid" style={{ marginTop: '10px' }}>
                <Suspense fallback={<article className="panel chart-panel">Loading chart...</article>}>
                  <ChartsSection topCountries={topCountries} />
                </Suspense>
                <article id="countries" className="panel list-panel">
                  <div className="section-head"><h2>Countries</h2><span>Top 8 by confirmed</span></div>
                  <div className="country-list">
                    {topCountries.map((country) => (
                      <div key={country.name} className="country-row">
                        <div>
                          <strong>{country.name}</strong>
                          <span>Deaths: {country.deaths.toLocaleString()} • Recovered: {country.recovered.toLocaleString()}</span>
                        </div>
                        <b>{country.confirmed.toLocaleString()}</b>
                      </div>
                    ))}
                  </div>
                </article>
              </section>
            )}

            {activeTab === 'reports' && (
              <section id="reports" className="panel bottom-panel" style={{ marginTop: '10px' }}>
                <div className="section-head"><h2>Province States</h2><span>Detailed country/province table</span></div>
                <div className="table-wrap">
                  <table className="data-table">
                    <thead>
                      <tr>
                        <th>Province</th>
                        <th>Country</th>
                        <th>Confirmed</th>
                        <th>Deaths</th>
                        <th>Recovered</th>
                        <th>Active</th>
                      </tr>
                    </thead>
                    <tbody>
                      {paginatedRows.map((row) => (
                        <tr key={`${row.country}-${row.name}`}>
                          <td>{row.name}</td>
                          <td>{row.country}</td>
                          <td>{row.confirmed.toLocaleString()}</td>
                          <td>{row.deaths.toLocaleString()}</td>
                          <td>{row.recovered.toLocaleString()}</td>
                          <td>{row.active.toLocaleString()}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
                {totalPages > 1 && (
                  <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: '12px', marginTop: '16px' }}>
                    <button 
                      type="button" 
                      onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                      disabled={currentPage === 1}
                      style={{ background: 'rgba(255,255,255,0.05)', color: currentPage === 1 ? '#64748b' : '#cfe4ff', border: '1px solid rgba(255,255,255,0.08)', borderRadius: '6px', padding: '6px 12px', fontSize: '13px', cursor: currentPage === 1 ? 'not-allowed' : 'pointer' }}
                    >
                      Previous
                    </button>
                    <span style={{ fontSize: '13px', color: '#b6c3dd' }}>
                      Page <strong>{currentPage}</strong> of <strong>{totalPages}</strong> (Total {provinceRows.length} items)
                    </span>
                    <button 
                      type="button" 
                      onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                      disabled={currentPage === totalPages}
                      style={{ background: 'rgba(255,255,255,0.05)', color: currentPage === totalPages ? '#64748b' : '#cfe4ff', border: '1px solid rgba(255,255,255,0.08)', borderRadius: '6px', padding: '6px 12px', fontSize: '13px', cursor: currentPage === totalPages ? 'not-allowed' : 'pointer' }}
                    >
                      Next
                    </button>
                  </div>
                )}
              </section>
            )}
          </>
        )}
      </main>
    </div>
  );
}
