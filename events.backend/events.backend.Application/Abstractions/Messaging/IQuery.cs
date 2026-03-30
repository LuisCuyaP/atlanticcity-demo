using events.backend.CrossCutting;
using MediatR;

namespace events.backend.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;