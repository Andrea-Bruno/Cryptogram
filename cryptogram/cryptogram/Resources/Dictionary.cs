using System;
using System.Collections.Generic;
using System.Text;

namespace cryptogram.Resources
{
  public static class Dictionary
  {
    static Dictionary()
    {
      string Lng = System.Globalization.CultureInfo.CurrentCulture.ToString().Substring(0, 2);
      switch (Lng)
      {
        case "it":
          ContactName = "Nome contatto";
          PublicKey = "Chiave pubblica";
          NewContact = "Nuovo contatto";
          Contacts = "Contatti";
          About = "Info";
          break;
        default:
          ContactName = "Contact name";
          PublicKey = "Public key";
          NewContact = "New contact";
          Contacts = "Contacts";
          About = "About";
          break;
      }
    }
    public static string ContactName;
    public static string PublicKey;
    public static string NewContact;
    public static string Contacts;
    public static string About;
  }
}
