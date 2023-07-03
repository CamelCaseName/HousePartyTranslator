using System;
using System.Collections.Generic;
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
                                algorithms.Add(SearchTemplate);
                                algorithms.Add(SearchTranslation);
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
                algorithms.Add(SearchID);
                algorithms.Add(SearchTranslation);
                algorithms.Add(SearchTemplate);
                algorithms.Add(SearchComments);
            }

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

        private static bool FindTemplateTranslationIdentical(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            return line.TemplateString == line.TranslationString;
        }

        private static bool SearchUntranslatedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            return !line.IsTranslated;
        }

        private static bool SearchTranslatedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            return line.IsTranslated;
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

        private static bool SearchComments(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            if (line.Comments != null)
            {
                foreach (var comment in line.Comments.AsSpan())
                {
                    if (comment.AsSpan().Contains(query, comparison))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool SearchUnapprovedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            return !line.IsApproved;
        }

        private static bool SearchApprovedOnly(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            return line.IsApproved;
        }

        private static bool SearchTemplate(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            if (line.TemplateString != null)
            {
                if (line.TemplateString.AsSpan().Contains(query, comparison))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SearchTranslation(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            if (line.TranslationString != null)
            {
                if (line.TranslationString.AsSpan().Contains(query, comparison))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SearchID(ReadOnlySpan<char> query, LineData line, StringComparison comparison)
        {
            if (line.ID.AsSpan().Contains(query, comparison))
            {
                return true;
            }
            return false;
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
