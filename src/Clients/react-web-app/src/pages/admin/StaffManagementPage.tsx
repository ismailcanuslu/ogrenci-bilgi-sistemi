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
  UsergroupAddOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  MailOutlined,
  PhoneOutlined,
  SearchOutlined,
  ApartmentOutlined,
  SafetyCertificateOutlined,
  IdcardOutlined
} from '@ant-design/icons';
import { 
  userManagementApi, 
  getUnits, 
  getStaffTitles, 
  getSystemRoles, 
} from '../../api/userManagement';
import type { 
  AdminUnitItem, 
  StaffTitleItem, 
  SystemRoleItem, 
  StaffItem 
} from '../../api/userManagement';

const { Title, Text } = Typography;

export const StaffManagementPage: React.FC = () => {
  const [staffList, setStaffList] = useState<StaffItem[]>([]);
  const [units, setUnits] = useState<AdminUnitItem[]>([]);
  const [titles, setTitles] = useState<StaffTitleItem[]>([]);
  const [roles, setRoles] = useState<SystemRoleItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [modalOpen, setModalOpen] = useState<boolean>(false);
  const [editingStaff, setEditingStaff] = useState<StaffItem | null>(null);
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const [staffData, unitData, titleData, roleData] = await Promise.all([
        userManagementApi.getStaff(),
        getUnits(),
        getStaffTitles(),
        getSystemRoles()
      ]);
      setStaffList(staffData);
      setUnits(unitData);
      setTitles(titleData);
      setRoles(roleData);
    } catch {
      message.error('Personel ve idari teşkilat verileri yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const filteredStaff = staffList.filter((s) => {
    if (!searchTerm.trim()) return true;
    const q = searchTerm.toLowerCase();
    return (
      s.registrationNumber.toLowerCase().includes(q) ||
      s.fullName.toLowerCase().includes(q) ||
      s.email.toLowerCase().includes(q) ||
      s.unitName.toLowerCase().includes(q) ||
      s.title.toLowerCase().includes(q)
    );
  });

  const handleOpenCreate = () => {
    setEditingStaff(null);
    form.resetFields();
    form.setFieldsValue({
      role: roles.length > 0 ? roles[0].name : 'Öğrenci İşleri Yöneticisi',
      title: titles.length > 0 ? titles[0].title : 'Memur',
      unitName: units.length > 0 ? units[0].name : 'Öğrenci İşleri Daire Başkanlığı',
    });
    setModalOpen(true);
  };

  const handleOpenEdit = (staff: StaffItem) => {
    setEditingStaff(staff);
    form.setFieldsValue({
      ...staff,
      role: staff.role || (roles.length > 0 ? roles[0].name : 'Öğrenci İşleri Personeli')
    });
    setModalOpen(true);
  };

  const handleSave = async (values: any) => {
    try {
      const payload = {
        ...values,
        fullName: `${values.firstName} ${values.lastName}`.trim(),
      };

      if (editingStaff) {
        await userManagementApi.updateStaff(editingStaff.id, payload);
        message.success(`${payload.fullName} personeli güncellendi.`);
      } else {
        await userManagementApi.createStaff(payload);
        message.success(`${payload.fullName} idari personel kadrosuna kaydedildi.`);
      }
      setModalOpen(false);
      loadData();
    } catch {
      message.error('İşlem kaydedilemedi.');
    }
  };

  const handleDelete = async (id: string, name: string) => {
    try {
      await userManagementApi.deleteStaff(id);
      message.success(`${name} kadrodan çıkarıldı.`);
      loadData();
    } catch {
      message.error('Personel silinemedi.');
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
          padding: '20px 24px',
          backgroundColor: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e2e8f0',
          flexWrap: 'wrap',
          gap: 12,
        }}
      >
        <div>
          <Title level={4} style={{ margin: 0, color: '#0f172a', fontWeight: 700 }}>
            İdari Personel Kadrosu Masası
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            İdari birimler, tanımlı unvanlar ve yetki rollerine göre atanmış personel kadrosu yönetimi
          </Text>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#0f4c81', borderRadius: 8, height: 40 }}
          onClick={handleOpenCreate}
        >
          Yeni Personel Tanımla
        </Button>
      </div>

      {/* İstatistikler */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Kayıtlı İdari Personel"
              value={staffList.length}
              suffix="Personel"
              prefix={<UsergroupAddOutlined style={{ color: '#0f4c81', marginRight: 6 }} />}
              valueStyle={{ color: '#0f4c81', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Tanımlı İdari Birimler"
              value={units.length}
              suffix="Birim"
              prefix={<ApartmentOutlined style={{ color: '#d97706', marginRight: 6 }} />}
              valueStyle={{ color: '#d97706', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Yetki & Sistem Rolleri"
              value={roles.length}
              suffix="Rol"
              prefix={<SafetyCertificateOutlined style={{ color: '#7c3aed', marginRight: 6 }} />}
              valueStyle={{ color: '#7c3aed', fontWeight: 700 }}
            />
          </Card>
        </Col>
      </Row>

      {/* Arama Barı */}
      <Card style={{ borderRadius: 10 }} styles={{ body: { padding: '14px 20px' } }}>
        <Input
          prefix={<SearchOutlined style={{ color: '#94a3b8' }} />}
          placeholder="Sicil no, isim, birim veya unvan ara..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          allowClear
        />
      </Card>

      {/* Personel Tablosu */}
      <Card style={{ borderRadius: 12 }}>
        <Table
          dataSource={filteredStaff.map((s) => ({ ...s, key: s.id }))}
          loading={loading}
          pagination={{ pageSize: 8 }}
          columns={[
            {
              title: 'Sicil No',
              dataIndex: 'registrationNumber',
              key: 'registrationNumber',
              width: 140,
              render: (reg: string) => (
                <Tag color="geekblue" style={{ fontWeight: 700, padding: '2px 8px' }}>
                  <IdcardOutlined style={{ marginRight: 4 }} />
                  {reg}
                </Tag>
              ),
            },
            {
              title: 'Adı Soyadı',
              dataIndex: 'fullName',
              key: 'fullName',
              render: (name: string, r: StaffItem) => (
                <div>
                  <strong style={{ fontSize: 14 }}>{name}</strong>
                  <div style={{ fontSize: 12, color: '#64748b' }}>
                    <MailOutlined style={{ marginRight: 4 }} />
                    {r.email}
                  </div>
                </div>
              ),
            },
            {
              title: 'Kadro Unvanı (Tanımlardan)',
              dataIndex: 'title',
              key: 'title',
              render: (t: string) => (
                <Tag color="purple" style={{ fontWeight: 600 }}>
                  {t}
                </Tag>
              ),
            },
            {
              title: 'Bağlı İdari Birim',
              dataIndex: 'unitName',
              key: 'unitName',
              render: (u: string) => (
                <span style={{ fontWeight: 600, color: '#0f172a' }}>{u}</span>
              ),
            },
            {
              title: 'Yetki / Sistem Rolü',
              dataIndex: 'role',
              key: 'role',
              render: (role: string) => (
                <Tag color="cyan" style={{ fontWeight: 600 }}>
                  <SafetyCertificateOutlined style={{ marginRight: 4 }} />
                  {role}
                </Tag>
              ),
            },
            {
              title: 'İletişim Dahili',
              dataIndex: 'phone',
              key: 'phone',
              render: (p?: string) =>
                p ? (
                  <span>
                    <PhoneOutlined style={{ marginRight: 4, color: '#94a3b8' }} />
                    {p}
                  </span>
                ) : (
                  '-'
                ),
            },
            {
              title: 'İşlemler',
              key: 'actions',
              align: 'right',
              render: (_: unknown, record: StaffItem) => (
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
                    title="Personeli kadrodan silmek istediğinize emin misiniz?"
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

      {/* Personel Ekle / Düzenle Modalı */}
      <Modal
        title={editingStaff ? 'İdari Personel Bilgilerini Düzenle' : 'Yeni İdari Personel Tanımla'}
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={null}
        width={600}
      >
        <Form form={form} layout="vertical" onFinish={handleSave}>
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="registrationNumber"
                label={<span style={{ fontWeight: 600 }}>Sicil Numarası</span>}
                rules={[{ required: true, message: 'Sicil no giriniz' }]}
              >
                <Input placeholder="Örn: PER-2026-042" style={{ textTransform: 'uppercase' }} />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="role"
                label={<span style={{ fontWeight: 600 }}>Yetki / Sistem Rolü (Rollerden)</span>}
                rules={[{ required: true, message: 'Sistem rolü seçiniz' }]}
              >
                <Select
                  placeholder="Sistem rolü seçin"
                  options={
                    roles.length > 0
                      ? roles.map((r) => ({
                          value: r.name,
                          label: `${r.name} (${r.code})`,
                        }))
                      : [
                          { value: 'Öğrenci İşleri Yöneticisi', label: 'Öğrenci İşleri Yöneticisi' },
                          { value: 'Sistem Yöneticisi', label: 'Sistem Yöneticisi' },
                        ]
                  }
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
                <Input placeholder="Adı" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="lastName"
                label={<span style={{ fontWeight: 600 }}>Soyadı</span>}
                rules={[{ required: true, message: 'Soyad giriniz' }]}
              >
                <Input placeholder="Soyadı" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="title"
                label={<span style={{ fontWeight: 600 }}>Kadro Unvanı (Unvanlardan)</span>}
                rules={[{ required: true, message: 'Kadro unvanı seçiniz' }]}
              >
                <Select
                  placeholder="Kadro unvanı seçin"
                  options={
                    titles.length > 0
                      ? titles.map((t) => ({
                          value: t.title,
                          label: `${t.title} (${t.category})`,
                        }))
                      : [
                          { value: 'Daire Başkanı', label: 'Daire Başkanı' },
                          { value: 'Şube Müdürü', label: 'Şube Müdürü' },
                          { value: 'Şef', label: 'Şef' },
                          { value: 'Memur', label: 'Memur' },
                          { value: 'Bilgisayar İşletmeni', label: 'Bilgisayar İşletmeni' },
                        ]
                  }
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="unitName"
                label={<span style={{ fontWeight: 600 }}>Bağlı İdari Birim (Birimlerden)</span>}
                rules={[{ required: true, message: 'Birim seçiniz' }]}
              >
                <Select
                  placeholder="İdari birim seçin"
                  options={
                    units.length > 0
                      ? units.map((u) => ({
                          value: u.name,
                          label: `${u.name} [${u.code}]`,
                        }))
                      : [
                          { value: 'Öğrenci İşleri Daire Başkanlığı', label: 'Öğrenci İşleri Daire Başkanlığı' },
                          { value: 'Personel Daire Başkanlığı', label: 'Personel Daire Başkanlığı' },
                          { value: 'Bilgi İşlem Daire Başkanlığı', label: 'Bilgi İşlem Daire Başkanlığı' },
                        ]
                  }
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="email"
                label={<span style={{ fontWeight: 600 }}>E-Posta Adresi</span>}
                rules={[
                  { required: true, message: 'E-posta giriniz' },
                  { type: 'email', message: 'Geçerli bir e-posta giriniz' },
                ]}
              >
                <Input placeholder="ad.soyad@universite.edu.tr" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="phone"
                label={<span style={{ fontWeight: 600 }}>Dahili Telefon</span>}
              >
                <Input placeholder="0 (212) 285 00 00 / 1234" />
              </Form.Item>
            </Col>
          </Row>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setModalOpen(false)}>İptal</Button>
            <Button
              type="primary"
              htmlType="submit"
              style={{ backgroundColor: '#0f4c81', borderRadius: 6 }}
            >
              {editingStaff ? 'Güncellemeleri Kaydet' : 'Personeli Kaydet'}
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
