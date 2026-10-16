import React, { useEffect, useState } from 'react';
import {
  Card,
  Table,
  Button,
  Modal,
  Form,
  Input,
  InputNumber,
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
  BankOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ApartmentOutlined,
  UserOutlined,
} from '@ant-design/icons';
import { academicManagementApi } from '../../api/academicManagement';
import type { FacultyItem, InstructorItem } from '../../api/academicManagement';

const { Title, Text } = Typography;

export const FacultiesPage: React.FC = () => {
  const [faculties, setFaculties] = useState<FacultyItem[]>([]);
  const [instructors, setInstructors] = useState<InstructorItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [modalOpen, setModalOpen] = useState<boolean>(false);
  const [editingFaculty, setEditingFaculty] = useState<FacultyItem | null>(null);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const [fData, instData] = await Promise.all([
        academicManagementApi.getFaculties(),
        academicManagementApi.getInstructors(),
      ]);
      setFaculties(fData);
      setInstructors(instData);
    } catch {
      message.error('Fakülteler ve eğitmenler yüklenirken hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleOpenCreate = () => {
    setEditingFaculty(null);
    form.resetFields();
    form.setFieldsValue({
      establishedYear: new Date().getFullYear(),
      deanId: undefined,
    });
    setModalOpen(true);
  };

  const handleOpenEdit = (record: FacultyItem) => {
    setEditingFaculty(record);
    form.setFieldsValue({
      code: record.code,
      name: record.name,
      deanId: record.deanId,
      establishedYear: record.establishedYear,
    });
    setModalOpen(true);
  };

  const handleSave = async (values: {
    code: string;
    name: string;
    deanId?: string;
    establishedYear?: number;
  }) => {
    try {
      if (editingFaculty) {
        await academicManagementApi.updateFaculty(editingFaculty.id, values);
        message.success(`${values.name} bilgileri güncellendi.`);
      } else {
        await academicManagementApi.createFaculty(values);
        message.success(`${values.name} fakültesi başarıyla kuruldu.`);
      }
      setModalOpen(false);
      loadData();
    } catch {
      message.error('İşlem kaydedilemedi.');
    }
  };

  const handleDelete = async (id: string, name: string) => {
    try {
      await academicManagementApi.deleteFaculty(id);
      message.success(`${name} fakültesi sistemden kaldırıldı.`);
      loadData();
    } catch {
      message.error('Fakülte silinemedi.');
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
        }}
      >
        <div>
          <Title level={4} style={{ margin: 0, color: '#0f172a', fontWeight: 700 }}>
            Fakülte Yönetimi Masası
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Üniversite bünyesindeki tüm fakültelerin kurulumu, dekan atamaları ve teşkilat yapısı
          </Text>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#0f4c81', borderRadius: 6, height: 40 }}
          onClick={handleOpenCreate}
        >
          Yeni Fakülte Ekle
        </Button>
      </div>

      {/* İstatistikler */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Toplam Aktif Fakülte"
              value={faculties.length}
              suffix="Fakülte"
              prefix={<BankOutlined style={{ color: '#0f4c81', marginRight: 6 }} />}
              valueStyle={{ color: '#0f4c81', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Bağlı Bölüm Sayısı"
              value={faculties.reduce((acc, curr) => acc + (curr.departmentCount || 0), 0)}
              suffix="Bölüm"
              prefix={<ApartmentOutlined style={{ color: '#10b981', marginRight: 6 }} />}
              valueStyle={{ color: '#10b981', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Atanmış Dekan Sayısı"
              value={faculties.filter((f) => f.deanName && f.deanName !== 'Atama Bekliyor').length}
              suffix="Dekan"
              prefix={<UserOutlined style={{ color: '#6366f1', marginRight: 6 }} />}
              valueStyle={{ color: '#6366f1', fontWeight: 700 }}
            />
          </Card>
        </Col>
      </Row>

      {/* Fakülte Listesi Tablosu */}
      <Card style={{ borderRadius: 10 }}>
        <Table
          dataSource={faculties.map((f) => ({ ...f, key: f.id }))}
          loading={loading}
          pagination={false}
          columns={[
            {
              title: 'Fakülte Kodu',
              dataIndex: 'code',
              key: 'code',
              width: 140,
              render: (code: string) => (
                <Tag color="geekblue" style={{ fontSize: 13, fontWeight: 700, padding: '2px 8px' }}>
                  {code}
                </Tag>
              ),
            },
            {
              title: 'Fakülte Adı',
              dataIndex: 'name',
              key: 'name',
              render: (name: string) => <strong style={{ fontSize: 14, color: '#0f172a' }}>{name}</strong>,
            },
            {
              title: 'Fakülte Dekanı',
              dataIndex: 'deanName',
              key: 'deanName',
              render: (dean?: string) =>
                dean && dean !== 'Atama Bekliyor' ? (
                  <Tag color="purple" style={{ fontSize: 13, fontWeight: 600 }}>
                    {dean}
                  </Tag>
                ) : (
                  <Text type="secondary" italic>
                    Atama Yapılmadı
                  </Text>
                ),
            },
            {
              title: 'Kuruluş Yılı',
              dataIndex: 'establishedYear',
              key: 'establishedYear',
              align: 'center',
            },
            {
              title: 'Bölüm Sayısı',
              dataIndex: 'departmentCount',
              key: 'departmentCount',
              align: 'center',
              render: (count?: number) => <Tag color="blue">{count || 0} Bölüm</Tag>,
            },
            {
              title: 'Durum',
              dataIndex: 'isActive',
              key: 'isActive',
              align: 'center',
              render: (active: boolean) => (
                <Tag color={active ? 'green' : 'default'}>{active ? 'Aktif' : 'Pasif'}</Tag>
              ),
            },
            {
              title: 'İşlemler',
              key: 'actions',
              align: 'right',
              render: (_: unknown, record: FacultyItem) => (
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
                    title="Fakülteyi silmek istediğinize emin misiniz?"
                    description="Fakülteye bağlı bölümler ve dersler bulunabilir."
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

      {/* Fakülte Ekle / Düzenle Modalı (Eğitmenlerden Dekan Seçimi) */}
      <Modal
        title={editingFaculty ? 'Fakülte Bilgilerini Düzenle' : 'Yeni Fakülte Tanımla'}
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={null}
        width={560}
      >
        <Form form={form} layout="vertical" onFinish={handleSave}>
          <Row gutter={16}>
            <Col span={8}>
              <Form.Item
                name="code"
                label="Fakülte Kodu"
                rules={[{ required: true, message: 'Kod giriniz (örn: MDBF)' }]}
              >
                <Input placeholder="Örn: MDBF" style={{ textTransform: 'uppercase' }} />
              </Form.Item>
            </Col>
            <Col span={16}>
              <Form.Item
                name="establishedYear"
                label="Kuruluş Yılı"
                rules={[{ required: true, message: 'Yıl giriniz' }]}
              >
                <InputNumber min={1900} max={2030} style={{ width: '100%' }} />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item
            name="name"
            label="Fakülte Tam Adı"
            rules={[{ required: true, message: 'Fakülte adını giriniz' }]}
          >
            <Input placeholder="Örn: Mühendislik ve Doğa Bilimleri Fakültesi" />
          </Form.Item>

          {/* Eğitmen Listesinden Dekan Seçimi */}
          <Form.Item
            name="deanId"
            label={
              <Space>
                <span style={{ fontWeight: 600 }}>Fakülte Dekanı</span>
                <Text type="secondary" style={{ fontSize: 11 }}>
                  (Kayıtlı Eğitmen Listesinden Seçilir)
                </Text>
              </Space>
            }
          >
            <Select
              allowClear
              placeholder="Dekan seçiniz (veya atama bekliyor bırakınız)"
              showSearch
              filterOption={(input, option) =>
                (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
              }
              options={[
                ...instructors.map((inst) => ({
                  value: inst.id,
                  label: `${inst.fullName} ${inst.departmentName ? `(${inst.departmentName})` : ''}`,
                })),
              ]}
            />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 20 }}>
            <Button onClick={() => setModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              {editingFaculty ? 'Güncellemeleri Kaydet' : 'Fakülteyi Kur'}
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
