namespace Academic.Domain.Dashboard;

public enum WidgetType
{
    CourseList = 1,           // Ders Listesi (1. görseldeki ana widget)
    WeeklySchedule = 2,       // Haftalık Ders Programı
    AcademicCalendar = 3,     // Akademik Takvim & Önemli Tarihler
    Announcements = 4,        // Bilgilendirmeler / Duyurular
    Surveys = 5,              // Anketler
    UpcomingExams = 6,        // Yaklaşan Sınavlar
    AttendanceSummary = 7,    // Devamsızlık Durumu
    TuitionStatus = 8,        // Harç ve Borç Durumu
    RecentGrades = 9,         // Son Açıklanan Notlar
    SupportTickets = 10,      // Destek Talepleri
    FacilityReservations = 11,// Kampüs Tesis Rezervasyonları
    QuickShortcuts = 12       // Kişisel Kısayollar
}
