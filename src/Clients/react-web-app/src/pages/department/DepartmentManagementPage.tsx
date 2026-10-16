import React, { useEffect, useState } from 'react';
import { Card, Table, Tag, Typography, Button, Modal, Form, Input, InputNumber, Space, Tabs, Row, Col, Statistic, message } from 'antd';
import {
  PlusOutlined,
  BookOutlined,
  ApartmentOutlined,
} from '@ant-design/icons';
import { academicManagementApi } from '../../api/academicManagement';
import type { CourseItem, CourseOfferingItem } from '../../api/academicManagement';

const { Title, Text } = Typography;

export const DepartmentManagementPage: React.FC = () => {
  const [courses, setCourses] = useState<CourseItem[]>([]);
  const [offerings, setOfferings] = useState<CourseOfferingItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [courseModalOpen, setCourseModalOpen] = useState<boolean>(false);
  const [form] = Form.useForm();

  const loadData = () => {
    setLoading(true);
    Promise.all([academicManagementApi.getCourses(), academicManagementApi.getCourseOfferings()]).then(
      ([cList, oList]) => {
        setCourses(cList);
        setOfferings(oList);
        setLoading(false);
      }
    );
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleCreateCourse = async (values: {
    code: string;
    name: string;
    defaultEcts: number;
    defaultCredit: number;
    description?: string;
  }) => {
    try {
      await academicManagementApi.createCourse({
        departmentId: 'dept-1',
        code: values.code,
        name: values.name,
        defaultEcts: values.defaultEcts,
        defaultCredit: values.defaultCredit,
        description: values.description,
      });
      message.success(`${values.code} katalog dersi başarıyla oluşturuldu.`);
      setCourseModalOpen(false);
      form.resetFields();
      loadData();
    } catch {
      message.error('Ders eklenirken hata oluştu.');
    }
  };

  return (
    <div>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: 20,
          padding: '20px 24px',
          backgroundColor: '#ffffff',
          borderRadius: 10,
          border: '1px solid #e2e8f0',
        }}
      >
        <div>
          <Title level={4} style={{ margin: 0, color: '#0f172a', fontWeight: 700 }}>
            Fakülte & Bölüm Başkanlığı Yönetim Konsolu
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Bilgisayar Mühendisliği Bölümü | Ders Kataloğu, Müfredat ve Görevlendirmeler
          </Text>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#0f4c81', borderRadius: 6 }}
          onClick={() => setCourseModalOpen(true)}
        >
          Yeni Katalog Dersi Ekle
        </Button>
      </div>

      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic title="Katalog Ders Sayısı" value={courses.length} suffix="Ders" valueStyle={{ color: '#0f4c81', fontWeight: 700 }} />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic title="Açılan Dönemlik Şube" value={offerings.length} suffix="Şube" valueStyle={{ color: '#10b981', fontWeight: 700 }} />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic title="Aktif Öğretim Elemanı" value={18} suffix="Akademisyen" valueStyle={{ color: '#6366f1', fontWeight: 700 }} />
          </Card>
        </Col>
        <Col xs={24} sm={6}>
          <Card style={{ borderRadius: 8 }}>
            <Statistic title="ÇAP Kontenjanı" value={10} suffix="Öğrenci" valueStyle={{ color: '#d97706', fontWeight: 700 }} />
          </Card>
        </Col>
      </Row>

      <Card style={{ borderRadius: 10 }}>
        <Tabs
          defaultActiveKey="offerings"
          items={[
            {
              key: 'offerings',
              label: (
                <Space>
                  <ApartmentOutlined />
                  <span>Dönemlik Ders Açımları & Şubeler</span>
                </Space>
              ),
              children: (
                <Table
                  dataSource={offerings.map((o) => ({ ...o, key: o.id }))}
                  columns={[
                    { title: 'Ders Kodu', dataIndex: 'courseCode', key: 'courseCode', render: (c: string) => <strong style={{ color: '#0f4c81' }}>{c}</strong> },
                    { title: 'Ders Adı', dataIndex: 'courseName', key: 'courseName', render: (n: string) => <span style={{ fontWeight: 500 }}>{n}</span> },
                    { title: 'Şube', dataIndex: 'section', key: 'section', align: 'center' as const, render: (s: string) => <Tag color="blue">{s}</Tag> },
                    {
                      title: 'Kontenjan / Doluluk',
                      key: 'capacity',
                      align: 'center' as const,
                      render: (_: unknown, r: CourseOfferingItem) => (
                        <span>
                          {r.enrolledCount} / {r.capacity}{' '}
                          <Tag color={r.enrolledCount >= r.capacity ? 'error' : 'success'}>
                            %{Math.round((r.enrolledCount / r.capacity) * 100)}
                          </Tag>
                        </span>
                      ),
                    },
                    { title: 'Sorumlu Öğretim Üyesi', dataIndex: 'primaryInstructorName', key: 'primaryInstructorName', render: (val?: string) => val || 'Atama Bekliyor' },
                    {
                      title: 'Kayıt Durumu',
                      dataIndex: 'isRegistrationOpen',
                      key: 'isRegistrationOpen',
                      align: 'center' as const,
                      render: (open: boolean) => <Tag color={open ? 'green' : 'red'}>{open ? 'Açık' : 'Kapalı'}</Tag>,
                    },
                  ]}
                  loading={loading}
                  pagination={false}
                />
              ),
            },
            {
              key: 'courses',
              label: (
                <Space>
                  <BookOutlined />
                  <span>Bölüm Katalog Dersleri ({courses.length})</span>
                </Space>
              ),
              children: (
                <Table
                  dataSource={courses.map((c) => ({ ...c, key: c.id }))}
                  columns={[
                    { title: 'Ders Kodu', dataIndex: 'code', key: 'code', render: (c: string) => <strong style={{ color: '#0f4c81' }}>{c}</strong> },
                    { title: 'Ders Adı', dataIndex: 'name', key: 'name', render: (n: string) => <span style={{ fontWeight: 500 }}>{n}</span> },
                    { title: 'Kredi', dataIndex: 'defaultCredit', key: 'defaultCredit', align: 'center' as const },
                    { title: 'AKTS', dataIndex: 'defaultEcts', key: 'defaultEcts', align: 'center' as const, render: (e: number) => <Tag color="purple">{e} AKTS</Tag> },
                    {
                      title: 'Durum',
                      dataIndex: 'isActive',
                      key: 'isActive',
                      align: 'center' as const,
                      render: (active: boolean) => <Tag color={active ? 'green' : 'default'}>{active ? 'Aktif' : 'Pasif'}</Tag>,
                    },
                  ]}
                  loading={loading}
                  pagination={false}
                />
              ),
            },
          ]}
        />
      </Card>

      {/* Yeni Ders Ekleme Modalı */}
      <Modal
        title="Yeni Bölüm Katalog Dersi Tanımla"
        open={courseModalOpen}
        onCancel={() => setCourseModalOpen(false)}
        footer={null}
        width={560}
      >
        <Form form={form} layout="vertical" onFinish={handleCreateCourse}>
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="code" label="Ders Kodu" rules={[{ required: true, message: 'Ders kodu giriniz.' }]}>
                <Input placeholder="Örn: BLG485" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="defaultEcts" label="AKTS Kredisi" rules={[{ required: true, message: 'AKTS giriniz.' }]}>
                <InputNumber min={1} max={30} style={{ width: '100%' }} />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="name" label="Ders Adı" rules={[{ required: true, message: 'Ders adı giriniz.' }]}>
            <Input placeholder="Örn: Derin Öğrenme ve Doğal Dil İşleme" />
          </Form.Item>

          <Form.Item name="defaultCredit" label="Yerel Kredi" rules={[{ required: true, message: 'Yerel kredi giriniz.' }]}>
            <InputNumber min={1} max={10} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item name="description" label="Ders Açıklaması ve Kapsamı">
            <Input.TextArea rows={3} placeholder="Dersin içeriği ve mühendislik katkısı..." />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setCourseModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              Dersi Kataloğa Kaydet
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
