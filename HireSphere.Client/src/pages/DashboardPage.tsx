import React, { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Briefcase, FileText, Sparkles } from 'lucide-react'
import { useAuthStore } from '@store/authStore'
import { apiClient } from '@services/api'
import { JobListResponse } from '@/types'

const DashboardPage: React.FC = () => {
  const { user } = useAuthStore()
  const [jobs, setJobs] = useState<JobListResponse[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    const fetchJobs = async () => {
      try {
        const response = await apiClient.getJobs()
        setJobs(response.data.data || [])
      } catch (err) {
        setError('Failed to load jobs')
        console.error(err)
      } finally {
        setLoading(false)
      }
    }

    fetchJobs()
  }, [])

  return (
    <div className="section container">
      <p className="eyebrow">Candidate workspace</p><h1 className="page-title mt-2">Good to see you, {user?.username}.</h1>
      <p className="mt-3 text-slate-600">Pick up where you left off or discover something new.</p>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-12">
        <div className="card"><Briefcase className="text-indigo-600" size={22}/>
          <h3 className="mt-4 text-sm font-semibold text-slate-500">Open opportunities</h3>
          <p className="mt-1 text-3xl font-bold text-slate-900">{jobs.filter(j => !j.isClosed).length}</p>
        </div>
        <div className="card"><Sparkles className="text-amber-500" size={22}/>
          <h3 className="mt-4 text-sm font-semibold text-slate-500">New matches this week</h3>
          <p className="mt-1 text-3xl font-bold text-slate-900">12</p>
        </div>
        <Link to="/my-applications" className="card transition hover:-translate-y-1 hover:shadow-md"><FileText className="text-violet-600" size={22}/>
          <h3 className="mt-4 text-sm font-semibold text-slate-500">My applications</h3>
          <p className="mt-1 text-3xl font-bold text-slate-900">View progress</p>
        </Link>
      </div>

      <div>
        <div className="mb-6 flex items-center justify-between"><h2 className="text-2xl font-bold text-slate-900">Fresh opportunities</h2><Link to="/jobs" className="text-sm font-bold text-indigo-600">Explore all →</Link></div>

        {loading ? (
          <p className="text-gray-600">Loading jobs...</p>
        ) : error ? (
          <p className="text-red-600">{error}</p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {jobs.slice(0, 6).map((job) => (
              <div key={job.id} className="card hover:shadow-lg transition-shadow">
                <h3 className="text-lg font-semibold text-gray-900">{job.title}</h3>
                <p className="text-gray-600 text-sm mb-2">{job.companyName}</p>
                <p className="text-gray-600 text-sm mb-4">{job.location}</p>
                {job.salaryRange && (
                  <p className="text-green-600 font-semibold mb-4">{job.salaryRange}</p>
                )}
                <div className="flex justify-between items-center">
                  <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                    job.isClosed
                      ? 'bg-red-100 text-red-800'
                      : 'bg-green-100 text-green-800'
                  }`}>
                    {job.isClosed ? 'Closed' : 'Open'}
                  </span>
                  <a
                    href={`/job/${job.id}`}
                    className="text-blue-600 hover:text-blue-800 font-medium"
                  >
                    View Details →
                  </a>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}

export default DashboardPage
