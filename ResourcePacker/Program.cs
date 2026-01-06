namespace ResourcePacker;

using System;
using System.IO;
using System.IO.Compression;

class Program
{
    
    static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0 || HasFlag(args, "-h", "--help"))
            {
                PrintUsage();
                return 0;
            }

            var source = GetOption(args, "-s", "--source");
            if (string.IsNullOrWhiteSpace(source))
            {
                Console.Error.WriteLine("Error: source folder is required. Use -s or --source to specify it.");
                return 2;
            }

            var output = GetOption(args, "-o", "--output");
            var includeRoot = HasFlag(args, "--include-root");
            var force = HasFlag(args, "--force");

            // Resolve source relative to current working directory if not absolute.
            var sourcePath = Path.GetFullPath(source);
            if (!Directory.Exists(sourcePath))
            {
                Console.Error.WriteLine($"Error: source folder not found: {sourcePath}");
                return 3;
            }

            // Determine default output name if not provided.
            if (string.IsNullOrWhiteSpace(output))
            {
                var folderName = new DirectoryInfo(sourcePath).Name;
                output = folderName + ".zip";
            }

            // If provided output is a directory, place zip inside it using default name.
            var outputPath = output;
            if (Directory.Exists(outputPath))
            { 
                var folderName = new DirectoryInfo(sourcePath).Name;
                outputPath = Path.Combine(outputPath, folderName + ".zip");
            }
            else
            {
                // If output does not contain a directory portion, make it relative to current directory.
                var outputDir = Path.GetDirectoryName(outputPath);
                if (string.IsNullOrEmpty(outputDir))
                {
                    Console.Error.WriteLine("Error: output folder path is required.");
                }
                outputPath = Path.GetFullPath(outputPath);
            }
            
            // Ensure file is true zip format, outputPath should use correct file prefix.
            if (!outputPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                outputPath = outputPath + ".zip";
            }

            if (File.Exists(outputPath) && !force)
            {
                Console.Write($"File {outputPath} already exists. Overwrite? (y/N): ");
                var key = Console.ReadKey(intercept: true);
                Console.WriteLine();
                if (key.KeyChar != 'y' && key.KeyChar != 'Y')
                {
                    Console.WriteLine("Aborted by user.");
                    return 4;
                }
            }

            // Create containing directory for the output if needed.
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            // If includeRoot is true, create zip from parent with entryName root or use temp directory approach.
            if (includeRoot)
            {
                // Create zip and include the directory as a top-level entry by zipping the parent and using the folder
                // name as entry root.
                var tempRoot = Path.Combine(Path.GetTempPath(), "packfolder_" + Guid.NewGuid().ToString("N"));
                var tempTarget = Path.Combine(tempRoot, new DirectoryInfo(sourcePath).Name);
                try
                {
                    Directory.CreateDirectory(tempTarget);
                    CopyDirectory(sourcePath, tempTarget);
                    // For default settings, the output file can override original file.
                    if (File.Exists(outputPath)) File.Delete(outputPath);
                    ZipFile.CreateFromDirectory(tempRoot, outputPath, 
                        CompressionLevel.Optimal, includeBaseDirectory: false);
                }
                finally
                {
                    try
                    {
                        Directory.Delete(tempRoot, true);
                    }
                    catch
                    {
                        // Ignored all exceptions which thrown by this branch, it is unnecessary.
                    }
                }
            }
            else
            {
                // Zip only the contents of the directory (no top-level folder) and overload it into output.
                if (File.Exists(outputPath)) File.Delete(outputPath);

                // Use temporary folder trick if it only included content.
                ZipFile.CreateFromDirectory(sourcePath, outputPath, 
                    CompressionLevel.Optimal, includeBaseDirectory: false);
            }

            Console.WriteLine($"Created zip: {outputPath}");
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Error: " + e.Message);
            return 1;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Resource Packer");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run -- -s <source-folder> [-o <output-zip>] [--include-root] [--force]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -s, --source        Path to the folder to pack. Can be relative to the repo (e.g. RepoName/TargetFolder).");
        Console.WriteLine("  -o, --output        Output zip file path or filename. If omitted uses <TargetFolder>.zip in current directory.");
        Console.WriteLine("      --include-root  Include the source folder itself as the top-level entry inside the zip (default: false).");
        Console.WriteLine("      --force         Overwrite existing output zip without prompting.");
        Console.WriteLine("  -h, --help          Show this help.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run -- -s RepoName/TargetFolder");
        Console.WriteLine("  dotnet run -- -s RepoName/TargetFolder -o myoutput.zip");
        Console.WriteLine("  dotnet run -- -s RepoName/TargetFolder -o ../artifacts/Target.zip --include-root --force");
    }

    static string? GetOption(string[] args, string shortOpt, string longOpt)
    {
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].Equals(shortOpt, StringComparison.OrdinalIgnoreCase) 
                || args[i].Equals(longOpt, StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 < args.Length) 
                    return args[i + 1];
                return null;
            }

            // Support "--option=value" or "-o=value" option.
            if (args[i].StartsWith(shortOpt + "=", StringComparison.OrdinalIgnoreCase) 
                || args[i].StartsWith(longOpt + "=", StringComparison.OrdinalIgnoreCase))
            {
                var idx = args[i].IndexOf('=');
                return args[i].Substring(idx + 1);
            }
        }

        return null;
    }

    static bool HasFlag(string[] args, params string[] flags)
    {
        foreach (var a in args)
        {
            foreach (var f in flags)
            {
                if (a.Equals(f, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        return false;
    }

    static void CopyDirectory(string sourceDir, string destinationDir)
    {
        Directory.CreateDirectory(destinationDir);
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var dest = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, dest, overwrite: true);
        }
        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var dest = Path.Combine(destinationDir, Path.GetFileName(dir));
            CopyDirectory(dir, dest);
        }
    }
    
}