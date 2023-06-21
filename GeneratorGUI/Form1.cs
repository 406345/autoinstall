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
                    MessageBox.Show("����ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("����ʧ�ܣ� " + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void ˢ��ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveConfigCollection();
        }

        private void SaveConfigCollection()
        {
            try
            {
                if (string.IsNullOrEmpty(comboBox1.Text))
                {
                    MessageBox.Show("�������Ʋ���Ϊ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (var item in configCollections)
                {
                    configManager.SaveConfig(comboBox1.Text, configItems);
                }

                this.RefreshConfigCollection();
                MessageBox.Show("����ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("����ʧ�ܣ� " + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configItems.Add(new ConfigItem() {
                Name = "����1"
            });
            RefreshConfigList();
        }

        private void ɾ��ToolStripMenuItem_Click(object sender, EventArgs e)
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
                MessageBox.Show("Name����Ϊ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!String.IsNullOrEmpty(tbFile.Text) && String.IsNullOrEmpty(tbRemote.Text))
            {
                MessageBox.Show("��дFile�ֶκ�Remote�ֶα���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void ���ɲ����ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ɾ��ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (configManager.RemoveConfig(comboBox1.Text) && !string.IsNullOrEmpty(comboBox1.Text))
            {
                RefreshConfigCollection();
                MessageBox.Show("ɾ���ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("ɾ��ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void �˳�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}