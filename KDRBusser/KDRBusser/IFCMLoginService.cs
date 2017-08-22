using System;

namespace KDRBusser
{
    public interface IFCMLoginService 
    {
        Boolean IsLoggedIn();
        void Createuser(String email, String password);
         void ToastUser(String title);
        void  LogInnUser(String email, String password);
        String GetToken();
    }
}
