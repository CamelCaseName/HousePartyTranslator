using System.Collections.Generic;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core.Data
{
    public class MenuItems : List<IMenuItem>
    {
        public MenuItems() : base() { }

        public MenuItems(int capacity) : base(capacity) { }
    }
}