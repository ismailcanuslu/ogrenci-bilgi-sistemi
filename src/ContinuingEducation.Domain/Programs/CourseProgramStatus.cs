namespace ContinuingEducation.Domain.Programs;

public enum CourseProgramStatus
{
    Draft = 0,
    Active = 1,
    Completed = 2,
    Cancelled = 3
}

public enum ApplicationStatus
{
    Pending = 0,    // Başvuru Alındı
    Approved = 1,   // Onaylandı / Kesin Kayıt
    Rejected = 2,   // Reddedildi
    Completed = 3,  // Başarıyla Tamamlandı (Sertifika Hak Etti)
    Failed = 4      // Devamsızlık veya Not Yetersizliği Nedeniyle Başarısız
}
