namespace RegexReplacer.Services
{
    public enum Mode
    {
        /// <summary>
        /// Files might be modified
        /// </summary>
        REAL,
        /// <summary>
        /// In test mode, no file will be modified
        /// </summary>
        TEST,
    }
}