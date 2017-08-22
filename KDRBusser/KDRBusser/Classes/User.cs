using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDRBusser.Classes
{
    class User
    {
        public int UserId { get; set; }
        public String UserName { get; set; }
        public String Appid { get; set; }
        public bool Active { get; set; }


        public User()
        {
           

        }

        public User( String _UserName,String _Appid, bool _Active)
        {
            UserId = 0;
            UserName = _UserName;
            Appid = _Appid;
            Active = _Active;

        }

      
    }

    
}
