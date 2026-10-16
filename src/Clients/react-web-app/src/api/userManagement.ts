export interface AdminUnitItem {
  id: string;
  code: string;
  name: string;
  parentUnit?: string;
  managerName?: string;
  phone?: string;
  email?: string;
  location?: string;
  isActive: boolean;
}

export interface StaffTitleItem {
  id: string;
  code: string;
  title: string;
  category: 'Genel İdare' | 'Teknik Hizmetler' | 'Sağlık Hizmetleri' | 'Yardımcı Hizmetler';
  order: number;
}

export interface SystemRoleItem {
  id: string;
  code: string; // 'ADMIN', 'STAFF', 'REGISTRAR', 'FINANCE', vb.
  name: string;
  description: string;
  permissions: string[];
  userCount?: number;
  isSystemProtected?: boolean;
}

export interface StaffItem {
  id: string;
  registrationNumber: string; // Sicil No
  titleId?: string;
  title: string; // Unvan: 'Daire Başkanı', 'Fakülte Sekreteri', 'Şef', 'Memur', vb.
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone?: string;
  unitId?: string;
  unitName: string; // Çalıştığı Birim
  roleId?: string;
  role: string; // Sistem Rol Kodu: 'ADMIN', 'STAFF', vb.
  roleName?: string;
  isActive: boolean;
}

export interface GuestStudentItem {
  id: string;
  guestNumber: string; // Misafir Öğrenci No: 'M2026001'
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  originUniversity: string;
  exchangeProgram: 'Erasmus+' | 'Farabi' | 'Mevlana' | 'Özel Öğrenci' | 'Serbest Değişim';
  hostFacultyName: string;
  hostDepartmentName: string;
  academicYear: string;
  term: 'Güz' | 'Bahar' | 'Tam Yıl';
  status: 'Active' | 'Completed' | 'Pending';
}

// 1. Başlangıç İdari Birimleri
const initialUnits: AdminUnitItem[] = [
  { id: 'unit-1', code: 'OIDB', name: 'Öğrenci İşleri Daire Başkanlığı', managerName: 'Hasan Korkmaz', phone: '+90 (222) 335 05 90', email: 'oidb@obs.edu.tr', location: 'Rektörlük Binası Zemin Kat', isActive: true },
  { id: 'unit-2', code: 'BIDB', name: 'Bilgi İşlem Daire Başkanlığı', managerName: 'Mehmet Ali Eren', phone: '+90 (222) 335 05 95', email: 'bidb@obs.edu.tr', location: 'Merkezi Kütüphane Yanı Blok B', isActive: true },
  { id: 'unit-3', code: 'PDB', name: 'Personel Daire Başkanlığı', managerName: 'Hülya Şener', phone: '+90 (222) 335 05 85', email: 'personel@obs.edu.tr', location: 'Rektörlük 2. Kat', isActive: true },
  { id: 'unit-4', code: 'SKS', name: 'Sağlık, Kültür ve Spor Daire Başkanlığı', managerName: 'Serdar Yıldız', phone: '+90 (222) 335 05 70', email: 'sks@obs.edu.tr', location: 'Öğrenci Merkezi', isActive: true },
  { id: 'unit-5', code: 'MDBF-SEK', name: 'Mühendislik ve Doğa Bilimleri Fakülte Sekreterliği', managerName: 'Zeynep Aktaş', phone: '+90 (222) 335 05 40', email: 'mdbf.sekreterlik@obs.edu.tr', location: 'Mühendislik Dekanlık Girişi', isActive: true },
  { id: 'unit-6', code: 'IIBF-SEK', name: 'İktisadi ve İdari Bilimler Fakülte Sekreterliği', managerName: 'Cemil Taş', phone: '+90 (222) 335 05 50', email: 'iibf.sekreterlik@obs.edu.tr', location: 'İİBF Zemin Kat', isActive: true },
];

