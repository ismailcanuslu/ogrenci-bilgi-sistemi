namespace StudentApplications.Domain.Applications;

public enum ApplicationType
{
    DoubleMajor = 1,        // ÇAP (Çift Ana Dal) Başvurusu
    Minor = 2,              // Yandal Başvurusu
    Transfer = 3,           // Yatay Geçiş Başvurusu
    SummerSchool = 4,       // Dış Kurum Yaz Okulu Ders Onay Başvurusu
    DocumentRequest = 5,    // Belge Talebi (Öğrenci Belgesi, Transkript vb.)
    Exemption = 6,          // Ders İntibak / Muafiyet
    Erasmus = 7,            // Erasmus / Değişim Programı Başvurusu
    IntegratedMaster = 8,   // Bütünleşik Yüksek Lisans Başvurusu
    Clearance = 9,          // İlişik Kesme Talebi
    TransferClearance = 10, // Yatay Geçişte Engel Yoktur Süreci Başlatma Talebi
    LeaveOfAbsence = 11     // İzin / Kayıt Dondurma Talebi
}

public enum ApplicationState
{
    Submitted = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Cancelled = 5
}
