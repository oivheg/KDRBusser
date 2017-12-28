using Android.App;
using Android.Content;

namespace KDRBusser.Droid.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "CancelVibrations" })]
    public class StopVibrationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Do stuff here when device reboots.

            Droid_MyFirebaseMessagingService.timer.Stop();
        }
    }
}