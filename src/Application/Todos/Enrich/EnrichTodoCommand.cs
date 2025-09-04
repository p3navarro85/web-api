using Application.Abstractions.Messaging;
using Domain.Todos;

namespace Application.Todos.Enrich;

public sealed record EnrichTodoCommand(string TextToEnrich) : ICommand<TodoItem>;
