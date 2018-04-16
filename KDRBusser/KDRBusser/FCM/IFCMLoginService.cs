using System;

namespace StaffBusser
{
    public interface IFCMLoginService
    {
        //Boolean IsLoggedIn();

        void Init();

        void UpdateTokenAsync();

        String GetEmail();

        void Createuser(String email, String password, String masterid, String UserName);

        //void ToastUser(String title);

        void LogInnUser(String email, String password);

        void LogOut();

        void CancelVIbrations();

        String GetToken();

        void LogInGoogle();
    }
}