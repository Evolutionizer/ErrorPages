using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

try
{
    var pattern = new Regex("""<base\s+href="\/"\s*\/>""");

    foreach (string filePath in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.html"))
    {
        Console.WriteLine("Preprocessing " + filePath);
        var content = File.ReadAllText(filePath);
        content = pattern.Replace(content, """<base href="https://raw.githubusercontent.com/Evolutionizer/ErrorPages/refs/heads/main/" />""");

        var newFilePath = Path.Combine(Path.GetDirectoryName(filePath), "compiled", Path.GetFileName(filePath));
        Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
        File.WriteAllText(newFilePath, content);
        GitAdd(newFilePath);
    }

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine("Error executing hook: " + ex);
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