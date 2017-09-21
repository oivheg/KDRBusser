﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KDRBusser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FCmLogin : ContentPage
    {
        
        Entry Email, Password, MasterId, UserName;

        public String GetMasterID()
        {
            return MasterId.Text;
        }
       
        public FCmLogin()
        {
            InitializeComponent();
            this.Title = "KDRBusser";
            btnLogin.Clicked += BtnLogin_Clicked;
            btncreateUser.Clicked += BtncreateUser_clicked;
  }

        Boolean IsCreating = false;
        private void BtncreateUser_clicked(object sender, EventArgs e)
        {
            if (IsCreating)
            {
                Email = emailEntry;
                Password = passwordEntry;
                MasterId = masterEntry;
                DependencyService.Get<IFCMLoginService>().Createuser(Email.Text, Password.Text, MasterId.Text);
            }
            else
            {
                userNameEntry.IsVisible = true;
                masterEntry.IsVisible = true;
                //show the MasterID Field,
                IsCreating = true;
            }
            
        }

        private void BtnLogin_Clicked(object sender, EventArgs e)
        {
            Email = emailEntry;
            Password = passwordEntry;
            DependencyService.Get<IFCMLoginService>().LogInnUser(Email.Text, Password.Text);
        }

      




    }




}