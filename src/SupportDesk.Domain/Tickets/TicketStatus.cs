namespace SupportDesk.Domain.Tickets;

public enum TicketPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}

public enum TicketStatus
{
    Open = 0,        // Açık / Yeni
    InProgress = 1,  // İşlemde / İnceleniyor
    Answered = 2,    // Yetkili Birim Tarafından Yanıtlandı
    Resolved = 3,    // Çözümlendi
    Closed = 4       // Kapatıldı
}
