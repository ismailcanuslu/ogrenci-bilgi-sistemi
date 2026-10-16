using System.Text.Json;
using Academic.Application.Abstractions;
using Academic.Domain.Courses;
using Academic.Domain.Curriculums;
using Academic.Domain.Programs;
using Microsoft.EntityFrameworkCore;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class EducationCatalogRepository : IEducationCatalogRepository
{
    private readonly AcademicDbContext _dbContext;

    public EducationCatalogRepository(AcademicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CatalogFacultyNodeDto>> GetAcademicHierarchyTreeAsync(
        DegreeLevel? degreeLevel = null,
        CancellationToken cancellationToken = default)
    {
        var faculties = await _dbContext.Faculties
            .Where(f => f.IsActive)
            .OrderBy(f => f.Name)
            .ToListAsync(cancellationToken);

        var departments = await _dbContext.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);

        var programsQuery = _dbContext.Programs.Where(p => p.IsActive);
        if (degreeLevel.HasValue)
        {
            programsQuery = programsQuery.Where(p => p.DegreeLevel == degreeLevel.Value);
        }

        var programs = await programsQuery.OrderBy(p => p.Name).ToListAsync(cancellationToken);

        var result = new List<CatalogFacultyNodeDto>();

        foreach (var faculty in faculties)
        {
            var facDepts = departments.Where(d => d.FacultyId == faculty.Id).ToList();
            var deptDtos = new List<CatalogDepartmentNodeDto>();

            foreach (var dept in facDepts)
            {
                var deptProgs = programs
                    .Where(p => p.DepartmentId == dept.Id)
                    .Select(p => new CatalogProgramNodeDto(p.Id, p.Code, p.Name, p.DegreeLevel))
                    .ToList();

                if (deptProgs.Count > 0 || !degreeLevel.HasValue)
                {
                    deptDtos.Add(new CatalogDepartmentNodeDto(dept.Id, dept.Name, deptProgs));
                }
            }

            if (deptDtos.Count > 0 || !degreeLevel.HasValue)
            {
                result.Add(new CatalogFacultyNodeDto(faculty.Id, faculty.Name, deptDtos));
            }
        }

        return result;
    }

    public async Task<ProgramProfileDto> GetProgramProfileAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.ProgramProfiles
            .FirstOrDefaultAsync(p => p.ProgramId == programId, cancellationToken);

        if (profile == null)
        {
            profile = ProgramProfile.CreateDefault(programId);
            await _dbContext.ProgramProfiles.AddAsync(profile, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new ProgramProfileDto(
            profile.ProgramId,
            profile.History,
            profile.AwardedDegree,
            profile.DegreeLevelDescription,
            profile.AdmissionRequirements,
            profile.RecognitionOfPriorLearning,
            profile.QualificationRequirements,
            profile.ProgramAims,
            profile.OccupationalProfiles,
            profile.AccessToFurtherStudies,
            profile.ExaminationRegulations,
            profile.GraduationRequirements,
            profile.ModeOfStudy,
            profile.DepartmentHeadAndContact,
            profile.Facilities,
            profile.ProgramEducationalObjectives,
            profile.UpdatedAtUtc);
    }

    public async Task<IReadOnlyCollection<string>> GetProgramOutcomesAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.ProgramProfiles
            .FirstOrDefaultAsync(p => p.ProgramId == programId, cancellationToken);

        if (profile == null)
        {
            profile = ProgramProfile.CreateDefault(programId);
            await _dbContext.ProgramProfiles.AddAsync(profile, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(profile.ProgramOutcomesJson) ?? new List<string>();
        }
        catch
        {
            return new List<string> { "Program çıktıları tanımlanmıştır." };
        }
    }

    public async Task<CatalogTeachingPlanDto?> GetProgramTeachingPlanAsync(
        Guid programId,
        Guid? curriculumVersionId = null,
        CancellationToken cancellationToken = default)
    {
        var curriculums = await _dbContext.Curriculums
            .Include(c => c.Versions)
                .ThenInclude(v => v.Courses)
            .Where(c => c.ProgramId == programId && c.IsActive)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        if (curriculums.Count == 0) return null;

        var curriculum = curriculums.First();
        CurriculumVersion? targetVersion;

        if (curriculumVersionId.HasValue)
        {
            targetVersion = curriculum.Versions.FirstOrDefault(v => v.Id == curriculumVersionId.Value);
        }
        else
        {
            // Önce yayındaki sürüm, yoksa en son sürüm
            targetVersion = curriculum.Versions
                .Where(v => v.Status == CurriculumVersionStatus.Published)
                .OrderByDescending(v => v.VersionNumber)
                .FirstOrDefault() ?? curriculum.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
        }

        if (targetVersion == null) return null;

        var courseCodes = targetVersion.Courses.Select(c => c.CourseCode.ToUpperInvariant()).Distinct().ToList();
        var syllabi = await _dbContext.CourseSyllabi
            .Where(s => courseCodes.Contains(s.CourseCode))
            .ToDictionaryAsync(s => s.CourseCode, s => s, cancellationToken);

        var semesterGroups = targetVersion.Courses
            .GroupBy(c => c.Semester)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var coursesList = g.Select(c =>
                {
                    syllabi.TryGetValue(c.CourseCode.ToUpperInvariant(), out var syl);
                    var theory = syl?.TheoryHours ?? (c.Credit >= 3 ? 3 : c.Credit);
                    var practice = syl?.PracticeHours ?? 0;
                    var lab = syl?.LabHours ?? (c.CourseName.Contains("Programlama", StringComparison.OrdinalIgnoreCase) || c.CourseName.Contains("Fizik", StringComparison.OrdinalIgnoreCase) || c.CourseName.Contains("Kimya", StringComparison.OrdinalIgnoreCase) ? 2 : 0);
                    var lang = syl?.Language ?? "Türkçe";
                    var cType = c.IsElective ? (c.Category == CourseCategory.GeneralElective ? "Üniversite Seçmeli" : "Bölüm Seçmeli") : "Zorunlu";

                    return new CatalogCourseItemDto(
                        c.Id,
                        c.CourseCode,
                        c.CourseName,
                        cType,
                        lang,
                        theory,
                        practice,
                        lab,
                        c.Credit,
                        c.Ects,
                        c.Prerequisites);
                }).ToList();

                var totTheory = coursesList.Sum(x => x.TheoryHours);
                var totPractice = coursesList.Sum(x => x.PracticeHours);
                var totLab = coursesList.Sum(x => x.LabHours);
                var totCredit = coursesList.Sum(x => x.LocalCredit);
                var totEcts = coursesList.Sum(x => x.Ects);

                return new CatalogSemesterGroupDto(
                    g.Key,
                    totTheory,
                    totPractice,
                    totLab,
                    totCredit,
                    totEcts,
                    coursesList);
            }).ToList();

        return new CatalogTeachingPlanDto(
            curriculum.Id,
            targetVersion.Id,
            targetVersion.VersionNumber,
            $"{curriculum.Name} (Sürüm {targetVersion.VersionNumber})",
            semesterGroups);
    }

    public async Task<CourseSyllabusDto?> GetCourseSyllabusAsync(string courseCode, CancellationToken cancellationToken = default)
    {
        var normalizedCode = courseCode.Trim().ToUpperInvariant();
        var syllabus = await _dbContext.CourseSyllabi
            .FirstOrDefaultAsync(s => s.CourseCode == normalizedCode, cancellationToken);

        // Course entity'sinden de temel bilgiyi çekelim
        var course = await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);

        var courseName = course?.Name ?? normalizedCode;
        var ects = course?.DefaultEcts ?? syllabus?.Ects ?? 6;
        var credit = course?.DefaultCredit ?? syllabus?.LocalCredits ?? 4;

        if (syllabus == null)
        {
            // Varsayılan zengin taslak oluşturalım
            return new CourseSyllabusDto(
                normalizedCode,
                courseName,
                "Türkçe",
                "Lisans",
                "Zorunlu",
                course?.Prerequisites ?? "Yok",
                $"{courseName} dersi, öğrencilere temel prensipleri, problem çözme yaklaşımlarını ve güncel yöntemleri aktarmayı amaçlar.",
                $"{courseName} teorik temelleri, haftalık konular, laboratuvar ve uygulama içerikleri.",
                "Ders Notları, Önerilen Ders Kitapları ve Çevrimiçi Akademik Kaynaklar.",
                "- Sözlü Anlatım\n- Tartışma\n- Problem Çözme\n- Laboratuvar / Uygulama",
                "Bulunmamaktadır.",
                "Bölüm Öğretim Elemanları",
                new List<string>
                {
                    $"{courseName} temel kavramlarını tanımlar.",
                    "Problemleri analiz ederek çözüm algoritmaları ve yöntemleri geliştirir.",
                    "Bireysel ve takım halinde laboratuvar çalışmalarını yürütür."
                },
                credit >= 3 ? 3 : credit,
                0,
                2,
                credit,
                ects,
                DateTime.UtcNow);
        }

        IReadOnlyCollection<string> outcomes;
        try
        {
            outcomes = JsonSerializer.Deserialize<List<string>>(syllabus.LearningOutcomesJson) ?? new List<string>();
        }
        catch
        {
            outcomes = new List<string> { $"{courseName} temel kavramlarını açıklar." };
        }

        return new CourseSyllabusDto(
            syllabus.CourseCode,
            courseName,
            syllabus.Language,
            syllabus.Level,
            syllabus.Type,
            syllabus.PrerequisitesText,
            syllabus.Aim,
            syllabus.Content,
            syllabus.TextbooksAndResources,
            syllabus.PlannedLearningActivities,
            syllabus.RecommendedOtherTopics,
            syllabus.InstructorsText,
            outcomes,
            syllabus.TheoryHours,
            syllabus.PracticeHours,
            syllabus.LabHours,
            syllabus.LocalCredits,
            syllabus.Ects,
            syllabus.LastUpdatedAtUtc);
    }

    public async Task UpdateProgramProfileAsync(Guid programId, UpdateProgramProfileDto dto, CancellationToken cancellationToken = default)
    {
        var profile = await _dbContext.ProgramProfiles
            .FirstOrDefaultAsync(p => p.ProgramId == programId, cancellationToken);

        if (profile == null)
        {
            profile = ProgramProfile.CreateDefault(programId);
            await _dbContext.ProgramProfiles.AddAsync(profile, cancellationToken);
        }

        profile.Update(
            dto.History,
            dto.AwardedDegree,
            dto.DegreeLevelDescription,
            dto.AdmissionRequirements,
            dto.RecognitionOfPriorLearning,
            dto.QualificationRequirements,
            dto.ProgramAims,
            dto.OccupationalProfiles,
            dto.AccessToFurtherStudies,
            dto.ExaminationRegulations,
            dto.GraduationRequirements,
            dto.ModeOfStudy,
            dto.DepartmentHeadAndContact,
            dto.Facilities,
            dto.ProgramEducationalObjectives,
            dto.ProgramOutcomesJson);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveCourseSyllabusAsync(CourseSyllabus syllabus, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.CourseSyllabi
            .FirstOrDefaultAsync(s => s.CourseCode == syllabus.CourseCode, cancellationToken);

        if (existing == null)
        {
            await _dbContext.CourseSyllabi.AddAsync(syllabus, cancellationToken);
        }
        else
        {
            existing.Update(
                syllabus.Language,
                syllabus.Level,
                syllabus.Type,
                syllabus.PrerequisitesText,
                syllabus.Aim,
                syllabus.Content,
                syllabus.TextbooksAndResources,
                syllabus.PlannedLearningActivities,
                syllabus.InstructorsText,
                syllabus.LearningOutcomesJson,
                syllabus.TheoryHours,
                syllabus.PracticeHours,
                syllabus.LabHours,
                syllabus.LocalCredits,
                syllabus.Ects);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
