using System;
using Xamarin.Forms;
using cryptogram.Views;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace cryptogram
{
  public partial class App : Application
  {

    public App()
    {
      // Initialize Live Reload.
#if DEBUG
      //LiveReload.Init();
#endif

      InitializeComponent();
      MainPage = new MainPage();
    }

    protected override void OnStart()
    {
      // Handle when your app starts
      if (!Application.Current.Properties.ContainsKey("keys"))
      {
        System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
        Application.Current.Properties.Add("keys", RSA.ExportCspBlob(true));
      }
      OnResume();
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
      Application.Current.Properties.TryGetValue("keys",out object GetKey);
      byte[] Keys = (byte[])GetKey;
      cryptogram.Core.Messaging.SetRSA(Keys);
    }
  }
}
