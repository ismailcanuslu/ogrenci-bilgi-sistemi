using BuildingBlocks.Domain.Primitives;

namespace Academic.Domain.Courses;

public sealed class CourseSyllabus : AggregateRoot<Guid>
{
    public string CourseCode { get; private set; } = string.Empty;
    public string Language { get; private set; } = "Türkçe"; // Dersin Dili
    public string Level { get; private set; } = "Lisans"; // Dersin Seviyesi
    public string Type { get; private set; } = "Zorunlu"; // Dersin Tipi (Zorunlu / Seçmeli)
    public string PrerequisitesText { get; private set; } = "Yok"; // Ön Koşullar
    public string Aim { get; private set; } = string.Empty; // Dersin Amacı
    public string Content { get; private set; } = string.Empty; // Dersin İçeriği
    public string TextbooksAndResources { get; private set; } = string.Empty; // Dersin Kitabı / Malzemesi / Önerilen Kaynaklar
    public string PlannedLearningActivities { get; private set; } = string.Empty; // Planlanan Öğrenme Etkinlikleri ve Öğretim Yöntemleri
    public string RecommendedOtherTopics { get; private set; } = "Bulunmamaktadır."; // Ders İçin Önerilen Diğer Hususlar
    public string InstructorsText { get; private set; } = string.Empty; // Dersi Veren Öğretim Elemanları
    public string LearningOutcomesJson { get; private set; } = "[]"; // Ders Öğrenme Çıktıları (JSON Dizi)
    public int TheoryHours { get; private set; }
    public int PracticeHours { get; private set; }
    public int LabHours { get; private set; }
    public int LocalCredits { get; private set; }
    public int Ects { get; private set; }
    public DateTime LastUpdatedAtUtc { get; private set; }

    private CourseSyllabus() { }

    public static CourseSyllabus Create(
        string courseCode,
        string language,
        string level,
        string type,
        string prerequisitesText,
        string aim,
        string content,
        string textbooksAndResources,
        string plannedLearningActivities,
        string instructorsText,
        string learningOutcomesJson,
        int theoryHours,
        int practiceHours,
        int labHours,
        int localCredits,
        int ects)
    {
        return new CourseSyllabus
        {
            Id = Guid.NewGuid(),
            CourseCode = courseCode.Trim().ToUpperInvariant(),
            Language = language.Trim(),
            Level = level.Trim(),
            Type = type.Trim(),
            PrerequisitesText = string.IsNullOrWhiteSpace(prerequisitesText) ? "Yok" : prerequisitesText.Trim(),
            Aim = aim.Trim(),
            Content = content.Trim(),
            TextbooksAndResources = textbooksAndResources.Trim(),
            PlannedLearningActivities = plannedLearningActivities.Trim(),
            InstructorsText = instructorsText.Trim(),
            LearningOutcomesJson = learningOutcomesJson.Trim(),
            TheoryHours = theoryHours,
            PracticeHours = practiceHours,
            LabHours = labHours,
            LocalCredits = localCredits,
            Ects = ects,
            LastUpdatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(
        string language,
        string level,
        string type,
        string prerequisitesText,
        string aim,
        string content,
        string textbooksAndResources,
        string plannedLearningActivities,
        string instructorsText,
        string learningOutcomesJson,
        int theoryHours,
        int practiceHours,
        int labHours,
        int localCredits,
        int ects)
    {
        Language = language.Trim();
        Level = level.Trim();
        Type = type.Trim();
        PrerequisitesText = string.IsNullOrWhiteSpace(prerequisitesText) ? "Yok" : prerequisitesText.Trim();
        Aim = aim.Trim();
        Content = content.Trim();
        TextbooksAndResources = textbooksAndResources.Trim();
        PlannedLearningActivities = plannedLearningActivities.Trim();
        InstructorsText = instructorsText.Trim();
        LearningOutcomesJson = learningOutcomesJson.Trim();
        TheoryHours = theoryHours;
        PracticeHours = practiceHours;
        LabHours = labHours;
        LocalCredits = localCredits;
        Ects = ects;
        LastUpdatedAtUtc = DateTime.UtcNow;
    }
}
