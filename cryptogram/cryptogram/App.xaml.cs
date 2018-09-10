using System;
using Xamarin.Forms;
using cryptogram.Views;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace cryptogram
{
  public partial class App : Application
  {
    public static int Version;
    public App()
    {
      InitializeComponent();
      MainPage = new MainPage();
      var EntryPoints = new System.Collections.Generic.Dictionary<String, String>();
#if DEBUG
      var NetworkName = "testnet";
      EntryPoints.Add(Environment.MachineName, "http://localhost:55007");
#else
      var NetworkName = "ANDREA";
      EntryPoints.Add(Environment.MachineName, "http://www.bitboxlab.com");
#endif
      BlockchainManager.HookToNetwork.Initialize(null, EntryPoints, NetworkName);
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

  }
}
