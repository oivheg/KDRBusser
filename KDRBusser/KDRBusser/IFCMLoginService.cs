using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDRBusser
{
    public interface IFCMLoginService
    {
        void Init();
        void Createuser(String email, String password);
         void ToastUser(String title);
        void  LogInnUser(String email, String password);
    }
}
