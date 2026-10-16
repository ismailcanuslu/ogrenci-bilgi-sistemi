import { apiClient } from './client';

export interface AcademicTitleItem {
  id: string;
  title: string;
  order: number;
}

export interface InstructorItem {
  id: string;
  title: string; // Örn: 'Prof. Dr.', 'Doç. Dr.', 'Unvansız' vb.
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone?: string;
  facultyId?: string;
  facultyName?: string;
  departmentId?: string;
  departmentName?: string;
  roomNumber?: string;
  isActive: boolean;
  // Detaylı Bilgi Alanları (Öğrenci işleri doldurmak zorunda değildir)
  educationInfo?: string;   // ÖĞRENİM BİLGİLERİ
  articles?: string;        // MAKALELER
  projects?: string;        // PROJELER
  declarations?: string;    // BİLDİRİLER
  books?: string;           // KİTAPLAR
  coursesTaught?: string;   // DERSLER
  editorships?: string;     // EDİTÖRLÜK
  advisings?: string;       // DANIŞMANLIKLAR
  academicTasks?: string;   // AKADEMİK GÖREVLER
  experiences?: string;     // DENEYİMLER
  refereeings?: string;     // HAKEMLİKLER
  openAccess?: string;      // AÇIK ERİŞİM
}

export interface FacultyItem {
  id: string;
  code: string;
  name: string;
  deanId?: string;
  deanName?: string;
  establishedYear?: number;
  isActive: boolean;
  departmentCount?: number;
}

export interface DepartmentItem {
  id: string;
  facultyId: string;
  facultyName?: string;
  code: string;
  name: string;
  headName?: string;
  language?: string;
  minEctsLimit: number; // Normal dönem AKTS sınırı (Örn: 30)
  maxEctsLimit: number; // Üstten ders alma / maksimum AKTS sınırı (Örn: 42)
  overCapEctsLimit?: number; // ÇAP / Başarılı öğrenci sınırı (Örn: 48)
  isActive: boolean;
}

export interface ProgramItem {
  id: string;
  departmentId: string;
  code: string;
  name: string;
  degreeLevel: 'Associate' | 'Undergraduate' | 'Master' | 'Doctorate';
  isActive: boolean;
}

export interface CourseItem {
  id: string;
  departmentId: string;
  departmentName?: string;
  code: string;
  name: string;
  description?: string;
  defaultEcts: number;
  defaultCredit: number;
  theoreticalHours?: number;
  practicalHours?: number;
  courseType?: 'Zorunlu' | 'Seçmeli' | 'Üniversite Ortak';
  prerequisites?: string;
  isActive: boolean;
}

export interface CourseOfferingItem {
  id: string;
  courseId: string;
  courseCode: string;
  courseName: string;
  academicTermId: string;
  section: string;
  capacity: number;
  enrolledCount: number;
  primaryInstructorId?: string;
  primaryInstructorName?: string;
  isRegistrationOpen: boolean;
}

export interface StudentListItem {
  id: string;
  studentNumber: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  departmentName: string;
  programName: string;
  enrollmentYear: number;
  status: 'Active' | 'Suspended' | 'Dismissed' | 'Graduated';
  gpa?: number;
}

export interface AcademicYearItem {
  id: string;
  code: string;
  name: string;
  startDate: string;
  endDate: string;
  isActive: boolean;
  terms: {
    id: string;
    termType: 'Fall' | 'Spring' | 'Summer';
    name: string;
    startDate: string;
    endDate: string;
    isRegistrationOpen: boolean;
  }[];
}

// Initial Mock Datası
const initialTitles: AcademicTitleItem[] = [
  { id: 't-1', title: 'Prof. Dr.', order: 1 },
  { id: 't-2', title: 'Doç. Dr.', order: 2 },
  { id: 't-3', title: 'Dr. Öğr. Üyesi', order: 3 },
  { id: 't-4', title: 'Öğr. Gör. Dr.', order: 4 },
  { id: 't-5', title: 'Öğr. Gör.', order: 5 },
  { id: 't-6', title: 'Arş. Gör. Dr.', order: 6 },
  { id: 't-7', title: 'Arş. Gör.', order: 7 },
  { id: 't-8', title: 'Uzman', order: 8 },
  { id: 't-9', title: 'Unvansız', order: 9 },
];

