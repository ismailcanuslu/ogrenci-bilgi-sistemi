using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Programs;

public sealed class ProgramProfile : AggregateRoot<Guid>
{
    public Guid ProgramId { get; private set; }
    public string History { get; private set; } = string.Empty; // Kuruluş
    public string AwardedDegree { get; private set; } = string.Empty; // Kazanılan Derece
    public string DegreeLevelDescription { get; private set; } = string.Empty; // Derecenin Düzeyi (Ön Lisans, Lisans, YL vb.)
    public string AdmissionRequirements { get; private set; } = string.Empty; // Kabul ve Kayıt Koşulları
    public string RecognitionOfPriorLearning { get; private set; } = string.Empty; // Önceki Öğrenmenin Tanınması
    public string QualificationRequirements { get; private set; } = string.Empty; // Yeterlilik Koşulları ve Kuralları (240 AKTS vb.)
    public string ProgramAims { get; private set; } = string.Empty; // Programın Amacı
    public string OccupationalProfiles { get; private set; } = string.Empty; // Mezunların İstihdam Profilleri
    public string AccessToFurtherStudies { get; private set; } = string.Empty; // Üst Derece Programlarına Geçiş
    public string ExaminationRegulations { get; private set; } = string.Empty; // Sınavlar, Ölçme ve Değerlendirme
    public string GraduationRequirements { get; private set; } = string.Empty; // Mezuniyet Koşulları
    public string ModeOfStudy { get; private set; } = "Tam Zamanlı"; // Çalışma Şekli
    public string DepartmentHeadAndContact { get; private set; } = string.Empty; // Bölüm Başkanı ve İletişim Bilgileri
    public string Facilities { get; private set; } = string.Empty; // Bölüm Olanakları
    public string ProgramEducationalObjectives { get; private set; } = string.Empty; // Program Eğitim Amaçları
    public string ProgramOutcomesJson { get; private set; } = "[]"; // Program Çıktıları (JSON Dizi)
    public DateTime UpdatedAtUtc { get; private set; }

    private ProgramProfile() { }

    public static ProgramProfile CreateDefault(Guid programId)
    {
        return new ProgramProfile
        {
            Id = Guid.NewGuid(),
            ProgramId = programId,
            History = "Program kuruluşu ve tarihçesi.",
            AwardedDegree = "Lisans Derecesi",
            DegreeLevelDescription = "Lisans (Birinci Düzey - TYYÇ 6. Düzey)",
            AdmissionRequirements = "ÖSYM yerleştirme sonuçları veya yatay/dikey geçiş esasları.",
            RecognitionOfPriorLearning = "Yükseköğretim kurumlarında daha önce alınan derslerin intibakı.",
            QualificationRequirements = "En az 240 AKTS kredisini başarıyla tamamlamak ve 4.00 üzerinden en az 2.00 GANO elde etmek.",
            ProgramAims = "Alanında yetkin, etik değerlere sahip, uluslararası standartlarda profesyoneller yetiştirmek.",
            OccupationalProfiles = "Mezunlar kamu ve özel sektörde ilgili uzmanlık alanlarında istihdam edilirler.",
            AccessToFurtherStudies = "Lisansüstü programlara (Tezli/Tezsiz YL ve Doktora) başvurabilirler.",
            ExaminationRegulations = "Dönem içi ara sınav, ödev/proje ve dönem sonu final sınavı değerlendirilir.",
            GraduationRequirements = "Tüm müfredat derslerini başarmak ve zorunlu stajı tamamlamak.",
            ModeOfStudy = "Tam Zamanlı",
            DepartmentHeadAndContact = "Bölüm Başkanlığı",
            Facilities = "Derslikler, modern bilgisayar ve araştırma laboratuvarları.",
            ProgramEducationalObjectives = "Sektörde aranan nitelikli mezunlar yetiştirmek.",
            ProgramOutcomesJson = "[\"Matematik, fen ve mühendislik bilgilerini uygulama becerisi.\",\"Karmaşık problemleri saptama, tanımlama ve çözme becerisi.\",\"Mesleki ve etik sorumluluk bilinci.\"]",
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(
        string history,
        string awardedDegree,
        string degreeLevelDescription,
        string admissionRequirements,
        string recognitionOfPriorLearning,
        string qualificationRequirements,
        string programAims,
        string occupationalProfiles,
        string accessToFurtherStudies,
        string examinationRegulations,
        string graduationRequirements,
        string modeOfStudy,
        string departmentHeadAndContact,
        string facilities,
        string programEducationalObjectives,
        string programOutcomesJson)
    {
        History = history.Trim();
        AwardedDegree = awardedDegree.Trim();
        DegreeLevelDescription = degreeLevelDescription.Trim();
        AdmissionRequirements = admissionRequirements.Trim();
        RecognitionOfPriorLearning = recognitionOfPriorLearning.Trim();
        QualificationRequirements = qualificationRequirements.Trim();
        ProgramAims = programAims.Trim();
        OccupationalProfiles = occupationalProfiles.Trim();
        AccessToFurtherStudies = accessToFurtherStudies.Trim();
        ExaminationRegulations = examinationRegulations.Trim();
        GraduationRequirements = graduationRequirements.Trim();
        ModeOfStudy = modeOfStudy.Trim();
        DepartmentHeadAndContact = departmentHeadAndContact.Trim();
        Facilities = facilities.Trim();
        ProgramEducationalObjectives = programEducationalObjectives.Trim();
        ProgramOutcomesJson = programOutcomesJson.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
