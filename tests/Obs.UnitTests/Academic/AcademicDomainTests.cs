using Academic.Domain.Departments;
using Academic.Domain.Faculties;
using BuildingBlocks.Domain.Exceptions;
using Xunit;

namespace UniversalSystem.UnitTests.Academic;

public class AcademicDomainTests
{
    [Fact]
    public void Faculty_Create_NormalizesCodeAndName()
    {
        var faculty = Faculty.Create("  eng ", "  Mühendislik  ", DateTime.UtcNow);

        Assert.Equal("ENG", faculty.Code);
        Assert.Equal("Mühendislik", faculty.Name);
        Assert.True(faculty.IsActive);
    }

    [Theory]
    [InlineData("", "Ad", "Faculty.CodeRequired")]
    [InlineData("ENG", " ", "Faculty.NameRequired")]
    public void Faculty_Create_RejectsInvalidInput(string code, string name, string expected)
    {
        var ex = Assert.Throws<DomainException>(() => Faculty.Create(code, name, DateTime.UtcNow));
        Assert.Equal(expected, ex.Code);
    }

    [Fact]
    public void Faculty_CannotBeUpdated_WhenInactive()
    {
        var faculty = Faculty.Create("ENG", "Mühendislik", DateTime.UtcNow);
        faculty.Deactivate(DateTime.UtcNow);

        var ex = Assert.Throws<DomainException>(() => faculty.Update("ENG", "Yeni", DateTime.UtcNow));
        Assert.Equal("Faculty.Inactive", ex.Code);
    }

    [Fact]
    public void Department_RequiresFaculty_AndKeepsItAfterUpdate()
    {
        Assert.Throws<DomainException>(() => Department.Create(Guid.Empty, "CSE", "Bilgisayar", DateTime.UtcNow));

        var facultyId = Guid.NewGuid();
        var department = Department.Create(facultyId, "cse", "Bilgisayar", DateTime.UtcNow);
        department.Update("ce", "Bilgisayar Müh.", DateTime.UtcNow);

        Assert.Equal(facultyId, department.FacultyId);
        Assert.Equal("CE", department.Code);
    }

    [Fact]
    public void Department_DeactivateTwice_Throws()
    {
        var department = Department.Create(Guid.NewGuid(), "CSE", "Bilgisayar", DateTime.UtcNow);
        department.Deactivate(DateTime.UtcNow);

        Assert.Throws<DomainException>(() => department.Deactivate(DateTime.UtcNow));
    }
}
