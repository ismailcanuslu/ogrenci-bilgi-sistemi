import React, { useEffect, useState } from 'react';
import {
  Card,
  Table,
  Button,
  Modal,
  Form,
  Input,
  Radio,
  Slider,
  InputNumber,
  Space,
  Tag,
  Typography,
  message,
  Popconfirm,
  Row,
  Col,
  Alert,
  Select,
} from 'antd';
import {
  PictureOutlined,
  SaveOutlined,
  PlusOutlined,
  DeleteOutlined,
  NotificationOutlined,
  InfoCircleOutlined,
} from '@ant-design/icons';
import { portalApi, defaultPortalSettings } from '../../api/portalSettings';
import type { PortalSettings, PortalAnnouncement } from '../../api/portalSettings';

const { Title, Text, Paragraph } = Typography;
const { TextArea } = Input;

export const LoginPageSettingsPage: React.FC = () => {
  const [settings, setSettings] = useState<PortalSettings>(defaultPortalSettings);
  const [saving, setSaving] = useState<boolean>(false);
  const [customImageUrl, setCustomImageUrl] = useState<string>('');

  // Duyuru Modalı
  const [announcementModalOpen, setAnnouncementModalOpen] = useState<boolean>(false);
  const [announcementForm] = Form.useForm();

  useEffect(() => {
    portalApi.getSettings().then((res) => {
      setSettings(res);
    });
  }, []);

  const handleSave = async () => {
    setSaving(true);
    try {
      await portalApi.updateSettings(settings);
      message.success('Giriş sayfası görünüm ve içerik ayarları başarıyla kaydedildi!');
    } catch {
      message.error('Ayarlar kaydedilirken hata oluştu.');
    } finally {
      setSaving(false);
    }
  };

  const handleAddAnnouncement = (values: {
    title: string;
    category: string;
    priority: 'High' | 'Medium' | 'Normal';
    content: string;
  }) => {
    const newId = (settings.announcements?.length || 0) + 1;
    const newAnn: PortalAnnouncement = {
      id: newId,
      title: values.title,
      category: values.category || 'Genel',
      priority: values.priority || 'Normal',
      content: values.content,
      date: new Date().toLocaleDateString('tr-TR', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
      }),
    };

    setSettings((prev) => ({
      ...prev,
      announcements: [newAnn, ...(prev.announcements || [])],
    }));

    setAnnouncementModalOpen(false);
    announcementForm.resetFields();
    message.success('Duyuru eklendi. Değişiklikleri kaydet butonuna basmayı unutmayınız.');
  };

  const handleDeleteAnnouncement = (id: number) => {
    setSettings((prev) => ({
      ...prev,
      announcements: (prev.announcements || []).filter((a) => a.id !== id),
    }));
    message.info('Duyuru listeden kaldırıldı.');
  };

  const handleAddImage = () => {
    if (!customImageUrl.trim()) return;
    if (settings.coverImages.includes(customImageUrl.trim())) {
      message.warning('Bu görsel zaten listede mevcut.');
      return;
    }
    setSettings((prev) => ({
      ...prev,
      coverImages: [...prev.coverImages, customImageUrl.trim()],
    }));
    setCustomImageUrl('');
    message.success('Görsel eklendi.');
  };

  const handleRemoveImage = (img: string) => {
    if (settings.coverImages.length <= 1) {
      message.warning('Sistemde en az bir kapak görseli bulunmalıdır.');
      return;
    }
    setSettings((prev) => ({
      ...prev,
      coverImages: prev.coverImages.filter((c) => c !== img),
    }));
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
            Genel Ayarlar: Giriş Sayfasını Özelleştirme
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Login ekranı slayt gösterisi, geçiş süresi, arka plan modları, Hakkımızda ve Duyuru içerikleri
          </Text>
        </div>
        <Button
          type="primary"
          icon={<SaveOutlined />}
          loading={saving}
          style={{ backgroundColor: '#16a34a', borderColor: '#16a34a', height: 40, borderRadius: 6 }}
          onClick={handleSave}
        >
          Değişiklikleri Sisteme Kaydet
        </Button>
      </div>

      <Alert
        message="Canlı Giriş Sayfası Görünüm Yönetimi"
        description="Buradan yapacağınız değişiklikler login ekranındaki dolan ilerleme çubuğuna, sinematik geçiş efektlerine ve üst bardaki modallara anlık olarak etki eder."
        type="info"
        showIcon
        style={{ borderRadius: 8 }}
      />

      {/* 1. Arka Plan Modu & Slayt Gösterisi */}
      <Card
        title={
          <Space>
            <PictureOutlined style={{ color: '#0f4c81' }} />
            <span>1. Arka Plan Modu & Slayt Süresi</span>
          </Space>
        }
        style={{ borderRadius: 10 }}
      >
        <Row gutter={[24, 24]}>
          <Col xs={24} md={12}>
            <Text strong style={{ display: 'block', marginBottom: 10, fontSize: 13 }}>
              Giriş Ekranı Arka Plan Modu:
            </Text>
            <Radio.Group
              value={settings.backgroundMode}
              onChange={(e) => setSettings((prev) => ({ ...prev, backgroundMode: e.target.value }))}
            >
              <Space direction="vertical" size="middle">
                <Radio value="empty">
                  <strong>Boş (Gradyan Koyu Mavi):</strong> Sade ve kurumsal degrade arka plan
                </Radio>
                <Radio value="single">
                  <strong>Tek Resim:</strong> İlk kapak görseli sabit arka plan olarak kullanılır
                </Radio>
                <Radio value="slideshow">
                  <strong>Slayt Gösterisi (Önerilen):</strong> Otomatik soldan sağa dolan çubuk ve sinematik yumuşak geçiş
                </Radio>
              </Space>
            </Radio.Group>

            {settings.backgroundMode === 'slideshow' && (
              <div
                style={{
                  marginTop: 20,
                  padding: 16,
                  backgroundColor: '#f8fafc',
                  borderRadius: 8,
                  border: '1px solid #e2e8f0',
                }}
              >
                <Text strong style={{ display: 'block', marginBottom: 8, color: '#0f4c81' }}>
                  Slayt Otomatik Geçiş Süresi (Saniye):
                </Text>
                <Row gutter={16} align="middle">
                  <Col span={14}>
                    <Slider
                      min={2}
                      max={30}
                      value={settings.slideIntervalSeconds}
                      onChange={(val) =>
                        setSettings((prev) => ({ ...prev, slideIntervalSeconds: val }))
                      }
                    />
                  </Col>
                  <Col span={8}>
                    <InputNumber
                      min={2}
                      max={30}
                      value={settings.slideIntervalSeconds}
                      onChange={(val) =>
                        setSettings((prev) => ({ ...prev, slideIntervalSeconds: val || 5 }))
                      }
                      addonAfter="sn"
                    />
                  </Col>
                </Row>
                <Text type="secondary" style={{ fontSize: 11, display: 'block', marginTop: 4 }}>
                  Alt barın üstündeki mavi çubuk bu süre içinde soldan sağa dolup resmi değiştirir.
                </Text>
              </div>
            )}
          </Col>

          <Col xs={24} md={12}>
            <Text strong style={{ display: 'block', marginBottom: 10, fontSize: 13 }}>
              Kapak Görselleri Listesi:
            </Text>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
              {settings.coverImages?.map((img, idx) => (
                <div
                  key={img}
                  style={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    padding: '8px 12px',
                    backgroundColor: '#f8fafc',
                    borderRadius: 8,
                    border: '1px solid #e2e8f0',
                  }}
                >
                  <Space>
                    <img
                      src={img}
                      alt={`cover-${idx}`}
                      style={{ width: 56, height: 36, objectFit: 'cover', borderRadius: 4 }}
                    />
                    <div>
                      <strong style={{ fontSize: 13, display: 'block' }}>{img}</strong>
                      <span style={{ fontSize: 11, color: '#64748b' }}>
                        {idx === 0 ? 'Ana Varsayılan Kapak' : `Slayt #${idx + 1}`}
                      </span>
                    </div>
                  </Space>
                  <Button
                    danger
                    type="text"
                    size="small"
                    icon={<DeleteOutlined />}
                    onClick={() => handleRemoveImage(img)}
                  />
                </div>
              ))}

              <Space.Compact style={{ width: '100%', marginTop: 8 }}>
                <Input
                  placeholder="/cover-3.jpg veya resim adresi URL"
                  value={customImageUrl}
                  onChange={(e) => setCustomImageUrl(e.target.value)}
                />
                <Button type="primary" icon={<PlusOutlined />} onClick={handleAddImage}>
                  Kapak Ekle
                </Button>
              </Space.Compact>
            </div>
          </Col>
        </Row>
      </Card>

      {/* 2. Üst Bar Hakkımızda Metni */}
      <Card
        title={
          <Space>
            <InfoCircleOutlined style={{ color: '#10b981' }} />
            <span>2. Giriş Sayfası 'Hakkımızda' Bilgilendirme Metni</span>
          </Space>
        }
        style={{ borderRadius: 10 }}
      >
        <Form layout="vertical">
          <Form.Item label={<span style={{ fontWeight: 600 }}>Modal Başlığı</span>}>
            <Input
              value={settings.aboutTitle}
              onChange={(e) => setSettings((prev) => ({ ...prev, aboutTitle: e.target.value }))}
              placeholder="T.C. Anadolu Devlet Üniversitesi ve OBS Hakkında"
            />
          </Form.Item>
          <Form.Item label={<span style={{ fontWeight: 600 }}>Detaylı Açıklama & Akreditasyon Metni</span>}>
            <TextArea
              rows={4}
              value={settings.aboutContent}
              onChange={(e) => setSettings((prev) => ({ ...prev, aboutContent: e.target.value }))}
              placeholder="Üniversite tanıtımı, YÖKAK kalite güvencesi, Bologna süreci ve öğrenci hizmetleri..."
            />
          </Form.Item>
        </Form>
      </Card>

      {/* 3. Giriş Sayfası Duyuruları */}
      <Card
        title={
          <Space>
            <NotificationOutlined style={{ color: '#d97706' }} />
            <span>3. Giriş Sayfası 'Duyurular' Masası</span>
          </Space>
        }
        extra={
          <Button
            type="primary"
            size="small"
            icon={<PlusOutlined />}
            style={{ backgroundColor: '#0f4c81' }}
            onClick={() => setAnnouncementModalOpen(true)}
          >
            Yeni Duyuru Yayınla
          </Button>
        }
        style={{ borderRadius: 10 }}
      >
        <Table
          dataSource={settings.announcements?.map((a) => ({ ...a, key: a.id }))}
          pagination={false}
          size="middle"
          columns={[
            {
              title: 'Öncelik',
              dataIndex: 'priority',
              key: 'priority',
              width: 100,
              render: (p: string) => (
                <Tag color={p === 'High' ? 'red' : p === 'Medium' ? 'orange' : 'blue'}>
                  {p === 'High' ? 'Kritik' : p === 'Medium' ? 'Önemli' : 'Normal'}
                </Tag>
              ),
            },
            {
              title: 'Kategori',
              dataIndex: 'category',
              key: 'category',
              width: 140,
              render: (c: string) => <Tag color="cyan">{c}</Tag>,
            },
            {
              title: 'Duyuru Başlığı & İçeriği',
              key: 'content',
              render: (_: unknown, r: PortalAnnouncement) => (
                <div>
                  <strong style={{ fontSize: 13, color: '#0f172a' }}>{r.title}</strong>
                  <Paragraph ellipsis={{ rows: 1 }} style={{ fontSize: 12, color: '#64748b', margin: 0 }}>
                    {r.content}
                  </Paragraph>
                </div>
              ),
            },
            {
              title: 'Yayın Tarihi',
              dataIndex: 'date',
              key: 'date',
              width: 150,
            },
            {
              title: 'İşlem',
              key: 'action',
              width: 80,
              align: 'right',
              render: (_: unknown, r: PortalAnnouncement) => (
                <Popconfirm
                  title="Duyuruyu yayından kaldırmak istiyor musunuz?"
                  onConfirm={() => handleDeleteAnnouncement(r.id)}
                  okText="Evet, Kaldır"
                  cancelText="İptal"
                >
                  <Button danger type="text" size="small" icon={<DeleteOutlined />}>
                    Sil
                  </Button>
                </Popconfirm>
              ),
            },
          ]}
        />
      </Card>

      {/* Kaydet Alt Butonu */}
      <div style={{ display: 'flex', justifyContent: 'flex-end', paddingBottom: 24 }}>
        <Button
          type="primary"
          size="large"
          icon={<SaveOutlined />}
          loading={saving}
          style={{
            backgroundColor: '#16a34a',
            borderColor: '#16a34a',
            height: 48,
            padding: '0 36px',
            fontSize: 16,
            fontWeight: 600,
            borderRadius: 8,
          }}
          onClick={handleSave}
        >
          Tüm Giriş Sayfası Ayarlarını Sisteme Kaydet
        </Button>
      </div>

      {/* Yeni Duyuru Modalı */}
      <Modal
        title="Giriş Ekranında Yeni Duyuru Yayınla"
        open={announcementModalOpen}
        onCancel={() => setAnnouncementModalOpen(false)}
        onOk={() => announcementForm.submit()}
        okText="Duyuruyu Ekle"
        cancelText="İptal"
      >
        <Form form={announcementForm} layout="vertical" onFinish={handleAddAnnouncement}>
          <Form.Item
            name="title"
            label="Duyuru Başlığı"
            rules={[{ required: true, message: 'Duyuru başlığı giriniz' }]}
          >
            <Input placeholder="Örn: 2026-2027 Bahar Yarıyılı Ders Kayıt Takvimi" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="category" label="Kategori" initialValue="Akademik Takvim">
                <Select
                  options={[
                    { value: 'Akademik Takvim', label: 'Akademik Takvim' },
                    { value: 'Ders Kayıt', label: 'Ders Kayıt & Katkı Payı' },
                    { value: 'Sınavlar', label: 'Sınavlar & Not İlanları' },
                    { value: 'Genel', label: 'Genel Duyuru' },
                  ]}
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="priority" label="Öncelik Derecesi" initialValue="Normal">
                <Select
                  options={[
                    { value: 'Normal', label: 'Normal (Mavi)' },
                    { value: 'Medium', label: 'Önemli (Turuncu)' },
                    { value: 'High', label: 'Acil / Kritik (Kırmızı)' },
                  ]}
                />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item
            name="content"
            label="Duyuru Detay Metni"
            rules={[{ required: true, message: 'Duyuru metni giriniz' }]}
          >
            <TextArea rows={4} placeholder="Duyurunun öğrencilere ve personele yönelik tam metni..." />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};
