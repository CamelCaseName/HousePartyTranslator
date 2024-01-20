using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Translator.Core.Data;

namespace Translator.Core.Helpers
{
    public static class Searcher
    {
        public delegate bool SearchImplementation(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern);

        private static readonly List<SearchImplementation> algorithms = new();
        private static StringComparison searchCulture = StringComparison.CurrentCultureIgnoreCase;

        public static bool Search(ReadOnlySpan<char> query, FileData data, out List<int>? results, out ReadOnlySpan<char> cleanedQuery)
        {
            results = null;
            cleanedQuery = ReadOnlySpan<char>.Empty;
            if (query.IsEmpty) return false;
            if (data.Count == 0) return false;

            results = new();
            Dictionary<EekStringID, LineData>.ValueCollection.Enumerator enumerator = data.Values.GetEnumerator();
            int x = 0;

            //case sensitive search
            query = ExtractCaseSensitivityCulture(query);

            algorithms.Clear();
            //2nd escaper
            bool useRegex = ExtractSearchModifiers(ref query);

            //we only extracted an specifyer, no query yet
            if (query.IsEmpty) return false;

            //fix search if we have nothing or only modifiers
            TrySetDefaultAlgorithms();

            //run all checks agains all lines
            bool successfull;
            Regex? regex = CreateRegex(query, useRegex);
            while (enumerator.MoveNext())
            {
                successfull = true;
                foreach (SearchImplementation searchAlgorithm in algorithms)
                {
                    if (searchAlgorithm.Invoke(query, enumerator.Current, searchCulture, regex))
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

        public static bool Search(ReadOnlySpan<char> query, LineData data, out ReadOnlySpan<char> cleanedQuery)
        {
            cleanedQuery = ReadOnlySpan<char>.Empty;
            if (query.IsEmpty) return false;

            //case sensitive search
            query = ExtractCaseSensitivityCulture(query);

            algorithms.Clear();
            //2nd escaper
            bool useRegex = ExtractSearchModifiers(ref query);

            //we only extracted an specifyer, no query yet
            if (query.IsEmpty) return false;

            //fix search if we have nothing or only modifiers
            TrySetDefaultAlgorithms();

            //run all checks agains all lines
            Regex? regex = CreateRegex(query, useRegex);

            bool successfull = true;
            foreach (SearchImplementation searchAlgorithm in algorithms)
            {
                if (searchAlgorithm.Invoke(query, data, searchCulture, regex))
                    continue;

                successfull = false;
                break;
            }

            cleanedQuery = query;
            return successfull;
        }

        private static Regex? CreateRegex(ReadOnlySpan<char> query, bool useRegex)
        {
            Regex? regex = null;
            if (useRegex)
            {
                try
                {
                    regex = searchCulture == StringComparison.CurrentCultureIgnoreCase
                        ? new Regex(query.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase)
                        : new Regex(query.ToString(), RegexOptions.Compiled);
                }
                catch { }
            }

            return regex;
        }

        private static void TrySetDefaultAlgorithms()
        {
            bool SearchOnlyContainsModifiers = !algorithms.Contains(SearchID)
                            && !algorithms.Contains(SearchTranslation)
                            && !algorithms.Contains(SearchTemplate)
                            && !algorithms.Contains(SearchComments);

            if (algorithms.Count == 0 || SearchOnlyContainsModifiers)
            {
                algorithms.Add(SearchAll);
            }
        }

        private static ReadOnlySpan<char> ExtractCaseSensitivityCulture(ReadOnlySpan<char> query)
        {
            if (!CheckAndClearEscapedChars(ref query))
            {
                if (query[0] == '!' && query.Length > 1) // we set the case sensitive flag
                {
                    query = query[1..];
                    searchCulture = StringComparison.CurrentCulture;
                }
            }

            return query;
        }

        private static bool ExtractSearchModifiers(ref ReadOnlySpan<char> query)
        {
            bool useRegex = false;
            if (!CheckAndClearEscapedChars(ref query))
            {
                //we enter specifyer mode
                if (query[0] == '§' && query.Length > 2)
                {
                    //just add mupltiple search types like this §id§tn
                    do
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
                                algorithms.Add(SearchText);
                                break;
                            //search approved only
                            case "ap":
                                algorithms.Add(SearchApprovedOnly);
                                break;
                            //search unapproved only
                            case "un":
                                algorithms.Add(SearchUnapprovedOnly);
                                break;
                            //translated lines only
                            case "td":
                                algorithms.Add(SearchTranslatedOnly);
                                break;
                            //untranslated lines only
                            case "ut":
                                algorithms.Add(SearchUntranslatedOnly);
                                break;
                            //finds lines where translation is equal to the template, disregards query
                            case "ma":
                                algorithms.Add(FindTemplateTranslationIdentical);
                                break;
                            //treat query as regex
                            case "rg":
                                useRegex = true;
                                break;
                            default:
                                break;
                        }
                        query = query[2..];
                    } while (!query.IsEmpty && query[0] == '§' && query.Length > 2);
                }
            }
            return useRegex;
        }

        private static bool RemoveModifiers(ref ReadOnlySpan<char> query)
        {
            bool useRegex = false;
            if (!CheckAndClearEscapedChars(ref query))
            {
                //we enter specifyer mode
                if (query[0] == '§' && query.Length > 2)
                {
                    //just add mupltiple search types like this §id§tn
                    do
                    {
                        query = query[1..];
                        switch (query[..2])
                        {
                            //treat query as regex
                            case "rg":
                                useRegex = true;
                                break;
                            default:
                                break;
                        }
                        query = query[2..];
                    } while (!query.IsEmpty && query[0] == '§' && query.Length > 2);
                }
            }
            return useRegex;
        }

        private static bool CheckAndClearEscapedChars(ref ReadOnlySpan<char> query)
        {
            if (query.IsEmpty) return false;
            if (query.Length > 1)
            {
                if (query[0] == '\\' && (query[1] == '!' || query[1] == '§')) // we have an escaped flag following, so we chop of escaper and continue
                {
                    query = query[1..];
                    return true;
                }
                //only check for inline escape when we find one
                int pos = query.IndexOf('\\');
                if (pos == -1) return false;
                if (pos == query.Length - 1) return false;

                if (query[pos] == '\\' && (query[pos + 1] == '!' || query[pos + 1] == '§')) // we have an escaped flag following, so we chop of escaper and continue
                {
                    query = query.RemoveAt(pos, 1);
                    return true;
                }
            }

            return false;
        }

        private static bool FindTemplateTranslationIdentical(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return line.TemplateString == line.TranslationString;
        }

        private static bool SearchUntranslatedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return !line.IsTranslated;
        }

