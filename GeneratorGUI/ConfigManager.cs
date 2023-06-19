using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorGUI
{
    internal class ConfigManager
    {
        public ConfigManager()
        {
        }

        private void CheckConfigDirectory()
        {
            if (!System.IO.Directory.Exists("configs"))
            {
                System.IO.Directory.CreateDirectory("configs");
            }
        }
        public List<ConfigCollection> Scan()
        {
            this.CheckConfigDirectory();
            List<ConfigCollection> list = new();

            var files = System.IO.Directory.GetFiles("configs");

            foreach (var file in files)
            {
                if (file.EndsWith(".json"))
                {
                    var tmp = new ConfigCollection();
                    tmp.Name = Path.GetFileNameWithoutExtension(file);
                    tmp.Items = this.LoadConfig(file);
                    list.Add(tmp);
                }
            }

            return list;
        }

        public bool RemoveConfig(string name)
        {
            string path = "configs/" + name + ".json";
            try
            {
                if(!System.IO.File.Exists(path))
                {
                    return false;
                }

                System.IO.File.Delete("configs/" + name + ".json");
            }
            catch
            {
                return false;
            }
            return true;
        }

        public List<ConfigItem> LoadConfig(string file)
        {
            List<ConfigItem> list = new List<ConfigItem>();
            try
            {
                if (File.Exists(file))
                {
                    list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfigItem>>(System.IO.File.ReadAllText(file));
                }
            }
            catch
            {

            }

            return list;
        }
        public void SaveConfig(ConfigCollection collection)
        {
            this.SaveConfig(collection.Name, collection.Items);
        }

        public void SaveConfig(string name, List<ConfigItem> configs)
        {
            this.CheckConfigDirectory();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(configs);
            File.WriteAllText(Path.Combine("./configs", name + ".json"), json);
        }
    }
}
