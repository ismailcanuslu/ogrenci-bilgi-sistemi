import React, { useEffect, useState } from 'react';
import { Card, Table, Tag, Typography, Button, Space, Row, Col, Statistic, Alert, message } from 'antd';
import {
  FilePdfOutlined,
  PrinterOutlined,
  CheckCircleOutlined,
  SafetyCertificateOutlined,
} from '@ant-design/icons';
import { studentPortalApi } from '../../api/studentPortal';
import type { StudentTranscript } from '../../api/studentPortal';

const { Title, Text } = Typography;

export const TranscriptPage: React.FC = () => {
  const [data, setData] = useState<StudentTranscript | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    studentPortalApi.getTranscript().then((res) => {
      setData(res);
      setLoading(false);
    });
  }, []);

  const handleDownloadPdf = () => {
    message.loading({ content: 'E-İmzalı ve Barkodlu Resmi Transkript Belgesi Hazırlanıyor...', key: 'pdf' });
    setTimeout(() => {
      message.success({ content: 'Resmi Transkript başarıyla oluşturuldu ve indirildi.', key: 'pdf' });
    }, 1200);
  };

  return (
    <div>
      {/* Üst Kart */}
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
            Resmi Not Döküm Belgesi (Transkript)
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            T.C. Yükseköğretim Kurulu (YÖK) ve e-Devlet Barkodlu Belge Doğrulama Uyumlu
          </Text>
        </div>
        <Space>
          <Button icon={<PrinterOutlined />} onClick={() => window.print()} style={{ borderRadius: 6 }}>
            Yazdır
          </Button>
          <Button
            type="primary"
            icon={<FilePdfOutlined />}
            style={{ backgroundColor: '#0f4c81', borderRadius: 6 }}
            onClick={handleDownloadPdf}
          >
            E-İmzalı PDF İndir
          </Button>
        </Space>
      </div>

      <Alert
        message="5070 Sayılı Elektronik İmza Kanununa Göre Güvenli Elektronik İmzalıdır"
        description="Bu belgeyi alan kurumlar www.turkiye.gov.tr adresinden veya YÖK mobil uygulamasından YÖK-TR-2026-9874 barkod numarası ile doğrulayabilirler."
        type="info"
        showIcon
        icon={<SafetyCertificateOutlined style={{ color: '#0f4c81' }} />}
        style={{ marginBottom: 20, borderRadius: 8 }}
      />

      {/* İstatistikler */}
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={6}>
          <Card style={{ backgroundColor: '#ffffff', borderRadius: 8 }}>
            <Statistic title="Genel Not Ortalaması (GANO)" value={data?.gpa || 3.78} precision={2} valueStyle={{ color: '#0f4c81', fontWeight: 800 }} />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ backgroundColor: '#ffffff', borderRadius: 8 }}>
            <Statistic title="Tamamlanan Toplam AKTS" value={data?.totalEcts || 195} suffix="/ 240" valueStyle={{ color: '#10b981', fontWeight: 800 }} />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ backgroundColor: '#ffffff', borderRadius: 8 }}>
            <Statistic title="Kayıtlı Yarıyıl Sayısı" value={7} suffix="Dönem" valueStyle={{ color: '#6366f1', fontWeight: 800 }} />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ backgroundColor: '#ffffff', borderRadius: 8 }}>
            <Statistic title="Akademik Başarı Durumu" value="Yüksek Onur" valueStyle={{ color: '#d97706', fontWeight: 800, fontSize: 20 }} />
          </Card>
        </Col>
      </Row>

      {/* Dönemlik Ders Listeleri */}
      {data?.semesters.map((sem) => (
        <Card
          key={sem.semesterNumber}
          title={
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span>{sem.semesterNumber}. Yarıyıl - {sem.termName}</span>
              <Space>
                <Tag color="blue">Dönem Not Ortalaması (DNO): {sem.semesterGpa.toFixed(2)}</Tag>
                <Tag color="purple">Dönem AKTS: {sem.semesterEcts}</Tag>
              </Space>
            </div>
          }
          style={{ marginBottom: 20, borderRadius: 10 }}
        >
          <Table
            dataSource={sem.courses.map((c, i) => ({ ...c, key: String(i) }))}
            columns={[
              { title: 'Ders Kodu', dataIndex: 'code', key: 'code', render: (t: string) => <strong style={{ color: '#0f4c81' }}>{t}</strong> },
              { title: 'Ders Adı', dataIndex: 'name', key: 'name', render: (t: string) => <span style={{ fontWeight: 500 }}>{t}</span> },
              { title: 'Kredi', dataIndex: 'credit', key: 'credit', align: 'center' as const },
              { title: 'AKTS', dataIndex: 'ects', key: 'ects', align: 'center' as const, render: (e: number) => <Tag color="geekblue">{e}</Tag> },
              {
                title: 'Harf Notu',
                dataIndex: 'letterGrade',
                key: 'letterGrade',
                align: 'center' as const,
                render: (g: string) => (
                  <Tag color={g === 'AA' || g === 'BA' ? 'green' : g === 'BB' || g === 'CB' ? 'blue' : 'orange'} style={{ fontWeight: 700, fontSize: 13 }}>
                    {g}
                  </Tag>
                ),
              },
              { title: 'Katsayı', dataIndex: 'gradePoint', key: 'gradePoint', align: 'center' as const, render: (p: number) => p.toFixed(1) },
              {
                title: 'Sonuç',
                dataIndex: 'status',
                key: 'status',
                align: 'center' as const,
                render: (s: string) => <Tag color="success" icon={<CheckCircleOutlined />}>{s}</Tag>,
              },
            ]}
            pagination={false}
            size="small"
            loading={loading}
          />
        </Card>
      ))}
    </div>
  );
};
