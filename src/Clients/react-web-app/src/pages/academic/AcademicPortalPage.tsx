import React, { useState } from 'react';
import { Card, Table, Tag, Typography, Button, Modal, InputNumber, Space, Tabs, Row, Col, Statistic, message } from 'antd';
import {
  BookOutlined,
  EditOutlined,
  CheckOutlined,
  CloseOutlined,
  UserOutlined,
} from '@ant-design/icons';

const { Title, Text } = Typography;

interface GradeStudent {
  key: string;
  studentNumber: string;
  fullName: string;
  midterm: number;
  finalExam: number;
  letterGrade: string;
}

export const AcademicPortalPage: React.FC = () => {
  const [gradeModalVisible, setGradeModalVisible] = useState<boolean>(false);
  const [selectedCourse, setSelectedCourse] = useState<string>('BLG411 - Bitirme Tasarım Projesi');

  const [students, setStudents] = useState<GradeStudent[]>([
    { key: '1', studentNumber: '20210101042', fullName: 'İsmailcan Uslu', midterm: 92, finalExam: 95, letterGrade: 'AA' },
    { key: '2', studentNumber: '20210101015', fullName: 'Burak Yıldırım', midterm: 78, finalExam: 82, letterGrade: 'BA' },
    { key: '3', studentNumber: '20210101033', fullName: 'Zeynep Aksoy', midterm: 65, finalExam: 70, letterGrade: 'BB' },
    { key: '4', studentNumber: '20210101089', fullName: 'Oğuzhan Çetin', midterm: 45, finalExam: 52, letterGrade: 'CC' },
  ]);

  const [pendingApprovals, setPendingApprovals] = useState([
    {
      key: '1',
      studentNumber: '20210101042',
      studentName: 'İsmailcan Uslu',
      program: 'Bilgisayar Müh. (Ana Dal) & Yazılım Müh. (ÇAP)',
      ects: 30,
      courseCount: 5,
      date: 'Bugün 14:20',
    },
    {
      key: '2',
      studentNumber: '20220101015',
      studentName: 'Elif Kaya',
      program: 'Bilgisayar Müh. (Lisans)',
      ects: 28,
      courseCount: 6,
      date: 'Bugün 11:45',
    },
  ]);

  const calculateGrade = (midterm: number, finalExam: number): string => {
    const avg = midterm * 0.4 + finalExam * 0.6;
    if (avg >= 90) return 'AA';
    if (avg >= 80) return 'BA';
    if (avg >= 70) return 'BB';
    if (avg >= 60) return 'CB';
    if (avg >= 50) return 'CC';
    if (avg >= 40) return 'DC';
    return 'FF';
  };

  const handleGradeChange = (key: string, field: 'midterm' | 'finalExam', value: number | null) => {
    setStudents((prev) =>
      prev.map((s) => {
        if (s.key === key) {
          const m = field === 'midterm' ? (value || 0) : s.midterm;
          const f = field === 'finalExam' ? (value || 0) : s.finalExam;
          return { ...s, [field]: value || 0, letterGrade: calculateGrade(m, f) };
        }
        return s;
      })
    );
  };

  const handleSaveGrades = () => {
    message.success(`${selectedCourse} notları başarıyla sisteme kaydedildi ve OBS veri tabanına işlendi.`);
    setGradeModalVisible(false);
  };

  const handleApproveRegistration = (key: string, studentName: string) => {
    setPendingApprovals(pendingApprovals.filter((a) => a.key !== key));
    message.success(`${studentName} isimli öğrencinin ders kaydı onaylandı.`);
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
            Öğretim Üyesi & Akademik Personel Yönetim Portalı
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Prof. Dr. Ahmet Bilgehan | Bilgisayar Mühendisliği Bölüm Başkanı
          </Text>
        </div>
        <Tag color="geekblue" style={{ padding: '4px 10px', fontSize: 12 }}>
          2026-2027 Güz Dönemi
        </Tag>
      </div>

      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic title="Aktif Verilen Dersler" value={3} suffix="Ders / 5 Şube" valueStyle={{ color: '#0f4c81', fontWeight: 700 }} />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic title="Toplam Öğrenci Sayısı" value={142} suffix="Öğrenci" valueStyle={{ color: '#10b981', fontWeight: 700 }} />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic title="Danışmanlık Verilen Öğrenci" value={28} suffix="Öğrenci" valueStyle={{ color: '#6366f1', fontWeight: 700 }} />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic title="Onay Bekleyen Ders Kaydı" value={pendingApprovals.length} suffix="Talep" valueStyle={{ color: '#ef4444', fontWeight: 700 }} />
          </Card>
        </Col>
      </Row>

      <Card style={{ borderRadius: 10 }}>
        <Tabs
          defaultActiveKey="courses"
          items={[
            {
              key: 'courses',
              label: (
                <Space>
                  <BookOutlined />
                  <span>Verdiğim Dersler & Not Girişi</span>
                </Space>
              ),
              children: (
                <Table
                  dataSource={[
                    { key: '1', code: 'BLG411', name: 'Bitirme Tasarım Projesi', section: 'A', students: 42, hours: 'Pazartesi 09:30-12:20', room: 'Amfi 3' },
                    { key: '2', code: 'BLG452', name: 'Bulut Bilişim ve Mikroservisler', section: 'A', students: 58, hours: 'Salı 13:30-16:20', room: 'Yazılım Lab 4' },
                    { key: '3', code: 'BLG301', name: 'Veritabanı Yönetim Sistemleri', section: 'B', students: 75, hours: 'Çarşamba 10:30-13:20', room: 'Amfi 1' },
                  ]}
                  columns={[
                    { title: 'Ders Kodu', dataIndex: 'code', key: 'code', render: (c: string) => <strong style={{ color: '#0f4c81' }}>{c}</strong> },
                    { title: 'Ders Adı', dataIndex: 'name', key: 'name', render: (n: string) => <span style={{ fontWeight: 500 }}>{n}</span> },
                    { title: 'Şube', dataIndex: 'section', key: 'section', align: 'center' as const, render: (s: string) => <Tag color="blue">{s}</Tag> },
                    { title: 'Kayıtlı Öğrenci', dataIndex: 'students', key: 'students', align: 'center' as const, render: (st: number) => <Tag color="purple">{st} Kişi</Tag> },
                    { title: 'Haftalık Saat', dataIndex: 'hours', key: 'hours' },
                    { title: 'Derslik', dataIndex: 'room', key: 'room' },
                    {
                      title: 'İşlem',
                      key: 'actions',
                      align: 'center' as const,
                      render: (_: unknown, record) => (
                        <Button
                          type="primary"
                          icon={<EditOutlined />}
                          size="small"
                          style={{ backgroundColor: '#0f4c81', borderRadius: 6 }}
                          onClick={() => {
                            setSelectedCourse(`${record.code} - ${record.name}`);
                            setGradeModalVisible(true);
                          }}
                        >
                          Not Girişi
                        </Button>
                      ),
                    },
                  ]}
                  pagination={false}
                />
              ),
            },
            {
              key: 'advisor',
              label: (
                <Space>
                  <UserOutlined />
                  <span>Danışmanlık Onay Masası ({pendingApprovals.length})</span>
                </Space>
              ),
              children: (
                <div>
                  <div style={{ marginBottom: 16 }}>
                    <Text type="secondary">
                      Danışmanı olduğunuz öğrencilerin dönemlik ders kayıt talepleri aşağıda listelenmiştir. Seçimleri inceleyip onaylayabilirsiniz.
                    </Text>
                  </div>
                  <Table
                    dataSource={pendingApprovals}
                    columns={[
                      { title: 'Öğrenci No', dataIndex: 'studentNumber', key: 'studentNumber', render: (n: string) => <strong style={{ color: '#0f4c81' }}>{n}</strong> },
                      { title: 'Öğrenci Adı Soyadı', dataIndex: 'studentName', key: 'studentName', render: (n: string) => <span style={{ fontWeight: 600 }}>{n}</span> },
                      { title: 'Program', dataIndex: 'program', key: 'program', render: (p: string) => <Tag color="geekblue">{p}</Tag> },
                      { title: 'Ders Sayısı', dataIndex: 'courseCount', key: 'courseCount', align: 'center' as const },
                      { title: 'Toplam AKTS', dataIndex: 'ects', key: 'ects', align: 'center' as const, render: (e: number) => <Tag color="purple">{e} AKTS</Tag> },
                      { title: 'Talep Zamanı', dataIndex: 'date', key: 'date' },
                      {
                        title: 'Onay İşlemi',
                        key: 'action',
                        align: 'center' as const,
                        render: (_: unknown, r) => (
                          <Space>
                            <Button
                              type="primary"
                              size="small"
                              icon={<CheckOutlined />}
                              style={{ backgroundColor: '#10b981', borderColor: 'transparent', borderRadius: 6 }}
                              onClick={() => handleApproveRegistration(r.key, r.studentName)}
                            >
                              Onayla
                            </Button>
                            <Button
                              danger
                              size="small"
                              icon={<CloseOutlined />}
                              style={{ borderRadius: 6 }}
                              onClick={() => {
                                setPendingApprovals(pendingApprovals.filter((a) => a.key !== r.key));
                                message.warning(`${r.studentName} isimli öğrencinin kaydı revizyon için iade edildi.`);
                              }}
                            >
                              Revizyon İste
                            </Button>
                          </Space>
                        ),
                      },
                    ]}
                    pagination={false}
                  />
                </div>
              ),
            },
          ]}
        />
      </Card>

      {/* Not Girişi Modalı */}
      <Modal
        title={`Not Girişi: ${selectedCourse}`}
        open={gradeModalVisible}
        onCancel={() => setGradeModalVisible(false)}
        onOk={handleSaveGrades}
        okText="Notları Kaydet ve Sisteme Kilitle"
        cancelText="Vazgeç"
        width={780}
      >
        <div style={{ marginBottom: 12 }}>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Vize (%40) ve Final (%60) notlarını girdiğinizde harf notu YÖK bağıl değerlendirme skalasına göre otomatik hesaplanır.
          </Text>
        </div>
        <Table
          dataSource={students}
          columns={[
            { title: 'Öğrenci No', dataIndex: 'studentNumber', key: 'studentNumber' },
            { title: 'Adı Soyadı', dataIndex: 'fullName', key: 'fullName', render: (n: string) => <strong>{n}</strong> },
            {
              title: 'Ara Sınav (Vize %40)',
              dataIndex: 'midterm',
              key: 'midterm',
              render: (val: number, r) => (
                <InputNumber
                  min={0}
                  max={100}
                  value={val}
                  onChange={(v) => handleGradeChange(r.key, 'midterm', v)}
                />
              ),
            },
            {
              title: 'Final Sınavı (%60)',
              dataIndex: 'finalExam',
              key: 'finalExam',
              render: (val: number, r) => (
                <InputNumber
                  min={0}
                  max={100}
                  value={val}
                  onChange={(v) => handleGradeChange(r.key, 'finalExam', v)}
                />
              ),
            },
            {
              title: 'Harf Notu',
              dataIndex: 'letterGrade',
              key: 'letterGrade',
              align: 'center' as const,
              render: (g: string) => (
                <Tag color={g === 'AA' || g === 'BA' ? 'green' : g === 'FF' ? 'red' : 'blue'} style={{ fontWeight: 700, fontSize: 14 }}>
                  {g}
                </Tag>
              ),
            },
          ]}
          pagination={false}
          size="small"
        />
      </Modal>
    </div>
  );
};