// 2. Başlangıç İdari Unvanları
const initialStaffTitles: StaffTitleItem[] = [
  { id: 'st-title-1', code: 'DB', title: 'Daire Başkanı', category: 'Genel İdare', order: 1 },
  { id: 'st-title-2', code: 'FS', title: 'Fakülte Sekreteri', category: 'Genel İdare', order: 2 },
  { id: 'st-title-3', code: 'ES', title: 'Enstitü Sekreteri', category: 'Genel İdare', order: 3 },
  { id: 'st-title-4', code: 'SM', title: 'Şube Müdürü', category: 'Genel İdare', order: 4 },
  { id: 'st-title-5', code: 'SEF', title: 'Şef', category: 'Genel İdare', order: 5 },
  { id: 'st-title-6', code: 'BI', title: 'Bilgisayar İşletmeni', category: 'Teknik Hizmetler', order: 6 },
  { id: 'st-title-7', code: 'MUH', title: 'Mühendis / Uzman', category: 'Teknik Hizmetler', order: 7 },
  { id: 'st-title-8', code: 'TEK', title: 'Tekniker', category: 'Teknik Hizmetler', order: 8 },
  { id: 'st-title-9', code: 'MEM', title: 'Memur', category: 'Genel İdare', order: 9 },
  { id: 'st-title-10', code: 'VHKI', title: 'Veri Hazırlama ve Kontrol İşletmeni (VHKİ)', category: 'Genel İdare', order: 10 },
];

// 3. Başlangıç Sistem Rolleri
const initialSystemRoles: SystemRoleItem[] = [
  {
    id: 'role-1',
    code: 'ADMIN',
    name: 'Sistem Yöneticisi (Super Admin)',
    description: 'Tüm modüllere, kullanıcı hesaplarına, kurumsal ayarlara ve veritabanı parametrelerine tam yetki.',
    permissions: ['ALL_PERMISSIONS', 'USER_MANAGEMENT', 'SYSTEM_SETTINGS', 'ACADEMIC_ADMIN'],
    userCount: 2,
    isSystemProtected: true,
  },
  {
    id: 'role-2',
    code: 'STAFF',
    name: 'Öğrenci İşleri Masası Yetkilisi',
    description: 'Öğrenci kayıtları, başvuru inceleme, ders intibakları, transkript ve mezuniyet işlemleri.',
    permissions: ['STUDENT_ENROLLMENT', 'APPLICATION_REVIEW', 'TRANSCRIPT_ISSUE', 'COURSE_CATALOG_VIEW'],
    userCount: 6,
    isSystemProtected: true,
  },
  {
    id: 'role-3',
    code: 'REGISTRAR',
    name: 'Kayıt Kabul & Evrak Memuru',
    description: 'Öğrenci asil kayıt belgelerinin incelenmesi, kimlik kartı basımı ve lise diploma doğrulaması.',
    permissions: ['STUDENT_ENROLLMENT_DOCS', 'STUDENT_CARD_PRINT'],
    userCount: 4,
    isSystemProtected: false,
  },
  {
    id: 'role-4',
    code: 'DEPARTMENT_SECRETARY',
    name: 'Bölüm & Fakülte Sekreteryası',
    description: 'Bölüm ders açımları, sınav programı girişi ve yönetim kurulu karar takibi.',
    permissions: ['OFFERING_MANAGEMENT', 'EXAM_SCHEDULE_ENTRY'],
    userCount: 8,
    isSystemProtected: false,
  },
];

