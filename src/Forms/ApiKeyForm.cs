using System;
using System.Diagnostics;
using System.Windows.Forms;
using WakaTime;

namespace CodeCave.WakaTime.Revit
{
    public partial class ApiKeyForm : Form
    {
        public ApiKeyForm()
        {
            InitializeComponent();
        }

        private void ApiKeyForm_Load(object sender, EventArgs e)
        {
            try
            {
                txtAPIKey.Text = WakaTimeConfigFile.ApiKey;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                Guid apiKey;
                var parse = Guid.TryParse(txtAPIKey.Text.Trim(), out apiKey);
                if (parse)
                {
                    WakaTimeConfigFile.ApiKey = apiKey.ToString();
                    WakaTimeConfigFile.Save();
                }
                else
                {
                    MessageBox.Show("Please enter valid API Key.");
                    DialogResult = DialogResult.None; // do not close dialog box
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string wakaTimeUrl = "https://wakatime.com/settings/account";

            try
            {
                Process.Start(wakaTimeUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
