import type { User } from '../types';

export const MOCK_USERS: User[] = [
  {
    id: 'user-student-1',
    username: '20210101',
    fullName: 'İsmailcan Uslu',
    role: 'STUDENT',
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
];

export function authenticateMock(username: string): { success: boolean; user?: User; token?: string; error?: string } {
  const user = MOCK_USERS.find(u => u.username.toLowerCase() === username.toLowerCase());
  if (user) {
    const mockToken = `eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.mock-obs-token-${user.id}`;
    return { success: true, user, token: mockToken };
  }
  return { success: false, error: 'Hatalı kullanıcı adı veya şifre!' };
}
