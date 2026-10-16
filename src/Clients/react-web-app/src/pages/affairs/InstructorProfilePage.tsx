import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Tabs,
  Button,
  Tag,
  Space,
  Modal,
  Form,
  Input,
  Select,
  InputNumber,
  Row,
  Col,
  Table,
  Popconfirm,
  message,
} from 'antd';
import {
  ReadOutlined,
  BookOutlined,
  FileTextOutlined,
  ProjectOutlined,
  ArrowLeftOutlined,
  PlusOutlined,
  LinkOutlined,
  GlobalOutlined,
  ExportOutlined,
  SolutionOutlined,
  EditOutlined,
  DeleteOutlined,
} from '@ant-design/icons';
import { academicManagementApi } from '../../api/academicManagement';
import type { InstructorItem } from '../../api/academicManagement';
import {
  getAcademicProfile,
  saveAcademicProfile
} from '../../api/academicProfileApi';
import type {
  InstructorAcademicProfile,
  EducationEntry,
  ArticleEntry,
  BookEntry,
  ConferenceEntry,
  ProjectEntry,
  CourseEntry,
  AdvisingEntry,
} from '../../api/academicProfileApi';

export const InstructorProfilePage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [instructors, setInstructors] = useState<InstructorItem[]>([]);
  const [selectedInstructorId, setSelectedInstructorId] = useState<string>(id || 'inst-1');
  const [instructor, setInstructor] = useState<InstructorItem | null>(null);
  const [profile, setProfile] = useState<InstructorAcademicProfile | null>(null);

  // Modallar
  const [openAccessModal, setOpenAccessModal] = useState<boolean>(false);
  const [educationModal, setEducationModal] = useState<boolean>(false);
  const [articleModal, setArticleModal] = useState<boolean>(false);
  const [bookModal, setBookModal] = useState<boolean>(false);
  const [conferenceModal, setConferenceModal] = useState<boolean>(false);
  const [projectModal, setProjectModal] = useState<boolean>(false);
  const [courseModal, setCourseModal] = useState<boolean>(false);
  const [advisingModal, setAdvisingModal] = useState<boolean>(false);

  // Düzenleme nesneleri
  const [editingItem, setEditingItem] = useState<any>(null);

  // Formlar
  const [openAccessForm] = Form.useForm();
  const [eduForm] = Form.useForm();
  const [artForm] = Form.useForm();
  const [bkForm] = Form.useForm();
  const [confForm] = Form.useForm();
  const [prjForm] = Form.useForm();
  const [crsForm] = Form.useForm();
  const [advForm] = Form.useForm();

  // Eğitmenleri ve seçili profil bilgilerini yükle
  const loadInstructorsAndProfile = async () => {
    try {
      const instList = await academicManagementApi.getInstructors();
      setInstructors(instList);

      const targetId = id || selectedInstructorId || (instList[0]?.id || 'inst-1');
      setSelectedInstructorId(targetId);

      const currentInst = instList.find(i => i.id === targetId) || instList[0];
      setInstructor(currentInst);

      if (currentInst) {
        const prof = await getAcademicProfile(currentInst.id);
        setProfile(prof);
      }
    } catch {
      message.error('Profil verileri yüklenemedi.');
    }
  };

  useEffect(() => {
    loadInstructorsAndProfile();
  }, [id, selectedInstructorId]);

  const handleSelectInstructorChange = (newInstId: string) => {
    setSelectedInstructorId(newInstId);
    navigate(`/affairs/instructors/${newInstId}/profile`);
  };

  // Açık Erişim & Linkler Kaydet
  const handleSaveOpenAccess = async (values: any) => {
    if (!profile) return;
    const updated: InstructorAcademicProfile = {
      ...profile,
      openAccess: {
        ...profile.openAccess,
        ...values,
      }
    };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Açık erişim, ORCID ve Google Scholar linkleri güncellendi.');
    setOpenAccessModal(false);
  };

  // Eğitim Ekle/Düzenle
  const handleSaveEducation = async (values: any) => {
    if (!profile) return;
    let educations = [...profile.educations];
    if (editingItem) {
      educations = educations.map(e => e.id === editingItem.id ? { ...editingItem, ...values } : e);
    } else {
      educations.push({ id: `edu-${Date.now()}`, ...values });
    }
    const updated = { ...profile, educations };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Öğrenim bilgisi kaydedildi.');
    setEducationModal(false);
  };

  const handleDeleteEducation = async (eduId: string) => {
    if (!profile) return;
    const educations = profile.educations.filter(e => e.id !== eduId);
    const updated = { ...profile, educations };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Öğrenim kaydı silindi.');
  };

  // Makale Ekle/Düzenle
  const handleSaveArticle = async (values: any) => {
    if (!profile) return;
    let articles = [...profile.articles];
    if (editingItem) {
      articles = articles.map(a => a.id === editingItem.id ? { ...editingItem, ...values } : a);
    } else {
      articles.push({ id: `art-${Date.now()}`, ...values });
    }
    const updated = { ...profile, articles };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Makale kaydı kaydedildi.');
    setArticleModal(false);
  };

  const handleDeleteArticle = async (artId: string) => {
    if (!profile) return;
    const articles = profile.articles.filter(a => a.id !== artId);
    const updated = { ...profile, articles };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Makale kaydı silindi.');
  };

  // Kitap Ekle/Düzenle
  const handleSaveBook = async (values: any) => {
    if (!profile) return;
    let books = [...profile.books];
    if (editingItem) {
      books = books.map(b => b.id === editingItem.id ? { ...editingItem, ...values } : b);
    } else {
      books.push({ id: `bk-${Date.now()}`, ...values });
    }
    const updated = { ...profile, books };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Kitap kaydı kaydedildi.');
    setBookModal(false);
  };

  const handleDeleteBook = async (bkId: string) => {
    if (!profile) return;
    const books = profile.books.filter(b => b.id !== bkId);
    const updated = { ...profile, books };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Kitap kaydı silindi.');
  };

  // Bildiri Ekle/Düzenle
  const handleSaveConference = async (values: any) => {
    if (!profile) return;
    let conferences = [...profile.conferences];
    if (editingItem) {
      conferences = conferences.map(c => c.id === editingItem.id ? { ...editingItem, ...values } : c);
    } else {
      conferences.push({ id: `conf-${Date.now()}`, ...values });
    }
    const updated = { ...profile, conferences };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Bildiri kaydı kaydedildi.');
    setConferenceModal(false);
  };

  const handleDeleteConference = async (confId: string) => {
    if (!profile) return;
    const conferences = profile.conferences.filter(c => c.id !== confId);
    const updated = { ...profile, conferences };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Bildiri kaydı silindi.');
  };

  // Proje Ekle/Düzenle
  const handleSaveProject = async (values: any) => {
    if (!profile) return;
    let projects = [...profile.projects];
    if (editingItem) {
      projects = projects.map(p => p.id === editingItem.id ? { ...editingItem, ...values } : p);
    } else {
      projects.push({ id: `prj-${Date.now()}`, ...values });
    }
    const updated = { ...profile, projects };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Proje kaydı kaydedildi.');
    setProjectModal(false);
  };

  const handleDeleteProject = async (prjId: string) => {
    if (!profile) return;
    const projects = profile.projects.filter(p => p.id !== prjId);
    const updated = { ...profile, projects };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Proje kaydı silindi.');
  };

  // Ders Ekle/Düzenle
  const handleSaveCourse = async (values: any) => {
    if (!profile) return;
    let courses = [...profile.courses];
    if (editingItem) {
      courses = courses.map(c => c.id === editingItem.id ? { ...editingItem, ...values } : c);
    } else {
      courses.push({ id: `crs-${Date.now()}`, ...values });
    }
    const updated = { ...profile, courses };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Ders kaydı kaydedildi.');
    setCourseModal(false);
  };

  const handleDeleteCourse = async (crsId: string) => {
    if (!profile) return;
    const courses = profile.courses.filter(c => c.id !== crsId);
    const updated = { ...profile, courses };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Ders kaydı silindi.');
  };

  // Danışmanlık Ekle/Düzenle
  const handleSaveAdvising = async (values: any) => {
    if (!profile) return;
    let advisings = [...profile.advisings];
    if (editingItem) {
      advisings = advisings.map(a => a.id === editingItem.id ? { ...editingItem, ...values } : a);
    } else {
      advisings.push({ id: `adv-${Date.now()}`, ...values });
    }
    const updated = { ...profile, advisings };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Danışmanlık kaydı kaydedildi.');
    setAdvisingModal(false);
  };

  const handleDeleteAdvising = async (advId: string) => {
    if (!profile) return;
    const advisings = profile.advisings.filter(a => a.id !== advId);
    const updated = { ...profile, advisings };
    await saveAcademicProfile(updated);
    setProfile(updated);
    message.success('Danışmanlık kaydı silindi.');
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
      {/* Üst Navigasyon & Eğitmen Seçici */}
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          padding: '18px 24px',
          backgroundColor: '#ffffff',
          borderRadius: 12,
          border: '1px solid #e2e8f0',
          flexWrap: 'wrap',
          gap: 12,
        }}
      >
        <div style={{ display: 'flex', alignItems: 'center', gap: 14 }}>
          <Button
            icon={<ArrowLeftOutlined />}
            onClick={() => navigate('/affairs/instructors')}
            style={{ borderRadius: 8 }}
          >
            Eğitmen Listesi
          </Button>
          <div>
            <div style={{ fontSize: 11, fontWeight: 700, color: '#94a3b8', textTransform: 'uppercase' }}>
              Öğrenim, Yayın & Araştırma Masası
            </div>
            <h1 style={{ fontSize: 18, fontWeight: 700, color: '#0f172a', margin: 0 }}>
              Akademik Özgeçmiş & Araştırma Yönetimi
            </h1>
          </div>
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
          <span style={{ fontSize: 12, fontWeight: 600, color: '#64748b' }}>
            İncelenen Eğitmen:
          </span>
          <Select
            value={selectedInstructorId}
            onChange={handleSelectInstructorChange}
            style={{ width: 280 }}
            options={instructors.map((i) => ({
              value: i.id,
              label: `${i.title || ''} ${i.fullName} (${i.departmentName || 'Bölüm Yok'})`,
            }))}
          />
        </div>
      </div>

      {/* Eğitmen Başlık & Açık Erişim Kartı */}
      {instructor && profile && (
        <div
          style={{
            background: 'linear-gradient(135deg, #0f172a 0%, #1e1b4b 50%, #0f172a 100%)',
            color: '#ffffff',
            borderRadius: 16,
            padding: 24,
            border: '1px solid rgba(99, 102, 241, 0.3)',
            boxShadow: '0 10px 25px -5px rgba(0, 0, 0, 0.2)',
          }}
        >
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'wrap', gap: 20 }}>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 8, flexWrap: 'wrap' }}>
                <Tag color="purple" style={{ fontSize: 13, padding: '2px 10px', fontWeight: 600 }}>
                  {instructor.title}
                </Tag>
                <span style={{ fontSize: 13, color: '#cbd5e1' }}>
                  {instructor.departmentName} &bull; {instructor.facultyName}
                </span>
              </div>

              <h2 style={{ fontSize: 26, fontWeight: 800, margin: '4px 0', color: '#ffffff' }}>
                {instructor.fullName}
              </h2>

              <div style={{ display: 'flex', flexWrap: 'wrap', gap: 16, fontSize: 13, color: '#94a3b8' }}>
                {instructor.email && <span>E-Posta: {instructor.email}</span>}
                {instructor.phone && <span>Dahili: {instructor.phone}</span>}
                {instructor.roomNumber && <span>Ofis: {instructor.roomNumber}</span>}
              </div>

              {/* Açık Erişim Rozetleri */}
              <div style={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center', gap: 8, marginTop: 8 }}>
                {profile.openAccess.orcidId ? (
                  <a
                    href={profile.openAccess.orcidUrl || `https://orcid.org/${profile.openAccess.orcidId}`}
                    target="_blank"
                    rel="noreferrer"
                    style={{
                      display: 'inline-flex',
                      alignItems: 'center',
                      gap: 6,
                      padding: '4px 10px',
                      backgroundColor: 'rgba(166, 206, 57, 0.15)',
                      color: '#a6ce39',
                      border: '1px solid rgba(166, 206, 57, 0.4)',
                      borderRadius: 6,
                      fontSize: 12,
                      fontWeight: 600,
                    }}
                  >
                    ORCID: {profile.openAccess.orcidId}
                    <ExportOutlined style={{ fontSize: 11 }} />
                  </a>
                ) : (
                  <span style={{ fontSize: 12, color: '#64748b', fontStyle: 'italic' }}>ORCID tanımlı değil</span>
                )}

                {profile.openAccess.scholarUrl && (
                  <a
                    href={profile.openAccess.scholarUrl}
                    target="_blank"
                    rel="noreferrer"
                    style={{
                      display: 'inline-flex',
                      alignItems: 'center',
                      gap: 6,
                      padding: '4px 10px',
                      backgroundColor: 'rgba(59, 130, 246, 0.15)',
                      color: '#93c5fd',
                      border: '1px solid rgba(59, 130, 246, 0.4)',
                      borderRadius: 6,
                      fontSize: 12,
                      fontWeight: 600,
                    }}
                  >
                    <GlobalOutlined />
                    Google Scholar Profili
                    <ExportOutlined style={{ fontSize: 11 }} />
                  </a>
                )}

                {profile.openAccess.yoksisId && (
                  <Tag color="gold" style={{ margin: 0, padding: '2px 8px' }}>
                    YÖKSİS ID: {profile.openAccess.yoksisId}
                  </Tag>
                )}

                {profile.openAccess.scopusId && (
                  <Tag color="volcano" style={{ margin: 0, padding: '2px 8px' }}>
                    Scopus ID: {profile.openAccess.scopusId}
                  </Tag>
                )}

                {profile.openAccess.personalWebsite && (
                  <a
                    href={profile.openAccess.personalWebsite}
                    target="_blank"
                    rel="noreferrer"
                    style={{
                      display: 'inline-flex',
                      alignItems: 'center',
                      gap: 4,
                      padding: '4px 10px',
                      backgroundColor: '#334155',
                      color: '#e2e8f0',
                      borderRadius: 6,
                      fontSize: 12,
                    }}
                  >
                    <LinkOutlined />
                    Web Sayfası
                  </a>
                )}
              </div>
            </div>

            <Button
              type="primary"
              style={{ backgroundColor: '#6366f1', borderRadius: 8, height: 40 }}
              onClick={() => {
                openAccessForm.setFieldsValue(profile.openAccess);
                setOpenAccessModal(true);
              }}
            >
              Açık Erişim / ORCID / Scholar Linklerini Düzenle
            </Button>
          </div>
        </div>
      )}

      {/* Sekmeler (Tabs): Eğitim, Makale, Kitap, Bildiri, Proje, vb. */}
      {profile && (
        <div style={{ backgroundColor: '#ffffff', borderRadius: 12, border: '1px solid #e2e8f0', padding: 20 }}>
          <Tabs
            defaultActiveKey="educations"
            type="card"
            items={[
              // 1. ÖĞRENİM / EĞİTİM BİLGİLERİ
              {
                key: 'educations',
                label: (
                  <Space>
                    <ReadOutlined style={{ color: '#6366f1' }} />
                    <span>Öğrenim / Eğitim ({profile.educations.length})</span>
                  </Space>
                ),
                children: (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span style={{ fontSize: 13, color: '#64748b' }}>
                        Eğitmenin lisans, yüksek lisans, doktora dereceleri, üniversite ve tez bilgileri
                      </span>
                      <Button
                        type="primary"
                        icon={<PlusOutlined />}
                        style={{ backgroundColor: '#0f4c81' }}
                        onClick={() => {
                          setEditingItem(null);
                          eduForm.resetFields();
                          eduForm.setFieldsValue({
                            degree: 'Doktora',
                            startYear: 2015,
                            endYear: 2020,
                            isOngoing: false,
                          });
                          setEducationModal(true);
                        }}
                      >
                        Öğrenim Bilgisi Ekle
                      </Button>
                    </div>

                    <Table
                      dataSource={profile.educations.map((e) => ({ ...e, key: e.id }))}
                      pagination={false}
                      columns={[
                        {
                          title: 'Derece',
                          dataIndex: 'degree',
                          key: 'degree',
                          width: 140,
                          render: (deg: string) => <Tag color="blue" style={{ fontWeight: 600 }}>{deg}</Tag>,
                        },
                        {
                          title: 'Üniversite & Fakülte / Enstitü',
                          key: 'uni',
                          render: (_: unknown, r: EducationEntry) => (
                            <div>
                              <strong style={{ color: '#0f172a' }}>{r.university}</strong>
                              <div style={{ fontSize: 12, color: '#64748b' }}>{r.faculty} - {r.department}</div>
                            </div>
                          ),
                        },
                        {
                          title: 'Dönem / Yıl',
                          key: 'years',
                          width: 160,
                          render: (_: unknown, r: EducationEntry) => (
                            <span style={{ fontFamily: 'monospace', fontSize: 13 }}>
                              {r.startYear} - {r.isOngoing ? <Tag color="green">Devam Ediyor</Tag> : (r.endYear || '—')}
                            </span>
                          ),
                        },
                        {
                          title: 'Tez Başlığı & Danışman',
                          key: 'thesis',
                          render: (_: unknown, r: EducationEntry) =>
                            r.thesisTitle ? (
                              <div style={{ fontSize: 12 }}>
                                <strong>Tez:</strong> {r.thesisTitle}
                                {r.thesisAdvisor && (
                                  <div style={{ color: '#64748b' }}>Danışman: {r.thesisAdvisor}</div>
                                )}
                              </div>
                            ) : (
                              '—'
                            ),
                        },
                        {
                          title: 'İşlemler',
                          key: 'act',
                          align: 'right',
                          width: 130,
                          render: (_: unknown, r: EducationEntry) => (
                            <Space>
                              <Button
                                size="small"
                                type="link"
                                icon={<EditOutlined />}
                                onClick={() => {
                                  setEditingItem(r);
                                  eduForm.setFieldsValue(r);
                                  setEducationModal(true);
                                }}
                              >
                                Düzenle
                              </Button>
                              <Popconfirm
                                title="Öğrenim kaydını silmek istiyor musunuz?"
                                onConfirm={() => handleDeleteEducation(r.id)}
                                okText="Evet"
                                cancelText="İptal"
                              >
                                <Button size="small" type="link" danger icon={<DeleteOutlined />}>
                                  Sil
                                </Button>
                              </Popconfirm>
                            </Space>
                          ),
                        },
                      ]}
                    />
                  </div>
                ),
              },

              // 2. MAKALELER & YAYINLAR
              {
                key: 'articles',
                label: (
                  <Space>
                    <FileTextOutlined style={{ color: '#10b981' }} />
                    <span>Makaleler ({profile.articles.length})</span>
                  </Space>
                ),
                children: (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span style={{ fontSize: 13, color: '#64748b' }}>
                        SCI, SSCI, Scopus ve ulusal hakemli dergilerdeki makaleler, DOI ve web linkleri
                      </span>
                      <Button
                        type="primary"
                        icon={<PlusOutlined />}
                        style={{ backgroundColor: '#059669' }}
                        onClick={() => {
                          setEditingItem(null);
                          artForm.resetFields();
                          artForm.setFieldsValue({
                            year: 2024,
                            indexType: 'SCI-Expanded',
                          });
                          setArticleModal(true);
                        }}
                      >
                        Yeni Makale Ekle
                      </Button>
                    </div>

                    <Table
                      dataSource={profile.articles.map((a) => ({ ...a, key: a.id }))}
                      pagination={false}
                      columns={[
                        {
                          title: 'Yıl & İndeks',
                          key: 'index',
                          width: 160,
                          render: (_: unknown, r: ArticleEntry) => (
                            <div>
                              <Tag color="geekblue" style={{ fontWeight: 700 }}>{r.year}</Tag>
                              <Tag color="purple">{r.indexType}</Tag>
                            </div>
                          ),
                        },
                        {
                          title: 'Makale Başlığı & Dergi',
                          key: 'title',
                          render: (_: unknown, r: ArticleEntry) => (
                            <div>
                              <strong style={{ color: '#0f172a', fontSize: 14 }}>{r.title}</strong>
                              <div style={{ fontSize: 12, color: '#4f46e5', fontWeight: 600 }}>
                                {r.journal} {r.volumeIssue ? `(${r.volumeIssue})` : ''} {r.pages ? `ss. ${r.pages}` : ''}
                              </div>
                              {r.coAuthors && (
                                <div style={{ fontSize: 12, color: '#94a3b8' }}>Yazarlar: {r.coAuthors}</div>
                              )}
                            </div>
                          ),
                        },
                        {
                          title: 'DOI / Web Bağlantısı',
                          key: 'doi',
                          width: 240,
                          render: (_: unknown, r: ArticleEntry) => (
                            <div style={{ fontSize: 12, display: 'flex', flexDirection: 'column', gap: 3 }}>
                              {r.doi && (
                                <a
                                  href={`https://doi.org/${r.doi}`}
                                  target="_blank"
                                  rel="noreferrer"
                                  style={{ color: '#2563eb', fontFamily: 'monospace' }}
                                >
                                  <LinkOutlined style={{ marginRight: 4 }} />
                                  DOI: {r.doi}
                                </a>
                              )}
                              {r.url && (
                                <a
                                  href={r.url}
                                  target="_blank"
                                  rel="noreferrer"
                                  style={{ color: '#059669' }}
                                >
                                  <ExportOutlined style={{ marginRight: 4 }} />
                                  Tam Metin Erişimi
                                </a>
                              )}
                            </div>
                          ),
                        },
                        {
                          title: 'İşlemler',
                          key: 'act',
                          align: 'right',
                          width: 130,
                          render: (_: unknown, r: ArticleEntry) => (
                            <Space>
                              <Button
                                size="small"
                                type="link"
                                icon={<EditOutlined />}
                                onClick={() => {
                                  setEditingItem(r);
                                  artForm.setFieldsValue(r);
                                  setArticleModal(true);
                                }}
                              >
                                Düzenle
                              </Button>
                              <Popconfirm
                                title="Makale kaydını silmek istiyor musunuz?"
                                onConfirm={() => handleDeleteArticle(r.id)}
                              >
                                <Button size="small" type="link" danger icon={<DeleteOutlined />}>
                                  Sil
                                </Button>
                              </Popconfirm>
                            </Space>
                          ),
                        },
                      ]}
                    />
                  </div>
                ),
              },

              // 3. KİTAPLAR & KİTAP BÖLÜMLERİ
              {
                key: 'books',
                label: (
                  <Space>
                    <BookOutlined style={{ color: '#d97706' }} />
                    <span>Kitaplar ({profile.books.length})</span>
                  </Space>
                ),
                children: (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span style={{ fontSize: 13, color: '#64748b' }}>
                        Yayınlanan telif kitaplar, bölümler, çeviriler, ISBN ve yayıncı linkleri
                      </span>
                      <Button
                        type="primary"
                        icon={<PlusOutlined />}
                        style={{ backgroundColor: '#d97706' }}
                        onClick={() => {
                          setEditingItem(null);
                          bkForm.resetFields();
                          bkForm.setFieldsValue({
                            year: 2023,
                            type: 'Telif Kitap',
                          });
                          setBookModal(true);
                        }}
                      >
                        Yeni Kitap Ekle
                      </Button>
                    </div>

                    <Table
                      dataSource={profile.books.map((b) => ({ ...b, key: b.id }))}
                      pagination={false}
                      columns={[
                        {
                          title: 'Kitap Türü & Yıl',
                          key: 'type',
                          width: 150,
                          render: (_: unknown, r: BookEntry) => (
                            <div>
                              <Tag color="orange" style={{ fontWeight: 600 }}>{r.type}</Tag>
                              <div style={{ fontSize: 12, color: '#64748b', marginTop: 2 }}>{r.year}</div>
                            </div>
                          ),
                        },
                        {
                          title: 'Kitap / Bölüm Başlığı & Yayınevi',
                          key: 'bookTitle',
                          render: (_: unknown, r: BookEntry) => (
                            <div>
                              <strong style={{ color: '#0f172a', fontSize: 14 }}>{r.title}</strong>
                              {r.chapterTitle && (
                                <div style={{ fontSize: 12, color: '#b45309', fontWeight: 600 }}>
                                  Bölüm: {r.chapterTitle}
                                </div>
                              )}
                              <div style={{ fontSize: 12, color: '#64748b' }}>
                                Yayınevi: {r.publisher} {r.city ? `(${r.city})` : ''}
                              </div>
                            </div>
                          ),
                        },
                        {
                          title: 'ISBN & Erişim Linki',
                          key: 'isbn',
                          width: 220,
                          render: (_: unknown, r: BookEntry) => (
                            <div style={{ fontSize: 12 }}>
                              {r.isbn && <div style={{ fontFamily: 'monospace' }}>ISBN: {r.isbn}</div>}
                              {r.url && (
                                <a href={r.url} target="_blank" rel="noreferrer" style={{ color: '#d97706' }}>
                                  <ExportOutlined style={{ marginRight: 4 }} />
                                  Kitap Sayfası / Link
                                </a>
                              )}
                            </div>
                          ),
                        },
                        {
                          title: 'İşlemler',
                          key: 'act',
                          align: 'right',
                          width: 130,
                          render: (_: unknown, r: BookEntry) => (
                            <Space>
                              <Button
                                size="small"
                                type="link"
                                icon={<EditOutlined />}
                                onClick={() => {
                                  setEditingItem(r);
                                  bkForm.setFieldsValue(r);
                                  setBookModal(true);
                                }}
                              >
                                Düzenle
                              </Button>
                              <Popconfirm
                                title="Kitap kaydını silmek istiyor musunuz?"
                                onConfirm={() => handleDeleteBook(r.id)}
                              >
                                <Button size="small" type="link" danger icon={<DeleteOutlined />}>
                                  Sil
                                </Button>
                              </Popconfirm>
                            </Space>
                          ),
                        },
                      ]}
                    />
                  </div>
                ),
              },

              // 4. BİLDİRİLER & KONFERANSLAR
              {
                key: 'conferences',
                label: (
                  <Space>
                    <SolutionOutlined style={{ color: '#0284c7' }} />
                    <span>Bildiriler ({profile.conferences.length})</span>
                  </Space>
                ),
                children: (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span style={{ fontSize: 13, color: '#64748b' }}>
                        Kongrelerde sunulan sözlü/poster bildiriler ve bildiri kitapçığı linkleri
                      </span>
                      <Button
                        type="primary"
                        icon={<PlusOutlined />}
                        style={{ backgroundColor: '#0284c7' }}
                        onClick={() => {
                          setEditingItem(null);
                          confForm.resetFields();
                          confForm.setFieldsValue({
                            presentationType: 'Sözlü Sunum',
                            publishedType: 'Tam Metin',
                          });
                          setConferenceModal(true);
                        }}
                      >
                        Bildiri Ekle
                      </Button>
                    </div>

                    <Table
                      dataSource={profile.conferences.map((c) => ({ ...c, key: c.id }))}
                      pagination={false}
                      columns={[
                        {
                          title: 'Sunum & Yayın Şekli',
                          key: 'type',
                          width: 160,
                          render: (_: unknown, r: ConferenceEntry) => (
                            <div>
                              <Tag color="cyan">{r.presentationType}</Tag>
                              <Tag color="blue">{r.publishedType}</Tag>
                            </div>
                          ),
                        },
                        {
                          title: 'Bildiri & Konferans Adı',
                          key: 'title',
                          render: (_: unknown, r: ConferenceEntry) => (
                            <div>
                              <strong style={{ color: '#0f172a', fontSize: 14 }}>{r.title}</strong>
                              <div style={{ fontSize: 12, color: '#0284c7', fontWeight: 600 }}>
                                {r.conferenceName}
                              </div>
                              <div style={{ fontSize: 12, color: '#94a3b8' }}>
                                {r.cityCountry} &bull; {r.date}
                              </div>
                            </div>
                          ),
                        },
                        {
                          title: 'Bildiri Metni Linki',
                          key: 'url',
                          width: 200,
                          render: (_: unknown, r: ConferenceEntry) =>
                            r.url ? (
                              <a href={r.url} target="_blank" rel="noreferrer" style={{ color: '#0284c7', fontSize: 12 }}>
                                <ExportOutlined style={{ marginRight: 4 }} />
                                Bildiri Linki
                              </a>
                            ) : (
                              '—'
                            ),
                        },
                        {
                          title: 'İşlemler',
                          key: 'act',
                          align: 'right',
                          width: 130,
                          render: (_: unknown, r: ConferenceEntry) => (
                            <Space>
                              <Button
                                size="small"
                                type="link"
                                icon={<EditOutlined />}
                                onClick={() => {
                                  setEditingItem(r);
                                  confForm.setFieldsValue(r);
                                  setConferenceModal(true);
                                }}
                              >
                                Düzenle
                              </Button>
                              <Popconfirm
                                title="Bildiriyi silmek istiyor musunuz?"
                                onConfirm={() => handleDeleteConference(r.id)}
                              >
                                <Button size="small" type="link" danger icon={<DeleteOutlined />}>
                                  Sil
                                </Button>
                              </Popconfirm>
                            </Space>
                          ),
                        },
                      ]}
                    />
                  </div>
                ),
              },

              // 5. PROJELER & AR-GE
              {
                key: 'projects',
                label: (
                  <Space>
                    <ProjectOutlined style={{ color: '#7c3aed' }} />
                    <span>Projeler & Ar-Ge ({profile.projects.length})</span>
                  </Space>
                ),
                children: (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span style={{ fontSize: 13, color: '#64748b' }}>
                        TÜBİTAK, BAP, AB Horizon ve Sanayi projeleri; bütçe, tarihler ve proje linkleri
                      </span>
                      <Button
                        type="primary"
                        icon={<PlusOutlined />}
                        style={{ backgroundColor: '#7c3aed' }}
                        onClick={() => {
                          setEditingItem(null);
                          prjForm.resetFields();
                          prjForm.setFieldsValue({
                            agency: 'TÜBİTAK 1001',
                            projectRole: 'Yürütücü',
                            currency: 'TRY',
                            status: 'Devam Ediyor',
                          });
                          setProjectModal(true);
                        }}
                      >
                        Yeni Proje Ekle
                      </Button>
                    </div>

                    <Table
                      dataSource={profile.projects.map((p) => ({ ...p, key: p.id }))}
                      pagination={false}
                      columns={[
                        {
                          title: 'Kurum & Görev',
                          key: 'agency',
                          width: 180,
                          render: (_: unknown, r: ProjectEntry) => (
                            <div>
                              <Tag color="purple" style={{ fontWeight: 700 }}>{r.agency}</Tag>
                              <Tag color="gold">{r.projectRole}</Tag>
                            </div>
                          ),
                        },
                        {
                          title: 'Proje Başlığı & Tarihler',
                          key: 'title',
                          render: (_: unknown, r: ProjectEntry) => (
                            <div>
                              <strong style={{ color: '#0f172a', fontSize: 14 }}>{r.title}</strong>
                              <div style={{ fontSize: 12, color: '#64748b', fontFamily: 'monospace' }}>
                                {r.startDate} &mdash; {r.endDate || 'Devam Ediyor'}
                              </div>
                            </div>
                          ),
                        },
                        {
                          title: 'Bütçe & Durum',
                          key: 'budget',
                          width: 160,
                          render: (_: unknown, r: ProjectEntry) => (
                            <div>
                              {r.budget && (
                                <strong style={{ color: '#059669', fontSize: 12, display: 'block' }}>
                                  {r.budget} {r.currency}
                                </strong>
                              )}
                              <Tag color={r.status === 'Tamamlandı' ? 'green' : 'blue'}>
                                {r.status}
                              </Tag>
                            </div>
                          ),
                        },
                        {
                          title: 'İşlemler',
                          key: 'act',
                          align: 'right',
                          width: 130,
                          render: (_: unknown, r: ProjectEntry) => (
                            <Space>
                              <Button
                                size="small"
                                type="link"
                                icon={<EditOutlined />}
                                onClick={() => {
                                  setEditingItem(r);
                                  prjForm.setFieldsValue(r);
                                  setProjectModal(true);
                                }}
                              >
                                Düzenle
                              </Button>
                              <Popconfirm
                                title="Projeyi silmek istiyor musunuz?"
                                onConfirm={() => handleDeleteProject(r.id)}
                              >
                                <Button size="small" type="link" danger icon={<DeleteOutlined />}>
                                  Sil
                                </Button>
                              </Popconfirm>
                            </Space>
                          ),
                        },
                      ]}
                    />
                  </div>
                ),
              },

              // 6. VERİLEN DERSLER
              {
                key: 'courses',
                label: <span>Dersler ({profile.courses.length})</span>,
                children: (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span style={{ fontSize: 13, color: '#64748b' }}>
                        Eğitmenin lisans ve lisansüstü yürüttüğü dersler ve haftalık ders yükleri
                      </span>
                      <Button
                        type="primary"
                        icon={<PlusOutlined />}
                        onClick={() => {
                          setEditingItem(null);
                          crsForm.resetFields();
                          crsForm.setFieldsValue({
                            term: 'Güz',
                            level: 'Lisans',
                            hoursPerWeek: 3,
                            language: 'Türkçe',
                          });
                          setCourseModal(true);
                        }}
                      >
                        Ders Ekle
                      </Button>
                    </div>

                    <Table
                      dataSource={profile.courses.map((c) => ({ ...c, key: c.id }))}
                      pagination={false}
                      columns={[
                        {
                          title: 'Ders Kodu',
                          dataIndex: 'courseCode',
                          key: 'courseCode',
                          width: 130,
                          render: (c: string) => <Tag color="geekblue" style={{ fontWeight: 700 }}>{c}</Tag>,
                        },
                        {
                          title: 'Ders Adı',
                          dataIndex: 'courseName',
                          key: 'courseName',
                          render: (n: string) => <strong style={{ color: '#0f172a' }}>{n}</strong>,
                        },
                        {
                          title: 'Dönem & Düzey',
                          key: 'level',
                          width: 170,
                          render: (_: unknown, r: CourseEntry) => (
                            <span>{r.level} &bull; {r.term}</span>
                          ),
                        },
                        {
                          title: 'Haftalık Saat / Dil',
                          key: 'hours',
                          width: 150,
                          render: (_: unknown, r: CourseEntry) => (
                            <span style={{ fontSize: 12 }}>{r.hoursPerWeek} Saat / {r.language}</span>
                          ),
                        },
                        {
                          title: 'İşlemler',
                          key: 'act',
                          align: 'right',
                          width: 130,
                          render: (_: unknown, r: CourseEntry) => (
                            <Space>
                              <Button
                                size="small"
                                type="link"
                                icon={<EditOutlined />}
                                onClick={() => {
                                  setEditingItem(r);
                                  crsForm.setFieldsValue(r);
                                  setCourseModal(true);
                                }}
                              >
                                Düzenle
                              </Button>
                              <Popconfirm
                                title="Dersi silmek istiyor musunuz?"
                                onConfirm={() => handleDeleteCourse(r.id)}
                              >
                                <Button size="small" type="link" danger icon={<DeleteOutlined />}>
                                  Sil
                                </Button>
                              </Popconfirm>
                            </Space>
                          ),
                        },
                      ]}
                    />
                  </div>
                ),
              },

              // 7. TEZ DANIŞMANLIKLARI
              {
                key: 'advisings',
                label: <span>Danışmanlıklar ({profile.advisings.length})</span>,
                children: (
                  <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span style={{ fontSize: 13, color: '#64748b' }}>
                        Yönetilen yüksek lisans ve doktora tezleri, mezun edilen öğrenciler
                      </span>
                      <Button
                        type="primary"
                        icon={<PlusOutlined />}
                        onClick={() => {
                          setEditingItem(null);
                          advForm.resetFields();
                          advForm.setFieldsValue({
                            level: 'Yüksek Lisans',
                            year: 2024,
                            status: 'Devam Ediyor',
                          });
                          setAdvisingModal(true);
                        }}
                      >
                        Danışmanlık Ekle
                      </Button>
                    </div>

                    <Table
                      dataSource={profile.advisings.map((a) => ({ ...a, key: a.id }))}
                      pagination={false}
                      columns={[
                        {
                          title: 'Düzey',
                          dataIndex: 'level',
                          key: 'level',
                          width: 140,
                          render: (lvl: string) => <Tag color="blue">{lvl}</Tag>,
                        },
                        {
                          title: 'Öğrenci Adı Soyadı & Tez Konusu',
                          key: 'student',
                          render: (_: unknown, r: AdvisingEntry) => (
                            <div>
                              <strong style={{ color: '#0f172a' }}>{r.studentName}</strong>
                              <div style={{ fontSize: 12, color: '#64748b' }}>Tez: {r.thesisTitle}</div>
                            </div>
                          ),
                        },
                        {
                          title: 'Yıl & Durum',
                          key: 'status',
                          width: 160,
                          render: (_: unknown, r: AdvisingEntry) => (
                            <div>
                              <span style={{ fontSize: 12, fontFamily: 'monospace' }}>{r.year}</span>
                              <div>
                                <Tag color={r.status === 'Tamamlandı' ? 'green' : 'blue'}>
                                  {r.status}
                                </Tag>
                              </div>
                            </div>
                          ),
                        },
                        {
                          title: 'İşlemler',
                          key: 'act',
                          align: 'right',
                          width: 130,
                          render: (_: unknown, r: AdvisingEntry) => (
                            <Space>
                              <Button
                                size="small"
                                type="link"
                                icon={<EditOutlined />}
                                onClick={() => {
                                  setEditingItem(r);
                                  advForm.setFieldsValue(r);
                                  setAdvisingModal(true);
                                }}
                              >
                                Düzenle
                              </Button>
                              <Popconfirm
                                title="Danışmanlık kaydını silmek istiyor musunuz?"
                                onConfirm={() => handleDeleteAdvising(r.id)}
                              >
                                <Button size="small" type="link" danger icon={<DeleteOutlined />}>
                                  Sil
                                </Button>
                              </Popconfirm>
                            </Space>
                          ),
                        },
                      ]}
                    />
                  </div>
                ),
              },
            ]}
          />
        </div>
      )}

      {/* AÇIK ERİŞİM MODAL (ORCID, SCHOLAR, YOKSIS VB.) */}
      <Modal
        title="Açık Erişim, ORCID ve Google Scholar Profili"
        open={openAccessModal}
        onCancel={() => setOpenAccessModal(false)}
        footer={null}
        width={560}
      >
        <Form form={openAccessForm} layout="vertical" onFinish={handleSaveOpenAccess}>
          <Form.Item
            name="orcidId"
            label={<span style={{ fontWeight: 600 }}>ORCID ID (0000-0000-0000-0000)</span>}
          >
            <Input placeholder="Örn: 0000-0002-1825-0097" />
          </Form.Item>

          <Form.Item
            name="orcidUrl"
            label={<span style={{ fontWeight: 600 }}>ORCID Profil Doğrudan URL Linki</span>}
          >
            <Input placeholder="https://orcid.org/0000-0002-1825-0097" />
          </Form.Item>

          <Form.Item
            name="scholarUrl"
            label={<span style={{ fontWeight: 600 }}>Google Scholar Profil URL Linki</span>}
          >
            <Input placeholder="https://scholar.google.com/citations?user=..." />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="yoksisId" label={<span style={{ fontWeight: 600 }}>YÖKSİS Araştırmacı No</span>}>
                <Input placeholder="1098421" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="scopusId" label={<span style={{ fontWeight: 600 }}>Scopus Author ID</span>}>
                <Input placeholder="57193829100" />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="personalWebsite" label={<span style={{ fontWeight: 600 }}>Kişisel Web Sayfası / Blog URL</span>}>
            <Input placeholder="https://akademisyen.edu.tr" />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setOpenAccessModal(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#6366f1', borderRadius: 6 }}>
              Bağlantıları Kaydet
            </Button>
          </div>
        </Form>
      </Modal>

      {/* ÖĞRENİM FORMU MODAL */}
      <Modal
        title={editingItem ? 'Öğrenim Bilgisini Düzenle' : 'Yeni Öğrenim Bilgisi Ekle'}
        open={educationModal}
        onCancel={() => setEducationModal(false)}
        footer={null}
        width={540}
      >
        <Form form={eduForm} layout="vertical" onFinish={handleSaveEducation}>
          <Form.Item name="degree" label="Derece / Kademe" rules={[{ required: true }]}>
            <Select
              options={[
                { value: 'Lisans', label: 'Lisans' },
                { value: 'Yüksek Lisans', label: 'Yüksek Lisans' },
                { value: 'Doktora', label: 'Doktora' },
                { value: 'Tıpta Uzmanlık', label: 'Tıpta Uzmanlık' },
                { value: 'Sanatta Yeterlik', label: 'Sanatta Yeterlik' },
                { value: 'Post-Doc', label: 'Post-Doc' },
              ]}
            />
          </Form.Item>

          <Form.Item name="university" label="Üniversite Adı" rules={[{ required: true }]}>
            <Input placeholder="Örn: İstanbul Teknik Üniversitesi" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="faculty" label="Fakülte / Enstitü">
                <Input placeholder="Örn: Fen Bilimleri Enstitüsü" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="department" label="Bölüm / Anabilim Dalı">
                <Input placeholder="Örn: Bilgisayar Mühendisliği" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="startYear" label="Başlangıç Yılı" rules={[{ required: true }]}>
                <InputNumber style={{ width: '100%' }} min={1960} max={2030} />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="endYear" label="Mezuniyet Yılı">
                <InputNumber style={{ width: '100%' }} min={1960} max={2030} />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="thesisTitle" label="Tez Başlığı">
            <Input placeholder="Tez konusu / başlığı" />
          </Form.Item>

          <Form.Item name="thesisAdvisor" label="Tez Danışmanı">
            <Input placeholder="Prof. Dr. Ad Soyad" />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setEducationModal(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0f4c81', borderRadius: 6 }}>
              Kaydet
            </Button>
          </div>
        </Form>
      </Modal>

      {/* MAKALE FORMU MODAL */}
      <Modal
        title={editingItem ? 'Makaleyi Düzenle' : 'Yeni Makale Ekle'}
        open={articleModal}
        onCancel={() => setArticleModal(false)}
        footer={null}
        width={580}
      >
        <Form form={artForm} layout="vertical" onFinish={handleSaveArticle}>
          <Form.Item name="title" label="Makale Başlığı" rules={[{ required: true }]}>
            <Input placeholder="Makalenin tam başlığı" />
          </Form.Item>

          <Form.Item name="journal" label="Yayınlandığı Dergi" rules={[{ required: true }]}>
            <Input placeholder="Örn: IEEE Transactions on Cloud Computing" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="indexType" label="İndeks Türü" rules={[{ required: true }]}>
                <Select
                  options={[
                    { value: 'SCI-Expanded', label: 'SCI-Expanded' },
                    { value: 'SSCI', label: 'SSCI' },
                    { value: 'AHCI', label: 'AHCI' },
                    { value: 'ESCI', label: 'ESCI' },
                    { value: 'Scopus', label: 'Scopus' },
                    { value: 'TR Dizin', label: 'TR Dizin' },
                    { value: 'Diğer Uluslararası', label: 'Diğer Uluslararası' },
                    { value: 'Ulusal Hakemli', label: 'Ulusal Hakemli' },
                  ]}
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="year" label="Yayın Yılı" rules={[{ required: true }]}>
                <InputNumber style={{ width: '100%' }} min={1970} max={2030} />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="volumeIssue" label="Cilt / Sayı">
                <Input placeholder="Vol. 12, Issue 3" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="pages" label="Sayfa Aralığı">
                <Input placeholder="110-125" />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="doi" label="DOI Numarası">
            <Input placeholder="10.1109/TSC.2024.123456" />
          </Form.Item>

          <Form.Item name="url" label="Makale / Tam Metin Web Linki">
            <Input placeholder="https://doi.org/... veya https://dergipark.org.tr/..." />
          </Form.Item>

          <Form.Item name="coAuthors" label="Ortak Yazarlar">
            <Input placeholder="A. Kaya, M. Demir, E. Yılmaz" />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setArticleModal(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#059669', borderRadius: 6 }}>
              Kaydet
            </Button>
          </div>
        </Form>
      </Modal>

      {/* KİTAP FORMU MODAL */}
      <Modal
        title={editingItem ? 'Kitap Bilgisini Düzenle' : 'Yeni Kitap / Bölüm Ekle'}
        open={bookModal}
        onCancel={() => setBookModal(false)}
        footer={null}
        width={560}
      >
        <Form form={bkForm} layout="vertical" onFinish={handleSaveBook}>
          <Form.Item name="title" label="Kitap Başlığı" rules={[{ required: true }]}>
            <Input placeholder="Kitabın tam başlığı" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="type" label="Yayın Türü" rules={[{ required: true }]}>
                <Select
                  options={[
                    { value: 'Telif Kitap', label: 'Telif Kitap' },
                    { value: 'Kitap Bölümü', label: 'Kitap Bölümü' },
                    { value: 'Kitap Çevirisi', label: 'Kitap Çevirisi' },
                    { value: 'Editörlük', label: 'Editörlük' },
                  ]}
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="year" label="Basım Yılı" rules={[{ required: true }]}>
                <InputNumber style={{ width: '100%' }} min={1970} max={2030} />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="chapterTitle" label="Bölüm Başlığı (Kitap Bölümü ise)">
            <Input placeholder="İlgili bölüm adı" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="publisher" label="Yayınevi" rules={[{ required: true }]}>
                <Input placeholder="Springer, Seçkin, vb." />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="city" label="Basım Şehri / Ülke">
                <Input placeholder="Ankara / Berlin" />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="isbn" label="ISBN Numarası">
            <Input placeholder="978-975-02-8492-1" />
          </Form.Item>

          <Form.Item name="url" label="Kitap Satış / Web Linki">
            <Input placeholder="https://..." />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setBookModal(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#d97706', borderRadius: 6 }}>
              Kaydet
            </Button>
          </div>
        </Form>
      </Modal>

      {/* BİLDİRİ FORMU MODAL */}
      <Modal
        title={editingItem ? 'Bildiriyi Düzenle' : 'Yeni Bildiri Ekle'}
        open={conferenceModal}
        onCancel={() => setConferenceModal(false)}
        footer={null}
        width={560}
      >
        <Form form={confForm} layout="vertical" onFinish={handleSaveConference}>
          <Form.Item name="title" label="Bildiri Başlığı" rules={[{ required: true }]}>
            <Input placeholder="Bildirinin adı" />
          </Form.Item>

          <Form.Item name="conferenceName" label="Konferans / Sempozyum Adı" rules={[{ required: true }]}>
            <Input placeholder="Örn: Uluslararası Bilgisayar Kongresi (UBK 2025)" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="presentationType" label="Sunum Şekli">
                <Select
                  options={[
                    { value: 'Sözlü Sunum', label: 'Sözlü Sunum' },
                    { value: 'Poster Sunum', label: 'Poster Sunum' },
                    { value: 'Davetli Konuşmacı', label: 'Davetli Konuşmacı' },
                  ]}
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="publishedType" label="Basım Şekli">
                <Select
                  options={[
                    { value: 'Tam Metin', label: 'Tam Metin Bildiri' },
                    { value: 'Özet Metin', label: 'Özet Metin Bildiri' },
                  ]}
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="cityCountry" label="Şehir & Ülke">
                <Input placeholder="İstanbul, Türkiye" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="date" label="Tarih / Tarih Aralığı">
                <Input placeholder="12-14 Ekim 2025" />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="url" label="Bildiri Metni Linki">
            <Input placeholder="https://..." />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setConferenceModal(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#0284c7', borderRadius: 6 }}>
              Kaydet
            </Button>
          </div>
        </Form>
      </Modal>

      {/* PROJE FORMU MODAL */}
      <Modal
        title={editingItem ? 'Projeyi Düzenle' : 'Yeni Proje Ekle'}
        open={projectModal}
        onCancel={() => setProjectModal(false)}
        footer={null}
        width={560}
      >
        <Form form={prjForm} layout="vertical" onFinish={handleSaveProject}>
          <Form.Item name="title" label="Proje Başlığı" rules={[{ required: true }]}>
            <Input placeholder="Ar-Ge / Proje başlığı" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="agency" label="Destekleyen Program / Kurum" rules={[{ required: true }]}>
                <Select
                  options={[
                    { value: 'TÜBİTAK 1001', label: 'TÜBİTAK 1001' },
                    { value: 'TÜBİTAK 1002', label: 'TÜBİTAK 1002' },
                    { value: 'TÜBİTAK 3501', label: 'TÜBİTAK 3501' },
                    { value: 'BAP', label: 'BAP (Bilimsel Araştırma Projeleri)' },
                    { value: 'AB Horizon', label: 'AB Horizon Europe' },
                    { value: 'Erasmus+', label: 'Erasmus+ KA2' },
                    { value: 'Sanayi / Ar-Ge', label: 'Sanayi / TEYDEB' },
                    { value: 'Diğer', label: 'Diğer' },
                  ]}
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="projectRole" label="Projedeki Görevi" rules={[{ required: true }]}>
                <Select
                  options={[
                    { value: 'Yürütücü', label: 'Yürütücü' },
                    { value: 'Araştırmacı', label: 'Araştırmacı' },
                    { value: 'Danışman', label: 'Danışman' },
                    { value: 'Bursiyer', label: 'Bursiyer' },
                  ]}
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="budget" label="Bütçe Tutarı">
                <Input placeholder="Örn: 1.500.000" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="currency" label="Para Birimi">
                <Select
                  options={[
                    { value: 'TRY', label: 'TRY (₺)' },
                    { value: 'USD', label: 'USD ($)' },
                    { value: 'EUR', label: 'EUR (€)' },
                  ]}
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="startDate" label="Başlangıç Tarihi">
                <Input placeholder="YYYY-AA-GG veya 2024" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="endDate" label="Bitiş Tarihi">
                <Input placeholder="YYYY-AA-GG veya Devam Ediyor" />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="status" label="Proje Durumu">
            <Select
              options={[
                { value: 'Tamamlandı', label: 'Tamamlandı' },
                { value: 'Devam Ediyor', label: 'Devam Ediyor' },
                { value: 'Öneri Aşamasında', label: 'Öneri Aşamasında' },
              ]}
            />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setProjectModal(false)}>İptal</Button>
            <Button type="primary" htmlType="submit" style={{ backgroundColor: '#7c3aed', borderRadius: 6 }}>
              Kaydet
            </Button>
          </div>
        </Form>
      </Modal>

      {/* DERS FORMU MODAL */}
      <Modal
        title={editingItem ? 'Dersi Düzenle' : 'Yeni Ders Ekle'}
        open={courseModal}
        onCancel={() => setCourseModal(false)}
        footer={null}
        width={500}
      >
        <Form form={crsForm} layout="vertical" onFinish={handleSaveCourse}>
          <Row gutter={16}>
            <Col span={10}>
              <Form.Item name="courseCode" label="Ders Kodu" rules={[{ required: true }]}>
                <Input placeholder="BLG-411" />
              </Form.Item>
            </Col>
            <Col span={14}>
              <Form.Item name="courseName" label="Ders Adı" rules={[{ required: true }]}>
                <Input placeholder="Dağıtık Sistemler" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="term" label="Dönem">
                <Select
                  options={[
                    { value: 'Güz', label: 'Güz' },
                    { value: 'Bahar', label: 'Bahar' },
                    { value: 'Yaz', label: 'Yaz' },
                  ]}
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="level" label="Öğrenim Düzeyi">
                <Select
                  options={[
                    { value: 'Ön Lisans', label: 'Ön Lisans' },
                    { value: 'Lisans', label: 'Lisans' },
                    { value: 'Yüksek Lisans', label: 'Yüksek Lisans' },
                    { value: 'Doktora', label: 'Doktora' },
                  ]}
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="hoursPerWeek" label="Haftalık Saat">
                <InputNumber style={{ width: '100%' }} min={1} max={20} />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="language" label="Öğretim Dili">
                <Input placeholder="Türkçe / İngilizce" />
              </Form.Item>
            </Col>
          </Row>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setCourseModal(false)}>İptal</Button>
            <Button type="primary" htmlType="submit">Kaydet</Button>
          </div>
        </Form>
      </Modal>

      {/* DANIŞMANLIK FORMU MODAL */}
      <Modal
        title={editingItem ? 'Danışmanlık Düzenle' : 'Yeni Tez Danışmanlığı'}
        open={advisingModal}
        onCancel={() => setAdvisingModal(false)}
        footer={null}
        width={500}
      >
        <Form form={advForm} layout="vertical" onFinish={handleSaveAdvising}>
          <Form.Item name="studentName" label="Öğrenci Adı Soyadı" rules={[{ required: true }]}>
            <Input placeholder="Öğrencinin adı" />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="level" label="Düzey">
                <Select
                  options={[
                    { value: 'Yüksek Lisans', label: 'Yüksek Lisans' },
                    { value: 'Doktora', label: 'Doktora' },
                  ]}
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="year" label="Mezuniyet / Kayıt Yılı">
                <InputNumber style={{ width: '100%' }} min={2000} max={2030} />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="thesisTitle" label="Tez Konusu / Başlığı" rules={[{ required: true }]}>
            <Input placeholder="Tezin konusu" />
          </Form.Item>

          <Form.Item name="status" label="Tez Durumu">
            <Select
              options={[
                { value: 'Devam Ediyor', label: 'Devam Ediyor' },
                { value: 'Tamamlandı', label: 'Tamamlandı / Mezun' },
              ]}
            />
          </Form.Item>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10, marginTop: 16 }}>
            <Button onClick={() => setAdvisingModal(false)}>İptal</Button>
            <Button type="primary" htmlType="submit">Kaydet</Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};
