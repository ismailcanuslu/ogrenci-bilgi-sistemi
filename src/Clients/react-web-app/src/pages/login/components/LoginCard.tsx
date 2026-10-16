import React, { useState, useEffect } from 'react';
import { Form, Input, Button, Checkbox, Typography, Divider, Card, message } from 'antd';
import type { FormInstance } from 'antd';
import {
  UserOutlined,
  LockOutlined,
  SafetyCertificateOutlined,
  IdcardOutlined,
} from '@ant-design/icons';
import { useLanguage } from '../../../context/LanguageContext';
import { LoginForgotPasswordView } from './LoginForgotPasswordView';

const { Title, Text } = Typography;

interface LoginCardProps {
  form: FormInstance;
  submitting: boolean;
  onSubmit: (values: { email: string; password: string }) => void;
  onQuickFill: (email: string) => void;
}

export const LoginCard: React.FC<LoginCardProps> = ({
  form,
  submitting,
  onSubmit,
  onQuickFill,
}) => {
  const { t } = useLanguage();
  const [isForgotPassword, setIsForgotPassword] = useState<boolean>(false);
  const [windowWidth, setWindowWidth] = useState<number>(
    typeof window !== 'undefined' ? window.innerWidth : 1200
  );

  useEffect(() => {
    const handleResize = () => setWindowWidth(window.innerWidth);
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  const isMobile = windowWidth < 768;

  // Masaüstünde şifremi unuttum modunda kart sola ve sağa simetrik genişler
  const cardWidth = isMobile ? '100%' : isForgotPassword ? 720 : 480;

  return (
    <Card
      style={{
        width: cardWidth,
        minWidth: isMobile ? undefined : cardWidth,
        maxWidth: isMobile ? 480 : cardWidth,
        boxSizing: 'border-box',
        borderRadius: 20,
        boxShadow: '0 24px 48px rgba(0, 0, 0, 0.45), 0 0 1px rgba(255,255,255,0.2)',
        backgroundColor: 'rgba(255, 255, 255, 0.96)',
        backdropFilter: 'blur(16px)',
        border: '1px solid rgba(255, 255, 255, 0.3)',
        transition:
          'width 0.45s cubic-bezier(0.16, 1, 0.3, 1), min-width 0.45s cubic-bezier(0.16, 1, 0.3, 1), max-width 0.45s cubic-bezier(0.16, 1, 0.3, 1)',
        overflow: 'hidden',
      }}
      styles={{
        body: {
          padding: isMobile ? '28px 20px' : isForgotPassword ? '36px 40px' : '36px 32px',
          transition: 'padding 0.35s ease',
        },
      }}
    >
      {isForgotPassword ? (
        /* Şifremi Unuttum Görünümü (NVİ ve Parola Sıfırlama) */
        <LoginForgotPasswordView onBackToLogin={() => setIsForgotPassword(false)} />
      ) : (
        /* Standart Giriş Görünümü */
        <div style={{ animation: 'fadeIn 0.35s cubic-bezier(0.16, 1, 0.3, 1)' }}>
          <div style={{ textAlign: 'center', marginBottom: 24 }}>
            <div
              style={{
                width: 52,
                height: 52,
                borderRadius: '50%',
                background: '#e6f4ff',
                color: '#0958d9',
                fontSize: 24,
                display: 'inline-flex',
                alignItems: 'center',
                justifyContent: 'center',
                marginBottom: 12,
              }}
            >
              <SafetyCertificateOutlined />
            </div>
            <Title level={3} style={{ margin: 0, fontWeight: 700, color: '#092b5a' }}>
              {t('login_card_title')}
            </Title>
            <Text type="secondary" style={{ fontSize: 13, display: 'block', marginTop: 4 }}>
              {t('login_card_subtitle')}
            </Text>
          </div>

          {/* e-Devlet ve obsKimlik Yan Yana Giriş Butonları */}
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: '1fr 1fr',
              gap: 10,
              marginBottom: 18,
            }}
          >
            <Button
              size="middle"
              style={{
                height: 44,
                backgroundColor: '#dc2626',
                color: '#ffffff',
                borderColor: '#dc2626',
                fontWeight: 600,
                fontSize: 12,
                borderRadius: 8,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                gap: 6,
                whiteSpace: 'nowrap',
                overflow: 'hidden',
                textOverflow: 'ellipsis',
              }}
              onClick={() =>
                message.info('e-Devlet Kapısı kimlik doğrulama servisine bağlanılıyor...')
              }
            >
              <span style={{ fontWeight: 800 }}>e-Devlet</span>
              <span>{t('edevlet_login').replace('e-Devlet ile ', '').replace('e-Devlet ', '')}</span>
            </Button>

            <Button
              size="middle"
              style={{
                height: 44,
                backgroundColor: '#0d9488',
                color: '#ffffff',
                borderColor: '#0d9488',
                fontWeight: 600,
                fontSize: 12,
                borderRadius: 8,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                gap: 6,
                whiteSpace: 'nowrap',
                overflow: 'hidden',
                textOverflow: 'ellipsis',
              }}
              onClick={() =>
                message.info('obsKimlik Güvenli Dijital Kampüs Doğrulamasına bağlanılıyor...')
              }
            >
              <IdcardOutlined style={{ fontSize: 15 }} />
              <span>{t('obskimlik_login')}</span>
            </Button>
          </div>

          <Divider style={{ margin: '14px 0 18px', fontSize: 12, color: '#94a3b8' }}>
            {t('or_credentials')}
          </Divider>

          {/* TEK LOGIN NOKTASI */}
          <Form
            form={form}
            layout="vertical"
            initialValues={{
              email: 'ogrenci@obs.edu.tr',
              password: 'Obs123456!*',
              remember: true,
            }}
            onFinish={onSubmit}
            requiredMark={false}
          >
            <Form.Item
              name="email"
              label={
                <span style={{ fontWeight: 600, fontSize: 13 }}>{t('email_or_tckn_label')}</span>
              }
              rules={[{ required: true, message: t('email_required_error') }]}
              style={{ marginBottom: 16 }}
            >
              <Input
                prefix={<UserOutlined style={{ color: '#94a3b8' }} />}
                placeholder={t('email_or_tckn_placeholder')}
                size="large"
                style={{ borderRadius: 8, height: 42 }}
              />
            </Form.Item>

            <Form.Item
              name="password"
              label={<span style={{ fontWeight: 600, fontSize: 13 }}>{t('password_label')}</span>}
              rules={[{ required: true, message: t('password_required_error') }]}
              style={{ marginBottom: 14 }}
            >
              <Input.Password
                prefix={<LockOutlined style={{ color: '#94a3b8' }} />}
                placeholder={t('password_placeholder')}
                size="large"
                style={{ borderRadius: 8, height: 42 }}
              />
            </Form.Item>

            {/* Beni Hatırla & Şifremi Unuttum? Yan Yana Doğru Konumda */}
            <div
              style={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                marginBottom: 18,
              }}
            >
              <Form.Item name="remember" valuePropName="checked" noStyle>
                <Checkbox style={{ fontSize: 13 }}>{t('remember_me')}</Checkbox>
              </Form.Item>

              <Button
                type="link"
                onClick={() => setIsForgotPassword(true)}
                style={{
                  padding: 0,
                  fontSize: 13,
                  fontWeight: 600,
                  color: '#0958d9',
                  height: 'auto',
                }}
              >
                {t('forgot_password')}
              </Button>
            </div>

            <Form.Item style={{ marginBottom: 12 }}>
              <Button
                type="primary"
                htmlType="submit"
                size="large"
                block
                loading={submitting}
                style={{
                  height: 44,
                  fontSize: 15,
                  fontWeight: 600,
                  borderRadius: 8,
                  backgroundColor: '#0958d9',
                }}
              >
                {t('login_button')}
              </Button>
            </Form.Item>
          </Form>

          {/* Hızlı Demo Seçicileri */}
          <div
            style={{
              marginTop: 16,
              padding: '12px 14px',
              borderRadius: 10,
              backgroundColor: '#f8fafc',
              border: '1px solid #e2e8f0',
            }}
          >
            <Text
              style={{
                fontSize: 11,
                fontWeight: 700,
                color: '#64748b',
                display: 'block',
                marginBottom: 8,
                whiteSpace: 'nowrap',
                overflow: 'hidden',
                textOverflow: 'ellipsis',
              }}
            >
              {t('demo_title')}
            </Text>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6 }}>
              <Button size="small" onClick={() => onQuickFill('ogrenci@obs.edu.tr')}>
                {t('demo_student')}
              </Button>
              <Button size="small" onClick={() => onQuickFill('akademik@obs.edu.tr')}>
                {t('demo_academic')}
              </Button>
              <Button size="small" onClick={() => onQuickFill('idari@obs.edu.tr')}>
                {t('demo_staff')}
              </Button>
              <Button
                size="small"
                style={{ borderColor: '#0958d9', color: '#0958d9', fontWeight: 600 }}
                onClick={() => onQuickFill('arastirma.gorevlisi@obs.edu.tr')}
              >
                {t('demo_multi_role')}
              </Button>
            </div>
          </div>
        </div>
      )}
    </Card>
  );
};
