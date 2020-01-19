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
      BlockchainManager.Network.Initialize();
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
