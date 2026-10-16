namespace DocumentManagement.Domain.Documents;

public enum DocumentType
{
    OfficialLetter = 1,      // Resmi Yazı
    IncomingDocument = 2,    // Gelen Evrak
    OutgoingDocument = 3,    // Giden Evrak
    BoardDecision = 4,       // Kurul / Senato Kararı
    Petition = 5,            // Dilekçe
    StudentCertificate = 6,  // Öğrenci Belgesi
    Transcript = 7           // Transkript
}

public enum DocumentStatus
{
    Draft = 1,
    InWorkflow = 2,
    Signed = 3,
    Rejected = 4,
    Revoked = 5
}

public enum WorkflowStepType
{
    Review = 1, // Paraf
    Sign = 2    // İmza
}

public enum WorkflowStepStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
