import React, { useEffect, useState } from 'react';
import {
  Card,
  Form,
  Input,
  Button,
  Typography,
  Row,
  Col,
  Space,
  Alert,
  message,
} from 'antd';
import {
  BankOutlined,
  SaveOutlined,
  PhoneOutlined,
  MailOutlined,
  GlobalOutlined,
  EnvironmentOutlined,
  SafetyCertificateOutlined,
} from '@ant-design/icons';
import { institutionApi } from '../../api/institutionSettings';
import type { InstitutionSettings } from '../../api/institutionSettings';

const { Title, Text } = Typography;

export const InstitutionSettingsPage: React.FC = () => {
  const [form] = Form.useForm<InstitutionSettings>();
  const [loading, setLoading] = useState<boolean>(true);
  const [saving, setSaving] = useState<boolean>(false);
  const [previewLogo, setPreviewLogo] = useState<string>('');

  useEffect(() => {
    institutionApi.getSettings().then((res) => {
      form.setFieldsValue(res);
      setPreviewLogo(res.logoUrl || '');
      setLoading(false);
    });
  }, [form]);

  const handleFinish = async (values: InstitutionSettings) => {
    setSaving(true);
    try {
      await institutionApi.updateSettings(values);
      message.success('Üniversite kurumsal kimlik ve iletişim bilgileri başarıyla güncellendi!');
    } catch {
      message.error('Bilgiler kaydedilirken hata oluştu.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
      {/* Üst Başlık */}
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          padding: '18px 24px',
          backgroundColor: '#ffffff',
          borderRadius: 10,
          border: '1px solid #e2e8f0',
        }}
      >
        <div>
          <Title level={4} style={{ margin: 0, color: '#0f172a', fontWeight: 700 }}>
            Genel Ayarlar: Üniversite Kimlik ve İletişim Bilgileri
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Sistem geneli üniversite adı, logosu, telefon rehberi ve resmi iletişim adreslerinin yönetimi
          </Text>
        </div>
        <Button
          type="primary"
          icon={<SaveOutlined />}
          loading={saving}
          style={{ backgroundColor: '#16a34a', borderColor: '#16a34a', height: 40, borderRadius: 6 }}
          onClick={() => form.submit()}
        >
          Değişiklikleri Kaydet
        </Button>
      </div>

      <Alert
        message="Merkezi Kurumsal Kimlik Entegrasyonu"
        description="Burada yapacağınız üniversite unvanı, logo ve iletişim bilgileri güncellemeleri; giriş ekranı (login), üst başlık çubuğu, transkript belgeleri ve alt bar (footer) telefon rehberine eşzamanlı olarak yansır."
        type="info"
        showIcon
        style={{ borderRadius: 8 }}
      />

      <Form form={form} layout="vertical" onFinish={handleFinish} disabled={loading}>
        {/* 1. Temel Kurumsal Kimlik */}
        <Card
          title={
            <Space>
              <BankOutlined style={{ color: '#0f4c81' }} />
              <span>1. Üniversite Temel Kimlik & Logo</span>
            </Space>
          }
          style={{ borderRadius: 10, marginBottom: 20 }}
        >
          <Row gutter={24}>
            <Col xs={24} md={16}>
              <Form.Item
                name="universityName"
                label={<span style={{ fontWeight: 600 }}>Üniversite Resmi Adı</span>}
                rules={[{ required: true, message: 'Üniversite adı zorunludur' }]}
              >
                <Input size="large" placeholder="Örn: T.C. ANADOLU DEVLET ÜNİVERSİTESİ" />
              </Form.Item>

              <Row gutter={16}>
                <Col span={12}>
                  <Form.Item
                    name="universityShortName"
                    label={<span style={{ fontWeight: 600 }}>Kısa Ad / Kısaltma</span>}
                  >
                    <Input size="large" placeholder="Örn: ADÜ" />
                  </Form.Item>
                </Col>
                <Col span={12}>
                  <Form.Item
                    name="rectorName"
                    label={<span style={{ fontWeight: 600 }}>Rektör (Unvan ve Ad Soyad)</span>}
                  >
                    <Input size="large" placeholder="Örn: Prof. Dr. Mehmet Fatih Yıldız" />
                  </Form.Item>
                </Col>
              </Row>

              <Form.Item
                name="logoUrl"
                label={<span style={{ fontWeight: 600 }}>Üniversite Logo URL'si veya Yolu</span>}
                help="Boş bırakıldığında modern kurumsal vektör armasını kullanır. Örn: /logo.png veya https://..."
              >
                <Input
                  size="large"
                  placeholder="/logo.png veya görsel bağlantısı"
                  onChange={(e) => setPreviewLogo(e.target.value)}
                />
              </Form.Item>
            </Col>

            <Col xs={24} md={8}>
              <div
                style={{
                  display: 'flex',
                  flexDirection: 'column',
                  alignItems: 'center',
                  justifyContent: 'center',
                  padding: 24,
                  backgroundColor: '#f8fafc',
                  border: '1px dashed #cbd5e1',
                  borderRadius: 12,
                  height: '100%',
                  minHeight: 200,
                  textAlign: 'center',
                }}
              >
                <Text type="secondary" style={{ fontSize: 12, marginBottom: 12, fontWeight: 600 }}>
                  LOGO CANLI ÖNİZLEME
                </Text>
                {previewLogo ? (
                  <img
                    src={previewLogo}
                    alt="Logo Önizleme"
                    style={{
                      maxHeight: 100,
                      maxWidth: '100%',
                      objectFit: 'contain',
                      borderRadius: 8,
                    }}
                    onError={() => setPreviewLogo('')}
                  />
                ) : (
                  <div
                    style={{
                      width: 80,
                      height: 80,
                      borderRadius: 16,
                      backgroundColor: '#0f4c81',
                      color: '#ffffff',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: 38,
                    }}
                  >
                    <BankOutlined />
                  </div>
                )}
                <Text style={{ fontSize: 11, color: '#64748b', marginTop: 12 }}>
                  Giriş ekranında ve sol üst barda bu amblem görünecektir.
                </Text>
              </div>
            </Col>
          </Row>
        </Card>

        {/* 2. Resmi İletişim & Santral */}
        <Card
          title={
            <Space>
              <PhoneOutlined style={{ color: '#10b981' }} />
              <span>2. Telefon & Santral & Çağrı Merkezi Bilgileri</span>
            </Space>
          }
          style={{ borderRadius: 10, marginBottom: 20 }}
        >
          <Row gutter={20}>
            <Col xs={24} md={12}>
              <Form.Item
                name="phoneNumber"
                label={<span style={{ fontWeight: 600 }}>Rektörlük Santral Telefonu</span>}
                rules={[{ required: true, message: 'Telefon numarası zorunludur' }]}
              >
                <Input
                  size="large"
                  prefix={<PhoneOutlined style={{ color: '#94a3b8' }} />}
                  placeholder="+90 (222) 335 05 80"
                />
              </Form.Item>
            </Col>

            <Col xs={24} md={12}>
              <Form.Item
                name="callCenterNumber"
                label={<span style={{ fontWeight: 600 }}>ÖBS Çağrı Merkezi / Çözüm Masası</span>}
              >
                <Input
                  size="large"
                  prefix={<PhoneOutlined style={{ color: '#94a3b8' }} />}
                  placeholder="0850 300 06 27"
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={20}>
            <Col xs={24} md={12}>
              <Form.Item
                name="email"
                label={<span style={{ fontWeight: 600 }}>Genel Sekreterlik Resmi E-Posta</span>}
                rules={[{ type: 'email' }]}
              >
                <Input
                  size="large"
                  prefix={<MailOutlined style={{ color: '#94a3b8' }} />}
                  placeholder="genelsekreterlik@anadolu.edu.tr"
                />
              </Form.Item>
            </Col>

            <Col xs={24} md={12}>
              <Form.Item
                name="supportEmail"
                label={<span style={{ fontWeight: 600 }}>ÖBS Teknik Destek E-Posta</span>}
                rules={[{ type: 'email' }]}
              >
                <Input
                  size="large"
                  prefix={<MailOutlined style={{ color: '#94a3b8' }} />}
                  placeholder="obs.destek@anadolu.edu.tr"
                />
              </Form.Item>
            </Col>
          </Row>
        </Card>

        {/* 3. Fiziki Adres & KEP */}
        <Card
          title={
            <Space>
              <EnvironmentOutlined style={{ color: '#d97706' }} />
              <span>3. Yerleşke Adresi & KEP & Web Bilgileri</span>
            </Space>
          }
          style={{ borderRadius: 10, marginBottom: 20 }}
        >
          <Form.Item
            name="address"
            label={<span style={{ fontWeight: 600 }}>Rektörlük Ana Kampüs Fiziki Adresi</span>}
            rules={[{ required: true, message: 'Adres zorunludur' }]}
          >
            <Input.TextArea
              rows={3}
              placeholder="Üniversite Caddesi, Kampüs Yerleşkesi No: 1, Tepebaşı / Eskişehir"
            />
          </Form.Item>

          <Row gutter={20}>
            <Col xs={24} md={12}>
              <Form.Item
                name="kepAddress"
                label={<span style={{ fontWeight: 600 }}>Resmi KEP (Kayıtlı Elektronik Posta) Adresi</span>}
              >
                <Input
                  size="large"
                  prefix={<SafetyCertificateOutlined style={{ color: '#94a3b8' }} />}
                  placeholder="anadolutelekom@hs01.kep.tr"
                />
              </Form.Item>
            </Col>

            <Col xs={24} md={12}>
              <Form.Item
                name="websiteUrl"
                label={<span style={{ fontWeight: 600 }}>Üniversite Resmi Web Sayfası</span>}
              >
                <Input
                  size="large"
                  prefix={<GlobalOutlined style={{ color: '#94a3b8' }} />}
                  placeholder="https://www.anadolu.edu.tr"
                />
              </Form.Item>
            </Col>
          </Row>
        </Card>

        {/* Kaydet Alt Çubuğu */}
        <div style={{ display: 'flex', justifyContent: 'flex-end', paddingBottom: 24 }}>
          <Button
            type="primary"
            size="large"
            icon={<SaveOutlined />}
            loading={saving}
            htmlType="submit"
            style={{
              backgroundColor: '#16a34a',
              borderColor: '#16a34a',
              height: 48,
              padding: '0 36px',
              fontSize: 16,
              fontWeight: 600,
              borderRadius: 8,
            }}
          >
            Tüm Kurumsal Bilgileri Sisteme Kaydet
          </Button>
        </div>
      </Form>
    </div>
  );
};
