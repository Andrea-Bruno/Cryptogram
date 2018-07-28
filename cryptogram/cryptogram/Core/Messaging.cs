using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockchainManager;
namespace cryptogram.Core
{
  public static class Messaging
  {
    private static string BlockChainName;
    private static Blockchain Blockchain;
    private static List<string> _Participants;
    public static List<string> Participants
    {
      get { return _Participants; }
      set
      {
        value.Sort();
        _Participants = value;
        string PtsStr = string.Join(" ", _Participants.ToArray());
        System.Security.Cryptography.HashAlgorithm hashType = new System.Security.Cryptography.SHA256Managed();
        byte[] hashBytes = hashType.ComputeHash(Encoding.ASCII.GetBytes(PtsStr));
        BlockChainName = Convert.ToBase64String(hashBytes);
        Blockchain = new Blockchain("cryptogram", BlockChainName, Blockchain.BlockchainType.Binary, true);
      }
    }


    private static byte[] GeneratePassword()
    {
      return Guid.NewGuid().ToByteArray();
    }

    private static byte[] EncryptPasswordForParticipants(byte[] Password)
    {
      //========================RESULT================================
      //[len ePass1] + [ePass1] + [len ePass2] + [ePass2] + ... + [0] 
      //==============================================================
      byte[] Result = new byte[0];
      foreach (var item in Participants)
      {
        System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
        RSA.ImportCspBlob(Convert.FromBase64String(item));
        var EncryptedPassword = RSA.Encrypt(Password, true);
        byte LanPass = (byte)EncryptedPassword.Length;
        byte[] Len = new byte[] { LanPass };
        Result = Result.Concat(Result).Concat(Len).Concat(EncryptedPassword).ToArray();
      }
      Result = Result.Concat(Result).Concat(new byte[] { 0 }).ToArray();
      return Result;
    }

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
            currPage.DisplayAlert(Resources.Dictionary.Alert, Resources.Dictionary.InvalidPublikKey, Resources.Dictionary.Ok);
          }
        }
      }
    }


    public static void SendMessage(string Message)
    {
      var Password = GeneratePassword();
      var GlobalPassword = EncryptPasswordForParticipants(Password);
      var EncryptedData = Cryptography.EncryptText(Message, Password);
      byte[] data = GlobalPassword.Concat(EncryptedData).ToArray();

      var PublicKeyBase64 = Convert.ToBase64String(RSA.ExportCspBlob(false));
      Blockchain.Block NewBlock = new Blockchain.Block(Blockchain, Convert.ToString(data));
      var Signature = RSA.SignHash(NewBlock.HashBody(), System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA256"));
      bool IsValid = NewBlock.AddBodySignature(PublicKeyBase64, Signature, true);

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
