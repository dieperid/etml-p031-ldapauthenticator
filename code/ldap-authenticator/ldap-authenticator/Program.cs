using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ldap_authenticator
{
    static class Program
    {
        /// <summary>
        /// Instance objet log4net
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frm1());

            //Tests log4net
            log.Info("Conneting to XXX.XX.XX.XX:YYY");
            log.Info("LDAP Connection OK");
            log.Error("Connection failed : cause");
            Console.ReadLine();
        }
    }
}
