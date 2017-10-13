using System;

namespace KDRBusser
{
    public interface IFCMLoginService 
    {
        Boolean IsLoggedIn();
        void Init();
        void UpdateToken(String Token);
        String GetEmail();
        void Createuser(String email, String password, String masterid, String UserName);
        void ToastUser(String title);
        void  LogInnUser(String email, String password);
        void LogOut();
        String GetToken();
        void LogInGoogle();
    }
}
