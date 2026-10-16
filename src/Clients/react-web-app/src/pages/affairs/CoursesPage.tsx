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
  BookOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  FilterOutlined,
} from '@ant-design/icons';
import { academicManagementApi } from '../../api/academicManagement';
import type { CourseItem, DepartmentItem } from '../../api/academicManagement';

const { Title, Text, Paragraph } = Typography;

export const CoursesPage: React.FC = () => {
  const [courses, setCourses] = useState<CourseItem[]>([]);
  const [departments, setDepartments] = useState<DepartmentItem[]>([]);
  const [selectedDeptFilter, setSelectedDeptFilter] = useState<string>('ALL');
  const [loading, setLoading] = useState<boolean>(true);

  // Modal State
  const [modalOpen, setModalOpen] = useState<boolean>(false);
  const [editingCourse, setEditingCourse] = useState<CourseItem | null>(null);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const [dList, cList] = await Promise.all([
        academicManagementApi.getDepartments(),
        academicManagementApi.getCourses(),
      ]);
      setDepartments(dList);
      setCourses(cList);
    } catch {
      message.error('Ders ve bölüm verileri yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const filteredCourses =
    selectedDeptFilter === 'ALL'
      ? courses
      : courses.filter((c) => c.departmentId === selectedDeptFilter);

  const handleOpenCreate = () => {
    setEditingCourse(null);
    form.resetFields();
    form.setFieldsValue({
      departmentId: departments.length > 0 ? departments[0].id : '',
      defaultEcts: 6,
      defaultCredit: 3,
      theoreticalHours: 3,
      practicalHours: 0,
      courseType: 'Zorunlu',
    });
    setModalOpen(true);
  };

  const handleOpenEdit = (course: CourseItem) => {
    setEditingCourse(course);
    form.setFieldsValue({
      departmentId: course.departmentId,
      code: course.code,
      name: course.name,
      defaultEcts: course.defaultEcts,
      defaultCredit: course.defaultCredit,
      theoreticalHours: course.theoreticalHours || 3,
      practicalHours: course.practicalHours || 0,
      courseType: course.courseType || 'Zorunlu',
      description: course.description,
    });
    setModalOpen(true);
  };

  const handleSave = async (values: {
    departmentId: string;
    code: string;
    name: string;
    defaultEcts: number;
    defaultCredit: number;
    theoreticalHours?: number;
    practicalHours?: number;
    courseType?: 'Zorunlu' | 'Seçmeli' | 'Üniversite Ortak';
    description?: string;
  }) => {
    try {
      if (editingCourse) {
        await academicManagementApi.updateCourse(editingCourse.id, values);
        message.success(`${values.code} katalog dersi güncellendi.`);
      } else {
        await academicManagementApi.createCourse(values);
        message.success(`${values.code} dersi başarıyla bölüme tanımlandı.`);
      }
      setModalOpen(false);
      loadData();
    } catch {
      message.error('Ders kaydedilirken hata oluştu.');
    }
  };

  const handleDelete = async (id: string, code: string) => {
    try {
      await academicManagementApi.deleteCourse(id);
      message.success(`${code} dersi müfredattan kaldırıldı.`);
      loadData();
    } catch {
      message.error('Ders silinemedi.');
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
            Bölüm Dersleri & Müfredat Kataloğu
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Bölümlere göre katalog dersleri, AKTS, yerel kredi, teorik/pratik saat ve zorunlu/seçmeli tanımları
          </Text>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#0f4c81', borderRadius: 6, height: 40 }}
          onClick={handleOpenCreate}
        >
          Yeni Katalog Dersi Ekle
        </Button>
      </div>

      {/* İstatistikler ve Filtre */}
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Katalogdaki Toplam Ders"
              value={courses.length}
              suffix="Ders"
              prefix={<BookOutlined style={{ color: '#0f4c81', marginRight: 6 }} />}
              valueStyle={{ color: '#0f4c81', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic
              title="Ortalama Ders AKTS Değeri"
              value={5.8}
              precision={1}
              suffix="AKTS"
              valueStyle={{ color: '#10b981', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card style={{ borderRadius: 8 }}>
            <Text strong style={{ display: 'block', marginBottom: 8, fontSize: 12 }}>
              <FilterOutlined style={{ marginRight: 4 }} /> Bölüme Göre Listele:
            </Text>
            <Select
              value={selectedDeptFilter}
              onChange={(val) => setSelectedDeptFilter(val)}
              style={{ width: '100%' }}
              options={[
                { value: 'ALL', label: 'Tüm Bölümlerin Dersleri' },
                ...departments.map((d) => ({
                  value: d.id,
                  label: `${d.code} - ${d.name}`,
                })),
              ]}
            />
          </Card>
        </Col>
      </Row>

      {/* Dersler Tablosu */}
      <Card style={{ borderRadius: 10 }}>
        <Table
          dataSource={filteredCourses.map((c) => ({ ...c, key: c.id }))}
          loading={loading}
          pagination={{ pageSize: 8 }}
          columns={[
            {
              title: 'Ders Kodu',
              dataIndex: 'code',
              key: 'code',
              width: 120,
              render: (code: string) => (
                <Tag color="geekblue" style={{ fontSize: 13, fontWeight: 700, padding: '2px 8px' }}>
                  {code}
                </Tag>
              ),
            },
            {
              title: 'Ders Adı',
              dataIndex: 'name',
              key: 'name',
              render: (name: string, r: CourseItem) => (
                <div>
                  <strong style={{ fontSize: 14, color: '#0f172a' }}>{name}</strong>
                  <div style={{ fontSize: 12, color: '#64748b' }}>{r.departmentName}</div>
                  {r.description && (
                    <Paragraph ellipsis={{ rows: 1 }} style={{ fontSize: 11, color: '#94a3b8', margin: 0 }}>
                      {r.description}
                    </Paragraph>
                  )}
                </div>
              ),
            },
            {
              title: 'Türü',
              dataIndex: 'courseType',
              key: 'courseType',
              align: 'center',
              render: (type?: string) => (
                <Tag color={type === 'Zorunlu' ? 'volcano' : type === 'Seçmeli' ? 'blue' : 'green'}>
                  {type || 'Zorunlu'}
                </Tag>
              ),
            },
            {
              title: 'AKTS',
              dataIndex: 'defaultEcts',
              key: 'defaultEcts',
              align: 'center',
              render: (val: number) => (
                <Tag color="purple" style={{ fontWeight: 700, fontSize: 13 }}>
                  {val} AKTS
                </Tag>
              ),
            },
            {
              title: 'Yerel Kredi',
              dataIndex: 'defaultCredit',
              key: 'defaultCredit',
              align: 'center',
              render: (val: number) => <strong>{val}</strong>,
            },
            {
              title: 'Haftalık Saat (T+U)',
              key: 'hours',
              align: 'center',
              render: (_: unknown, r: CourseItem) => (
                <span>{r.theoreticalHours || 3} Teo + {r.practicalHours || 0} Uyg</span>
              ),
            },
            {
              title: 'İşlemler',
              key: 'actions',
              align: 'right',
              render: (_: unknown, record: CourseItem) => (
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
                    title="Dersi katalogdan kaldırmak istiyor musunuz?"
                    onConfirm={() => handleDelete(record.id, record.code)}
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

      {/* Ders Ekle / Düzenle Modalı */}
      <Modal
        title={editingCourse ? 'Ders Bilgilerini Düzenle' : 'Bölüme Yeni Katalog Dersi Ekle'}
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={null}
        width={620}
      >
        <Form form={form} layout="vertical" onFinish={handleSave}>
          <Form.Item
            name="departmentId"
            label="Dersin Bağlı Olduğu Bölüm"
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

          <Row gutter={16}>
            <Col span={8}>
              <Form.Item
                name="code"
                label="Ders Kodu"
                rules={[{ required: true, message: 'Kod giriniz (örn: BLG302)' }]}
              >
                <Input placeholder="Örn: BLG302" style={{ textTransform: 'uppercase' }} />
              </Form.Item>
            </Col>
            <Col span={16}>
              <Form.Item
                name="name"
                label="Ders Tam Adı"
                rules={[{ required: true, message: 'Ders adını giriniz' }]}
              >
                <Input placeholder="Örn: İşletim Sistemleri" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={6}>
              <Form.Item
                name="defaultEcts"
                label="AKTS Kredisi"
                rules={[{ required: true, message: 'AKTS giriniz' }]}
              >
                <InputNumber min={1} max={30} style={{ width: '100%' }} />
              </Form.Item>
            </Col>
            <Col span={6}>
              <Form.Item
                name="defaultCredit"
                label="Yerel Kredi"
                rules={[{ required: true, message: 'Kredi giriniz' }]}
              >
                <InputNumber min={0} max={15} style={{ width: '100%' }} />
              </Form.Item>
            </Col>
            <Col span={6}>
              <Form.Item name="theoreticalHours" label="Teorik Saat" initialValue={3}>
                <InputNumber min={0} max={10} style={{ width: '100%' }} />
              </Form.Item>
            </Col>
            <Col span={6}>
              <Form.Item name="practicalHours" label="Uygulama Saat" initialValue={0}>
                <InputNumber min={0} max={10} style={{ width: '100%' }} />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="courseType" label="Ders Türü" initialValue="Zorunlu">
            <Select
              options={[
                { value: 'Zorunlu', label: 'Bölüm Zorunlu Dersi' },
                { value: 'Seçmeli', label: 'Bölüm Alan Seçmeli' },
                { value: 'Üniversite Ortak', label: 'Üniversite Ortak / Serbest Seçmeli' },
              ]}
            />
          </Form.Item>

          <Form.Item name="description" label="Ders Açıklaması & Öğrenim Kazanımları">
            <Input.TextArea rows={3} placeholder="Dersin amacı, içeriği ve öğrencilere kazandıracağı yetkinlikler..." />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10 }}>
            <Button onClick={() => setModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              {editingCourse ? 'Güncellemeleri Kaydet' : 'Dersi Kataloğa Ekle'}
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
