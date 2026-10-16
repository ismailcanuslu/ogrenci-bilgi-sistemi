import React, { useState } from 'react';
import { Card, Tabs, Table, Tag, Typography, Button, Modal, Space, Descriptions } from 'antd';
import {
  BookOutlined,
  FileSearchOutlined,
  InfoCircleOutlined,
  DownloadOutlined,
} from '@ant-design/icons';

const { Title, Text, Paragraph } = Typography;

export const EducationCatalogPage: React.FC = () => {
  const [selectedCourse, setSelectedCourse] = useState<{
    code: string;
    name: string;
    ects: number;
    credit: number;
    t: number;
    u: number;
    l: number;
    category: string;
    description: string;
    learningOutcomes: string[];
    assessment: { item: string; weight: number }[];
  } | null>(null);

  const curriculumSemester1 = [
    { key: '1', code: 'BLG101', name: 'Programlamaya Giriş (C/C++)', category: 'Zorunlu', t: 3, u: 2, l: 0, credit: 4, ects: 6 },
    { key: '2', code: 'MAT101', name: 'Matematik I (Kalkülüs)', category: 'Zorunlu', t: 4, u: 0, l: 0, credit: 4, ects: 6 },
    { key: '3', code: 'FZK101', name: 'Genel Fizik I', category: 'Zorunlu', t: 3, u: 0, l: 2, credit: 4, ects: 6 },
    { key: '4', code: 'ENG101', name: 'Academic English I', category: 'Zorunlu', t: 3, u: 0, l: 0, credit: 3, ects: 4 },
    { key: '5', code: 'ATA101', name: 'Atatürk İlkeleri ve İnkılap Tarihi I', category: 'Zorunlu', t: 2, u: 0, l: 0, credit: 2, ects: 2 },
    { key: '6', code: 'TRK101', name: 'Türk Dili I', category: 'Zorunlu', t: 2, u: 0, l: 0, credit: 2, ects: 2 },
    { key: '7', code: 'MDBF100', name: 'Mühendisliğe Giriş ve Etik', category: 'Zorunlu', t: 1, u: 0, l: 0, credit: 1, ects: 4 },
  ];

  const columns = [
    {
      title: 'Ders Kodu',
      dataIndex: 'code',
      key: 'code',
      render: (text: string) => <strong style={{ color: '#0f4c81' }}>{text}</strong>,
    },
    {
      title: 'Ders Adı',
      dataIndex: 'name',
      key: 'name',
      render: (text: string) => <span style={{ fontWeight: 500 }}>{text}</span>,
    },
    {
      title: 'Kategori',
      dataIndex: 'category',
      key: 'category',
      render: (cat: string) => <Tag color={cat === 'Zorunlu' ? 'blue' : 'green'}>{cat}</Tag>,
    },
    { title: 'T', dataIndex: 't', key: 't', width: 45, align: 'center' as const },
    { title: 'U', dataIndex: 'u', key: 'u', width: 45, align: 'center' as const },
    { title: 'L', dataIndex: 'l', key: 'l', width: 45, align: 'center' as const },
    { title: 'Kredi', dataIndex: 'credit', key: 'credit', width: 65, align: 'center' as const, render: (c: number) => <strong>{c}</strong> },
    { title: 'AKTS', dataIndex: 'ects', key: 'ects', width: 70, align: 'center' as const, render: (e: number) => <Tag color="purple">{e}</Tag> },
    {
      title: 'İzlence',
      key: 'action',
      render: (_: unknown, record: typeof curriculumSemester1[0]) => (
        <Button
          size="small"
          icon={<FileSearchOutlined />}
          style={{ borderRadius: 6 }}
          onClick={() =>
            setSelectedCourse({
              code: record.code,
              name: record.name,
              ects: record.ects,
              credit: record.credit,
              t: record.t,
              u: record.u,
              l: record.l,
              category: record.category,
              description: `${record.name} dersi, bilgisayar mühendisliği lisans programının temel zorunlu derslerinden biri olup öğrencilere teorik ve uygulamalı yetkinlik kazandırmayı hedefler.`,
              learningOutcomes: [
                'Temel mühendislik ve algoritma kavramlarını anlama',
                'Modern geliştirme ortamlarında problem çözebilme',
                'Analitik düşünme ve ekip içi yazılım geliştirme pratikleri',
              ],
              assessment: [
                { item: 'Ara Sınav (Vize)', weight: 30 },
                { item: 'Ödev ve Laboratuvar Uygulamaları', weight: 20 },
                { item: 'Yarıyıl Sonu Sınavı (Final)', weight: 50 },
              ],
            })
          }
        >
          Ders İzlencesi
        </Button>
      ),
    },
  ];

  return (
    <div>
      {/* Üst Başlık */}
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
            Eğitim Kataloğu & Müfredat Portalı
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Mühendislik ve Doğa Bilimleri Fakültesi | Bilgisayar Mühendisliği Lisans Programı (2024 Müfredatı)
          </Text>
        </div>
        <Button icon={<DownloadOutlined />} style={{ borderRadius: 6 }}>
          Müfredat PDF İndir
        </Button>
      </div>

      {/* Program Tanımı & Yarıyıllar Sekmeleri */}
      <Card style={{ borderRadius: 10 }}>
        <Tabs
          defaultActiveKey="plan"
          items={[
            {
              key: 'plan',
              label: (
                <Space>
                  <BookOutlined />
                  <span>Öğretim Planı (Dönemlik Dersler)</span>
                </Space>
              ),
              children: (
                <div>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
                    <Title level={5} style={{ margin: 0 }}>1. Yarıyıl (Güz Dönemi) Ders Tablosu</Title>
                    <Tag color="geekblue" style={{ fontSize: 13, padding: '4px 10px' }}>
                      Toplam Dönem Kredisi: 22 | Toplam AKTS: 30
                    </Tag>
                  </div>
                  <Table
                    dataSource={curriculumSemester1}
                    columns={columns}
                    pagination={false}
                    bordered
                    size="middle"
                  />
                </div>
              ),
            },
            {
              key: 'profile',
              label: (
                <Space>
                  <InfoCircleOutlined />
                  <span>Program Tanımı & Amaçları</span>
                </Space>
              ),
              children: (
                <Descriptions bordered column={1} size="middle">
                  <Descriptions.Item label="Kazanılan Derece">
                    Bilgisayar Mühendisliği Lisans Diploması (B.Sc. in Computer Engineering)
                  </Descriptions.Item>
                  <Descriptions.Item label="Derecenin Düzeyi">
                    Lisans - TYYÇ 6. Düzey (Bologna First Cycle)
                  </Descriptions.Item>
                  <Descriptions.Item label="Kabul ve Kayıt Koşulları">
                    ÖSYM Yükseköğretim Kurumları Sınavı (YKS) Sayısal puanı veya YÖK yatay/dikey geçiş yönergeleri.
                  </Descriptions.Item>
                  <Descriptions.Item label="Yeterlilik Koşulları">
                    4 yıllık öğrenim süresince toplam 240 AKTS kredisini başarıyla tamamlamak ve 4.00 üzerinden en az 2.00 GANO elde etmek.
                  </Descriptions.Item>
                  <Descriptions.Item label="İstihdam Profilleri">
                    Yazılım mühendisi, bulut mimarı, veri bilimci, siber güvenlik uzmanı, yapay zeka mühendisi ve sistem analisti.
                  </Descriptions.Item>
                  <Descriptions.Item label="Üst Derece Programlarına Geçiş">
                    Yurt içi ve yurt dışı tezli/tezsiz yüksek lisans ve doğrudan doktora programlarına başvuru hakkı.
                  </Descriptions.Item>
                </Descriptions>
              ),
            },
          ]}
        />
      </Card>

      {/* Ders İzlencesi Modal */}
      <Modal
        title={
          <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
            <FileSearchOutlined style={{ color: '#0f4c81' }} />
            <span>
              {selectedCourse?.code} - {selectedCourse?.name} Ders İzlencesi (Syllabus)
            </span>
          </div>
        }
        open={!!selectedCourse}
        onCancel={() => setSelectedCourse(null)}
        footer={[
          <Button key="close" type="primary" onClick={() => setSelectedCourse(null)} style={{ backgroundColor: '#0f4c81' }}>
            Kapat
          </Button>,
        ]}
        width={720}
      >
        {selectedCourse && (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
            <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap' }}>
              <Tag color="blue">Teorik: {selectedCourse.t} Saat</Tag>
              <Tag color="cyan">Uygulama: {selectedCourse.u} Saat</Tag>
              <Tag color="geekblue">Lab: {selectedCourse.l} Saat</Tag>
              <Tag color="orange">Yerel Kredi: {selectedCourse.credit}</Tag>
              <Tag color="purple">AKTS: {selectedCourse.ects}</Tag>
            </div>

            <div>
              <Title level={5} style={{ fontSize: 14 }}>Dersin Tanımı ve Amacı</Title>
              <Paragraph style={{ color: '#475569' }}>{selectedCourse.description}</Paragraph>
            </div>

            <div>
              <Title level={5} style={{ fontSize: 14 }}>Öğrenme Çıktıları</Title>
              <ul style={{ paddingLeft: 20, color: '#475569', fontSize: 13 }}>
                {selectedCourse.learningOutcomes.map((item, idx) => (
                  <li key={idx} style={{ marginBottom: 4 }}>{item}</li>
                ))}
              </ul>
            </div>

            <div>
              <Title level={5} style={{ fontSize: 14 }}>Ölçme ve Değerlendirme</Title>
              <Table
                dataSource={selectedCourse.assessment.map((a, i) => ({ ...a, key: String(i) }))}
                columns={[
                  { title: 'Değerlendirme Kalemi', dataIndex: 'item', key: 'item' },
                  { title: 'Katkı Oranı (%)', dataIndex: 'weight', key: 'weight', render: (w: number) => <strong>%{w}</strong> },
                ]}
                pagination={false}
                size="small"
              />
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};
