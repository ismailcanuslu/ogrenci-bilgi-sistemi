import React, { useState, useEffect } from 'react';
import {
  Card,
  Table,
  Button,
  Modal,
  Form,
  Input,
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
  ApartmentOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  SearchOutlined,
  PhoneOutlined,
  MailOutlined,
  EnvironmentOutlined,
  CheckCircleOutlined
} from '@ant-design/icons';
import { 
  getUnits, 
  createUnit, 
  updateUnit, 
  deleteUnit 
} from '../../api/userManagement';
import type { AdminUnitItem } from '../../api/userManagement';

const { Title, Text } = Typography;

export const AdminUnitsPage: React.FC = () => {
  const [units, setUnits] = useState<AdminUnitItem[]>([]);
  const [search, setSearch] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingUnit, setEditingUnit] = useState<AdminUnitItem | null>(null);
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const data = await getUnits();
      setUnits(data);
    } catch {
      message.error('Birimler yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleOpenModal = (unit?: AdminUnitItem) => {
    if (unit) {
      setEditingUnit(unit);
      form.setFieldsValue(unit);
    } else {
      setEditingUnit(null);
      form.resetFields();
      form.setFieldsValue({
        code: `BİRİM-${Math.floor(100 + Math.random() * 900)}`,
        isActive: true
      });
    }
    setIsModalOpen(true);
  };

  const handleSubmit = async (values: any) => {
    try {
      if (editingUnit) {
        await updateUnit(editingUnit.id, values);
        message.success('İdari birim güncellendi.');
      } else {
        await createUnit(values);
        message.success('Yeni idari birim oluşturuldu.');
      }
      setIsModalOpen(false);
      loadData();
    } catch {
      message.error('İşlem kaydedilemedi.');
    }
  };

  const handleDelete = async (id: string, name: string) => {
    try {
      await deleteUnit(id);
      message.success(`"${name}" idari birimi silindi.`);
      loadData();
    } catch {
      message.error('Silme işlemi başarısız.');
    }
  };

  const filteredUnits = units.filter(u => 
    u.name.toLowerCase().includes(search.toLowerCase()) ||
    u.code.toLowerCase().includes(search.toLowerCase()) ||
    (u.managerName && u.managerName.toLowerCase().includes(search.toLowerCase()))
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
            İdari Teşkilat & Birimler Masası
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Daire başkanlıkları, fakülte sekreterlikleri, müdürlükler, birim amirleri ve iletişim noktaları
          </Text>
        </div>

        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#d97706', borderRadius: 8, height: 40 }}
          onClick={() => handleOpenModal()}
        >
          Yeni Birim Tanımla
        </Button>
      </div>

      {/* İstatistikler */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Toplam İdari Birim"
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
              title="Aktif Teşkilat Birimleri"
              value={units.filter(u => u.isActive).length}
              suffix="Aktif"
              prefix={<CheckCircleOutlined style={{ color: '#10b981', marginRight: 6 }} />}
              valueStyle={{ color: '#10b981', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Sorumlu Atanmış Birimler"
              value={units.filter(u => u.managerName).length}
              suffix="Birim Amiri"
              valueStyle={{ color: '#0284c7', fontWeight: 700 }}
            />
          </Card>
        </Col>
      </Row>

      {/* Arama Barı */}
      <Card style={{ borderRadius: 10 }} styles={{ body: { padding: '14px 20px' } }}>
        <Input
          prefix={<SearchOutlined style={{ color: '#94a3b8' }} />}
          placeholder="Birim adı, kodu veya amirine göre ara..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          allowClear
        />
      </Card>

      {/* Birimler Tablosu */}
      <Card style={{ borderRadius: 12 }}>
        <Table
          dataSource={filteredUnits.map(u => ({ ...u, key: u.id }))}
          loading={loading}
          pagination={{ pageSize: 8 }}
          columns={[
            {
              title: 'Birim Kodu',
              dataIndex: 'code',
              key: 'code',
              width: 130,
              render: (code: string) => (
                <Tag color="orange" style={{ fontWeight: 700, fontFamily: 'monospace' }}>
                  {code}
                </Tag>
              ),
            },
            {
              title: 'İdari Birim Adı',
              dataIndex: 'name',
              key: 'name',
              render: (name: string, r: AdminUnitItem) => (
                <div>
                  <strong style={{ fontSize: 14, color: '#0f172a' }}>{name}</strong>
                  {r.location && (
                    <div style={{ fontSize: 12, color: '#64748b' }}>
                      <EnvironmentOutlined style={{ marginRight: 4 }} />
                      {r.location}
                    </div>
                  )}
                </div>
              ),
            },
            {
              title: 'Birim Sorumlusu / Daire Başkanı',
              dataIndex: 'managerName',
              key: 'managerName',
              render: (mgr?: string) => (
                <span style={{ fontWeight: 600, color: mgr ? '#0f4c81' : '#94a3b8' }}>
                  {mgr || 'Atanmamış'}
                </span>
              ),
            },
            {
              title: 'İletişim & Dahili',
              key: 'contact',
              render: (_: unknown, r: AdminUnitItem) => (
                <div style={{ fontSize: 12, color: '#64748b' }}>
                  {r.phone && (
                    <div>
                      <PhoneOutlined style={{ marginRight: 4 }} />
                      {r.phone}
                    </div>
                  )}
                  {r.email && (
                    <div>
                      <MailOutlined style={{ marginRight: 4 }} />
                      {r.email}
                    </div>
                  )}
                  {!r.phone && !r.email && '-'}
                </div>
              ),
            },
            {
              title: 'Durum',
              dataIndex: 'isActive',
              key: 'isActive',
              render: (active: boolean) => (
                <Tag color={active ? 'green' : 'red'}>
                  {active ? 'Aktif Teşkilat' : 'Pasif'}
                </Tag>
              ),
            },
            {
              title: 'İşlemler',
              key: 'actions',
              align: 'right',
              render: (_: unknown, record: AdminUnitItem) => (
                <Space>
                  <Button
                    type="link"
                    size="small"
                    icon={<EditOutlined />}
                    onClick={() => handleOpenModal(record)}
                  >
                    Düzenle
                  </Button>
                  <Popconfirm
                    title="Bu idari birimi silmek istediğinize emin misiniz?"
                    onConfirm={() => handleDelete(record.id, record.name)}
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

      {/* Modal */}
      <Modal
        title={editingUnit ? 'İdari Birimi Düzenle' : 'Yeni İdari Birim Ekle'}
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
        width={560}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Row gutter={16}>
            <Col span={16}>
              <Form.Item
                name="name"
                label={<span style={{ fontWeight: 600 }}>Birim Adı *</span>}
                rules={[{ required: true, message: 'Birim adı giriniz' }]}
              >
                <Input placeholder="Örn: Öğrenci İşleri Daire Başkanlığı" />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item
                name="code"
                label={<span style={{ fontWeight: 600 }}>Birim Kodu *</span>}
                rules={[{ required: true, message: 'Birim kodu giriniz' }]}
              >
                <Input placeholder="OIDB" style={{ textTransform: 'uppercase' }} />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item
            name="managerName"
            label={<span style={{ fontWeight: 600 }}>Birim Sorumlusu / Daire Başkanı</span>}
          >
            <Input placeholder="Örn: Hasan Korkmaz" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="phone"
                label={<span style={{ fontWeight: 600 }}>Dahili Telefon</span>}
              >
                <Input placeholder="+90 (222) 335 05 90" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="email"
                label={<span style={{ fontWeight: 600 }}>Kurumsal E-Posta</span>}
                rules={[{ type: 'email', message: 'Geçerli e-posta giriniz' }]}
              >
                <Input placeholder="oidb@universite.edu.tr" />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item
            name="location"
            label={<span style={{ fontWeight: 600 }}>Yerleşke / Konum Binası</span>}
          >
            <Input placeholder="Rektörlük Binası Zemin Kat, Ayazağa Kampüsü" />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setIsModalOpen(false)}>İptal</Button>
            <Button
              type="primary"
              htmlType="submit"
              style={{ backgroundColor: '#d97706', borderRadius: 6 }}
            >
              {editingUnit ? 'Değişiklikleri Kaydet' : 'Birimi Oluştur'}
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
