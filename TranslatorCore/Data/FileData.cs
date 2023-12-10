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
                Add(item.Key, new(item.Value));
            }
        }

        public FileData(string story, string file)
        {
            FileName = file;
            StoryName = story;
            Clear();
        }

        public FileData(FileData old)
        {
            FileName = old.FileName;
            StoryName = old.StoryName;
            foreach (KeyValuePair<string, LineData> item in old)
            {
                Add(item.Key, new(item.Value));
            }
        }

        public KeyValuePair<string, LineData> ElementAt(int index)
        {
            if (index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

            Enumerator enumerator = GetEnumerator();
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
