using System;
using System.DirectoryServices;
using System.Drawing;
using System.Windows.Forms;

namespace ldap_authenticator
{
    public partial class frm1 : Form
    {
        public frm1()
        {
            InitializeComponent();
        }

        private void Frm1_Load(object sender, EventArgs e)
        {
            
        }

        private string _path { get; set; }
        private string _filterAttribute { get; set; }

        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + "\\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);
            try
            {
                object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if ((result == null))
                {
                    return false;
                }

                _path = result.Path;
                _filterAttribute = result.Properties["cn"][0].ToString();

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            int Y = 10;
            int compt = 1;

            for (int i = 1; i < 4; i++)
            {
                Label lbl = new Label();
                lbl.Location = new Point(10, Y);
                lbl.Name = "lbl" + i;
                lbl.Size = new Size(75, 23);
                lbl.Text = "lable_" + i;
                pnl1.Controls.Add(lbl);

                Y = Y + 23;
            }

            if (IsAuthenticated("labdev.local", txtboxUsername.Text, txtboxPassword.Text) == false)
            {
                // not exist in active directory!
                
            }
            else
            {
                // user is exist in active directory

            }
        }

        private void BtnPass_MouseDown(object sender, MouseEventArgs e)
        {
            txtboxPassword.PasswordChar = default;
        }

        private void BtnPass_MouseUp(object sender, MouseEventArgs e)
        {
            txtboxPassword.PasswordChar = '*';
        }
    }
}
