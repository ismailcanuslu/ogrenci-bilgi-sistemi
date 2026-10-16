import React, { useState } from 'react';
import { Row, Col, Card, Typography, Tag, Progress, Table, Timeline, Button, Space, Drawer, Switch, message } from 'antd';
import {
  TrophyOutlined,
  ClockCircleOutlined,
  UserOutlined,
  NotificationOutlined,
  SettingOutlined,
  BookOutlined,
  CheckCircleOutlined,
  RightOutlined,
  FileTextOutlined,
  HeartOutlined,
  BarChartOutlined,
  MailOutlined,
} from '@ant-design/icons';
import { useAuth } from '../../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const { Title, Text, Paragraph } = Typography;

export const DashboardPage: React.FC = () => {
  const { user, activeProgram } = useAuth();
  const navigate = useNavigate();

  // Widget Görünürlük State'i (Özelleştirilebilir Widget Mimarisi)
  const [widgets, setWidgets] = useState({
    academicSummary: true,
    weeklySchedule: true,
    upcomingExams: true,
    advisor: true,
    announcements: true,
    shortcuts: true,
    socialSummary: true,
  });

  const [drawerOpen, setDrawerOpen] = useState<boolean>(false);

  const toggleWidget = (key: keyof typeof widgets) => {
    setWidgets((prev) => ({ ...prev, [key]: !prev[key] }));
    message.success('Widget yerleşimi güncellendi');
  };

  // Ders Programı Mock Tablosu
  const currentCourses = [
    { key: '1', code: 'BLG411', name: 'Bitirme Tasarım Projesi', instructor: 'Prof. Dr. Ahmet Bilgehan', day: 'Pazartesi 09:30', room: 'Amfi 3', ects: 8 },
    { key: '2', code: 'BLG452', name: 'Bulut Bilişim ve Mikroservisler', instructor: 'Doç. Dr. Selin Doğan', day: 'Salı 13:30', room: 'Yazılım Lab 4', ects: 5 },
    { key: '3', code: 'BLG301', name: 'Veritabanı Yönetim Sistemleri', instructor: 'Dr. Öğr. Üyesi Murat Kaya', day: 'Çarşamba 10:30', room: 'Amfi 1', ects: 6 },
    { key: '4', code: 'YZM302', name: 'Yazılım Kalite ve Test (ÇAP)', instructor: 'Dr. Emre Kurt', day: 'Perşembe 14:00', room: 'D-204', ects: 5 },
  ];

  const courseColumns = [
    {
      title: 'Ders Kodu & Adı',
      dataIndex: 'name',
      key: 'name',
      render: (_: string, record: typeof currentCourses[0]) => (
        <div>
          <div style={{ fontWeight: 600, color: '#0f4c81' }}>{record.code}</div>
          <div style={{ fontSize: 13, color: '#334155' }}>{record.name}</div>
        </div>
      ),
    },
    {
      title: 'Öğretim Üyesi',
      dataIndex: 'instructor',
      key: 'instructor',
      render: (val: string) => <Text type="secondary" style={{ fontSize: 13 }}>{val}</Text>,
    },
    {
      title: 'Gün & Saat',
      dataIndex: 'day',
      key: 'day',
      render: (val: string) => <Tag color="blue">{val}</Tag>,
    },
    {
      title: 'Derslik',
      dataIndex: 'room',
      key: 'room',
      render: (val: string) => <Tag color="default">{val}</Tag>,
    },
    {
      title: 'AKTS',
      dataIndex: 'ects',
      key: 'ects',
      render: (val: number) => <Tag color="purple">{val} AKTS</Tag>,
    },
  ];

  return (
    <div>
      {/* Karşılama ve Hızlı Durum Başlığı */}
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: 24,
          padding: '16px 20px',
          backgroundColor: '#ffffff',
          borderRadius: 10,
          border: '1px solid #e2e8f0',
        }}
      >
        <div>
          <Title level={4} style={{ margin: 0, color: '#0f172a', fontWeight: 700 }}>
            Hoş Geldiniz, {user?.fullName}
          </Title>
          <Text style={{ fontSize: 13, color: '#64748b' }}>
            {user?.department} | {activeProgram}
          </Text>
        </div>
        <Space size="middle">
          <Button
            icon={<SettingOutlined />}
            onClick={() => setDrawerOpen(true)}
            style={{ borderRadius: 6 }}
          >
            Widget Ekle / Özelleştir
          </Button>
          <Button
            type="primary"
            icon={<FileTextOutlined />}
            onClick={() => navigate('/student/transcript')}
            style={{ backgroundColor: '#0f4c81', borderRadius: 6 }}
          >
            Transkript Görüntüle
          </Button>
        </Space>
      </div>

      {/* Widget Grid */}
      <Row gutter={[20, 20]}>
        {/* 1. Akademik Durum Kartı */}
        {widgets.academicSummary && (
          <Col xs={24} md={12} lg={8}>
            <Card
              title={
                <Space>
                  <TrophyOutlined style={{ color: '#d97706' }} />
                  <span>Genel Akademik Durum</span>
                </Space>
              }
              extra={<Tag color="green">Aktif Öğrenci</Tag>}
              style={{ height: '100%' }}
            >
              <div style={{ display: 'flex', justifyContent: 'space-around', textAlign: 'center', marginBottom: 20 }}>
                <div>
                  <div style={{ fontSize: 32, fontWeight: 800, color: '#0f4c81', lineHeight: 1 }}>
                    {user?.gpa || 3.78}
                  </div>
                  <Text type="secondary" style={{ fontSize: 12 }}>Genel Ortalama (GANO)</Text>
                </div>
                <div style={{ width: 1, backgroundColor: '#e2e8f0' }} />
                <div>
                  <div style={{ fontSize: 32, fontWeight: 800, color: '#10b981', lineHeight: 1 }}>
                    {user?.completedCredits || 195}
                    <span style={{ fontSize: 16, color: '#64748b' }}>/{user?.totalCredits || 240}</span>
                  </div>
                  <Text type="secondary" style={{ fontSize: 12 }}>Tamamlanan AKTS</Text>
                </div>
              </div>

              <div style={{ marginBottom: 16 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 12, marginBottom: 4 }}>
                  <span>Mezuniyet İlerlemesi</span>
                  <span style={{ fontWeight: 600 }}>%81.25</span>
                </div>
                <Progress percent={81.25} strokeColor="#0f4c81" showInfo={false} />
              </div>

              <div style={{ display: 'flex', justifyContent: 'space-between', padding: '10px 12px', backgroundColor: '#f8fafc', borderRadius: 8 }}>
                <span style={{ fontSize: 12, color: '#475569' }}>Kayıtlı Sınıf: <strong>4. Sınıf</strong></span>
                <span style={{ fontSize: 12, color: '#475569' }}>Dönem: <strong>7. Yarıyıl</strong></span>
              </div>
            </Card>
          </Col>
        )}

        {/* 2. Danışman Bilgileri Kartı */}
        {widgets.advisor && (
          <Col xs={24} md={12} lg={8}>
            <Card
              title={
                <Space>
                  <UserOutlined style={{ color: '#0284c7' }} />
                  <span>Akademik Danışman</span>
                </Space>
              }
              extra={<Tag color="blue">Bölüm Başkanı</Tag>}
              style={{ height: '100%' }}
            >
              <div style={{ display: 'flex', alignItems: 'center', gap: 14, marginBottom: 16 }}>
                <div
                  style={{
                    width: 52,
                    height: 52,
                    borderRadius: '50%',
                    backgroundColor: '#e0f2fe',
                    color: '#0284c7',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: 22,
                    fontWeight: 700,
                  }}
                >
                  AB
                </div>
                <div>
                  <div style={{ fontWeight: 700, fontSize: 15, color: '#0f172a' }}>
                    {user?.advisorName || 'Prof. Dr. Ahmet Bilgehan'}
                  </div>
                  <Text type="secondary" style={{ fontSize: 12 }}>
                    Bilgisayar Mühendisliği Bölüm Bşk.
                  </Text>
                </div>
              </div>

              <div style={{ fontSize: 13, display: 'flex', flexDirection: 'column', gap: 8, marginBottom: 16 }}>
                <div><strong>Ofis:</strong> MDBF Mühendislik Binası D-302</div>
                <div><strong>Görüşme Saatleri:</strong> Salı & Perşembe 14:00 - 16:00</div>
                <div><strong>E-Posta:</strong> ahmet.bilgehan@obs.edu.tr</div>
              </div>

              <Space style={{ width: '100%' }}>
                <Button block icon={<MailOutlined />} size="small" style={{ borderRadius: 6 }} onClick={() => message.info('Danışman mesajlaşma modülü açılıyor.')}>
                  Mesaj Gönder
                </Button>
                <Button block type="primary" size="small" style={{ borderRadius: 6, backgroundColor: '#0f4c81' }} onClick={() => message.info('Görüşme randevu talebi iletildi.')}>
                  Randevu Talep Et
                </Button>
              </Space>
            </Card>
          </Col>
        )}

        {/* 3. Yaklaşan Sınavlar ve Teslimler */}
        {widgets.upcomingExams && (
          <Col xs={24} md={24} lg={8}>
            <Card
              title={
                <Space>
                  <ClockCircleOutlined style={{ color: '#ef4444' }} />
                  <span>Yaklaşan Sınavlar & Teslimler</span>
                </Space>
              }
              extra={<Tag color="red">3 Önemli Tarih</Tag>}
              style={{ height: '100%' }}
            >
              <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
                <div style={{ padding: '10px 12px', borderLeft: '4px solid #ef4444', backgroundColor: '#fef2f2', borderRadius: '0 8px 8px 0' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Text strong style={{ fontSize: 13, color: '#991b1b' }}>BLG411 Bitirme Projesi Raporu</Text>
                    <Tag color="error">2 Gün Kaldı</Tag>
                  </div>
                  <div style={{ fontSize: 12, color: '#b91c1c' }}>17 Ekim 2026, Saat 23:59 | Online Yükleme</div>
                </div>

                <div style={{ padding: '10px 12px', borderLeft: '4px solid #f59e0b', backgroundColor: '#fffbeb', borderRadius: '0 8px 8px 0' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Text strong style={{ fontSize: 13, color: '#92400e' }}>BLG452 Bulut Bilişim Ara Sınavı</Text>
                    <Tag color="warning">8 Gün Kaldı</Tag>
                  </div>
                  <div style={{ fontSize: 12, color: '#b45309' }}>23 Ekim 2026, 14:00 | Lab 4</div>
                </div>

                <div style={{ padding: '10px 12px', borderLeft: '4px solid #3b82f6', backgroundColor: '#eff6ff', borderRadius: '0 8px 8px 0' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Text strong style={{ fontSize: 13, color: '#1e40af' }}>BLG301 Veritabanı Proje Ödevi</Text>
                    <Tag color="processing">14 Gün Kaldı</Tag>
                  </div>
                  <div style={{ fontSize: 12, color: '#2563eb' }}>29 Ekim 2026, 18:00 | GitHub Repository</div>
                </div>
              </div>
            </Card>
          </Col>
        )}

        {/* 4. Dönem Dersleri Listesi (Görseldeki Ana Tablo) */}
        {widgets.weeklySchedule && (
          <Col xs={24} lg={16}>
            <Card
              title={
                <Space>
                  <BookOutlined style={{ color: '#0f4c81' }} />
                  <span>2026-2027 Güz Dönemi Kayıtlı Dersler</span>
                </Space>
              }
              extra={
                <Button size="small" type="link" onClick={() => navigate('/student/course-registration')}>
                  Tüm Müfredatı Gör <RightOutlined />
                </Button>
              }
            >
              <Table
                dataSource={currentCourses}
                columns={courseColumns}
                pagination={false}
                size="middle"
                style={{ borderRadius: 8 }}
              />
            </Card>
          </Col>
        )}

        {/* 5. Hızlı Kısayollar ve Toplumsal Katkı */}
        <Col xs={24} lg={8}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
            {widgets.shortcuts && (
              <Card
                title={
                  <Space>
                    <CheckCircleOutlined style={{ color: '#10b981' }} />
                    <span>Hızlı Kısayollar</span>
                  </Space>
                }
              >
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 10 }}>
                  <Button
                    style={{ height: 'auto', padding: '12px 10px', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 6, borderRadius: 8 }}
                    onClick={() => navigate('/student/transcript')}
                  >
                    <FileTextOutlined style={{ fontSize: 20, color: '#0f4c81' }} />
                    <span style={{ fontSize: 12, fontWeight: 500 }}>E-Transkript</span>
                  </Button>
                  <Button
                    style={{ height: 'auto', padding: '12px 10px', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 6, borderRadius: 8 }}
                    onClick={() => navigate('/education-catalog')}
                  >
                    <BookOutlined style={{ fontSize: 20, color: '#6366f1' }} />
                    <span style={{ fontSize: 12, fontWeight: 500 }}>Eğitim Kataloğu</span>
                  </Button>
                  <Button
                    style={{ height: 'auto', padding: '12px 10px', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 6, borderRadius: 8 }}
                    onClick={() => navigate('/social-contribution')}
                  >
                    <HeartOutlined style={{ fontSize: 20, color: '#ec4899' }} />
                    <span style={{ fontSize: 12, fontWeight: 500 }}>Toplumsal Katkı</span>
                  </Button>
                  <Button
                    style={{ height: 'auto', padding: '12px 10px', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 6, borderRadius: 8 }}
                    onClick={() => navigate('/institutional-evaluation')}
                  >
                    <BarChartOutlined style={{ fontSize: 20, color: '#0284c7' }} />
                    <span style={{ fontSize: 12, fontWeight: 500 }}>Kurumsal Analiz</span>
                  </Button>
                </div>
              </Card>
            )}

            {widgets.announcements && (
              <Card
                title={
                  <Space>
                    <NotificationOutlined style={{ color: '#8b5cf6' }} />
                    <span>Akademik Duyurular</span>
                  </Space>
                }
              >
                <Timeline
                  items={[
                    {
                      color: 'red',
                      children: (
                        <div>
                          <Text strong style={{ fontSize: 13 }}>Erasmus+ Dil Sınavı Başvuruları</Text>
                          <div style={{ fontSize: 12, color: '#64748b' }}>Son gün: 24 Ekim 2026</div>
                        </div>
                      ),
                    },
                    {
                      color: 'blue',
                      children: (
                        <div>
                          <Text strong style={{ fontSize: 13 }}>Bahar Dönemi ÇAP / Yandal Kontenjanları</Text>
                          <div style={{ fontSize: 12, color: '#64748b' }}>Senato kararı ilan edildi</div>
                        </div>
                      ),
                    },
                    {
                      color: 'green',
                      children: (
                        <div>
                          <Text strong style={{ fontSize: 13 }}>Kampüs Teknoloji Zirvesi 2026</Text>
                          <div style={{ fontSize: 12, color: '#64748b' }}>Kayıtlar başladı</div>
                        </div>
                      ),
                    },
                  ]}
                />
              </Card>
            )}
          </div>
        </Col>
      </Row>

      {/* Widget Özelleştirme Çekmecesi */}
      <Drawer
        title="Dashboard Widgetlarını Yönet"
        placement="right"
        onClose={() => setDrawerOpen(false)}
        open={drawerOpen}
        width={360}
      >
        <Paragraph type="secondary" style={{ fontSize: 13, marginBottom: 20 }}>
          Ana panelinizde görmek istediğiniz widget'ları açıp kapatabilirsiniz. Tercihleriniz anında kaydedilir.
        </Paragraph>

        <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span>Genel Akademik Durum</span>
            <Switch checked={widgets.academicSummary} onChange={() => toggleWidget('academicSummary')} />
          </div>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span>Haftalık Ders Takvimi & Tablosu</span>
            <Switch checked={widgets.weeklySchedule} onChange={() => toggleWidget('weeklySchedule')} />
          </div>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span>Yaklaşan Sınavlar ve Teslimler</span>
            <Switch checked={widgets.upcomingExams} onChange={() => toggleWidget('upcomingExams')} />
          </div>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span>Akademik Danışman Bilgileri</span>
            <Switch checked={widgets.advisor} onChange={() => toggleWidget('advisor')} />
          </div>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span>Hızlı Kısayollar Paneli</span>
            <Switch checked={widgets.shortcuts} onChange={() => toggleWidget('shortcuts')} />
          </div>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span>Akademik Duyurular Akışı</span>
            <Switch checked={widgets.announcements} onChange={() => toggleWidget('announcements')} />
          </div>
        </div>
      </Drawer>
    </div>
  );
};
