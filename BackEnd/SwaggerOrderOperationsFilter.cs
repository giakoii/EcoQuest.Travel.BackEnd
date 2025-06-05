using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BackEnd;

public class SwaggerOrderOperationsFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Sort paths by their keys (URLs)
        var paths = swaggerDoc.Paths.OrderBy(p => p.Key).ToList();
        swaggerDoc.Paths.Clear();

        foreach (var path in paths)
        {
            swaggerDoc.Paths.Add(path.Key, path.Value);
        }
    }
}