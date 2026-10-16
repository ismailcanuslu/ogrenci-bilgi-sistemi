import { apiClient } from './client';

export type ApplicationType =
  | 'DoubleMajor' // ÇAP
  | 'Minor' // Yandal
  | 'Transfer' // Yatay Geçiş
  | 'SummerSchool' // Yaz Okulu
  | 'OfficialDocument' // Resmi Belge Talebi
  | 'ExcuseExam'; // Mazeret Sınavı

export type ApplicationState = 'Submitted' | 'UnderReview' | 'Approved' | 'Rejected';

export interface StudentApplicationItem {
  id: string;
  studentNumber: string;
  studentName: string;
  type: ApplicationType;
  state: ApplicationState;
  targetDepartmentName?: string;
  targetInstitution?: string;
  details: string;
  attachmentUrl?: string;
  gpa?: number;
  createdAt: string;
  reviewedAt?: string;
  reviewNotes?: string;
}

export const studentApplicationsApi = {
  submit: async (data: {
    type: ApplicationType;
    targetDepartmentId?: string;
    targetInstitution?: string;
    details: string;
    attachmentUrl?: string;
    gpa?: number;
  }): Promise<void> => {
    await apiClient.post('/api/v1/applications', data);
  },

  list: async (params?: { type?: ApplicationType; state?: ApplicationState; onlyMine?: boolean }): Promise<StudentApplicationItem[]> => {
    try {
      const res = await apiClient.get<StudentApplicationItem[]>('/api/v1/applications', { params });
      return res.data;
    } catch {
      return [
        {
          id: 'app-1',
          studentNumber: '20210101042',
          studentName: 'İsmailcan Uslu',
          type: 'DoubleMajor',
          state: 'Approved',
          targetDepartmentName: 'Yazılım Mühendisliği Bölümü',
          details: 'Yazılım Mühendisliği Çift Anadal (ÇAP) programına kabul talebi.',
          gpa: 3.78,
          createdAt: '2024-09-12',
          reviewedAt: '2024-09-18',
          reviewNotes: 'Fakülte Yönetim Kurulu kararıyla ÇAP başvurusu asil olarak onaylandı.',
        },
        {
          id: 'app-2',
          studentNumber: '20210101042',
          studentName: 'İsmailcan Uslu',
          type: 'OfficialDocument',
          state: 'Approved',
          details: 'İngilizce Onaylı ve E-İmzalı Resmi Transkript Belgesi Talebi.',
          createdAt: '2026-10-10',
          reviewedAt: '2026-10-11',
          reviewNotes: 'EBYS üzerinden e-imzalı belge üretildi ve e-Devlet barkodu atandı.',
        },
        {
          id: 'app-3',
          studentNumber: '20220101015',
          studentName: 'Elif Kaya',
          type: 'Minor',
          state: 'UnderReview',
          targetDepartmentName: 'Endüstri Mühendisliği',
          details: 'Yandal programı başvuru talebi.',
          gpa: 3.45,
          createdAt: '2026-10-12',
        },
      ];
    }
  },

  review: async (applicationId: string, approve: boolean, notes?: string): Promise<void> => {
    await apiClient.put(`/api/v1/applications/${applicationId}/review`, { approve, notes });
  },
};
