import React, { useEffect, useState } from 'react';
import { Card, Table, Tag, Typography, Button, Modal, Form, Input, Select, message } from 'antd';
import {
  PlusOutlined,
  ClockCircleOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
} from '@ant-design/icons';
import { studentApplicationsApi } from '../../api/studentApplications';
import type { StudentApplicationItem, ApplicationType } from '../../api/studentApplications';

const { Title, Text } = Typography;

export const StudentApplicationsPage: React.FC = () => {
  const [applications, setApplications] = useState<StudentApplicationItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [modalOpen, setModalOpen] = useState<boolean>(false);
  const [form] = Form.useForm();

  const loadData = () => {
    setLoading(true);
    studentApplicationsApi.list().then((res) => {
      setApplications(res);
      setLoading(false);
    });
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleCreate = async (values: { type: ApplicationType; details: string; targetInstitution?: string }) => {
    try {
      await studentApplicationsApi.submit({
        type: values.type,
        details: values.details,
        targetInstitution: values.targetInstitution,
        gpa: 3.78,
      });
      message.success('Başvurunuz başarıyla oluşturuldu ve değerlendirmeye alındı.');
      setModalOpen(false);
      form.resetFields();
      loadData();
    } catch {
      message.error('Başvuru iletilirken hata oluştu.');
    }
  };

  const getTypeText = (type: ApplicationType) => {
    switch (type) {
      case 'DoubleMajor': return 'Çift Anadal (ÇAP)';
      case 'Minor': return 'Yandal';
      case 'Transfer': return 'Yatay Geçiş';
      case 'SummerSchool': return 'Yaz Okulu';
      case 'OfficialDocument': return 'Resmi Belge Talebi';
      case 'ExcuseExam': return 'Mazeret Sınavı';
      default: return type;
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
            Öğrenci Başvuru & Belge Talep Masası
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            ÇAP, Yandal, Mazeret Sınavı, Yaz Okulu ve Resmi Belge Başvuruları
          </Text>
        </div>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          style={{ backgroundColor: '#0f4c81', borderRadius: 6 }}
          onClick={() => setModalOpen(true)}
        >
          Yeni Başvuru Yap
        </Button>
      </div>

      <Card title="Kayıtlı Başvurularınız ve Süreç Takibi" style={{ borderRadius: 10 }}>
        <Table
          dataSource={applications.map((a) => ({ ...a, key: a.id }))}
          columns={[
            {
              title: 'Başvuru Türü',
              dataIndex: 'type',
              key: 'type',
              render: (t: ApplicationType) => <strong style={{ color: '#0f4c81' }}>{getTypeText(t)}</strong>,
            },
            {
              title: 'Açıklama / Hedef Bölüm',
              dataIndex: 'details',
              key: 'details',
              render: (d: string, r: StudentApplicationItem) => (
                <div>
                  <div style={{ fontSize: 13, color: '#1e293b' }}>{d}</div>
                  {r.targetDepartmentName && <Tag color="blue" style={{ marginTop: 4 }}>{r.targetDepartmentName}</Tag>}
                </div>
              ),
            },
            {
              title: 'Başvuru Tarihi',
              dataIndex: 'createdAt',
              key: 'createdAt',
            },
            {
              title: 'Durum',
              dataIndex: 'state',
              key: 'state',
              render: (state: string) => {
                if (state === 'Approved') return <Tag color="success" icon={<CheckCircleOutlined />}>Onaylandı</Tag>;
                if (state === 'Rejected') return <Tag color="error" icon={<CloseCircleOutlined />}>Reddedildi</Tag>;
                return <Tag color="warning" icon={<ClockCircleOutlined />}>İncelemede</Tag>;
              },
            },
            {
              title: 'Değerlendirme Notu',
              dataIndex: 'reviewNotes',
              key: 'reviewNotes',
              render: (note?: string) => <span style={{ fontSize: 12, color: '#64748b' }}>{note || 'Henüz değerlendirilmedi'}</span>,
            },
          ]}
          loading={loading}
          pagination={false}
        />
      </Card>

      {/* Yeni Başvuru Modalı */}
      <Modal
        title="Yeni Başvuru Formu"
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        footer={null}
        width={560}
      >
        <Form form={form} layout="vertical" onFinish={handleCreate}>
          <Form.Item
            name="type"
            label="Başvuru Kategorisi"
            rules={[{ required: true, message: 'Lütfen başvuru türünü seçiniz.' }]}
          >
            <Select
              options={[
                { value: 'DoubleMajor', label: 'Çift Anadal (ÇAP) Başvurusu' },
                { value: 'Minor', label: 'Yandal Başvurusu' },
                { value: 'OfficialDocument', label: 'E-İmzalı Resmi Belge / Transkript Talebi' },
                { value: 'ExcuseExam', label: 'Sağlık Raporu / Mazeret Sınavı Talebi' },
                { value: 'SummerSchool', label: 'Farklı Üniversiteden Yaz Okulu Alma' },
                { value: 'Transfer', label: 'Kurum İçi / Kurumlar Arası Yatay Geçiş' },
              ]}
              placeholder="Başvuru türünü seçiniz"
            />
          </Form.Item>

          <Form.Item
            name="details"
            label="Gerekçe ve Başvuru Detayı"
            rules={[{ required: true, message: 'Lütfen başvuru detayını yazınız.' }]}
          >
            <Input.TextArea rows={4} placeholder="Başvurunuza ilişkin ayrıntıları ve talebinizi açıklayınız..." />
          </Form.Item>

          <Form.Item name="targetInstitution" label="Hedef Kurum / Üniversite (Gerekliyse)">
            <Input placeholder="Örn: ODTÜ, İTÜ veya ilgili kurum adı" />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 20 }}>
            <Button onClick={() => setModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              Başvuruyu Tamamla
            </Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
