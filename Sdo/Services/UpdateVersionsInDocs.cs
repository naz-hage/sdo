using System.Text.Json;
using System.Text.RegularExpressions;

namespace Sdo.Services;

public sealed class UpdateVersionsInDocs
{
    private static readonly Dictionary<string, string[]> ToolNameMappings = new()
    {
        { "Azure CLI", ["AzureCLI", "Azure CLI"] },
        { "kubernetes", ["kubectl", "kubernetes"] },
        { "MongoDB Community Server", ["MongoDB", "MongoDB Community Server"] }
    };

    public void Update(string devSetupPath, string docsPath)
    {
        var appsYamlPath = Path.GetFullPath(Path.Combine(devSetupPath, "apps.yaml"));
        if (!File.Exists(appsYamlPath))
        {
            throw new FileNotFoundException($"apps.yaml not found at: {appsYamlPath}", appsYamlPath);
        }

        var versionMap = ReadVersions(appsYamlPath);
        UpdateMarkdownFile(docsPath, versionMap);
    }

    private static Dictionary<string, string> ReadVersions(string appsYamlPath)
    {
        var content = File.ReadAllText(appsYamlPath);
        var versionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (content.TrimStart().StartsWith("{", StringComparison.Ordinal))
        {
            using var jsonDocument = JsonDocument.Parse(content);
            if (jsonDocument.RootElement.TryGetProperty("NbuildAppList", out var appList) &&
                appList.ValueKind == JsonValueKind.Array)
            {
                foreach (var app in appList.EnumerateArray())
                {
                    if (app.TryGetProperty("Name", out var nameElement) &&
                        app.TryGetProperty("Version", out var versionElement) &&
                        nameElement.GetString() is { } name &&
                        versionElement.GetString() is { } version)
                    {
                        versionMap[name] = version;
                    }
                }
            }

            return versionMap;
        }

        var appMatches = Regex.Matches(
            content,
            @"(?ms)^\s*-\s+Name:\s*[""']?(?<name>[^""'\r\n]+)[""']?\s*\r?\n\s+Version:\s*[""']?(?<version>[^""'\r\n]+)[""']?");

        foreach (Match app in appMatches)
        {
            versionMap[app.Groups["name"].Value.Trim()] = app.Groups["version"].Value.Trim();
        }

        return versionMap;
    }

    private static void UpdateMarkdownFile(string docsPath, Dictionary<string, string> versionMap)
    {
        var lines = File.ReadAllLines(docsPath);
        var today = DateTime.Now.ToString("dd-MMM-yy");

        for (var i = 0; i < lines.Length; i++)
        {
            var match = Regex.Match(lines[i], @"^\| \[([^\]]+)\]");
            if (!match.Success)
            {
                continue;
            }

            var toolName = match.Groups[1].Value;
            var matchingVersion = versionMap
                .FirstOrDefault(entry => IsToolMatch(toolName, entry.Key))
                .Value;
            if (string.IsNullOrEmpty(matchingVersion))
            {
                continue;
            }

            var tableMatch = Regex.Match(
                lines[i],
                @"(\| \[[^\]]+\]\([^)]+\)\s+\| )([^|]+)(\| )([^|]+)(\|.*)");
            if (!tableMatch.Success)
            {
                continue;
            }

            lines[i] = tableMatch.Groups[1].Value +
                matchingVersion.PadRight(11) +
                tableMatch.Groups[3].Value +
                today.PadRight(15) +
                tableMatch.Groups[5].Value;
        }

        File.WriteAllLines(docsPath, lines);
    }

    private static bool IsToolMatch(string toolName, string manifestName)
    {
        if (toolName.Equals(manifestName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return ToolNameMappings.TryGetValue(toolName, out var aliases) &&
            aliases.Any(alias => alias.Equals(manifestName, StringComparison.OrdinalIgnoreCase));
    }
}
