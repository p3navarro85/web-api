using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;
using Domain.Todos;
using Infrastructure.Authentication;
using OllamaSharp;
using OllamaSharp.Models;
using SharedKernel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.AIServices;
internal sealed class TodoOllamaGenerator(
    IUserContext userContext,
    IOllamaApiClient ollamaApiClient) : ITodoGenerator
{
    public async Task<Result<TodoItem>> GenerateAsync(string textToEnrich, CancellationToken cancellationToken)
    {
        string prompt =
           $@"""
            Analyze the following task: '{textToEnrich}'; 
            and summarised task in no more than 15 words as the description, 
            due date (if any) taking in count that today is: {DateTime.UtcNow.Date}, 
            priority as an integer from 0 to 4 (Normal = 0,  Low = 1, Medium = 2, High = 3, Top = 4),
            and an array of relevant categories (e.g., 'Work', 'Personal', 'Shopping', 'Health') named as labels and
            add always the categories: 'Enriched' and 'AI-Generated' at the end. 
            Output the result in a JSON format with the following structure:
            {{
                'Description': 'string',
                'DueDate': 'YYYY-MM-DD or null',
                'Priority': 'integer',
                'Labels': '[string]'
            }}
            Respond only with the JSON, without additional text or explanation.
            """;

        var chatResponse = new StringBuilder();
        await foreach (GenerateResponseStream stream in ollamaApiClient.GenerateAsync(prompt, null, cancellationToken))
        {
            Console.Write(stream?.Response);
            chatResponse.Append(stream?.Response);
        }

        if (chatResponse.Length > 0)
        {
            string itemJsonString = chatResponse.ToString();
            TodoItem todoItem = JsonSerializer.Deserialize<TodoItem>(itemJsonString) ?? new TodoItem();
            todoItem.UserId = userContext.UserId;
            return Result.Success(todoItem);
        }
        else
        {
            return Result.Failure<TodoItem>(TodoItemErrors.CannotGenerate());
        }

    }
}
