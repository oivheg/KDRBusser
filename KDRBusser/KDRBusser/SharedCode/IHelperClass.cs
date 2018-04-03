using System;

namespace KDRBusser.SharedCode
{
    public interface IHelperClass
    {
        void IsLoading(bool isLoading, String text = "");

        void isAlert();
    }
}