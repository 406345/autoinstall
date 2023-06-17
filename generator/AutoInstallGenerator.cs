using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generator
{
    internal class AutoInstallGenerator
    {
        const string ORI_PE_NAME = "autoinstall.dat";
        const string GEN_PE_NAME = "autoinstall.exe";
        List<ConfigItem> configItems = new List<ConfigItem>();
        public AutoInstallGenerator()
        {

        }

        public bool AddFile(ConfigItem item)
        {
            configItems.Add(item);
            return true;
        }

        public void Generate(string path = GEN_PE_NAME)
        {
            File.Copy(ORI_PE_NAME, path, true);
            var file = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            var fileSize = file.Length;
            file.Seek(fileSize, SeekOrigin.Begin);
            BinaryWriter bw = new BinaryWriter(file);
            WriteConfig(bw);
            bw.Write((Int32)new FileInfo(ORI_PE_NAME).Length);
            bw.Close();
            bw.Dispose();
        }

        private void WriteFileBlock(ConfigItem config, BinaryWriter bw)
        {

            var buffer = new byte[1024 * 1024 * 10];

            if (!File.Exists(config.File))
            {
                bw.Write((ulong)0);
                return;
            }

            FileStream fs = File.OpenRead(config.File);
            bw.Write((ulong)fs.Length);

            while (true)
            {
                int reads = fs.Read(buffer);
                if (reads == 0) break;
                bw.Write(buffer, 0, reads);
            }

            fs.Close();
            fs.Dispose();
        }

        private void WriteConfig(BinaryWriter bw)
        {
            bw.Write((UInt32)configItems.Count);
            foreach (var config in configItems)
            {
                Console.WriteLine("Writing " + config.Name);

                bw.Write((UInt16)config.Name.Length);
                bw.Write(Encoding.UTF8.GetBytes(config.Name));

                if(config.Command == null)
                {
                    bw.Write((UInt16)0);
                }
                else
                {
                    bw.Write((UInt16)config.Command.Length);
                    bw.Write(Encoding.UTF8.GetBytes(config.Command));
                }

                if (config.Remote == null)
                {
                    bw.Write((UInt16)0);
                }
                else
                {
                    bw.Write((UInt16)config.Remote.Length);
                    bw.Write(Encoding.UTF8.GetBytes(config.Remote));
                }

                WriteFileBlock(config, bw);
            }
        }
    }
}