        private static bool SearchTranslatedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return line.IsTranslated;
        }

        private static bool SearchUnapprovedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return !line.IsApproved;
        }

        private static bool SearchApprovedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return line.IsApproved;
        }

        private static bool SearchComments(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            if (line.Comments is not null && line.Comments.Length > 0)
            {
                foreach (string comment in line.Comments)
                {
                    bool result = pattern is not null ? SafeRegexSearch(comment, pattern) : comment.AsSpan().Contains(query, comparison);
                    if (result) return true;
                }
            }
            return false;
        }

        private static bool SearchTemplate(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return line.TemplateString is not null
&& (pattern is not null ? SafeRegexSearch(line.TemplateString, pattern) : line.TemplateString.AsSpan().Contains(query, comparison));
        }

        private static bool SearchTranslation(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return line.TranslationString is not null
&& (pattern is not null
                    ? SafeRegexSearch(line.TranslationString, pattern)
                    : line.TranslationString.AsSpan().Contains(query, comparison));
        }

        private static bool SearchID(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return pattern is not null ? SafeRegexSearch(line.ID, pattern) : line.ID.AsSpan().Contains(query, comparison);
        }

        private static bool SearchAll(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return SearchID(query, line, comparison, pattern)
                || SearchTranslation(query, line, comparison, pattern)
                || SearchTemplate(query, line, comparison, pattern);
        }

        private static bool SearchText(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            return SearchTranslation(query, line, comparison, pattern)
                || SearchTemplate(query, line, comparison, pattern);
        }

        private static bool SafeRegexSearch(ReadOnlySpan<char> text, Regex pattern)
        {
            try
            {
                return pattern.IsMatch(text);
            }
            catch
            {
                return false;
            }
        }

        public static bool TryGetSearchResult(ReadOnlySpan<char> query, ReadOnlySpan<char> line, out int position, out int length)
        {
            position = -1;
            length = -1;
            if (query.IsEmpty) return false;
            if (line is "") return false;

            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;

            //case sensitive search
            if (!CheckAndClearEscapedChars(ref query))
            {
                if (query[0] == '!' && query.Length > 1) // we set the case sensitive flag
                {
                    query = query[1..];
                    comparison = StringComparison.CurrentCulture;
                }
            }

            //we only extracted an specifyer, no query yet
            if (query.IsEmpty) return false;

            if (RemoveModifiers(ref query))
            {
                //run all checks agains all lines
                Regex? regex = CreateRegex(query, true);
                if (regex is not null)
                {
                    try
                    {
                        Match match = regex.Match(line.ToString());
                        if (match.Success)
                        {
                            position = match.Index;
                            length = match.Length;
                            return true;
                        }
                        return false;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            position = line.IndexOf(query, comparison);
            length = query.Length;
            return position >= 0;
        }

        public static bool Search(ReadOnlySpan<char> query, FileData data, out List<int>? results)
        {
            return Search(query, data, out results, out _);
        }

        public static bool Search(string query, FileData data, out List<int>? results)
        {
            return Search(query, data, out results, out _);
        }

        public static bool Search(ReadOnlySpan<char> query, LineData data)
        {
            return Search(query, data, out _);
        }

        public static bool Search(string query, LineData data)
        {
            return Search(query.AsSpan(), data, out _);
        }
    }
}
