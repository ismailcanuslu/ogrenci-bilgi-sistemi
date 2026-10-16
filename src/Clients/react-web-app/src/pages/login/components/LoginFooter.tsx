import React from 'react';
import { Button, Typography } from 'antd';
import {
  PhoneOutlined,
  LinkOutlined,
  CustomerServiceOutlined,
  BarChartOutlined,
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useLanguage } from '../../../context/LanguageContext';

const { Text } = Typography;

interface LoginFooterProps {
  onOpenKvkk: () => void;
  onOpenPhoneDirectory: () => void;
  onOpenSupport: () => void;
}

export const LoginFooter: React.FC<LoginFooterProps> = ({
  onOpenKvkk,
  onOpenPhoneDirectory,
  onOpenSupport,
}) => {
  const { t } = useLanguage();
  const navigate = useNavigate();

  return (
    <footer
      style={{
        height: 52,
        minHeight: 52,
        padding: '0 32px',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        flexWrap: 'nowrap',
        backgroundColor: 'rgba(9, 25, 48, 0.92)',
        backdropFilter: 'blur(12px)',
        borderTop: '1px solid rgba(255, 255, 255, 0.12)',
        color: '#cbd5e1',
        fontSize: 12,
        zIndex: 10,
        flexShrink: 0,
      }}
    >
      {/* Sol Alt: Telif ve KVKK */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 12, flexShrink: 0 }}>
        <Text style={{ color: '#cbd5e1', whiteSpace: 'nowrap' }}>{t('footer_copyright')}</Text>
        <span style={{ color: '#475569' }}>•</span>
        <Button
          type="link"
          size="small"
          style={{ color: '#93c5fd', padding: 0, fontSize: 12, whiteSpace: 'nowrap' }}
          onClick={onOpenKvkk}
        >
          {t('footer_kvkk')}
        </Button>
      </div>

      {/* Sağ Alt: Telefon Rehberi, Web Sayfası, OBS Destek, Kurumsal Değerlendirme */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 16, flexShrink: 0 }}>
        <Button
          type="text"
          icon={<PhoneOutlined />}
          size="small"
          style={{ color: '#e2e8f0', fontSize: 12, whiteSpace: 'nowrap', flexShrink: 0 }}
          onClick={onOpenPhoneDirectory}
        >
          {t('footer_phone_directory')}
        </Button>

        <Button
          type="text"
          icon={<LinkOutlined />}
          size="small"
          style={{ color: '#e2e8f0', fontSize: 12, whiteSpace: 'nowrap', flexShrink: 0 }}
          onClick={() => window.open('https://www.yok.gov.tr', '_blank')}
        >
          {t('footer_website')}
        </Button>

        <Button
          type="text"
          icon={<CustomerServiceOutlined />}
          size="small"
          style={{ color: '#e2e8f0', fontSize: 12, whiteSpace: 'nowrap', flexShrink: 0 }}
          onClick={onOpenSupport}
        >
          {t('footer_support')}
        </Button>

        <Button
          type="text"
          icon={<BarChartOutlined />}
          size="small"
          style={{ color: '#38bdf8', fontWeight: 600, fontSize: 12, whiteSpace: 'nowrap', flexShrink: 0 }}
          onClick={() => navigate('/institutional-evaluation')}
        >
          {t('footer_evaluation')}
        </Button>
      </div>
    </footer>
  );
};
