using System.Text.RegularExpressions;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace BackEnd;

public class OrderOperationsProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        Console.WriteLine("Ordering Swagger Paths...");
    
        // Custom sorting for Ecq APIs
        var orderedPaths = context.Document.Paths
            .OrderBy(p => p.Key, new EcqNumberComparer())
            .ToList();

        context.Document.Paths.Clear();

        foreach (var path in orderedPaths)
        {
            context.Document.Paths.Add(path.Key, path.Value);
        }
    
        if (context.Document.Tags != null)
        {
            context.Document.Tags = context.Document.Tags
                .OrderBy(t => t.Name, new EcqNumberComparer())
                .ToList();
        }
    }
}

/// </summary>
public class EcqNumberComparer : IComparer<string>
{
    private readonly Regex _ecqPattern = new Regex(@"Ecq(\d+)", RegexOptions.IgnoreCase);
    
    public int Compare(string x, string y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        // Check for Ecq pattern in both strings
        var matchX = _ecqPattern.Match(x);
        var matchY = _ecqPattern.Match(y);

        // If both strings contain the Ecq pattern, compare by the number
        if (matchX.Success && matchY.Success)
        {
            int numX = int.Parse(matchX.Groups[1].Value);
            int numY = int.Parse(matchY.Groups[1].Value);
            return numX.CompareTo(numY);
        }
        
        // If only one has the pattern, prioritize Ecq patterns
        if (matchX.Success) return -1;
        if (matchY.Success) return 1;

        // Otherwise use the regular alphanumeric comparison
        return CompareAlphaNumeric(x, y);
    }

    private int CompareAlphaNumeric(string x, string y)
    {
        int ix = 0, iy = 0;

        while (ix < x.Length && iy < y.Length)
        {
            if (char.IsDigit(x[ix]) && char.IsDigit(y[iy]))
            {
                // Extract numeric parts
                string numX = "", numY = "";
                while (ix < x.Length && char.IsDigit(x[ix]))
                    numX += x[ix++];
                
                while (iy < y.Length && char.IsDigit(y[iy]))
                    numY += y[iy++];

                // Compare numerically
                if (int.TryParse(numX, out int nx) && int.TryParse(numY, out int ny))
                {
                    int numCompare = nx.CompareTo(ny);
                    if (numCompare != 0) return numCompare;
                }
                else
                {
                    int strCompare = string.Compare(numX, numY, StringComparison.OrdinalIgnoreCase);
                    if (strCompare != 0) return strCompare;
                }
            }
            else
            {
                // Compare non-numeric characters
                int charCompare = char.ToLowerInvariant(x[ix]).CompareTo(char.ToLowerInvariant(y[iy]));
                if (charCompare != 0) return charCompare;
                ix++;
                iy++;
            }
        }

        return x.Length.CompareTo(y.Length);
    }
}