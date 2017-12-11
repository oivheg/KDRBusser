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
        public async void CreateuserAsync(string email, string password, string masterid, string UserName)
        {
            try
            {
                Classes.User newUser = new Classes.User
                {
                    Email = email,
                    UserName = UserName.Trim(),
                    MasterKey = masterid.Trim(),
                    Appid = "not initialised".Trim(),
                    Active = false
                };
                await RestApiCommunication.Post(newUser, "CreateUser");
                   auth.CreateUser(email, password, CreateOnCompletion);
                
                
                SharedHelper.ToastedUserAsync("FCM User Created");

            }
            catch (Exception ex)
            {
                // Sign-up failed, display a message to the user
                // If sign in succeeds, the AuthState event handler will
                //  be notified and logic to handle the signed in user can happen there
                SharedHelper.ToastedUserAsync("user Creation Failed", ex.ToString());
                //ToastedUserAsync("Create user failed" + ex);
            }
        }

        public string GetEmail()
        {
            return auth.CurrentUser.Email;
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
            DependencyService.Get<IHelperClass>().IsLoading(true, "Logger INN");
            auth = Auth.DefaultInstance;
            var listenerHandle = Auth.DefaultInstance.AddAuthStateDidChangeListener(async (auth, user) => {
               
                if (user != null)
                {
                    //FirebaseApp.InitializeApp(this);
                    // User is signed in
                    //ToastedUserAsync("onAuthStateChanged:signed_in:" + user.Uid);
                    App.IsUserLoggedIn = true;
                    await UpdateUserToken();
                    ChangeActivity();
                }
                else
                {
                    // User is signed out
                    //ToastedUserAsync("onAuthStateChanged:signed_out");
                    App.IsUserLoggedIn = false;

                    Xamarin.Forms.Application.Current.MainPage = new FCmLogin();
                }
            });
        
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

        private void CreateOnCompletion(Firebase.Auth.User user, NSError error)
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
                        // Print error
                        break;
                }
            }
            else
            {
                UpdateUserToken();
                ChangeActivity();
                // Do your magic to handle authentication result
            }
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

        public void Createuser(string email, string password, string masterid, string UserName)
        {
            DependencyService.Get<IHelperClass>().IsLoading(true,"Creating User");
            CreateuserAsync(email, password, masterid, UserName);
        }

        String FCMToken;
       
    }




}