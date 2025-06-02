using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace BackEnd;

public class OrderOperationsProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        Console.WriteLine("Ordering Swagger Paths...");
        var orderedPaths = context.Document.Paths
            .OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        context.Document.Paths.Clear();

        foreach (var path in orderedPaths)
        {
            context.Document.Paths.Add(path.Key, path.Value);
        }
        
        if (context.Document.Tags != null)
        {
            context.Document.Tags = context.Document.Tags
                .OrderBy(t => t.Name)
                .ToList();
        }
    }
}