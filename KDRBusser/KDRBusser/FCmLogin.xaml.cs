using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KDRBusser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FCmLogin : ContentPage
    {
        
        Entry Email, Password;

       
        public FCmLogin()
        {
            InitializeComponent();
            this.Title = "Sample Weather App";
            btnLogin.Clicked += BtnLogin_Clicked;
            btncreateUser.Clicked += BtncreateUser_clicked;
            
        }

        private void BtncreateUser_clicked(object sender, EventArgs e)
        {
            Email = emailEntry;
            Password = passwordEntry;
            DependencyService.Get<IFCMLoginService>().Createuser(Email.Text, Password.Text);
        }

        private void BtnLogin_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IFCMLoginService>().LogInnUser(Email.Text, Password.Text);
        }

        private void CreateUser()
        {
            

        }




    }




}