using System;
using System.Text.Json;
using Academic.Domain.Courses;
using Academic.Domain.Programs;
using Xunit;

namespace UniversalSystem.UnitTests;

public class EducationCatalogTests
{
    [Fact]
    public void ProgramProfile_CreateDefault_ShouldContainValidStructure()
    {
        var programId = Guid.NewGuid();
        var profile = ProgramProfile.CreateDefault(programId);

        Assert.Equal(programId, profile.ProgramId);
        Assert.NotEmpty(profile.AwardedDegree);
        Assert.NotEmpty(profile.AdmissionRequirements);
        Assert.NotEmpty(profile.GraduationRequirements);

        var outcomes = JsonSerializer.Deserialize<string[]>(profile.ProgramOutcomesJson);
        Assert.NotNull(outcomes);
        Assert.True(outcomes.Length > 0);
    }

    [Fact]
    public void CourseSyllabus_CreateAndUpdate_ShouldMaintainDetailedFields()
    {
        var syllabus = CourseSyllabus.Create(
            "BMU101",
            "Türkçe",
            "Lisans",
            "Zorunlu",
            "Yok",
            "Bilgisayar Programlama-I dersi, öğrencilere temel programlama mantığını kazandırmayı amaçlar.",
            "Değişkenler, döngüler, diziler, metotlar ve nesne yönelimli temeller.",
            "Head First Java, Effective Java",
            "Sözlü Anlatım, Laboratuvar",
            "Öğr. Gör. Mehmet Said Nur Yağmahan",
            "[\"Algoritma mantığını kavrar.\",\"Java ile temel program yazar.\"]",
            3,
            0,
            2,
            4,
            6);

        Assert.Equal("BMU101", syllabus.CourseCode);
        Assert.Equal(3, syllabus.TheoryHours);
        Assert.Equal(0, syllabus.PracticeHours);
        Assert.Equal(2, syllabus.LabHours);
        Assert.Equal(4, syllabus.LocalCredits);
        Assert.Equal(6, syllabus.Ects);
        Assert.Equal("Zorunlu", syllabus.Type);

        var outcomes = JsonSerializer.Deserialize<string[]>(syllabus.LearningOutcomesJson);
        Assert.NotNull(outcomes);
        Assert.Equal(2, outcomes.Length);
    }
}
