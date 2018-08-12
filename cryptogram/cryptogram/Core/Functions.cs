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
      RSA.ImportCspBlob(Convert.FromBase64String(MyPrivateKey));
      return RSA;
    }

    /// <summary>
    /// Return the public key of current user in base64 format
    /// </summary>
    /// <returns></returns>
    public static string GetMyPublicKey()
    {
      return Convert.ToBase64String(GetMyRSA().ExportCspBlob(false));
    }

    private static string _MyPrivateKey;
    /// <summary>
    /// Return the private key stored in the device,if not present, it generates one
    /// </summary>
    /// <returns></returns>
    public static string MyPrivateKey
    {
      get {
        if (string.IsNullOrEmpty(_MyPrivateKey))
          _MyPrivateKey = (string)Storage.LoadObject(typeof(string), "MyPublicKey");
        if (string.IsNullOrEmpty(_MyPrivateKey)) {
          _MyPrivateKey = Convert.ToBase64String(new System.Security.Cryptography.RSACryptoServiceProvider().ExportCspBlob(true));
          MyPrivateKey = _MyPrivateKey; //Save
        }
        return _MyPrivateKey;
      }
      set {
        if (_MyPrivateKey != value)
        {
          try
          {
            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
            RSA.ImportCspBlob(Convert.FromBase64String(value));
            _MyPrivateKey = value;
            Storage.SaveObject(_MyPrivateKey, "MyPublicKey");
          }
          catch (Exception)
          {
            Alert(Resources.Dictionary.InvalidKey);
            throw;
          }
        }

      }
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
