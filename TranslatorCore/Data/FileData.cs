using System;
using System.Collections.Generic;

namespace Translator.Core.Data
{
    public sealed class FileData : Dictionary<string, LineData>
    {
        public readonly string FileName;
        public readonly string StoryName; 

        public FileData(Dictionary<string, LineData> data, string story, string file)
        {
            FileName = file;
            StoryName = story;
            foreach (KeyValuePair<string, LineData> item in data)
            {
                Add(item.Key, item.Value);
            }
        }

        public FileData(string story, string file)
        {
            FileName = file;
            StoryName = story;
            Clear();
        }

        public KeyValuePair<string, LineData> ElementAt(int index)
        {
            if(index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

            var enumerator = GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext())
            {
                if (i == index) return enumerator.Current;
                else ++i;
            }
            throw new IndexOutOfRangeException(nameof(index));
        }
    }
}
