import React, { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { ArrowLeft, Building2, Globe2, MapPin, UsersRound } from 'lucide-react'
import { apiClient } from '@services/api'
import { Company } from '@/types'

const CompanyDetailPage: React.FC = () => {
 const { id } = useParams(); const [company, setCompany] = useState<Company | null>(null); const [loading, setLoading] = useState(true); const [error, setError] = useState('')
 useEffect(() => { if (!id) return; apiClient.getCompanyById(id).then(r => setCompany(r.data.data)).catch(() => setError('This company could not be found.')).finally(() => setLoading(false)) }, [id])
 if (loading) return <div className="section container py-24 text-center text-slate-500">Loading company…</div>
 if (!company || error) return <div className="section container"><Link to="/companies" className="text-sm font-bold text-indigo-600">← Company directory</Link><div className="card mt-6 py-16 text-center text-rose-600">{error}</div></div>
 return <div className="section container"><Link to="/companies" className="inline-flex items-center gap-2 text-sm font-bold text-indigo-600"><ArrowLeft size={16}/> All companies</Link><div className="mt-6 overflow-hidden rounded-3xl bg-slate-950 text-white"><div className="bg-[radial-gradient(circle_at_80%_0,#4f46e5_0,transparent_32%)] px-7 py-12 sm:px-12"><span className="grid h-16 w-16 place-items-center rounded-2xl bg-white/10"><Building2 size={30}/></span><h1 className="mt-7 text-4xl font-bold tracking-tight">{company.name}</h1><div className="mt-4 flex flex-wrap gap-5 text-sm text-slate-300">{company.location && <span className="flex items-center gap-2"><MapPin size={16}/>{company.location}</span>}{company.website && <a className="flex items-center gap-2 hover:text-white" href={company.website} target="_blank" rel="noreferrer"><Globe2 size={16}/>Visit website</a>}</div></div></div><div className="mt-7 grid gap-7 lg:grid-cols-[1fr_300px]"><article className="card sm:p-8"><h2 className="text-xl font-bold">About {company.name}</h2><p className="mt-4 whitespace-pre-line leading-8 text-slate-600">{company.description || 'This company has not shared its story yet.'}</p></article><aside className="card h-fit"><p className="text-sm font-semibold text-slate-500">ON HIRESPHERE</p><div className="mt-5 flex items-center gap-3"><span className="rounded-xl bg-indigo-50 p-3 text-indigo-600"><UsersRound size={20}/></span><div><p className="text-2xl font-bold">{company.jobCount}</p><p className="text-sm text-slate-500">Open roles</p></div></div><Link to={`/jobs?company=${company.id}`} className="btn btn-primary mt-6 w-full">See open roles</Link></aside></div></div>
}
export default CompanyDetailPage
