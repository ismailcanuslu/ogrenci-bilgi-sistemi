using CampusLife.Domain.Clubs;
using CampusLife.Domain.Reservations;
using BuildingBlocks.Domain.Exceptions;
using Xunit;

namespace UniversalSystem.UnitTests.CampusLife;

public class CampusLifeDomainTests
{
    [Fact]
    public void FacilityReservation_Create_SetsPendingAndValidData()
    {
        var userId = Guid.NewGuid();
        var date = new DateOnly(2026, 11, 20);
        var start = new TimeSpan(14, 0, 0);
        var end = new TimeSpan(15, 0, 0);

        var reservation = FacilityReservation.Create(
            userId,
            FacilityType.AstroTurf,
            "Kuzey Kampüs Halı Saha 1",
            date,
            start,
            end,
            "Mühendislik turnuva maçı",
            DateTime.UtcNow);

        Assert.Equal(userId, reservation.UserId);
        Assert.Equal(FacilityType.AstroTurf, reservation.FacilityType);
        Assert.Equal("Kuzey Kampüs Halı Saha 1", reservation.FacilityName);
        Assert.Equal(ReservationStatus.Pending, reservation.Status);
        Assert.Null(reservation.ReviewedByUserId);
    }

    [Fact]
    public void FacilityReservation_Create_RejectsInvalidTimeRange()
    {
        var ex = Assert.Throws<DomainException>(() => FacilityReservation.Create(
            Guid.NewGuid(),
            FacilityType.SportsHall,
            "Kapalı Spor Salonu",
            new DateOnly(2026, 11, 20),
            new TimeSpan(16, 0, 0),
            new TimeSpan(15, 0, 0), // Bitiş başlangıçtan önce!
            null,
            DateTime.UtcNow));

        Assert.Equal("FacilityReservation.InvalidTimeRange", ex.Code);
    }

    [Fact]
    public void FacilityReservation_ApproveAndReject_ChangesStatusCorrectly()
    {
        var reviewerId = Guid.NewGuid();
        var res1 = FacilityReservation.Create(
            Guid.NewGuid(), FacilityType.PsychologicalCounseling, "PDR Görüşme Odası 1",
            new DateOnly(2026, 11, 20), new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0),
            "Görüşme talebi", DateTime.UtcNow);

        res1.Approve(reviewerId, "Onaylandı. Randevu saatinizde hazır bulununuz.", DateTime.UtcNow);
        Assert.Equal(ReservationStatus.Approved, res1.Status);
        Assert.Equal(reviewerId, res1.ReviewedByUserId);

        var res2 = FacilityReservation.Create(
            Guid.NewGuid(), FacilityType.SocialMarket, "Sosyal Market Dağıtım",
            new DateOnly(2026, 11, 20), new TimeSpan(13, 0, 0), new TimeSpan(14, 0, 0),
            "İhtiyaç paketi", DateTime.UtcNow);

        res2.Reject(reviewerId, "Belirtilen saatte market kapalıdır.", DateTime.UtcNow);
        Assert.Equal(ReservationStatus.Rejected, res2.Status);
        Assert.Equal(reviewerId, res2.ReviewedByUserId);
        Assert.Equal("Belirtilen saatte market kapalıdır.", res2.ReviewNotes);
    }

    [Fact]
    public void FacilityReservation_OverlapsWith_DetectsTimeConflict()
    {
        var date = new DateOnly(2026, 11, 20);
        var res = FacilityReservation.Create(
            Guid.NewGuid(), FacilityType.AstroTurf, "Saha 1",
            date, new TimeSpan(18, 0, 0), new TimeSpan(19, 0, 0), null, DateTime.UtcNow);

        // Çakışan: 18:30 - 19:30
        Assert.True(res.OverlapsWith(date, new TimeSpan(18, 30, 0), new TimeSpan(19, 30, 0)));

        // Çakışmayan: 19:00 - 20:00
        Assert.False(res.OverlapsWith(date, new TimeSpan(19, 0, 0), new TimeSpan(20, 0, 0)));

        // Farklı tarih
        Assert.False(res.OverlapsWith(new DateOnly(2026, 11, 21), new TimeSpan(18, 0, 0), new TimeSpan(19, 0, 0)));
    }

    [Fact]
    public void StudentClub_And_ClubEvent_ApprovalFlow()
    {
        var advisorId = Guid.NewGuid();
        var presidentId = Guid.NewGuid();
        var reviewerId = Guid.NewGuid();

        var club = StudentClub.Create(
            "Yazılım ve Yapay Zeka Topluluğu",
            "YYZT",
            "Bilim ve Teknoloji",
            "Yazılım ve AI odaklı öğrenci kulübü",
            advisorId,
            presidentId,
            DateTime.UtcNow);

        Assert.True(club.IsActive);
        Assert.Equal("YYZT", club.Code);

        var ev = ClubEvent.Create(
            club.Id,
            "Güz Hackathon 2026",
            "48 saatlik yapay zeka hackathonu",
            DateTime.UtcNow.AddDays(10),
            "Merkez Kütüphane İnovasyon Alanı",
            100,
            DateTime.UtcNow);

        Assert.Equal(EventStatus.PendingApproval, ev.Status);

        ev.Approve(reviewerId, "SKS tarafından etkinlik onaylandı.", DateTime.UtcNow);
        Assert.Equal(EventStatus.Approved, ev.Status);
        Assert.Equal(reviewerId, ev.ReviewedByUserId);
    }
}
