import { apiClient } from './client';

export interface StudentDashboardOverview {
  studentNumber: string;
  fullName: string;
  facultyName: string;
  departmentName: string;
  primaryProgramName: string;
  activeEnrollmentType: string;
  advisorName: string;
  advisorTitle: string;
  advisorEmail: string;
  gpa: number;
  completedEcts: number;
  totalEcts: number;
  currentClassLevel: number;
  currentSemester: number;
  activeTermName: string;
  isTuitionPaid: boolean;
  profilePictureUrl?: string;
  availablePrograms: { id: string; programName: string; isPrimary: boolean; enrollmentType: string }[];
}

export interface StudentTranscript {
  studentNumber: string;
  fullName: string;
  gpa: number;
  totalEcts: number;
  semesters: {
    semesterNumber: number;
    termName: string;
    semesterGpa: number;
    semesterEcts: number;
    courses: {
      code: string;
      name: string;
      credit: number;
      ects: number;
      letterGrade: string;
      gradePoint: number;
      status: 'Başarılı' | 'Koşullu' | 'Başarısız';
    }[];
  }[];
}

export interface WeeklyScheduleItem {
  id: string;
  courseCode: string;
  courseName: string;
  instructorName: string;
  dayOfWeek: string;
  startTime: string;
  endTime: string;
  classroom: string;
  facultyName: string;
  isOnline: boolean;
}

export interface TuitionStatus {
  termName: string;
  amountDue: number;
  amountPaid: number;
  isPaid: boolean;
  dueDate: string;
  bankAccount: string;
}

export interface MilitaryStatus {
  status: string;
  postponementDate?: string;
  asalDocumentNumber?: string;
  isCompliant: boolean;
}

export interface DisciplinaryRecord {
  id: string;
  decisionDate: string;
  incidentDescription: string;
  penaltyType: string;
  isExpunged: boolean;
}

