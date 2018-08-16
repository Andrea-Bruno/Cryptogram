using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using cryptogram.ViewModels;

namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ChatRoom : ContentPage
  {
    ItemDetailViewModel viewModel;

    public ChatRoom(ItemDetailViewModel viewModel)
    {
      InitializeComponent();
      Messages = this.FindByName<StackLayout>("MessageList");
      BindingContext = this.viewModel = viewModel;
      TextMessage.Focus();

      Device.BeginInvokeOnMainThread(delegate
      {
        Core.Messaging.CreateChatRoom(viewModel.Item.PublicKey);
      });


      //this.Appearing += delegate
      //          {
      //          };
    }

    public static StackLayout Messages;

    public ChatRoom()
    {
      InitializeComponent();

      var item = new Core.Messaging.Contact
      {
        Name = "",
        PublicKey = ""
      };

      viewModel = new ItemDetailViewModel(item);
      BindingContext = viewModel;


      Core.Functions.Alert("prova");
    }

    private void Send_Clicked(object sender, EventArgs e)
    {
      Core.Messaging.SendText(TextMessage.Text);
      TextMessage.Text = "";
    }

    async void Remove_Clicked(object sender, EventArgs e)
    {
      var Item = viewModel.Item;
      Core.Messaging.RemoveContact(Item);
      await Navigation.PopAsync();
    }

    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
      var Item = viewModel.Item;
      if (Item.Name != _NameContact)
        Item.Save();
    }

    private Color? Watermark;
    private string _NameContact;
    private void ContentPage_Appearing(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(TextMessage.Text))
      {
        Watermark = TextMessage.TextColor;
        TextMessage.TextColor = Color.FromRgba(128, 128, 128, 128);
        TextMessage.IsSpellCheckEnabled = false;
        TextMessage.Text = cryptogram.Resources.Dictionary.strictlyConfidentialMessage + ": «We are anonymous!»";
      };
      var Item = viewModel.Item;
      _NameContact = Item.Name;
    }

    private void TextMessage_Focused(object sender, FocusEventArgs e)
    {
      if (Watermark != null)
      {
        TextMessage.TextColor = (Color)Watermark;
        Watermark = null;
        TextMessage.Text = "";
        TextMessage.IsSpellCheckEnabled = false;
      }

    }
  }
}