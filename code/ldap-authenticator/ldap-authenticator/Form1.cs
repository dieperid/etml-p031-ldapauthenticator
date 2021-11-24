using System;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ldap_authenticator
{
    public partial class frm1 : Form
    {
        /// <summary>
        /// Instance objet log4net
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public frm1()
        {
            InitializeComponent();
        }

        private void Frm1_Load(object sender, EventArgs e)
        {
            
        }

        private DirectorySearcher search;
        
        private string _path { get; set; }


        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            string domainAndUsername = domain + "\\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);
            search = new DirectorySearcher(entry);
            try
            {
                var Address = Dns.GetHostAddresses(domain).FirstOrDefault();
                log.Info("Connecting to" + " " + Address.ToString());
                search.Filter = "(SAMAccountName=" + username + ")";
                object obj = entry.NativeObject;             

                SearchResult result = search.FindOne();
                if (result == null)
                {
                    return false;               
                }
            }
            catch (Exception ex)
            {             
                // Si l'utilisateur n'existe pas dans le domaine
                MessageBox.Show("L'utilisateur n'existe pas");
                log.Error("Connection failed: user not found");
                return false;
            }
            return true;
        }

        public static string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Récupèration des données du fichier config.json 
            StreamReader r = new StreamReader("ldap-authenticator.json");
            string jsonString = r.ReadToEnd();

            ConfigServer serverData = JsonConvert.DeserializeObject<ConfigServer>(jsonString);

            if (IsAuthenticated(serverData.DomainName, txtboxUsername.Text, txtboxPassword.Text))
            {
                // user is exist in active directory
                MessageBox.Show("Bienvenue!");
                log.Info("LDAP Connection OK");

                foreach (SearchResult sResultSet in search.FindAll())
                {
                    lbl1.Text = "User Logon Name : " + txtboxUsername.Text;
                    lbl2.Text = "Last Password Changed : " + GetProperty(sResultSet, "whenchanged");
                    lbl3.Text = "Group(s) Member(s) : " + GetProperty(sResultSet, "extensionAttribute3");
                    lbl4.Text = "First Name : " + GetProperty(sResultSet, "givenName");
                    lbl5.Text = "Last Name : " + GetProperty(sResultSet, "sn");
                    lbl6.Text = "Display Name : " + GetProperty(sResultSet, "cn");
                    lbl7.Text = "E-mail-Addresses : " + GetProperty(sResultSet, "mail");
                }
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
    class ConfigServer
    {
        public string DomainName { get; set; }
    }
}
