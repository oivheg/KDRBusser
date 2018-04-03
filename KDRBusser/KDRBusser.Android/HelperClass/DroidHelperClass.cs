using Acr.UserDialogs;
using Android.Support.V7.App;
using KDRBusser.Droid.HelperClass;
using KDRBusser.SharedCode;

[assembly: Xamarin.Forms.Dependency(typeof(DroidHelperClass))]

namespace KDRBusser.Droid.HelperClass
{
    public class DroidHelperClass : IHelperClass
    {
        public void IsLoading(bool isLoading, string text = "")
        {
            if (!isLoading)
            {
                UserDialogs.Instance.HideLoading();
            }
            else
            {
                UserDialogs.Instance.ShowLoading(text, MaskType.Black);
            }
        }

        public void isAlert()
        {
            //UserDialogs.Instance.ShowLoading("User Already Excist", MaskType.Black);
            UserDialogs.Instance.Alert("User Already Excist", "Thankyou");
        }
    }
}