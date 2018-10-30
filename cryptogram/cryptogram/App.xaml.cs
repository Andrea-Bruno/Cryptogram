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
      var entryPoints = new System.Collections.Generic.Dictionary<String, String>();
#if DEBUG
      var networkName = "testnet";
      entryPoints.Add(Environment.MachineName, "http://localhost:55007");
#else
      var NetworkName = "ANDREA";
      EntryPoints.Add(Environment.MachineName, "http://www.bitboxlab.com");
#endif
      CryptogramLibrary.Functions.Initialize(entryPoints, networkName);
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
    public static void Alert(string message)
    {
#if __ANDROID__
      Device.OpenUri(new Uri("mailto:?to=&subject=Alert&body=" + message));
#else
      Xamarin.Forms.Device.BeginInvokeOnMainThread(delegate
      {
        Xamarin.Forms.Page currPage;
        if (Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count <= 0) return;
        var index = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count - 1;
        currPage = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack[index];
        currPage.DisplayAlert(CryptogramLibrary.Resources.Dictionary.Alert, message, CryptogramLibrary.Resources.Dictionary.Ok);
      });
#endif
    }

    public static bool ShareText(string text)
    {
      try
      {
        Device.OpenUri(new Uri("mailto:?to=&subject=Cryptogram&body=" + System.Net.WebUtility.UrlEncode(text)));
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private static void ViewMessage(DateTime timestamp,  DataType type, Byte[] data, bool isMyMessage)
    {
      Device.BeginInvokeOnMainThread(delegate
      {
        var messageLocalTime = timestamp.ToLocalTime();
        var paddingLeft = 5; var paddingRight = 5;
        Color background;
        if (isMyMessage)
        {
          paddingLeft = 20;
          background = Config.Settings.Graphics.BackgroundMyMessage;
        }
        else
        {
          background = Config.Settings.Graphics.BackgroundMessage;
          paddingRight = 20;
        }
        var frame = new Frame() { CornerRadius = 10, BackgroundColor = background, Padding = 0 };
        var box = new StackLayout() { Padding = new Thickness(paddingLeft, 5, paddingRight, 5) };
        frame.Content = box;
        var container = ChatRoom.Messages;
        container.Children.Insert(0, frame);
        var timeLabel = new Label();
        var difference = DateTime.Now - messageLocalTime;
        if (difference.TotalDays < 1)
          timeLabel.Text = messageLocalTime.ToLongTimeString();
        else
          timeLabel.Text = messageLocalTime.ToLongDateString() + " - " + messageLocalTime.ToLongTimeString();
        timeLabel.FontSize = 8;
        box.Children.Add(timeLabel);
        switch (type)
        {
          case DataType.Text:
            var label = new Label();
            label.Text = Encoding.Unicode.GetString(data);
            box.Children.Add(label);
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
