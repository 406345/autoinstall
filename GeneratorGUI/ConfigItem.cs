using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GeneratorGUI
{
    internal class ConfigItem
    { 
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; }
        [Newtonsoft.Json.JsonProperty("command")]
        public List<string> Command { get; set; } = new ();
        [Newtonsoft.Json.JsonProperty("file")]
        public string File { get; set; }
        [Newtonsoft.Json.JsonProperty("remote")]
        public string Remote { get; set; }
        [Newtonsoft.Json.JsonProperty("user")]
        public string User { get; set; }
        [Newtonsoft.Json.JsonProperty("password")]
        public string Password { get; set; }
    }
}
