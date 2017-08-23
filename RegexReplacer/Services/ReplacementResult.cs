namespace RegexReplacer.Services
{
    public class ReplacementResult
    {
        public ReplacementResult(int nbFilesFound, int nbFilesWhereRegexFound)
        {
            NbFilesFound = nbFilesFound;
            NbFilesWhereRegexFound = nbFilesWhereRegexFound;
        }

        public int NbFilesFound { get; private set; }

        public int NbFilesWhereRegexFound{ get; private set; }
    }
}