using BuildingBlocks.Application.Abstractions.Messaging;
using BuildingBlocks.Application.Results;
using FluentValidation;
using Translations.Application.Abstractions;
using Translations.Domain;

namespace Translations.Application.Features;

public static class GetTranslation
{
    public sealed record Query(
        string Code,
        string? Lang = null)
        : IQuery<ServiceResult<Response>>;

    public sealed record Response(
        string Code,
        string Language,
        string TranslatedText);

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(100);
        }
    }

    public sealed class Handler(ITranslationService translationService)
        : IQueryHandler<Query, ServiceResult<Response>>
    {
        public Task<ServiceResult<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var targetLang = LanguageCode.Normalize(request.Lang);
            var translated = translationService.Translate(request.Code, targetLang);

            var response = new Response(
                request.Code,
                targetLang,
                translated);

            return Task.FromResult(ServiceResult<Response>.Ok(response));
        }
    }
}
