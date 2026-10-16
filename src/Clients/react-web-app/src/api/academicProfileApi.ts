// src/api/academicProfileApi.ts
// Eğitmenlerin Öğrenim, Yayın, Kitap, Bildiri, Proje ve Açık Erişim (ORCID, Google Scholar vb.) profilleri için API

export interface EducationEntry {
  id: string;
  degree: 'Lisans' | 'Yüksek Lisans' | 'Doktora' | 'Tıpta Uzmanlık' | 'Sanatta Yeterlik' | 'Post-Doc';
  university: string;
  faculty: string;
  department: string;
  startYear: number;
  endYear?: number;
  isOngoing?: boolean;
  thesisTitle?: string;
  thesisAdvisor?: string;
}

export interface ArticleEntry {
  id: string;
  title: string;
  journal: string;
  year: number;
  volumeIssue?: string;
  pages?: string;
  indexType: 'SCI-Expanded' | 'SSCI' | 'AHCI' | 'ESCI' | 'Scopus' | 'TR Dizin' | 'Diğer Uluslararası' | 'Ulusal Hakemli';
  doi?: string;
  url?: string;
  coAuthors?: string;
}

export interface BookEntry {
  id: string;
  title: string;
  type: 'Telif Kitap' | 'Kitap Bölümü' | 'Kitap Çevirisi' | 'Editörlük';
  publisher: string;
  city?: string;
  year: number;
  isbn?: string;
  url?: string;
  chapterTitle?: string;
}

export interface ConferenceEntry {
  id: string;
  title: string;
  conferenceName: string;
  cityCountry: string;
  date: string;
  presentationType: 'Sözlü Sunum' | 'Poster Sunum' | 'Davetli Konuşmacı';
  publishedType: 'Tam Metin' | 'Özet Metin';
  url?: string;
}

export interface ProjectEntry {
  id: string;
  title: string;
  agency: 'TÜBİTAK 1001' | 'TÜBİTAK 1002' | 'TÜBİTAK 3501' | 'BAP' | 'AB Horizon' | 'Erasmus+' | 'Sanayi / Ar-Ge' | 'Diğer';
  projectRole: 'Yürütücü' | 'Araştırmacı' | 'Danışman' | 'Bursiyer';
  budget?: string;
  currency?: 'TRY' | 'USD' | 'EUR';
  startDate: string;
  endDate?: string;
  status: 'Tamamlandı' | 'Devam Ediyor' | 'Öneri Aşamasında';
  url?: string;
}

export interface CourseEntry {
  id: string;
  courseCode: string;
  courseName: string;
  term: 'Güz' | 'Bahar' | 'Yaz';
  level: 'Ön Lisans' | 'Lisans' | 'Yüksek Lisans' | 'Doktora';
  hoursPerWeek: number;
  language: string;
}

export interface AdvisingEntry {
  id: string;
  studentName: string;
  level: 'Yüksek Lisans' | 'Doktora';
  thesisTitle: string;
  year: number;
  status: 'Tamamlandı' | 'Devam Ediyor';
}

export interface OpenAccessProfile {
  orcidId: string;
  orcidUrl: string;
  scholarId: string;
  scholarUrl: string;
  yoksisId: string;
  scopusId: string;
  researchgateUrl?: string;
  personalWebsite?: string;
}

export interface InstructorAcademicProfile {
  instructorId: string;
  educations: EducationEntry[];
  articles: ArticleEntry[];
  books: BookEntry[];
  conferences: ConferenceEntry[];
  projects: ProjectEntry[];
  courses: CourseEntry[];
  advisings: AdvisingEntry[];
  openAccess: OpenAccessProfile;
}

const STORAGE_KEY_PREFIX = 'obs_academic_profile_';

