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
      AppName.Text = "Cryptogram" + " " + "V.0.1";
      PubKey.Text = Core.Messaging.GetMyPublicKey();

//#if __ANDROID__

//#else
//       PrivateKey.AutoSize=EditorAutoSizeOption.TextChanges;
//#endif
    }

    private void PubKey_Clicked(object sender, EventArgs e)
    {
      if (!Core.Functions.ShareText(PubKey.Text))
      {
        PubKey.IsVisible = false;
        PubKeyCopy.Text = PubKey.Text;
        PubKeyCopy.IsVisible = true;
        PubKeyCopy.Focus();
      }
    }

    private void EditPrivateKey_Clicked(object sender, EventArgs e)
    {
      PrivateKey.IsVisible = true;
      PrivateKey.Text = Core.Messaging.MyPrivateKey;
      PrivateKey.Focus();
    }

    private void PrivateKey_Unfocused(object sender, FocusEventArgs e)
    {
      Core.Messaging.MyPrivateKey = PrivateKey.Text;
      PrivateKey.Text = "";
      PrivateKey.IsVisible = false;
    }
  }
}