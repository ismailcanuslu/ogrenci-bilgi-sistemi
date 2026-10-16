export interface PortalAnnouncement {
  id: number;
  title: string;
  date: string;
  content: string;
  priority: 'High' | 'Medium' | 'Normal';
  category: string;
}

export interface PortalSettings {
  backgroundMode: 'slideshow' | 'single' | 'empty';
  slideIntervalSeconds: number;
  coverImages: string[];
  aboutTitle: string;
  aboutContent: string;
  announcements: PortalAnnouncement[];
}

export const defaultPortalSettings: PortalSettings = {
  backgroundMode: 'slideshow',
  slideIntervalSeconds: 5,
  coverImages: ['/cover-1.jpg', '/cover-2.jpg'],
  aboutTitle: 'T.C. Anadolu Devlet Üniversitesi - Öğrenci Bilgi Sistemi (OBS)',
  aboutContent: 'Üniversitemiz Öğrenci Bilgi Sistemi (OBS); çağdaş yükseköğretim standartlarına uygun, güvenli, hızlı ve kullanıcı odaklı bir dijital kampüs deneyimi sunmaktadır. Sistemimiz YÖKAK kurumsal akreditasyon süreçleri, Bologna süreci uyumu, dijital transkript karekod doğrulama ve engelsiz erişim standartlarıyla tam entegredir.',
  announcements: [
    {
      id: 1,
      title: '2026-2027 Güz Yarıyılı Ders Kayıt ve Danışman Onayları',
      date: '15 Ekim 2026',
      content: 'Ders kayıt işlemleri 15-20 Ekim 2026 tarihleri arasında portal üzerinden gerçekleştirilecektir. Danışman onaylarının son günü 21 Ekim 2026 saat 17:00\'dir.',
      priority: 'High',
      category: 'Akademik Takvim'
    },
    {
      id: 2,
      title: 'Çift Anadal (ÇAP) ve Yandal Başvuruları Başladı',
      date: '12 Ekim 2026',
      content: 'Genel Not Ortalaması (GNO) 3.00 ve üzeri olan öğrencilerimiz Öğrenci Başvuruları sekmesinden çevrimiçi başvuru yapabilirler.',
      priority: 'Medium',
      category: 'Başvuru'
    },
    {
      id: 3,
      title: 'obsKimlik Güvenli Dijital Kampüs Kartı Geçişi',
      date: '10 Ekim 2026',
      content: 'Tüm öğrencilerimiz ve akademik personelimiz yemekhane, kütüphane ve laboratuvar geçişlerinde yeni obsKimlik dijital karekodlarını kullanabilir.',
      priority: 'Normal',
      category: 'Genel'
    }
  ]
};

export const portalApi = {
  async getSettings(): Promise<PortalSettings> {
    try {
      const res = await fetch('/api/v1/portal/settings');
      if (res.ok) {
        const data = await res.json();
        return {
          ...defaultPortalSettings,
          ...data,
          coverImages: data.coverImages && data.coverImages.length > 0 ? data.coverImages : defaultPortalSettings.coverImages,
          announcements: data.announcements && data.announcements.length > 0 ? data.announcements : defaultPortalSettings.announcements
        };
      }
    } catch {
      // Offline fallback
    }
    const local = localStorage.getItem('obs_portal_settings');
    if (local) {
      try {
        return JSON.parse(local);
      } catch {
        // fallback
      }
    }
    return defaultPortalSettings;
  },

  async updateSettings(settings: PortalSettings): Promise<boolean> {
    localStorage.setItem('obs_portal_settings', JSON.stringify(settings));
    try {
      const res = await fetch('/api/v1/portal/settings', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(settings)
      });
      return res.ok;
    } catch {
      return true; // LocalStorage'da tutuldu
    }
  }
};
