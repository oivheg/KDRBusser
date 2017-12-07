using KDRBusser.Classes;
using KDRBusser.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KDRBusser.SharedCode
{
    public class SharedHelper
    {

        public async Task InformmasterAsync()
        {
            User user = new User
            {
                Appid = DependencyService.Get<IFCMLoginService>().GetToken()
            };
            await RestApiCommunication.Post(user, "Msgreceived");

        }
    }
}
