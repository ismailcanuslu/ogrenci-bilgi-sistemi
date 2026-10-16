import React from 'react';
import { Layout, Dropdown, Space, Avatar, Badge, Button, Select, Tag, Popover, List, Typography } from 'antd';
import {
  BellOutlined,
  GlobalOutlined,
  LogoutOutlined,
  UserOutlined,
  SwapOutlined,
  FileTextOutlined,
  TeamOutlined,
} from '@ant-design/icons';
import { useAuth } from '../../context/AuthContext';
import { useLanguage, LANGUAGES } from '../../context/LanguageContext';
import type { SupportedLanguage } from '../../context/LanguageContext';
import { useNavigate } from 'react-router-dom';

const { Header } = Layout;
const { Text } = Typography;

interface HeaderBarProps {
  collapsed: boolean;
  onToggleCollapse: () => void;
}

export const HeaderBar: React.FC<HeaderBarProps> = () => {
  const { user, logout, activeRole, switchRole, activeProgram, setActiveProgram, switchUser } = useAuth();
  const { language, setLanguage } = useLanguage();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const notifications = [
    { id: 1, title: 'BLG411 Bitirme Projesi Rapor Teslimi', time: 'Bugün 17:00', type: 'urgent' },
    { id: 2, title: '2026-2027 Bahar Dönemi Ön Kayıt Başladı', time: 'Dün', type: 'info' },
    { id: 3, title: 'Toplumsal Katkı Fidan Dikimi Onaylandı', time: '2 gün önce', type: 'success' },
  ];

  const notificationContent = (
    <div style={{ width: 320 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', paddingBottom: 8, borderBottom: '1px solid #f1f5f9' }}>
        <Text strong>Bildirimler & Duyurular</Text>
        <a href="#all" style={{ fontSize: 12 }}>Tümünü Gör</a>
      </div>
      <List
        size="small"
        dataSource={notifications}
        renderItem={(item) => (
          <List.Item style={{ padding: '8px 0' }}>
            <List.Item.Meta
              avatar={<Badge status={item.type === 'urgent' ? 'error' : item.type === 'success' ? 'success' : 'processing'} />}
              title={<span style={{ fontSize: 13, fontWeight: 500 }}>{item.title}</span>}
              description={<span style={{ fontSize: 11, color: '#94a3b8' }}>{item.time}</span>}
            />
          </List.Item>
        )}
      />
    </div>
  );

  const roleSwitcherItems = [
    {
      key: 'student',
      label: '👨‍🎓 Öğrenci: İsmailcan Uslu (ÇAP)',
      onClick: () => {
        switchUser('ogrenci@obs.edu.tr');
        navigate('/dashboard');
      },
    },
    {
      key: 'academic',
      label: '👨‍🏫 Akademisyen: Prof. Dr. Ahmet Bilgehan',
      onClick: () => {
        switchUser('akademik@obs.edu.tr');
        navigate('/academic/portal');
      },
    },
    {
      key: 'staff',
      label: '🏛️ Öğrenci İşleri: Ayşe Yılmaz',
      onClick: () => {
        switchUser('idari@obs.edu.tr');
        navigate('/affairs/console');
      },
    },
  ];

  const userMenuItems = [
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: 'Profil Bilgileri',
      onClick: () => navigate(user?.role === 'STUDENT' ? '/dashboard' : user?.role === 'ACADEMIC' ? '/academic/portal' : '/affairs/console'),
    },
    {
      type: 'divider' as const,
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Güvenli Çıkış',
      danger: true,
      onClick: handleLogout,
    },
  ];

  const languageMenuItems = LANGUAGES.map((lang) => ({
    key: lang.code,
    label: (
      <Space>
        <span>{lang.flag}</span>
        <span>{lang.name}</span>
      </Space>
    ),
    onClick: () => setLanguage(lang.code as SupportedLanguage),
  }));

  const currentLang = LANGUAGES.find((l) => l.code === language) || LANGUAGES[0];

  return (
    <Header
      style={{
        backgroundColor: '#ffffff',
        borderBottom: '1px solid #e2e8f0',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        padding: '0 24px',
        position: 'sticky',
        top: 0,
        zIndex: 100,
      }}
    >
      {/* Sol: Program Seçici ve Dönem Rozeti */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
        {activeRole === 'STUDENT' ? (
          <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
            <SwapOutlined style={{ color: '#0f4c81', fontSize: 16 }} />
            <Text strong style={{ fontSize: 13, color: '#475569' }}>
              Program:
            </Text>
            <Select
              value={activeProgram}
              onChange={setActiveProgram}
              style={{ width: 320 }}
              options={(user?.availablePrograms || [user?.currentProgram || 'Lisans Programı']).map((prog) => ({
                value: prog,
                label: (
                  <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <span style={{ fontWeight: 500 }}>{prog}</span>
                    {prog.includes('ÇAP') && <Tag color="purple" style={{ margin: 0, fontSize: 10 }}>ÇAP</Tag>}
                    {prog.includes('Ana Dal') && <Tag color="blue" style={{ margin: 0, fontSize: 10 }}>Asil</Tag>}
                  </div>
                ),
              }))}
            />
          </div>
        ) : (
          <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
            <Text strong style={{ color: '#0f4c81', fontSize: 14 }}>
              {user?.faculty} | {user?.department}
            </Text>
          </div>
        )}

        <Tag color="geekblue" style={{ borderRadius: 6, padding: '2px 8px', fontSize: 12 }}>
          {user?.currentTerm || '2026-2027 Güz Dönemi'}
        </Tag>
      </div>

      {/* Sağ: Çoklu Rol Seçici, Bildirimler, Dil Seçici ve Kullanıcı Profili */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 14 }}>
        {/* Birden Fazla Rolü Olan Kullanıcılar İçin Rol/Dashboard Değiştirici */}
        {user?.roles && user.roles.length > 1 && (
          <div
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: 8,
              padding: '4px 10px',
              backgroundColor: '#eff6ff',
              borderRadius: 8,
              border: '1px solid #bfdbfe',
            }}
          >
            <TeamOutlined style={{ color: '#1d4ed8' }} />
            <span style={{ fontSize: 12, fontWeight: 600, color: '#1e3a8a' }}>Aktif Portal:</span>
            <Select
              size="small"
              value={activeRole}
              onChange={(newRole) => {
                switchRole(newRole as any);
                if (newRole === 'STUDENT') navigate('/dashboard');
                else if (newRole === 'ACADEMIC') navigate('/academic/portal');
                else if (newRole === 'STAFF') navigate('/affairs/console');
                else if (newRole === 'ADMIN') navigate('/affairs/console');
              }}
              style={{ width: 175 }}
              options={user.roles.map((r) => ({
                value: r,
                label:
                  r === 'STUDENT'
                    ? '🎓 Öğrenci Portalı'
                    : r === 'ACADEMIC'
                    ? '🏛️ Akademik Portal'
                    : r === 'STAFF'
                    ? '🏢 Öğrenci İşleri'
                    : '👑 Sistem Yönetimi',
              }))}
            />
          </div>
        )}

        {/* Hızlı Rol Değiştirici (Demo/Yetki Testi) */}
        {(!user?.roles || user.roles.length <= 1) && (
          <Dropdown menu={{ items: roleSwitcherItems }} trigger={['click']}>
            <Button
              size="small"
              icon={<TeamOutlined style={{ color: '#0f4c81' }} />}
              style={{ borderRadius: 6, borderColor: '#cbd5e1', fontWeight: 500 }}
            >
              Rol Değiştir
            </Button>
          </Dropdown>
        )}

        {activeRole === 'STUDENT' && (
          <Button
            type="default"
            size="small"
            icon={<FileTextOutlined style={{ color: '#0f4c81' }} />}
            style={{ borderRadius: 6, fontWeight: 500, borderColor: '#cbd5e1' }}
            onClick={() => navigate('/student/transcript')}
          >
            E-İmzalı Transkript
          </Button>
        )}

        <Popover content={notificationContent} trigger="click" placement="bottomRight">
          <Badge count={3} offset={[-2, 4]} size="small">
            <Button
              type="text"
              shape="circle"
              icon={<BellOutlined style={{ fontSize: 18, color: '#475569' }} />}
            />
          </Badge>
        </Popover>

        <Dropdown menu={{ items: languageMenuItems }} trigger={['click']}>
          <Button type="text" style={{ display: 'flex', alignItems: 'center', gap: 6, color: '#334155' }}>
            <GlobalOutlined />
            <span>{currentLang.flag}</span>
          </Button>
        </Dropdown>

        <div style={{ width: 1, height: 24, backgroundColor: '#e2e8f0' }} />

        {/* Profil Menüsü */}
        <Dropdown menu={{ items: userMenuItems }} trigger={['click']}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, cursor: 'pointer' }}>
            <Avatar
              src={user?.avatarUrl}
              icon={<UserOutlined />}
              style={{ backgroundColor: '#0f4c81', border: '1px solid #e2e8f0' }}
            />
            <div style={{ lineHeight: 1.2, textAlign: 'left' }}>
              <div style={{ fontWeight: 600, fontSize: 13, color: '#0f172a' }}>
                {user?.fullName || 'Kullanıcı'}
              </div>
              <div style={{ fontSize: 11, color: '#64748b' }}>
                {user?.studentNumber ? `Öğr. No: ${user.studentNumber}` : user?.title || 'OBS Kullanıcısı'}
              </div>
            </div>
          </div>
        </Dropdown>
      </div>
    </Header>
  );
};
