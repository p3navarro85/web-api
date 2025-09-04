
using Application.Abstractions.Messaging;
using Application.Todos.Create;
using Application.Todos.Enrich;
using Domain.Todos;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Enrich : IEndpoint
{
    public sealed class EnrichRequest
    {
        public string TextToEnrich { get; set; } = string.Empty;
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("todos/enrich", async (
            EnrichRequest request,
            ICommandHandler<EnrichTodoCommand, TodoItem> enrichHandler,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.TextToEnrich))
            {
                return Results.BadRequest(new { Error = "TextToEnrich cannot be empty." });
            }
            var command = new EnrichTodoCommand(request.TextToEnrich);

            Result<TodoItem> result = await enrichHandler.Handle(command, cancellationToken);

            return result.Match(
                success => Results.Ok(result),
                failure => CustomResults.Problem(failure)
            );
        })
        .WithDescription("Enriches a todo item with ai generated data based on the provided text.")
        .WithTags(Tags.Todos)
        .RequireAuthorization();
    }
}
