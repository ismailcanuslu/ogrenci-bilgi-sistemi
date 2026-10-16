import React from 'react';
import {
  Modal,
  Button,
  Typography,
  Space,
  Tag,
  Divider,
  List,
  Alert,
} from 'antd';
import {
  BankOutlined,
  BellOutlined,
  PhoneOutlined,
  CustomerServiceOutlined,
  SafetyOutlined,
} from '@ant-design/icons';
import { useLanguage } from '../../../context/LanguageContext';
import type { PortalSettings } from '../../../api/portalSettings';

const { Text, Paragraph } = Typography;

interface LoginModalsProps {
  settings: PortalSettings;
  aboutVisible: boolean;
  announcementsVisible: boolean;
  phoneDirectoryVisible: boolean;
  supportVisible: boolean;
  kvkkVisible: boolean;
  onCloseAbout: () => void;
  onCloseAnnouncements: () => void;
  onClosePhoneDirectory: () => void;
  onCloseSupport: () => void;
  onCloseKvkk: () => void;
}

export const LoginModals: React.FC<LoginModalsProps> = ({
  settings,
  aboutVisible,
  announcementsVisible,
  phoneDirectoryVisible,
  supportVisible,
  kvkkVisible,
  onCloseAbout,
  onCloseAnnouncements,
  onClosePhoneDirectory,
  onCloseSupport,
  onCloseKvkk,
}) => {
  const { t } = useLanguage();

  return (
    <>
      {/* 1. Hakkımızda Modalı */}
      <Modal
        title={
          <Space>
            <BankOutlined style={{ color: '#0958d9' }} />
            <span>{settings.aboutTitle || t('modal_about_title')}</span>
          </Space>
        }
        open={aboutVisible}
        onOk={onCloseAbout}
        onCancel={onCloseAbout}
        footer={[
          <Button key="close" type="primary" onClick={onCloseAbout}>
            {t('modal_close')}
          </Button>,
        ]}
        width={680}
      >
        <Paragraph style={{ fontSize: 14, lineHeight: 1.7, color: '#334155' }}>
          {settings.aboutContent || t('modal_about_content')}
        </Paragraph>
        <Divider />
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
          <div style={{ padding: 12, background: '#f8fafc', borderRadius: 8 }}>
            <Text strong>{t('modal_accreditation').split(':')[0]}:</Text>
            <div>{t('modal_accreditation').split(':')[1]}</div>
          </div>
          <div style={{ padding: 12, background: '#f8fafc', borderRadius: 8 }}>
            <Text strong>{t('modal_security').split(':')[0]}:</Text>
            <div>{t('modal_security').split(':')[1]}</div>
          </div>
        </div>
      </Modal>

      {/* 2. Duyurular Modalı */}
      <Modal
        title={
          <Space>
            <BellOutlined style={{ color: '#0958d9' }} />
            <span>{t('modal_announcements_title')}</span>
          </Space>
        }
        open={announcementsVisible}
        onOk={onCloseAnnouncements}
        onCancel={onCloseAnnouncements}
        footer={[
          <Button key="close" type="primary" onClick={onCloseAnnouncements}>
            {t('modal_understood')}
          </Button>,
        ]}
        width={720}
      >
        <List
          itemLayout="vertical"
          dataSource={settings.announcements}
          renderItem={(item) => (
            <List.Item
              key={item.id}
              style={{ padding: '14px 0', borderBottom: '1px solid #f1f5f9' }}
            >
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 6 }}>
                <Space>
                  <Tag color={item.priority === 'High' ? 'red' : item.priority === 'Medium' ? 'orange' : 'blue'}>
                    {item.category}
                  </Tag>
                  <Text strong style={{ fontSize: 15, color: '#0f172a' }}>
                    {item.title}
                  </Text>
                </Space>
                <Text type="secondary" style={{ fontSize: 12 }}>
                  {item.date}
                </Text>
              </div>
              <Paragraph style={{ margin: 0, color: '#475569', fontSize: 13, lineHeight: 1.6 }}>
                {item.content}
              </Paragraph>
            </List.Item>
          )}
        />
      </Modal>

      {/* 3. Telefon Rehberi Modalı */}
      <Modal
        title={
          <Space>
            <PhoneOutlined style={{ color: '#0958d9' }} />
            <span>{t('modal_phone_directory_title')}</span>
          </Space>
        }
        open={phoneDirectoryVisible}
        onCancel={onClosePhoneDirectory}
        footer={[
          <Button key="close" type="primary" onClick={onClosePhoneDirectory}>
            {t('modal_close')}
          </Button>,
        ]}
        width={600}
      >
        <List
          bordered
          dataSource={[
            { unit: 'Öğrenci İşleri Daire Başkanlığı', phone: '+90 (212) 444 0 627', ext: '1010 - 1015' },
            { unit: 'Bilgi İşlem Daire Başkanlığı (OBS Destek)', phone: '+90 (212) 444 0 627', ext: '2020' },
            { unit: 'Mühendislik Fakültesi Sekreterliği', phone: '+90 (212) 444 0 627', ext: '3100' },
            { unit: 'Sağlık, Kültür ve Spor Daire Bşk.', phone: '+90 (212) 444 0 627', ext: '1040' },
            { unit: 'Kütüphane ve Dokümantasyon Daire Bşk.', phone: '+90 (212) 444 0 627', ext: '1050' },
          ]}
          renderItem={(item) => (
            <List.Item>
              <div>
                <Text strong>{item.unit}</Text>
                <div style={{ color: '#64748b', fontSize: 12 }}>Dahili: {item.ext}</div>
              </div>
              <Tag color="blue">{item.phone}</Tag>
            </List.Item>
          )}
        />
      </Modal>

      {/* 4. OBS Destek Modalı */}
      <Modal
        title={
          <Space>
            <CustomerServiceOutlined style={{ color: '#0958d9' }} />
            <span>{t('modal_support_title')}</span>
          </Space>
        }
        open={supportVisible}
        onCancel={onCloseSupport}
        footer={[
          <Button key="close" type="primary" onClick={onCloseSupport}>
            {t('modal_close')}
          </Button>,
        ]}
        width={560}
      >
        <Paragraph>
          {t('modal_support_desc')}
        </Paragraph>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          <Alert message={t('modal_support_email')} type="info" showIcon />
          <Alert message={t('modal_support_call')} type="success" showIcon />
          <Alert message={t('modal_support_desk')} type="warning" showIcon />
        </div>
      </Modal>

      {/* 5. KVKK Aydınlatma Metni Modalı */}
      <Modal
        title={
          <Space>
            <SafetyOutlined style={{ color: '#0958d9' }} />
            <span>{t('modal_kvkk_title')}</span>
          </Space>
        }
        open={kvkkVisible}
        onCancel={onCloseKvkk}
        footer={[
          <Button key="close" type="primary" onClick={onCloseKvkk}>
            {t('modal_understood')}
          </Button>,
        ]}
        width={700}
      >
        <div style={{ maxHeight: 380, overflowY: 'auto', paddingRight: 8, fontSize: 13, lineHeight: 1.7, color: '#334155' }}>
          <Paragraph strong>1. Veri Sorumlusunun Kimliği</Paragraph>
          <Paragraph>
            6698 sayılı Kişisel Verilerin Korunması Kanunu ("KVKK") uyarınca, T.C. Anadolu Devlet Üniversitesi olarak, veri sorumlusu sıfatıyla, kişisel verileriniz aşağıda açıklanan çerçevede kaydedilmekte, saklanmakta ve işlenmektedir.
          </Paragraph>
          <Paragraph strong>2. Kişisel Verilerin İşlenme Amaçları</Paragraph>
          <Paragraph>
            Öğrenci kayıt işlemleri, transkript ve not dökümü hazırlama, ders kaydı ve akademik değerlendirme, mezuniyet takibi, YÖKSİS entegrasyonu, kütüphane ve yemekhane geçiş kontrolü amaçlarıyla verileriniz güvenli sunucularımızda işlenmektedir.
          </Paragraph>
          <Paragraph strong>3. Bilgi Güvenliği Tedbirleri</Paragraph>
          <Paragraph>
            Veritabanlarımızda tüm parolalar Argon2id algoritmasıyla kriptografik olarak tuzlanarak saklanmakta; oturum yönetimi 256-Bit SSL/TLS şifreleme ve RSA-256 JWT imzaları ile korunmaktadır.
          </Paragraph>
        </div>
      </Modal>
    </>
  );
};
