import React from 'react';
import { Card, Tabs, Row, Col, Statistic, Progress, Typography, Tag, Space } from 'antd';
import {
  UserOutlined,
  TrophyOutlined,
  ExperimentOutlined,
} from '@ant-design/icons';

const { Title, Text, Paragraph } = Typography;

export const InstitutionalEvaluationPage: React.FC = () => {
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
            Kurumsal Değerlendirme & İstatistik Portalı
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            T.C. Yükseköğretim Kalite Kurulu (YÖKAK) ve Kurumsal Akreditasyon Göstergeleri
          </Text>
        </div>
        <Tag color="blue" style={{ padding: '4px 10px', fontSize: 12 }}>
          2026 Yıllık Raporu Hazır
        </Tag>
      </div>

      <Card style={{ borderRadius: 10 }}>
        <Tabs
          defaultActiveKey="students"
          items={[
            {
              key: 'students',
              label: (
                <Space>
                  <UserOutlined />
                  <span>Öğrenci İstatistikleri</span>
                </Space>
              ),
              children: (
                <div>
                  <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
                    <Col xs={24} sm={12} md={6}>
                      <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                        <Statistic title="Toplam Kayıtlı Öğrenci" value={28450} valueStyle={{ color: '#0f4c81', fontWeight: 700 }} />
                      </Card>
                    </Col>
                    <Col xs={24} sm={12} md={6}>
                      <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                        <Statistic title="Lisans Öğrenci Sayısı" value={21200} valueStyle={{ color: '#10b981', fontWeight: 700 }} />
                      </Card>
                    </Col>
                    <Col xs={24} sm={12} md={6}>
                      <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                        <Statistic title="Lisansüstü (YL / Doktora)" value={7250} valueStyle={{ color: '#6366f1', fontWeight: 700 }} />
                      </Card>
                    </Col>
                    <Col xs={24} sm={12} md={6}>
                      <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                        <Statistic title="Uluslararası Öğrenci" value={3410} suffix="(%12)" valueStyle={{ color: '#ec4899', fontWeight: 700 }} />
                      </Card>
                    </Col>
                  </Row>

                  <Title level={5}>Fakülte Bazında Dağılım</Title>
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                    <div>
                      <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 13 }}>
                        <span>Mühendislik ve Doğa Bilimleri Fakültesi</span>
                        <strong>6,850 Öğrenci (%24)</strong>
                      </div>
                      <Progress percent={24} strokeColor="#0f4c81" />
                    </div>
                    <div>
                      <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 13 }}>
                        <span>İktisadi ve İdari Bilimler Fakültesi</span>
                        <strong>5,420 Öğrenci (%19)</strong>
                      </div>
                      <Progress percent={19} strokeColor="#0284c7" />
                    </div>
                    <div>
                      <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 13 }}>
                        <span>Tıp Fakültesi & Sağlık Bilimleri</span>
                        <strong>4,200 Öğrenci (%15)</strong>
                      </div>
                      <Progress percent={15} strokeColor="#10b981" />
                    </div>
                  </div>
                </div>
              ),
            },
            {
              key: 'alumni',
              label: (
                <Space>
                  <TrophyOutlined />
                  <span>Mezun Takip & İstihdam</span>
                </Space>
              ),
              children: (
                <div>
                  <Row gutter={[16, 16]} style={{ marginBottom: 20 }}>
                    <Col xs={24} sm={8}>
                      <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                        <Statistic title="İlk 6 Ayda İstihdam Oranı" value={91.4} precision={1} suffix="%" valueStyle={{ color: '#10b981', fontWeight: 700 }} />
                      </Card>
                    </Col>
                    <Col xs={24} sm={8}>
                      <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                        <Statistic title="Yurt Dışında Çalışan Mezun" value={14.8} precision={1} suffix="%" valueStyle={{ color: '#0f4c81', fontWeight: 700 }} />
                      </Card>
                    </Col>
                    <Col xs={24} sm={8}>
                      <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                        <Statistic title="Girişimci / Şirket Kurucusu" value={6.2} precision={1} suffix="%" valueStyle={{ color: '#d97706', fontWeight: 700 }} />
                      </Card>
                    </Col>
                  </Row>
                  <Paragraph type="secondary">
                    YÖK Mezun Takip Sistemi verilerine göre üniversitemiz mühendislik programları istihdam sıralamasında ilk 5 içerisinde yer almaktadır.
                  </Paragraph>
                </div>
              ),
            },
            {
              key: 'research',
              label: (
                <Space>
                  <ExperimentOutlined />
                  <span>Yayınlar & BAP Projeleri</span>
                </Space>
              ),
              children: (
                <Row gutter={[16, 16]}>
                  <Col xs={24} sm={8}>
                    <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                      <Statistic title="SCI / Scopus İndeksli Yayın" value={842} valueStyle={{ color: '#0f4c81', fontWeight: 700 }} />
                    </Card>
                  </Col>
                  <Col xs={24} sm={8}>
                    <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                      <Statistic title="Q1 ve Q2 Yayın Oranı" value={68.5} suffix="%" valueStyle={{ color: '#10b981', fontWeight: 700 }} />
                    </Card>
                  </Col>
                  <Col xs={24} sm={8}>
                    <Card bordered={false} style={{ backgroundColor: '#f8fafc' }}>
                      <Statistic title="Aktif BAP & TÜBİTAK Projesi" value={124} valueStyle={{ color: '#6366f1', fontWeight: 700 }} />
                    </Card>
                  </Col>
                </Row>
              ),
            },
          ]}
        />
      </Card>
    </div>
  );
};
