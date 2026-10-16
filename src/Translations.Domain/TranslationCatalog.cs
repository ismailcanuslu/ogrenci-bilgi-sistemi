namespace Translations.Domain;

public static class TranslationCatalog
{
    private static readonly Dictionary<string, Dictionary<string, string>> Translations = new(StringComparer.OrdinalIgnoreCase)
    {
        [LanguageCode.Turkish] = new(StringComparer.OrdinalIgnoreCase)
        {
            // Security
            ["Security.InvalidCredentials"] = "Geçersiz e-posta veya parola.",
            ["Security.EmailAlreadyExists"] = "Bu e-posta adresi zaten kullanımda.",
            ["Security.UserNotFound"] = "Kullanıcı bulunamadı.",
            ["Security.InvalidRefreshToken"] = "Yenileme belirteci geçersiz veya süresi dolmuş.",
            ["Security.SessionRevoked"] = "Oturum sonlandırıldı.",
            ["Security.InvalidEmail"] = "Geçersiz e-posta formatı.",
            ["Security.WeakPassword"] = "Parola güvenlik kriterlerini karşılamıyor.",
            ["Security.AccountLocked"] = "Hesap çok fazla hatalı giriş nedeniyle kilitlendi.",

            // Files
            ["Files.FileNotFound"] = "İstenen dosya bulunamadı.",
            ["Files.FileTooLarge"] = "Dosya boyutu izin verilen sınırı aşıyor.",
            ["Files.InvalidSignature"] = "Dosya içeriği imza (magic byte) doğrulaması başarısız.",
            ["Files.EmptyFile"] = "Yüklenen dosya boş olamaz.",
            ["Files.AccessDenied"] = "Bu dosyaya erişim yetkiniz bulunmamaktadır.",
            ["Files.InvalidContentType"] = "Desteklenmeyen dosya türü.",
            ["Files.UploadFailed"] = "Dosya depolama alanına yüklenemedi.",
            ["Files.UnauthorizedUpload"] = "Bu dosyayı yükleme yetkiniz bulunmamaktadır.",

            // Notifications
            ["Notifications.InvalidEmail"] = "Geçersiz e-posta adresi formatı.",
            ["Notifications.InvalidPhoneNumber"] = "Geçersiz telefon numarası formatı.",
            ["Notifications.InvalidSubject"] = "Bildirim başlığı geçersiz.",
            ["Notifications.InvalidBody"] = "Bildirim gövdesi geçersiz veya boş.",
            ["Notifications.AlreadySent"] = "Bu bildirim zaten gönderilmiş.",
            ["Notifications.AlreadyCancelled"] = "Bu bildirim zaten iptal edilmiş.",
            ["Notifications.NotFound"] = "Bildirim kaydı bulunamadı.",
            ["Notifications.SendFailed"] = "Bildirim gönderimi başarısız oldu.",
            ["Notifications.DeliveryFailed"] = "E-posta teslimatı başarısız oldu.",
            ["Notifications.SmsDeliveryFailed"] = "SMS teslimatı başarısız oldu.",
            ["Notifications.RetryFailed"] = "Bildirim yeniden denemesi başarısız oldu.",

            // Auditing & Messaging
            ["Auditing.NotFound"] = "Denetim günlüğü kaydı bulunamadı.",
            ["Messaging.PublishFailed"] = "Mesaj yayınlanamadı.",
            ["Messaging.MessageNotFound"] = "İşlenecek outbox mesajı bulunamadı.",
            ["Scheduling.JobNotFound"] = "Zamanlanmış iş kaydı bulunamadı.",
            ["Webhooks.SubscriptionNotFound"] = "Webhook aboneliği bulunamadı.",
            ["Webhooks.InvalidUrl"] = "Geçersiz webhook hedef URL adresi.",
            ["Webhooks.DeliveryFailed"] = "Webhook bildirim teslimatı başarısız oldu.",
            ["Webhooks.InvalidSignature"] = "Webhook HMAC-SHA256 imzası geçersiz.",

            // Common / General
            ["Common.ValidationFailed"] = "Bir veya birden fazla doğrulama hatası oluştu.",
            ["Common.NotFound"] = "İstenen kaynak bulunamadı.",
            ["Common.Unauthorized"] = "Bu işlem için kimlik doğrulaması gereklidir.",
            ["Common.Forbidden"] = "Bu işlem için yetkiniz bulunmamaktadır.",
            ["Common.Conflict"] = "İşlem mevcut durumla çakışıyor.",
            ["Common.InternalError"] = "Beklenmeyen bir sunucu hatası meydana geldi.",

            // OBS - Academic & Programs
            ["Obs.SelectAcademicProgram"] = "Akademik Program Seçiniz",
            ["Obs.Transcript"] = "Transkript",
            ["Obs.MyCourses"] = "Derslerim",
            ["Obs.LiveCourses"] = "Canlı Derslerim",
            ["Obs.Calendar"] = "Akademik Takvim",
            ["Obs.OnlineExam"] = "Online Sınav",
            ["Obs.CourseRegistration"] = "Ders Seçimi ve Kayıt Yenileme",
            ["Obs.AlumniNetwork"] = "Mezun İletişim Sistemi",
            ["Obs.WeeklySchedule"] = "Haftalık Ders Programı",
            ["Obs.Announcements"] = "Duyurular",
            ["Obs.Surveys"] = "Anketler",
            ["Obs.Messages"] = "Mesajlar",
            ["Obs.AccountSettings"] = "Hesap Ayarları",
            ["Obs.PortalSettings"] = "Kullanıcı Portal Ayarları",
            ["Obs.PhoneBook"] = "Telefon Rehberi",
            ["Obs.SupportService"] = "ÜBYS Destek",
            ["Obs.InstitutionalEvaluation"] = "Kurumsal Değerlendirme",
            ["Obs.QuickShortcuts"] = "Kişisel Kısayollar",
            ["Obs.AddWidget"] = "Yeni Widget Ekle",
            ["Obs.CourseListWidget"] = "Ders Listesi (Widget)",

            // OBS - "Diğer" (Other 14 Items)
            ["Obs.TuitionInfo"] = "Harç Bilgileri",
            ["Obs.AdvisorInfo"] = "Danışman Bilgisi",
            ["Obs.HistoryInfo"] = "Tarihçe Bilgileri",
            ["Obs.RegistrationApprovalReport"] = "Kayıtlanma Onay Raporu",
            ["Obs.Activities"] = "Aktiviteler",
            ["Obs.DisenrollmentRequest"] = "İlişik Kesme Talebinde Bulun",
            ["Obs.NoObstacleTransferProcess"] = "Yatay Geçişte Engel Yoktur Sürecini Başlat",
            ["Obs.MilitaryStatus"] = "Askerlik Durumu",
            ["Obs.ProfilePictureManagement"] = "Profil Resmi Yönetimi",
            ["Obs.DisciplinaryRecords"] = "Ceza Bilgileri",
            ["Obs.LeaveOfAbsence"] = "İzin / Kayıt Dondurma Bilgileri",
            ["Obs.BankAccountIban"] = "IBAN Bilgileri",
            ["Obs.AdditionalTimeInfo"] = "Ek Süre Bilgileri",

            // OBS - Applications & Services
            ["Obs.DoubleMajorApplication"] = "ÇAP Başvurusu",
            ["Obs.MinorApplication"] = "Yandal Başvurusu",
            ["Obs.TransferApplication"] = "Yatay Geçiş Başvurusu",
            ["Obs.SummerSchoolExternalCourse"] = "Dış Kurum Yaz Okulu Ders Onay Başvurusu",
            ["Obs.DocumentRequest"] = "Resmi Belge Talebi",
            ["Obs.ErasmusApplication"] = "Erasmus / Değişim Başvurusu",
            ["Obs.IntegratedMasterApplication"] = "Bütünleşik Yüksek Lisans Başvurusu",

            // Modules (EBYS, SEM, CampusLife, SocialContribution)
            ["Obs.ElectronicDocumentManagement"] = "Elektronik Belge Yönetim Sistemi (EBYS)",
            ["Obs.VerifyDocument"] = "Karekodlu Belge Doğrulama",
            ["Obs.ContinuingEducation"] = "Sertifika / Kurs Eğitim Programları",
            ["Obs.CampusLifeAndReservations"] = "Etkinlik & Tesis Rezervasyon İşlemleri",
            ["Obs.SupportDesk"] = "Servis / Destek İşlemleri",
            ["Obs.SocialContribution"] = "Toplumsal Katkı Koordinatörlüğü",
            ["Obs.ScientificResearchProjects"] = "Bilimsel Araştırma Projeleri (BAP)"
        },

        [LanguageCode.English] = new(StringComparer.OrdinalIgnoreCase)
        {
            // Security
            ["Security.InvalidCredentials"] = "Invalid email or password.",
            ["Security.EmailAlreadyExists"] = "This email address is already in use.",
            ["Security.UserNotFound"] = "User not found.",
            ["Security.InvalidRefreshToken"] = "Refresh token is invalid or expired.",
            ["Security.SessionRevoked"] = "Session has been revoked.",
            ["Security.InvalidEmail"] = "Invalid email format.",
            ["Security.WeakPassword"] = "Password does not meet complexity requirements.",
            ["Security.AccountLocked"] = "Account has been locked due to too many failed attempts.",

            // Files
            ["Files.FileNotFound"] = "Requested file was not found.",
            ["Files.FileTooLarge"] = "File size exceeds the allowed limit.",
            ["Files.InvalidSignature"] = "File signature (magic byte) verification failed.",
            ["Files.EmptyFile"] = "Uploaded file cannot be empty.",
            ["Files.AccessDenied"] = "You do not have permission to access this file.",
            ["Files.InvalidContentType"] = "Unsupported file type.",
            ["Files.UploadFailed"] = "Failed to upload file to storage.",
            ["Files.UnauthorizedUpload"] = "You do not have permission to upload this file.",

            // Notifications
            ["Notifications.InvalidEmail"] = "Invalid email address format.",
            ["Notifications.InvalidPhoneNumber"] = "Invalid phone number format.",
            ["Notifications.InvalidSubject"] = "Notification subject is invalid.",
            ["Notifications.InvalidBody"] = "Notification body is invalid or empty.",
            ["Notifications.AlreadySent"] = "This notification has already been sent.",
            ["Notifications.AlreadyCancelled"] = "This notification has already been cancelled.",
            ["Notifications.NotFound"] = "Notification record not found.",
            ["Notifications.SendFailed"] = "Notification dispatch failed.",
            ["Notifications.DeliveryFailed"] = "Email delivery failed.",
            ["Notifications.SmsDeliveryFailed"] = "SMS delivery failed.",
            ["Notifications.RetryFailed"] = "Notification retry failed.",

            // Auditing & Messaging
            ["Auditing.NotFound"] = "Audit log entry was not found.",
            ["Messaging.PublishFailed"] = "Failed to publish message.",
            ["Messaging.MessageNotFound"] = "Outbox message not found.",
            ["Scheduling.JobNotFound"] = "Scheduled job was not found.",
            ["Webhooks.SubscriptionNotFound"] = "Webhook subscription was not found.",
            ["Webhooks.InvalidUrl"] = "Invalid webhook target URL.",
            ["Webhooks.DeliveryFailed"] = "Webhook delivery failed.",
            ["Webhooks.InvalidSignature"] = "Invalid webhook HMAC-SHA256 signature.",

            // Common / General
            ["Common.ValidationFailed"] = "One or more validation errors occurred.",
            ["Common.NotFound"] = "The requested resource was not found.",
            ["Common.Unauthorized"] = "Authentication is required for this action.",
            ["Common.Forbidden"] = "You do not have permission to perform this action.",
            ["Common.Conflict"] = "The operation conflicts with current state.",
            ["Common.InternalError"] = "An unexpected server error occurred.",

            // OBS - Academic & Programs
            ["Obs.SelectAcademicProgram"] = "Select Academic Program",
            ["Obs.Transcript"] = "Transcript",
            ["Obs.MyCourses"] = "My Courses",
            ["Obs.LiveCourses"] = "Live Courses",
            ["Obs.Calendar"] = "Academic Calendar",
            ["Obs.OnlineExam"] = "Online Examination",
            ["Obs.CourseRegistration"] = "Course Enrollment & Registration",
            ["Obs.AlumniNetwork"] = "Alumni Network System",
            ["Obs.WeeklySchedule"] = "Weekly Schedule",
            ["Obs.Announcements"] = "Announcements",
            ["Obs.Surveys"] = "Surveys",
            ["Obs.Messages"] = "Messages",
            ["Obs.AccountSettings"] = "Account Settings",
            ["Obs.PortalSettings"] = "User Portal Settings",
            ["Obs.PhoneBook"] = "Telephone Directory",
            ["Obs.SupportService"] = "UBYS Support Desk",
            ["Obs.InstitutionalEvaluation"] = "Institutional Evaluation",
            ["Obs.QuickShortcuts"] = "Personal Shortcuts",
            ["Obs.AddWidget"] = "Add New Widget",
            ["Obs.CourseListWidget"] = "Course List (Widget)",

            // OBS - "Other" (14 Items)
            ["Obs.TuitionInfo"] = "Tuition Fee Information",
            ["Obs.AdvisorInfo"] = "Academic Advisor Information",
            ["Obs.HistoryInfo"] = "Academic History",
            ["Obs.RegistrationApprovalReport"] = "Registration Approval Report",
            ["Obs.Activities"] = "Activities",
            ["Obs.DisenrollmentRequest"] = "Request Disenrollment",
            ["Obs.NoObstacleTransferProcess"] = "Initiate Clearance for Transfer",
            ["Obs.MilitaryStatus"] = "Military Service Status",
            ["Obs.ProfilePictureManagement"] = "Profile Picture Management",
            ["Obs.DisciplinaryRecords"] = "Disciplinary Records",
            ["Obs.LeaveOfAbsence"] = "Leave of Absence Information",
            ["Obs.BankAccountIban"] = "Bank Account & IBAN Details",
            ["Obs.AdditionalTimeInfo"] = "Additional Duration & Exam Rights",

            // OBS - Applications & Services
            ["Obs.DoubleMajorApplication"] = "Double Major Application",
            ["Obs.MinorApplication"] = "Minor Application",
            ["Obs.TransferApplication"] = "Transfer Application",
            ["Obs.SummerSchoolExternalCourse"] = "External Summer School Course Approval Application",
            ["Obs.DocumentRequest"] = "Official Document Request",
            ["Obs.ErasmusApplication"] = "Erasmus / Exchange Application",
            ["Obs.IntegratedMasterApplication"] = "Integrated Master's Application",

            // Modules (EBYS, SEM, CampusLife, SocialContribution)
            ["Obs.ElectronicDocumentManagement"] = "Electronic Document Management System (EDMS)",
            ["Obs.VerifyDocument"] = "QR Code Document Verification",
            ["Obs.ContinuingEducation"] = "Certificate & Continuing Education Programs",
            ["Obs.CampusLifeAndReservations"] = "Event & Facility Reservations",
            ["Obs.SupportDesk"] = "Service & Support Desk",
            ["Obs.SocialContribution"] = "Social Contribution Coordinatorship",
            ["Obs.ScientificResearchProjects"] = "Scientific Research Projects (BAP)"
        },

        [LanguageCode.German] = new(StringComparer.OrdinalIgnoreCase)
        {
            // Security
            ["Security.InvalidCredentials"] = "Ungültige E-Mail-Adresse oder Passwort.",
            ["Security.EmailAlreadyExists"] = "Diese E-Mail-Adresse wird bereits verwendet.",
            ["Security.UserNotFound"] = "Benutzer nicht gefunden.",
            ["Security.InvalidRefreshToken"] = "Refresh-Token ist ungültig oder abgelaufen.",
            ["Security.SessionRevoked"] = "Sitzung wurde beendet.",
            ["Security.InvalidEmail"] = "Ungültiges E-Mail-Format.",
            ["Security.WeakPassword"] = "Passwort erfüllt die Sicherheitsanforderungen nicht.",
            ["Security.AccountLocked"] = "Konto wurde aufgrund zu vieler Fehlversuche gesperrt.",

            // Files
            ["Files.FileNotFound"] = "Die angeforderte Datei wurde nicht gefunden.",
            ["Files.FileTooLarge"] = "Dateigröße überschreitet das zulässige Limit.",
            ["Files.InvalidSignature"] = "Dateisignatur-Überprüfung fehlgeschlagen.",
            ["Files.EmptyFile"] = "Die hochgeladene Datei darf nicht leer sein.",
            ["Files.AccessDenied"] = "Sie haben keine Berechtigung für diese Datei.",
            ["Files.InvalidContentType"] = "Nicht unterstützter Dateityp.",
            ["Files.UploadFailed"] = "Datei konnte nicht gespeichert werden.",
            ["Files.UnauthorizedUpload"] = "Sie haben keine Berechtigung, diese Datei hochzuladen.",

            // Notifications
            ["Notifications.InvalidEmail"] = "Ungültiges E-Mail-Adressformat.",
            ["Notifications.InvalidPhoneNumber"] = "Ungültiges Telefonnummernformat.",
            ["Notifications.InvalidSubject"] = "Betreff ist ungültig.",
            ["Notifications.InvalidBody"] = "Nachrichtentext ist ungültig oder leer.",
            ["Notifications.AlreadySent"] = "Diese Benachrichtigung wurde bereits gesendet.",
            ["Notifications.AlreadyCancelled"] = "Diese Benachrichtigung wurde bereits storniert.",
            ["Notifications.NotFound"] = "Benachrichtigung nicht gefunden.",
            ["Notifications.SendFailed"] = "Zustellung der Benachrichtigung fehlgeschlagen.",
            ["Notifications.DeliveryFailed"] = "E-Mail-Zustellung fehlgeschlagen.",
            ["Notifications.SmsDeliveryFailed"] = "SMS-Zustellung fehlgeschlagen.",
            ["Notifications.RetryFailed"] = "Wiederholung der Benachrichtigung fehlgeschlagen.",

            // Auditing & Messaging
            ["Auditing.NotFound"] = "Audit-Protokolleintrag wurde nicht gefunden.",
            ["Messaging.PublishFailed"] = "Nachricht konnte nicht veröffentlicht werden.",
            ["Messaging.MessageNotFound"] = "Outbox-Nachricht nicht gefunden.",
            ["Scheduling.JobNotFound"] = "Geplanter Auftrag wurde nicht gefunden.",
            ["Webhooks.SubscriptionNotFound"] = "Webhook-Abonnement wurde nicht gefunden.",
            ["Webhooks.InvalidUrl"] = "Ungültige Webhook-Ziel-URL.",
            ["Webhooks.DeliveryFailed"] = "Webhook-Zustellung fehlgeschlagen.",
            ["Webhooks.InvalidSignature"] = "Ungültige Webhook HMAC-SHA256 Signatur.",

            // Common / General
            ["Common.ValidationFailed"] = "Ein oder mehrere Validierungsfehler sind aufgetreten.",
            ["Common.NotFound"] = "Die angeforderte Ressource wurde nicht gefunden.",
            ["Common.Unauthorized"] = "Für diese Aktion ist eine Authentifizierung erforderlich.",
            ["Common.Forbidden"] = "Sie haben keine Berechtigung für diese Aktion.",
            ["Common.Conflict"] = "Der Vorgang steht im Konflikt mit dem aktuellen Status.",
            ["Common.InternalError"] = "Ein unerwarteter Serverfehler ist aufgetreten.",

            // OBS - Academic & Programs
            ["Obs.SelectAcademicProgram"] = "Akademisches Programm auswählen",
            ["Obs.Transcript"] = "Notenübersicht (Transkript)",
            ["Obs.MyCourses"] = "Meine Kurse",
            ["Obs.LiveCourses"] = "Live-Vorlesungen",
            ["Obs.Calendar"] = "Akademischer Kalender",
            ["Obs.OnlineExam"] = "Online-Prüfung",
            ["Obs.CourseRegistration"] = "Kurswahl und Rückmeldung",
            ["Obs.AlumniNetwork"] = "Alumni-Netzwerk",
            ["Obs.WeeklySchedule"] = "Wochenstundenplan",
            ["Obs.Announcements"] = "Ankündigungen",
            ["Obs.Surveys"] = "Umfragen",
            ["Obs.Messages"] = "Nachrichten",
            ["Obs.AccountSettings"] = "Kontoeinstellungen",
            ["Obs.PortalSettings"] = "Benutzerportal-Einstellungen",
            ["Obs.PhoneBook"] = "Telefonverzeichnis",
            ["Obs.SupportService"] = "UBYS-Support",
            ["Obs.InstitutionalEvaluation"] = "Institutionelle Evaluation",
            ["Obs.QuickShortcuts"] = "Persönliche Verknüpfungen",
            ["Obs.AddWidget"] = "Neues Widget hinzufügen",
            ["Obs.CourseListWidget"] = "Kursliste (Widget)",

            // OBS - "Other" (14 Items)
            ["Obs.TuitionInfo"] = "Studiengebühren",
            ["Obs.AdvisorInfo"] = "Studienberater-Information",
            ["Obs.HistoryInfo"] = "Studienverlauf",
            ["Obs.RegistrationApprovalReport"] = "Einschreibe-Genehmigungsbericht",
            ["Obs.Activities"] = "Aktivitäten",
            ["Obs.DisenrollmentRequest"] = "Exmatrikulationsantrag",
            ["Obs.NoObstacleTransferProcess"] = "Unbedenklichkeitsbescheinigung für Hochschulwechsel",
            ["Obs.MilitaryStatus"] = "Wehrdienststatus",
            ["Obs.ProfilePictureManagement"] = "Profilbild-Verwaltung",
            ["Obs.DisciplinaryRecords"] = "Disziplinarmaßnahmen",
            ["Obs.LeaveOfAbsence"] = "Urlaubssemester / Beurlaubung",
            ["Obs.BankAccountIban"] = "Bankkonto und IBAN-Daten",
            ["Obs.AdditionalTimeInfo"] = "Verlängerungszeiten und Zusatzprüfungen",

            // OBS - Applications & Services
            ["Obs.DoubleMajorApplication"] = "Doppelstudium-Bewerbung",
            ["Obs.MinorApplication"] = "Nebenfach-Bewerbung",
            ["Obs.TransferApplication"] = "Hochschulwechsel-Bewerbung",
            ["Obs.SummerSchoolExternalCourse"] = "Genehmigung externer Sommerkurs",
            ["Obs.DocumentRequest"] = "Amtlicher Dokumentenantrag",
            ["Obs.ErasmusApplication"] = "Erasmus / Austausch-Bewerbung",
            ["Obs.IntegratedMasterApplication"] = "Integriertes Master-Studium Bewerbung",

            // Modules (EBYS, SEM, CampusLife, SocialContribution)
            ["Obs.ElectronicDocumentManagement"] = "Elektronisches Dokumentenmanagementsystem (EDMS)",
            ["Obs.VerifyDocument"] = "QR-Code Dokumentenprüfung",
            ["Obs.ContinuingEducation"] = "Zertifikate und Weiterbildungsprogramme",
            ["Obs.CampusLifeAndReservations"] = "Campus-Events und Raumbuchungen",
            ["Obs.SupportDesk"] = "Service & Support-Desk",
            ["Obs.SocialContribution"] = "Koordinationsstelle für gesellschaftliches Engagement",
            ["Obs.ScientificResearchProjects"] = "Wissenschaftliche Forschungsprojekte (BAP)"
        },

        [LanguageCode.Russian] = new(StringComparer.OrdinalIgnoreCase)
        {
            // Security
            ["Security.InvalidCredentials"] = "Неверный адрес электронной почты или пароль.",
            ["Security.EmailAlreadyExists"] = "Этот адрес электронной почты уже используется.",
            ["Security.UserNotFound"] = "Пользователь не найден.",
            ["Security.InvalidRefreshToken"] = "Токен обновления недействителен или истек.",
            ["Security.SessionRevoked"] = "Сессия была отозвана.",
            ["Security.InvalidEmail"] = "Неверный формат адреса электронной почты.",
            ["Security.WeakPassword"] = "Пароль не соответствует требованиям безопасности.",
            ["Security.AccountLocked"] = "Аккаунт заблокирован из-за множества неудачных попыток входа.",

            // Files
            ["Files.FileNotFound"] = "Запрошенный файл не найден.",
            ["Files.FileTooLarge"] = "Размер файла превышает допустимый предел.",
            ["Files.InvalidSignature"] = "Проверка сигнатуры (magic byte) файла не удалась.",
            ["Files.EmptyFile"] = "Загруженный файл не может быть пустым.",
            ["Files.AccessDenied"] = "У вас нет прав на доступ к этому файлу.",
            ["Files.InvalidContentType"] = "Неподдерживаемый тип файла.",
            ["Files.UploadFailed"] = "Не удалось сохранить файл в хранилище.",
            ["Files.UnauthorizedUpload"] = "У вас нет разрешения на загрузку этого файла.",

            // Notifications
            ["Notifications.InvalidEmail"] = "Неверный формат адреса электронной почты.",
            ["Notifications.InvalidPhoneNumber"] = "Неверный формат номера телефона.",
            ["Notifications.InvalidSubject"] = "Тема уведомления недействительна.",
            ["Notifications.InvalidBody"] = "Тело уведомления недействительно или пусто.",
            ["Notifications.AlreadySent"] = "Это уведомление уже отправлено.",
            ["Notifications.AlreadyCancelled"] = "Это уведомление уже отменено.",
            ["Notifications.NotFound"] = "Запись уведомления не найдена.",
            ["Notifications.SendFailed"] = "Отправка уведомления не удалась.",
            ["Notifications.DeliveryFailed"] = "Доставка электронной почты не удалась.",
            ["Notifications.SmsDeliveryFailed"] = "Доставка SMS не удалась.",
            ["Notifications.RetryFailed"] = "Повторная попытка отправки уведомления не удалась.",

            // Auditing & Messaging
            ["Auditing.NotFound"] = "Запись журнала аудита не найдена.",
            ["Messaging.PublishFailed"] = "Не удалось опубликовать сообщение.",
            ["Messaging.MessageNotFound"] = "Сообщение outbox не найдено.",
            ["Scheduling.JobNotFound"] = "Запланированная задача не найдена.",
            ["Webhooks.SubscriptionNotFound"] = "Подписка на вебхук не найдена.",
            ["Webhooks.InvalidUrl"] = "Недействительный целевой URL вебхука.",
            ["Webhooks.DeliveryFailed"] = "Доставка вебхука не удалась.",
            ["Webhooks.InvalidSignature"] = "Недействительная подпись HMAC-SHA256 вебхука.",

            // Common / General
            ["Common.ValidationFailed"] = "Произошла одна или несколько ошибок проверки.",
            ["Common.NotFound"] = "Запрошенный ресурс не найден.",
            ["Common.Unauthorized"] = "Для этого действия требуется аутентификация.",
            ["Common.Forbidden"] = "У вас нет прав на выполнение этого действия.",
            ["Common.Conflict"] = "Операция конфликтует с текущим состоянием.",
            ["Common.InternalError"] = "Произошла непредвиденная ошибка сервера.",

            // OBS - Academic & Programs
            ["Obs.SelectAcademicProgram"] = "Выберите академическую программу",
            ["Obs.Transcript"] = "Транскрипт",
            ["Obs.MyCourses"] = "Мои курсы",
            ["Obs.LiveCourses"] = "Онлайн-занятия",
            ["Obs.Calendar"] = "Академический календарь",
            ["Obs.OnlineExam"] = "Онлайн-экзамен",
            ["Obs.CourseRegistration"] = "Выбор курсов и регистрация",
            ["Obs.AlumniNetwork"] = "Система выпускников",
            ["Obs.WeeklySchedule"] = "Еженедельное расписание",
            ["Obs.Announcements"] = "Объявления",
            ["Obs.Surveys"] = "Опросы",
            ["Obs.Messages"] = "Сообщения",
            ["Obs.AccountSettings"] = "Настройки аккаунта",
            ["Obs.PortalSettings"] = "Настройки портала пользователя",
            ["Obs.PhoneBook"] = "Телефонный справочник",
            ["Obs.SupportService"] = "Служба поддержки ÜBYS",
            ["Obs.InstitutionalEvaluation"] = "Институциональная оценка",
            ["Obs.QuickShortcuts"] = "Личные ярлыки",
            ["Obs.AddWidget"] = "Добавить новый виджет",
            ["Obs.CourseListWidget"] = "Список курсов (Виджет)",

            // OBS - "Other" (14 Items)
            ["Obs.TuitionInfo"] = "Информация об оплате за обучение",
            ["Obs.AdvisorInfo"] = "Информация об академическом кураторе",
            ["Obs.HistoryInfo"] = "История обучения",
            ["Obs.RegistrationApprovalReport"] = "Отчет об утверждении регистрации",
            ["Obs.Activities"] = "Мероприятия",
            ["Obs.DisenrollmentRequest"] = "Запрос на отчисление",
            ["Obs.NoObstacleTransferProcess"] = "Справка об отсутствии препятствий для перевода",
            ["Obs.MilitaryStatus"] = "Воинский учет",
            ["Obs.ProfilePictureManagement"] = "Управление фото профиля",
            ["Obs.DisciplinaryRecords"] = "Дисциплинарные взыскания",
            ["Obs.LeaveOfAbsence"] = "Академический отпуск",
            ["Obs.BankAccountIban"] = "Банковские реквизиты и IBAN",
            ["Obs.AdditionalTimeInfo"] = "Дополнительное время и права на пересдачу",

            // OBS - Applications & Services
            ["Obs.DoubleMajorApplication"] = "Заявление на второй диплом (ÇAP)",
            ["Obs.MinorApplication"] = "Заявление на дополнительную специальность",
            ["Obs.TransferApplication"] = "Заявление на перевод",
            ["Obs.SummerSchoolExternalCourse"] = "Заявление на курс внешней летней школы",
            ["Obs.DocumentRequest"] = "Заказ официальной справки / документа",
            ["Obs.ErasmusApplication"] = "Заявление на участие в Erasmus / обмене",
            ["Obs.IntegratedMasterApplication"] = "Заявление в интегрированную магистратуру",

            // Modules (EBYS, SEM, CampusLife, SocialContribution)
            ["Obs.ElectronicDocumentManagement"] = "Система электронного документооборота (СЭД)",
            ["Obs.VerifyDocument"] = "Проверка подлинности документов по QR-коду",
            ["Obs.ContinuingEducation"] = "Программы непрерывного образования и сертификации",
            ["Obs.CampusLifeAndReservations"] = "Бронирование кампуса и мероприятий",
            ["Obs.SupportDesk"] = "Служба поддержки и заявок",
            ["Obs.SocialContribution"] = "Координационный центр социального вклада",
            ["Obs.ScientificResearchProjects"] = "Научно-исследовательские проекты (НИП)"
        },

        [LanguageCode.Georgian] = new(StringComparer.OrdinalIgnoreCase)
        {
            // Security
            ["Security.InvalidCredentials"] = "არასწორი ელ.ფოსტა ან პაროლი.",
            ["Security.EmailAlreadyExists"] = "ეს ელ.ფოსტა უკვე გამოყენებულია.",
            ["Security.UserNotFound"] = "მომხმარებელი ვერ მოიძებნა.",
            ["Security.InvalidRefreshToken"] = "განახლების ტოკენი არასწორია ან ვადაგასულია.",
            ["Security.SessionRevoked"] = "სესია გაუქმებულია.",
            ["Security.InvalidEmail"] = "არასწორი ელ.ფოსტის ფორმატი.",
            ["Security.WeakPassword"] = "პაროლი არ აკმაყოფილებს უსაფრთხოების მოთხოვნებს.",
            ["Security.AccountLocked"] = "ანგარიში დაბლოკილია განმეორებითი შეცდომების გამო.",

            // Files
            ["Files.FileNotFound"] = "მოთხოვნილი ფაილი ვერ მოიძებნა.",
            ["Files.FileTooLarge"] = "ფაილის ზომა აღემატება დასაშვებ ლიმიტს.",
            ["Files.InvalidSignature"] = "ფაილის ციფრული ხელმოწერის შემოწმება ვერ მოხერხდა.",
            ["Files.EmptyFile"] = "ატვირთული ფაილი არ შეიძლება იყოს ცარიელი.",
            ["Files.AccessDenied"] = "თქვენ არ გაქვთ ამ ფაილზე წვდომის უფლება.",
            ["Files.InvalidContentType"] = "მხარდაუჭერელი ფაილის ტიპი.",
            ["Files.UploadFailed"] = "ფაილის შენახვა ვერ მოხერხდა.",
            ["Files.UnauthorizedUpload"] = "თქვენ არ გაქვთ ფაილის ატვირთვის უფლება.",

            // Notifications
            ["Notifications.InvalidEmail"] = "არასწორი ელ.ფოსტის ფორმატი.",
            ["Notifications.InvalidPhoneNumber"] = "არასწორი ტელეფონის ნომრის ფორმატი.",
            ["Notifications.InvalidSubject"] = "შეტყობინების სათაური არასწორია.",
            ["Notifications.InvalidBody"] = "შეტყობინების შინაარსი ცარიელია ან არასწორია.",
            ["Notifications.AlreadySent"] = "ეს შეტყობინება უკვე გაგზავნილია.",
            ["Notifications.AlreadyCancelled"] = "ეს შეტყობინება უკვე გაუქმებულია.",
            ["Notifications.NotFound"] = "შეტყობინების ჩანაწერი ვერ მოიძებნა.",
            ["Notifications.SendFailed"] = "შეტყობინების გაგზავნა ვერ მოხერხდა.",
            ["Notifications.DeliveryFailed"] = "ელ.ფოსტის მიწოდება ვერ მოხერხდა.",
            ["Notifications.SmsDeliveryFailed"] = "SMS შეტყობინების მიწოდება ვერ მოხერხდა.",
            ["Notifications.RetryFailed"] = "შეტყობინების ხელახლა გაგზავნა ვერ მოხერხდა.",

            // Auditing & Messaging
            ["Auditing.NotFound"] = "აუდიტის ჟურნალის ჩანაწერი ვერ მოიძებნა.",
            ["Messaging.PublishFailed"] = "შეტყობინების გამოქვეყნება ვერ მოხერხდა.",
            ["Messaging.MessageNotFound"] = "Outbox შეტყობინება ვერ მოიძებნა.",
            ["Scheduling.JobNotFound"] = "დაგეგმილი დავალება ვერ მოიძებნა.",
            ["Webhooks.SubscriptionNotFound"] = "Webhook გამოწერა ვერ მოიძებნა.",
            ["Webhooks.InvalidUrl"] = "არასწორი Webhook მისამართი.",
            ["Webhooks.DeliveryFailed"] = "Webhook მიწოდება ვერ მოხერხდა.",
            ["Webhooks.InvalidSignature"] = "არასწორი Webhook HMAC-SHA256 ხელმოწერა.",

            // Common / General
            ["Common.ValidationFailed"] = "დაფიქსირდა ვალიდაციის ერთი ან რამდენიმე შეცდომა.",
            ["Common.NotFound"] = "მოთხოვნილი რესურსი ვერ მოიძებნა.",
            ["Common.Unauthorized"] = "მოქმედების შესასრულებლად საჭიროა ავტორიზაცია.",
            ["Common.Forbidden"] = "თქვენ არ გაქვთ ამ მოქმედების უფლება.",
            ["Common.Conflict"] = "ოპერაცია ეწინააღმდეგება მიმდინარე მდგომარეობას.",
            ["Common.InternalError"] = "სერვერზე დაფიქსირდა მოულოდნელი შეცდომა.",

            // OBS - Academic & Programs
            ["Obs.SelectAcademicProgram"] = "აირჩიეთ აკადემიური პროგრამა",
            ["Obs.Transcript"] = "ტრანსკრიპტი",
            ["Obs.MyCourses"] = "ჩემი საგნები",
            ["Obs.LiveCourses"] = "ონლაინ ლექციები",
            ["Obs.Calendar"] = "აკადემიური კალენდარი",
            ["Obs.OnlineExam"] = "ონლაინ გამოცდა",
            ["Obs.CourseRegistration"] = "საგნების არჩევა და რეგისტრაცია",
            ["Obs.AlumniNetwork"] = "კურსდამთავრებულთა ქსელი",
            ["Obs.WeeklySchedule"] = "კვირის ცხრილი",
            ["Obs.Announcements"] = "განცხადებები",
            ["Obs.Surveys"] = "გამოკითხვები",
            ["Obs.Messages"] = "შეტყობინებები",
            ["Obs.AccountSettings"] = "ანგარიშის პარამეტრები",
            ["Obs.PortalSettings"] = "პორტალის პარამეტრები",
            ["Obs.PhoneBook"] = "სატელეფონო ცნობარი",
            ["Obs.SupportService"] = "ÜBYS მხარდაჭერის ცენტრი",
            ["Obs.InstitutionalEvaluation"] = "ინსტიტუციური შეფასება",
            ["Obs.QuickShortcuts"] = "სწრაფი ბმულები",
            ["Obs.AddWidget"] = "ახალი ვიჯეტის დამატება",
            ["Obs.CourseListWidget"] = "საგნების სია (ვიჯეტი)",

            // OBS - "Other" (14 Items)
            ["Obs.TuitionInfo"] = "სწავლის საფასურის ინფორმაცია",
            ["Obs.AdvisorInfo"] = "აკადემიური მრჩეველის ინფორმაცია",
            ["Obs.HistoryInfo"] = "აკადემიური ისტორია",
            ["Obs.RegistrationApprovalReport"] = "რეგისტრაციის დამტკიცების ანგარიში",
            ["Obs.Activities"] = "აქტივობები",
            ["Obs.DisenrollmentRequest"] = "სტატუსის შეწყვეტის მოთხოვნა",
            ["Obs.NoObstacleTransferProcess"] = "მობილობის დამაბრკოლებელი გარემოებების არარსებობის ცნობა",
            ["Obs.MilitaryStatus"] = "სამხედრო აღრიცხვის სტატუსი",
            ["Obs.ProfilePictureManagement"] = "პროფილის სურათის მართვა",
            ["Obs.DisciplinaryRecords"] = "დისციპლინური ჩანაწერები",
            ["Obs.LeaveOfAbsence"] = "აკადემიური შვებულება",
            ["Obs.BankAccountIban"] = "საბანკო რეკვიზიტები და IBAN",
            ["Obs.AdditionalTimeInfo"] = "დამატებითი დრო და გამოცდის უფლებები",

            // OBS - Applications & Services
            ["Obs.DoubleMajorApplication"] = "ორმაგი სპეციალობის (ÇAP) განაცხადი",
            ["Obs.MinorApplication"] = "დამატებითი სპეციალობის (Minor) განაცხადი",
            ["Obs.TransferApplication"] = "მობილობის განაცხადი",
            ["Obs.SummerSchoolExternalCourse"] = "გარე საზაფხულო სკოლის საგნის აღიარების განაცხადი",
            ["Obs.DocumentRequest"] = "ოფიციალური დოკუმენტის მოთხოვნა",
            ["Obs.ErasmusApplication"] = "Erasmus / გაცვლითი პროგრამის განაცხადი",
            ["Obs.IntegratedMasterApplication"] = "ინტეგრირებული მაგისტრატურის განაცხადი",

            // Modules (EBYS, SEM, CampusLife, SocialContribution)
            ["Obs.ElectronicDocumentManagement"] = "ელექტრონული დოკუმენტბრუნვის სისტემა (EBYS)",
            ["Obs.VerifyDocument"] = "დოკუმენტის გადამოწმება QR კოდით",
            ["Obs.ContinuingEducation"] = "სერტიფიკატები და უწყვეტი განათლების პროგრამები",
            ["Obs.CampusLifeAndReservations"] = "კამპუსის ღონისძიებები და ჯავშნები",
            ["Obs.SupportDesk"] = "მხარდაჭერის სერვისი და განაცხადები",
            ["Obs.SocialContribution"] = "სოციალური პასუხისმგებლობის კოორდინაცია",
            ["Obs.ScientificResearchProjects"] = "სამეცნიერო კვლევითი პროექტები (BAP)"
        }
    };

    /// <summary>
    /// Translates an error code into the target language.
    /// If translation is missing in the target language, it automatically falls back to Turkish (tr).
    /// If no translation exists at all, returns the code itself.
    /// </summary>
    public static string Translate(string code, string? culture = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return string.Empty;
        }

        var normalizedCulture = LanguageCode.Normalize(culture);

        // 1. Try specified culture
        if (Translations.TryGetValue(normalizedCulture, out var langDict) &&
            langDict.TryGetValue(code, out var translatedText))
        {
            return translatedText;
        }

        // 2. Fallback to Turkish if requested culture wasn't Turkish
        if (!string.Equals(normalizedCulture, LanguageCode.Turkish, StringComparison.OrdinalIgnoreCase) &&
            Translations.TryGetValue(LanguageCode.Turkish, out var trDict) &&
            trDict.TryGetValue(code, out var trFallbackText))
        {
            return trFallbackText;
        }

        // 3. Fallback to original code
        return code;
    }

    public static IReadOnlyDictionary<string, string> GetAll(string? culture = null)
    {
        var normalizedCulture = LanguageCode.Normalize(culture);
        return Translations.TryGetValue(normalizedCulture, out var dict)
            ? dict
            : Translations[LanguageCode.Turkish];
    }
}
