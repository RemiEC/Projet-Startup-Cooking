﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projet_Startup_Cooking_BDD
{
    /// <summary>
    /// Logique d'interaction pour Creation_compte_client.xaml
    /// </summary>
    public partial class Creation_compte_client : Page
    {
        public Creation_compte_client()
        {
            InitializeComponent();
        }

        private void Button_Click_creer_compte_client(object sender, RoutedEventArgs e)
        {
            
            string id = idTextBox.Text;
            string mdp = mdpTextBox.Text;
            string nom = nomTextBox.Text;
            string tel = telTextBox.Text;
            string requete = $"INSERT INTO cooking.client VALUES (\"{id}\",\"{mdp}\",\"{nom}\",\"{tel}\",0,False);";
            Commandes_SQL.Insert_Requete(requete);
            Interface_Home homepage = new Interface_Home();
            this.NavigationService.Navigate(homepage);


        }
    }
}
