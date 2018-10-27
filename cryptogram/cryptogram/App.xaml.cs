using System;
using Xamarin.Forms;
using cryptogram.Views;
using Xamarin.Forms.Xaml;
using static CryptogramLibrary.Messaging;
using System.Text;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace cryptogram
{
  public partial class App : Application
  {
    public static int Version;
    public App()
    {
      InitializeComponent();
      CryptogramLibrary.Functions.Alert = Alert;
      CryptogramLibrary.Functions.ShareText = ShareText;
      CryptogramLibrary.Messaging.ViewMessage = ViewMessage;
      MainPage = new MainPage();
      var EntryPoints = new System.Collections.Generic.Dictionary<String, String>();
#if DEBUG
      var NetworkName = "testnet";
      EntryPoints.Add(Environment.MachineName, "http://localhost:55007");
#else
      var NetworkName = "ANDREA";
      EntryPoints.Add(Environment.MachineName, "http://www.bitboxlab.com");
#endif
      CryptogramLibrary.Functions.Initialize(EntryPoints, NetworkName);
      //NetworkExtension.Network.Initialize();

    }

    protected override void OnStart()
    {
      // Handle when your app starts
      OnResume();
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
    }

    //=========================================================================================
    public static void Alert(string Message)
    {
#if __ANDROID__
      Xamarin.Forms.Device.OpenUri(new Uri("mailto:?to=&subject=Alert&body=" + Message));
#else
      Xamarin.Forms.Device.BeginInvokeOnMainThread(delegate
      {
        Xamarin.Forms.Page currPage;
        if (Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count > 0)
        {
          int index = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count - 1;
          currPage = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack[index];
          currPage.DisplayAlert(CryptogramLibrary.Resources.Dictionary.Alert, Message, CryptogramLibrary.Resources.Dictionary.Ok);
        }
      });
#endif
    }

    public static bool ShareText(string Text)
    {
      try
      {
        Xamarin.Forms.Device.OpenUri(new Uri("mailto:?to=&subject=Cryptogram&body=" + System.Net.WebUtility.UrlEncode(Text)));
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private static void ViewMessage(DateTime Timestamp,  DataType Type, Byte[] Data, bool IsMyMessage)
    {
      Xamarin.Forms.Device.BeginInvokeOnMainThread(delegate
      {
        var MessageLocalTime = Timestamp.ToLocalTime();
        var PaddingLeft = 5; var PaddingRight = 5;
        Xamarin.Forms.Color Background;
        if (IsMyMessage)
        {
          PaddingLeft = 20;
          Background = Config.Settings.Graphics.BackgroundMyMessage;
        }
        else
        {
          Background = Config.Settings.Graphics.BackgroundMessage;
          PaddingRight = 20;
        }
        var Frame = new Xamarin.Forms.Frame() { CornerRadius = 10, BackgroundColor = Background, Padding = 0 };
        var Box = new Xamarin.Forms.StackLayout() { Padding = new Xamarin.Forms.Thickness(PaddingLeft, 5, PaddingRight, 5) };
        Frame.Content = Box;
        var Container = Views.ChatRoom.Messages;
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
      });
    }
  }
}
