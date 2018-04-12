using Plugin.Toasts.UWP;
using Xamarin.Forms;

namespace StaffBusser.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init();

            LoadApplication(new StaffBusser.App());
        }
    }
}