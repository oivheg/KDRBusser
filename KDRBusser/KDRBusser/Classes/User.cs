﻿using System;
using Xamarin.Forms.Internals;

namespace StaffBusser.Classes
{
    [Preserve(AllMembers = true)]
    public class User
    {
        public int UserId { get; set; }
        public String UserName { get; set; }
        public String Appid { get; set; }
        public bool Active { get; set; }
        public String Email { get; set; }
        public String MasterKey { get; set; }

        public User()
        {
        }

        public User(String _UserName, String _Appid, bool _Active)
        {
            UserId = 0;
            UserName = _UserName;
            Appid = _Appid;
            Active = _Active;
        }
    }
}