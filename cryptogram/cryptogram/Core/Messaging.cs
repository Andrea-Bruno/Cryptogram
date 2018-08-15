using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockchainManager;

// C:\Users\Andrea\AppData\Local\Packages\0aecfc2f-ac07-4958-873b-e4079421c833_805bqyqnptpj0\LocalState\blockchains\cryptogram

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
    const int MaxPartecipants = 10;
    private static string BlockChainName;
    private static Blockchain Blockchain;
    /// <summary>
    /// This function starts the messaging chat room
    /// </summary>
    /// <param name="PublicKeys">A string containing all the participants' public keys</param>
    public static bool CreateChatRoom(string PublicKeys)
    {
      string MyPublicKey = GetMyPublicKey();
      if (!PublicKeys.Contains(MyPublicKey))
        PublicKeys += MyPublicKey;
      PublicKeys = PublicKeys.Replace("==", "== ");
      var Keys = PublicKeys.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      return CreateChatRool(new List<string>(Keys));
    }

    private static List<string> _Participants; //List in base64 format
    public static bool CreateChatRool(List<string> Partecipants)
    {
      if (Partecipants.Count > MaxPartecipants)
      {
        Functions.Alert(Resources.Dictionary.TooManyParticipants);
        return false;
      }
      foreach (var MemberKey in Partecipants)
      {
        if (!ValidateKey(MemberKey))
        {
          Functions.Alert(Resources.Dictionary.InvalidKey);
          return false;
        }
      }
      Partecipants.Sort();
      _Participants = Partecipants;
      string PtsStr = string.Join(" ", _Participants.ToArray());
      System.Security.Cryptography.HashAlgorithm hashType = new System.Security.Cryptography.SHA256Managed();
      byte[] hashBytes = hashType.ComputeHash(Converter.StringToByteArray((PtsStr)));
      BlockChainName = Convert.ToBase64String(hashBytes);
      Blockchain = new Blockchain("cryptogram", BlockChainName, Blockchain.BlockchainType.Binary, false, 8192);
      Blockchain.RequestAnyNewBlocks();
      ReadBlockchain();
      return true;
      //Xamarin.Forms.Device.BeginInvokeOnMainThread(delegate
      //{
      //  Blockchain.RequestAnyNewBlocks();
      //  ReadBlockchain();
      //});
    }

    private static void ReadBlockchain()
    {
      var Blocks = Blockchain.GetBlocks(0);
      foreach (var Block in Blocks)
      {
        if (Block != null && Block.IsValid())
        {
          var DateAndTime = Block.Timestamp;
          var BlockData = Block.DataByteArray;
          byte Version = BlockData[0];
          DataType Type = (DataType)BlockData[1];
          var Password = DecryptPassword(BlockData, out int EncryptedDataPosition);
          var Len = BlockData.Length - EncryptedDataPosition;
          var EncryptedData = new byte[Len];
          Buffer.BlockCopy(BlockData, EncryptedDataPosition, EncryptedData, 0, Len);
          var DataElement = Cryptography.Decrypt(EncryptedData, Password);
          int DataLen = DataElement.Length - 128;
          byte[] Data = new byte[DataLen];
          Buffer.BlockCopy(DataElement, 0, Data, 0, DataLen);
          System.Security.Cryptography.HashAlgorithm hashType = new System.Security.Cryptography.SHA256Managed();
          byte[] HashData = hashType.ComputeHash(Data);
          byte[] SignatureOfData = new byte[128];
          Buffer.BlockCopy(DataElement, DataLen, SignatureOfData, 0, 128);
          // Find the author
          string Author = null;
          foreach (var Partecipant in _Participants)
          {
            System.Security.Cryptography.RSACryptoServiceProvider RSAalg = new System.Security.Cryptography.RSACryptoServiceProvider();
            RSAalg.ImportCspBlob(Convert.FromBase64String(Partecipant));
            if (RSAalg.VerifyHash(HashData, System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA256"), SignatureOfData))
            {
              Author = Partecipant;
              break;
            }
          }
          //var SignatureOfData = GetMyRSA().SignHash(HashData, System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA256"));
          //var Signatures = Block.GetAllBodySignature();
          //var Author = _Participants.Find(x => Signatures.ContainsKey(x));
          if (Author == null)
            System.Diagnostics.Debug.WriteLine("Block written by an impostor");
          else
          {
            var IsMy = Author == GetMyPublicKey();
            AddMessageView(DateAndTime, Type, Data, IsMy);
          }
        }
      }
    }

    private static void AddMessageView(DateTime Timestamp, DataType Type, Byte[] Data, bool IsMyMessage)
    {
      var MessageLocalTime = Timestamp.ToLocalTime();
      var Container = cryptogram.Views.ItemDetailPage.Messages;
      var PaddingLeft = 5; var PaddingRight = 5;
      Xamarin.Forms.Color Background;
      if (IsMyMessage)
      {
        PaddingLeft = 20;
        Background = Settings.Graphics.BackgroundMyMessage;
      }
      else
      {
        Background = Settings.Graphics.BackgroundMessage;
        PaddingRight = 20;
      }
      var Frame = new Xamarin.Forms.Frame() { CornerRadius = 10, BackgroundColor = Background, Padding = 0 };
      var Box = new Xamarin.Forms.StackLayout() { Padding = new Xamarin.Forms.Thickness(PaddingLeft, 5, PaddingRight, 5) };
      Frame.Content = Box;
      Container.Children.Insert(0, Frame);
      var TimeLabel = new Xamarin.Forms.Label();
      TimeSpan Difference = DateTime.Now - MessageLocalTime;
      if (Difference.TotalDays < 1)
        TimeLabel.Text = MessageLocalTime.ToLongTimeString();
      else
        TimeLabel.Text = MessageLocalTime.ToLongDateString() + " - " + MessageLocalTime.ToLongTimeString();
      TimeLabel.FontSize = 8;
      Box.Children.Add(TimeLabel);
      switch (Type)
      {
        case DataType.Text:
          var Label = new Xamarin.Forms.Label();
          Label.Text = Encoding.Unicode.GetString(Data);
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

    private static byte[] GeneratePassword()
    {
      return Guid.NewGuid().ToByteArray();
    }

    //private static byte[] pw;
    private static byte[] EncryptPasswordForParticipants(byte[] Password)
    {
      //========================RESULT================================
      //[len ePass1] + [ePass1] + [len ePass2] + [ePass2] + ... + [0] 
      //==============================================================
      byte[] Result = new byte[0];
      foreach (var PublicKey in _Participants)
      {
        System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
        RSA.ImportCspBlob(Convert.FromBase64String(PublicKey));
        var EncryptedPassword = RSA.Encrypt(Password, true);

        //test
        //if (PublicKey == Functions.GetMyPublicKey())
        //{
        //  pw = EncryptedPassword;
        //  var PW = Functions.GetMyRSA().Decrypt(EncryptedPassword, true);
        //}

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

      int MyId = _Participants.IndexOf(GetMyPublicKey());
      if (MyId == -1)
      {
        //Im not in this chat
        return null;
      }
      var EPassword = EncryptedPasswords[MyId];
      var RSA = GetMyRSA();

      return RSA.Decrypt(EPassword, true);
    }

    private static void SendData(DataType Type, byte[] Data)
    {
      try
      {
        const byte Version = 0;
        byte[] BlockchainData = { Version, (byte)Type };
        var Password = GeneratePassword();
        var GlobalPassword = EncryptPasswordForParticipants(Password);
        System.Security.Cryptography.HashAlgorithm hashType = new System.Security.Cryptography.SHA256Managed();
        byte[] HashData = hashType.ComputeHash(Data);
        var SignatureOfData = GetMyRSA().SignHash(HashData, System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA256"));
        var EncryptedData = Cryptography.Encrypt(Data.Concat(SignatureOfData).ToArray(), Password);
        BlockchainData = BlockchainData.Concat(GlobalPassword).Concat(EncryptedData).ToArray();
        Blockchain.RequestAnyNewBlocks();
        if (BlockchainData.Length * 2 + 4096 <= Blockchain.MaxBlockLenght)
        {
          Blockchain.Block NewBlock = new Blockchain.Block(Blockchain, BlockchainData);
          var BlockPosition = Blockchain.Length();
          if (!NewBlock.IsValid()) System.Diagnostics.Debugger.Break();
          Blockchain.SyncBlockToNetwork(NewBlock, BlockPosition);
          AddMessageView(NewBlock.Timestamp, Type, Data, true);
        }
        else
          Functions.Alert(Resources.Dictionary.ExceededBlockSizeLimit);
      }
      catch (Exception Ex)
      {
        Functions.Alert(Ex.Message);
      }
    }

    public static void SendText(string Text)
    {
      if (Text != null)
        Text = Text.Trim(" ".ToCharArray());
      if (!string.IsNullOrEmpty(Text))
        SendData(DataType.Text, Encoding.Unicode.GetBytes(Text));
    }

    public static void SendPicture(object Image)
    {

    }

    public static void SendAudio(object Audio)
    {

    }

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
      get
      {
        if (string.IsNullOrEmpty(_MyPrivateKey))
          _MyPrivateKey = (string)Storage.LoadObject(typeof(string), "MyPrivateKey");
        if (string.IsNullOrEmpty(_MyPrivateKey))
        {
          MyPrivateKey = Convert.ToBase64String(new System.Security.Cryptography.RSACryptoServiceProvider().ExportCspBlob(true)); //Save
        }
        return _MyPrivateKey;
      }
      set
      {
        if (_MyPrivateKey != value)
        {
          try
          {
            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
            RSA.ImportCspBlob(Convert.FromBase64String(value));
            _MyPrivateKey = value;
            Storage.SaveObject(_MyPrivateKey, "MyPrivateKey");
          }
          catch (Exception)
          {
            Functions.Alert(Resources.Dictionary.InvalidKey);
            throw;
          }
        }

      }
    }

    public class Contact
    {
      private string FirstUpper(string Text)
      {
        string Value = "";
        if (!string.IsNullOrEmpty(Text))
        {
          bool Last = false;
          foreach (char c in Text)
          {
            if (char.IsLetter(c))
            {
              if (!Last)
                Value += char.ToUpper(c);
              else
                Value += c;
              Last = true;
            }
            else
            {
              Last = false;
              Value += c;
            }
          }
        }
        return Value;
      }
      private string _Name;
      public string Name
      {
        get { return _Name; }
        set
        {
          _Name = FirstUpper(value);
        }
      }
      private string _PublicKey;
      public string PublicKey
      {
        get { return _PublicKey; }
        set
        {
          _PublicKey = "";
          if (value != null)
          {
            foreach (var c in value.ToCharArray())
            {
              //Clear Base64 string
              if (char.IsLetterOrDigit(c) || @"+=/".Contains(c))
                _PublicKey += c;
            }
          }
        }
      }
    }

    private static readonly List<Contact> _Contacts = InitContacts();
    private static List<Contact> InitContacts()
    {
      List<Contact> List = (List<Contact>)Storage.LoadObject(typeof(List<Contact>), "Contacts");
      if (List == null)
        List = new List<Contact>();
#if DEBUG
      if (List.Count == 0)
        List.Add(new Contact() { Name = "Pippo", PublicKey = Convert.ToBase64String(new System.Security.Cryptography.RSACryptoServiceProvider().ExportCspBlob(false)) });
#endif
      return List;
    }

    public static Contact[] GetContacts()
    {
      lock (_Contacts)
        return _Contacts.ToArray();
    }
    public static bool AddContact(Contact Contact)
    {
      if (ValidateKey(Contact.PublicKey))
      {
        lock (_Contacts)
        {
          Contact Duplicate = _Contacts.Find(X => X.PublicKey == Contact.PublicKey);
          if (Duplicate != null)
            _Contacts.Remove(Duplicate);
          _Contacts.Add(Contact);
          var Sorted = _Contacts.OrderBy(o => o.Name).ToList();
          _Contacts.Clear();
          _Contacts.AddRange(Sorted);
        }
        Storage.SaveObject(_Contacts, "Contacts");
        return true;
      }
      else
        Functions.Alert(Resources.Dictionary.InvalidKey);
      return false;
    }

    public static void RemoveContact(String Key)
    {
      Contact Contact = _Contacts.Find(X => X.PublicKey == Key);
      if (Contact != null)
      {
        lock (_Contacts)
          _Contacts.Remove(Contact);
        Storage.SaveObject(_Contacts, "Contacts");
      }
    }
    private static bool ValidateKey(string Key)
    {
      try
      {
        var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
        RSA.ImportCspBlob(Convert.FromBase64String(Key));
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

  }
}
