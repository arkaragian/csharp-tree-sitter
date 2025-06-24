using SampleUsage;
using System.Globalization;
using System.Runtime.InteropServices;
using TreeSitter.CSharp;

internal class Program {

    [DllImport("tree-sitter-cpp.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern nint tree_sitter_cpp();

    [DllImport("tree-sitter-latex.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern nint tree_sitter_latex();

    private static void Main(string[] args) {

        // Check if the args have at least two elements and the first one is "-files"
        if (args.Length < 2 || args[0] != "-files") {
            Console.WriteLine("Invalid arguments. Please use -files followed by one or more file paths.");
            return;
        }

        int a = 0;
        List<string>? files = ArgsToPaths(ref a, args);

        if (files is not null) {
            _ = PrintTree(files);
        }
    }

    public static bool ParseTree(string fileContents, TSLanguage lang) {
        using TSParser parser = new() {
            Language = lang
        };


        using TSTree? tree = parser.ParseString(oldTree: null, fileContents);
        if (tree is null) {
            Console.WriteLine("First Parsing Failed");
            return false;
        }

        SampleCPPTreeSitter.PostOrderTraverseTree(tree, lang, fileContents);
        return true;
    }

    public static bool PrintTree(List<string> paths) {
        bool good = true;
        foreach (string path in paths) {
            string filetext = File.ReadAllText(path);
            TSLanguage? lang = null;
            if (path[^3..] is "cpp") {
                lang = new(tree_sitter_cpp());
            }
            if (path[^3..] is "tex") {
                lang = new(tree_sitter_latex());
            }

            if (lang is null) {
                return false;
            }

            if (!ParseTree(filetext, lang)) {
                good = false;
            }
        }
        return good;
    }

    public static List<string> ArgsToPaths(ref int pos, string[] args) {
        if (++pos >= args.Length) {
            PrintErrorAt("", "XR0100: No input files to process.");
            return Enumerable.Empty<string>().ToList();
        }

        List<string> files = new();
        HashSet<string> used = new();
        EnumerationOptions options = new() {
            RecurseSubdirectories = false,
            ReturnSpecialDirectories = false,
            MatchCasing = MatchCasing.CaseInsensitive,
            MatchType = MatchType.Simple,
            IgnoreInaccessible = false
        };

        for (; pos < args.Length; pos++) {
            string arg = args[pos];

            if (arg[0] == '-') {
                pos--;
                break;
            }

            if (Directory.Exists(arg)) {
                PrintErrorAt(arg, "XR0101: Path is a directory, not a file spec.");
                return Enumerable.Empty<string>().ToList();
            }

            string? directory = Path.GetDirectoryName(arg);
            string pattern = Path.GetFileName(arg);
            if (directory == string.Empty) {
                directory = ".";
            }
            if (directory == null || string.IsNullOrEmpty(pattern)) {
                PrintErrorAt(arg, "XR0102: Path is anot a valid file spec.");
                return Enumerable.Empty<string>().ToList();
            }

            if (!Directory.Exists(directory)) {
                PrintErrorAt(directory, "XR0103: Couldn't find direvtory.");
                return Enumerable.Empty<string>().ToList();
            }

            int cnt = 0;
            foreach (string filepath in Directory.EnumerateFiles(directory, pattern, options)) {
                cnt++;
                if (!used.Contains(filepath)) {
                    files.Add(filepath);
                    _ = used.Add(filepath);
                }
            }
        }

        return files;
    }

    public static void PrintErrorAt(string path, string error, params object[] args) {
        Console.ForegroundColor = ConsoleColor.Red;
        if (Console.CursorLeft != 0) {
            Console.Error.WriteLine();
        }

        Console.Error.WriteLine("{0}(): error {1}", path, string.Format(CultureInfo.InvariantCulture, error, args));
        Console.ForegroundColor = Console.ForegroundColor;
    }
}