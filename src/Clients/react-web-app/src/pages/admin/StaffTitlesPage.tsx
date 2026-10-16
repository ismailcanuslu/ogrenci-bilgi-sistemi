import React, { useState, useEffect } from 'react';
import {
  Card,
  Table,
  Button,
  Modal,
  Form,
  Input,
  Select,
  InputNumber,
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
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  SearchOutlined,
  IdcardOutlined
} from '@ant-design/icons';
import { 
  getStaffTitles, 
  createStaffTitle, 
  updateStaffTitle, 
  deleteStaffTitle 
} from '../../api/userManagement';
import type { StaffTitleItem } from '../../api/userManagement';

const { Title, Text } = Typography;

export const StaffTitlesPage: React.FC = () => {
  const [titles, setTitles] = useState<StaffTitleItem[]>([]);
  const [search, setSearch] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingTitle, setEditingTitle] = useState<StaffTitleItem | null>(null);
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const data = await getStaffTitles();
      setTitles(data.sort((a, b) => a.order - b.order));
    } catch {
      message.error('Kadro unvanları yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleOpenModal = (titleItem?: StaffTitleItem) => {
    if (titleItem) {
      setEditingTitle(titleItem);
      form.setFieldsValue(titleItem);
    } else {
      setEditingTitle(null);
      form.resetFields();
      form.setFieldsValue({
        code: `KADRO-${Math.floor(10 + Math.random() * 90)}`,
        category: 'Genel İdare',
        order: titles.length + 1,
      });
    }
    setIsModalOpen(true);
  };

  const handleSubmit = async (values: any) => {
    try {
      if (editingTitle) {
        await updateStaffTitle(editingTitle.id, values);
        message.success('Kadro unvanı güncellendi.');
      } else {
        await createStaffTitle(values);
        message.success('Yeni kadro unvanı eklendi.');
      }
      setIsModalOpen(false);
      loadData();
    } catch {
      message.error('İşlem kaydedilemedi.');
    }
  };

  const handleDelete = async (id: string, name: string) => {
    try {
      await deleteStaffTitle(id);
      message.success(`"${name}" unvanı silindi.`);
      loadData();
    } catch {
      message.error('Silme işlemi başarısız.');
    }
  };

  const filteredTitles = titles.filter(t => 
    t.title.toLowerCase().includes(search.toLowerCase()) ||
    t.code.toLowerCase().includes(search.toLowerCase()) ||
    t.category.toLowerCase().includes(search.toLowerCase())
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
            İdari Kadro Unvanları Masası
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Personel atamalarında ve özlük dosyalarında kullanılan resmi hizmet kadroları ve unvan tanımları
          </Text>
        </div>

        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#4f46e5', borderRadius: 8, height: 40 }}
          onClick={() => handleOpenModal()}
        >
          Yeni Unvan Tanımla
        </Button>
      </div>

      {/* İstatistikler */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Tanımlı Kadro Unvanı"
              value={titles.length}
              suffix="Unvan"
              prefix={<IdcardOutlined style={{ color: '#4f46e5', marginRight: 6 }} />}
              valueStyle={{ color: '#4f46e5', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Genel İdare Hizmetleri"
              value={titles.filter(t => t.category === 'Genel İdare').length}
              suffix="Kadro"
              valueStyle={{ color: '#0f4c81', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 10 }}>
            <Statistic
              title="Teknik & Diğer Hizmetler"
              value={titles.filter(t => t.category !== 'Genel İdare').length}
              suffix="Kadro"
              valueStyle={{ color: '#10b981', fontWeight: 700 }}
            />
          </Card>
        </Col>
      </Row>

      {/* Arama Barı */}
      <Card style={{ borderRadius: 10 }} styles={{ body: { padding: '14px 20px' } }}>
        <Input
          prefix={<SearchOutlined style={{ color: '#94a3b8' }} />}
          placeholder="Unvan adı, kodu veya hizmet kategorisine göre ara..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          allowClear
        />
      </Card>

      {/* Tablo */}
      <Card style={{ borderRadius: 12 }}>
        <Table
          dataSource={filteredTitles.map(t => ({ ...t, key: t.id }))}
          loading={loading}
          pagination={{ pageSize: 10 }}
          columns={[
            {
              title: 'Sıra',
              dataIndex: 'order',
              key: 'order',
              width: 80,
              render: (order: number) => (
                <span style={{ fontWeight: 600, color: '#94a3b8' }}>#{order}</span>
              ),
            },
            {
              title: 'Unvan Kodu',
              dataIndex: 'code',
              key: 'code',
              width: 140,
              render: (code: string) => (
                <Tag color="geekblue" style={{ fontWeight: 700, fontFamily: 'monospace' }}>
                  {code}
                </Tag>
              ),
            },
            {
              title: 'Kadro / Unvan Adı',
              dataIndex: 'title',
              key: 'title',
              render: (name: string) => (
                <strong style={{ fontSize: 14, color: '#0f172a' }}>{name}</strong>
              ),
            },
            {
              title: 'Hizmet Sınıfı / Kategori',
              dataIndex: 'category',
              key: 'category',
              render: (cat: string) => {
                const color = cat === 'Genel İdare' ? 'blue' : cat === 'Teknik Hizmetler' ? 'purple' : 'cyan';
                return <Tag color={color}>{cat}</Tag>;
              },
            },
            {
              title: 'İşlemler',
              key: 'actions',
              align: 'right',
              render: (_: unknown, record: StaffTitleItem) => (
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
                    title="Bu unvan tanımını silmek istediğinize emin misiniz?"
                    onConfirm={() => handleDelete(record.id, record.title)}
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
        title={editingTitle ? 'Kadro Unvanını Düzenle' : 'Yeni Kadro Unvanı Tanımla'}
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
        width={500}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item
            name="title"
            label={<span style={{ fontWeight: 600 }}>Kadro Unvan Adı *</span>}
            rules={[{ required: true, message: 'Unvan adı giriniz' }]}
          >
            <Input placeholder="Örn: Bilgisayar İşletmeni" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="code"
                label={<span style={{ fontWeight: 600 }}>Kadro Kodu *</span>}
                rules={[{ required: true, message: 'Kadro kodu giriniz' }]}
              >
                <Input placeholder="BI" style={{ textTransform: 'uppercase' }} />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="order"
                label={<span style={{ fontWeight: 600 }}>Sıralama Önceliği</span>}
                rules={[{ required: true }]}
              >
                <InputNumber style={{ width: '100%' }} min={1} max={99} />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item
            name="category"
            label={<span style={{ fontWeight: 600 }}>Hizmet Sınıfı Kategori</span>}
            rules={[{ required: true }]}
          >
            <Select
              options={[
                { value: 'Genel İdare', label: 'Genel İdare Hizmetleri (GİH)' },
                { value: 'Teknik Hizmetler', label: 'Teknik Hizmetler (TH)' },
                { value: 'Sağlık Hizmetleri', label: 'Sağlık Hizmetleri (SH)' },
                { value: 'Yardımcı Hizmetler', label: 'Yardımcı Hizmetler (YH)' },
              ]}
            />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setIsModalOpen(false)}>İptal</Button>
            <Button
              type="primary"
              htmlType="submit"
              style={{ backgroundColor: '#4f46e5', borderRadius: 6 }}
            >
              {editingTitle ? 'Güncelle' : 'Unvanı Ekle'}
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
