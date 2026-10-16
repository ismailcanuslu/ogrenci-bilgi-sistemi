using Academic.Domain.Curriculums;
using BuildingBlocks.Domain.Exceptions;
using Xunit;

namespace UniversalSystem.UnitTests.Academic;

public class CurriculumDomainTests
{
    [Fact]
    public void Curriculum_Create_InitializesWithFirstDraftVersion()
    {
        var programId = Guid.NewGuid();
        var yearId = Guid.NewGuid();

        var curriculum = Curriculum.Create(programId, yearId, "ceng-2026", "Bilgisayar Müh. 2026", DateTime.UtcNow);

        Assert.Equal("CENG-2026", curriculum.Code);
        Assert.Equal("Bilgisayar Müh. 2026", curriculum.Name);
        Assert.Single(curriculum.Versions);

        var firstVersion = curriculum.Versions.First();
        Assert.Equal(1, firstVersion.VersionNumber);
        Assert.Equal(CurriculumVersionStatus.Draft, firstVersion.Status);
        Assert.Equal(0, firstVersion.TotalEcts);
    }

    [Fact]
    public void CurriculumVersion_AddCourse_IncreasesTotalEcts()
    {
        var curriculum = Curriculum.Create(Guid.NewGuid(), Guid.NewGuid(), "CENG", "Bilgisayar", DateTime.UtcNow);
        var version = curriculum.Versions.First();

        version.AddCourse("CENG101", "Giriş", 6, 3, 1, false, CourseCategory.Mandatory, null);
        version.AddCourse("MATH101", "Kalkülüs I", 7, 4, 1, false, CourseCategory.Mandatory, null);

        Assert.Equal(2, version.Courses.Count);
        Assert.Equal(13, version.TotalEcts);
    }

    [Fact]
    public void CurriculumVersion_CannotPublishWhenEmpty()
    {
        var curriculum = Curriculum.Create(Guid.NewGuid(), Guid.NewGuid(), "CENG", "Bilgisayar", DateTime.UtcNow);
        var version = curriculum.Versions.First();

        var ex = Assert.Throws<DomainException>(() => curriculum.PublishVersion(version.Id, DateTime.UtcNow));
        Assert.Equal("CurriculumVersion.CannotPublishEmptyCurriculum", ex.Code);
    }

    [Fact]
    public void CurriculumVersion_PublishedVersionIsImmutable()
    {
        var curriculum = Curriculum.Create(Guid.NewGuid(), Guid.NewGuid(), "CENG", "Bilgisayar", DateTime.UtcNow);
        var version = curriculum.Versions.First();
        version.AddCourse("CENG101", "Giriş", 6, 3, 1, false, CourseCategory.Mandatory, null);

        curriculum.PublishVersion(version.Id, DateTime.UtcNow);
        Assert.Equal(CurriculumVersionStatus.Published, version.Status);

        // Yayınlanmış sürüme ders eklenemez
        var ex = Assert.Throws<DomainException>(() =>
            version.AddCourse("CENG102", "Giriş II", 6, 3, 2, false, CourseCategory.Mandatory, null));
        Assert.Equal("CurriculumVersion.ImmutableWhenNotDraft", ex.Code);
    }

    [Fact]
    public void Curriculum_CreateNewVersion_CopiesPreviousPublishedCourses_AndArchivesPreviousOnPublish()
    {
        var curriculum = Curriculum.Create(Guid.NewGuid(), Guid.NewGuid(), "CENG", "Bilgisayar", DateTime.UtcNow);
        var v1 = curriculum.Versions.First();
        v1.AddCourse("CENG101", "Giriş", 6, 3, 1, false, CourseCategory.Mandatory, null);
        curriculum.PublishVersion(v1.Id, DateTime.UtcNow);

        // Yeni sürüm oluştur
        var v2 = curriculum.CreateNewVersion("2026 Revizyon", DateTime.UtcNow);
        Assert.Equal(2, v2.VersionNumber);
        Assert.Equal(CurriculumVersionStatus.Draft, v2.Status);
        Assert.Single(v2.Courses); // v1'den kopyalandı
        Assert.Equal("CENG101", v2.Courses.First().CourseCode);

        // Yeni sürüme ders ekle ve yayınla
        v2.AddCourse("CENG102", "OOP", 6, 3, 2, false, CourseCategory.Mandatory, null);
        curriculum.PublishVersion(v2.Id, DateTime.UtcNow);

        Assert.Equal(CurriculumVersionStatus.Archived, v1.Status);
        Assert.Equal(CurriculumVersionStatus.Published, v2.Status);
        Assert.Equal(12, v2.TotalEcts);
    }
}
