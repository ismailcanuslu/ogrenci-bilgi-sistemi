using BuildingBlocks.Api.Errors;
using BuildingBlocks.Api.Versioning;
using CampusLife.Application.Features.Clubs;
using CampusLife.Application.Features.Reservations;
using CampusLife.Domain.Clubs;
using CampusLife.Domain.Reservations;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CampusLife.Api.Endpoints;

public sealed record CreateReservationRequest(
    FacilityType FacilityType,
    string FacilityName,
    DateOnly ReservationDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string? Notes);

public sealed record ReviewReservationRequest(bool IsApproved, string? Notes);

public sealed record CreateClubRequest(
    string Name,
    string Code,
    string? Category,
    string? Description,
    Guid? AdvisorUserId,
    Guid? PresidentStudentId);

public sealed record CreateEventRequest(
    string Title,
    string? Description,
    DateTime EventDateUtc,
    string Location,
    int Capacity);

public sealed record ReviewEventRequest(bool IsApproved, string? Notes);

public static class CampusLifeEndpoints
{
    public static IEndpointRouteBuilder MapCampusLifeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("campus-life", 1, 0)
            .WithTags("CampusLife")
            .RequireAuthorization();

        MapReservations(group);
        MapClubs(group);

        return app;
    }

    private static void MapReservations(RouteGroupBuilder group)
    {
        var reservations = group.MapGroup("/reservations");

        reservations.MapPost("/", async (
            CreateReservationRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateFacilityReservationCommand(
                body.FacilityType,
                body.FacilityName,
                body.ReservationDate,
                body.StartTime,
                body.EndTime,
                body.Notes), ct)).ToHttpResult(http))
            .WithName("CreateReservation")
            .WithSummary("Spor salonu, halı saha, psikolojik danışmanlık veya sosyal market rezervasyonu oluşturur");

        reservations.MapGet("/", async (
            [AsParameters] ListFacilityReservationsQuery query,
            ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(query, ct)).ToHttpResult(http))
            .WithName("ListReservations")
            .WithSummary("Rezervasyonları listeler (kendi rezervasyonları veya yetkili inceleme filtresi)");

        reservations.MapPut("/{id:guid}/review", async (
            Guid id, ReviewReservationRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ReviewFacilityReservationCommand(id, body.IsApproved, body.Notes), ct)).ToHttpResult(http))
            .WithName("ReviewReservation")
            .WithSummary("Rezervasyon başvurusunu onaylar veya gerekçeli reddeder (campus-life.reservation.manage)");
    }

    private static void MapClubs(RouteGroupBuilder group)
    {
        var clubs = group.MapGroup("/clubs");

        clubs.MapPost("/", async (
            CreateClubRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateStudentClubCommand(
                body.Name,
                body.Code,
                body.Category,
                body.Description,
                body.AdvisorUserId,
                body.PresidentStudentId), ct)).ToHttpResult(http))
            .WithName("CreateStudentClub")
            .WithSummary("Öğrenci topluluğu / kulüp oluşturur (campus-life.club.manage)");

        clubs.MapGet("/", async (
            bool? activeOnly, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ListStudentClubsQuery(activeOnly), ct)).ToHttpResult(http))
            .WithName("ListStudentClubs")
            .WithSummary("Öğrenci topluluklarını listeler");

        clubs.MapPost("/{id:guid}/events", async (
            Guid id, CreateEventRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new CreateClubEventCommand(
                id,
                body.Title,
                body.Description,
                body.EventDateUtc,
                body.Location,
                body.Capacity), ct)).ToHttpResult(http))
            .WithName("CreateClubEvent")
            .WithSummary("Topluluk adına etkinlik başvuru talebi oluşturur (campus-life.event.manage)");

        group.MapGet("/events", async (
            Guid? clubId, EventStatus? status, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ListClubEventsQuery(clubId, status), ct)).ToHttpResult(http))
            .WithName("ListClubEvents")
            .WithSummary("Öğrenci etkinliklerini listeler");

        group.MapPut("/events/{id:guid}/review", async (
            Guid id, ReviewEventRequest body, ISender mediator, HttpContext http, CancellationToken ct)
            => (await mediator.Send(new ReviewClubEventCommand(id, body.IsApproved, body.Notes), ct)).ToHttpResult(http))
            .WithName("ReviewClubEvent")
            .WithSummary("Etkinlik başvurusunu onaylar veya reddeder (campus-life.event.manage)");
    }
}
