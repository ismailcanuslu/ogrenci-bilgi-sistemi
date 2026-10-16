using Academic.Domain.AcademicCalendars;
using Academic.Domain.AcademicYears;
using Academic.Domain.Courses;
using Academic.Domain.Curriculums;
using Academic.Domain.Dashboard;
using Academic.Domain.Departments;
using Academic.Domain.Faculties;
using Academic.Domain.Programs;
using Academic.Domain.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Academic.Infrastructure.Persistence;

public sealed class AcademicDataSeeder(
    IServiceScopeFactory scopeFactory,
    ILogger<AcademicDataSeeder> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AcademicDbContext>();

            await SeedAcademicCoreDataAsync(db, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Academic modülü veri tohumlama (seed) hatası oluştu.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task SeedAcademicCoreDataAsync(AcademicDbContext db, CancellationToken ct)
    {
        var nowUtc = DateTime.UtcNow;

        // 1. Fakülte Seed
        var faculty = await db.Faculties.FirstOrDefaultAsync(f => f.Code == "MDBF", ct);
        if (faculty is null)
        {
            faculty = Faculty.Create("MDBF", "Mühendislik ve Doğa Bilimleri Fakültesi", nowUtc);
            db.Faculties.Add(faculty);
            await db.SaveChangesAsync(ct);
        }

        // 2. Bölümler Seed
        var compDept = await db.Departments.FirstOrDefaultAsync(d => d.Code == "BLG", ct);
        if (compDept is null)
        {
            compDept = Department.Create(faculty.Id, "BLG", "Bilgisayar Mühendisliği Bölümü", nowUtc);
            db.Departments.Add(compDept);
            await db.SaveChangesAsync(ct);
        }

        var softDept = await db.Departments.FirstOrDefaultAsync(d => d.Code == "YZM", ct);
        if (softDept is null)
        {
            softDept = Department.Create(faculty.Id, "YZM", "Yazılım Mühendisliği Bölümü", nowUtc);
            db.Departments.Add(softDept);
            await db.SaveChangesAsync(ct);
        }

        // 3. Akademik Yıl ve Dönem Seed
        var year = await db.AcademicYears.Include(y => y.Terms).FirstOrDefaultAsync(y => y.Code == "2026-2027", ct);
        if (year is null)
        {
            year = AcademicYear.Create(
                "2026-2027",
                "2026-2027 Akademik Yılı",
                new DateOnly(2026, 9, 15),
                new DateOnly(2027, 6, 20),
                nowUtc);

            year.AddTerm(
                TermType.Fall,
                "2026-2027 Güz Dönemi",
                new DateOnly(2026, 9, 15),
                new DateOnly(2027, 1, 20),
                isRegistrationOpen: true);

            db.AcademicYears.Add(year);
            await db.SaveChangesAsync(ct);
        }

        // 4. Lisans Programları Seed
        var compProgram = await db.Programs.FirstOrDefaultAsync(p => p.Code == "BLG-BS", ct);
        if (compProgram is null)
        {
            compProgram = Program.Create(compDept.Id, "BLG-BS", "Bilgisayar Mühendisliği Lisans Programı", DegreeLevel.Undergraduate, nowUtc);
            db.Programs.Add(compProgram);
            await db.SaveChangesAsync(ct);
        }

        var softProgram = await db.Programs.FirstOrDefaultAsync(p => p.Code == "YZM-BS", ct);
        if (softProgram is null)
        {
            softProgram = Program.Create(softDept.Id, "YZM-BS", "Yazılım Mühendisliği Lisans Programı", DegreeLevel.Undergraduate, nowUtc);
            db.Programs.Add(softProgram);
            await db.SaveChangesAsync(ct);
        }

        // 5. Müfredat & Versiyon Seed
        var curriculum = await db.Curriculums.Include(c => c.Versions).FirstOrDefaultAsync(c => c.ProgramId == compProgram.Id, ct);
        if (curriculum is null)
        {
            curriculum = Curriculum.Create(compProgram.Id, year.Id, "BLG-2024", "2024 Bilgisayar Mühendisliği Müfredatı", nowUtc);
            db.Curriculums.Add(curriculum);
            await db.SaveChangesAsync(ct);
        }

        var curVersion = curriculum.Versions.First();
        curVersion.Publish(nowUtc);

        // 6. Dersler Seed
        var coursesToSeed = new[]
        {
            ("BLG101", "Programlamaya Giriş (C/C++)", "Temel algoritma ve programlama mantığı", 6, 4, 1),
            ("MAT101", "Matematik I", "Kalkülüs, türev ve integral temel kavramları", 6, 4, 1),
            ("FZK101", "Genel Fizik I", "Mekanik ve hareket dinamikleri", 6, 4, 1),
            ("BLG201", "Veri Yapıları ve Algoritmalar", "Ağaçlar, graflar, sıralama algoritmaları", 6, 4, 3),
            ("BLG301", "Veritabanı Yönetim Sistemleri", "İlişkisel veritabanları, SQL ve indeksleme", 6, 4, 5),
            ("BLG411", "Bitirme Tasarım Projesi", "Yazılım geliştirme mühendislik projesi", 8, 3, 7),
            ("BLG452", "Bulut Bilişim ve Mikroservisler", "Dağıtık sistemler, Docker, Kubernetes mimarisi", 5, 3, 7)
        };

        foreach (var (code, name, desc, ects, credit, semester) in coursesToSeed)
        {
            var course = await db.Courses.FirstOrDefaultAsync(c => c.Code == code, ct);
            if (course is null)
            {
                course = Course.Create(compDept.Id, code, name, desc, ects, credit, null, nowUtc);
                db.Courses.Add(course);
                await db.SaveChangesAsync(ct);

                // Müfredata versiyon üzerinden ders ekle
                var curCourseExists = curVersion.Courses.Any(c => c.CourseCode == code);
                if (!curCourseExists)
                {
                    curVersion.AddCourse(code, name, ects, credit, semester, isElective: false, CourseCategory.Mandatory, null);
                }
            }
        }
        await db.SaveChangesAsync(ct);

        // 7. Öğrenci Seed: İsmailcan Uslu
        var student = await db.Students.Include(s => s.ProgramEnrollments).FirstOrDefaultAsync(s => s.StudentNumber == "20210101042", ct);
        if (student is null)
        {
            student = Student.Create(
                studentNumber: "20210101042",
                firstName: "İsmailcan",
                lastName: "Uslu",
                email: "ogrenci@obs.edu.tr",
                departmentId: compDept.Id,
                programId: compProgram.Id,
                curriculumVersionId: curVersion.Id,
                enrollmentYear: 2021,
                enrollmentDateUtc: new DateTime(2021, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                userId: null,
                createdAtUtc: nowUtc);

            // Çift Anadal (ÇAP) Kaydı: Yazılım Mühendisliği
            student.EnrollInProgram(
                softDept.Id,
                softProgram.Id,
                curVersion.Id,
                EnrollmentType.DoubleMajor,
                2022,
                new DateTime(2022, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                isPrimary: false,
                notes: "Yazılım Mühendisliği Çift Anadal (ÇAP) Programı");

            db.Students.Add(student);
            await db.SaveChangesAsync(ct);

            // Danışman Ataması Seed
            var advisorAssignment = new StudentAdvisorAssignment(
                Guid.NewGuid(),
                student.Id,
                instructorId: Guid.NewGuid(),
                assignedDateUtc: nowUtc,
                isActive: true);
            db.StudentAdvisorAssignments.Add(advisorAssignment);

            // Dashboard Widget'ları Seed (1. görseldeki gibi)
            var widgets = new[]
            {
                (WidgetType.CourseList, "Dönem Dersleri", 1, 2),
                (WidgetType.WeeklySchedule, "Haftalık Ders Takvimi", 2, 2),
                (WidgetType.AcademicCalendar, "Akademik Takvim & Önemli Tarihler", 3, 1),
                (WidgetType.UpcomingExams, "Yaklaşan Sınavlar ve Teslimler", 4, 1),
                (WidgetType.Announcements, "Duyurular ve Bilgilendirmeler", 5, 2),
                (WidgetType.QuickShortcuts, "Hızlı Kısayollar", 6, 2)
            };

            foreach (var (wType, title, order, span) in widgets)
            {
                var w = new UserDashboardWidget(
                    Guid.NewGuid(),
                    student.Id,
                    wType,
                    title,
                    order,
                    span,
                    configurationJson: null,
                    nowUtc);
                db.UserDashboardWidgets.Add(w);
            }

            // Kişisel Kısayollar Seed
            var shortcuts = new[]
            {
                ("Resmi Transkript İste", "/student/transcript", "file-text", "#0f4c81", 1),
                ("Ders Kaydı / Ekle-Bırak", "/student/course-registration", "form", "#10b981", 2),
                ("Sınav Sonuçları", "/student/exams", "schedule", "#f59e0b", 3),
                ("Eğitim Kataloğu & Müfredat", "/education-catalog", "book", "#6366f1", 4),
                ("Toplumsal Katkı Portalı", "/social-contribution", "heart", "#ec4899", 5),
                ("Kurumsal Değerlendirme", "/institutional-evaluation", "bar-chart", "#0284c7", 6)
            };

            foreach (var (sTitle, sUrl, sIcon, sColor, sOrder) in shortcuts)
            {
                var sc = new UserPersonalShortcut(
                    Guid.NewGuid(),
                    student.Id,
                    sTitle,
                    sUrl,
                    sIcon,
                    sColor,
                    sOrder);
                db.UserPersonalShortcuts.Add(sc);
            }

            await db.SaveChangesAsync(ct);
        }

        // 8. Akademik Takvim Etkinlikleri Seed
        var existingEvents = await db.AcademicCalendarEvents.AnyAsync(ct);
        if (!existingEvents)
        {
            var calendarEvents = new[]
            {
                new AcademicCalendarEvent(Guid.NewGuid(), "2026-2027 Güz Yarıyılı Ders Kayıtları", "Ders seçimi ve danışman onayları", new DateOnly(2026, 9, 15), new DateOnly(2026, 9, 25), "2026-2027", "Güz", true),
                new AcademicCalendarEvent(Guid.NewGuid(), "Ders Ekle / Bırak Haftası", "Mazeretli ders ekle-bırak süreci", new DateOnly(2026, 9, 26), new DateOnly(2026, 10, 2), "2026-2027", "Güz", false),
                new AcademicCalendarEvent(Guid.NewGuid(), "Ara Sınav (Vize) Dönemi", "1. Dönem ara sınavları", new DateOnly(2026, 11, 1), new DateOnly(2026, 11, 15), "2026-2027", "Güz", true),
                new AcademicCalendarEvent(Guid.NewGuid(), "Yarıyıl Sonu (Final) Sınavları", "Güz dönemi final sınavları", new DateOnly(2027, 1, 5), new DateOnly(2027, 1, 18), "2026-2027", "Güz", true)
            };
            db.AcademicCalendarEvents.AddRange(calendarEvents);
            await db.SaveChangesAsync(ct);
        }

        // 9. Program Tanımı (Eğitim Kataloğu) Seed
        var programProfile = await db.ProgramProfiles.FirstOrDefaultAsync(p => p.ProgramId == compProgram.Id, ct);
        if (programProfile is null)
        {
            programProfile = ProgramProfile.CreateDefault(compProgram.Id);
            db.ProgramProfiles.Add(programProfile);
            await db.SaveChangesAsync(ct);
        }
    }
}
