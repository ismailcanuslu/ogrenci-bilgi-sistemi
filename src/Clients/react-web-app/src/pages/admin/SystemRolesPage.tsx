import React, { useState, useEffect } from 'react';
import {
  Card,
  Table,
  Button,
  Modal,
  Form,
  Input,
  Checkbox,
  Space,
  Tag,
  Typography,
  message,
  Popconfirm,
  Row,
  Col,
  Statistic
} from 'antd';
import {
  SafetyCertificateOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  SearchOutlined,
  LockOutlined,
  UnlockOutlined
} from '@ant-design/icons';
import { 
  getSystemRoles, 
  createSystemRole, 
  updateSystemRole, 
  deleteSystemRole 
} from '../../api/userManagement';
import type { SystemRoleItem } from '../../api/userManagement';

const { Title, Text } = Typography;
const { TextArea } = Input;

const AVAILABLE_PERMISSIONS = [
  { label: 'Öğrenci Kayıt ve Kabul Belgeleri', value: 'STUDENT_ENROLLMENT' },
  { label: 'Başvuru ve Evrak İnceleme', value: 'APPLICATION_REVIEW' },
  { label: 'Transkript ve Mezuniyet Onayı', value: 'TRANSCRIPT_ISSUE' },
  { label: 'Ders Kataloğu & Müfredat Yönetimi', value: 'COURSE_CATALOG_ADMIN' },
  { label: 'Dönemlik Şube Açımları', value: 'OFFERING_MANAGEMENT' },
  { label: 'Harç & Mali İşlemler', value: 'FINANCIAL_ADMIN' },
  { label: 'Kullanıcı ve Hesap Yönetimi', value: 'USER_MANAGEMENT' },
  { label: 'Akademik Takvim & Genel Sistem Ayarları', value: 'SYSTEM_SETTINGS' },
  { label: 'Tam Sistem Süper Yönetici Yetkisi', value: 'ALL_PERMISSIONS' },
];

