﻿using GameLauncher.Models;
using GameLauncher.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using GameLauncher.Views;
using IWshRuntimeLibrary;

namespace GameLauncher
{
    public partial class AddGames : Page
    {
        private LoadAllGames lag = new LoadAllGames();

        public AddGames()
        {
            InitializeComponent();
        }

        private void AddGame_OnClick(object sender, RoutedEventArgs e)
        {
            //This part repairs the link so it launches properly
            string ngl = NewGameLink.Text;
            if (!ngl.Contains("http") && (ngl != ""))
            {
                UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "http";
                uriBuilder.Host = NewGameLink.Text;
                Uri uri = uriBuilder.Uri;
                NewGameLink.Text = uri.ToString();
            }
            //Write all the fields to the text file
            try
            {
                //string NewGameIconRelative = NewGameIcon.Text.Trim();
                //Console.WriteLine(NewGameIconRelative);
                TextWriter tsw = new StreamWriter(@"./Resources/GamesList.txt", true);
                Guid gameGuid = Guid.NewGuid();
                tsw.WriteLine(NewGameTitle.Text + "|" +
                              NewGameGenre.Text + "|" +
                              NewGamePath.Text + "|" +
                              NewGameLink.Text + "|" +
                              NewGameIcon.Text + "|" +
                              NewGamePoster.Text + "|" +
                              NewGameBanner.Text + "|" +
                              gameGuid);
                tsw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            clearFields();
            ((MainWindow)Application.Current.MainWindow)?.RefreshGames();
            ClearGenreBoxes();
            AddGameDialog.IsOpen = false;
        }

        private void CancelAddGame_OnClick(object sender, RoutedEventArgs e)
        {
            ClearGenreBoxes();
            AddGameDialog.IsOpen = false;
            clearFields();
        }

        private void clearFields()
        {
            NewGameTitle.Text = "";
            NewGamePath.Text = "";
            NewGameGenre.Text = "";
            NewGameLink.Text = "";
            NewGameIcon.Text = "";
            NewGamePoster.Text = "";
            NewGameBanner.Text = "";
        }

        private void AddGenre_OnClick(object sender, RoutedEventArgs e)
        {
            string genrePlaceHolder = null;
            //Check the itemscontrol and for each checked item, add it to the list
            for (int i = 0; i < GenreAGList.Items.Count; i++)
            {
                ContentPresenter c = (ContentPresenter)GenreAGList.ItemContainerGenerator.ContainerFromItem(GenreAGList.Items[i]);
                CheckBox cb = c.ContentTemplate.FindName("genreCheckBox", c) as CheckBox;
                if (cb.IsChecked.Value)
                {
                    genrePlaceHolder += cb.Content.ToString() + ";";
                }
            }
            NewGameGenre.Text = genrePlaceHolder;
            return;
        }

        private void ClearGenreSelection_OnClick(object sender, RoutedEventArgs e)
        {
            ClearGenreBoxes();
        }

        private void ClearGenreBoxes()
        {
            for (int i = 0; i < GenreAGList.Items.Count; i++)
            {
                ContentPresenter c = (ContentPresenter)GenreAGList.ItemContainerGenerator.ContainerFromItem(GenreAGList.Items[i]);
                if (c != null)
                {
                    CheckBox cb = c.ContentTemplate.FindName("genreCheckBox", c) as CheckBox;
                    if (cb.IsChecked.Value)
                    {
                        cb.IsChecked = false;
                    }
                }
            }
        }

        private void AttachLauncher_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.Filter = "Executable Files (*.exe) | *.exe;*.lnk;*.url";
            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == true && NewGameTitle.Text != "")
            {
                CreateShortcut(fileDialog.FileName);
                string installPath = AppDomain.CurrentDomain.BaseDirectory;
                installPath = installPath.Replace("\\", "/");
                string ngNewShortcut = installPath + "Resources/shortcuts/" + NewGameTitle.Text + ".lnk";
                NewGamePath.Text = ngNewShortcut;
            }
            else if (dialogResult == true && NewGameTitle.Text == "")
            {
                MessageBox.Show("Please enter a game title first.");
            }
        }

