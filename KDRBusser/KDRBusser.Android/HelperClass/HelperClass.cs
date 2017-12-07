using Acr.UserDialogs;
using KDRBusser.Classes;
using KDRBusser.Communication;
using KDRBusser.Droid.HelperClass;
using KDRBusser.SharedCode;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(HelperClass))]
namespace KDRBusser.Droid.HelperClass
{
  
   public  class HelperClass : IHelperClass
    {

        public HelperClass()
        {

        }
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

        
    }
}