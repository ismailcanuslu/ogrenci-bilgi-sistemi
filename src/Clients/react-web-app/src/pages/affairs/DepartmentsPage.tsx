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
  Alert,
} from 'antd';
import {
  ApartmentOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  ControlOutlined,
  ThunderboltOutlined,
} from '@ant-design/icons';
import { academicManagementApi } from '../../api/academicManagement';
import type { DepartmentItem, FacultyItem } from '../../api/academicManagement';

const { Title, Text } = Typography;

export const DepartmentsPage: React.FC = () => {
  const [departments, setDepartments] = useState<DepartmentItem[]>([]);
  const [faculties, setFaculties] = useState<FacultyItem[]>([]);
  const [selectedFacultyFilter, setSelectedFacultyFilter] = useState<string>('ALL');
  const [loading, setLoading] = useState<boolean>(true);

  // Ekleme / Düzenleme Modalı
  const [modalOpen, setModalOpen] = useState<boolean>(false);
  const [editingDept, setEditingDept] = useState<DepartmentItem | null>(null);
  const [form] = Form.useForm();

  // AKTS Sınırlarını Hızlı Düzenleme Modalı
  const [ectsModalOpen, setEctsModalOpen] = useState<boolean>(false);
  const [ectsTargetDept, setEctsTargetDept] = useState<DepartmentItem | null>(null);
  const [ectsForm] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const [fList, dList] = await Promise.all([
        academicManagementApi.getFaculties(),
        academicManagementApi.getDepartments(),
      ]);
      setFaculties(fList);
      setDepartments(dList);
    } catch {
      message.error('Bölüm ve fakülte listesi yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const filteredDepartments =
    selectedFacultyFilter === 'ALL'
      ? departments
      : departments.filter((d) => d.facultyId === selectedFacultyFilter);

  const handleOpenCreate = () => {
    setEditingDept(null);
    form.resetFields();
    form.setFieldsValue({
      facultyId: faculties.length > 0 ? faculties[0].id : '',
      language: 'Türkçe',
      minEctsLimit: 30,
      maxEctsLimit: 42,
      overCapEctsLimit: 48,
    });
    setModalOpen(true);
  };

  const handleOpenEdit = (dept: DepartmentItem) => {
    setEditingDept(dept);
    form.setFieldsValue({
      facultyId: dept.facultyId,
      code: dept.code,
      name: dept.name,
      headName: dept.headName,
      language: dept.language || 'Türkçe',
      minEctsLimit: dept.minEctsLimit || 30,
      maxEctsLimit: dept.maxEctsLimit || 42,
      overCapEctsLimit: dept.overCapEctsLimit || 48,
    });
    setModalOpen(true);
  };

  const handleOpenEctsModal = (dept: DepartmentItem) => {
    setEctsTargetDept(dept);
    ectsForm.setFieldsValue({
      minEctsLimit: dept.minEctsLimit,
      maxEctsLimit: dept.maxEctsLimit,
      overCapEctsLimit: dept.overCapEctsLimit || 48,
    });
    setEctsModalOpen(true);
  };

  const handleSave = async (values: {
    facultyId: string;
    code: string;
    name: string;
    headName?: string;
    language?: string;
    minEctsLimit: number;
    maxEctsLimit: number;
    overCapEctsLimit?: number;
  }) => {
    try {
      if (editingDept) {
        await academicManagementApi.updateDepartment(editingDept.id, values);
        message.success(`${values.name} bölümü güncellendi.`);
      } else {
        await academicManagementApi.createDepartment(values);
        message.success(`${values.name} bölümü başarıyla fakülteye bağlandı.`);
      }
      setModalOpen(false);
      loadData();
    } catch {
      message.error('İşlem kaydedilirken hata oluştu.');
    }
  };

  const handleSaveEctsLimits = async (values: {
    minEctsLimit: number;
    maxEctsLimit: number;
    overCapEctsLimit?: number;
  }) => {
    if (!ectsTargetDept) return;
    try {
      await academicManagementApi.updateDepartmentEctsLimits(ectsTargetDept.id, values);
      message.success(
        `${ectsTargetDept.name} için AKTS sınırları (Dönemlik: ${values.minEctsLimit}, Üstten Alma: ${values.maxEctsLimit}, ÇAP: ${values.overCapEctsLimit}) başarıyla güncellendi!`
      );
      setEctsModalOpen(false);
      loadData();
    } catch {
      message.error('AKTS sınırları kaydedilemedi.');
    }
  };

  const handleDelete = async (id: string, name: string) => {
    try {
      await academicManagementApi.deleteDepartment(id);
      message.success(`${name} bölümü silindi.`);
      loadData();
    } catch {
      message.error('Bölüm silinemedi.');
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
            Fakülteye Bağlı Bölüm & AKTS Sınırı Yönetimi
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Fakültelere bağlı lisans/önlisans bölümleri, bölüm başkanları ve dönemlik AKTS alma kuralları
          </Text>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#0f4c81', borderRadius: 6, height: 40 }}
          onClick={handleOpenCreate}
        >
          Yeni Bölüm Ekle
        </Button>
      </div>

      <Alert
        message="Dönemlik ve Üstten Alma AKTS Sınırları"
        description="Öğrenciler ders kayıt döneminde her bölüm için belirlenen 'Normal Dönem AKTS Sınırı' ve GANO'su yüksek başarılı öğrenciler için 'Üstten Ders Alma AKTS Sınırı' kurallarına göre ders seçebilirler. Aşağıdaki tablodan AKTS kurallarını doğrudan değiştirebilirsiniz."
        type="info"
        showIcon
        style={{ borderRadius: 8 }}
      />

      {/* İstatistikler & Fakülte Filtresi */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Kayıtlı Bölüm Sayısı"
              value={departments.length}
              suffix="Bölüm"
              prefix={<ApartmentOutlined style={{ color: '#0f4c81', marginRight: 6 }} />}
              valueStyle={{ color: '#0f4c81', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Varsayılan Standart AKTS"
              value={30}
              suffix="AKTS / Dönem"
              prefix={<ControlOutlined style={{ color: '#10b981', marginRight: 6 }} />}
              valueStyle={{ color: '#10b981', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Maksimum Üst Sınır"
              value={48}
              suffix="AKTS (ÇAP Dahil)"
              prefix={<ThunderboltOutlined style={{ color: '#d97706', marginRight: 6 }} />}
              valueStyle={{ color: '#d97706', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Text strong style={{ display: 'block', marginBottom: 8, fontSize: 12 }}>
              Fakülteye Göre Filtrele:
            </Text>
            <Select
              value={selectedFacultyFilter}
              onChange={(val) => setSelectedFacultyFilter(val)}
              style={{ width: '100%' }}
              options={[
                { value: 'ALL', label: 'Tüm Fakülteler' },
                ...faculties.map((f) => ({ value: f.id, label: f.name })),
              ]}
            />
          </Card>
        </Col>
      </Row>

      {/* Bölümler Tablosu */}
      <Card style={{ borderRadius: 10 }}>
        <Table
          dataSource={filteredDepartments.map((d) => ({ ...d, key: d.id }))}
          loading={loading}
          pagination={{ pageSize: 8 }}
          columns={[
            {
              title: 'Bölüm Kodu',
              dataIndex: 'code',
              key: 'code',
              width: 110,
              render: (code: string) => (
                <Tag color="blue" style={{ fontSize: 13, fontWeight: 700, padding: '2px 8px' }}>
                  {code}
                </Tag>
              ),
            },
            {
              title: 'Bölüm Adı',
              dataIndex: 'name',
              key: 'name',
              render: (name: string, r: DepartmentItem) => (
                <div>
                  <strong style={{ fontSize: 14, color: '#0f172a' }}>{name}</strong>
                  <div style={{ fontSize: 12, color: '#64748b' }}>{r.facultyName}</div>
                </div>
              ),
            },
            {
              title: 'Bölüm Başkanı',
              dataIndex: 'headName',
              key: 'headName',
              render: (head?: string) => head || <Text type="secondary">Atama Yapılmadı</Text>,
            },
            {
              title: 'Eğitim Dili',
              dataIndex: 'language',
              key: 'language',
              align: 'center',
              render: (lang?: string) => <Tag color="cyan">{lang || 'Türkçe'}</Tag>,
            },
            {
              title: 'Normal AKTS Sınırı',
              dataIndex: 'minEctsLimit',
              key: 'minEctsLimit',
              align: 'center',
              render: (val: number) => (
                <Space>
                  <Tag color="purple" style={{ fontWeight: 600, fontSize: 13 }}>
                    {val} AKTS
                  </Tag>
                </Space>
              ),
            },
            {
              title: 'Üstten Alma AKTS Sınırı',
              dataIndex: 'maxEctsLimit',
              key: 'maxEctsLimit',
              align: 'center',
              render: (val: number, r: DepartmentItem) => (
                <Space>
                  <Tag color="gold" style={{ fontWeight: 600, fontSize: 13 }}>
                    {val} AKTS
                  </Tag>
                  <Button
                    type="link"
                    size="small"
                    icon={<ControlOutlined />}
                    onClick={() => handleOpenEctsModal(r)}
                  >
                    Sınırı Değiştir
                  </Button>
                </Space>
              ),
            },
            {
              title: 'İşlemler',
              key: 'actions',
              align: 'right',
              render: (_: unknown, record: DepartmentItem) => (
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
                    title="Bölümü silmek istediğinize emin misiniz?"
                    description="Bu bölüme ait katalog dersleri etkilenebilir."
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

      {/* Yeni Bölüm Ekle / Düzenle Modalı */}
      <Modal
        title={editingDept ? 'Bölüm Bilgilerini Düzenle' : 'Fakülteye Yeni Bölüm Bağla'}
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={null}
        width={620}
      >
        <Form form={form} layout="vertical" onFinish={handleSave}>
          <Form.Item
            name="facultyId"
            label="Bağlı Olduğu Fakülte"
            rules={[{ required: true, message: 'Fakülte seçiniz' }]}
          >
            <Select
              options={faculties.map((f) => ({ value: f.id, label: f.name }))}
              placeholder="Fakülte seçiniz"
            />
          </Form.Item>

          <Row gutter={16}>
            <Col span={8}>
              <Form.Item
                name="code"
                label="Bölüm Kodu"
                rules={[{ required: true, message: 'Kod giriniz (örn: BLG)' }]}
              >
                <Input placeholder="Örn: BLG" style={{ textTransform: 'uppercase' }} />
              </Form.Item>
            </Col>
            <Col span={16}>
              <Form.Item
                name="name"
                label="Bölüm Tam Adı"
                rules={[{ required: true, message: 'Bölüm adı giriniz' }]}
              >
                <Input placeholder="Örn: Bilgisayar Mühendisliği Bölümü" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="headName" label="Bölüm Başkanı">
                <Input placeholder="Örn: Prof. Dr. Ahmet Bilgehan" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="language" label="Eğitim Dili" initialValue="Türkçe">
                <Select
                  options={[
                    { value: 'Türkçe', label: 'Türkçe' },
                    { value: '%30 İngilizce', label: '%30 İngilizce Destekli' },
                    { value: '%100 İngilizce', label: '%100 İngilizce' },
                  ]}
                />
              </Form.Item>
            </Col>
          </Row>

          <div
            style={{
              padding: '14px 16px',
              backgroundColor: '#f8fafc',
              borderRadius: 8,
              border: '1px solid #e2e8f0',
              marginBottom: 20,
            }}
          >
            <Text strong style={{ display: 'block', marginBottom: 12, color: '#0f4c81' }}>
              Bölüm AKTS Limit Kuralları:
            </Text>
            <Row gutter={16}>
              <Col span={8}>
                <Form.Item
                  name="minEctsLimit"
                  label="Standart Dönem AKTS"
                  rules={[{ required: true }]}
                >
                  <InputNumber min={20} max={45} style={{ width: '100%' }} addonAfter="AKTS" />
                </Form.Item>
              </Col>
              <Col span={8}>
                <Form.Item
                  name="maxEctsLimit"
                  label="Üstten Alma AKTS Sınırı"
                  rules={[{ required: true }]}
                >
                  <InputNumber min={30} max={60} style={{ width: '100%' }} addonAfter="AKTS" />
                </Form.Item>
              </Col>
              <Col span={8}>
                <Form.Item
                  name="overCapEctsLimit"
                  label="ÇAP / Özel İzin Sınırı"
                  rules={[{ required: true }]}
                >
                  <InputNumber min={35} max={60} style={{ width: '100%' }} addonAfter="AKTS" />
                </Form.Item>
              </Col>
            </Row>
          </div>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10 }}>
            <Button onClick={() => setModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              {editingDept ? 'Güncellemeleri Kaydet' : 'Bölümü Oluştur'}
            </Button>
          </div>
        </Form>
      </Modal>

      {/* AKTS Sınırlarını Hızlı Düzenleme Modalı */}
      <Modal
        title={`AKTS Sınırlarını Değiştir: ${ectsTargetDept?.name || ''}`}
        open={ectsModalOpen}
        onCancel={() => setEctsModalOpen(false)}
        footer={null}
        width={480}
      >
        <Form form={ectsForm} layout="vertical" onFinish={handleSaveEctsLimits}>
          <Alert
            message="AKTS Değişikliği Bildirimi"
            description="Bu ayar ders kayıt ekranında öğrencilerin alabileceği maksimum ders kredisini doğrudan kısıtlar."
            type="warning"
            showIcon
            style={{ marginBottom: 16 }}
          />

          <Form.Item
            name="minEctsLimit"
            label="Normal Dönemlik AKTS Sınırı (Varsayılan: 30)"
            rules={[{ required: true, message: 'AKTS sınırı giriniz' }]}
          >
            <InputNumber min={20} max={45} style={{ width: '100%' }} addonAfter="AKTS" />
          </Form.Item>

          <Form.Item
            name="maxEctsLimit"
            label="Üstten Ders Alma / Başarılı Öğrenci AKTS Sınırı"
            rules={[{ required: true, message: 'AKTS sınırı giriniz' }]}
          >
            <InputNumber min={30} max={60} style={{ width: '100%' }} addonAfter="AKTS" />
          </Form.Item>

          <Form.Item
            name="overCapEctsLimit"
            label="ÇAP / Çift Ana Dal & Mezuniyet Aşaması AKTS Sınırı"
            rules={[{ required: true, message: 'AKTS sınırı giriniz' }]}
          >
            <InputNumber min={35} max={65} style={{ width: '100%' }} addonAfter="AKTS" />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 20 }}>
            <Button onClick={() => setEctsModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              Yeni AKTS Sınırlarını Uygula
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
