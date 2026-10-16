using Academic.Domain.Courses;
using BuildingBlocks.Domain.Exceptions;
using Xunit;

namespace UniversalSystem.UnitTests.Academic;

public class CourseDomainTests
{
    [Fact]
    public void Course_Create_NormalizesCodeAndPrerequisites()
    {
        var course = Course.Create(
            Guid.NewGuid(),
            "  ceng101  ",
            "  Bilgisayar Programlama I  ",
            "Temel programlama",
            6,
            3,
            " math101 , ceng100 ",
            DateTime.UtcNow);

        Assert.Equal("CENG101", course.Code);
        Assert.Equal("Bilgisayar Programlama I", course.Name);
        Assert.Equal("MATH101,CENG100", course.Prerequisites);
        Assert.True(course.IsActive);
    }

    [Theory]
    [InlineData(0, "Course.InvalidEcts")]
    [InlineData(65, "Course.InvalidEcts")]
    public void Course_Create_RejectsInvalidEcts(int ects, string expected)
    {
        var ex = Assert.Throws<DomainException>(() => Course.Create(
            Guid.NewGuid(), "CENG101", "Giriş", null, ects, 3, null, DateTime.UtcNow));
        Assert.Equal(expected, ex.Code);
    }

    [Fact]
    public void CourseOffering_Capacity_CannotBeDecreasedBelowEnrolled()
    {
        var offering = CourseOffering.Create(Guid.NewGuid(), Guid.NewGuid(), 1, 30, null, DateTime.UtcNow);

        offering.IncrementEnrolled();
        offering.IncrementEnrolled();
        Assert.Equal(2, offering.EnrolledCount);

        var ex = Assert.Throws<DomainException>(() => offering.UpdateCapacity(1, DateTime.UtcNow));
        Assert.Equal("CourseOffering.CapacityCannotBeLessThanEnrolled", ex.Code);
    }

    [Fact]
    public void CourseOffering_Schedule_RejectsOverlappingTimesInSameSection()
    {
        var offering = CourseOffering.Create(Guid.NewGuid(), Guid.NewGuid(), 1, 50, null, DateTime.UtcNow);

        // Pazartesi 09:00 - 11:00
        offering.AddSchedule(
            SessionType.Theory,
            "CSE Fakültesi",
            "D101",
            DayOfWeek.Monday,
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(11),
            null,
            false);

        // Aynı gün çakışan saat: Pazartesi 10:00 - 12:00
        var ex = Assert.Throws<DomainException>(() => offering.AddSchedule(
            SessionType.Practice,
            "CSE Fakültesi",
            "D102",
            DayOfWeek.Monday,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(12),
            null,
            false));

        Assert.Equal("CourseOffering.ScheduleConflictInSameSection", ex.Code);
    }

    [Fact]
    public void CourseOffering_Schedule_AllowsDifferentFacultyAndClassroomForTheoryAndLab()
    {
        var primaryProfId = Guid.NewGuid();
        var labAssistantId = Guid.NewGuid();

        var offering = CourseOffering.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            50,
            primaryProfId,
            DateTime.UtcNow);

        // İkinci eğitmen olarak lab asistanını ekle
        offering.AssignInstructor(labAssistantId, false, "Laboratuvar Asistanı", DateTime.UtcNow);

        // Teori: CSE Fakültesi - D101 (Prof anlatıyor)
        var theorySchedule = offering.AddSchedule(
            SessionType.Theory,
            "CSE Fakültesi",
            "D101",
            DayOfWeek.Monday,
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(11),
            primaryProfId,
            false);

        // Uygulama / Laboratuvar: EEE Fakültesi - LAB3 (Asistan yürütüyor)
        var labSchedule = offering.AddSchedule(
            SessionType.Laboratory,
            "EEE Fakültesi",
            "LAB3",
            DayOfWeek.Wednesday,
            TimeSpan.FromHours(13),
            TimeSpan.FromHours(15),
            labAssistantId,
            false);

        Assert.Equal(2, offering.Schedules.Count);
        Assert.Equal(2, offering.Instructors.Count);
        Assert.Equal(primaryProfId, offering.PrimaryInstructorId);
        Assert.True(offering.IsInstructorAssigned(primaryProfId));
        Assert.True(offering.IsInstructorAssigned(labAssistantId));

        Assert.Equal(SessionType.Theory, theorySchedule.SessionType);
        Assert.Equal("CSE Fakültesi", theorySchedule.BuildingOrFaculty);
        Assert.Equal("D101", theorySchedule.Classroom);

        Assert.Equal(SessionType.Laboratory, labSchedule.SessionType);
        Assert.Equal("EEE Fakültesi", labSchedule.BuildingOrFaculty);
        Assert.Equal("LAB3", labSchedule.Classroom);
    }

    [Fact]
    public void CourseOffering_Instructors_CanBeAssignedAndRemoved()
    {
        var prof1 = Guid.NewGuid();
        var prof2 = Guid.NewGuid();
        var offering = CourseOffering.Create(Guid.NewGuid(), Guid.NewGuid(), 1, 40, prof1, DateTime.UtcNow);

        Assert.Single(offering.Instructors);
        Assert.Equal(prof1, offering.PrimaryInstructorId);

        // İkinci eğitmeni birincil olarak ata
        offering.AssignInstructor(prof2, true, "Koordinatör", DateTime.UtcNow);
        Assert.Equal(2, offering.Instructors.Count);
        Assert.Equal(prof2, offering.PrimaryInstructorId);

        // İlk eğitmeni kaldır
        offering.RemoveInstructor(prof1, DateTime.UtcNow);
        Assert.Single(offering.Instructors);
        Assert.Equal(prof2, offering.Instructors.First().InstructorId);
    }
}
