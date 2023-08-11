using System.Diagnostics;
using System.Xml.Linq;

namespace OrderProcessing
{
    /// <summary>
    /// The reader.
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Reads a property value async.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>A Task containing a string.<see cref="Task{string}"/></returns>
        public static async Task<string> ReadPropertyValueAsync(XElement element)
        {
            if (element != null)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                string value = element.Value.Trim();

                stopwatch.Stop();
                Console.WriteLine($"Response time for '{element.Name.LocalName}': {stopwatch.ElapsedMilliseconds} ms");

                return await Task.FromResult(value);
            }
            return null;
        }
    }
}