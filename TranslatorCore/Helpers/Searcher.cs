using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Translator.Core.Data;

namespace Translator.Core.Helpers
{
    internal static class Searcher
    {
        public delegate bool SearchImplementation(ReadOnlySpan<char> query, LineData line, StringComparison comparison);
        public static bool Search(ReadOnlySpan<char> query, FileData data, out List<int>? results, out ReadOnlySpan<char> cleanedQuery)
        {
            results = null;
            cleanedQuery = ReadOnlySpan<char>.Empty;
            if (query.IsEmpty) return false;
            if (data.Count == 0) return false;

            results = new();
            var enumerator = data.Values.GetEnumerator();
            int x = 0;
            StringComparison searchCulture = StringComparison.CurrentCultureIgnoreCase;
            List<SearchImplementation> algorithms = new();

            //escaper
            query = CheckAndClearEscapedChars(query);

            //case sensitive search
            if (query[0] == '!' && query.Length > 1) // we set the case sensitive flag
            {
                query = query[1..];
                searchCulture = StringComparison.CurrentCulture;
            }

            //2nd escaper
            query = CheckAndClearEscapedChars(query);

            //we enter specifyer mode
            if (query[0] == '§' && query.Length > 2)
            {
                query = query[1..];
                switch (query[..2])
                {
                    //search id
                    case "id":
                        algorithms.Add(SearchID);
                        break;
                    //search translation
                    case "tn":
                        algorithms.Add(SearchTranslation);
                        break;
                    //search english/template
                    case "en":
                        algorithms.Add(SearchTemplate);
                        break;
                    //search comments
                    case "cm":
                        algorithms.Add(SearchComments);
                        break;
                    //search text only, no id
                    case "tx":
                        algorithms.Add(SearchTextOnly);
                        break;
                    //search approved only
                    case "ap":
                        algorithms.Add(SearchApprovedOnly);
                        break;
                    //search unapproved only
                    case "un":
                        algorithms.Add(SearchUnapprovedOnly);
                        break;
                    default:
                        query = query[2..];
                        break;
                }
            }

            if (algorithms.Count == 0) algorithms.Add(SearchAll);

            //run all checks agains all lines
            bool successfull = false;
            while (enumerator.MoveNext())
            {
                successfull = true;
                foreach (var searchAlgorithm in algorithms)
                {
                    if (searchAlgorithm.Invoke(query, enumerator.Current, searchCulture))
                        continue;

                    successfull = false;
                    break;
                }
                if (successfull)
                    results.Add(x);

                ++x;
            }

            cleanedQuery = query;
            return results.Count > 0;
        }

        private static ReadOnlySpan<char> CheckAndClearEscapedChars(ReadOnlySpan<char> query)
        {
            if (query[0] == '\\') // we have an escaped flag following, so we chop of escaper, the character and continue
            {
                if (query[1] == '\\')
                    query = query[1..];
                else
                    query = query[2..];
            }

            return query;
        }

        private static bool SearchComments(ReadOnlySpan<char> query, LineData line, StringComparison comparison) => throw new NotImplementedException();
        private static bool SearchUnapprovedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison) => throw new NotImplementedException();
        private static bool SearchApprovedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison) => throw new NotImplementedException();
        private static bool SearchTextOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison) => throw new NotImplementedException();
        private static bool SearchTemplate(ReadOnlySpan<char> query, LineData line, StringComparison comparison) => throw new NotImplementedException();
        private static bool SearchTranslation(ReadOnlySpan<char> query, LineData line, StringComparison comparison) => throw new NotImplementedException();
        private static bool SearchID(ReadOnlySpan<char> query, LineData line, StringComparison comparison) => throw new NotImplementedException();

        private static bool SearchAll(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            if (line.TranslationString != null)
            {
                if (line.TranslationString.AsSpan().Contains(query, comparison))
                {
                    return true;
                }
            }
            if (line.TemplateString != null)
            {
                if (line.TemplateString.AsSpan().Contains(query, comparison))
                {
                    return true;
                }
            }
            if (line.ID.AsSpan().Contains(query, comparison))
            {
                return true;
            }

            return false;
        }

        public static bool Search(Span<char> query, FileData data, out List<int>? results)
        {
            return Search(query, data, out results, out _);
        }

        public static bool Search(string query, FileData data, out List<int>? results)
        {
            return Search(query, data, out results, out _);
        }
    }
}
