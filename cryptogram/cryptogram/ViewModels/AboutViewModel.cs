using System;
using System.Windows.Input;
using cryptogram.Resources;

using Xamarin.Forms;

namespace cryptogram.ViewModels
{
  public class AboutViewModel : BaseViewModel
  {
    public AboutViewModel()
    {
      Title = Dictionary.About;
      OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://github.com/Andrea-Bruno/cryptogram")));
    }

    public ICommand OpenWebCommand { get; }
  }
}