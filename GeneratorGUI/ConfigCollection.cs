using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorGUI
{
    internal class ConfigCollection
    {
        public string Name { get; set; } = "";
        public List<ConfigItem> Items { get; set; } = new();
        public ConfigCollection() { }
    }
}
