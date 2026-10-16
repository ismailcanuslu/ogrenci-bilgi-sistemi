import React, { createContext, useContext, useState, useEffect } from 'react';
import { authApi } from '../api/auth';
import type { LoginRequest } from '../api/auth';
import type { User, UserRole } from '../types';

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  activeRole: UserRole;
  activeProgram: string;
  setActiveProgram: (prog: string) => void;
  switchRole: (role: UserRole) => void;
  switchUser: (email: string) => void;
  login: (credentials: LoginRequest) => Promise<UserRole>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const mapBackendRoleToFrontend = (roleName: string): UserRole => {
  switch (roleName) {
    case 'Student':
      return 'STUDENT';
    case 'Instructor':
      return 'ACADEMIC';
    case 'StudentAffairs':
      return 'STAFF';
    case 'SystemAdministrator':
      return 'ADMIN';
    default:
      return 'STUDENT';
  }
};

// Standart varsayılan kullanıcı profilleri
const PROFILE_MAPPINGS: Record<string, Partial<User>> = {
  'ogrenci@obs.edu.tr': {
    fullName: 'İsmailcan Uslu',
    role: 'STUDENT',
    roles: ['STUDENT'],
    activeRole: 'STUDENT',
    studentNumber: '20210101042',
    faculty: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    department: 'Bilgisayar Mühendisliği',
    currentProgram: 'Bilgisayar Mühendisliği (Lisans - Ana Dal)',
    availablePrograms: [
      'Bilgisayar Mühendisliği (Lisans - Ana Dal)',
      'Yazılım Mühendisliği (Çift Anadal - ÇAP)'
    ],
    advisorName: 'Prof. Dr. Ahmet Bilgehan',
    gpa: 3.78,
    completedCredits: 195,
    totalCredits: 240,
    currentTerm: '2026-2027 Güz Dönemi',
    avatarUrl: 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=256&q=80',
  },
  'akademik@obs.edu.tr': {
    fullName: 'Prof. Dr. Ahmet Bilgehan',
    role: 'ACADEMIC',
    roles: ['ACADEMIC'],
    activeRole: 'ACADEMIC',
    title: 'Bölüm Başkanı / Öğretim Üyesi',
    faculty: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    department: 'Bilgisayar Mühendisliği',
    currentProgram: 'Akademik Personel Portalı',
    availablePrograms: ['Akademik Personel Portalı'],
    currentTerm: '2026-2027 Güz Dönemi',
    avatarUrl: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=256&q=80',
  },
  'arastirma.gorevlisi@obs.edu.tr': {
    fullName: 'Arş. Gör. Caner Kara (Doktora Öğr.)',
    role: 'ACADEMIC',
    roles: ['ACADEMIC', 'STUDENT'],
    activeRole: 'ACADEMIC',
    title: 'Araştırma Görevlisi & Doktora Öğrencisi',
    studentNumber: '20249010012',
    faculty: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    department: 'Bilgisayar Mühendisliği',
    currentProgram: 'Bilgisayar Mühendisliği (Doktora)',
    availablePrograms: [
      'Bilgisayar Mühendisliği (Doktora)',
      'Akademik Personel Portalı'
    ],
    advisorName: 'Prof. Dr. Ahmet Bilgehan',
    gpa: 3.92,
    completedCredits: 42,
    totalCredits: 240,
    currentTerm: '2026-2027 Güz Dönemi',
    avatarUrl: 'https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?auto=format&fit=crop&w=256&q=80',
  },
  'idari@obs.edu.tr': {
    fullName: 'Ayşe Yılmaz',
    role: 'STAFF',
    roles: ['STAFF'],
    activeRole: 'STAFF',
    title: 'Öğrenci İşleri Uzmanı',
    faculty: 'Rektörlük',
    department: 'Öğrenci İşleri Daire Başkanlığı',
    currentProgram: 'İdari Yönetim Portalı',
    availablePrograms: ['İdari Yönetim Portalı'],
    currentTerm: '2026-2027 Güz Dönemi',
    avatarUrl: 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=256&q=80',
  },
  'admin@obs.edu.tr': {
    fullName: 'Sistem Yöneticisi',
    role: 'ADMIN',
    roles: ['ADMIN', 'STAFF', 'ACADEMIC', 'STUDENT'],
    activeRole: 'ADMIN',
    title: 'Bilgi İşlem Daire Bşk. Sistem Yöneticisi',
    faculty: 'Bilgi İşlem Daire Başkanlığı',
    department: 'Sistem ve Ağ Yönetimi',
    currentProgram: 'Sistem Yönetim Konsolu',
    availablePrograms: ['Sistem Yönetim Konsolu'],
    currentTerm: '2026-2027 Güz Dönemi',
    avatarUrl: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=256&q=80',
  },
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [activeRole, setActiveRole] = useState<UserRole>('STUDENT');
  const [activeProgram, setActiveProgram] = useState<string>('Bilgisayar Mühendisliği (Lisans - Ana Dal)');

  useEffect(() => {
    const savedToken = localStorage.getItem('obs_access_token');
    const savedProfile = localStorage.getItem('obs_user_profile');

    if (savedToken && savedProfile) {
      try {
        const parsed = JSON.parse(savedProfile) as User;
        setUser(parsed);
        setToken(savedToken);
        setActiveRole(parsed.activeRole || parsed.role || 'STUDENT');
        if (parsed.currentProgram) {
          setActiveProgram(parsed.currentProgram);
        }
      } catch {
        localStorage.removeItem('obs_access_token');
        localStorage.removeItem('obs_user_profile');
      }
    }
    setIsLoading(false);
  }, []);

  const switchRole = (newRole: UserRole) => {
    setActiveRole(newRole);
    if (user) {
      const updatedUser: User = {
        ...user,
        activeRole: newRole,
        role: newRole,
      };
      setUser(updatedUser);
      localStorage.setItem('obs_user_profile', JSON.stringify(updatedUser));
    }
  };

  const login = async (credentials: LoginRequest): Promise<UserRole> => {
    setIsLoading(true);
    try {
      const data = await authApi.login(credentials);
      localStorage.setItem('obs_access_token', data.accessToken);
      localStorage.setItem('obs_refresh_token', data.refreshToken);
      setToken(data.accessToken);

      const normalizedEmail = credentials.email.trim().toLowerCase();
      const meta = PROFILE_MAPPINGS[normalizedEmail] || {
        fullName: credentials.email.split('@')[0],
        role: 'STUDENT' as UserRole,
        roles: ['STUDENT' as UserRole],
        faculty: 'Mühendislik Fakültesi',
        department: 'Genel',
        currentProgram: 'Lisans Programı',
        availablePrograms: ['Lisans Programı'],
        currentTerm: '2026-2027 Güz Dönemi',
      };

      // Backend'den dönen roller varsa onları eşle
      let userRoles: UserRole[] = meta.roles || [meta.role || 'STUDENT'];
      if (data.roles && data.roles.length > 0) {
        userRoles = data.roles.map(mapBackendRoleToFrontend);
      }

      const initialRole: UserRole = userRoles[0] || 'STUDENT';

      const userProfile: User = {
        id: normalizedEmail,
        username: credentials.email,
        fullName: meta.fullName || credentials.email,
        role: initialRole,
        roles: userRoles,
        activeRole: initialRole,
        title: meta.title,
        studentNumber: meta.studentNumber,
        faculty: meta.faculty || '',
        department: meta.department || '',
        currentProgram: meta.currentProgram || 'Lisans Programı',
        availablePrograms: meta.availablePrograms,
        advisorName: meta.advisorName,
        gpa: meta.gpa,
        completedCredits: meta.completedCredits,
        totalCredits: meta.totalCredits,
        currentTerm: meta.currentTerm || '2026-2027 Güz Dönemi',
        avatarUrl: meta.avatarUrl,
      };

      setUser(userProfile);
      setActiveRole(initialRole);
      setActiveProgram(userProfile.currentProgram);
      localStorage.setItem('obs_user_profile', JSON.stringify(userProfile));
      return initialRole;
    } finally {
      setIsLoading(false);
    }
  };

  const switchUser = (email: string) => {
    const normalizedEmail = email.trim().toLowerCase();
    const meta = PROFILE_MAPPINGS[normalizedEmail] || PROFILE_MAPPINGS['ogrenci@obs.edu.tr'];
    const mockToken = `mock-token-${normalizedEmail}`;
    const initialRole: UserRole = meta.role || 'STUDENT';

    const userProfile: User = {
      id: normalizedEmail,
      username: normalizedEmail,
      fullName: meta.fullName || normalizedEmail,
      role: initialRole,
      roles: meta.roles || [initialRole],
      activeRole: initialRole,
      title: meta.title,
      studentNumber: meta.studentNumber,
      faculty: meta.faculty || '',
      department: meta.department || '',
      currentProgram: meta.currentProgram || 'Lisans Programı',
      availablePrograms: meta.availablePrograms,
      advisorName: meta.advisorName,
      gpa: meta.gpa,
      completedCredits: meta.completedCredits,
      totalCredits: meta.totalCredits,
      currentTerm: meta.currentTerm || '2026-2027 Güz Dönemi',
      avatarUrl: meta.avatarUrl,
    };

    localStorage.setItem('obs_access_token', mockToken);
    localStorage.setItem('obs_user_profile', JSON.stringify(userProfile));
    setUser(userProfile);
    setToken(mockToken);
    setActiveRole(initialRole);
    setActiveProgram(userProfile.currentProgram);
  };

  const logout = () => {
    authApi.logout();
    setUser(null);
    setToken(null);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token && !!user,
        isLoading,
        activeRole,
        activeProgram,
        setActiveProgram,
        switchRole,
        switchUser,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
