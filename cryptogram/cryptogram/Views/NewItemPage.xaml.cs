using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class NewItemPage : ContentPage
  {
    public Core.Messaging.Contact Item { get; set; }

    public NewItemPage()
    {
      InitializeComponent();
      Item = new Core.Messaging.Contact
      {
        Name = "",
        PublicKey = ""
      };
      BindingContext = this;
    }

    async void Save_Clicked(object sender, EventArgs e)
    {
      Item.Name = Name.Text;
      Item.PublicKey = PublicKey.Text;

      MessagingCenter.Send(this, "AddItem", Item);
      await Navigation.PopModalAsync();
    }

    async private void Exit_Clicked(object sender, EventArgs e)
    {
      await Navigation.PopModalAsync();
    }
  }
}