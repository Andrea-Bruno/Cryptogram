using System;
using System.Collections.Generic;
using System.Text;
using cryptogram.Resources;
namespace cryptogram.Core
{
  public static class Messaging
  {
    private static System.Security.Cryptography.RSACryptoServiceProvider RSA;
    public static void SetRSA(byte[] Keys)
    {
      System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
      RSA.ImportCspBlob(Keys);
    }
    private static byte[] _RecipientPublicKey;

    public static byte[] RecipientPublicKey
    {
      get { return _RecipientPublicKey; }
      set { _RecipientPublicKey = value; }
    }

    public static string RecipientPublicKeyBase64
    {
      get { return Convert.ToBase64String(_RecipientPublicKey); }
      set
      {
        try
        {
          _RecipientPublicKey = Convert.FromBase64String(value);
        }
        catch (Exception)
        {
          Xamarin.Forms.Page currPage;
          if (Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
          {
            int index = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count - 1;
            currPage = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack[index];
            currPage.DisplayAlert(Dictionary.Alert, Dictionary.InvalidPublikKey, Dictionary.Ok);
          }
        }
      }
    }


    public static void SendMessage(string Message)
    {
    }
    public static void SendPicture(string Message)
    {
    }
    public static void SendAudio(string Message)
    {
    }
    public class EncapsuleMessage
    {

    }
    public class Message
    {
      string Recipient;
      string Type;
      string Data;
    }
  }
}
