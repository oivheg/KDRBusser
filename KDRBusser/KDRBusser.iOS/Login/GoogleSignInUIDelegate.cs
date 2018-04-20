using Foundation;
using Google.SignIn;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace StaffBusser.iOS.Login
{
    internal class GoogleSignInUIDelegate : SignInUIDelegate, ISignInDelegate
    {
        public override void WillDispatch(SignIn signIn, NSError error)
        {
            var sign = signIn;
            NSError err = error;
        }

        public override void PresentViewController(SignIn signIn, UIViewController viewController)
        {
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(viewController, true, null);
        }

        public override void DismissViewController(SignIn signIn, UIViewController viewController)
        {
            UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null);
        }

        public void DidSignIn(SignIn signIn, GoogleUser user, NSError error)
        {
            throw new NotImplementedException();
        }
    }
}