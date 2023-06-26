using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using Translator.Core.Data;

namespace Translator.Core.Helpers
{
    internal static class Searcher
    {
        public static bool Search(ReadOnlySpan<char> query, FileData data, out List<int>? results, out ReadOnlySpan<char> cleanedQuery)
        {
            results = null;
            cleanedQuery = ReadOnlySpan<char>.Empty;
            if (query.IsEmpty) return false;
            if (data.Count == 0) return false;

            results = new();

            //decide on case sensitivity
            if (query[0] == '!' && query.Length > 1) // we set the case sensitive flag
            {
                query = query[1..];
                //methodolgy: highlight items which fulfill search and show count
                int x = 0;
                var enumerator = data.Values.GetEnumerator();
                do
                {
                    if (enumerator.Current.TranslationString != null)
                    {
                        if (enumerator.Current.TranslationString.AsSpan().Contains((ReadOnlySpan<char>)query, StringComparison.CurrentCulture))
                        {
                            results.Add(x++);
                            break;
                        }
                    }
                    if (enumerator.Current.TemplateString != null)
                    {
                        if (enumerator.Current.TemplateString.AsSpan().Contains((ReadOnlySpan<char>)query, StringComparison.CurrentCulture))
                        {
                            results.Add(x++);
                            break;
                        }
                    }
                    if (enumerator.Current.ID.AsSpan().Contains((ReadOnlySpan<char>)query, StringComparison.CurrentCulture))
                    {
                        results.Add(x++);
                        break;
                    }
                    ++x;
                } 
                while (enumerator.MoveNext());
            }
            else
            {
                if (query[0] == '\\') // we have an escaped flag following, so we chop of escaper and continue
                {
                    query = query[1..];
                }

                //methodolgy: highlight items which fulfill search and show count
                int x = 0;
                var enumerator = data.Values.GetEnumerator();
                do
                {
                    if (enumerator.Current.TranslationString != null)
                    {
                        if (enumerator.Current.TranslationString.AsSpan().Contains((ReadOnlySpan<char>)query, StringComparison.CurrentCultureIgnoreCase))
                        {
                            results.Add(x++);
                            break;
                        }
                    }
                    if (enumerator.Current.TemplateString != null)
                    {
                        if (enumerator.Current.TemplateString.AsSpan().Contains((ReadOnlySpan<char>)query, StringComparison.CurrentCultureIgnoreCase))
                        {
                            results.Add(x++);
                            break;
                        }
                    }
                    if (enumerator.Current.ID.AsSpan().Contains((ReadOnlySpan<char>)query, StringComparison.CurrentCultureIgnoreCase))
                    {
                        results.Add(x++);
                        break;
                    }
                    ++x;
                }
                while (enumerator.MoveNext());
            }
            cleanedQuery = query;
            return results.Count > 0;
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
