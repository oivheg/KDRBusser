using Firebase.Auth;
using Firebase.InstanceID;
using Foundation;
using KDRBusser.Communication;
using KDRBusser.iOS.FCM;
using KDRBusser.SharedCode;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(IOS_FCMLoginService))]

namespace KDRBusser.iOS.FCM
{
    internal class IOS_FCMLoginService : IFCMLoginService
    {
        private Auth auth;

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
                auth.CreateUser(email, password, CreateOnCompletionAsync);

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
                auth.SignOut(out NSError error);
                //mAuth.SignOut();
            }
            return refreshedToken;
        }

        public void Init()
        {
            DependencyService.Get<IHelperClass>().IsLoading(true, "Logger INN");
            auth = Auth.DefaultInstance;
            var listenerHandle = Auth.DefaultInstance.AddAuthStateDidChangeListener(async (auth, user) =>
            {
                if (user != null)
                {
                    //FirebaseApp.InitializeApp(this);
                    // User is signed in
                    //ToastedUserAsync("onAuthStateChanged:signed_in:" + user.Uid);
                    App.IsUserLoggedIn = true;
                    await SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken());
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

            auth.SignIn(email, password, SignInOnCompletionAsync);
        }

        private async void CreateOnCompletionAsync(Firebase.Auth.User user, NSError error)
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
                await SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken());
                //ChangeActivity();
                // Do your magic to handle authentication result
            }
        }

        private async void SignInOnCompletionAsync(Firebase.Auth.User user, NSError error)
        {
            // stop animation
            //indicatorView.StopAnimating();

            if (error != null)
            {
                var NSERROR = error.UserInfo;
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
            await SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken());
            ChangeActivity();
        }

        private static void ChangeActivity()
        {
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ActiveUser());
        }

        public async void LogOut()
        {
            await SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken(), true);
            auth.SignOut(out NSError error);
            //Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new FCmLogin()); not used, done in authstate listener.
        }

        public void LogoutFirebase()
        {
            auth.SignOut(out NSError error);
        }

        public void ToastUser(string title)
        {
            throw new NotImplementedException();
        }

        public void UpdateToken(string Token)
        {
            FCMToken = Token;
            SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken());
        }

        //private async System.Threading.Tasks.Task UpdateUserToken()
        //{
        //    Classes.User user = new Classes.User
        //    {
        //        Email = auth.CurrentUser.Email,
        //        Appid = GetToken()
        //    };
        //    await RestApiCommunication.Post(user, "UpdatUser");
        //}

        public void Createuser(string email, string password, string masterid, string UserName)
        {
            DependencyService.Get<IHelperClass>().IsLoading(true, "Creating User");
            CreateuserAsync(email, password, masterid, UserName);
        }

        private String FCMToken;
    }
}