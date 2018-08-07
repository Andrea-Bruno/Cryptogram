using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using cryptogram.Models;

namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class NewItemPage : ContentPage
  {
    public Item Item { get; set; }

    public NewItemPage()
    {
      InitializeComponent();

      Item = new Item
      {
        ContactName = "",
        PublicKey = ""
      };
      BindingContext = this;
    }

    async void Save_Clicked(object sender, EventArgs e)
    {
      Item.ContactName = ContactName.Text;
      Item.PublicKey = PublicKey.Text;

      MessagingCenter.Send(this, "AddItem", Item);
      await Navigation.PopModalAsync();
    }
  }
}