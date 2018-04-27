using System;

namespace StaffBusser.SharedCode
{
    public interface IHelperClass
    {
        void IsLoading(bool isLoading, String text = "");

        void IsAlert();

        void UploadImage();
    }
}