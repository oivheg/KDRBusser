using System;

namespace KDRBusser
{
    public interface IFCMLoginService 
    {
        Boolean IsLoggedIn();
        void Init();
        void UpdateToken();
        void Createuser(String email, String password);
        void ToastUser(String title);
        void  LogInnUser(String email, String password);
        void LogOut();
        String GetToken();
    }
}