// 4. Başlangıç Personeli
const initialStaff: StaffItem[] = [
  {
    id: 'st-1',
    registrationNumber: 'PER2018042',
    titleId: 'st-title-1',
    title: 'Daire Başkanı',
    firstName: 'Hasan',
    lastName: 'Korkmaz',
    fullName: 'Hasan Korkmaz',
    email: 'hasan.korkmaz@obs.edu.tr',
    phone: '+90 (222) 335 05 90',
    unitId: 'unit-1',
    unitName: 'Öğrenci İşleri Daire Başkanlığı',
    roleId: 'role-1',
    role: 'ADMIN',
    roleName: 'Sistem Yöneticisi (Super Admin)',
    isActive: true,
  },
  {
    id: 'st-2',
    registrationNumber: 'PER2020115',
    titleId: 'st-title-2',
    title: 'Fakülte Sekreteri',
    firstName: 'Zeynep',
    lastName: 'Aktaş',
    fullName: 'Zeynep Aktaş',
    email: 'zeynep.aktas@obs.edu.tr',
    phone: '+90 (222) 335 05 91',
    unitId: 'unit-5',
    unitName: 'Mühendislik ve Doğa Bilimleri Fakülte Sekreterliği',
    roleId: 'role-2',
    role: 'STAFF',
    roleName: 'Öğrenci İşleri Masası Yetkilisi',
    isActive: true,
  },
  {
    id: 'st-3',
    registrationNumber: 'PER2022088',
    titleId: 'st-title-5',
    title: 'Şef',
    firstName: 'Mustafa',
    lastName: 'Güler',
    fullName: 'Mustafa Güler',
    email: 'mustafa.guler@obs.edu.tr',
    phone: '+90 (222) 335 05 92',
    unitId: 'unit-1',
    unitName: 'Öğrenci İşleri Daire Başkanlığı',
    roleId: 'role-2',
    role: 'STAFF',
    roleName: 'Öğrenci İşleri Masası Yetkilisi',
    isActive: true,
  },
  {
    id: 'st-4',
    registrationNumber: 'PER2023004',
    titleId: 'st-title-6',
    title: 'Bilgisayar İşletmeni',
    firstName: 'Burak',
    lastName: 'Çetin',
    fullName: 'Burak Çetin',
    email: 'burak.cetin@obs.edu.tr',
    phone: '+90 (222) 335 05 93',
    unitId: 'unit-2',
    unitName: 'Bilgi İşlem Daire Başkanlığı',
    roleId: 'role-1',
    role: 'ADMIN',
    roleName: 'Sistem Yöneticisi (Super Admin)',
    isActive: true,
  },
];

