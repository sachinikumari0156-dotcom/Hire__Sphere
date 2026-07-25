import React from 'react'
import { Link } from 'react-router-dom'
import { FileCheck2, Search } from 'lucide-react'

const ApplicationsPage: React.FC = () => <div className="section container"><p className="eyebrow">Candidate workspace</p><h1 className="page-title mt-2">Your applications</h1><p className="mt-3 text-slate-600">Keep an eye on every opportunity you have put yourself forward for.</p><div className="card mt-8 py-16 text-center"><span className="mx-auto grid h-14 w-14 place-items-center rounded-2xl bg-indigo-50 text-indigo-600"><FileCheck2 size={26}/></span><h2 className="mt-5 text-xl font-bold">Your application journey starts here.</h2><p className="mx-auto mt-2 max-w-md text-sm leading-6 text-slate-500">When you apply for a role, its progress and recruiter updates will appear in this space.</p><Link to="/jobs" className="btn btn-primary mt-6"><Search size={17}/> Explore roles</Link></div></div>
export default ApplicationsPage