        private void AttachIcon_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.Filter = "Images (*.jpg;*.png;*.bmp | *.jpg;*.png;*.bmp";
            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == true && NewGameTitle.Text != "")
            {
                string installPath = AppDomain.CurrentDomain.BaseDirectory;
                installPath = installPath.Replace("\\", "/");
                string ngIconFile = fileDialog.FileName;
                if (System.IO.File.Exists(@"./Resources/img/" + NewGameTitle.Text + "-icon.png"))
                {
                    MessageBox.Show("A game with that name already exists");
                }
                else
                {
                    System.IO.File.Copy(ngIconFile, @"./Resources/img/" + NewGameTitle.Text + "-icon.png");
                    NewGameIcon.Text = installPath + "Resources/img/" + NewGameTitle.Text + "-icon.png";
                }
            }
            else if (dialogResult == true && NewGameTitle.Text == "")
            {
                MessageBox.Show("Please enter a game title first.");
            }
        }

        private void AttachPoster_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.Filter = "Images (*.jpg;*.png;*.bmp | *.jpg;*.png;*.bmp";
            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == true && NewGameTitle.Text != "")
            {
                string installPath = AppDomain.CurrentDomain.BaseDirectory;
                installPath = installPath.Replace("\\", "/");
                string ngPosterFile = fileDialog.FileName;
                if (System.IO.File.Exists(@"./Resources/img/" + NewGameTitle.Text + "-poster.png"))
                {
                    MessageBox.Show("A game with that name already exists");
                }
                else
                {
                    System.IO.File.Copy(ngPosterFile, @"./Resources/img/" + NewGameTitle.Text + "-poster.png");
                    NewGamePoster.Text = installPath + "Resources/img/" + NewGameTitle.Text + "-poster.png";
                }
            }
            else if (dialogResult == true && NewGameTitle.Text == "")
            {
                MessageBox.Show("Please enter a game title first.");
            }
        }

        private void AttachBanner_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Multiselect = false;
            fileDialog.Filter = "Images (*.jpg;*.png;*.bmp | *.jpg;*.png;*.bmp";
            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == true && NewGameTitle.Text != "")
            {
                string installPath = AppDomain.CurrentDomain.BaseDirectory;
                installPath = installPath.Replace("\\", "/");
                string ngBannerFile = fileDialog.FileName;
                if (System.IO.File.Exists(@"./Resources/img/" + NewGameTitle.Text + "-banner.png"))
                {
                    MessageBox.Show("A game with that name already exists");
                }
                else if (dialogResult == true && NewGameTitle.Text == "")
                {
                    System.IO.File.Copy(ngBannerFile, @"./Resources/img/" + NewGameTitle.Text + "-banner.png");
                    NewGameBanner.Text = installPath + "Resources/img/" + NewGameTitle.Text + "-banner.png";
                }
            }
            else
            {
                MessageBox.Show("Please enter a game title first.");
            }
        }

        private void CreateShortcut(string linkname)
        {
            string installPath = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(installPath + "\\Resources\\shortcuts"))
            {
                System.IO.Directory.CreateDirectory(installPath + "\\Resources\\shortcuts");
            }
            //create shortcut from linkname, place shortut in dir
            WshShell wsh = new WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
                installPath + "\\Resources\\shortcuts" + "\\" + NewGameTitle.Text + ".lnk") as IWshRuntimeLibrary.IWshShortcut;
            shortcut.Arguments = "";
            shortcut.TargetPath = linkname;
            shortcut.WindowStyle = 1;
            shortcut.Description = "Shortcut to " + NewGameTitle.Text;
            shortcut.WorkingDirectory = "C:\\App";
            shortcut.IconLocation = linkname;
            shortcut.Save();
        }
    }
}