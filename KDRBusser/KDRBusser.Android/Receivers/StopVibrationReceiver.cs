using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KDRBusser.Droid;

namespace KDRBusser.Droid.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "CancelVibrations" })]
  public  class StopVibrationReceiver : BroadcastReceiver
    {
        
      
        
        
        public override void OnReceive(Context context, Intent intent)
        {
            // Do stuff here when device reboots.
         

            Droid_MyFirebaseMessagingService.timer.Stop();
        }
    }
}