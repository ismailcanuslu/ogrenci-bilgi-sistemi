import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
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
  Tabs,
  Alert,
  Collapse,
  Descriptions,
} from 'antd';
import {
  UserOutlined,
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  IdcardOutlined,
  EyeOutlined,
  TagsOutlined,
  MailOutlined,
  PhoneOutlined,
} from '@ant-design/icons';
import { academicManagementApi } from '../../api/academicManagement';
import type {
  InstructorItem,
  AcademicTitleItem,
  FacultyItem,
  DepartmentItem,
} from '../../api/academicManagement';

const { Title, Text, Paragraph } = Typography;
const { TextArea } = Input;

export const InstructorsPage: React.FC = () => {
  const navigate = useNavigate();
  const [instructors, setInstructors] = useState<InstructorItem[]>([]);
  const [titles, setTitles] = useState<AcademicTitleItem[]>([]);
  const [faculties, setFaculties] = useState<FacultyItem[]>([]);
  const [departments, setDepartments] = useState<DepartmentItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);

  // Filtreler
  const [selectedFacultyFilter, setSelectedFacultyFilter] = useState<string>('ALL');
  const [selectedDeptFilter, setSelectedDeptFilter] = useState<string>('ALL');
  const [searchTerm, setSearchTerm] = useState<string>('');

  // Eğitmen Ekle / Düzenle Modalı
  const [instructorModalOpen, setInstructorModalOpen] = useState<boolean>(false);
  const [editingInstructor, setEditingInstructor] = useState<InstructorItem | null>(null);
  const [form] = Form.useForm();

  // Eğitmen Detay & Özgeçmiş Modalı
  const [detailModalOpen, setDetailModalOpen] = useState<boolean>(false);
  const [selectedInstructorDetail, setSelectedInstructorDetail] = useState<InstructorItem | null>(null);

  // Unvan Ekleme Modalı
  const [titleModalOpen, setTitleModalOpen] = useState<boolean>(false);
  const [titleForm] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const [tList, iList, fList, dList] = await Promise.all([
        academicManagementApi.getTitles(),
        academicManagementApi.getInstructors(),
        academicManagementApi.getFaculties(),
        academicManagementApi.getDepartments(),
      ]);
      setTitles(tList);
      setInstructors(iList);
      setFaculties(fList);
      setDepartments(dList);
    } catch {
      message.error('Eğitmen ve unvan bilgileri yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  // Filtrelenmiş liste
  const filteredInstructors = instructors.filter((inst) => {
    if (selectedFacultyFilter !== 'ALL' && inst.facultyId !== selectedFacultyFilter) return false;
    if (selectedDeptFilter !== 'ALL' && inst.departmentId !== selectedDeptFilter) return false;
    if (searchTerm.trim()) {
      const q = searchTerm.toLowerCase();
      const matchName = inst.fullName.toLowerCase().includes(q);
      const matchEmail = inst.email.toLowerCase().includes(q);
      const matchDept = (inst.departmentName || '').toLowerCase().includes(q);
      if (!matchName && !matchEmail && !matchDept) return false;
    }
    return true;
  });

  // Modal Açılışları
  const handleOpenCreateInstructor = () => {
    setEditingInstructor(null);
    form.resetFields();
    form.setFieldsValue({
      title: titles.length > 0 ? titles[0].title : 'Unvansız',
      facultyId: faculties.length > 0 ? faculties[0].id : undefined,
      departmentId: departments.length > 0 ? departments[0].id : undefined,
    });
    setInstructorModalOpen(true);
  };

  const handleOpenEditInstructor = (inst: InstructorItem) => {
    setEditingInstructor(inst);
    form.setFieldsValue({
      title: inst.title || 'Unvansız',
      firstName: inst.firstName,
      lastName: inst.lastName,
      email: inst.email,
      phone: inst.phone,
      facultyId: inst.facultyId,
      departmentId: inst.departmentId,
      roomNumber: inst.roomNumber,
      // 12 Opsiyonel Alan
      educationInfo: inst.educationInfo,
      articles: inst.articles,
      projects: inst.projects,
      declarations: inst.declarations,
      books: inst.books,
      coursesTaught: inst.coursesTaught,
      editorships: inst.editorships,
      advisings: inst.advisings,
      academicTasks: inst.academicTasks,
      experiences: inst.experiences,
      refereeings: inst.refereeings,
      openAccess: inst.openAccess,
    });
    setInstructorModalOpen(true);
  };

  const handleSaveInstructor = async (values: any) => {
    try {
      if (editingInstructor) {
        await academicManagementApi.updateInstructor(editingInstructor.id, values);
        message.success(`${values.firstName} ${values.lastName} eğitmen bilgileri güncellendi.`);
      } else {
        await academicManagementApi.createInstructor(values);
        message.success(`${values.firstName} ${values.lastName} sisteme başarıyla eğitmen olarak kaydedildi.`);
      }
      setInstructorModalOpen(false);
      loadData();
    } catch {
      message.error('İşlem kaydedilemedi.');
    }
  };

  const handleDeleteInstructor = async (id: string, name: string) => {
    try {
      await academicManagementApi.deleteInstructor(id);
      message.success(`${name} sistemden kaldırıldı.`);
      loadData();
    } catch {
      message.error('Eğitmen silinemedi.');
    }
  };

  // Unvan Ekleme
  const handleCreateTitle = async (values: { title: string }) => {
    try {
      await academicManagementApi.createTitle(values.title);
      message.success(`"${values.title}" akademik unvanlar listesine eklendi.`);
      setTitleModalOpen(false);
      titleForm.resetFields();
      loadData();
    } catch {
      message.error('Unvan eklenemedi.');
    }
  };

  const handleDeleteTitle = async (id: string, title: string) => {
    try {
      await academicManagementApi.deleteTitle(id);
      message.success(`"${title}" unvanı kaldırıldı.`);
      loadData();
    } catch {
      message.error('Unvan silinemedi.');
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
            Eğitmen & Akademisyen Masası
          </Title>
          <Text type="secondary" style={{ fontSize: 13 }}>
            Öğretim üyeleri ve görevlileri kaydı, akademik unvanlar ve detaylı özgeçmiş yönetimi
          </Text>
        </div>

        <Space>
          <Button
            icon={<TagsOutlined />}
            onClick={() => setTitleModalOpen(true)}
          >
            Unvan Yönetimi ({titles.length})
          </Button>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            style={{ backgroundColor: '#0f4c81', borderRadius: 6, height: 40 }}
            onClick={handleOpenCreateInstructor}
          >
            Yeni Eğitmen Kaydet
          </Button>
        </Space>
      </div>

      <Tabs
        defaultActiveKey="instructors"
        items={[
          {
            key: 'instructors',
            label: (
              <Space>
                <UserOutlined />
                <span>Kayıtlı Eğitmenler ({instructors.length})</span>
              </Space>
            ),
            children: (
              <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
                {/* İstatistikler */}
                <Row gutter={[16, 16]}>
                  <Col xs={24} sm={6}>
                    <Card style={{ borderRadius: 8 }}>
                      <Statistic
                        title="Toplam Eğitmen"
                        value={instructors.length}
                        suffix="Öğretim Elemanı"
                        prefix={<IdcardOutlined style={{ color: '#0f4c81', marginRight: 6 }} />}
                        valueStyle={{ color: '#0f4c81', fontWeight: 700 }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} sm={6}>
                    <Card style={{ borderRadius: 8 }}>
                      <Statistic
                        title="Profesör Sayısı"
                        value={instructors.filter((i) => i.title.includes('Prof')).length}
                        suffix="Prof. Dr."
                        valueStyle={{ color: '#7c3aed', fontWeight: 700 }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} sm={6}>
                    <Card style={{ borderRadius: 8 }}>
                      <Statistic
                        title="Doçent & Dr. Öğr. Üyesi"
                        value={
                          instructors.filter(
                            (i) => i.title.includes('Doç') || i.title.includes('Dr. Öğr')
                          ).length
                        }
                        suffix="Öğretim Üyesi"
                        valueStyle={{ color: '#0284c7', fontWeight: 700 }}
                      />
                    </Card>
                  </Col>
                  <Col xs={24} sm={6}>
                    <Card style={{ borderRadius: 8 }}>
                      <Statistic
                        title="Araştırma & Öğretim Gör."
                        value={
                          instructors.filter(
                            (i) => i.title.includes('Gör') || i.title === 'Unvansız'
                          ).length
                        }
                        suffix="Akademisyen"
                        valueStyle={{ color: '#10b981', fontWeight: 700 }}
                      />
                    </Card>
                  </Col>
                </Row>

                {/* Filtre Barı */}
                <Card style={{ borderRadius: 8 }} styles={{ body: { padding: '14px 20px' } }}>
                  <Row gutter={[16, 16]} align="middle">
                    <Col xs={24} md={8}>
                      <Input.Search
                        placeholder="İsim, e-posta veya birim ara..."
                        allowClear
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                      />
                    </Col>
                    <Col xs={24} md={8}>
                      <Select
                        value={selectedFacultyFilter}
                        onChange={(val) => {
                          setSelectedFacultyFilter(val);
                          setSelectedDeptFilter('ALL');
                        }}
                        style={{ width: '100%' }}
                        options={[
                          { value: 'ALL', label: 'Tüm Fakülteler' },
                          ...faculties.map((f) => ({ value: f.id, label: f.name })),
                        ]}
                      />
                    </Col>
                    <Col xs={24} md={8}>
                      <Select
                        value={selectedDeptFilter}
                        onChange={(val) => setSelectedDeptFilter(val)}
                        style={{ width: '100%' }}
                        options={[
                          { value: 'ALL', label: 'Tüm Bölümler' },
                          ...departments
                            .filter(
                              (d) =>
                                selectedFacultyFilter === 'ALL' ||
                                d.facultyId === selectedFacultyFilter
                            )
                            .map((d) => ({ value: d.id, label: d.name })),
                        ]}
                      />
                    </Col>
                  </Row>
                </Card>

                {/* Eğitmenler Tablosu */}
                <Card style={{ borderRadius: 10 }}>
                  <Table
                    dataSource={filteredInstructors.map((i) => ({ ...i, key: i.id }))}
                    loading={loading}
                    pagination={{ pageSize: 8 }}
                    columns={[
                      {
                        title: 'Unvan',
                        dataIndex: 'title',
                        key: 'title',
                        width: 140,
                        render: (t: string) => {
                          const color =
                            t.includes('Prof')
                              ? 'purple'
                              : t.includes('Doç')
                              ? 'geekblue'
                              : t.includes('Dr.')
                              ? 'blue'
                              : t === 'Unvansız'
                              ? 'default'
                              : 'cyan';
                          return (
                            <Tag color={color} style={{ fontWeight: 600, fontSize: 12 }}>
                              {t}
                            </Tag>
                          );
                        },
                      },
                      {
                        title: 'Adı Soyadı',
                        dataIndex: 'fullName',
                        key: 'fullName',
                        render: (name: string, r: InstructorItem) => (
                          <div>
                            <button
                              onClick={() => navigate(`/affairs/instructors/${r.id}/profile`)}
                              className="text-left font-bold text-sm text-[#0f4c81] hover:text-[#0284c7] hover:underline cursor-pointer block"
                            >
                              {name}
                            </button>
                            <div style={{ fontSize: 12, color: '#64748b' }}>
                              <MailOutlined style={{ marginRight: 4 }} />
                              {r.email}
                            </div>
                          </div>
                        ),
                      },
                      {
                        title: 'Bağlı Olduğu Birim (Fakülte & Bölüm)',
                        key: 'unit',
                        render: (_: unknown, r: InstructorItem) => (
                          <div>
                            <div style={{ fontWeight: 600, color: '#0f4c81' }}>
                              {r.departmentName || 'Bölüm Atanmadı'}
                            </div>
                            <div style={{ fontSize: 11, color: '#64748b' }}>{r.facultyName}</div>
                          </div>
                        ),
                      },
                      {
                        title: 'Oda / İletişim',
                        key: 'contact',
                        render: (_: unknown, r: InstructorItem) => (
                          <div style={{ fontSize: 12 }}>
                            {r.roomNumber && (
                              <div>
                                <strong>Ofis:</strong> {r.roomNumber}
                              </div>
                            )}
                            {r.phone && (
                              <div style={{ color: '#64748b' }}>
                                <PhoneOutlined style={{ marginRight: 4 }} />
                                {r.phone}
                              </div>
                            )}
                          </div>
                        ),
                      },
                      {
                        title: 'İşlemler',
                        key: 'actions',
                        align: 'right',
                        render: (_: unknown, record: InstructorItem) => (
                          <Space>
                            <Button
                              type="primary"
                              ghost
                              size="small"
                              onClick={() => navigate(`/affairs/instructors/${record.id}/profile`)}
                              style={{ borderColor: '#6366f1', color: '#6366f1' }}
                            >
                              Akademik Profil
                            </Button>
                            <Button
                              type="default"
                              size="small"
                              icon={<EyeOutlined />}
                              onClick={() => {
                                setSelectedInstructorDetail(record);
                                setDetailModalOpen(true);
                              }}
                            >
                              Özet
                            </Button>
                            <Button
                              type="link"
                              size="small"
                              icon={<EditOutlined />}
                              onClick={() => handleOpenEditInstructor(record)}
                            >
                              Düzenle
                            </Button>
                            <Popconfirm
                              title="Eğitmeni sistemden kaldırmak istiyor musunuz?"
                              onConfirm={() => handleDeleteInstructor(record.id, record.fullName)}
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
              </div>
            ),
          },
          {
            key: 'titles',
            label: (
              <Space>
                <TagsOutlined />
                <span>Akademik Unvan Yönetimi ({titles.length})</span>
              </Space>
            ),
            children: (
              <Card
                title="Sistemde Tanımlı Akademik Unvanlar"
                extra={
                  <Button
                    type="primary"
                    icon={<PlusOutlined />}
                    style={{ backgroundColor: '#0f4c81' }}
                    onClick={() => {
                      titleForm.resetFields();
                      setTitleModalOpen(true);
                    }}
                  >
                    Yeni Unvan Ekle
                  </Button>
                }
                style={{ borderRadius: 10 }}
              >
                <Alert
                  message="Akademik Unvan Listesi"
                  description="Burada tanımlanan unvanlar, eğitmen kayıt ve düzenleme formunda doğrudan seçenek olarak sunulur. 'Unvansız' seçeneği her zaman mevcuttur."
                  type="info"
                  showIcon
                  style={{ marginBottom: 16 }}
                />

                <Table
                  dataSource={titles.map((t) => ({ ...t, key: t.id }))}
                  pagination={false}
                  columns={[
                    {
                      title: 'Sıra',
                      dataIndex: 'order',
                      key: 'order',
                      width: 80,
                      align: 'center',
                    },
                    {
                      title: 'Unvan Adı',
                      dataIndex: 'title',
                      key: 'title',
                      render: (t: string) => (
                        <Tag color={t === 'Unvansız' ? 'default' : 'geekblue'} style={{ fontSize: 13, fontWeight: 600, padding: '3px 10px' }}>
                          {t}
                        </Tag>
                      ),
                    },
                    {
                      title: 'Kullanımdaki Eğitmen Sayısı',
                      key: 'count',
                      render: (_: unknown, r: AcademicTitleItem) => {
                        const count = instructors.filter((i) => i.title === r.title).length;
                        return <span>{count} Eğitmen</span>;
                      },
                    },
                    {
                      title: 'İşlem',
                      key: 'action',
                      align: 'right',
                      render: (_: unknown, r: AcademicTitleItem) =>
                        r.title !== 'Unvansız' ? (
                          <Popconfirm
                            title="Unvanı listeden kaldırmak istediğinize emin misiniz?"
                            onConfirm={() => handleDeleteTitle(r.id, r.title)}
                          >
                            <Button danger type="text" size="small" icon={<DeleteOutlined />}>
                              Sil
                            </Button>
                          </Popconfirm>
                        ) : (
                          <Tag>Sabit Sistem Değeri</Tag>
                        ),
                    },
                  ]}
                />
              </Card>
            ),
          },
        ]}
      />

      {/* YENİ EĞİTMEN KAYIT / DÜZENLEME MODALI */}
      <Modal
        title={editingInstructor ? 'Eğitmen Bilgilerini Düzenle' : 'Yeni Eğitmen Kaydı'}
        open={instructorModalOpen}
        onCancel={() => setInstructorModalOpen(false)}
        footer={null}
        width={780}
      >
        <Form form={form} layout="vertical" onFinish={handleSaveInstructor}>
          <Alert
            message="Hızlı Kayıt Kuralı"
            description="Öğrenci işleri aşağıdaki 12 detaylı akademik alanı doldurmak zorunda DEĞİLDİR. Sadece Ad, Soyad ve Unvan seçimi ile kayıt oluşturulabilir. Diğer alanlar opsiyoneldir."
            type="info"
            showIcon
            style={{ marginBottom: 18 }}
          />

          {/* Zorunlu Temel Bilgiler */}
          <Card title="1. Temel Eğitmen Kimlik Bilgileri" size="small" style={{ marginBottom: 16 }}>
            <Row gutter={16}>
              <Col span={8}>
                <Form.Item
                  name="title"
                  label={<span style={{ fontWeight: 600 }}>Akademik Unvan</span>}
                  rules={[{ required: true, message: 'Lütfen unvan seçiniz' }]}
                >
                  <Select
                    options={titles.map((t) => ({ value: t.title, label: t.title }))}
                    placeholder="Unvan seçiniz"
                  />
                </Form.Item>
              </Col>
              <Col span={8}>
                <Form.Item
                  name="firstName"
                  label={<span style={{ fontWeight: 600 }}>Adı</span>}
                  rules={[{ required: true, message: 'Eğitmen adı zorunludur' }]}
                >
                  <Input placeholder="Örn: Ahmet" />
                </Form.Item>
              </Col>
              <Col span={8}>
                <Form.Item
                  name="lastName"
                  label={<span style={{ fontWeight: 600 }}>Soyadı</span>}
                  rules={[{ required: true, message: 'Eğitmen soyadı zorunludur' }]}
                >
                  <Input placeholder="Örn: Bilgehan" />
                </Form.Item>
              </Col>
            </Row>

            <Row gutter={16}>
              <Col span={12}>
                <Form.Item
                  name="email"
                  label={<span style={{ fontWeight: 600 }}>Kurumsal E-Posta</span>}
                  rules={[{ required: true, type: 'email', message: 'Geçerli bir kurumsal e-posta giriniz' }]}
                >
                  <Input placeholder="ad.soyad@obs.edu.tr" prefix={<MailOutlined style={{ color: '#94a3b8' }} />} />
                </Form.Item>
              </Col>
              <Col span={12}>
                <Form.Item name="phone" label="Dahili / İletişim Telefonu">
                  <Input placeholder="+90 (222) 335 05 ..." prefix={<PhoneOutlined style={{ color: '#94a3b8' }} />} />
                </Form.Item>
              </Col>
            </Row>

            <Row gutter={16}>
              <Col span={10}>
                <Form.Item name="facultyId" label="Bağlı Olduğu Fakülte">
                  <Select
                    allowClear
                    placeholder="Fakülte seçiniz"
                    options={faculties.map((f) => ({ value: f.id, label: f.name }))}
                  />
                </Form.Item>
              </Col>
              <Col span={10}>
                <Form.Item name="departmentId" label="Bağlı Olduğu Bölüm">
                  <Select
                    allowClear
                    placeholder="Bölüm seçiniz"
                    options={departments.map((d) => ({
                      value: d.id,
                      label: `${d.code} - ${d.name}`,
                    }))}
                  />
                </Form.Item>
              </Col>
              <Col span={4}>
                <Form.Item name="roomNumber" label="Ofis / Oda">
                  <Input placeholder="M-204" />
                </Form.Item>
              </Col>
            </Row>
          </Card>

          {/* 12 OPSİYONEL AKADEMİK ALAN (Collapse / Akordeon) */}
          <Collapse
            size="small"
            style={{ marginBottom: 20 }}
            items={[
              {
                key: '1',
                label: <span style={{ fontWeight: 600, color: '#0f4c81' }}>Öğrenim, Yayın ve Araştırma Bilgileri (İsteğe Bağlı 12 Alanı Aç)</span>,
                children: (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
                    <Form.Item name="educationInfo" label="1. ÖĞRENİM BİLGİLERİ (Lisans, Yüksek Lisans, Doktora)">
                      <TextArea rows={2} placeholder="Örn: Lisans: ODTÜ (2000), Doktora: İTÜ (2007)..." />
                    </Form.Item>

                    <Form.Item name="articles" label="2. MAKALELER (SCI, SSCI, Uluslararası & Ulusal Hakemli)">
                      <TextArea rows={2} placeholder="Yayımlanmış makale listesi ve atıflar..." />
                    </Form.Item>

                    <Form.Item name="projects" label="3. PROJELER (TÜBİTAK, BAP, AB vb.)">
                      <TextArea rows={2} placeholder="Yürütücü veya araştırmacı olunan projeler..." />
                    </Form.Item>

                    <Form.Item name="declarations" label="4. BİLDİRİLER (Kongre ve Sempozyum Sunumları)">
                      <TextArea rows={2} placeholder="Uluslararası ve ulusal bildiri künyeleri..." />
                    </Form.Item>

                    <Form.Item name="books" label="5. KİTAPLAR (Kitap veya Kitap Bölümü Yazarlığı)">
                      <TextArea rows={2} placeholder="Yayımlanan kitaplar ve yayınevi bilgileri..." />
                    </Form.Item>

                    <Form.Item name="coursesTaught" label="6. DERSLER (Verilen Lisans ve Lisansüstü Dersler)">
                      <TextArea rows={2} placeholder="Verdiği güncel ve geçmiş dersler..." />
                    </Form.Item>

                    <Form.Item name="editorships" label="7. EDİTÖRLÜK (Bilimsel Dergi ve Kitap Editörlüğü)">
                      <TextArea rows={2} placeholder="Editörlük yapılan akademik yayın organları..." />
                    </Form.Item>

                    <Form.Item name="advisings" label="8. DANIŞMANLIKLAR (Yüksek Lisans ve Doktora Tezleri)">
                      <TextArea rows={2} placeholder="Tamamlanan ve devam eden öğrenci tez danışmanlıkları..." />
                    </Form.Item>

                    <Form.Item name="academicTasks" label="9. AKADEMİK GÖREVLER (Dekanlık, Bölüm Bşk, Komisyon)">
                      <TextArea rows={2} placeholder="Üniversite içi idari ve akademik görevler..." />
                    </Form.Item>

                    <Form.Item name="experiences" label="10. DENEYİMLER (Sektör ve Akademik İş Deneyimleri)">
                      <TextArea rows={2} placeholder="Geçmiş çalışma tecrübeleri ve kurumlar..." />
                    </Form.Item>

                    <Form.Item name="refereeings" label="11. HAKEMLİKLER (Dergi ve Proje Hakemliği)">
                      <TextArea rows={2} placeholder="Hakemlik yapılan dergi ve bilimsel kurullar..." />
                    </Form.Item>

                    <Form.Item name="openAccess" label="12. AÇIK ERİŞİM (ORCID, YÖKSİS, Google Scholar Linkleri)">
                      <TextArea rows={2} placeholder="ORCID: 0000-0002-..., Google Scholar URL..." />
                    </Form.Item>
                  </div>
                ),
              },
            ]}
          />

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10 }}>
            <Button onClick={() => setInstructorModalOpen(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81' }}>
              {editingInstructor ? 'Güncellemeleri Kaydet' : 'Eğitmeni Sisteme Kaydet'}
            </Button>
          </div>
        </Form>
      </Modal>

      {/* DETAY VE ÖZGEÇMİŞ GÖRÜNTÜLEME MODALI */}
      <Modal
        title={`Akademisyen Özgeçmiş Detayı: ${selectedInstructorDetail?.fullName || ''}`}
        open={detailModalOpen}
        onCancel={() => setDetailModalOpen(false)}
        footer={[
          <Button key="close" type="primary" onClick={() => setDetailModalOpen(false)}>
            Kapat
          </Button>,
        ]}
        width={760}
      >
        {selectedInstructorDetail && (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
            <Descriptions bordered size="small" column={2}>
              <Descriptions.Item label="Unvan & Ad Soyad">
                <strong>{selectedInstructorDetail.fullName}</strong>
              </Descriptions.Item>
              <Descriptions.Item label="E-Posta">
                {selectedInstructorDetail.email}
              </Descriptions.Item>
              <Descriptions.Item label="Fakülte">
                {selectedInstructorDetail.facultyName || '-'}
              </Descriptions.Item>
              <Descriptions.Item label="Bölüm">
                {selectedInstructorDetail.departmentName || '-'}
              </Descriptions.Item>
              <Descriptions.Item label="Ofis / Oda">
                {selectedInstructorDetail.roomNumber || '-'}
              </Descriptions.Item>
              <Descriptions.Item label="Telefon">
                {selectedInstructorDetail.phone || '-'}
              </Descriptions.Item>
            </Descriptions>

            <Collapse
              defaultActiveKey={['1', '2', '6', '9']}
              size="small"
              items={[
                {
                  key: '1',
                  label: 'ÖĞRENİM BİLGİLERİ',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.educationInfo || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '2',
                  label: 'MAKALELER',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.articles || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '3',
                  label: 'PROJELER',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.projects || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '4',
                  label: 'BİLDİRİLER',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.declarations || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '5',
                  label: 'KİTAPLAR',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.books || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '6',
                  label: 'DERSLER',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.coursesTaught || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '7',
                  label: 'EDİTÖRLÜK',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.editorships || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '8',
                  label: 'DANIŞMANLIKLAR',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.advisings || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '9',
                  label: 'AKADEMİK GÖREVLER',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.academicTasks || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '10',
                  label: 'DENEYİMLER',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.experiences || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '11',
                  label: 'HAKEMLİKLER',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.refereeings || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
                {
                  key: '12',
                  label: 'AÇIK ERİŞİM (ORCID / YÖKSİS)',
                  children: <Paragraph style={{ whiteSpace: 'pre-line', margin: 0 }}>{selectedInstructorDetail.openAccess || 'Kayıt bulunmuyor.'}</Paragraph>,
                },
              ]}
            />
          </div>
        )}
      </Modal>

      {/* YENİ AKADEMİK UNVAN EKLEME MODALI */}
      <Modal
        title="Yeni Akademik Unvan Tanımla"
        open={titleModalOpen}
        onCancel={() => setTitleModalOpen(false)}
        onOk={() => titleForm.submit()}
        okText="Unvanı Ekle"
        cancelText="İptal"
      >
        <Form form={titleForm} layout="vertical" onFinish={handleCreateTitle}>
          <Form.Item
            name="title"
            label="Akademik Unvan"
            rules={[{ required: true, message: 'Unvan yazınız (örn: Dr. Öğr. Üyesi)' }]}
          >
            <Input placeholder="Örn: Dr. Öğr. Üyesi, Doç. Dr., vb." />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};
