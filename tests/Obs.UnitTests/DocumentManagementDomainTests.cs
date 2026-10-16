using DocumentManagement.Domain.Documents;
using Xunit;

namespace UniversalSystem.UnitTests;

public class DocumentManagementDomainTests
{
    [Fact]
    public void Create_OfficialDocument_ShouldInitializeWithVerificationCodeAndHash()
    {
        // Arrange
        var creatorUserId = Guid.NewGuid();

        // Act
        var doc = OfficialDocument.Create(
            "Yaz Okulu Yönetmelik Değişikliği",
            "Üniversite senatosuna sunulacak yaz okulu esasları.",
            DocumentType.OfficialLetter,
            creatorUserId,
            "CENG",
            null,
            DateTime.UtcNow);

        // Assert
        Assert.NotNull(doc);
        Assert.Equal("Yaz Okulu Yönetmelik Değişikliği", doc.Subject);
        Assert.Equal(DocumentType.OfficialLetter, doc.Type);
        Assert.Equal(DocumentStatus.Draft, doc.Status);
        Assert.Equal("CENG", doc.DepartmentCode);
        Assert.False(string.IsNullOrWhiteSpace(doc.VerificationCode));
        Assert.StartsWith("DOC-", doc.VerificationCode);
        Assert.False(string.IsNullOrWhiteSpace(doc.VerificationHash));
        Assert.Null(doc.DocumentNumber);
        Assert.Null(doc.SignedAtUtc);
    }

    [Fact]
    public void Workflow_ShouldProgressThroughReviewAndSignSteps_ToSignedStatus()
    {
        // Arrange
        var creatorUserId = Guid.NewGuid();
        var reviewerUserId = Guid.NewGuid();
        var signerUserId = Guid.NewGuid();

        var doc = OfficialDocument.Create(
            "Bölüm Kurulu Kararı",
            "Ders intibaklarının onaylanması hk.",
            DocumentType.BoardDecision,
            creatorUserId,
            "CENG",
            null,
            DateTime.UtcNow);

        doc.AddWorkflowStep(WorkflowStepType.Review, reviewerUserId);
        doc.AddWorkflowStep(WorkflowStepType.Sign, signerUserId);
        doc.SubmitToWorkflow();

        Assert.Equal(DocumentStatus.InWorkflow, doc.Status);
        Assert.Equal(2, doc.WorkflowSteps.Count);

        var step1 = doc.WorkflowSteps.First(s => s.StepOrder == 1);
        var step2 = doc.WorkflowSteps.First(s => s.StepOrder == 2);

        // Act 1: Paraflayan onaylar
        doc.ProcessStep(step1.Id, reviewerUserId, true, "Uygun görülmüştür.", DateTime.UtcNow);
        Assert.Equal(DocumentStatus.InWorkflow, doc.Status);
        Assert.Equal(WorkflowStepStatus.Approved, step1.Status);

        // Act 2: İmzalayan nihai onayı verir
        doc.ProcessStep(step2.Id, signerUserId, true, "Onaylandı.", DateTime.UtcNow);

        // Assert
        Assert.Equal(DocumentStatus.Signed, doc.Status);
        Assert.Equal(WorkflowStepStatus.Approved, step2.Status);
        Assert.NotNull(doc.SignedAtUtc);
        Assert.NotNull(doc.DocumentNumber);
        Assert.StartsWith("E-CENG-", doc.DocumentNumber);
    }

    [Fact]
    public void Workflow_WhenStepIsRejected_ShouldRejectDocument()
    {
        // Arrange
        var creatorUserId = Guid.NewGuid();
        var reviewerUserId = Guid.NewGuid();

        var doc = OfficialDocument.Create(
            "Öğrenci Dilekçesi",
            "Mazeret sınav talebi",
            DocumentType.Petition,
            creatorUserId,
            "CENG",
            null,
            DateTime.UtcNow);

        doc.AddWorkflowStep(WorkflowStepType.Review, reviewerUserId);
        doc.SubmitToWorkflow();

        var step1 = doc.WorkflowSteps.First();

        // Act: Reddet
        doc.ProcessStep(step1.Id, reviewerUserId, false, "Sağlık raporu süresi geçmiş.", DateTime.UtcNow);

        // Assert
        Assert.Equal(DocumentStatus.Rejected, doc.Status);
        Assert.Equal(WorkflowStepStatus.Rejected, step1.Status);
        Assert.Null(doc.DocumentNumber);
    }

    [Fact]
    public void Revoke_SignedDocument_ShouldSetRevokedStatus()
    {
        // Arrange
        var creatorUserId = Guid.NewGuid();
        var signerUserId = Guid.NewGuid();

        var doc = OfficialDocument.Create(
            "Geçici Mezuniyet Belgesi",
            "Mezuniyet yazısı",
            DocumentType.StudentCertificate,
            creatorUserId,
            "OGRIS",
            null,
            DateTime.UtcNow);

        doc.AddWorkflowStep(WorkflowStepType.Sign, signerUserId);
        doc.SubmitToWorkflow();

        var step = doc.WorkflowSteps.First();
        doc.ProcessStep(step.Id, signerUserId, true, "İmzalandı.", DateTime.UtcNow);
        Assert.Equal(DocumentStatus.Signed, doc.Status);

        // Act
        doc.Revoke(DateTime.UtcNow);

        // Assert
        Assert.Equal(DocumentStatus.Revoked, doc.Status);
    }
}
