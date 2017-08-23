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
            var mode = Mode.FILE;
            var noMatchIsError = true;
            var showHelp = false;

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
                    "m|mode=",
                    "Specifies the mode to use:\n-" + Mode.FILE + ": replace in files\n-" + Mode.DISPLAY + ": display the replacement results in the console",
                    t => mode = (Mode) Enum.Parse(typeof(Mode), t)
                },
                {
                    "n|noMatchNoError",
                    "Flag: if set the return code will be 0 even if no match found. By default return code is '0' if at least one match found and '1' if no match found.",
                    h => noMatchIsError = false
                },
                {
                    "h|help",
                    "Shows this message and exit",
                    h => showHelp = true
                }
            };

            try
            {
                var extraArgs = optionSet.Parse(args);
                if (extraArgs != null && extraArgs.Count > 0)
                {
                    var unknownArgs = string.Join(", ", extraArgs);
                    var exMessage = string.Format(Resources.CommandLine_UnknownArgs, unknownArgs);
                    throw new ArgumentException(exMessage);
                }

                if (showHelp)
                {
                    ShowHelp(optionSet);
                    return 0;
                }
                
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

                var replacementResult = _regexReplacerService.Replace(directory, fileSearchPattern, findRegex, replace, mode);

                if (noMatchIsError && replacementResult.NbFilesWhereRegexFound <= 0)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(Resources.RegexReplacer_Error, ex.Message);
                return 2;
            }
        }

        private static void ShowHelp(OptionSet optionSet)
        {
            var exeName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);

            var usage = exeName + " [-d|--directory=<path>] -s|--searchFilePattern=<pattern> -f|--find=<regex> -r|--replace=<replacement> [-m|--mode=<mode>] [-n|--noMatchNoError] [-h|--help]";

            var stringWriter = new StringWriter();
            optionSet.WriteOptionDescriptions(stringWriter);

            Console.WriteLine(Resources.CommandLine_Help, usage, stringWriter);
        }
    }
}