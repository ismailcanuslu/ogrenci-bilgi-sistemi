import React, { useState } from 'react';
import { Card, Table, Tag, Typography, Button, Row, Col, message, Modal } from 'antd';
import {
  SendOutlined,
  ExclamationCircleOutlined,
} from '@ant-design/icons';

const { Title, Text } = Typography;

interface CourseOption {
  key: string;
  code: string;
  name: string;
  instructor: string;
  day: string;
  ects: number;
  credit: number;
  capacity: string;
  isCompulsory: boolean;
}

export const CourseRegistrationPage: React.FC = () => {
  const [availableCourses] = useState<CourseOption[]>([
    { key: '1', code: 'BLG411', name: 'Bitirme Tasarım Projesi', instructor: 'Prof. Dr. Ahmet Bilgehan', day: 'Pazartesi 09:30', ects: 8, credit: 3, capacity: '42/50', isCompulsory: true },
    { key: '2', code: 'BLG452', name: 'Bulut Bilişim ve Mikroservisler', instructor: 'Doç. Dr. Selin Doğan', day: 'Salı 13:30', ects: 5, credit: 3, capacity: '58/60', isCompulsory: false },
    { key: '3', code: 'BLG481', name: 'Yapay Zeka ve Derin Öğrenme', instructor: 'Dr. Öğr. Üyesi Cemil Arslan', day: 'Çarşamba 14:00', ects: 5, credit: 3, capacity: '35/50', isCompulsory: false },
    { key: '4', code: 'BLG492', name: 'Siber Güvenlik ve Ağ Savunması', instructor: 'Dr. Canan Eren', day: 'Perşembe 10:30', ects: 6, credit: 3, capacity: '40/45', isCompulsory: false },
    { key: '5', code: 'YZM302', name: 'Yazılım Kalite ve Test (ÇAP Dersi)', instructor: 'Dr. Emre Kurt', day: 'Cuma 09:30', ects: 5, credit: 3, capacity: '25/30', isCompulsory: false },
  ]);

  const [selectedKeys, setSelectedKeys] = useState<string[]>(['1', '2', '5']);
  const [submitted, setSubmitted] = useState<boolean>(false);

  const selectedCourses = availableCourses.filter((c) => selectedKeys.includes(c.key));
  const totalEcts = selectedCourses.reduce((sum, c) => sum + c.ects, 0);
  const maxEcts = 40;

  const handleToggleSelect = (record: CourseOption) => {
    if (submitted) {
      message.warning('Ders kaydınız danışman onayına gönderilmiştir, değişiklik yapılamaz.');
      return;
    }

    if (selectedKeys.includes(record.key)) {
      if (record.isCompulsory) {
        message.error('Zorunlu dersler kayıttan çıkarılamaz.');
        return;
      }
      setSelectedKeys(selectedKeys.filter((k) => k !== record.key));
      message.info(`${record.code} dersten çıkarıldı.`);
    } else {
      if (totalEcts + record.ects > maxEcts) {
        message.error(`Maksimum dönem AKTS sınırı (${maxEcts} AKTS) aşılamaz!`);
        return;
      }
      setSelectedKeys([...selectedKeys, record.key]);
      message.success(`${record.code} ders seçiminize eklendi.`);
    }
  };

  const handleSubmitForApproval = () => {
    Modal.confirm({
      title: 'Ders Kaydını Danışman Onayına Gönder',
      icon: <ExclamationCircleOutlined />,
      content: `Toplam ${selectedCourses.length} ders (${totalEcts} AKTS) danışmanınız Prof. Dr. Ahmet Bilgehan'ın onayına gönderilecektir. Emin misiniz?`,
      okText: 'Evet, Onaya Gönder',
      cancelText: 'Vazgeç',
      onOk: () => {
        setSubmitted(true);
        message.success('Ders kaydınız başarıyla danışman onayına iletildi.');
      },
    });
  };

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
            2026-2027 Güz Dönemi Ders Kayıt & Ekle-Bırak Ekranı
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Ders Seçimi & Danışman Onay Süreci (Kayıt Bitiş: 25 Ekim 2026, 23:59)
          </Text>
        </div>
        <Tag color={submitted ? 'orange' : 'green'} style={{ padding: '6px 12px', fontSize: 13 }}>
          {submitted ? '⏳ Danışman Onayı Bekleniyor' : '🟢 Ders Seçimi Açık'}
        </Tag>
      </div>

      <Row gutter={[20, 20]}>
        {/* Sol: Açılan Dersler */}
        <Col xs={24} lg={16}>
          <Card title="Dönemde Açılan Dersler Listesi" style={{ borderRadius: 10 }}>
            <Table
              dataSource={availableCourses}
              columns={[
                {
                  title: 'Ders Kodu & Adı',
                  dataIndex: 'name',
                  key: 'name',
                  render: (_: string, r: CourseOption) => (
                    <div>
                      <div style={{ fontWeight: 600, color: '#0f4c81' }}>
                        {r.code} {r.isCompulsory && <Tag color="red">Zorunlu</Tag>}
                      </div>
                      <div style={{ fontSize: 13, color: '#334155' }}>{r.name}</div>
                    </div>
                  ),
                },
                { title: 'Öğretim Üyesi', dataIndex: 'instructor', key: 'instructor', render: (val: string) => <Text type="secondary" style={{ fontSize: 12 }}>{val}</Text> },
                { title: 'Gün & Saat', dataIndex: 'day', key: 'day', render: (d: string) => <Tag color="blue">{d}</Tag> },
                { title: 'AKTS', dataIndex: 'ects', key: 'ects', align: 'center' as const, render: (e: number) => <Tag color="purple">{e} AKTS</Tag> },
                { title: 'Kontenjan', dataIndex: 'capacity', key: 'capacity', align: 'center' as const },
                {
                  title: 'İşlem',
                  key: 'action',
                  align: 'center' as const,
                  render: (_: unknown, r: CourseOption) => {
                    const isSelected = selectedKeys.includes(r.key);
                    return (
                      <Button
                        size="small"
                        type={isSelected ? 'default' : 'primary'}
                        danger={isSelected && !r.isCompulsory}
                        disabled={submitted}
                        onClick={() => handleToggleSelect(r)}
                        style={{ borderRadius: 6 }}
                      >
                        {isSelected ? 'Bırak' : 'Ekle +'}
                      </Button>
                    );
                  },
                },
              ]}
              pagination={false}
              size="middle"
            />
          </Card>
        </Col>

        {/* Sağ: Seçilen Dersler Sepeti & Danışman Onayı */}
        <Col xs={24} lg={8}>
          <Card title="Seçtiğiniz Dersler (Ön Kayıt Sepeti)" style={{ borderRadius: 10 }}>
            <div style={{ marginBottom: 16 }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 8, fontSize: 13 }}>
                <span>Toplam Seçilen AKTS:</span>
                <strong>{totalEcts} / {maxEcts} AKTS</strong>
              </div>
              <div style={{ height: 8, backgroundColor: '#f1f5f9', borderRadius: 4, overflow: 'hidden' }}>
                <div style={{ width: `${(totalEcts / maxEcts) * 100}%`, height: '100%', backgroundColor: '#0f4c81' }} />
              </div>
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 10, marginBottom: 20 }}>
              {selectedCourses.map((c) => (
                <div
                  key={c.key}
                  style={{
                    padding: '10px 12px',
                    borderRadius: 8,
                    backgroundColor: '#f8fafc',
                    border: '1px solid #e2e8f0',
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                  }}
                >
                  <div>
                    <div style={{ fontWeight: 600, fontSize: 13, color: '#0f172a' }}>{c.code}</div>
                    <div style={{ fontSize: 11, color: '#64748b' }}>{c.name}</div>
                  </div>
                  <Tag color="purple">{c.ects} AKTS</Tag>
                </div>
              ))}
            </div>

            <Button
              type="primary"
              size="large"
              block
              icon={<SendOutlined />}
              disabled={submitted || selectedCourses.length === 0}
              onClick={handleSubmitForApproval}
              style={{ backgroundColor: submitted ? '#94a3b8' : '#10b981', borderColor: 'transparent', borderRadius: 8, height: 44, fontWeight: 600 }}
            >
              {submitted ? 'Onaya Gönderildi' : 'Danışman Onayına Gönder'}
            </Button>
          </Card>
        </Col>
      </Row>
    </div>
  );
};
