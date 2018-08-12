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
      AppName.Text = "Cryptogram" + " " + "V.0.0";
      PubKey.Text = Core.Functions.GetMyPublicKey();

#if __ANDROID__

#else
       PrivateKey.AutoSize=EditorAutoSizeOption.TextChanges;
#endif
    }

    private void PubKey_Clicked(object sender, EventArgs e)
    {
      Device.OpenUri(new Uri("mailto:?to=&subject=Cryptogram&body=" + PubKey.Text));
    }

    private void EditPrivateKey_Clicked(object sender, EventArgs e)
    {
      PrivateKey.IsVisible = true;
      PrivateKey.Text = Core.Functions.MyPrivateKey;
      PrivateKey.Focus();
    }

    private void PrivateKey_Unfocused(object sender, FocusEventArgs e)
    {
      Core.Functions.MyPrivateKey = PrivateKey.Text;
      PrivateKey.Text = "";
      PrivateKey.IsVisible = false;
    }
  }
}