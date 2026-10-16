namespace SocialContribution.Domain;

public enum ContributionCategory
{
    SocialResponsibility = 1, // Sosyal Sorumluluk
    Volunteering = 2,          // Gönüllülük Faaliyetleri
    PublicEducation = 3,       // Halka Açık Eğitim ve Bilinçlendirme
    EnvironmentalSustainability = 4, // Çevre ve Sürdürülebilirlik
    DisasterRelief = 5,        // Afet ve Acil Durum Desteği
    CulturalHeritage = 6       // Kültürel Miras ve Sanat
}

public enum ProjectStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Rejected = 4,
    InProgress = 5,
    Completed = 6,
    Cancelled = 7
}

public enum VolunteerApplicationStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Withdrawn = 4
}
