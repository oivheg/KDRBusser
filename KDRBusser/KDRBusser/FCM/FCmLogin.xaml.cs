using KDRBusser.SharedCode;
using ProgressRingControl.Forms.Plugin;
using System;

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
            btnGoogle.Clicked += BtnGoogle_Clicked;
           
     
        }

        private void BtnGoogle_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IFCMLoginService>().LogInGoogle();
        }

        Boolean IsCreating = false;
       
        private void BtncreateUser_clicked(object sender, EventArgs e)
        {
            if (IsCreating)
            {
                Email = emailEntry;
                Password = passwordEntry;
                MasterId = masterEntry;
                UserName = userNameEntry;
                DependencyService.Get<IFCMLoginService>().Createuser(Email.Text, Password.Text, MasterId.Text, UserName.Text);
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

        private void BtnLogin_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IHelperClass>().IsLoading(true, "Loading");
           Email = emailEntry;
            Password = passwordEntry;
            DependencyService.Get<IFCMLoginService>().LogInnUser(Email.Text, Password.Text);
        }
    }
    
}