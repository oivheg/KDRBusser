using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Firebase.Auth;
using Xamarin.Forms;
using KDRBusser.iOS.FCM;
using KDRBusser.SharedCode;
using Firebase.InstanceID;
using KDRBusser.Classes;
using KDRBusser.Communication;

[assembly: Dependency(typeof(IOS_FCMLoginService))]
namespace KDRBusser.iOS.FCM
{
   
    class IOS_FCMLoginService : IFCMLoginService
    {

        Auth auth;
        public void Createuser(string email, string password, string masterid, string UserName)
        {
            throw new NotImplementedException();
        }

        public string GetEmail()
        {
            throw new NotImplementedException();
        }

        public string GetToken()
        {
            String tkn = InstanceId.SharedInstance.Token;
            var mauttoken = auth.CurrentUser.Uid;

            var refreshedToken = InstanceId.SharedInstance.Token;

            if (refreshedToken == null)
            {

                LogOut();
                //mAuth.SignOut();

            }
            return refreshedToken;
          
        }

        public void Init()
        {
            auth = Auth.DefaultInstance;
            //throw new NotImplementedException();
        }

        public void IsLoading(bool IsLoading, string text = "")
        {
            throw new NotImplementedException();
        }

        public void LogInGoogle()
        {
            throw new NotImplementedException();
        }

        public void LogInnUser(string email, string password)
        {
            //throw new NotImplementedException();
            auth.SignIn(email, password, SignInOnCompletion);
            
        }

        private void SignInOnCompletion(Firebase.Auth.User user, NSError error)
        {
            // stop animation 
            //indicatorView.StopAnimating();

            if (error != null)
            {
               var NSERROR =  error.UserInfo;
                AuthErrorCode errorCode;
                if (IntPtr.Size == 8) // 64 bits devices
                    errorCode = (AuthErrorCode)((long)error.Code);
                else // 32 bits devices
                    errorCode = (AuthErrorCode)((int)error.Code);

                // Posible error codes that SignIn method with email and password could throw
                // Visit https://firebase.google.com/docs/auth/ios/errors for more information
                switch (errorCode)
                {
                    case AuthErrorCode.OperationNotAllowed:
                    case AuthErrorCode.InvalidEmail:
                    case AuthErrorCode.UserDisabled:
                    case AuthErrorCode.WrongPassword:
                    default:
                        //show some irro message etc
                        //AppDelegate.ShowMessage("Could not login!", error.LocalizedDescription, NavigationController);
                       
                        DependencyService.Get<IHelperClass>().IsLoading(false, "Loading");
                        break;
                }

                return;
            }
            // start ActiveUser Activity
            //NavigationController.PushViewController(new UserViewController("Firebase"), true);
            UpdateUserToken();
            ChangeActivity();
        }



        private static void ChangeActivity()
        {
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ActiveUser());
        }

        public void LogOut()
        {

            auth.SignOut(out NSError error);
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new FCmLogin());
        }

        public void ToastUser(string title)
        {
            throw new NotImplementedException();
        }

        public void UpdateToken(string Token)
        {
            FCMToken = Token;
            UpdateUserToken();
            
        }

        private async System.Threading.Tasks.Task UpdateUserToken()
        {
            Classes.User user = new Classes.User
            {
                Email = auth.CurrentUser.Email,
                Appid = GetToken()
            };
            await RestApiCommunication.Post(user, "UpdatUser");
        }

        String FCMToken;
        //public void IsLoading(bool isLoading, String text)
        //{
        //    if (!isLoading)
        //    {
        //        UserDialogs.Instance.HideLoading();
        //    }
        //    else
        //    {

        //        UserDialogs.Instance.ShowLoading(text, MaskType.Black);


        //    }


        //}
    }
}