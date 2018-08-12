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
          InvalidKey = "Chiave non valida!";
          Add = "Aggiungi";
          Remove = "Rimuovi";
          Info = "Basato su tecnologia blockchain e decentralizzazione, le vostre comunicazioni sono sicure";
          OpenSource = "Open Source";
          EditPrivateKey="Edita chiave privata";
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
          InvalidKey = "Key not valid!";
          Add = "Add";
          Remove = "Remove";
          Info = "Based on blockchain technology and decentralization, your communications are secure";
          OpenSource = "Open Source";
          EditPrivateKey = "Edit private key";
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
    public static string InvalidKey;
    public static string Add;
    public static string Remove;
    public static string Info;
    public static string OpenSource;
    public static string EditPrivateKey;
  }
}
