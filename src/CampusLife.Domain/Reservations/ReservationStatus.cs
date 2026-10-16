namespace CampusLife.Domain.Reservations;

public enum ReservationStatus
{
    Pending = 0,    // Onay Bekliyor
    Approved = 1,   // Onaylandı
    Rejected = 2,   // Reddedildi
    Cancelled = 3,  // İptal Edildi
    Completed = 4   // Gerçekleşti / Tamamlandı
}
