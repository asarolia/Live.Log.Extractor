namespace Live.Log.Extractor.IndexerService.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    public static class ExtensionHelpers
    {
        /// <summary>
        /// To the custom string.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <returns></returns>
        public static string ToCustomString(this HashSet<string> set)
        {
            StringBuilder result = new StringBuilder(string.Empty);
            foreach (var item in set)
            {
                result.Append(item + " ");
            }
            return result.ToString();
        }
    }
}
