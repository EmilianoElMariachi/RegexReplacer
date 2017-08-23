namespace RegexReplacer.Services
{
    public enum Mode
    {
        /// <summary>
        /// Rewrites each file for which the expression could be found and replaced
        /// </summary>
        FILE,
        /// <summary>
        /// Displays in the console the replacement result for each file where the expression could be found and replaced
        /// </summary>
        DISPLAY,
    }
}