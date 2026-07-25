import React from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuthStore } from '@store/authStore'
import { Menu, X, LogOut, Sparkles, LayoutDashboard } from 'lucide-react'
import { useState } from 'react'

const Navbar: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuthStore()
  const navigate = useNavigate()
  const [isOpen, setIsOpen] = useState(false)

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const getDashboardLink = () => {
    if (!user) return '/dashboard'
    switch (user.role) {
      case 'Admin':
        return '/admin'
      case 'Recruiter':
        return '/recruiter'
      case 'Candidate':
        return '/dashboard'
      default:
        return '/dashboard'
    }
  }

  return (
    <nav className="sticky top-0 z-50 border-b border-slate-100 bg-white/90 backdrop-blur">
      <div className="container mx-auto px-4 py-3">
        <div className="flex justify-between items-center">
          {/* Logo */}
          <Link to="/" className="flex items-center gap-2 text-xl font-bold tracking-tight text-slate-900">
            <span className="grid h-9 w-9 place-items-center rounded-xl bg-indigo-600 text-white"><Sparkles size={18} /></span>
            HireSphere
          </Link>

          {/* Desktop Menu */}
          <div className="hidden md:flex items-center space-x-6">
            <Link to="/jobs" className="text-sm font-medium text-slate-600 hover:text-indigo-600">
              Jobs
            </Link>
            <Link to="/companies" className="text-sm font-medium text-slate-600 hover:text-indigo-600">
              Companies
            </Link>

            {isAuthenticated ? (
              <>
                <Link to={getDashboardLink()} className="flex items-center gap-1 text-sm font-medium text-slate-600 hover:text-indigo-600">
                  <LayoutDashboard size={16} /> Dashboard
                </Link>
                <span className="rounded-lg bg-slate-100 px-3 py-2 text-xs font-semibold text-slate-600">
                  {user?.username}
                </span>
                <button
                  onClick={handleLogout}
                  className="flex items-center space-x-2 rounded-xl px-3 py-2 text-sm font-semibold text-rose-600 hover:bg-rose-50"
                >
                  <LogOut size={18} />
                  <span>Logout</span>
                </button>
              </>
            ) : (
              <>
                <Link
                  to="/login"
                  className="text-sm font-semibold text-slate-700 hover:text-indigo-600"
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="btn btn-primary text-sm !px-4 !py-2"
                >
                  Register
                </Link>
              </>
            )}
          </div>

          {/* Mobile Menu Button */}
          <button aria-label="Toggle navigation"
            className="rounded-lg p-2 text-slate-700 md:hidden"
            onClick={() => setIsOpen(!isOpen)}
          >
            {isOpen ? <X size={24} /> : <Menu size={24} />}
          </button>
        </div>

        {/* Mobile Menu */}
        {isOpen && (
          <div className="md:hidden mt-4 flex flex-col space-y-2">
            <Link to="/jobs" className="text-gray-600 hover:text-blue-600 py-2">
              Jobs
            </Link>
            <Link to="/companies" className="text-gray-600 hover:text-blue-600 py-2">
              Companies
            </Link>

            {isAuthenticated ? (
              <>
                <Link to={getDashboardLink()} className="text-gray-600 hover:text-blue-600 py-2">
                  Dashboard
                </Link>
                <button
                  onClick={handleLogout}
                  className="flex items-center space-x-2 bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
                >
                  <LogOut size={18} />
                  <span>Logout</span>
                </button>
              </>
            ) : (
              <>
                <Link
                  to="/login"
                  className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
                >
                  Register
                </Link>
              </>
            )}
          </div>
        )}
      </div>
    </nav>
  )
}

export default Navbar
