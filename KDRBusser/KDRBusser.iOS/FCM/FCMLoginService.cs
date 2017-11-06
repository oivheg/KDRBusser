using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Firebase.Auth;
using Xamarin.Forms;

namespace KDRBusser.iOS.FCM
{
    class FCMLoginService : IFCMLoginService
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
            
            throw new NotImplementedException();
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

        private void SignInOnCompletion(User user, NSError error)
        {
            // stop animation 
            //indicatorView.StopAnimating();

            if (error != null)
            {
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
                        break;
                }

                return;
            }
            // start ActiveUser Activity
            //NavigationController.PushViewController(new UserViewController("Firebase"), true);
            ChangeActivity();
        }



        private static void ChangeActivity()
        {
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ActiveUser());
        }

        public void LogOut()
        {
           
            auth.SignOut();
        }

        public void ToastUser(string title)
        {
            throw new NotImplementedException();
        }

        public void UpdateToken(string Token)
        {
            throw new NotImplementedException();
        }

        public void IsLoading(bool isLoading, String text)
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