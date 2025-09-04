using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Todos;
using Domain.Users;
using Application.AIServices;
using SharedKernel;

namespace Application.Todos.Enrich;
internal sealed class EnrichTodoCommandHandler(
    IUserContext userContext,
    ITodoGenerator todoGenerator
    )
    : ICommandHandler<EnrichTodoCommand, TodoItem>
{
    public async Task<Result<TodoItem>> Handle(EnrichTodoCommand command, CancellationToken cancellationToken)
    {
        if (userContext.UserId == Guid.Empty)
        {
            return Result.Failure<TodoItem>(UserErrors.Unauthorized());
        }

        return await todoGenerator.GenerateAsync(command.TextToEnrich, cancellationToken);
    }
}
