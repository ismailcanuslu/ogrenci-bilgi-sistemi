using MediatR;

namespace BuildingBlocks.Application.Abstractions.Messaging;


public interface ICommand<out TResponse> : IRequest<TResponse>
{
}