export interface Company {
  id: string;
  name: string;
  description?: string;
  website?: string;
  location?: string;
  createdAt: string;
  jobCount: number;
  recruiterCount: number;
}

export interface Job {
  id: string;
  title: string;
  description: string;
  location?: string;
  salaryRange?: string;
  isClosed: boolean;
  companyId: string;
  createdAt: string;
  applicationCount?: number;
}

export interface JobListResponse {
  id: string;
  title: string;
  location?: string;
  salaryRange?: string;
  companyId: string;
  companyName?: string;
  isClosed: boolean;
  createdAt: string;
  applicationCount: number;
}

export interface JobDetail extends JobListResponse {
  description: string;
  requiredSkills?: { id: string; name: string }[];
}

export interface CompanyListResponse {
  id: string;
  name: string;
  description?: string;
  website?: string;
  location?: string;
  createdAt: string;
}

export interface Application {
  id: string;
  jobId: string;
  candidateProfileId: string;
  coverLetter?: string;
  status: 'Submitted' | 'Shortlisted' | 'Interviewing' | 'Accepted' | 'Rejected';
  matchScore: number;
  matchExplanation?: string;
  createdAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}
