using System;

namespace Translator.Core.Helpers
{
    internal static class Replacer
    {
        public static ReadOnlySpan<char> Replace(ReadOnlySpan<char> source, ReadOnlySpan<char> replacement, ReadOnlySpan<char> query)
        {
            return Searcher.TryGetSearchResult(query, source, out int position, out int length)
                ? (ReadOnlySpan<char>)string.Concat(source[..position], replacement, source[(position + length)..])
                : source;
        }
    }
}
