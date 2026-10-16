using System;
using System.Linq;
using SocialContribution.Domain;
using Xunit;

namespace UniversalSystem.UnitTests;

public class SocialContributionTests
{
    [Fact]
    public void CreateProject_ValidInput_ShouldCreatePendingApprovalProject()
    {
        var projectId = Guid.NewGuid();
        var coordinatorId = Guid.NewGuid();

        var project = SocialContributionProject.Create(
            projectId,
            "Köy Okullarına Kitap Yardımı",
            "Kırsal bölgelerdeki okullara kütüphane kurulumu desteği",
            ContributionCategory.SocialResponsibility,
            coordinatorId,
            "Prof. Dr. Ahmet Yılmaz",
            "İlköğretim Öğrencileri",
            "MEB & Kızılay",
            DateTime.UtcNow.AddDays(5),
            DateTime.UtcNow.AddDays(30),
            15);

        Assert.Equal(projectId, project.Id);
        Assert.Equal("Köy Okullarına Kitap Yardımı", project.Title);
        Assert.Equal(ProjectStatus.PendingApproval, project.Status);
        Assert.Equal(15, project.VolunteerQuota);
        Assert.Equal(0, project.ApprovedVolunteerCount);
        Assert.Empty(project.Volunteers);
    }

    [Fact]
    public void ApproveAndStartProject_ShouldTransitionStatus()
    {
        var project = SocialContributionProject.Create(
            Guid.NewGuid(),
            "Çevre Temizliği Seferberliği",
            "Sahil ve orman atıklarının toplanması",
            ContributionCategory.EnvironmentalSustainability,
            Guid.NewGuid(),
            "Doç. Dr. Ayşe Kaya",
            "Halk",
            "Belediye",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(10),
            20);

        project.Approve();
        Assert.Equal(ProjectStatus.Approved, project.Status);

        project.StartProject();
        Assert.Equal(ProjectStatus.InProgress, project.Status);

        project.CompleteProject("500 kg atık toplandı", 300);
        Assert.Equal(ProjectStatus.Completed, project.Status);
        Assert.Equal("500 kg atık toplandı", project.ImpactSummary);
        Assert.Equal(300, project.BeneficiaryCount);
    }

    [Fact]
    public void VolunteerApplication_AndApproval_ShouldUpdateCountAndQuota()
    {
        var project = SocialContributionProject.Create(
            Guid.NewGuid(),
            "Kodlama Atölyesi",
            "Lise öğrencilerine Python ve Yapay Zeka eğitimi",
            ContributionCategory.PublicEducation,
            Guid.NewGuid(),
            "Dr. Mehmet Demir",
            "Lise Öğrencileri",
            "TÜBİTAK",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(20),
            1); // Kontenjan 1

        project.Approve();

        var student1Id = Guid.NewGuid();
        var vol1Id = Guid.NewGuid();

        var vol1 = project.AddVolunteerApplication(vol1Id, student1Id, "2023101", "Can Uslu", "can@uni.edu.tr", "Python biliyorum");
        Assert.Single(project.Volunteers);
        Assert.Equal(VolunteerApplicationStatus.Pending, vol1.Status);

        // Onayla
        project.UpdateVolunteerStatus(vol1Id, VolunteerApplicationStatus.Approved);
        Assert.Equal(1, project.ApprovedVolunteerCount);
        Assert.Equal(VolunteerApplicationStatus.Approved, vol1.Status);

        // Kontenjan doldu, 2. kişi başvurup onaylanmaya çalışırsa hata vermeli
        var student2Id = Guid.NewGuid();
        var vol2Id = Guid.NewGuid();
        project.AddVolunteerApplication(vol2Id, student2Id, "2023102", "Ali Veli", "ali@uni.edu.tr", "Mentorlük yapabilirim");

        Assert.Throws<InvalidOperationException>(() =>
            project.UpdateVolunteerStatus(vol2Id, VolunteerApplicationStatus.Approved));
    }
}
