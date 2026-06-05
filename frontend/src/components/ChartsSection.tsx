import { memo, Suspense } from 'react';
import { Bar, BarChart, CartesianGrid, Cell, Legend, Pie, PieChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';

type CountryRow = { name: string; confirmed: number; deaths: number; recovered: number };

function ChartsSection({ topCountries, variant = 'bar' }: { topCountries: CountryRow[]; variant?: 'bar' | 'pie' }) {
  const colors = ['#60a5fa', '#f87171', '#4ade80', '#facc15', '#a78bfa'];

  if (variant === 'pie') {
    const pieData = topCountries.slice(0, 5).map((x) => ({ name: x.name, value: x.confirmed }));
    return (
      <article className="panel chart-panel">
        <div className="section-head"><h2>Distribution</h2><span>Confirmed share</span></div>
        <ResponsiveContainer width="100%" height={320}>
          <PieChart>
            <Pie data={pieData} dataKey="value" nameKey="name" innerRadius={70} outerRadius={110} paddingAngle={4}>
              {pieData.map((_, index) => <Cell key={index} fill={colors[index % colors.length]} />)}
            </Pie>
            <Tooltip />
            <Legend />
          </PieChart>
        </ResponsiveContainer>
      </article>
    );
  }

  return (
    <article className="panel chart-panel">
      <div className="section-head"><h2>Top Countries - Confirmed</h2><span>Interactive chart</span></div>
      <ResponsiveContainer width="100%" height={320}>
        <BarChart data={topCountries}>
          <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.08)" />
          <XAxis dataKey="name" tick={{ fill: '#cbd5e1', fontSize: 12 }} />
          <YAxis tick={{ fill: '#cbd5e1' }} />
          <Tooltip />
          <Legend />
          <Bar dataKey="confirmed" fill="#60a5fa" radius={[8, 8, 0, 0]} />
          <Bar dataKey="deaths" fill="#f87171" radius={[8, 8, 0, 0]} />
          <Bar dataKey="recovered" fill="#4ade80" radius={[8, 8, 0, 0]} />
        </BarChart>
      </ResponsiveContainer>
    </article>
  );
}

export default memo(ChartsSection);
