import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ConfigProvider } from 'antd';
import trTR from 'antd/locale/tr_TR';
import { corporateTheme } from './styles/theme';
import { AuthProvider, useAuth } from './context/AuthContext';
import { LanguageProvider } from './context/LanguageContext';
import { LoginPage } from './pages/login/LoginPage';
import { ForgotPasswordPage } from './pages/login/ForgotPasswordPage';
import { MainLayout } from './components/layout/MainLayout';
import { DashboardPage } from './pages/dashboard/DashboardPage';
import { EducationCatalogPage } from './pages/catalog/EducationCatalogPage';
import { InstitutionalEvaluationPage } from './pages/evaluation/InstitutionalEvaluationPage';
import { SocialContributionPage } from './pages/social/SocialContributionPage';
import { TranscriptPage } from './pages/student/TranscriptPage';
import { CourseRegistrationPage } from './pages/student/CourseRegistrationPage';
import { StudentApplicationsPage } from './pages/student/StudentApplicationsPage';
import { StudentDetailsPage } from './pages/student/StudentDetailsPage';
import { AcademicPortalPage } from './pages/academic/AcademicPortalPage';
import { DepartmentManagementPage } from './pages/department/DepartmentManagementPage';
import { StudentAffairsPage } from './pages/affairs/StudentAffairsPage';
import { FacultiesPage } from './pages/affairs/FacultiesPage';
import { DepartmentsPage } from './pages/affairs/DepartmentsPage';
import { CoursesPage } from './pages/affairs/CoursesPage';
import { InstructorsPage } from './pages/affairs/InstructorsPage';
import { StudentsManagementPage } from './pages/users/StudentsManagementPage';
import { StaffManagementPage } from './pages/users/StaffManagementPage';
import { GuestStudentsPage } from './pages/users/GuestStudentsPage';
import { InstructorProfilePage } from './pages/affairs/InstructorProfilePage';
import { AdminUnitsPage } from './pages/admin/AdminUnitsPage';
import { StaffTitlesPage } from './pages/admin/StaffTitlesPage';
import { SystemRolesPage } from './pages/admin/SystemRolesPage';
import { InstitutionSettingsPage } from './pages/settings/InstitutionSettingsPage';
import { LoginPageSettingsPage } from './pages/settings/LoginPageSettingsPage';

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div style={{ height: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        Yükleniyor...
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

const RoleIndexRedirect: React.FC = () => {
  const { activeRole } = useAuth();
  if (activeRole === 'ACADEMIC') {
    return <Navigate to="/academic/portal" replace />;
  }
  if (activeRole === 'STAFF' || activeRole === 'ADMIN') {
    return <Navigate to="/affairs/console" replace />;
  }
  return <Navigate to="/dashboard" replace />;
};

export const App: React.FC = () => {
  return (
    <ConfigProvider theme={corporateTheme} locale={trTR}>
      <LanguageProvider>
        <AuthProvider>
          <BrowserRouter>
            <Routes>
              {/* Giriş Sayfaları */}
              <Route path="/login" element={<LoginPage />} />
              <Route path="/forgot-password" element={<ForgotPasswordPage />} />

              {/* Korumalı OBS Portalı */}
              <Route
                path="/"
                element={
                  <ProtectedRoute>
                    <MainLayout />
                  </ProtectedRoute>
                }
              >
                <Route index element={<RoleIndexRedirect />} />
                
                {/* 1. Öğrenci Portalı */}
                <Route path="dashboard" element={<DashboardPage />} />
                <Route path="student/transcript" element={<TranscriptPage />} />
                <Route path="student/course-registration" element={<CourseRegistrationPage />} />
                <Route path="student/applications" element={<StudentApplicationsPage />} />
                <Route path="student/details" element={<StudentDetailsPage />} />

                {/* 2. Öğretim Üyesi / Akademik Portal */}
                <Route path="academic/portal" element={<AcademicPortalPage />} />

                {/* 3. Fakülte & Bölüm Başkanlığı */}
                <Route path="department/management" element={<DepartmentManagementPage />} />

                {/* 4. Kullanıcı & Nüfus Yönetimi */}
                <Route path="users/students" element={<StudentsManagementPage />} />
                <Route path="users/instructors" element={<InstructorsPage />} />
                <Route path="users/staff" element={<StaffManagementPage />} />
                <Route path="users/guest-students" element={<GuestStudentsPage />} />

                {/* 5. Öğrenim, Yayın & Araştırma Masası (Detaylı Profil) */}
                <Route path="affairs/instructors/profile" element={<InstructorProfilePage />} />
                <Route path="affairs/instructors/:id/profile" element={<InstructorProfilePage />} />

                {/* 6. İdari Teşkilat & Personel Masası */}
                <Route path="admin/staff" element={<StaffManagementPage />} />
                <Route path="admin/units" element={<AdminUnitsPage />} />
                <Route path="admin/staff-titles" element={<StaffTitlesPage />} />
                <Route path="admin/roles" element={<SystemRolesPage />} />

                {/* 7. Öğrenci İşleri Portalı & Akademik Yapı */}
                <Route path="affairs/console" element={<StudentAffairsPage />} />
                <Route path="affairs/faculties" element={<FacultiesPage />} />
                <Route path="affairs/departments" element={<DepartmentsPage />} />
                <Route path="affairs/instructors" element={<InstructorsPage />} />
                <Route path="affairs/courses" element={<CoursesPage />} />

                {/* 8. Genel Ayarlar */}
                <Route path="settings/institution" element={<InstitutionSettingsPage />} />
                <Route path="settings/login-page" element={<LoginPageSettingsPage />} />

                {/* 6. Ortak Modüller */}
                <Route path="education-catalog" element={<EducationCatalogPage />} />
                <Route path="institutional-evaluation" element={<InstitutionalEvaluationPage />} />
                <Route path="social-contribution" element={<SocialContributionPage />} />

                {/* Fallback */}
                <Route path="*" element={<RoleIndexRedirect />} />
              </Route>
            </Routes>
          </BrowserRouter>
        </AuthProvider>
      </LanguageProvider>
    </ConfigProvider>
  );
};

export default App;
