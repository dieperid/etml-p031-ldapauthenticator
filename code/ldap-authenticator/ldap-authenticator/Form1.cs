///ETML
///Auteur : Alexis Rojas, David Dieperink, Stefan Petrovic, Samuel Hörler
///Date : 24.11.2021
///Description: Permet de se connecter à un serveur de domaine ADDS et afficher les données de l'utilisateur
using Newtonsoft.Json;
using System;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace ldap_authenticator
{
    /// <summary>
    /// Class frm1
    /// </summary>
    public partial class frm1 : Form
    {
        #region Attributs 
        /// <summary>
        /// Instance objet log4net
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Variable pour effectuer les requetes au service du domaine ADDS
        /// </summary>
        private DirectorySearcher _search;
        /// <summary>
        /// Variable pour récupérer le chemin
        /// </summary>
        private string Path { get; set; }
        #endregion

        #region Constructeurs
        /// <summary>
        /// Default constructor
        /// </summary>
        public frm1()
        {
            InitializeComponent();
        }
        #endregion

        #region Methodes
        /// <summary>
        /// Vérifie les informations de connexion
        /// </summary>
        /// <param name="domain">Nom du domaine</param>
        /// <param name="username">Nom d'utilisateur</param>
        /// <param name="pwd">Mot de passe de l'utilisateur</param>
        /// <returns>True si la connexion avec le serveur de domaine est reussie</returns>
        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            //Concatenation du nom du domaine + nom d'utilisateur
            string domainAndUsername = domain + "\\" + username;

            //Entrée au serveur ADDS
            DirectoryEntry entry = new DirectoryEntry(Path, domainAndUsername, pwd);

            //Initialisation _search pour effectuer les requetes au service du domaine ADDS
            _search = new DirectorySearcher(entry);

            //Essaie d'effectuer la connexion avec le serveur
            try
            {
                //Récupération de l'adresse IP du serveur de domaine
                var Address = Dns.GetHostAddresses(domain).FirstOrDefault();

                //Message log
                log.Info("Connecting to" + " " + Address.ToString());

                //Requete des informations de l'utilisateur logué
                _search.Filter = "(SAMAccountName=" + username + ")";

                SearchResult result = _search.FindOne();

                //Si l'utilisateur n'existe pas
                if (result == null)
                {
                    return false;
                }
            }
            //Catch l'error de connexion
            catch (Exception)
            {
                //Si la connexion a échouée
                MessageBox.Show("Password ou Username incorrect", "Error de connexion");

                //Message d'error sur le log
                log.Error("Connection failed: user not found");

                //Retourne false vu que la connexion a échoué
                return false;
            }
            //Retourne true dès que la connexion a reussie
            return true;
        }
        /// <summary>
        /// Permet de récupérer les informations d'une requete sur le serveur ADDS
        /// </summary>
        /// <param name="searchResult">Résultat de la requete</param>
        /// <param name="PropertyName">Propriété qu'on veut récupérer</param>
        /// <returns>Retourne la valeur de la propriété en string</returns>
        public static string GetProperty(SearchResult searchResult, string PropertyName)
        {
            //Si la propriété recherché existe
            if (searchResult.Properties.Contains(PropertyName))
            {
                //Retourne la valeur de la propriété de l'utilisateur recherché
                return searchResult.Properties[PropertyName][0].ToString();
            }
            //Sinon, on retourne une chaine vide
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Evenement dès qu'on click sur le bouton Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Récupèration des données du fichier config.json 
            StreamReader r = new StreamReader("ldap-authenticator.json");
            string jsonString = r.ReadToEnd();

            //Crée un objet Config ServerData avec les informations du fichier de config JSON
            ConfigServer serverData = JsonConvert.DeserializeObject<ConfigServer>(jsonString);

            //Si la connexion a été reussie
            if (IsAuthenticated(serverData.DomainName, txtboxUsername.Text, txtboxPassword.Text))
            {
                //Message de bienvenue et message de connexion sur le fichier log.txt
                MessageBox.Show("Bienvenue!");
                log.Info("LDAP Connection OK");

                //Parcourt les données de la requete et les affiche sur le formulaire
                foreach (SearchResult sResultSet in _search.FindAll())
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
        /// <summary>
        /// Permet d'afficher le mot de passe en brut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPass_MouseDown(object sender, MouseEventArgs e)
        {
            txtboxPassword.PasswordChar = default;
        }
        /// <summary>
        /// Masque le mot de passe en remplaçant les caractères par des étoiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPass_MouseUp(object sender, MouseEventArgs e)
        {
            txtboxPassword.PasswordChar = '*';
        }
        #endregion
    }
    /// <summary>
    /// Class qui permet de récupérer les informations du fichier de config JSON
    /// </summary>
    class ConfigServer
    {
        /// <summary>
        /// Nom du domaine
        /// </summary>
        public string DomainName { get; set; }
    }
}
