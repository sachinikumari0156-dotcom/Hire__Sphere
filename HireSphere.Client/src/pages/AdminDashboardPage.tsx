import React from 'react'
import { Building2, ShieldCheck, UsersRound } from 'lucide-react'

const AdminDashboardPage: React.FC = () => <div className="section container"><p className="eyebrow">Administration</p><h1 className="page-title mt-2">Platform overview</h1><p className="mt-3 text-slate-600">A focused view of the HireSphere community and activity.</p><div className="mt-8 grid gap-5 md:grid-cols-3">{[[UsersRound,'Users','Manage candidate and recruiter accounts.'],[Building2,'Companies','Review company profiles and their postings.'],[ShieldCheck,'Platform health','Keep the experience secure and trustworthy.']].map(([Icon,title,text]) => {const ItemIcon=Icon as typeof UsersRound; return <div key={title as string} className="card"><ItemIcon className="text-indigo-600"/><h2 className="mt-4 font-bold">{title as string}</h2><p className="mt-2 text-sm leading-6 text-slate-500">{text as string}</p></div>})}</div></div>
export default AdminDashboardPage