export const studentPortalApi = {
  getOverview: async (): Promise<StudentDashboardOverview> => {
    try {
      const res = await apiClient.get<StudentDashboardOverview>('/api/v1/student-portal/dashboard');
      return res.data;
    } catch {
      // Offline fallback: Seeder ile uyumlu zengin mock
      return {
        studentNumber: '20210101042',
        fullName: 'İsmailcan Uslu',
        facultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi',
        departmentName: 'Bilgisayar Mühendisliği',
        primaryProgramName: 'Bilgisayar Mühendisliği Lisans Programı (Ana Dal)',
        activeEnrollmentType: 'Major',
        advisorName: 'Prof. Dr. Ahmet Bilgehan',
        advisorTitle: 'Bölüm Başkanı',
        advisorEmail: 'ahmet.bilgehan@obs.edu.tr',
        gpa: 3.78,
        completedEcts: 195,
        totalEcts: 240,
        currentClassLevel: 4,
        currentSemester: 7,
        activeTermName: '2026-2027 Güz Dönemi',
        isTuitionPaid: true,
        availablePrograms: [
          { id: 'prog-1', programName: 'Bilgisayar Mühendisliği (Lisans - Ana Dal)', isPrimary: true, enrollmentType: 'Major' },
          { id: 'prog-2', programName: 'Yazılım Mühendisliği (Çift Anadal - ÇAP)', isPrimary: false, enrollmentType: 'DoubleMajor' },
        ],
      };
    }
  },

  getTranscript: async (): Promise<StudentTranscript> => {
    try {
      const res = await apiClient.get<StudentTranscript>('/api/v1/student-portal/transcript');
      return res.data;
    } catch {
      return {
        studentNumber: '20210101042',
        fullName: 'İsmailcan Uslu',
        gpa: 3.78,
        totalEcts: 195,
        semesters: [
          {
            semesterNumber: 1,
            termName: '2023-2024 Güz',
            semesterGpa: 3.85,
            semesterEcts: 30,
            courses: [
              { code: 'BLG101', name: 'Programlamaya Giriş (C/C++)', credit: 4, ects: 6, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
              { code: 'MAT101', name: 'Matematik I (Kalkülüs)', credit: 4, ects: 6, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
              { code: 'FZK101', name: 'Genel Fizik I', credit: 4, ects: 6, letterGrade: 'BA', gradePoint: 3.5, status: 'Başarılı' },
              { code: 'ENG101', name: 'Academic English I', credit: 3, ects: 4, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
              { code: 'TRK101', name: 'Türk Dili I', credit: 2, ects: 2, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
              { code: 'ATA101', name: 'Atatürk İlkeleri ve İnkılap Tarihi I', credit: 2, ects: 2, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
              { code: 'MDBF100', name: 'Mühendisliğe Giriş ve Etik', credit: 1, ects: 4, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
            ],
          },
          {
            semesterNumber: 2,
            termName: '2023-2024 Bahar',
            semesterGpa: 3.72,
            semesterEcts: 30,
            courses: [
              { code: 'BLG102', name: 'Nesne Yönelimli Programlama (Java)', credit: 4, ects: 6, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
              { code: 'MAT102', name: 'Matematik II (Lineer Cebir)', credit: 4, ects: 6, letterGrade: 'BA', gradePoint: 3.5, status: 'Başarılı' },
              { code: 'FZK102', name: 'Genel Fizik II (Elektromanyetizma)', credit: 4, ects: 6, letterGrade: 'BB', gradePoint: 3.0, status: 'Başarılı' },
              { code: 'BLG104', name: 'Ayrık Matematik', credit: 3, ects: 5, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
              { code: 'ENG102', name: 'Academic English II', credit: 3, ects: 4, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
              { code: 'TRK102', name: 'Türk Dili II', credit: 2, ects: 2, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
              { code: 'ATA102', name: 'Atatürk İlkeleri ve İnkılap Tarihi II', credit: 2, ects: 2, letterGrade: 'AA', gradePoint: 4.0, status: 'Başarılı' },
            ],
          },
        ],
      };
    }
  },

  getWeeklySchedule: async (): Promise<WeeklyScheduleItem[]> => {
    try {
      const res = await apiClient.get<WeeklyScheduleItem[]>('/api/v1/student-portal/weekly-schedule');
      return res.data;
    } catch {
      return [
        { id: '1', courseCode: 'BLG411', courseName: 'Bitirme Tasarım Projesi', instructorName: 'Prof. Dr. Ahmet Bilgehan', dayOfWeek: 'Pazartesi', startTime: '09:30', endTime: '12:20', classroom: 'Amfi 3', facultyName: 'Mühendislik Fak.', isOnline: false },
        { id: '2', courseCode: 'BLG452', courseName: 'Bulut Bilişim ve Mikroservisler', instructorName: 'Doç. Dr. Selin Doğan', dayOfWeek: 'Salı', startTime: '13:30', endTime: '16:20', classroom: 'Yazılım Lab 4', facultyName: 'Mühendislik Fak.', isOnline: false },
        { id: '3', courseCode: 'BLG301', courseName: 'Veritabanı Yönetim Sistemleri', instructorName: 'Dr. Öğr. Üyesi Murat Kaya', dayOfWeek: 'Çarşamba', startTime: '10:30', endTime: '13:20', classroom: 'Amfi 1', facultyName: 'Mühendislik Fak.', isOnline: false },
        { id: '4', courseCode: 'YZM302', courseName: 'Yazılım Kalite ve Test (ÇAP)', instructorName: 'Dr. Emre Kurt', dayOfWeek: 'Perşembe', startTime: '14:00', endTime: '16:50', classroom: 'D-204', facultyName: 'Mühendislik Fak.', isOnline: false },
      ];
    }
  },

  getTuition: async (): Promise<TuitionStatus> => {
    try {
      const res = await apiClient.get<TuitionStatus>('/api/v1/student-portal/tuition');
      return res.data;
    } catch {
      return {
        termName: '2026-2027 Güz Dönemi',
        amountDue: 0,
        amountPaid: 0,
        isPaid: true,
        dueDate: '25.09.2026',
        bankAccount: 'TR56 0001 0000 0000 0000 0000 01 (Halkbank)',
      };
    }
  },

  getMilitary: async (): Promise<MilitaryStatus> => {
    try {
      const res = await apiClient.get<MilitaryStatus>('/api/v1/student-portal/military');
      return res.data;
    } catch {
      return {
        status: 'Öğrenim Nedeniyle Tecilli (ASAL Onaylı)',
        postponementDate: '31.12.2028',
        asalDocumentNumber: 'ASAL-2021-987452',
        isCompliant: true,
      };
    }
  },

  getDisciplinary: async (): Promise<DisciplinaryRecord[]> => {
    try {
      const res = await apiClient.get<DisciplinaryRecord[]>('/api/v1/student-portal/disciplinary');
      return res.data;
    } catch {
      return [];
    }
  },
};
