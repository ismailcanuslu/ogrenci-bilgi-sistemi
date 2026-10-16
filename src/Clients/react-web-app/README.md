# 🎓 Üniversite Öğrenci Bilgi Sistemi (OBS) — React Web SPA

Modern, yüksek performanslı ve kurumsal standartlarda geliştirilmiş **Öğrenci Bilgi Sistemi (OBS / ÜBYS)** istemci arayüzü.

## 🚀 Teknolojiler
* **React 19 & TypeScript 6**
* **Vite 8:** Anlık HMR ve hızlı üretim paketi
* **Ant Design (antd v6):** Zengin kurumsal tablolar, modal formlar ve istatistik kartları
* **Tailwind CSS & Vanilla CSS:** Duyarlı (responsive) gridler, cam efektleri (glassmorphism) ve animasyonlar
* **React Router v7:** Rol bazlı yönlendirme ve korumalı rotalar (`ProtectedRoute`, `RoleIndexRedirect`)
* **Context API:** `AuthContext` (JWT & Oturum), `LanguageContext` (TR, EN, DE, RU çoklu dil motoru)

## 🗂️ Modül & Sayfa Yapısı
* **`/login` & `/forgot-password`:** Giriş ekranı (arka plan slayt gösterisi, progress bar, e-Devlet & obsKimlik giriş desteği, animasyonlu şifre sıfırlama)
* **`/users/students`:** Asil Öğrenci Masası (Öğrenci listesi, kayıt dondurma, ilişik kesme, mezuniyet takibi)
* **`/users/guest-students`:** Misafir & Değişim Öğrencileri (Erasmus+, Farabi, Mevlana, Özel Öğrenci masası)
* **`/users/instructors` & `/affairs/instructors`:** Eğitmen & Akademisyenler Masası (Akademik kadro, unvan filtreleme)
* **`/affairs/instructors/profile`:** Öğrenim, Yayın & Araştırma Masası (Öğrenim dereceleri, makaleler, kitaplar, bildiriler, projeler, açık erişim: ORCID & Google Scholar bağlantıları)
* **`/admin/staff`:** İdari Personel Kadrosu Masası
* **`/admin/units`:** İdari Teşkilat Birimleri Masası (Daire Başkanlıkları, Koordinatörlükler, Sekreterlikler)
* **`/admin/staff-titles`:** İdari Kadro Unvanları Masası (Hizmet sınıfları, hiyerarşi sıralaması)
* **`/admin/roles`:** Yetki & Sistem Rolleri Masası (Detaylı işlem izinleri şablonu)
* **`/affairs/faculties` & `/affairs/departments`:** Fakülte ve Bölüm Teşkilatı Yönetimi (Dekan atama, dönemlik AKTS sınırları)
* **`/affairs/courses` & `/education-catalog`:** Bölüm Ders Kataloğu ve Eğitim Müfredatı
* **`/settings/institution` & `/settings/login-page`:** Üniversite Kurumsal Bilgileri & Giriş Ekranı Özelleştirme Masası

## 💻 Yerel Geliştirme (Local Development)

```bash
# Bağımlılıkları yükleyin
npm install

# Geliştirme sunucusunu başlatın
npm run dev
```

* **Tarayıcıda Açın:** `http://localhost:5173`

## 📦 Üretim Paketi Derleme (Production Build)

```bash
npm run build
```
TypeScript derleme kontrolü (`tsc -b`) ve Vite optimizasyonundan sonra `/dist` klasörüne statik varlıklar üretilir.
