import React from 'react'
import { Link } from 'react-router-dom'
import { useAuthStore } from '@store/authStore'
import { ArrowRight, BadgeCheck, Briefcase, Building2, Search, Sparkles, UsersRound } from 'lucide-react'

const HomePage: React.FC = () => {
  const { isAuthenticated, user } = useAuthStore()
  const dashboard = user?.role === 'Recruiter' ? '/recruiter' : user?.role === 'Admin' ? '/admin' : '/dashboard'

  return <div className="overflow-hidden bg-[#f7f8fc]">
    <section className="relative isolate">
      <div className="absolute inset-x-0 top-0 -z-10 h-[540px] bg-[radial-gradient(circle_at_72%_20%,#dfe4ff_0,transparent_28%),radial-gradient(circle_at_20%_20%,#e8f7ff_0,transparent_24%)]" />
      <div className="container px-4 py-16 sm:py-24">
        <div className="grid items-center gap-12 lg:grid-cols-[1.05fr_.95fr]">
          <div>
            <p className="eyebrow mb-5 flex items-center gap-2"><Sparkles size={15} /> Modern recruiting, made human</p>
            <h1 className="max-w-3xl text-5xl font-bold leading-[1.06] tracking-tight text-slate-950 sm:text-6xl">Find work that <span className="text-indigo-600">fits your future.</span></h1>
            <p className="mt-6 max-w-xl text-lg leading-8 text-slate-600">HireSphere connects ambitious people with teams worth joining—through a calmer, smarter hiring experience.</p>
            <div className="mt-9 flex flex-wrap gap-3">
              <Link to={isAuthenticated ? dashboard : '/jobs'} className="btn btn-primary">{isAuthenticated ? 'Open my workspace' : 'Explore opportunities'} <ArrowRight size={18} /></Link>
              {!isAuthenticated && <Link to="/register" className="btn btn-secondary">Create free account</Link>}
            </div>
            <div className="mt-10 flex flex-wrap gap-x-6 gap-y-3 text-sm font-medium text-slate-600">
              <span className="flex items-center gap-2"><BadgeCheck size={17} className="text-emerald-500" /> Verified companies</span>
              <span className="flex items-center gap-2"><BadgeCheck size={17} className="text-emerald-500" /> Clear application status</span>
            </div>
          </div>
          <div className="relative mx-auto w-full max-w-lg">
            <div className="rounded-[2rem] border border-white bg-white p-5 shadow-2xl shadow-indigo-200/50">
              <div className="flex items-center justify-between"><div><p className="text-xs font-semibold uppercase tracking-widest text-slate-400">Recommended for you</p><h2 className="mt-1 text-xl font-bold">Your next role is closer</h2></div><span className="rounded-full bg-indigo-50 p-3 text-indigo-600"><Sparkles size={20} /></span></div>
              <div className="mt-5 space-y-3">
                {[['Product Designer','Northstar Labs','94% match','bg-violet-100 text-violet-600'],['Frontend Engineer','Orbit Studio','89% match','bg-cyan-100 text-cyan-600'],['People Operations Lead','Luma Works','85% match','bg-amber-100 text-amber-600']].map(([role, company, match, color]) => <div key={role} className="flex items-center gap-3 rounded-2xl border border-slate-100 p-4"><span className={`grid h-11 w-11 place-items-center rounded-xl ${color}`}><Briefcase size={19} /></span><div className="min-w-0 flex-1"><p className="font-semibold text-slate-800">{role}</p><p className="text-sm text-slate-500">{company}</p></div><span className="text-xs font-bold text-emerald-600">{match}</span></div>)}
              </div>
            </div>
            <div className="absolute -bottom-7 -left-7 hidden rounded-2xl bg-slate-900 p-4 text-white shadow-xl sm:block"><p className="text-2xl font-bold">10k+</p><p className="text-xs text-slate-300">careers started here</p></div>
          </div>
        </div>
      </div>
    </section>
    <section className="container px-4 py-12 sm:py-20">
      <div className="mb-10 flex flex-wrap items-end justify-between gap-4"><div><p className="eyebrow">Built for momentum</p><h2 className="mt-2 text-3xl font-bold tracking-tight">Everything hiring needs, in one place.</h2></div><Link to="/jobs" className="text-sm font-bold text-indigo-600 hover:text-indigo-700">Browse all roles →</Link></div>
      <div className="grid gap-5 md:grid-cols-3">
        {[[Search,'Discover better matches','Search thoughtfully curated roles and see the details you need up front.'],[UsersRound,'Move together','Keep candidates and hiring teams aligned at every point in the process.'],[Building2,'Meet great teams','Explore company profiles before you decide where to spend your time.']].map(([Icon, title, text]) => { const ItemIcon = Icon as typeof Search; return <article key={title as string} className="card group transition hover:-translate-y-1 hover:shadow-lg"><span className="mb-6 grid h-12 w-12 place-items-center rounded-2xl bg-indigo-50 text-indigo-600"><ItemIcon size={23} /></span><h3 className="text-lg font-bold">{title as string}</h3><p className="mt-2 leading-7 text-slate-600">{text as string}</p></article> })}
      </div>
    </section>
  </div>
}

export default HomePage
