using System.Collections.Generic;

namespace Translator.Core.Data
{
    public sealed class FileData : Dictionary<string, LineData>
    {
        public FileData(Dictionary<string, LineData> data)
        {
            foreach (KeyValuePair<string, LineData> item in data)
            {
                Add(item.Key, item.Value);
            }
        }

        public FileData()
        {
            Clear();
        }
    }
}