const initialSampleProfiles: Record<string, InstructorAcademicProfile> = {
  'inst-1': {
    instructorId: 'inst-1',
    openAccess: {
      orcidId: '0000-0002-1825-0097',
      orcidUrl: 'https://orcid.org/0000-0002-1825-0097',
      scholarId: 'AbC1234XYZ',
      scholarUrl: 'https://scholar.google.com/citations?user=AbC1234XYZ',
      yoksisId: '1098421',
      scopusId: '57193829100',
      researchgateUrl: 'https://researchgate.net/profile/Ahmet-Kaya-4',
      personalWebsite: 'https://ahmetkaya.itu.edu.tr'
    },
    educations: [
      {
        id: 'edu-1',
        degree: 'Doktora',
        university: 'İstanbul Teknik Üniversitesi',
        faculty: 'Bilgisayar ve Bilişim Fakültesi',
        department: 'Bilgisayar Mühendisliği',
        startYear: 2012,
        endYear: 2016,
        isOngoing: false,
        thesisTitle: 'Büyük Dil Modelleri ve Dağıtık Sistemlerde Derin Pekiştirmeli Öğrenme Optimizasyonları',
        thesisAdvisor: 'Prof. Dr. Mehmet Özgür'
      },
      {
        id: 'edu-2',
        degree: 'Yüksek Lisans',
        university: 'Boğaziçi Üniversitesi',
        faculty: 'Mühendislik Fakültesi',
        department: 'Bilgisayar Mühendisliği',
        startYear: 2009,
        endYear: 2012,
        isOngoing: false,
        thesisTitle: 'Semantik Web Mimarilerinde Dağıtık Sorgu Yürütme Motoru Tasarımı',
        thesisAdvisor: 'Doç. Dr. Ayşe Yılmaz'
      },
      {
        id: 'edu-3',
        degree: 'Lisans',
        university: 'Orta Doğu Teknik Üniversitesi',
        faculty: 'Mühendislik Fakültesi',
        department: 'Bilgisayar Mühendisliği',
        startYear: 2004,
        endYear: 2009,
        isOngoing: false
      }
    ],
    articles: [
      {
        id: 'art-1',
        title: 'Scalable Microservices Architecture for High-Throughput Academic Information Systems',
        journal: 'IEEE Transactions on Services Computing',
        year: 2024,
        volumeIssue: 'Vol. 17, No. 3',
        pages: '1120-1135',
        indexType: 'SCI-Expanded',
        doi: '10.1109/TSC.2024.3129841',
        url: 'https://doi.org/10.1109/TSC.2024.3129841',
        coAuthors: 'A. Kaya, S. Demir, M. Kara'
      },
      {
        id: 'art-2',
        title: 'Deep Residual Learning for Automated Code Vulnerability Identification in Multi-Tenant Clouds',
        journal: 'Computers & Security (Elsevier)',
        year: 2023,
        volumeIssue: 'Vol. 129',
        pages: '103-118',
        indexType: 'SCI-Expanded',
        doi: '10.1016/j.cose.2023.103215',
        url: 'https://doi.org/10.1016/j.cose.2023.103215',
        coAuthors: 'A. Kaya, E. Can'
      }
    ],
    books: [
      {
        id: 'bk-1',
        title: 'Modern Bulut Bilişim ve Mikroservis Mimarileri: Tasarım Desenlerinden Üretime',
        type: 'Telif Kitap',
        publisher: 'Seçkin Akademik Yayıncılık',
        city: 'Ankara',
        year: 2023,
        isbn: '978-975-02-8492-1',
        url: 'https://www.seckin.com.tr/kitap/modern-bulut-bilisim'
      },
      {
        id: 'bk-2',
        title: 'Handbook of Cloud Security & Federated Trust: Chapter 8: Zero-Trust Identity Protocols',
        type: 'Kitap Bölümü',
        publisher: 'Springer Nature',
        city: 'Berlin / New York',
        year: 2022,
        isbn: '978-3-030-94812-4',
        url: 'https://link.springer.com/chapter/10.1007/978-3-030-94812-4_8',
        chapterTitle: 'Zero-Trust Identity Protocols in Hybrid Campus Networks'
      }
    ],
    conferences: [
      {
        id: 'conf-1',
        title: 'Next-Generation Reactive State Management in Complex Student Affairs Platforms',
        conferenceName: 'International Conference on Software Engineering & Architecture (ICSEA 2025)',
        cityCountry: 'Viyana, Avusturya',
        date: '14-16 Mayıs 2025',
        presentationType: 'Sözlü Sunum',
        publishedType: 'Tam Metin',
        url: 'https://icsea-conf.org/proceedings/2025/kaya'
      }
    ],
    projects: [
      {
        id: 'prj-1',
        title: 'Yüksek Başarımlı Üniversite Bilgi Sistemleri İçin Dağıtık ve Güvenli Veri Tabanı Katmanı Geliştirilmesi',
        agency: 'TÜBİTAK 1001',
        projectRole: 'Yürütücü',
        budget: '2.450.000',
        currency: 'TRY',
        startDate: '2023-01-01',
        endDate: '2025-12-31',
        status: 'Devam Ediyor',
        url: 'https://tubitak.gov.tr/proje/1001-23k491'
      }
    ],
    courses: [
      {
        id: 'crs-1',
        courseCode: 'BLG-411',
        courseName: 'Dağıtık Sistemler ve Bulut Mimarileri',
        term: 'Güz',
        level: 'Lisans',
        hoursPerWeek: 4,
        language: 'Türkçe'
      },
      {
        id: 'crs-2',
        courseCode: 'BLG-530',
        courseName: 'İleri Yazılım Mühendisliği Desenleri',
        term: 'Bahar',
        level: 'Yüksek Lisans',
        hoursPerWeek: 3,
        language: 'İngilizce'
      }
    ],
    advisings: [
      {
        id: 'adv-1',
        studentName: 'Burak Yalçın',
        level: 'Doktora',
        thesisTitle: 'Federated Learning Frameworks for Multi-Campus IoT Sensors',
        year: 2025,
        status: 'Devam Ediyor'
      },
      {
        id: 'adv-2',
        studentName: 'Gamze Şahin',
        level: 'Yüksek Lisans',
        thesisTitle: 'Mikroservis Dağıtımlarında Otomatik Canary Deployment Optimizasyonu',
        year: 2023,
        status: 'Tamamlandı'
      }
    ]
  }
};

