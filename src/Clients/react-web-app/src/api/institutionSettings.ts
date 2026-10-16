export interface InstitutionSettings {
  universityName: string;
  universityShortName: string;
  rectorName: string;
  logoUrl: string;
  phoneNumber: string;
  callCenterNumber: string;
  email: string;
  supportEmail: string;
  address: string;
  kepAddress: string;
  websiteUrl: string;
}

export const defaultInstitutionSettings: InstitutionSettings = {
  universityName: 'T.C. ANADOLU DEVLET ÜNİVERSİTESİ',
  universityShortName: 'ADÜ',
  rectorName: 'Prof. Dr. Mehmet Fatih Yıldız',
  logoUrl: '', // Boş olduğunda varsayılan BankOutlined ikonu kullanılır
  phoneNumber: '+90 (222) 335 05 80',
  callCenterNumber: '0850 300 06 27',
  email: 'genelsekreterlik@anadolu.edu.tr',
  supportEmail: 'obs.destek@anadolu.edu.tr',
  address: 'Üniversite Caddesi, Kampüs Yerleşkesi No: 1, 26470 Tepebaşı / Eskişehir',
  kepAddress: 'anadolutelekom@hs01.kep.tr',
  websiteUrl: 'https://www.anadolu.edu.tr',
};

const STORAGE_KEY = 'obs_institution_settings';

export const institutionApi = {
  getSettings: async (): Promise<InstitutionSettings> => {
    try {
      const stored = localStorage.getItem(STORAGE_KEY);
      if (stored) {
        return JSON.parse(stored);
      }
    } catch {
      // ignore
    }
    return defaultInstitutionSettings;
  },

  updateSettings: async (settings: InstitutionSettings): Promise<InstitutionSettings> => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(settings));
    window.dispatchEvent(new Event('institution_settings_updated'));
    return settings;
  },
};
