import axios, { AxiosInstance } from 'axios';

// The API development profile listens on port 5009. Allow deployments to
// override this without changing source code.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5009/api';

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Add token to requests
    this.client.interceptors.request.use((config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    // Handle response errors
    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('token');
          localStorage.removeItem('user');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Auth endpoints
  login(username: string, password: string) {
    return this.client.post('/auth/login', { username, password });
  }

  register(username: string, email: string, password: string, role: string) {
    return this.client.post('/auth/register', { username, email, password, role });
  }

  verifyToken(token: string) {
    return this.client.post('/auth/verify-token', { token });
  }

  // Company endpoints
  getCompanies() {
    return this.client.get('/company');
  }

  getCompanyById(id: string) {
    return this.client.get(`/company/${id}`);
  }

  createCompany(data: any) {
    return this.client.post('/company', data);
  }

  updateCompany(id: string, data: any) {
    return this.client.put(`/company/${id}`, data);
  }

  deleteCompany(id: string) {
    return this.client.delete(`/company/${id}`);
  }

  // Job endpoints
  getJobs(companyId?: string) {
    const params = companyId ? { companyId } : {};
    return this.client.get('/job', { params });
  }

  getJobById(id: string) {
    return this.client.get(`/job/${id}`);
  }

  createJob(data: any) {
    return this.client.post('/job', data);
  }

  updateJob(id: string, data: any) {
    return this.client.put(`/job/${id}`, data);
  }

  closeJob(id: string) {
    return this.client.post(`/job/${id}/close`, {});
  }
}

export const apiClient = new ApiClient();