export const getAcademicProfile = async (instructorId: string): Promise<InstructorAcademicProfile> => {
  const raw = localStorage.getItem(`${STORAGE_KEY_PREFIX}${instructorId}`);
  if (raw) {
    try {
      return JSON.parse(raw);
    } catch (e) {
      console.error('Failed to parse academic profile from storage', e);
    }
  }

  if (initialSampleProfiles[instructorId]) {
    localStorage.setItem(`${STORAGE_KEY_PREFIX}${instructorId}`, JSON.stringify(initialSampleProfiles[instructorId]));
    return initialSampleProfiles[instructorId];
  }

  // Varsayılan boş profil
  const emptyProfile: InstructorAcademicProfile = {
    instructorId,
    educations: [],
    articles: [],
    books: [],
    conferences: [],
    projects: [],
    courses: [],
    advisings: [],
    openAccess: {
      orcidId: '',
      orcidUrl: '',
      scholarId: '',
      scholarUrl: '',
      yoksisId: '',
      scopusId: '',
      researchgateUrl: '',
      personalWebsite: ''
    }
  };

  localStorage.setItem(`${STORAGE_KEY_PREFIX}${instructorId}`, JSON.stringify(emptyProfile));
  return emptyProfile;
};

export const saveAcademicProfile = async (profile: InstructorAcademicProfile): Promise<InstructorAcademicProfile> => {
  localStorage.setItem(`${STORAGE_KEY_PREFIX}${profile.instructorId}`, JSON.stringify(profile));
  return profile;
};
