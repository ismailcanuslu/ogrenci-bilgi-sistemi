export type UserRole = 'STUDENT' | 'ACADEMIC' | 'STAFF' | 'ADMIN';

export interface User {
  id: string;
  username: string;
  fullName: string;
  role: UserRole;
  roles?: UserRole[];
  activeRole?: UserRole;
  title?: string;
  studentNumber?: string;
  faculty: string;
  department: string;
  currentProgram: string;
  availablePrograms?: string[];
  advisorName?: string;
  gpa?: number;
  completedCredits?: number;
  totalCredits?: number;
  currentTerm: string;
  avatarUrl?: string;
}

export interface WidgetConfig {
  id: string;
  title: string;
  description: string;
  iconName: string;
  isVisible: boolean;
  order: number;
  colSpan: { xs: number; sm: number; md: number; lg: number; xl: number };
}

export interface CourseScheduleItem {
  id: string;
  courseCode: string;
  courseName: string;
  instructor: string;
  day: string; // 'Pazartesi' | 'Salı' ...
  startTime: string;
  endTime: string;
  classroom: string;
  type: 'Teorik' | 'Uygulama' | 'Laboratuvar';
}

export interface ExamItem {
  id: string;
  courseCode: string;
  courseName: string;
  examType: 'Vize' | 'Final' | 'Bütünleme' | 'Ödev/Proje';
  date: string;
  time: string;
  classroom: string;
  daysRemaining: number;
}

export interface AnnouncementItem {
  id: string;
  title: string;
  content: string;
  date: string;
  author: string;
  category: 'Akademik' | 'Etkinlik' | 'Duyuru' | 'Toplumsal Katkı';
  isImportant?: boolean;
}

export interface CurriculumCourse {
  code: string;
  name: string;
  type: 'Zorunlu' | 'Seçmeli';
  theoreticalHours: number;
  practicalHours: number;
  labHours: number;
  localCredits: number;
  ectsCredits: number;
  syllabusDescription?: string;
}

export interface CurriculumSemester {
  semesterNumber: number;
  semesterName: string;
  courses: CurriculumCourse[];
}
