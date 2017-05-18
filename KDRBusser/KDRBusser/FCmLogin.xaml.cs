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
        public FCmLogin()
        {
            InitializeComponent();
            this.Title = "Sample Weather App";
            btnLogin.Clicked += BtnLogin_Clicked;
            btncreateUser.Clicked += BtncreateUser_clicked;
        }

        private void BtncreateUser_clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnLogin_Clicked(object sender, EventArgs e)
        {
           fcm
        }

        private async Task CreateUserAsync()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    // to ios stuff
                    break;
                case Device.Android:
                    //android stuff:
                   
                    break;
                case Device.Windows:
                    //windows stuff
                    break;
            }

        }

        


    }




}