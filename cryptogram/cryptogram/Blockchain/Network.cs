using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlockchainManager
{
  public static class Network
  {
    public static string SendObjectSync(object Obj, string WebAddress = null, System.Collections.Specialized.NameValueCollection Dictionary = null, string ToUser = null, int SecTimeOut = 0, int SecWaitAnswer = 0, OnReceivedObject ExecuteOnReceivedObject = null, Action ExecuteIfNoAnswer = null, bool CancellAllMyRequest = false, bool RemoveObjectsToMe = false, bool RemoveMyObjects = false)
    {
      var Reader = ExecuteServerRequest(false, WebAddress, ExecuteOnReceivedObject, SecWaitAnswer, ExecuteIfNoAnswer, Obj, Dictionary, SecTimeOut, ToUser, CancellAllMyRequest, RemoveObjectsToMe, RemoveMyObjects);
      return Reader.HTML;
    }
    public delegate void OnReceivedObject(string FromUser, string ObjectName, string XmlObject);
    private static string AppName = System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(',')[0];
    private static string UrlServer()
    {
      if (Setup.Network.MasterServer == "")
        return null;
      return Setup.Network.MasterServer.TrimEnd('/');
    }
    private static WebReader ExecuteServerRequest(bool Async, string WebAddress = null, OnReceivedObject ExecuteOnReceivedObject = null, int SecWaitAnswer = 0, Action ExecuteIfNoAnswer = null, object Obj = null, System.Collections.Specialized.NameValueCollection Dictionary = null, int SecTimeOut = 0, string ToUser = null, bool CancellAllMyRequest = false, bool RemoveObjectsToMe = false, bool RemoveMyObjects = false)
    {
      if (WebAddress == null)
        WebAddress = UrlServer();
      WebAddress = WebAddress.TrimEnd('/');
      WebAddress += "/appserver.aspx?app=" + System.Uri.EscapeDataString(AppName) + "&fromuser=" + System.Uri.EscapeDataString(Setup.Network.MachineName) + "&secwaitanswer=" + SecWaitAnswer.ToString();
      if (CancellAllMyRequest)
        WebAddress += "&cancellrequest=true";
      if (RemoveObjectsToMe)
        WebAddress += "&removeobjects=true";
      if (RemoveMyObjects)
        WebAddress += "&removemyobjects=true";

      // NUOVA VERSIONE AGGIUNGERE QUESTA LINEA
      if (ToUser == "")
        ToUser = Setup.Network.MasterServerMachineName + ".";
      if (ToUser != "")
        WebAddress += "&touser=" + ToUser;

      Action<string> Parser = null;
      if (ExecuteOnReceivedObject != null)
      {
        Parser = (String Html) =>
        {
          ObjectVector ObjectVector = null;
          object ReturmObj;
          Converter.XmlToObject(Html, typeof(ObjectVector), out ReturmObj);
          ObjectVector = (ObjectVector)ReturmObj;
          if (ObjectVector != null)
            ExecuteOnReceivedObject.Invoke(ObjectVector.FromUser, ObjectVector.ObjectName, ObjectVector.XmlObject);
        };
      }
      else
        WebAddress += "&nogetobject=true";

      if (Obj != null)
      {
        WebAddress += "&post=" + Obj.GetType().Name + "&sectimeout=" + SecTimeOut.ToString();
        System.IO.StringWriter Str = new System.IO.StringWriter();
        System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(Obj.GetType());
        System.Xml.Serialization.XmlSerializerNamespaces xmlns = new System.Xml.Serialization.XmlSerializerNamespaces();
        xmlns.Add(string.Empty, string.Empty);
        xml.Serialize(Str, Obj, xmlns);
        string postData = Str.ToString();
        if (Dictionary == null)
          Dictionary = new System.Collections.Specialized.NameValueCollection();

        string StrCod = Converter.StringToBase64(postData, false);
        Dictionary.Add("object", StrCod);
      }

      return ReadWeb(Async, WebAddress, Parser, null, Dictionary, SecWaitAnswer, ExecuteIfNoAnswer);
    }

    public static WebReader ReadWeb(bool Async, string Url, Action<string> Parser, Action Elapse, System.Collections.Specialized.NameValueCollection Dictionary = null, int SecTimeout = 0, Action ExecuteAtTimeout = null)
    {
      return new WebReader(Async, Url, Parser, Elapse, Dictionary, SecTimeout, ExecuteAtTimeout);
    }

    public class WebReader
    {
      public WebReader(bool Async, string Url, Action<string> Parser, Action Elapse, System.Collections.Specialized.NameValueCollection Dictionary = null, int SecTimeout = 0, Action ExecuteAtTimeout = null)
      {
        WebClient = new System.Net.WebClient();
        Execute = Parser;
        this.Elapse = Elapse;
        this.ExecuteAtTimeout = ExecuteAtTimeout;
        this.Dictionary = Dictionary;
        if (SecTimeout != 0)
        {
          Timeout = new System.Timers.Timer();
          Timeout.Interval = TimeSpan.FromSeconds(SecTimeout).TotalMilliseconds;
          Timeout.Start();
        }
        Start(Url, Async);
      }
      private System.Collections.Specialized.NameValueCollection Dictionary;
      private Action<string> Execute; // Parser = Sub(Html As String)
      private Action Elapse;
      private Action ExecuteAtTimeout;
      private System.Timers.Timer _Timeout;
      private System.Timers.Timer Timeout
      {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
          return _Timeout;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
          if (_Timeout != null)
          {
            _Timeout.Elapsed -= Timeout_Tick;
          }

          _Timeout = value;
          if (_Timeout != null)
          {
            _Timeout.Elapsed += Timeout_Tick;
          }
        }
      }

      private void Timeout_Tick(object sender, System.EventArgs e)
      {
        WebClient.CancelAsync();
        if (ExecuteAtTimeout != null)
          ExecuteAtTimeout.Invoke();
      }
      public void CancelAsync()
      {
        if (Timeout != null)
          Timeout.Stop();
        WebClient.CancelAsync();
      }
      private System.Net.WebClient _WebClient;

      private System.Net.WebClient WebClient
      {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
          return _WebClient;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
          if (_WebClient != null)
          {
            _WebClient.OpenReadCompleted -= WebClient_OpenReadCompleted;
          }

          _WebClient = value;
          if (_WebClient != null)
          {
            _WebClient.OpenReadCompleted += WebClient_OpenReadCompleted;
          }
        }
      }

      private void Start(string Url, bool Async)
      {
        if (Async)
          WebClient.OpenReadAsync(new Uri(Url));
        else
        {
          if (Dictionary == null)
            Dictionary = new System.Collections.Specialized.NameValueCollection();
          try
          {
            var responsebytes = WebClient.UploadValues(Url, "POST", Dictionary);
            HTML = (new System.Text.UTF8Encoding()).GetString(responsebytes);
          }
          catch (Exception ex)
          {
          }


          if (Execute != null && HTML != null)
            Execute(HTML);
          if (Elapse != null)
            Elapse();
        }
      }
      public string HTML;
      private void WebClient_OpenReadCompleted(object sender, System.Net.OpenReadCompletedEventArgs e)
      {
        if (Timeout != null)
          Timeout.Stop();
        if (e.Error == null && !e.Cancelled)
        {
          System.IO.BinaryReader BinaryStreamReader = new System.IO.BinaryReader(e.Result);
          byte[] Bytes;
          Bytes = BinaryStreamReader.ReadBytes(System.Convert.ToInt32(BinaryStreamReader.BaseStream.Length));
          if (Bytes != null)
          {
            var ContentType = WebClient.ResponseHeaders["Content-Type"];
            System.Text.Encoding Encoding = null;
            if (ContentType != "")
            {
              string[] Parts = ContentType.Split('=');
              if (Parts.Length == 2)
              {
                try
                {
                  Encoding = System.Text.Encoding.GetEncoding(Parts[1]);
                }
                catch (Exception ex)
                {
                }
              }
            }

            if (Encoding == null)
            {
              var Row = System.Text.Encoding.UTF8.GetString(Bytes, 0, Bytes.Length);
              if (Row != "")
              {
                try
                {
                  int P1 = Row.IndexOf("charset=") + 1;
                  if (P1 > 0)
                  {
                    if (Row[P1 + 7] == '"')
                      P1 += 9;
                    else
                      P1 += 8;
                    int P2 = Row.IndexOf("\"", P1);
                    if (P2 > 0)
                    {
                      var EncodeStr = Row.Substring(P1 - 1, P2 - P1);
                      try
                      {
                        Encoding = System.Text.Encoding.GetEncoding(EncodeStr); // http://msdn.microsoft.com/library/vstudio/system.text.encoding(v=vs.100).aspx
                      }
                      catch (Exception ex)
                      {
                      }
                    }
                  }
                }
                catch (Exception ex)
                {
                }
              }
            }
            if (Encoding != null)
            {
              HTML = Encoding.GetString(Bytes, 0, Bytes.Length);
              if (Execute != null && HTML != null)
                Execute(HTML);
            }
          }
        }
        if (Elapse != null)
          Elapse();
      }
    }

    public class ObjectVector
    {
      public ObjectVector()
      {
      }
      public ObjectVector(string FromUser, string ObjectName, string XmlObject)
      {
        this.FromUser = FromUser; this.ObjectName = ObjectName; this.XmlObject = XmlObject;
      }
      public ObjectVector(string FromUser, object Obj)
      {
        this.FromUser = FromUser;
        ObjectName = Obj.GetType().Name;

        System.Xml.Serialization.XmlSerializer XmlSerializer = new System.Xml.Serialization.XmlSerializer(Obj.GetType());

        using (System.IO.StringWriter textWriter = new System.IO.StringWriter())
        {
          XmlSerializer.Serialize(textWriter, Obj);
          XmlObject = textWriter.ToString();
        }
      }
      public string FromUser;
      public string ObjectName;
      public string XmlObject;
    }

  }

}
