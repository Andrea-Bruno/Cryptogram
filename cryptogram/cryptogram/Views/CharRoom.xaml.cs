using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using cryptogram.ViewModels;

namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ChatRoom : ContentPage
  {
    private readonly ItemDetailViewModel _viewModel;

    public ChatRoom(ItemDetailViewModel viewModel)
    {
      InitializeComponent();
      Messages = MessageList;
      BindingContext = _viewModel = viewModel;
      TextMessage.Focus();
    
      Device.BeginInvokeOnMainThread(delegate
      {
        CryptogramLibrary.Messaging.CreateChatRoom(viewModel.Item.PublicKey);
      });


      //this.Appearing += delegate
      //          {
      //          };
    }
    public static StackLayout Messages;

    public ChatRoom()
    {
      InitializeComponent();
      var item = new CryptogramLibrary.Messaging.Contact
      {
        Name = "",
        PublicKey = ""
      };
      _viewModel = new ItemDetailViewModel(item);
      BindingContext = _viewModel;
    }

    private void Send_Clicked(object sender, EventArgs e)
    {
      CryptogramLibrary.Messaging.SendText(TextMessage.Text);
      TextMessage.Text = "";
    }

    private async void Remove_Clicked(object sender, EventArgs e)
    {
      var item = _viewModel.Item;
      CryptogramLibrary.Messaging.RemoveContact(item);
      await Navigation.PopAsync();
    }

    private void ContentPage_Disappearing(object sender, EventArgs e)
    {
      var item = _viewModel.Item;
      if (item.Name != _nameContact)
        item.Save();
    }

    private string _nameContact;
    private void ContentPage_Appearing(object sender, EventArgs e)
    {
      TextMessage_Unfocused(null, null);
      var item = _viewModel.Item;
      _nameContact = item.Name;
    }

    private Color? _watermark;
    private void TextMessage_Focused(object sender, FocusEventArgs e)
    {
      if (_watermark == null) return;
      TextMessage.TextColor = (Color)_watermark;
      TextMessage.IsSpellCheckEnabled = true;
      TextMessage.Text = "";
      _watermark = null;
    }

    private void TextMessage_Unfocused(object sender, FocusEventArgs e)
    {
      if (string.IsNullOrEmpty(TextMessage.Text))
      {
        _watermark = TextMessage.TextColor;
        TextMessage.TextColor = Color.FromRgba(128, 128, 128, 128);
        TextMessage.IsSpellCheckEnabled = false;
        TextMessage.Text = cryptogram.Resources.Dictionary.strictlyConfidentialMessage + ": «We are anonymous!»";
      };
    }
  }
}