namespace GeneratorGUI
{
    public partial class Form1 : Form
    {
        List<ConfigCollection> configCollections = new ();
        List<ConfigItem> configItems = new();
        private ConfigManager configManager = new ConfigManager();
        public Form1()
        {
            InitializeComponent();
        }

        private void tbFile_MouseClick(object sender, MouseEventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    tbFile.Text = ofd.FileName;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.SaveModify())
                {
                    MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败： " + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCurrentItem();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            configItems = configCollections[comboBox1.SelectedIndex].Items;
            RefreshConfigList();
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshConfigCollection();
        }

        private void RefreshConfigCollection()
        {
            configCollections = configManager.Scan();
            comboBox1.Items.Clear();

            foreach (var item in configCollections)
            {
                comboBox1.Items.Add(item.Name);
            }
        }

        private void RefreshConfigList()
        {
            listItem.Items.Clear();

            foreach (var item in configItems)
            {
                listItem.Items.Add(item.Name);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.RefreshConfigCollection();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveConfigCollection();
        }

        private void SaveConfigCollection()
        {
            try
            {
                if (string.IsNullOrEmpty(comboBox1.Text))
                {
                    MessageBox.Show("配置名称不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (var item in configCollections)
                {
                    configManager.SaveConfig(comboBox1.Text, configItems);
                }

                this.RefreshConfigCollection();
                MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败： " + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 新增ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configItems.Add(new ConfigItem() {
                Name = "新增1"
            });
            RefreshConfigList();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = listItem.SelectedIndex;
            configItems.RemoveAt(index);
            RefreshConfigList();

        }

        private void RefreshCurrentItem()
        {
            var index = listItem.SelectedIndex;
            if(index < 0) { return; }

            tbName.Text = configItems[index].Name;
            tbCommand.Lines = configItems[index].Command == null ? null : configItems[index].Command.ToArray();
            tbRemote.Text = configItems[index].Remote;
            tbFile.Text = configItems[index].File;
            tbPassword.Text = configItems[index].Password;
            tbUser.Text = configItems[index].User;
        }

        private bool SaveModify()
        {
            var index = listItem.SelectedIndex;
            if(index < 0) { return false;}

            if(String.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("Name不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!String.IsNullOrEmpty(tbFile.Text) && String.IsNullOrEmpty(tbRemote.Text))
            {
                MessageBox.Show("填写File字段后Remote字段必填", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            configItems[index].Name = tbName.Text;
            configItems[index].Remote = tbRemote.Text;
            configItems[index].File = tbFile.Text;
            configItems[index].User = tbUser.Text;
            configItems[index].Password= tbPassword.Text;

            if (configItems[index].Command == null)
            {
                configItems[index].Command = new List<string>();
            }
            configItems[index].Command.Clear();
            configItems[index].Command.AddRange(tbCommand.Lines);

            configManager.SaveConfig(comboBox1.Text,configItems);

            return true;
        }

        private void 生成部署包ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "*.exe|*.exe";
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    var gen = new AutoInstallGenerator();
                    foreach (var item in configItems)
                    {
                        gen.AddFile(item);
                    }

                    gen.Generate(sfd.FileName);
                }
            }
        }

        private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (configManager.RemoveConfig(comboBox1.Text) && !string.IsNullOrEmpty(comboBox1.Text))
            {
                RefreshConfigCollection();
                MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("删除失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}