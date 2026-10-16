using Academic.Domain.Instructors;
using Academic.Domain.Students;
using BuildingBlocks.Domain.Exceptions;
using Xunit;

namespace UniversalSystem.UnitTests.Academic;

public class StudentDomainTests
{
    [Fact]
    public void Student_Create_NormalizesDataAndSetsActive()
    {
        var student = Student.Create(
            "  202601001 ",
            "  Ahmet ",
            "  Yılmaz ",
            " Ahmet.Yilmaz@Uni.Edu.Tr ",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            2026,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);

        Assert.Equal("202601001", student.StudentNumber);
        Assert.Equal("Ahmet", student.FirstName);
        Assert.Equal("Yılmaz", student.LastName);
        Assert.Equal("ahmet.yilmaz@uni.edu.tr", student.Email);
        Assert.Equal(StudentStatus.Active, student.Status);
    }

    [Fact]
    public void Student_Create_AutomaticallyCreatesPrimaryMajorProgramEnrollment()
    {
        var deptId = Guid.NewGuid();
        var progId = Guid.NewGuid();
        var curvId = Guid.NewGuid();

        var student = Student.Create(
            "202601001",
            "Ayşe",
            "Demir",
            "ayse@uni.edu.tr",
            deptId,
            progId,
            curvId,
            2026,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);

        Assert.Single(student.ProgramEnrollments);
        var primary = student.PrimaryEnrollment;
        Assert.NotNull(primary);
        Assert.Equal(progId, primary.ProgramId);
        Assert.Equal(deptId, primary.DepartmentId);
        Assert.Equal(EnrollmentType.Major, primary.EnrollmentType);
        Assert.True(primary.IsPrimary);
        Assert.Equal(StudentStatus.Active, primary.Status);
    }

    [Fact]
    public void Student_EnrollInProgram_SupportsDoubleMajorAndMinor()
    {
        var cseDept = Guid.NewGuid();
        var cseProg = Guid.NewGuid();
        var eeeDept = Guid.NewGuid();
        var eeeProg = Guid.NewGuid();
        var busDept = Guid.NewGuid();
        var busProg = Guid.NewGuid();

        var student = Student.Create(
            "202601001", "Can", "Yıldız", "can@uni.edu.tr",
            cseDept, cseProg, Guid.NewGuid(), 2026, DateTime.UtcNow, null, DateTime.UtcNow);

        // ÇAP: Elektrik Elektronik Mühendisliği
        var capEnrollment = student.EnrollInProgram(
            eeeDept, eeeProg, Guid.NewGuid(),
            EnrollmentType.DoubleMajor, 2027, DateTime.UtcNow, isPrimary: false, notes: "ÇAP Başvurusu Onaylandı");

        // Yandal: İşletme
        var yandalEnrollment = student.EnrollInProgram(
            busDept, busProg, Guid.NewGuid(),
            EnrollmentType.Minor, 2027, DateTime.UtcNow, isPrimary: false, notes: "Yandal Kaydı");

        Assert.Equal(3, student.ProgramEnrollments.Count);
        Assert.Equal(cseProg, student.ProgramId); // Birincil hala CSE

        // Ekran görüntüsü 2: Kullanıcı üst menüden ÇAP programını seçiyor ("Akademik Program Seçiniz")
        student.SetPrimaryProgram(capEnrollment.Id, DateTime.UtcNow);
        Assert.Equal(eeeProg, student.ProgramId);
        Assert.Equal(eeeDept, student.DepartmentId);
        Assert.True(capEnrollment.IsPrimary);
        Assert.False(student.ProgramEnrollments.First(p => p.ProgramId == cseProg).IsPrimary);
    }

    [Fact]
    public void Instructor_DepartmentAffiliation_SupportsPrimaryCadreAndSecondaryAssignment()
    {
        var instructorId = Guid.NewGuid();
        var cseFaculty = Guid.NewGuid();
        var cseDept = Guid.NewGuid();
        var aiFaculty = Guid.NewGuid();
        var aiDept = Guid.NewGuid();

        // Asil Kadro (CSE)
        var primaryAffiliation = InstructorDepartmentAffiliation.Create(
            instructorId, cseFaculty, cseDept,
            AffiliationType.PrimaryCadre, "Prof. Dr.", isPrimary: true,
            DateTime.UtcNow, null, DateTime.UtcNow);

        // Ek Görevlendirme (Yapay Zeka Müh.)
        var secondaryAffiliation = InstructorDepartmentAffiliation.Create(
            instructorId, aiFaculty, aiDept,
            AffiliationType.SecondaryAssignment, "Prof. Dr.", isPrimary: false,
            DateTime.UtcNow, null, DateTime.UtcNow);

        Assert.True(primaryAffiliation.IsPrimary);
        Assert.Equal(AffiliationType.PrimaryCadre, primaryAffiliation.AffiliationType);
        Assert.False(secondaryAffiliation.IsPrimary);
        Assert.Equal(AffiliationType.SecondaryAssignment, secondaryAffiliation.AffiliationType);
        Assert.True(primaryAffiliation.IsActive);
        Assert.True(secondaryAffiliation.IsActive);
    }

    [Theory]
    [InlineData("", "Student.StudentNumberRequired")]
    [InlineData("   ", "Student.StudentNumberRequired")]
    public void Student_Create_RejectsInvalidNumber(string number, string expected)
    {
        var ex = Assert.Throws<DomainException>(() => Student.Create(
            number, "Ahmet", "Yılmaz", "a@u.edu", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2026, DateTime.UtcNow, null, DateTime.UtcNow));
        Assert.Equal(expected, ex.Code);
    }

    [Fact]
    public void Student_Create_RejectsInvalidEmail()
    {
        var ex = Assert.Throws<DomainException>(() => Student.Create(
            "2026001", "Ahmet", "Yılmaz", "invalid-email", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2026, DateTime.UtcNow, null, DateTime.UtcNow));
        Assert.Equal("Student.InvalidEmail", ex.Code);
    }

    [Fact]
    public void Student_Dismissed_CannotBeReversed_AndCannotBeUpdated()
    {
        var student = Student.Create(
            "2026001", "Ahmet", "Yılmaz", "a@u.edu", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 2026, DateTime.UtcNow, null, DateTime.UtcNow);

        student.ChangeStatus(StudentStatus.Dismissed, DateTime.UtcNow);
        Assert.Equal(StudentStatus.Dismissed, student.Status);

        var exStatus = Assert.Throws<DomainException>(() => student.ChangeStatus(StudentStatus.Active, DateTime.UtcNow));
        Assert.Equal("Student.DismissedStatusCannotBeReversed", exStatus.Code);

        var exUpdate = Assert.Throws<DomainException>(() => student.UpdateProfile("Yeni", "Ad", "y@u.edu", DateTime.UtcNow));
        Assert.Equal("Student.Dismissed", exUpdate.Code);
    }
}
