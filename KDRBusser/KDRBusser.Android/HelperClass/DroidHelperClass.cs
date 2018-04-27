using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Firebase.Storage;
using StaffBusser.Droid.HelperClass;
using StaffBusser.SharedCode;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(DroidHelperClass))]

namespace StaffBusser.Droid.HelperClass
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

        public readonly static DroidHelperClass Default = new DroidHelperClass();

        public string GetToken { get; set; } // add setting properties as you wish

        public void IsAlert()
        {
            //UserDialogs.Instance.ShowLoading("User Already Excist", MaskType.Black);
            UserDialogs.Instance.Alert("User Already Excist", "Thankyou");
        }

        public void DebugMEssage(string text)
        {
            System.Console.WriteLine("text");
        }

        private const int Pick_image_request = 71;

        public void UploadImage()
        {
            //var storage = FirebaseStorage.Instance;
            //var storageRef = storage.GetReference("gs://staff-busser.appspot.com");
            //var spaceRef = storageRef.Child("Jens/profile.jpg");

            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            ((Activity)Forms.Context).StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), Pick_image_request);
        }
    }
}