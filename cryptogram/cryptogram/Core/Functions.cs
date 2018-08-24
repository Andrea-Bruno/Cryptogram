using System;
using System.Collections.Generic;
using System.Text;

namespace cryptogram.Core
{
  static class Functions
  {

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
          currPage.DisplayAlert(Resources.Dictionary.Alert, Message, Resources.Dictionary.Ok);
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
  }
}
