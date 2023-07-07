using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Translator.Core.Data;

namespace Translator.Core.Helpers
{
    internal static class Searcher
    {
        public delegate bool SearchImplementation(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern);
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
            bool useRegex = false;

            //case sensitive search
            if (!CheckAndClearEscapedChars(ref query))
            {
                if (query[0] == '!' && query.Length > 1) // we set the case sensitive flag
                {
                    query = query[1..];
                    searchCulture = StringComparison.CurrentCulture;
                }
            }

            //2nd escaper
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
            //we only extracted an specifyer, no query yet
            if (query.IsEmpty) return false;

            //fix search if we have nothing or only modifiers
            bool SearchOnlyContainsModifiers = !algorithms.Contains(SearchID)
                && !algorithms.Contains(SearchTranslation)
                && !algorithms.Contains(SearchTemplate)
                && !algorithms.Contains(SearchComments);

            if (algorithms.Count == 0 || SearchOnlyContainsModifiers)
            {
                algorithms.Add(SearchAll);
            }

            //run all checks agains all lines
            bool successfull = false;
            Regex? regex = null;
            if (useRegex)
            {
                try
                {
                    if (searchCulture == StringComparison.CurrentCultureIgnoreCase) regex = new Regex(query.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    else regex = new Regex(query.ToString(), RegexOptions.Compiled);
                }
                catch { }
            }
            while (enumerator.MoveNext())
            {
                successfull = true;
                foreach (var searchAlgorithm in algorithms)
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
            if (line.Comments != null && line.Comments.Length > 0)
            {
                foreach (var comment in line.Comments)
                {
                    bool result;
                    if (pattern != null)
                    {
                        result = SafeRegexSearch(comment, pattern);
                    }
                    else
                    {
                        result = comment.AsSpan().Contains(query, comparison);
                    }
                    if (result) return true;
                }
            }
            return false;
        }

        private static bool SearchTemplate(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            if (line.TemplateString != null)
            {
                if (pattern != null)
                {
                    return SafeRegexSearch(line.TemplateString, pattern);
                }
                else
                {
                    return line.TemplateString.AsSpan().Contains(query, comparison);
                }
            }
            return false;
        }

        private static bool SearchTranslation(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            if (line.TranslationString != null)
            {
                if (pattern != null)
                {
                    return SafeRegexSearch(line.TranslationString, pattern);
                }
                else
                {
                    return line.TranslationString.AsSpan().Contains(query, comparison);
                }
            }
            return false;
        }

        private static bool SearchID(ReadOnlySpan<char> query, LineData line, StringComparison comparison, Regex? pattern)
        {
            if (pattern != null)
            {
                return SafeRegexSearch(line.ID, pattern);
            }
            else
            {
                return line.ID.AsSpan().Contains(query, comparison);
            }
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

        public static bool Search(ReadOnlySpan<char> query, FileData data, out List<int>? results)
        {
            return Search(query, data, out results, out _);
        }

        public static bool Search(string query, FileData data, out List<int>? results)
        {
            return Search(query, data, out results, out _);
        }
    }
}
