namespace RegexReplacer.Services
{
    public interface IRegexReplacerService
    {
        /// <summary>
        /// Replace in files
        /// </summary>
        /// <param name="directory">The root directory where to look for files</param>
        /// <param name="fileSearchPattern">The file seach pattern (support ant like globstar)</param>
        /// <param name="findRegex">The regular expression used for searching in matching files</param>
        /// <param name="replace">The replacement value</param>
        /// <param name="mode">The mode in which the replacement should operate</param>
        void Replace(string directory, string fileSearchPattern, string findRegex, string replace, Mode mode);
    }
}