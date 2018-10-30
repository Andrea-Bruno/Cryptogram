using System;

//using Plugin.Share;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class AboutPage : ContentPage
  {
    public AboutPage()
    {
      InitializeComponent();
      AppName.Text = "Cryptogram" + " " + "V.0.3";
      PubKey.Text = CryptogramLibrary.Messaging.GetMyPublicKey();

//#if __ANDROID__

//#else
//       PrivateKey.AutoSize=EditorAutoSizeOption.TextChanges;
//#endif
    }

    private void PubKey_Clicked(object sender, EventArgs e)
    {
      if (CryptogramLibrary.Functions.ShareText(PubKey.Text)) return;
      PubKey.IsVisible = false;
      PubKeyCopy.Text = PubKey.Text;
      PubKeyCopy.IsVisible = true;
      PubKeyCopy.Focus();
    }

    private void EditPrivateKey_Clicked(object sender, EventArgs e)
    {
      PrivateKey.IsVisible = true;
      PrivateKey.Text = CryptogramLibrary.Messaging.MyPrivateKey;
      PrivateKey.Focus();
    }

    private void PrivateKey_Unfocused(object sender, FocusEventArgs e)
    {
      CryptogramLibrary.Messaging.MyPrivateKey = PrivateKey.Text;
      PrivateKey.Text = "";
      PrivateKey.IsVisible = false;
    }
  }
}