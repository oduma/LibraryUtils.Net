using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Sciendo.Junk.Detect.Library;

// Build configuration
var configuration = new ConfigurationBuilder()
    .AddCommandLine(args)
    .Build();

// If no arguments provided, show usage instructions
if (args.Length == 0)
{
    Console.WriteLine("Junk Detector - Analyzes directories for file extensions");
    Console.WriteLine("\nUsage:");
    Console.WriteLine("  --path <directory>    Specify the directory to analyze (optional, defaults to current directory)");
    Console.WriteLine("  --outfile <filepath>  Write results to specified file (optional)");
    Console.WriteLine("  --keep <filepath>     Path to file containing extensions to detect (one per line)");
    Console.WriteLine("\nExamples:");
    Console.WriteLine("  Analyze current directory:");
    Console.WriteLine("    Sciendo.Junk.Detect");
    Console.WriteLine("  Analyze specific directory:");
    Console.WriteLine("    Sciendo.Junk.Detect --path \"C:\\MyFolder\"");
    Console.WriteLine("  Write results to file:");
    Console.WriteLine("    Sciendo.Junk.Detect --path \"C:\\MyFolder\" --outfile \"results.txt\"");
    Console.WriteLine("  Detect specific extensions:");
    Console.WriteLine("    Sciendo.Junk.Detect --path \"C:\\MyFolder\" --keep \"extensions.txt\"");
    return;
}

// Setup services
var services = new ServiceCollection();
services.AddTransient<IJunkDetector, JunkDetector>();
services.AddSingleton<IConfiguration>(configuration);

var serviceProvider = services.BuildServiceProvider();

var junkDetector = serviceProvider.GetRequiredService<IJunkDetector>();

// Get command line args
var path = configuration["path"] ?? Directory.GetCurrentDirectory();
var outfile = configuration["outfile"];
var keepFile = configuration["keep"];

IEnumerable<string> results;

Console.WriteLine($"Analyzing directory: {path}");
Console.WriteLine($"Output file: {outfile}");
Console.WriteLine($"Keep file: {keepFile}");
if (!string.IsNullOrEmpty(keepFile) && File.Exists(keepFile))
{
    // Read extensions from the keep file
    var extensions = File.ReadAllLines(keepFile);
    // Use Detect method to find files with specific extensions
    results = junkDetector.Detect(path, extensions)
                         .Select(file => $"del \"{file}\"");
}
else
{
    Console.WriteLine("No keep file specified or file does not exist. Getting all extensions.");
    // Get all extensions
    results = junkDetector.GetExtensions(path);
}

// Display results to console
foreach (var result in results)
{
    Console.WriteLine(result);
}

// If outfile is specified, write results to file
if (!string.IsNullOrEmpty(outfile))
{
    File.WriteAllLines(outfile, results);
}
