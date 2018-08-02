using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockchainManager;
namespace cryptogram.Core
{
  public static class Messaging
  {
    enum DataType
    {
      Text,
      Image,
      Audio,
    }
    private static string BlockChainName;
    private static Blockchain Blockchain;
    private static List<string> _Participants; //List in base64 format
    public static List<string> Participants
    {
      get { return _Participants; }
      set
      {
        value.Sort();
        _Participants = value;
        string PtsStr = string.Join(" ", _Participants.ToArray());
        System.Security.Cryptography.HashAlgorithm hashType = new System.Security.Cryptography.SHA256Managed();
        byte[] hashBytes = hashType.ComputeHash(Converter.StringToByteArray((PtsStr)));
        BlockChainName = Convert.ToBase64String(hashBytes);
        Blockchain = new Blockchain("cryptogram", BlockChainName, Blockchain.BlockchainType.Binary, true);
        ReadBlockchain();
      }
    }

    private static void ReadBlockchain()
    {
      var Block = Blockchain.GetLastBlock();
      if (Block != null && Block.IsValid())
      {
        var DateAndTime = Block.Timestamp;
        var Data = Block.DataByteArray;
        byte Version = Data[0];
        DataType Type = (DataType)Data[1];
        var Password = DecryptPassword(Data, out int EncryptedDataPosition);
        var Len = Data.Length - EncryptedDataPosition;
        var EncryptedData = new byte[Len];
        Buffer.BlockCopy(Data, EncryptedDataPosition, EncryptedData, 0, Len);
        var DataElement = Cryptography.Decrypt(EncryptedData, Password);
        var Signatures = Block.GetAllBodySignature();
        var Author = Participants.Find(x => Signatures.ContainsKey(x));
        if (Author == null)
          System.Diagnostics.Debug.WriteLine("Block written by an impostor");
        else
        {
          var IsMy = Author == Functions.GetMyPublicKey();
          var Container = cryptogram.Views.ItemDetailPage.Messages;
          var PaddingLeft = 5; var PaddingRight = 5;
          if (IsMy)
            PaddingLeft = 20;
          else
            PaddingRight = 20;
          var Box = new Xamarin.Forms.StackLayout() { Padding = new Xamarin.Forms.Thickness(PaddingLeft, 5, PaddingRight, 5) };
          Container.Children.Add(Box);
          switch (Type)
          {
            case DataType.Text:
              var Label = new Xamarin.Forms.Label();
              Label.Text = Encoding.Unicode.GetString(DataElement);
              Box.Children.Add(Label);
              break;
            case DataType.Image:
              break;
            case DataType.Audio:
              break;
            default:
              break;
          }
        }
      }
    }

    private static byte[] GeneratePassword()
    {
      return Guid.NewGuid().ToByteArray();
    }

    private static byte[] pw;
    private static byte[] EncryptPasswordForParticipants(byte[] Password)
    {
      //========================RESULT================================
      //[len ePass1] + [ePass1] + [len ePass2] + [ePass2] + ... + [0] 
      //==============================================================
      byte[] Result = new byte[0];
      foreach (var PublicKey in Participants)
      {
        System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
        RSA.ImportCspBlob(Convert.FromBase64String(PublicKey));
        var EncryptedPassword = RSA.Encrypt(Password, true);

        //test
        if (PublicKey == Functions.GetMyPublicKey())
        {
          pw = EncryptedPassword;
          var PW = Functions.GetMyRSA().Decrypt(EncryptedPassword, true);
        }



        byte LanPass = (byte)EncryptedPassword.Length;
        byte[] Len = new byte[] { LanPass };
        Result = Result.Concat(Len).Concat(EncryptedPassword).ToArray();
      }
      Result = Result.Concat(new byte[] { 0 }).ToArray();
      return Result;
    }
    private static byte[] DecryptPassword(byte[] Data, out int EncryptedDataPosition)
    {
      //START ==== Obtain all password encrypted ====
      var EncryptedPasswords = new List<Byte[]>();
      int P = 2;
      int Len = Data[P];
      do
      {
        P += 1;
        byte[] EncryptedPassword = new byte[Len];
        Buffer.BlockCopy(Data, P, EncryptedPassword, 0, Len);
        EncryptedPasswords.Add(EncryptedPassword);
        P += Len;
        Len = Data[P];
      } while (Len != 0);
      //END  ==== Obtain all password encrypted ====
      EncryptedDataPosition = P + 1;

      int MyId = _Participants.IndexOf(Functions.GetMyPublicKey());
      if (MyId == -1)
      {
        //Im not in this chat
        return null;
      }
      var EPassword = EncryptedPasswords[MyId];
      var RSA = Functions.GetMyRSA();
      var x = RSA.Decrypt(pw, true);
      for (int i = 0; i < pw.Length - 1; i++)
      {
        if (pw[i] != EPassword[i])
        {
          int fail = 1;
        }
      }

      return RSA.Decrypt(EPassword, true);
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
          Core.Functions.Alert(Resources.Dictionary.InvalidPublikKey);
        }
      }
    }

    private static void SendData(DataType Type, byte[] Data)
    {
      const byte Version = 0;
      byte[] BlockchainData = { Version, (byte)Type };
      var Password = GeneratePassword();
      var GlobalPassword = EncryptPasswordForParticipants(Password);
      var EncryptedData = Cryptography.Encrypt(Data, Password);
      BlockchainData = BlockchainData.Concat(GlobalPassword).Concat(EncryptedData).ToArray();

      var PublicKeyBase64 = Functions.GetMyPublicKey();
      Blockchain.Block NewBlock = new Blockchain.Block(Blockchain, BlockchainData);
      var Signature = Functions.GetMyRSA().SignHash(NewBlock.HashBody(), System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA256"));
      bool IsValid = NewBlock.AddBodySignature(PublicKeyBase64, Signature, true); //Add signature e add the block to blockchain now
    }

    public static void SendText(string Text)
    {
      SendData(DataType.Text, Encoding.Unicode.GetBytes(Text));
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
