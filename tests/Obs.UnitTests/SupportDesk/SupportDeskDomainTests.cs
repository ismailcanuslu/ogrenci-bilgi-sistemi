using SupportDesk.Domain.Tickets;
using BuildingBlocks.Domain.Exceptions;
using Xunit;

namespace UniversalSystem.UnitTests.SupportDesk;

public class SupportDeskDomainTests
{
    [Fact]
    public void SupportTicket_Create_GeneratesTicketNumberAndOpensStatus()
    {
        var studentId = Guid.NewGuid();
        var ticket = SupportTicket.Create(
            studentId,
            SupportUnit.StudentAffairs,
            "Transkript Belgesi Talebi",
            TicketPriority.Normal,
            "İngilizce onaylı transkript talep ediyorum.",
            null,
            DateTime.UtcNow);

        Assert.Equal(studentId, ticket.RequesterUserId);
        Assert.Equal(SupportUnit.StudentAffairs, ticket.TargetUnit);
        Assert.Equal("Transkript Belgesi Talebi", ticket.Subject);
        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.StartsWith("SRV-", ticket.TicketNumber);
        Assert.Single(ticket.Messages);
        Assert.Equal("İngilizce onaylı transkript talep ediyorum.", ticket.Messages.First().Content);
        Assert.False(ticket.Messages.First().IsFromStaff);
    }

    [Fact]
    public void SupportTicket_AddMessage_UpdatesStatusToAnsweredWhenFromStaff()
    {
        var studentId = Guid.NewGuid();
        var staffId = Guid.NewGuid();

        var ticket = SupportTicket.Create(
            studentId,
            SupportUnit.ITDepartment,
            "Öğrenci e-posta şifre sıfırlama",
            TicketPriority.High,
            "Şifremi unuttum, yardımcı olabilir misiniz?",
            null,
            DateTime.UtcNow);

        // Personel bilet atama
        ticket.AssignStaff(staffId, DateTime.UtcNow);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
        Assert.Equal(staffId, ticket.AssignedStaffUserId);

        // Personel yanıtı
        ticket.AddMessage(staffId, isStaff: true, "Şifreniz sıfırlandı. SMS ile iletildi.", null, DateTime.UtcNow);
        Assert.Equal(TicketStatus.Answered, ticket.Status);
        Assert.Equal(2, ticket.Messages.Count);

        // Çözümleme
        ticket.Resolve(staffId, "İşlem başarıyla tamamlandı.", DateTime.UtcNow);
        Assert.Equal(TicketStatus.Resolved, ticket.Status);
    }

    [Fact]
    public void SupportTicket_CannotReply_WhenTicketIsClosed()
    {
        var ticket = SupportTicket.Create(
            Guid.NewGuid(), SupportUnit.Library, "Kitap iadesi", TicketPriority.Low, "Kitap uzatma talebi", null, DateTime.UtcNow);

        ticket.Close(DateTime.UtcNow);
        Assert.Equal(TicketStatus.Closed, ticket.Status);

        var ex = Assert.Throws<DomainException>(() =>
            ticket.AddMessage(Guid.NewGuid(), false, "Tekrar açar mısınız?", null, DateTime.UtcNow));

        Assert.Equal("SupportTicket.CannotReplyToClosedTicket", ex.Code);
    }
}
