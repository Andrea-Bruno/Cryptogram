using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using cryptogram.ViewModels;

namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ItemDetailPage : ContentPage
  {
    ItemDetailViewModel viewModel;

    public ItemDetailPage(ItemDetailViewModel viewModel)
    {
      InitializeComponent();
      Messages = this.FindByName<StackLayout>("MessageList");
      BindingContext = this.viewModel = viewModel;
      Core.Messaging.CreateChatRoom(PublicKey.Text);
      //this.Appearing += delegate
      //          {
      //          };
      TextMessage.Focus();
    }

    public static StackLayout Messages;

    public ItemDetailPage()
    {
      InitializeComponent();

      var item = new Core.Messaging.Contact
      {
        Name = "",
        PublicKey = ""
      };

      viewModel = new ItemDetailViewModel(item);
      BindingContext = viewModel;
    }

    private void Send_Clicked(object sender, EventArgs e)
    {
      Core.Messaging.SendText(TextMessage.Text);
      TextMessage.Text = "";
    }

    async void Remove_Clicked(object sender, EventArgs e)
    {
      var Item = viewModel.Item;
      MessagingCenter.Send(this, "DeleteItem", Item);
      await Navigation.PopAsync();
    }

    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
      var Item = viewModel.Item;
      if (Item.Name != _NameContact)
      {
        MessagingCenter.Send(this, "UpdateItem", Item);
      }
    }

    private string _NameContact;
    private void ContentPage_Appearing(object sender, EventArgs e)
    {
      var Item = viewModel.Item;
      _NameContact = Item.Name;
    }

    private void PublicKey_Clicked(object sender, EventArgs e)
    {
      Core.Functions.ShareText(PublicKey.Text);
    }
  }
}