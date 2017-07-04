using System.Text;

namespace RegexReplacer
{
    public interface IEncodingHelper
    {

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        Encoding GetEncoding(string filename);
    }
}