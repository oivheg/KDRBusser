using Firebase.CloudMessaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace KDRBusser.iOS.FCM
{
    class IOS_MyFirebaseMessagingService : IMessagingDelegate
    {
        public IntPtr Handle => throw new NotImplementedException();

        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
