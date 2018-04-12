﻿using StaffBusser.Classes;
using StaffBusser.Communication;
using Plugin.Toasts;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StaffBusser.SharedCode
{
    public class SharedHelper
    {
        public static async Task InformmasterAsync()
        {
            User user = new User
            {
                Appid = DependencyService.Get<IFCMLoginService>().GetToken()
            };
            await RestApiCommunication.Post(user, "Msgreceived");
        }

        public static async void ToastedUserAsync(String title, String content = "Default Content")
        {
            var options = new NotificationOptions()
            {
                Title = title,
                Description = content,
                IsClickable = false // Set to true if you want the result Clicked to come back (if the user clicks it)
            };
            var notification = DependencyService.Get<IToastNotificator>();
            var result = await notification.Notify(options);
        }

        //public void IsLoading(bool isLoading, string text = "")
        //{
        //    if (!isLoading)
        //    {
        //        UserDialogs.Instance.HideLoading();
        //    }
        //    else
        //    {
        //        UserDialogs.Instance.ShowLoading(text, MaskType.Black);

        //    }
        //}

        public static async Task UpdateUserTokenAsync(String _email, String _appid, Boolean logout = false)
        {
            User user = new User
            {
                Email = _email,
                Appid = _appid
            };
            if (logout)
            {
                user.Appid = "logged Out";
            }
            await RestApiCommunication.Post(user, "UpdatUser");
        }
    }
}