const initialInstructors: InstructorItem[] = [
  {
    id: 'inst-1',
    title: 'Prof. Dr.',
    firstName: 'Haldun',
    lastName: 'Özcan',
    fullName: 'Prof. Dr. Haldun Özcan',
    email: 'haldun.ozcan@obs.edu.tr',
    phone: '+90 (222) 335 05 81',
    facultyId: 'fac-1',
    facultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    departmentId: 'dept-1',
    departmentName: 'Bilgisayar Mühendisliği Bölümü',
    roomNumber: 'M-301',
    isActive: true,
    educationInfo: 'Lisans: ODTÜ Bilgisayar Müh. (1995)\nYüksek Lisans: Boğaziçi Üniv. (1998)\nDoktora: ETH Zurich (2003)',
    academicTasks: 'Mühendislik ve Doğa Bilimleri Fakültesi Dekanı (2020 - Devam)',
    articles: '1. Özcan, H. et al. (2025). "Distributed Fault Tolerance in Cloud Microservices", IEEE TSE.\n2. Özcan, H. (2024). "Scalable Consensus Algorithms", ACM Computing Surveys.',
  },
  {
    id: 'inst-2',
    title: 'Prof. Dr.',
    firstName: 'Ayşe',
    lastName: 'Karaca',
    fullName: 'Prof. Dr. Ayşe Karaca',
    email: 'ayse.karaca@obs.edu.tr',
    phone: '+90 (222) 335 05 82',
    facultyId: 'fac-2',
    facultyName: 'İktisadi ve İdari Bilimler Fakültesi',
    departmentId: 'dept-4',
    departmentName: 'İktisat Bölümü',
    roomNumber: 'İİBF-104',
    isActive: true,
    academicTasks: 'İktisadi ve İdari Bilimler Fakültesi Dekanı (2019 - Devam)',
  },
  {
    id: 'inst-3',
    title: 'Prof. Dr.',
    firstName: 'Kerem',
    lastName: 'Aydın',
    fullName: 'Prof. Dr. Kerem Aydın',
    email: 'kerem.aydin@obs.edu.tr',
    facultyId: 'fac-3',
    facultyName: 'Tıp Fakültesi',
    roomNumber: 'TIP-Dekanlık',
    isActive: true,
    academicTasks: 'Tıp Fakültesi Dekanı (2022 - Devam)',
  },
  {
    id: 'inst-4',
    title: 'Prof. Dr.',
    firstName: 'Selami',
    lastName: 'Şahin',
    fullName: 'Prof. Dr. Selami Şahin',
    email: 'selami.sahin@obs.edu.tr',
    facultyId: 'fac-4',
    facultyName: 'Hukuk Fakültesi',
    roomNumber: 'HUK-201',
    isActive: true,
    academicTasks: 'Hukuk Fakültesi Dekanı (2023 - Devam)',
  },
  {
    id: 'inst-5',
    title: 'Prof. Dr.',
    firstName: 'Ahmet',
    lastName: 'Bilgehan',
    fullName: 'Prof. Dr. Ahmet Bilgehan',
    email: 'ahmet.bilgehan@obs.edu.tr',
    facultyId: 'fac-1',
    facultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    departmentId: 'dept-1',
    departmentName: 'Bilgisayar Mühendisliği Bölümü',
    roomNumber: 'M-204',
    isActive: true,
    academicTasks: 'Bilgisayar Mühendisliği Bölüm Başkanı',
    coursesTaught: 'BLG101 Programlamaya Giriş\nBLG411 Bitirme Tasarım Projesi',
  },
  {
    id: 'inst-6',
    title: 'Doç. Dr.',
    firstName: 'Selin',
    lastName: 'Doğan',
    fullName: 'Doç. Dr. Selin Doğan',
    email: 'selin.dogan@obs.edu.tr',
    facultyId: 'fac-1',
    facultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    departmentId: 'dept-2',
    departmentName: 'Yazılım Mühendisliği Bölümü',
    roomNumber: 'M-208',
    isActive: true,
    coursesTaught: 'BLG452 Bulut Bilişim ve Mikroservisler',
  },
  {
    id: 'inst-7',
    title: 'Dr. Öğr. Üyesi',
    firstName: 'Murat',
    lastName: 'Kaya',
    fullName: 'Dr. Öğr. Üyesi Murat Kaya',
    email: 'murat.kaya@obs.edu.tr',
    facultyId: 'fac-1',
    facultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    departmentId: 'dept-1',
    departmentName: 'Bilgisayar Mühendisliği Bölümü',
    roomNumber: 'M-112',
    isActive: true,
    coursesTaught: 'BLG301 Veritabanı Yönetim Sistemleri',
  },
  {
    id: 'inst-8',
    title: 'Arş. Gör.',
    firstName: 'Canan',
    lastName: 'Tekin',
    fullName: 'Arş. Gör. Canan Tekin',
    email: 'canan.tekin@obs.edu.tr',
    facultyId: 'fac-1',
    facultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    departmentId: 'dept-1',
    departmentName: 'Bilgisayar Mühendisliği Bölümü',
    roomNumber: 'M-Lab2',
    isActive: true,
  },
];

