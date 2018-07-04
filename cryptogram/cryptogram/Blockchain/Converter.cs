using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainManager
{
  public static class Converter
  {
    public static string StringToBase64(string Text, bool AsciiEncoding = false)
    {
      // This function is a quick way to crypt a text string
      byte[] Bytes = StringToByteArray(Text, AsciiEncoding);
      return System.Convert.ToBase64String(Bytes);
    }
    public static string Base64ToString(string Text, bool AsciiEncoding = false)
    {
      // Now easy to decrypt a data
      byte[] Bytes = System.Convert.FromBase64String(Text);
      return ByteArrayToString(Bytes, AsciiEncoding);
    }
    public static byte[] StringToByteArray(string Text, bool ASCIIEncoding = false)
    {
      if (!string.IsNullOrEmpty(Text))
      {
        if (ASCIIEncoding)
          return System.Text.Encoding.ASCII.GetBytes(Text);
        else
          // The object System.Text.Encoding.Unicode have a problem in Windows x64. Replache this object with System.Text.Encoding.GetEncoding("utf-16LE") 
          return System.Text.Encoding.GetEncoding("utf-16LE").GetBytes(Text);// Unicode encoding
      }
      return null;
    }

    public static string ByteArrayToString(byte[] Bytes, bool ASCIIEncoding = false)
    {
      if (ASCIIEncoding)
        return System.Text.Encoding.ASCII.GetString(Bytes);
      else
        return System.Text.Encoding.GetEncoding("utf-16LE").GetString(Bytes);// Unicode encodin
    }

    public static void XmlToObject(string Xml, Type Type, out object Obj)
    {
      System.Xml.Serialization.XmlSerializer XmlSerializer = new System.Xml.Serialization.XmlSerializer(Type);
      try
      {
        Obj = XmlSerializer.Deserialize(new System.IO.StringReader(Xml));
      }
      catch (Exception ex)
      {
        Obj = null;
      }
    }

  }

}
