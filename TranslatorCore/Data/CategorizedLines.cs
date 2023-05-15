using System;
using System.Collections.Generic;

namespace Translator.Core.Data
{
    public struct CategorizedLines
    {
        public List<LineData> lines;
        public StringCategory category;

        public CategorizedLines(List<LineData> lines, StringCategory category)
        {
            this.lines = lines;
            this.category = category;
        }

        public override bool Equals(object? obj) => obj is CategorizedLines other && EqualityComparer<List<LineData>>.Default.Equals(lines, other.lines) && category == other.category;

        public override int GetHashCode()
        {
            return HashCode.Combine(lines, category);
        }

        public void Deconstruct(out List<LineData> lines, out StringCategory category)
        {
            lines = this.lines;
            category = this.category;
        }

        public static implicit operator (List<LineData> lines, StringCategory category)(CategorizedLines value) => (value.lines, value.category);
        public static implicit operator CategorizedLines((List<LineData> lines, StringCategory category) value) => new(value.lines, value.category);

        public static bool operator ==(CategorizedLines left, CategorizedLines right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CategorizedLines left, CategorizedLines right)
        {
            return !(left == right);
        }
    }
}
