﻿using StaffBusser.SharedCode;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StaffBusser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FCmLogin : ContentPage
    {
        private Entry Email, Password, MasterId, UserName;

        public String GetMasterID()
        {
            return MasterId.Text;
        }

        public FCmLogin()
        {
            InitializeComponent();
            this.Title = "StaffBusser";
            btnLogin.Clicked += BtnLogin_Clicked;
            btncreateUser.Clicked += BtncreateUser_clicked;
            btnGoogle.Clicked += BtnGoogle_Clicked;
        }

        private void BtnGoogle_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IFCMLoginService>().LogInGoogle();
        }

        public void Notifyuser(String text)
        {
        }

        private Boolean IsCreating = false;

        private void BtncreateUser_clicked(object sender, EventArgs e)
        {
            if (IsCreating)
            {
                Entry[] entrylist = { emailEntry, passwordEntry, masterEntry, userNameEntry };
                Boolean isValid = CheckTextBox(entrylist);
                if (!isValid)
                {
                    App.Current.MainPage.DisplayAlert("Noen Felt er tommer", "Vennligst fyll alel felt", "OK");
                }
                else
                {
                    Email = emailEntry;
                    Password = passwordEntry;
                    MasterId = masterEntry;
                    UserName = userNameEntry;
                    DependencyService.Get<IFCMLoginService>().Createuser(Email.Text.Trim(), Password.Text, MasterId.Text, UserName.Text);
                }
            }
            else
            {
                // hides the login button and shows the entrys needed for creating a user.
                btnLogin.IsVisible = false;
                userNameEntry.IsVisible = true;
                masterEntry.IsVisible = true;
                IsCreating = true;
                btncreateUser.Text = "Lagre og Logg inn";
            }
        }

        private Boolean CheckTextBox(Entry[] tb)
        {
            foreach (var item in tb)
            {
                if (string.IsNullOrEmpty(item.Text))
                {
                    return false;
                }
            }
            return true;
        }

        private void BtnLogin_Clicked(object sender, EventArgs e)
        {
            Entry[] entrylist = { emailEntry, passwordEntry };
            Boolean isValid = CheckTextBox(entrylist);
            if (!isValid)
            {
                App.Current.MainPage.DisplayAlert("Noen Felt er tommer", "Vennligst fyll alel felt", "OK");
            }
            else
            {
                DependencyService.Get<IHelperClass>().IsLoading(true, "Loading");
                Email = emailEntry;
                Password = passwordEntry;
                DependencyService.Get<IFCMLoginService>().LogInnUser(Email.Text.Trim(), Password.Text);
            }
        }
    }
}