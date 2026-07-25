import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import { useAuthStore } from '@store/authStore'
import { useEffect } from 'react'

// Pages
import HomePage from '@pages/HomePage'
import LoginPage from '@pages/LoginPage'
import RegisterPage from '@pages/RegisterPage'
import DashboardPage from '@pages/DashboardPage'
import JobListingPage from '@pages/JobListingPage'
import JobDetailPage from '@pages/JobDetailPage'
import CompanyListPage from '@pages/CompanyListPage'
import CompanyDetailPage from '@pages/CompanyDetailPage'
import CandidateProfilePage from '@pages/CandidateProfilePage'
import RecruiterDashboardPage from '@pages/RecruiterDashboardPage'
import AdminDashboardPage from '@pages/AdminDashboardPage'
import ApplicationsPage from '@pages/ApplicationsPage'

import ProtectedRoute from '@components/ProtectedRoute'
import Navbar from '@components/Navbar'

function App() {
  const loadFromLocalStorage = useAuthStore((state) => state.loadFromLocalStorage)

  useEffect(() => {
    loadFromLocalStorage()
  }, [loadFromLocalStorage])

  return (
    <Router>
      <Navbar />
      <main className="min-h-screen bg-gray-50">
        <Routes>
          {/* Public Routes */}
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/jobs" element={<JobListingPage />} />
          <Route path="/companies" element={<CompanyListPage />} />
          <Route path="/job/:id" element={<JobDetailPage />} />
          <Route path="/company/:id" element={<CompanyDetailPage />} />

          {/* Protected Routes */}
          <Route
            path="/dashboard"
            element={<ProtectedRoute><DashboardPage /></ProtectedRoute>}
          />
          <Route
            path="/profile"
            element={<ProtectedRoute><CandidateProfilePage /></ProtectedRoute>}
          />
          <Route
            path="/my-applications"
            element={<ProtectedRoute><ApplicationsPage /></ProtectedRoute>}
          />
          <Route
            path="/recruiter"
            element={<ProtectedRoute><RecruiterDashboardPage /></ProtectedRoute>}
          />
          <Route
            path="/admin"
            element={<ProtectedRoute><AdminDashboardPage /></ProtectedRoute>}
          />

          {/* Fallback Route */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </Router>
  )
}

export default App
