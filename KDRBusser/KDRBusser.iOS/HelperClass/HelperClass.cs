using Acr.UserDialogs;
using KDRBusser.iOS.HelperClass;
using KDRBusser.SharedCode;
using System;
using System.Collections.Generic;
using System.Text;
[assembly: Xamarin.Forms.Dependency(typeof(HelperClass))]
namespace KDRBusser.iOS.HelperClass
{
    public class HelperClass : IHelperClass
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
