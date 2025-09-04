using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Todos;
using SharedKernel;

namespace Application.AIServices;
public interface ITodoGenerator
{
    /// <summary>
    /// Generates a TodoItem based on the provided text using AI enrichment.
    /// </summary>
    /// <param name="textToEnrich"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<TodoItem>> GenerateAsync(string textToEnrich, CancellationToken cancellationToken);
}
