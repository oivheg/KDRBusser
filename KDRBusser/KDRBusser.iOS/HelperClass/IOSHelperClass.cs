using Acr.UserDialogs;
using StaffBusser.iOS.HelperClass;
using StaffBusser.SharedCode;

[assembly: Xamarin.Forms.Dependency(typeof(IOSHelperClass))]

namespace StaffBusser.iOS.HelperClass
{
    public class IOSHelperClass : IHelperClass
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
            // UserDialogs.Instance.AlertAsync("User Already Excist", "Thankyou");
        }

        public void IsAlert()
        {
            throw new System.NotImplementedException();
        }

        public void UploadImage()
        {
            throw new System.NotImplementedException();
        }
    }
}