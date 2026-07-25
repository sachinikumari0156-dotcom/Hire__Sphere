import { create } from 'zustand';

export interface User {
  userId: string;
  username: string;
  email: string;
  role: 'Admin' | 'Recruiter' | 'Candidate';
  token: string;
}

interface AuthStore {
  user: User | null;
  isAuthenticated: boolean;
  setUser: (user: User | null) => void;
  logout: () => void;
  loadFromLocalStorage: () => void;
}

export const useAuthStore = create<AuthStore>((set) => ({
  user: null,
  isAuthenticated: false,
  setUser: (user) => {
    if (user) {
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('token', user.token);
      set({ user, isAuthenticated: true });
    } else {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      set({ user: null, isAuthenticated: false });
    }
  },
  logout: () => {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    set({ user: null, isAuthenticated: false });
  },
  loadFromLocalStorage: () => {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        set({ user, isAuthenticated: true });
      } catch (e) {
        console.error('Failed to parse user from localStorage', e);
      }
    }
  },
}));
