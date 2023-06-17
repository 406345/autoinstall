using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace generator
{
    internal class ConfigItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("command")]
        public string Command { get; set; }
        [JsonPropertyName("file")]
        public string File { get; set; }
        [JsonPropertyName("remote")]
        public string Remote { get; set; }
    }
}
