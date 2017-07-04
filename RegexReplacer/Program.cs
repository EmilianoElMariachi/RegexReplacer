using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using GlobstarFileSearch;
using NDesk.Options;
using RegexReplacer.Properties;
using RegexReplacer.Services;

namespace RegexReplacer
{
    public static class Program
    {
        private static readonly IRegexReplacerService _regexReplacerService;
        private static readonly FileSystem _fileSystem;

        static Program()
        {
            _fileSystem = new FileSystem();
            _regexReplacerService = new RegexReplacerService(new PatternFileSetService(_fileSystem), _fileSystem, new EncodingHelper());
        }


        public static int Main(string[] args)
        {
            string fileSearchPattern = null;
            string findRegex = null;
            string replace = null;
            var directory = _fileSystem.Directory.GetCurrentDirectory();
            var showHelp = false;

            var mode = Mode.REAL;

            var optionSet = new OptionSet
            {
                {
                    "d|directory=",
                    "The input directory where to look for files, if not specified current directory is used",
                    d => directory = d
                },
                {
                    "s|searchFilePattern=",
                    "File search pattern (ANT like patterns)",
                    s => fileSearchPattern = s
                },
                {
                    "f|find=",
                    "Regular expression to use for searching in files",
                    f => findRegex = f
                },
                {
                    "r|replace=",
                    "The replacement value for matches",
                    r => replace = r
                },
                {
                    "t|test",
                    "When set, no files will be modified",
                    t => mode = Mode.TEST
                },
                {
                    "h|help",
                    "Shows this message and exit",
                    h => showHelp = true
                },
            };


            try
            {
                var extraArgs = optionSet.Parse(args);
                if (extraArgs != null && extraArgs.Count > 0)
                {
                    var unknownArgs = string.Join(" ", extraArgs);
                    throw new ArgumentException(Resources.CommandLine_UnknownArgs, unknownArgs);
                }

                if (showHelp)
                {
                    ShowHelp(optionSet);
                }
                else
                {
                    if (fileSearchPattern == null)
                    {
                        throw new ArgumentException(Resources.CommandLine_FileSearchPatternNotSpecified);
                    }
                    if (findRegex == null)
                    {
                        throw new ArgumentException(Resources.CommandLine_FindRegexNotSpecified);
                    } 
                    if (replace == null)
                    {
                        throw new ArgumentException(Resources.CommandLine_ReplaceNotSpecified);
                    }
                    

                    _regexReplacerService.Replace(directory, fileSearchPattern, findRegex, replace, mode);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(Resources.RegexReplacer_Error, ex.Message);
                return 1;
            }
        }

        private static void ShowHelp(OptionSet optionSet)
        {
            var exeName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);

            var usage = exeName + " [-d|--directory=<path>] -s|--searchFilePattern=<pattern> -f|--find=<regex> [-t|--test] [-h|--help]";

            var stringWriter = new StringWriter();
            optionSet.WriteOptionDescriptions(stringWriter);

            Console.WriteLine(Resources.CommandLine_Help, usage, stringWriter);
        }
    }
}