import React, { useEffect, useState } from 'react';
import {
  Card,
  Table,
  Button,
  Modal,
  Form,
  Input,
  Select,
  Tag,
  Typography,
  message,
  Row,
  Col,
  Statistic,
} from 'antd';
import {
  UserAddOutlined,
  IdcardOutlined,
  CheckCircleOutlined,
  PauseCircleOutlined,
  SearchOutlined,
} from '@ant-design/icons';
import { academicManagementApi } from '../../api/academicManagement';
import type { StudentListItem, DepartmentItem } from '../../api/academicManagement';

const { Title, Text } = Typography;

export const StudentsManagementPage: React.FC = () => {
  const [students, setStudents] = useState<StudentListItem[]>([]);
  const [departments, setDepartments] = useState<DepartmentItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [modalOpen, setModalOpen] = useState<boolean>(false);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [statusFilter, setStatusFilter] = useState<string>('ALL');
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const [sList, dList] = await Promise.all([
        academicManagementApi.getStudents(),
        academicManagementApi.getDepartments(),
      ]);
      setStudents(sList);
      setDepartments(dList);
    } catch {
      message.error('Öğrenci verileri yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const filteredStudents = students.filter((s) => {
    if (statusFilter !== 'ALL' && s.status !== statusFilter) return false;
    if (searchTerm.trim()) {
      const q = searchTerm.toLowerCase();
      const matchNo = s.studentNumber.toLowerCase().includes(q);
      const matchName = s.fullName.toLowerCase().includes(q);
      const matchEmail = s.email.toLowerCase().includes(q);
      const matchDept = s.departmentName.toLowerCase().includes(q);
      if (!matchNo && !matchName && !matchEmail && !matchDept) return false;
    }
    return true;
  });

  const handleCreateStudent = async (values: {
    studentNumber: string;
    firstName: string;
    lastName: string;
    email: string;
    departmentId: string;
    enrollmentYear: number;
  }) => {
    try {
      const dept = departments.find((d) => d.id === values.departmentId);
      await academicManagementApi.createStudent({
        studentNumber: values.studentNumber,
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        departmentId: values.departmentId,
        programId: 'prog-1',
        curriculumVersionId: 'v-1',
        enrollmentYear: values.enrollmentYear,
      });

      // Yerel listeye de anında yansıt
      const newStudent: StudentListItem = {
        id: `stu-${Date.now()}`,
        studentNumber: values.studentNumber,
        firstName: values.firstName,
        lastName: values.lastName,
        fullName: `${values.firstName} ${values.lastName}`.trim(),
        email: values.email,
        departmentName: dept ? dept.name : 'Bilinmeyen Bölüm',
        programName: dept ? `${dept.name} (Lisans)` : 'Lisans Programı',
        enrollmentYear: values.enrollmentYear,
        status: 'Active',
        gpa: 4.00,
      };

      setStudents((prev) => [newStudent, ...prev]);
      message.success(`${values.firstName} ${values.lastName} isimli asil öğrenci kaydı oluşturuldu.`);
      setModalOpen(false);
      form.resetFields();
    } catch {
      message.error('Öğrenci kaydedilirken hata oluştu.');
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
          flexWrap: 'wrap',
          gap: 12,
        }}
      >
        <div>
          <Title level={4} style={{ margin: 0, color: '#0f172a', fontWeight: 700 }}>
            Öğrenci Yönetimi Masası
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Kayıtlı asil öğrenciler, yeni öğrenci kabul işlemleri, GANO ve öğrenim durumu takibi
          </Text>
        </div>
        <Button
          type="primary"
          icon={<UserAddOutlined />}
          style={{ backgroundColor: '#0f4c81', borderRadius: 6, height: 40 }}
          onClick={() => {
            form.resetFields();
            form.setFieldsValue({
              enrollmentYear: 2026,
              departmentId: departments.length > 0 ? departments[0].id : undefined,
            });
            setModalOpen(true);
          }}
        >
          Yeni Öğrenci Kaydet
        </Button>
      </div>

      {/* İstatistikler */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Toplam Kayıtlı Öğrenci"
              value={students.length}
              suffix="Öğrenci"
              prefix={<IdcardOutlined style={{ color: '#0f4c81', marginRight: 6 }} />}
              valueStyle={{ color: '#0f4c81', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Aktif Öğrenim Durumu"
              value={students.filter((s) => s.status === 'Active').length}
              suffix="Aktif"
              prefix={<CheckCircleOutlined style={{ color: '#16a34a', marginRight: 6 }} />}
              valueStyle={{ color: '#16a34a', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Kayıt Donduranlar"
              value={students.filter((s) => s.status === 'Suspended').length}
              suffix="Dondurulmuş"
              prefix={<PauseCircleOutlined style={{ color: '#d97706', marginRight: 6 }} />}
              valueStyle={{ color: '#d97706', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Ortalama GANO Başarısı"
              value={3.45}
              precision={2}
              suffix="/ 4.00"
              valueStyle={{ color: '#6366f1', fontWeight: 700 }}
            />
          </Card>
        </Col>
      </Row>

      {/* Filtre ve Arama Barı */}
      <Card style={{ borderRadius: 8 }} styles={{ body: { padding: '14px 20px' } }}>
        <Row gutter={[16, 16]} align="middle">
          <Col xs={24} md={16}>
            <Input
              prefix={<SearchOutlined style={{ color: '#94a3b8' }} />}
              placeholder="Öğrenci numarası, adı soyadı veya e-posta ara..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              allowClear
              size="middle"
            />
          </Col>
          <Col xs={24} md={8}>
            <Select
              value={statusFilter}
              onChange={(val) => setStatusFilter(val)}
              style={{ width: '100%' }}
              options={[
                { value: 'ALL', label: 'Tüm Öğrenim Durumları' },
                { value: 'Active', label: 'Yalnızca Aktif Öğrenciler' },
                { value: 'Suspended', label: 'Kayıt Dondurulmuş' },
                { value: 'Graduated', label: 'Mezun' },
              ]}
            />
          </Col>
        </Row>
      </Card>

      {/* Öğrenci Listesi Tablosu */}
      <Card style={{ borderRadius: 10 }}>
        <Table
          dataSource={filteredStudents.map((s) => ({ ...s, key: s.id }))}
          loading={loading}
          pagination={{ pageSize: 8 }}
          columns={[
            {
              title: 'Öğrenci No',
              dataIndex: 'studentNumber',
              key: 'studentNumber',
              render: (n: string) => <strong style={{ color: '#0f4c81', fontSize: 13 }}>{n}</strong>,
            },
            {
              title: 'Adı Soyadı',
              dataIndex: 'fullName',
              key: 'fullName',
              render: (n: string, r: StudentListItem) => (
                <div>
                  <strong style={{ fontSize: 14 }}>{n}</strong>
                  <div style={{ fontSize: 11, color: '#64748b' }}>{r.email}</div>
                </div>
              ),
            },
            {
              title: 'Bölüm & Program',
              dataIndex: 'departmentName',
              key: 'departmentName',
              render: (d: string, r: StudentListItem) => (
                <div>
                  <div style={{ fontWeight: 600 }}>{d}</div>
                  <div style={{ fontSize: 11, color: '#64748b' }}>{r.programName}</div>
                </div>
              ),
            },
            {
              title: 'Kayıt Yılı',
              dataIndex: 'enrollmentYear',
              key: 'enrollmentYear',
              align: 'center',
            },
            {
              title: 'GANO',
              dataIndex: 'gpa',
              key: 'gpa',
              align: 'center',
              render: (g?: number) => (
                <Tag color={g && g >= 3.5 ? 'green' : g && g >= 3.0 ? 'blue' : 'default'} style={{ fontWeight: 700 }}>
                  {g ? g.toFixed(2) : '-'}
                </Tag>
              ),
            },
            {
              title: 'Öğrenim Durumu',
              dataIndex: 'status',
              key: 'status',
              align: 'center',
              render: (st: string) => (
                <Tag color={st === 'Active' ? 'green' : st === 'Suspended' ? 'orange' : 'blue'}>
                  {st === 'Active' ? 'Aktif Öğrenci' : st === 'Suspended' ? 'Kayıt Donduruldu' : 'Mezun'}
                </Tag>
              ),
            },
          ]}
        />
      </Card>

      {/* Yeni Öğrenci Kayıt Modalı */}
      <Modal
        title="Yeni Asil Öğrenci Kaydı"
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={null}
        width={580}
      >
        <Form form={form} layout="vertical" onFinish={handleCreateStudent}>
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="studentNumber"
                label={<span style={{ fontWeight: 600 }}>Öğrenci Numarası</span>}
                rules={[{ required: true, message: 'Öğrenci no giriniz' }]}
              >
                <Input placeholder="Örn: 20260101099" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="enrollmentYear"
                label={<span style={{ fontWeight: 600 }}>Kayıt Yılı</span>}
                rules={[{ required: true }]}
              >
                <Input type="number" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="firstName"
                label={<span style={{ fontWeight: 600 }}>Adı</span>}
                rules={[{ required: true, message: 'Ad giriniz' }]}
              >
                <Input placeholder="Örn: Barış" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="lastName"
                label={<span style={{ fontWeight: 600 }}>Soyadı</span>}
                rules={[{ required: true, message: 'Soyad giriniz' }]}
              >
                <Input placeholder="Örn: Yılmaz" />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item
            name="email"
            label={<span style={{ fontWeight: 600 }}>Kurumsal Öğrenci E-Postası</span>}
            rules={[{ required: true, type: 'email', message: 'Geçerli e-posta giriniz' }]}
          >
            <Input placeholder="ad.soyad@obs.edu.tr" />
          </Form.Item>

          <Form.Item
            name="departmentId"
            label={<span style={{ fontWeight: 600 }}>Yerleştiği Bölüm</span>}
            rules={[{ required: true, message: 'Bölüm seçiniz' }]}
          >
            <Select
              options={departments.map((d) => ({
                value: d.id,
                label: `${d.code} - ${d.name} (${d.facultyName || ''})`,
              }))}
              placeholder="Bölüm seçiniz"
            />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 20 }}>
            <Button onClick={() => setModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              Öğrenciyi Sisteme Kaydet
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
