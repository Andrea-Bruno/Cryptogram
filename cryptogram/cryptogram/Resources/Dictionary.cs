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
          Send = "Invia";
          Save = "Salva";
          Alert = "Avviso";
          Ok = "Ok";
          InvalidPublikKey = "Chiave pubblica non valida!";
          Add = "Aggiungi";
          Remove = "Rimuovi";
          break;
        default:
          ContactName = "Contact name";
          PublicKey = "Public key";
          NewContact = "New contact";
          Contacts = "Contacts";
          About = "About";
          Send = "Send";
          Save = "Save";
          Alert = "Alert";
          Ok = "Ok";
          InvalidPublikKey = "Public key not valid!";
          Add = "Add";
          Remove = "Remove";
          break;
      }
    }
    public static string ContactName;
    public static string PublicKey;
    public static string NewContact;
    public static string Contacts;
    public static string About;
    public static string Send; //Send message
    public static string Save;
    public static string Alert;
    public static string Ok;
    public static string InvalidPublikKey;
    public static string Add;
    public static string Remove;
  }
}
