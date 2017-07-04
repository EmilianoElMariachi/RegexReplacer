using System;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using GlobstarFileSearch;
using RegexReplacer.Properties;

namespace RegexReplacer.Services
{
    public class RegexReplacerService : IRegexReplacerService
    {
        private readonly IPatternFileSetService _patternFileSetService;
        private readonly IFileSystem _fileSystem;
        private readonly IEncodingHelper _encodingHelper;

        public RegexReplacerService(IPatternFileSetService patternFileSetService, IFileSystem fileSystem, IEncodingHelper encodingHelper)
        {
            _patternFileSetService = patternFileSetService;
            _fileSystem = fileSystem;
            _encodingHelper = encodingHelper;
        }


        public void Replace(string directory, string fileSearchPattern, string findRegex, string replace, Mode mode)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentException(Resources.RegexReplacer_DirectoryUndefined);
            }

            if (string.IsNullOrEmpty(fileSearchPattern))
            {
                throw new ArgumentException(Resources.RegexReplacer_RegexUndefined);
            }

            if (replace == null)
            {
                throw new ArgumentException(string.Format(Resources.RegexReplacer_ReplaceNull));
            }

            Regex regex = null;
            try
            {
                regex = new Regex(findRegex);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format(Resources.RegexReplacer_RegexInvalid, ex.Message));
            }

            Console.WriteLine(Resources.RegexReplacer_Summary, directory, fileSearchPattern, findRegex, replace, mode);

            string relSrcDir;
            string[] relFilePaths;
            _patternFileSetService.Execute(directory, fileSearchPattern, out relSrcDir, out relFilePaths);

            var nbModFiles = 0;

            foreach (var relFilePath in relFilePaths)
            {
                var filePath = _fileSystem.Path.Combine(directory, relSrcDir, relFilePath);

                var fileEncoding = _encodingHelper.GetEncoding(filePath);

                var initialContent = _fileSystem.File.ReadAllText(filePath, fileEncoding);

                var newContent = regex.Replace(initialContent, replace);

                if (initialContent != newContent)
                {
                    nbModFiles++;
                    if (mode != Mode.TEST)
                    {
                        _fileSystem.File.WriteAllText(filePath, newContent, fileEncoding);
                    }

                    Console.WriteLine(filePath);
                }
            }

            Console.WriteLine(Resources.RegexReplacer_NbFoundFiles, relFilePaths.Length);
            Console.WriteLine(Resources.RegexReplacer_NbModifiedFiles, nbModFiles);
        }
    }
}