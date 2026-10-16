using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using Translations.Application.Abstractions;
using Translations.Domain;

namespace Translations.Application.Features;

public static class GetTranslationsCatalog
{
    public sealed record Query(string? Lang = null) : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        string Language,
        IReadOnlyDictionary<string, string> Translations);

    public sealed class Handler(ITranslationService translationService)
        : IQueryHandler<Query, ServiceResult<Response>>
    {
        public Task<ServiceResult<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var targetLang = LanguageCode.Normalize(request.Lang);
            var catalog = translationService.GetAll(targetLang);

            var response = new Response(
                targetLang,
                catalog);

            return Task.FromResult(ServiceResult<Response>.Ok(response));
        }
    }
}
