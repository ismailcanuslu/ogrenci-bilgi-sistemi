import React, { useEffect, useState } from 'react';
import {
  Card,
  Table,
  Tag,
  Typography,
  Button,
  Modal,
  Form,
  Input,
  Select,
  Space,
  Tabs,
  Row,
  Col,
  Switch,
  message,
} from 'antd';
import {
  UserAddOutlined,
  SolutionOutlined,
  CheckOutlined,
  CloseOutlined,
  CalendarOutlined,
  BankOutlined,
  ApartmentOutlined,
  BookOutlined,
  SettingOutlined,
  UserOutlined,
} from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { academicManagementApi } from '../../api/academicManagement';
import type { StudentListItem, AcademicYearItem } from '../../api/academicManagement';
import { studentApplicationsApi } from '../../api/studentApplications';
import type { StudentApplicationItem } from '../../api/studentApplications';

const { Title, Text } = Typography;

export const StudentAffairsPage: React.FC = () => {
  const navigate = useNavigate();
  const [students, setStudents] = useState<StudentListItem[]>([]);
  const [applications, setApplications] = useState<StudentApplicationItem[]>([]);
  const [academicYears, setAcademicYears] = useState<AcademicYearItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [studentModalOpen, setStudentModalOpen] = useState<boolean>(false);
  const [form] = Form.useForm();

  const loadData = () => {
    setLoading(true);
    Promise.all([
      academicManagementApi.getStudents(),
      studentApplicationsApi.list({ onlyMine: false }),
      academicManagementApi.getAcademicYears(),
    ]).then(([sList, aList, yList]) => {
      setStudents(sList);
      setApplications(aList);
      setAcademicYears(yList);
      setLoading(false);
    });
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleCreateStudent = async (values: {
    studentNumber: string;
    firstName: string;
    lastName: string;
    email: string;
    enrollmentYear: number;
  }) => {
    try {
      await academicManagementApi.createStudent({
        studentNumber: values.studentNumber,
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        departmentId: 'dept-1',
        programId: 'prog-1',
        curriculumVersionId: 'v-1',
        enrollmentYear: values.enrollmentYear,
      });
      message.success('Yeni öğrenci asil kaydı başarıyla oluşturuldu.');
      setStudentModalOpen(false);
      form.resetFields();
      loadData();
    } catch {
      message.error('Öğrenci kaydı oluşturulurken hata meydana geldi.');
    }
  };

  const handleDecideApplication = async (id: string, decision: 'Approved' | 'Rejected') => {
    try {
      await studentApplicationsApi.review(id, decision === 'Approved', 'Öğrenci İşleri komisyon onayı.');
      message.success(`Başvuru kararı işlendi: ${decision === 'Approved' ? 'Onaylandı' : 'Reddedildi'}`);
      loadData();
    } catch {
      message.error('İşlem başarısız oldu.');
    }
  };

  const handleToggleTermRegistration = async (termId: string, open: boolean) => {
    try {
      await academicManagementApi.setTermRegistrationStatus(termId, open);
      message.success(`Ders kayıt dönemi durumu güncellendi: ${open ? 'Açıldı' : 'Kapatıldı'}`);
      loadData();
    } catch {
      message.error('Dönem durumu güncellenemedi.');
    }
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
      {/* Üst Başlık & Hızlı Geçiş Çubuğu */}
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          padding: '16px 24px',
          backgroundColor: '#ffffff',
          borderRadius: 10,
          border: '1px solid #e2e8f0',
          flexWrap: 'wrap',
          gap: 12,
        }}
      >
        <div>
          <Title level={4} style={{ margin: 0, color: '#0f172a', fontWeight: 700 }}>
            Öğrenci İşleri Daire Başkanlığı Portalı
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Kayıt Yönetimi, Öğrenci Başvuru Masası ve Dönemlik Akademik Takvim
          </Text>
        </div>

        <Space wrap>
          <Button
            icon={<UserOutlined />}
            onClick={() => navigate('/users/students')}
          >
            Öğrenci Masası
          </Button>
          <Button
            icon={<UserOutlined />}
            onClick={() => navigate('/users/instructors')}
          >
            Eğitmenler
          </Button>
          <Button
            icon={<BankOutlined />}
            onClick={() => navigate('/affairs/faculties')}
          >
            Fakülteler
          </Button>
          <Button
            icon={<ApartmentOutlined />}
            onClick={() => navigate('/affairs/departments')}
          >
            Bölüm & AKTS
          </Button>
          <Button
            icon={<BookOutlined />}
            onClick={() => navigate('/affairs/courses')}
          >
            Ders Kataloğu
          </Button>
          <Button
            icon={<SettingOutlined />}
            onClick={() => navigate('/settings/institution')}
          >
            Genel Ayarlar
          </Button>
          <Button
            type="primary"
            icon={<UserAddOutlined />}
            style={{ backgroundColor: '#0f4c81', borderRadius: 6 }}
            onClick={() => setStudentModalOpen(true)}
          >
            Yeni Öğrenci Kaydı Yap
          </Button>
        </Space>
      </div>

      <Card style={{ borderRadius: 10 }}>
        <Tabs
          defaultActiveKey="students"
          items={[
            {
              key: 'students',
              label: (
                <Space>
                  <SolutionOutlined />
                  <span>Kayıtlı Öğrenciler ({students.length})</span>
                </Space>
              ),
              children: (
                <Table
                  dataSource={students.map((s) => ({ ...s, key: s.id }))}
                  columns={[
                    { title: 'Öğrenci No', dataIndex: 'studentNumber', key: 'studentNumber', render: (n: string) => <strong style={{ color: '#0f4c81' }}>{n}</strong> },
                    { title: 'Adı Soyadı', dataIndex: 'fullName', key: 'fullName', render: (n: string) => <span style={{ fontWeight: 600 }}>{n}</span> },
                    { title: 'Bölüm & Program', dataIndex: 'departmentName', key: 'departmentName' },
                    { title: 'Kayıt Yılı', dataIndex: 'enrollmentYear', key: 'enrollmentYear', align: 'center' as const },
                    {
                      title: 'GANO',
                      dataIndex: 'gpa',
                      key: 'gpa',
                      align: 'center' as const,
                      render: (g?: number) => (g ? <strong>{g.toFixed(2)}</strong> : '-'),
                    },
                    {
                      title: 'Öğrenim Durumu',
                      dataIndex: 'status',
                      key: 'status',
                      align: 'center' as const,
                      render: (st: string) => (
                        <Tag color={st === 'Active' ? 'green' : 'orange'}>
                          {st === 'Active' ? 'Aktif Öğrenci' : 'Kayıt Donduruldu'}
                        </Tag>
                      ),
                    },
                  ]}
                  loading={loading}
                  pagination={{ pageSize: 8 }}
                />
              ),
            },
            {
              key: 'applications',
              label: (
                <Space>
                  <SolutionOutlined />
                  <span>Öğrenci Başvuru Masası ({applications.filter((a) => a.state === 'Submitted' || a.state === 'UnderReview').length} Bekleyen)</span>
                </Space>
              ),
              children: (
                <Table
                  dataSource={applications.map((a) => ({ ...a, key: a.id }))}
                  columns={[
                    { title: 'Başvuran', dataIndex: 'studentName', key: 'studentName', render: (n: string, r: StudentApplicationItem) => <div><strong>{n}</strong><div style={{ fontSize: 11, color: '#64748b' }}>{r.studentNumber}</div></div> },
                    { title: 'Başvuru Türü', dataIndex: 'type', key: 'type', render: (t: string) => <Tag color="blue">{t}</Tag> },
                    { title: 'Açıklama / Gerekçe', dataIndex: 'details', key: 'details' },
                    { title: 'Tarih', dataIndex: 'createdAt', key: 'createdAt', render: (d: string) => new Date(d).toLocaleDateString('tr-TR') },
                    {
                      title: 'Durum',
                      dataIndex: 'state',
                      key: 'state',
                      render: (st: string) => (
                        <Tag color={st === 'Approved' ? 'green' : st === 'Rejected' ? 'red' : 'gold'}>
                          {st === 'Approved' ? 'Onaylandı' : st === 'Rejected' ? 'Reddedildi' : 'İnceleme Bekliyor'}
                        </Tag>
                      ),
                    },
                    {
                      title: 'İşlemler',
                      key: 'actions',
                      render: (_: unknown, row: StudentApplicationItem) => (
                        (row.state === 'Submitted' || row.state === 'UnderReview') ? (
                          <Space>
                            <Button
                              type="primary"
                              size="small"
                              icon={<CheckOutlined />}
                              style={{ backgroundColor: '#16a34a' }}
                              onClick={() => handleDecideApplication(row.id, 'Approved')}
                            >
                              Onayla
                            </Button>
                            <Button
                              danger
                              size="small"
                              icon={<CloseOutlined />}
                              onClick={() => handleDecideApplication(row.id, 'Rejected')}
                            >
                              Reddet
                            </Button>
                          </Space>
                        ) : <Text type="secondary">Sonuçlandı</Text>
                      ),
                    },
                  ]}
                  loading={loading}
                  pagination={{ pageSize: 8 }}
                />
              ),
            },
            {
              key: 'academicYears',
              label: (
                <Space>
                  <CalendarOutlined />
                  <span>Akademik Takvim ve Kayıt Açma</span>
                </Space>
              ),
              children: (
                <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
                  {academicYears.map((yr) => (
                    <Card key={yr.id} title={yr.name} size="small" style={{ borderRadius: 8 }}>
                      <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
                        {yr.terms.map((t) => (
                          <div
                            key={t.id}
                            style={{
                              display: 'flex',
                              justifyContent: 'space-between',
                              alignItems: 'center',
                              padding: '12px 16px',
                              backgroundColor: '#f8fafc',
                              borderRadius: 8,
                              border: '1px solid #e2e8f0',
                            }}
                          >
                            <div>
                              <strong style={{ fontSize: 14, color: '#0f172a' }}>{t.name}</strong>
                              <div style={{ fontSize: 12, color: '#64748b' }}>
                                Tarih Aralığı: {t.startDate} - {t.endDate}
                              </div>
                            </div>
                            <Space size="large">
                              <span style={{ fontSize: 13, fontWeight: 500 }}>
                                {t.isRegistrationOpen ? <Tag color="green">Kayıtlar Açık</Tag> : <Tag color="default">Kayıtlar Kapalı</Tag>}
                              </span>
                              <Switch
                                checked={t.isRegistrationOpen}
                                onChange={(checked) => handleToggleTermRegistration(t.id, checked)}
                              />
                            </Space>
                          </div>
                        ))}
                      </div>
                    </Card>
                  ))}
                </div>
              ),
            },
          ]}
        />
      </Card>

      {/* Yeni Manuel Öğrenci Kayıt Modalı */}
      <Modal
        title="Yeni Öğrenci Asil Kayıt Formu"
        open={studentModalOpen}
        onCancel={() => setStudentModalOpen(false)}
        footer={null}
        width={560}
      >
        <Form form={form} layout="vertical" onFinish={handleCreateStudent}>
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="studentNumber" label="Öğrenci Numarası" rules={[{ required: true, message: 'Öğrenci no giriniz.' }]}>
                <Input placeholder="Örn: 20260101099" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="enrollmentYear" label="Kayıt Yılı" initialValue={2026} rules={[{ required: true }]}>
                <Input type="number" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="firstName" label="Adı" rules={[{ required: true, message: 'Ad giriniz.' }]}>
                <Input placeholder="Örn: Barış" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="lastName" label="Soyadı" rules={[{ required: true, message: 'Soyad giriniz.' }]}>
                <Input placeholder="Örn: Yılmaz" />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="email" label="Kurumsal E-Posta" rules={[{ required: true, type: 'email' }]}>
            <Input placeholder="ad.soyad@obs.edu.tr" />
          </Form.Item>

          <Form.Item name="department" label="Bölüm" initialValue="Bilgisayar Mühendisliği">
            <Select options={[{ value: 'Bilgisayar Mühendisliği', label: 'Bilgisayar Mühendisliği Bölümü' }]} />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setStudentModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              Öğrenciyi Sisteme Kaydet
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
