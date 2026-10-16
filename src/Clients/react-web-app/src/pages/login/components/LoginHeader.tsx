import React from 'react';
import { Button, Dropdown, Space, Badge } from 'antd';
import {
  BankOutlined,
  GlobalOutlined,
  InfoCircleOutlined,
  BellOutlined,
} from '@ant-design/icons';
import { useLanguage, LANGUAGES } from '../../../context/LanguageContext';
import type { SupportedLanguage } from '../../../context/LanguageContext';

interface LoginHeaderProps {
  announcementsCount: number;
  onOpenAbout: () => void;
  onOpenAnnouncements: () => void;
}

export const LoginHeader: React.FC<LoginHeaderProps> = ({
  announcementsCount,
  onOpenAbout,
  onOpenAnnouncements,
}) => {
  const { language, setLanguage, t } = useLanguage();

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

  const currentLangInfo = LANGUAGES.find((l) => l.code === language) || LANGUAGES[0];

  return (
    <header
      style={{
        height: 64,
        minHeight: 64,
        padding: '0 32px',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        backgroundColor: 'rgba(9, 25, 48, 0.88)',
        backdropFilter: 'blur(12px)',
        borderBottom: '1px solid rgba(255, 255, 255, 0.12)',
        zIndex: 10,
        flexShrink: 0,
      }}
    >
      {/* Sol Alan: Logo ve Üniversite Bilgisi (Sabit ve Taşmaz) */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 14, flexShrink: 0 }}>
        <div
          style={{
            width: 40,
            height: 40,
            borderRadius: 10,
            background: 'linear-gradient(135deg, #1890ff 0%, #0958d9 100%)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            color: '#ffffff',
            fontSize: 20,
            boxShadow: '0 4px 12px rgba(24, 144, 255, 0.35)',
            flexShrink: 0,
          }}
        >
          <BankOutlined />
        </div>
        <div style={{ display: 'flex', flexDirection: 'column' }}>
          <div
            style={{
              color: '#ffffff',
              fontWeight: 700,
              fontSize: 14,
              letterSpacing: 0.3,
              whiteSpace: 'nowrap',
            }}
          >
            {t('university_name')}
          </div>
          <div
            style={{
              color: '#93c5fd',
              fontSize: 11,
              fontWeight: 500,
              whiteSpace: 'nowrap',
            }}
          >
            {t('system_title')}
          </div>
        </div>
      </div>

      {/* Sağ Alan: Butonlar ve Sabit Genişlikli Dil Seçici */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, flexShrink: 0 }}>
        <Button
          type="text"
          icon={<InfoCircleOutlined />}
          style={{ color: '#e2e8f0', fontWeight: 500, minWidth: 90, flexShrink: 0 }}
          onClick={onOpenAbout}
        >
          {t('about_us')}
        </Button>

        <Badge count={announcementsCount} size="small" offset={[-2, 4]}>
          <Button
            type="text"
            icon={<BellOutlined />}
            style={{ color: '#e2e8f0', fontWeight: 500, minWidth: 90, flexShrink: 0 }}
            onClick={onOpenAnnouncements}
          >
            {t('announcements')}
          </Button>
        </Badge>

        {/* Dil Seçici (SABİT 135px GENİŞLİK - Asla Layout Shift Yapmaz) */}
        <Dropdown menu={{ items: languageMenuItems }} trigger={['click']}>
          <Button
            type="text"
            size="middle"
            style={{
              width: 135,
              minWidth: 135,
              maxWidth: 135,
              color: '#ffffff',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              gap: 6,
              backgroundColor: 'rgba(255,255,255,0.08)',
              borderRadius: 6,
              flexShrink: 0,
            }}
          >
            <GlobalOutlined />
            <span style={{ whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
              {currentLangInfo.flag} {currentLangInfo.name}
            </span>
          </Button>
        </Dropdown>
      </div>
    </header>
  );
};
