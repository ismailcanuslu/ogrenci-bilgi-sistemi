import React from 'react';
import { Card, Row, Col, Typography, Tag, Button, Progress, Table } from 'antd';
import {
  HeartOutlined,
  TeamOutlined,
  CheckCircleOutlined,
  PlusOutlined,
} from '@ant-design/icons';

const { Title, Text } = Typography;

export const SocialContributionPage: React.FC = () => {
  const projects = [
    {
      key: '1',
      title: 'Köy Okullarına Kodlama ve Robotik Eğitimi',
      category: 'Eğitim & Teknoloji',
      hours: 24,
      status: 'Tamamlandı',
      date: 'Ekim 2026',
    },
    {
      key: '2',
      title: 'Yeşil Kampüs Sıfır Atık Farkındalık Çalıştayı',
      category: 'Çevre & Sürdürülebilirlik',
      hours: 12,
      status: 'Devam Ediyor',
      date: 'Kasım 2026',
    },
    {
      key: '3',
      title: 'Huzurevi Ziyareti ve Dijital Okuryazarlık Desteği',
      category: 'Sosyal Dayanışma',
      hours: 16,
      status: 'Onaylandı',
      date: 'Aralık 2026',
    },
  ];

  return (
    <div>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: 20,
          padding: '20px 24px',
          backgroundColor: '#ffffff',
          borderRadius: 10,
          border: '1px solid #e2e8f0',
        }}
      >
        <div>
          <Title level={4} style={{ margin: 0, color: '#0f172a', fontWeight: 700 }}>
            Toplumsal Katkı & Gönüllülük Koordinatörlüğü
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Öğrenci Sosyal Sorumluluk ve Gönüllülük Faaliyetleri Takip Portalı
          </Text>
        </div>
        <Button type="primary" icon={<PlusOutlined />} style={{ backgroundColor: '#ec4899', borderColor: '#ec4899', borderRadius: 6 }}>
          Yeni Proje Önerisi / Başvuru
        </Button>
      </div>

      <Row gutter={[20, 20]} style={{ marginBottom: 20 }}>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12, marginBottom: 12 }}>
              <HeartOutlined style={{ fontSize: 28, color: '#ec4899' }} />
              <div>
                <div style={{ fontSize: 24, fontWeight: 700, color: '#0f172a' }}>36 Saat</div>
                <Text type="secondary" style={{ fontSize: 12 }}>Tamamlanan Gönüllülük</Text>
              </div>
            </div>
            <Progress percent={60} strokeColor="#ec4899" />
            <Text type="secondary" style={{ fontSize: 11, display: 'block', marginTop: 6 }}>
              Mezuniyet Sosyal Katkı Yönergesi Hedefi: 60 Saat
            </Text>
          </Card>
        </Col>

        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
              <CheckCircleOutlined style={{ fontSize: 28, color: '#10b981' }} />
              <div>
                <div style={{ fontSize: 24, fontWeight: 700, color: '#0f172a' }}>3 Proje</div>
                <Text type="secondary" style={{ fontSize: 12 }}>Katılınan Sosyal Faaliyet</Text>
              </div>
            </div>
            <div style={{ marginTop: 16 }}>
              <Tag color="green">Sosyal Katkı Sertifikası Hak Edildi</Tag>
            </div>
          </Card>
        </Col>

        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
              <TeamOutlined style={{ fontSize: 28, color: '#0f4c81' }} />
              <div>
                <div style={{ fontSize: 24, fontWeight: 700, color: '#0f4c81' }}>Topluluk Üyeliği</div>
                <Text type="secondary" style={{ fontSize: 12 }}>Yazılım ve İnovasyon Kulübü</Text>
              </div>
            </div>
            <div style={{ marginTop: 16 }}>
              <Tag color="blue">Aktif Proje Lideri</Tag>
            </div>
          </Card>
        </Col>
      </Row>

      <Card title="Kayıtlı Gönüllülük ve Sosyal Katkı Faaliyetleri" style={{ borderRadius: 10 }}>
        <Table
          dataSource={projects}
          columns={[
            {
              title: 'Faaliyet Adı',
              dataIndex: 'title',
              key: 'title',
              render: (t: string) => <strong style={{ color: '#0f4c81' }}>{t}</strong>,
            },
            {
              title: 'Alan',
              dataIndex: 'category',
              key: 'category',
              render: (c: string) => <Tag color="magenta">{c}</Tag>,
            },
            {
              title: 'Saat',
              dataIndex: 'hours',
              key: 'hours',
              render: (h: number) => <span>{h} Saat</span>,
            },
            {
              title: 'Tarih',
              dataIndex: 'date',
              key: 'date',
            },
            {
              title: 'Durum',
              dataIndex: 'status',
              key: 'status',
              render: (s: string) => (
                <Tag color={s === 'Tamamlandı' ? 'green' : s === 'Devam Ediyor' ? 'blue' : 'orange'}>
                  {s}
                </Tag>
              ),
            },
          ]}
          pagination={false}
        />
      </Card>
    </div>
  );
};
