import React, { useEffect, useState } from 'react';
import {
  Card,
  Table,
  Button,
  Modal,
  Form,
  Input,
  Select,
  Space,
  Tag,
  Typography,
  message,
  Popconfirm,
  Row,
  Col,
  Statistic,
} from 'antd';
import {
  GlobalOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  SearchOutlined,
  CheckCircleOutlined,
  ClockCircleOutlined,
} from '@ant-design/icons';
import { userManagementApi } from '../../api/userManagement';
import type { GuestStudentItem } from '../../api/userManagement';
import { academicManagementApi } from '../../api/academicManagement';
import type { FacultyItem, DepartmentItem } from '../../api/academicManagement';

const { Title, Text } = Typography;

export const GuestStudentsPage: React.FC = () => {
  const [guests, setGuests] = useState<GuestStudentItem[]>([]);
  const [faculties, setFaculties] = useState<FacultyItem[]>([]);
  const [departments, setDepartments] = useState<DepartmentItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [modalOpen, setModalOpen] = useState<boolean>(false);
  const [editingGuest, setEditingGuest] = useState<GuestStudentItem | null>(null);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [programFilter, setProgramFilter] = useState<string>('ALL');
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const [gList, fList, dList] = await Promise.all([
        userManagementApi.getGuestStudents(),
        academicManagementApi.getFaculties(),
        academicManagementApi.getDepartments(),
      ]);
      setGuests(gList);
      setFaculties(fList);
      setDepartments(dList);
    } catch {
      message.error('Misafir öğrenci listesi yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const filteredGuests = guests.filter((g) => {
    if (programFilter !== 'ALL' && g.exchangeProgram !== programFilter) return false;
    if (searchTerm.trim()) {
      const q = searchTerm.toLowerCase();
      return (
        g.guestNumber.toLowerCase().includes(q) ||
        g.fullName.toLowerCase().includes(q) ||
        g.originUniversity.toLowerCase().includes(q) ||
        g.hostDepartmentName.toLowerCase().includes(q)
      );
    }
    return true;
  });

  const handleOpenCreate = () => {
    setEditingGuest(null);
    form.resetFields();
    form.setFieldsValue({
      exchangeProgram: 'Erasmus+',
      term: 'Güz',
      academicYear: '2026-2027',
      status: 'Active',
      hostFacultyName: faculties.length > 0 ? faculties[0].name : '',
      hostDepartmentName: departments.length > 0 ? departments[0].name : '',
    });
    setModalOpen(true);
  };

  const handleOpenEdit = (guest: GuestStudentItem) => {
    setEditingGuest(guest);
    form.setFieldsValue(guest);
    setModalOpen(true);
  };

  const handleSave = async (values: any) => {
    try {
      if (editingGuest) {
        await userManagementApi.updateGuestStudent(editingGuest.id, values);
        message.success(`${values.firstName} ${values.lastName} misafir kaydı güncellendi.`);
      } else {
        await userManagementApi.createGuestStudent(values);
        message.success(`${values.firstName} ${values.lastName} sisteme misafir/değişim öğrencisi olarak kaydedildi.`);
      }
      setModalOpen(false);
      loadData();
    } catch {
      message.error('İşlem kaydedilemedi.');
    }
  };

  const handleDelete = async (id: string, name: string) => {
    try {
      await userManagementApi.deleteGuestStudent(id);
      message.success(`${name} misafir kaydı silindi.`);
      loadData();
    } catch {
      message.error('Kayıt silinemedi.');
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
            Misafir & Değişim Öğrencileri Masası
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Erasmus+, Farabi, Mevlana ve Özel Öğrenci statüsündeki misafir akademik kayıtlar
          </Text>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#0f4c81', borderRadius: 6, height: 40 }}
          onClick={handleOpenCreate}
        >
          Yeni Misafir Öğrenci Ekle
        </Button>
      </div>

      {/* İstatistikler */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Toplam Misafir Öğrenci"
              value={guests.length}
              suffix="Öğrenci"
              prefix={<GlobalOutlined style={{ color: '#0f4c81', marginRight: 6 }} />}
              valueStyle={{ color: '#0f4c81', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Erasmus+ Değişim"
              value={guests.filter((g) => g.exchangeProgram === 'Erasmus+').length}
              suffix="Öğrenci"
              valueStyle={{ color: '#0284c7', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Farabi & Özel Öğrenci"
              value={guests.filter((g) => g.exchangeProgram === 'Farabi' || g.exchangeProgram === 'Özel Öğrenci').length}
              suffix="Öğrenci"
              valueStyle={{ color: '#10b981', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Aktif Dönem Kaydı"
              value={guests.filter((g) => g.status === 'Active').length}
              suffix="Öğrenci"
              valueStyle={{ color: '#7c3aed', fontWeight: 700 }}
            />
          </Card>
        </Col>
      </Row>

      {/* Filtre ve Arama */}
      <Card style={{ borderRadius: 8 }} styles={{ body: { padding: '14px 20px' } }}>
        <Row gutter={[16, 16]} align="middle">
          <Col xs={24} md={16}>
            <Input
              prefix={<SearchOutlined style={{ color: '#94a3b8' }} />}
              placeholder="Misafir no, isim, geldiği üniversite veya bölüm ara..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              allowClear
            />
          </Col>
          <Col xs={24} md={8}>
            <Select
              value={programFilter}
              onChange={(val) => setProgramFilter(val)}
              style={{ width: '100%' }}
              options={[
                { value: 'ALL', label: 'Tüm Değişim Programları' },
                { value: 'Erasmus+', label: 'Erasmus+ (Avrupa)' },
                { value: 'Farabi', label: 'Farabi (Ulusal)' },
                { value: 'Mevlana', label: 'Mevlana (Uluslararası)' },
                { value: 'Özel Öğrenci', label: 'Özel Öğrenci' },
              ]}
            />
          </Col>
        </Row>
      </Card>

      {/* Misafir Öğrenciler Tablosu */}
      <Card style={{ borderRadius: 10 }}>
        <Table
          dataSource={filteredGuests.map((g) => ({ ...g, key: g.id }))}
          loading={loading}
          pagination={{ pageSize: 8 }}
          columns={[
            {
              title: 'Misafir No',
              dataIndex: 'guestNumber',
              key: 'guestNumber',
              width: 140,
              render: (no: string) => <Tag color="cyan" style={{ fontWeight: 700 }}>{no}</Tag>,
            },
            {
              title: 'Adı Soyadı',
              dataIndex: 'fullName',
              key: 'fullName',
              render: (name: string, r: GuestStudentItem) => (
                <div>
                  <strong style={{ fontSize: 14 }}>{name}</strong>
                  <div style={{ fontSize: 11, color: '#64748b' }}>{r.email}</div>
                </div>
              ),
            },
            {
              title: 'Program',
              dataIndex: 'exchangeProgram',
              key: 'exchangeProgram',
              render: (prog: string) => {
                const color = prog === 'Erasmus+' ? 'blue' : prog === 'Farabi' ? 'green' : 'purple';
                return <Tag color={color} style={{ fontWeight: 600 }}>{prog}</Tag>;
              },
            },
            {
              title: 'Geldiği Üniversite / Kurum',
              dataIndex: 'originUniversity',
              key: 'originUniversity',
              render: (uni: string) => <strong>{uni}</strong>,
            },
            {
              title: 'Misafir Olduğu Bölüm',
              dataIndex: 'hostDepartmentName',
              key: 'hostDepartmentName',
              render: (dept: string, r: GuestStudentItem) => (
                <div>
                  <div style={{ fontWeight: 600, color: '#0f4c81' }}>{dept}</div>
                  <div style={{ fontSize: 11, color: '#64748b' }}>{r.hostFacultyName}</div>
                </div>
              ),
            },
            {
              title: 'Dönem',
              key: 'termInfo',
              render: (_: unknown, r: GuestStudentItem) => (
                <span>
                  {r.academicYear} / {r.term}
                </span>
              ),
            },
            {
              title: 'Durum',
              dataIndex: 'status',
              key: 'status',
              align: 'center',
              render: (st: string) => (
                <Tag
                  icon={st === 'Active' ? <CheckCircleOutlined /> : <ClockCircleOutlined />}
                  color={st === 'Active' ? 'green' : 'gold'}
                >
                  {st === 'Active' ? 'Aktif Misafir' : 'Onay Bekliyor'}
                </Tag>
              ),
            },
            {
              title: 'İşlemler',
              key: 'actions',
              align: 'right',
              render: (_: unknown, record: GuestStudentItem) => (
                <Space>
                  <Button
                    type="link"
                    size="small"
                    icon={<EditOutlined />}
                    onClick={() => handleOpenEdit(record)}
                  >
                    Düzenle
                  </Button>
                  <Popconfirm
                    title="Misafir öğrenci kaydını silmek istediğinize emin misiniz?"
                    onConfirm={() => handleDelete(record.id, record.fullName)}
                    okText="Evet, Sil"
                    cancelText="İptal"
                  >
                    <Button danger type="link" size="small" icon={<DeleteOutlined />}>
                      Sil
                    </Button>
                  </Popconfirm>
                </Space>
              ),
            },
          ]}
        />
      </Card>

      {/* Misafir Öğrenci Ekle / Düzenle Modalı */}
      <Modal
        title={editingGuest ? 'Misafir Öğrenci Bilgilerini Düzenle' : 'Yeni Misafir / Değişim Öğrencisi Kaydı'}
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={null}
        width={620}
      >
        <Form form={form} layout="vertical" onFinish={handleSave}>
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="guestNumber"
                label={<span style={{ fontWeight: 600 }}>Misafir Numarası</span>}
                rules={[{ required: true, message: 'Misafir no giriniz' }]}
              >
                <Input placeholder="Örn: M2026ER009" style={{ textTransform: 'uppercase' }} />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="exchangeProgram"
                label={<span style={{ fontWeight: 600 }}>Değişim Programı Türü</span>}
                rules={[{ required: true }]}
              >
                <Select
                  options={[
                    { value: 'Erasmus+', label: 'Erasmus+ (Avrupa Değişimi)' },
                    { value: 'Farabi', label: 'Farabi (Yurtiçi Değişimi)' },
                    { value: 'Mevlana', label: 'Mevlana (Uluslararası)' },
                    { value: 'Özel Öğrenci', label: 'Özel Öğrenci Statüsü' },
                    { value: 'Serbest Değişim', label: 'Serbest Değişim (Protokollü)' },
                  ]}
                />
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
                <Input placeholder="Örn: Lukas" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="lastName"
                label={<span style={{ fontWeight: 600 }}>Soyadı</span>}
                rules={[{ required: true, message: 'Soyad giriniz' }]}
              >
                <Input placeholder="Örn: Müller" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="email"
                label={<span style={{ fontWeight: 600 }}>İletişim E-Postası</span>}
                rules={[{ required: true, type: 'email', message: 'Geçerli e-posta giriniz' }]}
              >
                <Input placeholder="lukas.muller@uni-heidelberg.de" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="originUniversity"
                label={<span style={{ fontWeight: 600 }}>Geldiği Üniversite / Kurum</span>}
                rules={[{ required: true, message: 'Geldiği üniversiteyi giriniz' }]}
              >
                <Input placeholder="Örn: Heidelberg Üniversitesi (Almanya)" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="hostFacultyName"
                label={<span style={{ fontWeight: 600 }}>Misafir Edileceği Fakülte</span>}
                rules={[{ required: true }]}
              >
                <Select
                  options={faculties.map((f) => ({ value: f.name, label: f.name }))}
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="hostDepartmentName"
                label={<span style={{ fontWeight: 600 }}>Misafir Edileceği Bölüm</span>}
                rules={[{ required: true }]}
              >
                <Select
                  options={departments.map((d) => ({ value: d.name, label: d.name }))}
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={8}>
              <Form.Item name="academicYear" label="Akademik Yıl" initialValue="2026-2027">
                <Input placeholder="2026-2027" />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item name="term" label="Değişim Süresi" initialValue="Güz">
                <Select
                  options={[
                    { value: 'Güz', label: 'Güz Dönemi' },
                    { value: 'Bahar', label: 'Bahar Dönemi' },
                    { value: 'Tam Yıl', label: 'Tam Akademik Yıl' },
                  ]}
                />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item name="status" label="Kayıt Durumu" initialValue="Active">
                <Select
                  options={[
                    { value: 'Active', label: 'Aktif' },
                    { value: 'Pending', label: 'Onay Bekliyor' },
                    { value: 'Completed', label: 'Tamamlandı' },
                  ]}
                />
              </Form.Item>
            </Col>
          </Row>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 20 }}>
            <Button onClick={() => setModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              {editingGuest ? 'Güncellemeleri Kaydet' : 'Misafir Öğrenciyi Kaydet'}
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
