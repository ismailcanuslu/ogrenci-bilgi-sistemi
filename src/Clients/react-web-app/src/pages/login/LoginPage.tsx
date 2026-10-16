import React, { useState, useEffect } from 'react';
import { Form, message } from 'antd';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { useLanguage } from '../../context/LanguageContext';
import { portalApi, defaultPortalSettings } from '../../api/portalSettings';
import type { PortalSettings } from '../../api/portalSettings';

import { LoginBackground } from './components/LoginBackground';
import { LoginHeader } from './components/LoginHeader';
import { LoginCard } from './components/LoginCard';
import { LoginProgressBar } from './components/LoginProgressBar';
import { LoginFooter } from './components/LoginFooter';
import { LoginModals } from './components/LoginModals';

export const LoginPage: React.FC = () => {
  const { login } = useAuth();
  const { t } = useLanguage();
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [submitting, setSubmitting] = useState<boolean>(false);

  // Portal Ayarları & Slayt State
  const [settings, setSettings] = useState<PortalSettings>(defaultPortalSettings);
  const [currentSlideIndex, setCurrentSlideIndex] = useState<number>(0);
  const [slideProgress, setSlideProgress] = useState<number>(0);

  // Modalların Görünürlük Durumları
  const [aboutModalVisible, setAboutModalVisible] = useState<boolean>(false);
  const [announcementsModalVisible, setAnnouncementsModalVisible] = useState<boolean>(false);
  const [phoneDirectoryModalVisible, setPhoneDirectoryModalVisible] = useState<boolean>(false);
  const [supportModalVisible, setSupportModalVisible] = useState<boolean>(false);
  const [kvkkModalVisible, setKvkkModalVisible] = useState<boolean>(false);

  // Portal ayarlarını yükle
  useEffect(() => {
    portalApi.getSettings().then((res) => {
      setSettings(res);
    });
  }, []);

  // Soldan Sağa Dolan Bar ve Slayt Değişim Zamanlayıcısı
  useEffect(() => {
    const images = settings.coverImages || [];
    if (settings.backgroundMode !== 'slideshow' || images.length <= 1) {
      setSlideProgress(0);
      return;
    }

    const durationMs = Math.max(2, settings.slideIntervalSeconds || 5) * 1000;
    const intervalMs = 50;
    let elapsedMs = 0;

    const timer = setInterval(() => {
      elapsedMs += intervalMs;
      const progressPercent = (elapsedMs / durationMs) * 100;

      if (progressPercent >= 100) {
        elapsedMs = 0;
        setSlideProgress(0);
        setCurrentSlideIndex((prevIdx) => (prevIdx + 1) % images.length);
      } else {
        setSlideProgress(progressPercent);
      }
    }, intervalMs);

    return () => clearInterval(timer);
  }, [settings.backgroundMode, settings.coverImages, settings.slideIntervalSeconds]);

  const handleSubmit = async (values: { email: string; password: string }) => {
    setSubmitting(true);
    try {
      const assignedRole = await login({
        email: values.email,
        password: values.password,
      });

      message.success(t('login_success'));

      // Role uygun dashboard'a yönlendir
      if (assignedRole === 'ACADEMIC') {
        navigate('/academic/portal');
      } else if (assignedRole === 'STAFF' || assignedRole === 'ADMIN') {
        navigate('/affairs/console');
      } else {
        navigate('/dashboard');
      }
    } catch (err: unknown) {
      const errorMsg =
        (err as { response?: { data?: { detail?: string; error?: string } } })?.response?.data?.detail ||
        (err as { response?: { data?: { detail?: string; error?: string } } })?.response?.data?.error ||
        t('login_failed');
      message.error(errorMsg);
    } finally {
      setSubmitting(false);
    }
  };

  const handleQuickFill = (email: string) => {
    form.setFieldsValue({
      email,
      password: 'Obs123456!*',
    });
  };

  const isSlideshowActive =
    settings.backgroundMode === 'slideshow' &&
    !!settings.coverImages &&
    settings.coverImages.length > 1;

  return (
    <div
      style={{
        minHeight: '100vh',
        width: '100vw',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'space-between',
        position: 'relative',
        overflowX: 'hidden',
      }}
    >
      {/* 1. Arka Plan Katmanı (Sinematik Cross-Fade & Ken Burns Efekti) */}
      <LoginBackground
        images={settings.coverImages || []}
        currentIndex={currentSlideIndex}
        mode={settings.backgroundMode}
      />

      {/* 2. Üst Bar (Header) */}
      <LoginHeader
        announcementsCount={settings.announcements?.length || 0}
        onOpenAbout={() => setAboutModalVisible(true)}
        onOpenAnnouncements={() => setAnnouncementsModalVisible(true)}
      />

      {/* 3. Merkez Giriş Kartı */}
      <main
        style={{
          flex: 1,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          padding: '24px 16px',
          position: 'relative',
          zIndex: 10,
        }}
      >
        <LoginCard
          form={form}
          submitting={submitting}
          onSubmit={handleSubmit}
          onQuickFill={handleQuickFill}
        />
      </main>

      {/* 4. Alt Barın Hemen Üstünde Soldan Sağa Dolan Çubuk (Progress Bar) */}
      <LoginProgressBar
        progress={slideProgress}
        isVisible={isSlideshowActive}
      />

      {/* 5. Alt Bar (Footer) */}
      <LoginFooter
        onOpenKvkk={() => setKvkkModalVisible(true)}
        onOpenPhoneDirectory={() => setPhoneDirectoryModalVisible(true)}
        onOpenSupport={() => setSupportModalVisible(true)}
      />

      {/* 6. Tüm Modallar */}
      <LoginModals
        settings={settings}
        aboutVisible={aboutModalVisible}
        announcementsVisible={announcementsModalVisible}
        phoneDirectoryVisible={phoneDirectoryModalVisible}
        supportVisible={supportModalVisible}
        kvkkVisible={kvkkModalVisible}
        onCloseAbout={() => setAboutModalVisible(false)}
        onCloseAnnouncements={() => setAnnouncementsModalVisible(false)}
        onClosePhoneDirectory={() => setPhoneDirectoryModalVisible(false)}
        onCloseSupport={() => setSupportModalVisible(false)}
        onCloseKvkk={() => setKvkkModalVisible(false)}
      />
    </div>
  );
};