export const SystemRolesPage: React.FC = () => {
  const [roles, setRoles] = useState<SystemRoleItem[]>([]);
  const [search, setSearch] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRole, setEditingRole] = useState<SystemRoleItem | null>(null);
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const data = await getSystemRoles();
      setRoles(data);
    } catch {
      message.error('Sistem rolleri yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleOpenModal = (role?: SystemRoleItem) => {
    if (role) {
      setEditingRole(role);
      form.setFieldsValue({
        name: role.name,
        code: role.code,
        description: role.description,
        permissions: role.permissions || [],
      });
    } else {
      setEditingRole(null);
      form.resetFields();
      form.setFieldsValue({
        code: `ROLE_${Math.floor(100 + Math.random() * 900)}`,
        permissions: ['STUDENT_ENROLLMENT'],
      });
    }
    setIsModalOpen(true);
  };

  const handleSubmit = async (values: any) => {
    try {
      if (editingRole) {
        await updateSystemRole(editingRole.id, values);
        message.success('Sistem rolü ve yetkileri güncellendi.');
      } else {
        await createSystemRole(values);
        message.success('Yeni sistem rolü başarıyla tanımlandı.');
      }
      setIsModalOpen(false);
      loadData();
    } catch {
      message.error('İşlem kaydedilemedi.');
    }
  };

  const handleDelete = async (id: string, name: string) => {
    try {
      await deleteSystemRole(id);
      message.success(`"${name}" rolü silindi.`);
      loadData();
    } catch {
      message.error('Silme işlemi başarısız.');
    }
  };

  const filteredRoles = roles.filter(r => 
    r.name.toLowerCase().includes(search.toLowerCase()) ||
    r.code.toLowerCase().includes(search.toLowerCase()) ||
    (r.description && r.description.toLowerCase().includes(search.toLowerCase()))
  );

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
      {/* Header */}
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
            Yetki & Sistem Rolleri Masası
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Kullanıcı ve idari personellere atanabilecek sistem yetki şablonları ve işlem izinleri
          </Text>
        </div>

        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#7c3aed', borderRadius: 8, height: 40 }}
          onClick={() => handleOpenModal()}
        >
          Yeni Sistem Rolü Tanımla
        </Button>
      </div>

      {/* İstatistikler */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Tanımlı Sistem Rolü"
              value={roles.length}
              suffix="Rol Şablonu"
              prefix={<SafetyCertificateOutlined style={{ color: '#7c3aed', marginRight: 6 }} />}
              valueStyle={{ color: '#7c3aed', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Sistem Korumalı Roller"
              value={roles.filter(r => r.isSystemProtected).length}
              suffix="Kritik Rol"
              prefix={<LockOutlined style={{ color: '#0f4c81', marginRight: 6 }} />}
              valueStyle={{ color: '#0f4c81', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Özelleştirilebilir Roller"
              value={roles.filter(r => !r.isSystemProtected).length}
              suffix="Rol"
              prefix={<UnlockOutlined style={{ color: '#10b981', marginRight: 6 }} />}
              valueStyle={{ color: '#10b981', fontWeight: 700 }}
            />
          </Card>
        </Col>
      </Row>

      {/* Arama Barı */}
      <Card style={{ borderRadius: 10 }} styles={{ body: { padding: '14px 20px' } }}>
        <Input
          prefix={<SearchOutlined style={{ color: '#94a3b8' }} />}
          placeholder="Rol adı veya koduna göre ara..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          allowClear
        />
      </Card>

      {/* Roller Tablosu */}
      <Card style={{ borderRadius: 12 }}>
        <Table
          dataSource={filteredRoles.map(r => ({ ...r, key: r.id }))}
          loading={loading}
          pagination={{ pageSize: 8 }}
          columns={[
            {
              title: 'Rol Kodu',
              dataIndex: 'code',
              key: 'code',
              width: 140,
              render: (code: string) => (
                <Tag color="purple" style={{ fontWeight: 700, fontFamily: 'monospace' }}>
                  {code}
                </Tag>
              ),
            },
            {
              title: 'Sistem Rolü Adı & Açıklama',
              key: 'name',
              render: (_: unknown, r: SystemRoleItem) => (
                <div>
                  <strong style={{ fontSize: 14, color: '#0f172a' }}>{r.name}</strong>
                  <div style={{ fontSize: 12, color: '#64748b', marginTop: 2 }}>
                    {r.description}
                  </div>
                </div>
              ),
            },
            {
              title: 'Yetki Kapsamı',
              dataIndex: 'permissions',
              key: 'permissions',
              render: (perms: string[]) => (
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: 4, maxWidth: 360 }}>
                  {(perms || []).map((p, idx) => (
                    <Tag key={idx} color="blue" style={{ fontSize: 11 }}>
                      {p}
                    </Tag>
                  ))}
                </div>
              ),
            },
            {
              title: 'Kullanıcı Sayısı',
              dataIndex: 'userCount',
              key: 'userCount',
              width: 120,
              align: 'center',
              render: (count?: number) => (
                <Tag color="cyan">{count || 0} Kullanıcı</Tag>
              ),
            },
            {
              title: 'İşlemler',
              key: 'actions',
              align: 'right',
              render: (_: unknown, record: SystemRoleItem) => (
                <Space>
                  <Button
                    type="link"
                    size="small"
                    icon={<EditOutlined />}
                    onClick={() => handleOpenModal(record)}
                  >
                    Düzenle
                  </Button>
                  {!record.isSystemProtected ? (
                    <Popconfirm
                      title="Bu sistem rolünü silmek istediğinize emin misiniz?"
                      onConfirm={() => handleDelete(record.id, record.name)}
                      okText="Evet, Sil"
                      cancelText="İptal"
                    >
                      <Button danger type="link" size="small" icon={<DeleteOutlined />}>
                        Sil
                      </Button>
                    </Popconfirm>
                  ) : (
                    <Tag color="default">Korumalı</Tag>
                  )}
                </Space>
              ),
            },
          ]}
        />
      </Card>

      {/* Modal */}
      <Modal
        title={editingRole ? 'Sistem Rolünü Düzenle' : 'Yeni Sistem Rolü Tanımla'}
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
        width={580}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Row gutter={16}>
            <Col span={14}>
              <Form.Item
                name="name"
                label={<span style={{ fontWeight: 600 }}>Rol Adı *</span>}
                rules={[{ required: true, message: 'Rol adı giriniz' }]}
              >
                <Input placeholder="Örn: Bölüm Sekreteri" />
              </Form.Item>
            </Col>
            <Col span={10}>
              <Form.Item
                name="code"
                label={<span style={{ fontWeight: 600 }}>Sistem Kodu *</span>}
                rules={[{ required: true, message: 'Kod giriniz' }]}
              >
                <Input placeholder="ROLE_SEC" style={{ textTransform: 'uppercase' }} />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item
            name="description"
            label={<span style={{ fontWeight: 600 }}>Açıklama / Görev Kapsamı</span>}
          >
            <TextArea rows={2} placeholder="Bu role sahip personellerin yetki kapsamı hakkında not..." />
          </Form.Item>

          <Form.Item
            name="permissions"
            label={<span style={{ fontWeight: 600 }}>Erişim ve İşlem Yetkileri</span>}
            rules={[{ required: true, message: 'En az bir yetki seçiniz' }]}
          >
            <Checkbox.Group options={AVAILABLE_PERMISSIONS} />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setIsModalOpen(false)}>İptal</Button>
            <Button
              type="primary"
              htmlType="submit"
              style={{ backgroundColor: '#7c3aed', borderRadius: 6 }}
            >
              {editingRole ? 'Yetkileri Güncelle' : 'Rolü Oluştur'}
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
