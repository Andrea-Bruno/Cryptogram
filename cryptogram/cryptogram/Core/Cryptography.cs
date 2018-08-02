using System;
using System.Security.Cryptography;
using System.Text;

namespace cryptogram.Core
{
  static class  Cryptography
    {
    public static byte[] Encrypt(byte[] Data, byte[] Password)
    {
      var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
      var encryptor = symmetricKey.CreateEncryptor(Password, Password );

      byte[] EncryptedData;

      using (var memoryStream = new System.IO.MemoryStream())
      {
        using (var CryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        {
          CryptoStream.Write(Data, 0, Data.Length);
          CryptoStream.FlushFinalBlock();
          EncryptedData = memoryStream.ToArray();
          CryptoStream.Close();
        }
        memoryStream.Close();
      }
      return EncryptedData;
    }

    public static byte[] Decrypt( byte[] EncryptedData, byte[] Password)
    {
      var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };
      var decryptor = symmetricKey.CreateDecryptor(Password, Password);
      var memoryStream = new System.IO.MemoryStream(EncryptedData);
      var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
      byte[] Data = new byte[EncryptedData.Length];

      int decryptedByteCount = cryptoStream.Read(Data, 0, Data.Length);
      memoryStream.Close();
      cryptoStream.Close();
      return Data;
    }

    //public static byte[] EncryptText(string Text, byte[] Password)
    //{ return Encrypt(Encoding.UTF8.GetBytes(Text), Password); }

    //public static string DecryptText(byte[] EncryptedText, byte[] Password)
    //{
    //  return Encoding.UTF8.GetString(Decrypt(EncryptedText, Password)).TrimEnd('\0');
    //}


  }
}
