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
    /// Logique d'interaction pour Page_Client.xaml
    /// </summary>
    public partial class Page_Client : Page
    {
        private string id_client;
        /// <summary>
        /// Initialisation de la page permettant l'écriture du welcome message, solde en cook, de la listview des recettes dispos ainsi que des différents bouton, textbox, ...
        /// </summary>
        /// <param name="id_client">Identifiant du client actuel</param>
        public Page_Client(string id_client)
        {
            InitializeComponent();
            Valider.IsEnabled = false;
            this.id_client = id_client;
            string query = $"Select Nom_Client from cooking.client where Identifiant = \"{this.id_client}\" ;";
            List<List<string>> liste = Commandes_SQL.Select_Requete(query);
            welcome_message.Content = "Bonjour "+ liste[0][0];

            // Ajout de toutes les recettes
            List<List<string>> list_recette = new List<List<string>>();
            query = "Select Nom_Recette, Type, Prix_Vente, Descriptif from cooking.recette;";
            list_recette = Commandes_SQL.Select_Requete(query);

            for (int i = 0; i < list_recette.Count(); i++)
            {

                //déterminer nb de produit dans la recette
                query = $"select Nom_Produit, Quantite_Produit from cooking.composition_recette where Nom_Recette = \"{list_recette[i][0]}\";";
                List<List<string>> List_Produit_QT_dans_recette = Commandes_SQL.Select_Requete(query);

                int min_qt_faisable = int.MaxValue;
                for (int j = 0; j < List_Produit_QT_dans_recette.Count; j++) //pour chaque produit
                {
                    query = $"Select Stock from cooking.produit where Nom_Produit = \"{List_Produit_QT_dans_recette[j][0]}\";";
                    List<List<string>> List_Stock = Commandes_SQL.Select_Requete(query);
                    int stock_produit = Convert.ToInt32(List_Stock[0][0]);
                    int quantite_pour_recette = Convert.ToInt32(List_Produit_QT_dans_recette[j][1]);
                    int qt_faisable = stock_produit / quantite_pour_recette;
                    if (qt_faisable < min_qt_faisable) min_qt_faisable = qt_faisable;
                }

                Liste_Recette.Items.Add(new Recette_complete { Nom_Recette = list_recette[i][0], Type = list_recette[i][1], Descriptif= list_recette[i][3], Prix = list_recette[i][2], Qt_Faisable = min_qt_faisable });
            }

            

           

            string query2 = $"select CdR,Credit_Cook from client where Identifiant = \"{this.id_client}\" ;";
            liste = Commandes_SQL.Select_Requete(query2);
            if (Convert.ToInt32(liste[0][0]) == 0)
            {
                CdR.Content = "Devenir CdR";
            }
            if (Convert.ToInt32(liste[0][0]) == 1)
            {
                CdR.Content = "Page CdR";
            }
            if (Convert.ToInt32(liste[0][0]) == 2)
            {
                CdR.Content = "Client";
                CdR.IsEnabled = false;
            }

            Solde.Content = liste[0][1];

        }

        /// <summary>
        /// Méthode reliée au bouton "Ajouter" permettant de passer le montant choisi de la recette sélectionnée dans le panier. La quantité faisable pour chaque recette se met à jour après l'ajout dans le panier
        /// Le montant total de la commande se met à jour après chaque ajout
        /// </summary>
        /// <param name="sender">Bouton "Ajouter"</param>
        /// <param name="e">Evenement Click</param>
        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            Recette_complete selection = Liste_Recette.SelectedItem as Recette_complete;
            if (selection == null)
            {
                erreur.Content = "Aucune recette sélectionnée";
            }
            if(Quantité.Text=="" || !int.TryParse(Quantité.Text,out _) || Convert.ToInt32(Quantité.Text)<=0)
            {
                erreur.Content = "Quantité invalide";
            }
            else
            {
                erreur.Content = "";
                Recette_Panier recette_selectionnee = new Recette_Panier { Nom_Recette = selection.Nom_Recette, Quantite_Recette = Quantité.Text, Prix = selection.Prix };

                List<Recette_Panier> Liste_Recette_Panier = new List<Recette_Panier>();
                Liste_Recette_Panier.Add(recette_selectionnee);
                for (int i = 0; i < Panier.Items.Count; i++)
                {
                    Liste_Recette_Panier.Add(Panier.Items[i] as Recette_Panier);
                }

                //Liste_Recette_Panier contient toutes les recettes qu'on veut évaluer
                List<List<string>> Nom_Conso = new List<List<string>>();
                for (int i = 0; i < Liste_Recette_Panier.Count; i++)
                {
                    string query = $"select Nom_Produit, Quantite_Produit from cooking.composition_recette where Nom_Recette = \"{Liste_Recette_Panier[i].Nom_Recette}\";";
                    List<List<string>> List_Produit_QT_dans_recette = Commandes_SQL.Select_Requete(query);

                    for (int j = 0; j < List_Produit_QT_dans_recette.Count; j++) // pour chaque produit de cette recette
                    {
                        if (Nom_Conso.Count == 0)//premier item
                        {
                            List<string> premieritem = new List<string>();
                            premieritem.Add(List_Produit_QT_dans_recette[j][0]);
                            premieritem.Add(Convert.ToString(Convert.ToInt32(List_Produit_QT_dans_recette[j][1]) * Convert.ToInt32(Liste_Recette_Panier[i].Quantite_Recette)));
                            Nom_Conso.Add(premieritem);
                        }
                        else
                        {
                            bool copie = false;
                            for (int k = 0; k < Nom_Conso.Count; k++) //pour chaque produit déjà enregistré
                            {
                                if (Nom_Conso[k][0] == List_Produit_QT_dans_recette[j][0])
                                {
                                    copie = true;
                                    Nom_Conso[k][1] = Convert.ToString(Convert.ToInt32(Nom_Conso[k][1]) + (Convert.ToInt32(List_Produit_QT_dans_recette[j][1]) * Convert.ToInt32(Liste_Recette_Panier[i].Quantite_Recette)));
                                }
                            }

                            if (!copie)
                            {
                                List<string> nv_item = new List<string>();
                                nv_item.Add(List_Produit_QT_dans_recette[j][0]);
                                nv_item.Add(Convert.ToString(Convert.ToInt32(List_Produit_QT_dans_recette[j][1]) * Convert.ToInt32(Liste_Recette_Panier[i].Quantite_Recette)));
                                Nom_Conso.Add(nv_item);
                            }
                        }

                    }


                }

                //on récupère le vrai stock

                string query2 = "Select Nom_Produit, Stock from cooking.produit";
                List<List<string>> Nom_Stock = Commandes_SQL.Select_Requete(query2);
                bool check = true;
                for (int i = 0; i < Nom_Stock.Count; i++)
                {
                    for (int j = 0; j < Nom_Conso.Count; j++)
                    {
                        if (Nom_Stock[i][0] == Nom_Conso[j][0])
                        {
                            Nom_Stock[i][1] = Convert.ToString(Convert.ToInt32(Nom_Stock[i][1]) - Convert.ToInt32(Nom_Conso[j][1]));
                            if (Convert.ToInt32(Nom_Stock[i][1]) < 0) check = false; //pas assez de stock
                        }
                    }
                }

                if (!check)
                {
                    erreur.Content = "ERREUR pas assez de stock";
                }
                else
                {
                    Valider.IsEnabled = true; // on réactive le bouton

                    erreur.Content = "";
                    List<Recette_complete> nouvelle_table = new List<Recette_complete>();
                    for (int i = 0; i < Liste_Recette.Items.Count; i++)
                    {
                        Recette_complete recette_observee = Liste_Recette.Items[i] as Recette_complete;
                        string nom_recette = recette_observee.Nom_Recette;
                        string query = $"select Nom_Produit, Quantite_Produit from cooking.composition_recette where Nom_Recette = \"{nom_recette}\";";
                        List<List<string>> List_Produit_QT_dans_recette = Commandes_SQL.Select_Requete(query);

                        int min_qt_faisable = int.MaxValue;
                        for (int j = 0; j < List_Produit_QT_dans_recette.Count; j++) //pour chaque produit
                        {
                            int stock_produit = 0;
                            for (int k = 0; k < Nom_Stock.Count; k++)
                            {
                                if (Nom_Stock[k][0] == List_Produit_QT_dans_recette[j][0]) stock_produit = Convert.ToInt32(Nom_Stock[k][1]);
                            }

                            int quantite_pour_recette = Convert.ToInt32(List_Produit_QT_dans_recette[j][1]);
                            int qt_faisable = stock_produit / quantite_pour_recette;
                            if (qt_faisable < min_qt_faisable) min_qt_faisable = qt_faisable;
                        }

                        recette_observee.Qt_Faisable = min_qt_faisable; 
                        Recette_complete copie_recette = new Recette_complete { Nom_Recette = recette_observee.Nom_Recette, Type = recette_observee.Type, Descriptif = recette_observee.Descriptif, Prix = recette_observee.Prix, Qt_Faisable = min_qt_faisable };
                        nouvelle_table.Add(copie_recette);
    
                    }
                    
                    Liste_Recette.Items.Clear();
                    for (int i = 0; i < nouvelle_table.Count; i++)
                    {
                        Liste_Recette.Items.Add(nouvelle_table[i]);
                    }
                    int ajout = Convert.ToInt32(selection.Prix) * Convert.ToInt32(Quantité.Text);
                    Total.Content = Convert.ToString(Convert.ToInt32(Total.Content) + ajout);
                    Panier.Items.Add(new Recette_Panier { Nom_Recette = selection.Nom_Recette, Quantite_Recette = Quantité.Text, Prix = selection.Prix });
                    Quantité.Text = "";
                }

                

            }


        }

        /// <summary>
        /// Méthode reliée au bouton "Retirer" permettant de retirer une recette choisie du panier (on retire l'intégralité de la recette). La quantité faisable pour toutes les recettes se remet à jour après le retrait d'une recette du panier
        /// Le montant total se met à jour après chaque retrait
        /// </summary>
        /// <param name="sender">Bouton "Retirer"</param>
        /// <param name="e">Evenement Click</param>
        private void Retirer_Click(object sender, RoutedEventArgs e)
        {
            
            Recette_Panier selection = Panier.SelectedItem as Recette_Panier;
            if(selection== null)
            {
                erreur.Content = "Aucune recette sélectionnée";
            }
            else
            {
                erreur.Content = "";
                int retrait = Convert.ToInt32(selection.Prix) * Convert.ToInt32(selection.Quantite_Recette);
                Total.Content = Convert.ToString(Convert.ToInt32(Total.Content) - retrait);
                Panier.Items.Remove(Panier.SelectedItem);

                List<Recette_Panier> Liste_Recette_Panier = new List<Recette_Panier>();
                for (int i = 0; i < Panier.Items.Count; i++)
                {
                    Liste_Recette_Panier.Add(Panier.Items[i] as Recette_Panier);
                }

                //Liste_Recette_Panier contient toutes les recettes qu'on veut évaluer
                List<List<string>> Nom_Conso = new List<List<string>>();
                for (int i = 0; i < Liste_Recette_Panier.Count; i++)
                {
                    string query = $"select Nom_Produit, Quantite_Produit from cooking.composition_recette where Nom_Recette = \"{Liste_Recette_Panier[i].Nom_Recette}\";";
                    List<List<string>> List_Produit_QT_dans_recette = Commandes_SQL.Select_Requete(query);

                    for (int j = 0; j < List_Produit_QT_dans_recette.Count; j++) // pour chaque produit de cette recette
                    {
                        if (Nom_Conso.Count == 0)//premier item
                        {
                            List<string> premieritem = new List<string>();
                            premieritem.Add(List_Produit_QT_dans_recette[j][0]);
                            premieritem.Add(Convert.ToString(Convert.ToInt32(List_Produit_QT_dans_recette[j][1]) * Convert.ToInt32(Liste_Recette_Panier[i].Quantite_Recette)));
                            Nom_Conso.Add(premieritem);
                        }
                        else
                        {
                            bool copie = false;
                            for (int k = 0; k < Nom_Conso.Count; k++) //pour chaque produit déjà enregistré
                            {
                                if (Nom_Conso[k][0] == List_Produit_QT_dans_recette[j][0])
                                {
                                    copie = true;
                                    Nom_Conso[k][1] = Convert.ToString(Convert.ToInt32(Nom_Conso[k][1]) + (Convert.ToInt32(List_Produit_QT_dans_recette[j][1]) * Convert.ToInt32(Liste_Recette_Panier[i].Quantite_Recette)));
                                }
                            }

                            if (!copie)
                            {
                                List<string> nv_item = new List<string>();
                                nv_item.Add(List_Produit_QT_dans_recette[j][0]);
                                nv_item.Add(Convert.ToString(Convert.ToInt32(List_Produit_QT_dans_recette[j][1]) * Convert.ToInt32(Liste_Recette_Panier[i].Quantite_Recette)));
                                Nom_Conso.Add(nv_item);
                            }
                        }

                    }


                }

                //on récupère le vrai stock

                string query2 = "Select Nom_Produit, Stock from cooking.produit";
                List<List<string>> Nom_Stock = Commandes_SQL.Select_Requete(query2);
                for (int i = 0; i < Nom_Stock.Count; i++)
                {
                    for (int j = 0; j < Nom_Conso.Count; j++)
                    {
                        if (Nom_Stock[i][0] == Nom_Conso[j][0])
                        {
                            Nom_Stock[i][1] = Convert.ToString(Convert.ToInt32(Nom_Stock[i][1]) - Convert.ToInt32(Nom_Conso[j][1]));
                        }
                    }
                }

                List<Recette_complete> nouvelle_table = new List<Recette_complete>();
                for (int i = 0; i < Liste_Recette.Items.Count; i++)
                {
                    Recette_complete recette_observee = Liste_Recette.Items[i] as Recette_complete;
                    string nom_recette = recette_observee.Nom_Recette;
                    string query = $"select Nom_Produit, Quantite_Produit from cooking.composition_recette where Nom_Recette = \"{nom_recette}\";";
                    List<List<string>> List_Produit_QT_dans_recette = Commandes_SQL.Select_Requete(query);

                    int min_qt_faisable = int.MaxValue;
                    for (int j = 0; j < List_Produit_QT_dans_recette.Count; j++) //pour chaque produit
                    {
                        int stock_produit = 0;
                        for (int k = 0; k < Nom_Stock.Count; k++)
                        {
                            if (Nom_Stock[k][0] == List_Produit_QT_dans_recette[j][0]) stock_produit = Convert.ToInt32(Nom_Stock[k][1]);
                        }

                        int quantite_pour_recette = Convert.ToInt32(List_Produit_QT_dans_recette[j][1]);
                        int qt_faisable = stock_produit / quantite_pour_recette;
                        if (qt_faisable < min_qt_faisable) min_qt_faisable = qt_faisable;
                    }

                    recette_observee.Qt_Faisable = min_qt_faisable;
                    Recette_complete copie_recette = new Recette_complete { Nom_Recette = recette_observee.Nom_Recette, Type = recette_observee.Type, Descriptif = recette_observee.Descriptif, Prix = recette_observee.Prix, Qt_Faisable = min_qt_faisable };
                    nouvelle_table.Add(copie_recette);

                }

                Liste_Recette.Items.Clear();
                for (int i = 0; i < nouvelle_table.Count; i++)
                {
                    Liste_Recette.Items.Add(nouvelle_table[i]);
                }
                if (Panier.Items.Count == 0) Valider.IsEnabled = false; // on désactive le bouton
            }

            
        }

        /// <summary>
        /// Méthode reliée au bouton CdR permettant de devenir CdR si on ne l'est pas, ou accéder à la page CdR si on l'est. Inaccessible pour les utilisateurs restreints en clients
        /// </summary>
        /// <param name="sender">Bouton ""</param>
        /// <param name="e">Evenement Click</param>
        private void CdR_Click(object sender, RoutedEventArgs e)
        {
            string query = $"select CdR from client where Identifiant = \"{this.id_client}\" ;";
            List<List<string>> liste = Commandes_SQL.Select_Requete(query);
            if(Convert.ToInt32(liste[0][0]) == 0)
            {
                string query2 = $"Update cooking.client set CdR = 1 where Identifiant = \"{this.id_client}\" ;";
                string ex = Commandes_SQL.Insert_Requete(query2);
                CdR.Content = "Page CdR";
            }
            else
            {
                Page_CdR Page_CdR = new Page_CdR(this.id_client);
                this.NavigationService.Navigate(Page_CdR);
            }
        }

        /// <summary>
        /// Méthode reliée au bouton "Déconnexion" permettant de se déconnecter et retourner au menu principal
        /// </summary>
        /// <param name="sender">Bouton "Déconnexion"</param>
        /// <param name="e">Evenement Click</param>
        private void Deco_Click(object sender, RoutedEventArgs e)
        {
            Interface_Home interhome = new Interface_Home();
            this.NavigationService.Navigate(interhome);
        }

        /// <summary>
        /// Méthode reliée au bouton "Valider" permettant de valider la commande et passer au paiement. Le bouton est innaccessible quand le panier est vide
        /// </summary>
        /// <param name="sender">Bouton "Valider"</param>
        /// <param name="e">Evenement Click</param>
        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            if (Panier.Items.Count > 0) //s'il y a des choses dans le panier
            {


                List<List<string>> liste_panier = new List<List<string>>();
                for (int index_panier = 0; index_panier < Panier.Items.Count; index_panier++)
                {
                    Recette_Panier recette = Panier.Items[index_panier] as Recette_Panier;
                    List<string> liste_recette_quantite = new List<string>();
                    liste_recette_quantite.Add(recette.Nom_Recette);
                    liste_recette_quantite.Add(recette.Quantite_Recette);
                    liste_panier.Add(liste_recette_quantite);
                }


                Validation_Paiement page_validation = new Validation_Paiement(Total.Content.ToString(), Solde.Content.ToString(), this.id_client, liste_panier);
                this.NavigationService.Navigate(page_validation);
            }
            
        }

        /// <summary>
        /// Classe utilisée pour remplir la ListView des recettes disponibles
        /// </summary>
        public class Recette_complete
        {
            /// <summary>
            /// Nom de la Recette
            /// </summary>
            public string Nom_Recette { get; set; }
            /// <summary>
            /// Type de la Recette
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// Descriptif de la Recette
            /// </summary>
            public string Descriptif { get; set; }
            /// <summary>
            /// Prix de la Recette
            /// </summary>
            public string Prix { get; set; }
            /// <summary>
            /// Qt faisable de cette Recette
            /// </summary>
            public int Qt_Faisable { get; set; }
        }
        /// <summary>
        /// Classe utilisée pour remplir la ListView des recettes dans le panier
        /// </summary>
        public class Recette_Panier
        {
            /// <summary>
            /// Nom de la Recette
            /// </summary>
            public string Nom_Recette { get; set; }
            /// <summary>
            /// Quantité de la Recette choisie
            /// </summary>
            public string Quantite_Recette { get; set; }
            /// <summary>
            /// Prix de la Recette
            /// </summary>
            public string Prix { get; set; }
        }


        /// <summary>
        /// Méthode permettant d'interdire certains caractères pour les input (caratères provoquant des erreurs sur MySQL)
        /// </summary>
        /// <param name="sender">Textbox</param>
        /// <param name="e">Evenement texte modifié</param>
        private void Caractere_interdit(object sender, TextChangedEventArgs e)
        {
            TextBox id_textbox = sender as TextBox;
            if (id_textbox.Text.Contains('"') || id_textbox.Text.Contains('é') || id_textbox.Text.Contains('è')
                || id_textbox.Text.Contains('î') || id_textbox.Text.Contains('ê') || id_textbox.Text.Contains('ô')
                || id_textbox.Text.Contains('ï') || id_textbox.Text.Contains('ë') || id_textbox.Text.Contains('ç')
                || id_textbox.Text.Contains('à') || id_textbox.Text.Contains('ù'))
            {
                // on efface le dernier caractère s'il est interdit, ne laissant ainsi pas l'opportunité à l'utilisateur de l'écrire
                string a = id_textbox.Text.Remove(id_textbox.Text.Length - 1);
                id_textbox.Text = a;
                erreur.Content = "Accents/Guillemets interdits";
            }
            return;
        }
    }
}