const initialFaculties: FacultyItem[] = [
  { id: 'fac-1', code: 'MDBF', name: 'Mühendislik ve Doğa Bilimleri Fakültesi', deanId: 'inst-1', deanName: 'Prof. Dr. Haldun Özcan', establishedYear: 1993, isActive: true, departmentCount: 6 },
  { id: 'fac-2', code: 'IIBF', name: 'İktisadi ve İdari Bilimler Fakültesi', deanId: 'inst-2', deanName: 'Prof. Dr. Ayşe Karaca', establishedYear: 1982, isActive: true, departmentCount: 5 },
  { id: 'fac-3', code: 'TIP', name: 'Tıp Fakültesi', deanId: 'inst-3', deanName: 'Prof. Dr. Kerem Aydın', establishedYear: 2005, isActive: true, departmentCount: 3 },
  { id: 'fac-4', code: 'HUK', name: 'Hukuk Fakültesi', deanId: 'inst-4', deanName: 'Prof. Dr. Selami Şahin', establishedYear: 2010, isActive: true, departmentCount: 1 },
];

const initialDepartments: DepartmentItem[] = [
  { id: 'dept-1', facultyId: 'fac-1', facultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi', code: 'BLG', name: 'Bilgisayar Mühendisliği Bölümü', headName: 'Prof. Dr. Ahmet Bilgehan', language: '%100 İngilizce', minEctsLimit: 30, maxEctsLimit: 42, overCapEctsLimit: 48, isActive: true },
  { id: 'dept-2', facultyId: 'fac-1', facultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi', code: 'YZM', name: 'Yazılım Mühendisliği Bölümü', headName: 'Doç. Dr. Selin Doğan', language: 'Türkçe', minEctsLimit: 30, maxEctsLimit: 40, overCapEctsLimit: 45, isActive: true },
  { id: 'dept-3', facultyId: 'fac-1', facultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi', code: 'ELK', name: 'Elektrik-Elektronik Mühendisliği', headName: 'Prof. Dr. Cengiz Aksoy', language: '%30 İngilizce', minEctsLimit: 30, maxEctsLimit: 40, overCapEctsLimit: 45, isActive: true },
  { id: 'dept-4', facultyId: 'fac-2', facultyName: 'İktisadi ve İdari Bilimler Fakültesi', code: 'IKT', name: 'İktisat Bölümü', headName: 'Prof. Dr. Deniz Demirtaş', language: 'Türkçe', minEctsLimit: 30, maxEctsLimit: 38, overCapEctsLimit: 45, isActive: true },
  { id: 'dept-5', facultyId: 'fac-2', facultyName: 'İktisadi ve İdari Bilimler Fakültesi', code: 'ISL', name: 'İşletme Bölümü', headName: 'Doç. Dr. Harun Gürlek', language: '%100 İngilizce', minEctsLimit: 30, maxEctsLimit: 40, overCapEctsLimit: 45, isActive: true },
];

const initialCourses: CourseItem[] = [
  { id: 'c-1', departmentId: 'dept-1', departmentName: 'Bilgisayar Mühendisliği Bölümü', code: 'BLG101', name: 'Programlamaya Giriş (C/C++)', defaultEcts: 6, defaultCredit: 4, theoreticalHours: 3, practicalHours: 2, courseType: 'Zorunlu', isActive: true },
  { id: 'c-2', departmentId: 'dept-1', departmentName: 'Bilgisayar Mühendisliği Bölümü', code: 'MAT101', name: 'Matematik I (Kalkülüs)', defaultEcts: 6, defaultCredit: 4, theoreticalHours: 4, practicalHours: 0, courseType: 'Zorunlu', isActive: true },
  { id: 'c-3', departmentId: 'dept-1', departmentName: 'Bilgisayar Mühendisliği Bölümü', code: 'FZK101', name: 'Genel Fizik I', defaultEcts: 6, defaultCredit: 4, theoreticalHours: 3, practicalHours: 2, courseType: 'Zorunlu', isActive: true },
  { id: 'c-4', departmentId: 'dept-1', departmentName: 'Bilgisayar Mühendisliği Bölümü', code: 'BLG201', name: 'Veri Yapıları ve Algoritmalar', defaultEcts: 6, defaultCredit: 4, theoreticalHours: 3, practicalHours: 2, courseType: 'Zorunlu', isActive: true },
  { id: 'c-5', departmentId: 'dept-1', departmentName: 'Bilgisayar Mühendisliği Bölümü', code: 'BLG301', name: 'Veritabanı Yönetim Sistemleri', defaultEcts: 6, defaultCredit: 4, theoreticalHours: 3, practicalHours: 2, courseType: 'Zorunlu', isActive: true },
  { id: 'c-6', departmentId: 'dept-1', departmentName: 'Bilgisayar Mühendisliği Bölümü', code: 'BLG411', name: 'Bitirme Tasarım Projesi', defaultEcts: 8, defaultCredit: 3, theoreticalHours: 1, practicalHours: 4, courseType: 'Zorunlu', isActive: true },
  { id: 'c-7', departmentId: 'dept-1', departmentName: 'Bilgisayar Mühendisliği Bölümü', code: 'BLG452', name: 'Bulut Bilişim ve Mikroservisler', defaultEcts: 5, defaultCredit: 3, theoreticalHours: 3, practicalHours: 0, courseType: 'Seçmeli', isActive: true },
  { id: 'c-8', departmentId: 'dept-2', departmentName: 'Yazılım Mühendisliği Bölümü', code: 'YZM202', name: 'Yazılım Tasarım Kalıpları', defaultEcts: 5, defaultCredit: 3, theoreticalHours: 3, practicalHours: 0, courseType: 'Zorunlu', isActive: true },
];

const getStored = <T>(key: string, defaultVal: T): T => {
  try {
    const s = localStorage.getItem(key);
    if (s) return JSON.parse(s);
  } catch {}
  return defaultVal;
};

const setStored = <T>(key: string, val: T): void => {
  localStorage.setItem(key, JSON.stringify(val));
};

export const academicManagementApi = {
  // 0. AKADEMİK UNVANLAR
  getTitles: async (): Promise<AcademicTitleItem[]> => {
    return getStored('obs_academic_titles', initialTitles);
  },

  createTitle: async (title: string): Promise<AcademicTitleItem> => {
    const titles = getStored('obs_academic_titles', initialTitles);
    const newTitle: AcademicTitleItem = {
      id: `title-${Date.now()}`,
      title: title.trim(),
      order: titles.length + 1,
    };
    const updated = [...titles, newTitle];
    setStored('obs_academic_titles', updated);
    return newTitle;
  },

  deleteTitle: async (id: string): Promise<void> => {
    const titles = getStored('obs_academic_titles', initialTitles);
    setStored('obs_academic_titles', titles.filter((t) => t.id !== id));
  },

  // 1. EĞİTMEN & AKADEMİSYEN YÖNETİMİ
  getInstructors: async (facultyId?: string, departmentId?: string): Promise<InstructorItem[]> => {
    const list = getStored('obs_instructors', initialInstructors);
    return list.filter((inst) => {
      if (facultyId && facultyId !== 'ALL' && inst.facultyId !== facultyId) return false;
      if (departmentId && departmentId !== 'ALL' && inst.departmentId !== departmentId) return false;
      return true;
    });
  },

  createInstructor: async (data: Omit<InstructorItem, 'id' | 'fullName' | 'isActive'>): Promise<InstructorItem> => {
    const list = getStored('obs_instructors', initialInstructors);
    const faculties = getStored('obs_faculties', initialFaculties);
    const departments = getStored('obs_departments', initialDepartments);

    const faculty = faculties.find((f) => f.id === data.facultyId);
    const department = departments.find((d) => d.id === data.departmentId);

    const titlePrefix = data.title === 'Unvansız' ? '' : `${data.title} `;
    const fullName = `${titlePrefix}${data.firstName} ${data.lastName}`.trim();

    const newInst: InstructorItem = {
      ...data,
      id: `inst-${Date.now()}`,
      fullName,
      facultyName: faculty ? faculty.name : data.facultyName,
      departmentName: department ? department.name : data.departmentName,
      isActive: true,
    };

    const updated = [...list, newInst];
    setStored('obs_instructors', updated);
    return newInst;
  },

  updateInstructor: async (id: string, data: Partial<InstructorItem>): Promise<void> => {
    const list = getStored('obs_instructors', initialInstructors);
    const updated = list.map((inst) => {
      if (inst.id === id) {
        const merged = { ...inst, ...data };
        const titlePrefix = merged.title === 'Unvansız' ? '' : `${merged.title} `;
        merged.fullName = `${titlePrefix}${merged.firstName} ${merged.lastName}`.trim();
        return merged;
      }
      return inst;
    });
    setStored('obs_instructors', updated);
  },

  deleteInstructor: async (id: string): Promise<void> => {
    const list = getStored('obs_instructors', initialInstructors);
    setStored('obs_instructors', list.filter((inst) => inst.id !== id));
  },

  // 2. FAKÜLTE YÖNETİMİ (DEKAN SEÇİMİ ENTEGRASYONLU)
  getFaculties: async (): Promise<FacultyItem[]> => {
    try {
      const res = await apiClient.get<FacultyItem[]>('/api/v1/faculties');
      if (res.data && res.data.length > 0) return res.data;
    } catch {}
    return getStored('obs_faculties', initialFaculties);
  },

  createFaculty: async (data: { code: string; name: string; deanId?: string; establishedYear?: number }): Promise<FacultyItem> => {
    const faculties = getStored('obs_faculties', initialFaculties);
    const instructors = getStored('obs_instructors', initialInstructors);
    const dean = instructors.find((i) => i.id === data.deanId);

    const newFaculty: FacultyItem = {
      id: `fac-${Date.now()}`,
      code: data.code.toUpperCase(),
      name: data.name,
      deanId: data.deanId,
      deanName: dean ? dean.fullName : 'Atama Bekliyor',
      establishedYear: data.establishedYear || new Date().getFullYear(),
      isActive: true,
      departmentCount: 0,
    };
    const updated = [...faculties, newFaculty];
    setStored('obs_faculties', updated);
    return newFaculty;
  },

  updateFaculty: async (id: string, data: Partial<FacultyItem>): Promise<void> => {
    const faculties = getStored('obs_faculties', initialFaculties);
    const instructors = getStored('obs_instructors', initialInstructors);
    
    let deanName = data.deanName;
    if (data.deanId) {
      const dean = instructors.find((i) => i.id === data.deanId);
      if (dean) deanName = dean.fullName;
    }

    const updated = faculties.map((f) => (f.id === id ? { ...f, ...data, ...(deanName ? { deanName } : {}) } : f));
    setStored('obs_faculties', updated);
  },

  deleteFaculty: async (id: string): Promise<void> => {
    const faculties = getStored('obs_faculties', initialFaculties);
    setStored('obs_faculties', faculties.filter((f) => f.id !== id));
  },

  // 3. BÖLÜM YÖNETİMİ & AKTS SINIRI AYARLARI
  getDepartments: async (facultyId?: string): Promise<DepartmentItem[]> => {
    try {
      const res = await apiClient.get<DepartmentItem[]>('/api/v1/departments', { params: { facultyId } });
      if (res.data && res.data.length > 0) return res.data;
    } catch {}
    const list = getStored('obs_departments', initialDepartments);
    if (facultyId && facultyId !== 'ALL') {
      return list.filter((d) => d.facultyId === facultyId);
    }
    return list;
  },

  createDepartment: async (data: {
    facultyId: string;
    code: string;
    name: string;
    headName?: string;
    language?: string;
    minEctsLimit: number;
    maxEctsLimit: number;
    overCapEctsLimit?: number;
  }): Promise<DepartmentItem> => {
    const departments = getStored('obs_departments', initialDepartments);
    const faculties = getStored('obs_faculties', initialFaculties);
    const faculty = faculties.find((f) => f.id === data.facultyId);

    const newDept: DepartmentItem = {
      id: `dept-${Date.now()}`,
      facultyId: data.facultyId,
      facultyName: faculty ? faculty.name : 'Bilinmeyen Fakülte',
      code: data.code.toUpperCase(),
      name: data.name,
      headName: data.headName || 'Atama Bekliyor',
      language: data.language || 'Türkçe',
      minEctsLimit: data.minEctsLimit || 30,
      maxEctsLimit: data.maxEctsLimit || 42,
      overCapEctsLimit: data.overCapEctsLimit || 48,
      isActive: true,
    };

    const updated = [...departments, newDept];
    setStored('obs_departments', updated);

    // Fakültenin bölüm sayısını güncelle
    if (faculty) {
      faculty.departmentCount = (faculty.departmentCount || 0) + 1;
      setStored('obs_faculties', faculties);
    }

    return newDept;
  },

  updateDepartment: async (id: string, data: Partial<DepartmentItem>): Promise<void> => {
    const departments = getStored('obs_departments', initialDepartments);
    const updated = departments.map((d) => (d.id === id ? { ...d, ...data } : d));
    setStored('obs_departments', updated);
  },

  updateDepartmentEctsLimits: async (
    departmentId: string,
    limits: { minEctsLimit: number; maxEctsLimit: number; overCapEctsLimit?: number }
  ): Promise<void> => {
    const departments = getStored('obs_departments', initialDepartments);
    const updated = departments.map((d) => (d.id === departmentId ? { ...d, ...limits } : d));
    setStored('obs_departments', updated);
  },

  deleteDepartment: async (id: string): Promise<void> => {
    const departments = getStored('obs_departments', initialDepartments);
    setStored('obs_departments', departments.filter((d) => d.id !== id));
  },

  // 4. KATALOG DERSLERİ & MÜFREDAT
  getCourses: async (departmentId?: string): Promise<CourseItem[]> => {
    try {
      const res = await apiClient.get<CourseItem[]>('/api/v1/courses', { params: { departmentId } });
      if (res.data && res.data.length > 0) return res.data;
    } catch {}
    const list = getStored('obs_courses', initialCourses);
    if (departmentId && departmentId !== 'ALL') {
      return list.filter((c) => c.departmentId === departmentId);
    }
    return list;
  },

  createCourse: async (data: Omit<CourseItem, 'id' | 'isActive'>): Promise<CourseItem> => {
    const courses = getStored('obs_courses', initialCourses);
    const departments = getStored('obs_departments', initialDepartments);
    const dept = departments.find((d) => d.id === data.departmentId);

    const newCourse: CourseItem = {
      ...data,
      id: `c-${Date.now()}`,
      departmentName: dept ? dept.name : '',
      isActive: true,
    };
    const updated = [...courses, newCourse];
    setStored('obs_courses', updated);
    return newCourse;
  },

  updateCourse: async (id: string, data: Partial<CourseItem>): Promise<void> => {
    const courses = getStored('obs_courses', initialCourses);
    const updated = courses.map((c) => (c.id === id ? { ...c, ...data } : c));
    setStored('obs_courses', updated);
  },

  deleteCourse: async (id: string): Promise<void> => {
    const courses = getStored('obs_courses', initialCourses);
    setStored('obs_courses', courses.filter((c) => c.id !== id));
  },

  // 5. DÖNEMLİK ŞUBELER & DERS AÇIMLARI
  getCourseOfferings: async (termId?: string): Promise<CourseOfferingItem[]> => {
    try {
      const res = await apiClient.get<CourseOfferingItem[]>('/api/v1/course-offerings', { params: { academicTermId: termId } });
      return res.data;
    } catch {
      return [
        { id: 'off-1', courseId: 'c-6', courseCode: 'BLG411', courseName: 'Bitirme Tasarım Projesi', academicTermId: 'term-1', section: 'A', capacity: 50, enrolledCount: 42, primaryInstructorName: 'Prof. Dr. Ahmet Bilgehan', isRegistrationOpen: true },
        { id: 'off-2', courseId: 'c-7', courseCode: 'BLG452', courseName: 'Bulut Bilişim ve Mikroservisler', academicTermId: 'term-1', section: 'A', capacity: 60, enrolledCount: 58, primaryInstructorName: 'Doç. Dr. Selin Doğan', isRegistrationOpen: true },
        { id: 'off-3', courseId: 'c-5', courseCode: 'BLG301', courseName: 'Veritabanı Yönetim Sistemleri', academicTermId: 'term-1', section: 'B', capacity: 80, enrolledCount: 75, primaryInstructorName: 'Dr. Öğr. Üyesi Murat Kaya', isRegistrationOpen: true },
      ];
    }
  },

  // 6. ÖĞRENCİ YÖNETİMİ
  getStudents: async (searchTerm?: string): Promise<StudentListItem[]> => {
    try {
      const res = await apiClient.get<StudentListItem[]>('/api/v1/students', { params: { searchTerm } });
      return res.data;
    } catch {
      return [
        { id: 'stu-1', studentNumber: '20210101042', firstName: 'İsmailcan', lastName: 'Uslu', fullName: 'İsmailcan Uslu', email: 'ogrenci@obs.edu.tr', departmentName: 'Bilgisayar Mühendisliği', programName: 'Bilgisayar Mühendisliği (Lisans)', enrollmentYear: 2021, status: 'Active', gpa: 3.78 },
        { id: 'stu-2', studentNumber: '20220101015', firstName: 'Elif', lastName: 'Kaya', fullName: 'Elif Kaya', email: 'elif.kaya@obs.edu.tr', departmentName: 'Bilgisayar Mühendisliği', programName: 'Bilgisayar Mühendisliği (Lisans)', enrollmentYear: 2022, status: 'Active', gpa: 3.45 },
        { id: 'stu-3', studentNumber: '20230102008', firstName: 'Mehmet', lastName: 'Demir', fullName: 'Mehmet Demir', email: 'mehmet.demir@obs.edu.tr', departmentName: 'Yazılım Mühendisliği', programName: 'Yazılım Mühendisliği (Lisans)', enrollmentYear: 2023, status: 'Active', gpa: 3.12 },
      ];
    }
  },

  createStudent: async (data: {
    studentNumber: string;
    firstName: string;
    lastName: string;
    email: string;
    departmentId: string;
    programId: string;
    curriculumVersionId: string;
    enrollmentYear: number;
  }): Promise<void> => {
    await apiClient.post('/api/v1/students', {
      ...data,
      enrollmentDateUtc: new Date().toISOString(),
    });
  },

  // 7. AKADEMİK TAKVİM
  getAcademicYears: async (): Promise<AcademicYearItem[]> => {
    try {
      const res = await apiClient.get<AcademicYearItem[]>('/api/v1/academic-years');
      return res.data;
    } catch {
      return [
        {
          id: 'year-1',
          code: '2026-2027',
          name: '2026-2027 Akademik Yılı',
          startDate: '2026-09-15',
          endDate: '2027-06-20',
          isActive: true,
          terms: [
            { id: 'term-1', termType: 'Fall', name: '2026-2027 Güz Dönemi', startDate: '2026-09-15', endDate: '2027-01-20', isRegistrationOpen: true },
            { id: 'term-2', termType: 'Spring', name: '2026-2027 Bahar Dönemi', startDate: '2027-02-15', endDate: '2027-06-20', isRegistrationOpen: false },
          ],
        },
      ];
    }
  },

  setTermRegistrationStatus: async (termId: string, isOpen: boolean): Promise<void> => {
    await apiClient.put(`/api/v1/academic-terms/${termId}/registration`, { isRegistrationOpen: isOpen });
  },
};
