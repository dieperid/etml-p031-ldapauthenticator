///ETML
///Auteur : Alexis Rojas, David Dieperink, Stefan Petrovic, Samuel Hörler
///Date : 24.11.2021
///Description: Gère le déroulement de l'application 
using System;
using System.Windows.Forms;

namespace ldap_authenticator
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frm1());
        }
    }
}
