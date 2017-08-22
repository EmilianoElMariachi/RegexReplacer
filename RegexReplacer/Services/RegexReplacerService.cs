using System;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using GlobstarFileSearch;
using log4net;
using RegexReplacer.Properties;

namespace RegexReplacer.Services
{
    public class RegexReplacerService : IRegexReplacerService
    {
        private readonly IPatternFileSetService _patternFileSetService;
        private readonly IFileSystem _fileSystem;
        private readonly IEncodingHelper _encodingHelper;
        private readonly ILog _logger;

        public RegexReplacerService(IPatternFileSetService patternFileSetService, IFileSystem fileSystem, IEncodingHelper encodingHelper)
        {
            _patternFileSetService = patternFileSetService;
            _fileSystem = fileSystem;
            _encodingHelper = encodingHelper;
            _logger = LogManager.GetLogger(this.GetType());
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

            _logger.Info(string.Format(Resources.RegexReplacer_Summary, directory, fileSearchPattern, findRegex, replace, mode));

            string relSrcDir;
            string[] relFilePaths;
            _patternFileSetService.Execute(directory, fileSearchPattern, out relSrcDir, out relFilePaths);

            var nbFilesWhereExpressionFound = 0;

            foreach (var relFilePath in relFilePaths)
            {
                var filePath = _fileSystem.Path.Combine(directory, relSrcDir, relFilePath);

                var fileEncoding = _encodingHelper.GetEncoding(filePath);

                var initialContent = _fileSystem.File.ReadAllText(filePath, fileEncoding);

                var newContent = regex.Replace(initialContent, replace);

                if (initialContent != newContent)
                {
                    nbFilesWhereExpressionFound++;
                    switch (mode)
                    {
                        case Mode.FILE:
                            _fileSystem.File.WriteAllText(filePath, newContent, fileEncoding);
                            _logger.Info(string.Format(Resources.RegexReplacer_FileModified, filePath));
                            break;
                        case Mode.DISPLAY:
                            _logger.Info(string.Format(Resources.RegexReplacer_ExpressionFoundInFile, filePath));
                            Console.WriteLine(newContent);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("mode", mode, null);
                    }
                }
                else
                {
                    _logger.Info(string.Format(Resources.RegexReplacer_ExpressionNotFoundInFile, filePath));
                }
            }

            _logger.Info(string.Format(Resources.RegexReplacer_NbFoundFiles, relFilePaths.Length));
            _logger.Info(string.Format(Resources.RegexReplacer_NbFilesWhereExpressionFound, nbFilesWhereExpressionFound));
        }
    }
}