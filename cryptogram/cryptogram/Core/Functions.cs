using System;
using System.Collections.Generic;
using System.Text;

namespace cryptogram.Core
{
  static class Functions
  {


    /// <summary>
    /// Return a RSA of current user
    /// </summary>
    /// <returns></returns>
    public static System.Security.Cryptography.RSACryptoServiceProvider GetMyRSA()
    {
      var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
      RSA.ImportCspBlob((byte[])Xamarin.Forms.Application.Current.Properties["keys"]);
      return RSA;
    }

    /// <summary>
    /// Return the public key of current user in base64 format
    /// </summary>
    /// <returns></returns>
    public static string GetMyPublicKey()
    {
      System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
      RSA.ImportCspBlob((byte[])Xamarin.Forms.Application.Current.Properties["keys"]);
      return Convert.ToBase64String(RSA.ExportCspBlob(false));
    }

    public static void Alert(string Message)
    {
      Xamarin.Forms.Page currPage;
      if (Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
      {
        int index = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count - 1;
        currPage = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack[index];
        currPage.DisplayAlert(Resources.Dictionary.Alert, Message, Resources.Dictionary.Ok);
      }
    }

  }
}