const initialGuestStudents: GuestStudentItem[] = [
  {
    id: 'gst-1',
    guestNumber: 'M2026ER001',
    firstName: 'Lukas',
    lastName: 'Müller',
    fullName: 'Lukas Müller',
    email: 'lukas.muller@uni-heidelberg.de',
    originUniversity: 'Heidelberg Üniversitesi (Almanya)',
    exchangeProgram: 'Erasmus+',
    hostFacultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    hostDepartmentName: 'Bilgisayar Mühendisliği Bölümü',
    academicYear: '2026-2027',
    term: 'Güz',
    status: 'Active',
  },
  {
    id: 'gst-2',
    guestNumber: 'M2026FA002',
    firstName: 'Gökçe',
    lastName: 'Yıldırım',
    fullName: 'Gökçe Yıldırım',
    email: 'gokce.yildirim@hacettepe.edu.tr',
    originUniversity: 'Hacettepe Üniversitesi',
    exchangeProgram: 'Farabi',
    hostFacultyName: 'İktisadi ve İdari Bilimler Fakültesi',
    hostDepartmentName: 'İşletme Bölümü',
    academicYear: '2026-2027',
    term: 'Tam Yıl',
    status: 'Active',
  },
  {
    id: 'gst-3',
    guestNumber: 'M2026OZ003',
    firstName: 'Dmitri',
    lastName: 'Ivanov',
    fullName: 'Dmitri Ivanov',
    email: 'dmitri.ivanov@mail.ru',
    originUniversity: 'Moskova Devlet Üniversitesi',
    exchangeProgram: 'Özel Öğrenci',
    hostFacultyName: 'Mühendislik ve Doğa Bilimleri Fakültesi',
    hostDepartmentName: 'Yazılım Mühendisliği Bölümü',
    academicYear: '2026-2027',
    term: 'Bahar',
    status: 'Pending',
  },
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

export const userManagementApi = {
  // 1. İDARİ BİRİMLER (UNITS)
  getUnits: async (): Promise<AdminUnitItem[]> => {
    return getStored('obs_admin_units', initialUnits);
  },

  createUnit: async (data: Omit<AdminUnitItem, 'id' | 'isActive'>): Promise<AdminUnitItem> => {
    const list = getStored('obs_admin_units', initialUnits);
    const newUnit: AdminUnitItem = {
      ...data,
      id: `unit-${Date.now()}`,
      isActive: true,
    };
    const updated = [...list, newUnit];
    setStored('obs_admin_units', updated);
    return newUnit;
  },

  updateUnit: async (id: string, data: Partial<AdminUnitItem>): Promise<void> => {
    const list = getStored('obs_admin_units', initialUnits);
    const updated = list.map((u) => (u.id === id ? { ...u, ...data } : u));
    setStored('obs_admin_units', updated);
  },

  deleteUnit: async (id: string): Promise<void> => {
    const list = getStored('obs_admin_units', initialUnits);
    setStored('obs_admin_units', list.filter((u) => u.id !== id));
  },

  // 2. İDARİ KADRO UNVANLARI (STAFF TITLES)
  getStaffTitles: async (): Promise<StaffTitleItem[]> => {
    return getStored('obs_staff_titles', initialStaffTitles);
  },

  createStaffTitle: async (data: Omit<StaffTitleItem, 'id' | 'order'>): Promise<StaffTitleItem> => {
    const list = getStored('obs_staff_titles', initialStaffTitles);
    const newTitle: StaffTitleItem = {
      ...data,
      id: `st-title-${Date.now()}`,
      order: list.length + 1,
    };
    const updated = [...list, newTitle];
    setStored('obs_staff_titles', updated);
    return newTitle;
  },

  updateStaffTitle: async (id: string, data: Partial<StaffTitleItem>): Promise<void> => {
    const list = getStored('obs_staff_titles', initialStaffTitles);
    const updated = list.map((t) => (t.id === id ? { ...t, ...data } : t));
    setStored('obs_staff_titles', updated);
  },

  deleteStaffTitle: async (id: string): Promise<void> => {
    const list = getStored('obs_staff_titles', initialStaffTitles);
    setStored('obs_staff_titles', list.filter((t) => t.id !== id));
  },

  // 3. SİSTEM ROLLERİ & YETKİLER (SYSTEM ROLES)
  getSystemRoles: async (): Promise<SystemRoleItem[]> => {
    return getStored('obs_system_roles', initialSystemRoles);
  },

  createSystemRole: async (data: Omit<SystemRoleItem, 'id' | 'userCount' | 'isSystemProtected'>): Promise<SystemRoleItem> => {
    const list = getStored('obs_system_roles', initialSystemRoles);
    const newRole: SystemRoleItem = {
      ...data,
      id: `role-${Date.now()}`,
      userCount: 0,
      isSystemProtected: false,
    };
    const updated = [...list, newRole];
    setStored('obs_system_roles', updated);
    return newRole;
  },

  updateSystemRole: async (id: string, data: Partial<SystemRoleItem>): Promise<void> => {
    const list = getStored('obs_system_roles', initialSystemRoles);
    const updated = list.map((r) => (r.id === id ? { ...r, ...data } : r));
    setStored('obs_system_roles', updated);
  },

  deleteSystemRole: async (id: string): Promise<void> => {
    const list = getStored('obs_system_roles', initialSystemRoles);
    setStored('obs_system_roles', list.filter((r) => r.id !== id));
  },

  // 4. İDARİ PERSONEL KADROSU (STAFF)
  getStaff: async (): Promise<StaffItem[]> => {
    return getStored('obs_staff_list', initialStaff);
  },

  createStaff: async (data: {
    registrationNumber: string;
    titleId?: string;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    unitId?: string;
    roleId?: string;
  }): Promise<StaffItem> => {
    const list = getStored('obs_staff_list', initialStaff);
    const units = getStored('obs_admin_units', initialUnits);
    const titles = getStored('obs_staff_titles', initialStaffTitles);
    const roles = getStored('obs_system_roles', initialSystemRoles);

    const unit = units.find((u) => u.id === data.unitId);
    const title = titles.find((t) => t.id === data.titleId);
    const role = roles.find((r) => r.id === data.roleId);

    const newStaff: StaffItem = {
      id: `st-${Date.now()}`,
      registrationNumber: data.registrationNumber.toUpperCase(),
      titleId: data.titleId,
      title: title ? title.title : 'Memur',
      firstName: data.firstName,
      lastName: data.lastName,
      fullName: `${data.firstName} ${data.lastName}`.trim(),
      email: data.email,
      phone: data.phone,
      unitId: data.unitId,
      unitName: unit ? unit.name : 'Genel İdare',
      roleId: data.roleId,
      role: role ? role.code : 'STAFF',
      roleName: role ? role.name : 'Öğrenci İşleri Masası Yetkilisi',
      isActive: true,
    };

    const updated = [...list, newStaff];
    setStored('obs_staff_list', updated);
    return newStaff;
  },

  updateStaff: async (id: string, data: Partial<StaffItem>): Promise<void> => {
    const list = getStored('obs_staff_list', initialStaff);
    const units = getStored('obs_admin_units', initialUnits);
    const titles = getStored('obs_staff_titles', initialStaffTitles);
    const roles = getStored('obs_system_roles', initialSystemRoles);

    const updated = list.map((s) => {
      if (s.id === id) {
        const merged = { ...s, ...data };
        if (data.firstName || data.lastName) {
          merged.fullName = `${merged.firstName} ${merged.lastName}`.trim();
        }
        if (data.unitId) {
          const u = units.find((x) => x.id === data.unitId);
          if (u) merged.unitName = u.name;
        }
        if (data.titleId) {
          const t = titles.find((x) => x.id === data.titleId);
          if (t) merged.title = t.title;
        }
        if (data.roleId) {
          const r = roles.find((x) => x.id === data.roleId);
          if (r) {
            merged.role = r.code;
            merged.roleName = r.name;
          }
        }
        return merged;
      }
      return s;
    });
    setStored('obs_staff_list', updated);
  },

  deleteStaff: async (id: string): Promise<void> => {
    const list = getStored('obs_staff_list', initialStaff);
    setStored('obs_staff_list', list.filter((s) => s.id !== id));
  },

  // 5. MİSAFİR & DEĞİŞİM ÖĞRENCİLERİ
  getGuestStudents: async (): Promise<GuestStudentItem[]> => {
    return getStored('obs_guest_students', initialGuestStudents);
  },

  createGuestStudent: async (data: Omit<GuestStudentItem, 'id' | 'fullName'>): Promise<GuestStudentItem> => {
    const list = getStored('obs_guest_students', initialGuestStudents);
    const newGuest: GuestStudentItem = {
      ...data,
      id: `gst-${Date.now()}`,
      fullName: `${data.firstName} ${data.lastName}`.trim(),
    };
    const updated = [...list, newGuest];
    setStored('obs_guest_students', updated);
    return newGuest;
  },

  updateGuestStudent: async (id: string, data: Partial<GuestStudentItem>): Promise<void> => {
    const list = getStored('obs_guest_students', initialGuestStudents);
    const updated = list.map((g) => {
      if (g.id === id) {
        const merged = { ...g, ...data };
        merged.fullName = `${merged.firstName} ${merged.lastName}`.trim();
        return merged;
      }
      return g;
    });
    setStored('obs_guest_students', updated);
  },

  deleteGuestStudent: async (id: string): Promise<void> => {
    const list = getStored('obs_guest_students', initialGuestStudents);
    setStored('obs_guest_students', list.filter((g) => g.id !== id));
  },
};

export const getUnits = userManagementApi.getUnits;
export const createUnit = userManagementApi.createUnit;
export const updateUnit = userManagementApi.updateUnit;
export const deleteUnit = userManagementApi.deleteUnit;

export const getStaffTitles = userManagementApi.getStaffTitles;
export const createStaffTitle = userManagementApi.createStaffTitle;
export const updateStaffTitle = userManagementApi.updateStaffTitle;
export const deleteStaffTitle = userManagementApi.deleteStaffTitle;

export const getSystemRoles = userManagementApi.getSystemRoles;
export const createSystemRole = userManagementApi.createSystemRole;
export const updateSystemRole = userManagementApi.updateSystemRole;
export const deleteSystemRole = userManagementApi.deleteSystemRole;

