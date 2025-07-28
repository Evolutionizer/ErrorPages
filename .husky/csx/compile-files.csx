#r "System.Text.Json"
#r "System.Linq"

using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

record ErrorPage(string Title, string Subtitle, string[] Content);

try
{
    var baseTagPattern = new Regex("""<base\s+href="\.\/resources\/"\s*\/>""");
    var templateFileContent = File.ReadAllText("template.html");
    templateFileContent= baseTagPattern.Replace(templateFileContent, """<base href="https://raw.githubusercontent.com/Evolutionizer/ErrorPages/refs/heads/main/resources/" />""");
    var errorPageDefinitions = JsonSerializer.Deserialize<List<ErrorPage>>(File.ReadAllText("error-pages.json"), new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

    foreach (var errorPage in errorPageDefinitions)
    {
        if (string.IsNullOrEmpty(errorPage.Title))
            throw new InvalidOperationException("Title cannot be null or empty");

        Console.WriteLine("Processing error page " + errorPage.Title);

        if (string.IsNullOrEmpty(errorPage.Subtitle))
            throw new InvalidOperationException("Subtitle cannot be null or empty");

        if (errorPage.Content is null || errorPage.Content.Length == 0)
            throw new InvalidOperationException("Content cannot be empty");

        var content = errorPage.Content.Select(c => c.Replace("{Support}", "<a href=\"mailto:support@evolutionizer.com\">support</a>", StringComparison.OrdinalIgnoreCase)).Select(x => $"<p>{x}</p>");
        var combinedContent = string.Join(" ", content);
        var errorPageContent = templateFileContent
            .Replace("{Title}", errorPage.Title)
            .Replace("{Subtitle}", errorPage.Subtitle)
            .Replace("{Content}", combinedContent);

        var newFilePath = Path.Combine("compiled", $"{errorPage.Title.Replace(" ", "-")}.html");
        Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
        File.WriteAllText(newFilePath, errorPageContent);
        GitAdd(newFilePath);

        Console.WriteLine($"Compiled {errorPage.Title} error page to {newFilePath}");
        Console.WriteLine();
    }

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine("Error during compilation: " + ex);
}

return 1;

public static void GitAdd(string path)
{
    var processInfo = new ProcessStartInfo("git", $"add \"{path}\"")
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (var process = new Process { StartInfo = processInfo })
    {
        process.Start();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Git add failed: {error}");
        }
    }
}