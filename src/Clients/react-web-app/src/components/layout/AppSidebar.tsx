import React, { useEffect, useState } from 'react';
import { Layout, Menu, Typography, Tag } from 'antd';
import {
  DashboardOutlined,
  BookOutlined,
  FormOutlined,
  DollarOutlined,
  HeartOutlined,
  BarChartOutlined,
  PartitionOutlined,
  BankOutlined,
  SolutionOutlined,
  ApartmentOutlined,
  SettingOutlined,
  PictureOutlined,
  ControlOutlined,
  UserOutlined,
  TeamOutlined,
  IdcardOutlined,
  GlobalOutlined,
} from '@ant-design/icons';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { institutionApi } from '../../api/institutionSettings';
import type { InstitutionSettings } from '../../api/institutionSettings';

const { Sider } = Layout;
const { Title, Text } = Typography;

interface AppSidebarProps {
  collapsed: boolean;
  onCollapse: (val: boolean) => void;
}

export const AppSidebar: React.FC<AppSidebarProps> = ({ collapsed, onCollapse }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, activeRole } = useAuth();
  const [instSettings, setInstSettings] = useState<InstitutionSettings | null>(null);

  const role = activeRole || user?.activeRole || user?.role || 'STUDENT';

  // Kurumsal ayarları yükle ve dinle
  useEffect(() => {
    const fetchSettings = () => {
      institutionApi.getSettings().then((res) => setInstSettings(res));
    };
    fetchSettings();
    window.addEventListener('institution_settings_updated', fetchSettings);
    return () => window.removeEventListener('institution_settings_updated', fetchSettings);
  }, []);

  // Role Özel Menü Ağacı
  let menuItems: any[] = [];

  if (role === 'STUDENT') {
    menuItems = [
      {
        key: '/dashboard',
        icon: <DashboardOutlined />,
        label: 'Genel Bilgiler & Profil',
      },
      {
        key: 'course-menu',
        icon: <BookOutlined />,
        label: 'Ders & Dönem İşlemleri',
        children: [
          { key: '/student/course-registration', label: 'Ders Kayıt / Ekle-Bırak' },
          { key: '/student/transcript', label: 'Not Dökümü (Transkript)' },
        ],
      },
      {
        key: '/student/applications',
        icon: <FormOutlined />,
        label: 'Başvuru & Belge Masası',
      },
      {
        key: '/student/details',
        icon: <DollarOutlined />,
        label: 'Harç & Askerlik & IBAN',
      },
      {
        key: '/education-catalog',
        icon: <PartitionOutlined style={{ color: '#10b981' }} />,
        label: 'Eğitim Kataloğu & Müfredat',
      },
      {
        key: '/social-contribution',
        icon: <HeartOutlined style={{ color: '#ec4899' }} />,
        label: 'Toplumsal Katkı Portalı',
      },
      {
        key: '/institutional-evaluation',
        icon: <BarChartOutlined style={{ color: '#0284c7' }} />,
        label: 'Kurumsal Değerlendirme',
      },
    ];
  } else if (role === 'ACADEMIC') {
    menuItems = [
      {
        key: '/academic/portal',
        icon: <DashboardOutlined />,
        label: 'Akademik Panel & Not Girişi',
      },
      {
        key: '/department/management',
        icon: <ApartmentOutlined />,
        label: 'Bölüm Dersleri & Açımlar',
      },
      {
        key: '/education-catalog',
        icon: <PartitionOutlined style={{ color: '#10b981' }} />,
        label: 'Eğitim Kataloğu & Müfredat',
      },
      {
        key: '/institutional-evaluation',
        icon: <BarChartOutlined style={{ color: '#0284c7' }} />,
        label: 'Kurumsal Değerlendirme',
      },
      {
        key: '/social-contribution',
        icon: <HeartOutlined style={{ color: '#ec4899' }} />,
        label: 'Toplumsal Katkı Faaliyetleri',
      },
    ];
  } else {
    // STAFF veya ADMIN (Öğrenci İşleri / Yönetici)
    menuItems = [
      {
        key: '/affairs/console',
        icon: <SolutionOutlined />,
        label: 'Öğrenci İşleri Masası',
      },
      {
        key: 'user-management',
        icon: <TeamOutlined />,
        label: 'Kullanıcı & Nüfus Masası',
        children: [
          { key: '/users/students', icon: <UserOutlined />, label: 'Asil Öğrenci Masası' },
          { key: '/users/instructors', icon: <SolutionOutlined />, label: 'Eğitmen & Akademisyenler' },
          { key: '/users/guest-students', icon: <GlobalOutlined />, label: 'Misafir & Değişim Öğrencileri' },
        ],
      },
      {
        key: 'research-management',
        icon: <BookOutlined style={{ color: '#0284c7' }} />,
        label: 'Öğrenim, Yayın & Araştırma',
        children: [
          { key: '/affairs/instructors/profile', icon: <FormOutlined />, label: 'Akademik Profil & Yayın Masası' },
        ],
      },
      {
        key: 'admin-management',
        icon: <BankOutlined style={{ color: '#d97706' }} />,
        label: 'İdari Teşkilat & Personel',
        children: [
          { key: '/admin/staff', icon: <IdcardOutlined />, label: 'İdari Personel Kadrosu' },
          { key: '/admin/units', icon: <ApartmentOutlined />, label: 'İdari Birimler Masası' },
          { key: '/admin/staff-titles', icon: <SolutionOutlined />, label: 'İdari Kadro Unvanları' },
          { key: '/admin/roles', icon: <ControlOutlined />, label: 'Yetki & Sistem Rolleri' },
        ],
      },
      {
        key: 'academic-structure',
        icon: <ApartmentOutlined />,
        label: 'Akademik Yapı & Müfredat',
        children: [
          { key: '/affairs/faculties', icon: <BankOutlined />, label: 'Fakülte Yönetimi' },
          { key: '/affairs/departments', icon: <ApartmentOutlined />, label: 'Bölüm & AKTS Sınırları' },
          { key: '/affairs/courses', icon: <BookOutlined />, label: 'Bölüm Ders Kataloğu' },
          { key: '/department/management', icon: <ControlOutlined />, label: 'Dönemlik Şube Açımları' },
        ],
      },
      {
        key: 'general-settings',
        icon: <SettingOutlined />,
        label: 'Genel Ayarlar',
        children: [
          { key: '/settings/institution', icon: <BankOutlined />, label: 'Üniversite & İletişim Bilgileri' },
          { key: '/settings/login-page', icon: <PictureOutlined />, label: 'Giriş Sayfasını Özelleştirme' },
        ],
      },
      {
        key: '/education-catalog',
        icon: <PartitionOutlined style={{ color: '#10b981' }} />,
        label: 'Eğitim Kataloğu & Müfredat',
      },
      {
        key: '/institutional-evaluation',
        icon: <BarChartOutlined style={{ color: '#0284c7' }} />,
        label: 'Kurumsal Kalite & Akreditasyon',
      },
      {
        key: '/social-contribution',
        icon: <HeartOutlined style={{ color: '#ec4899' }} />,
        label: 'Toplumsal Katkı Portalı',
      },
    ];
  }

  const getRoleBadge = () => {
    switch (role) {
      case 'STUDENT':
        return <Tag color="blue">Öğrenci</Tag>;
      case 'ACADEMIC':
        return <Tag color="geekblue">Öğretim Üyesi</Tag>;
      case 'STAFF':
        return <Tag color="purple">Öğrenci İşleri</Tag>;
      case 'ADMIN':
        return <Tag color="gold">Sistem Yöneticisi</Tag>;
    }
  };

  const displayName =
    instSettings?.universityShortName ||
    instSettings?.universityName ||
    'ÜNİVERSİTE OBS';

  return (
    <Sider
      collapsible
      collapsed={collapsed}
      onCollapse={onCollapse}
      width={270}
      style={{
        backgroundColor: '#ffffff',
        borderRight: '1px solid #e2e8f0',
        minHeight: '100vh',
        boxShadow: '1px 0 3px rgba(0,0,0,0.02)',
      }}
    >
      {/* Logo & Kurumsal Başlık */}
      <div
        style={{
          height: 64,
          display: 'flex',
          alignItems: 'center',
          padding: '0 18px',
          gap: 12,
          borderBottom: '1px solid #e2e8f0',
          cursor: 'pointer',
        }}
        onClick={() => {
          if (role === 'STUDENT') navigate('/dashboard');
          else if (role === 'ACADEMIC') navigate('/academic/portal');
          else navigate('/affairs/console');
        }}
      >
        <div
          style={{
            width: 38,
            height: 38,
            borderRadius: 8,
            backgroundColor: '#0f4c81',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            color: '#ffffff',
            flexShrink: 0,
            overflow: 'hidden',
          }}
        >
          {instSettings?.logoUrl ? (
            <img
              src={instSettings.logoUrl}
              alt="Logo"
              style={{ width: '100%', height: '100%', objectFit: 'contain' }}
              onError={(e) => {
                e.currentTarget.style.display = 'none';
              }}
            />
          ) : (
            <BankOutlined style={{ fontSize: 20 }} />
          )}
        </div>
        {!collapsed && (
          <div style={{ overflow: 'hidden' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              <Title
                level={5}
                style={{
                  margin: 0,
                  color: '#0f4c81',
                  fontWeight: 700,
                  letterSpacing: -0.3,
                  whiteSpace: 'nowrap',
                  textOverflow: 'ellipsis',
                  overflow: 'hidden',
                  maxWidth: 130,
                }}
              >
                {displayName}
              </Title>
              {getRoleBadge()}
            </div>
            <Text
              style={{
                fontSize: 11,
                color: '#64748b',
                display: 'block',
                whiteSpace: 'nowrap',
              }}
            >
              Bilgi Yönetim Sistemi
            </Text>
          </div>
        )}
      </div>

      {/* Menü Ağacı */}
      <div
        style={{
          padding: '12px 0',
          overflowY: 'auto',
          maxHeight: 'calc(100vh - 120px)',
        }}
      >
        <Menu
          mode="inline"
          selectedKeys={[location.pathname]}
          defaultOpenKeys={['user-management', 'academic-structure', 'general-settings', 'course-menu']}
          items={menuItems}
          onClick={({ key }) => {
            if (key.startsWith('/')) {
              navigate(key);
            }
          }}
          style={{ borderRight: 0 }}
        />
      </div>
    </Sider>
  );
};
