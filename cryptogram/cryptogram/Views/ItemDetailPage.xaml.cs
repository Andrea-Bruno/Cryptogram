using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using cryptogram.Models;
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
            BindingContext = this.viewModel = viewModel;
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new Item
            {
                ContactName = "Item 1",
                PublicKey = "This is an item description."
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }

    private void Send_Clicked(object sender, EventArgs e)
    {

      Core.Messaging.SendMessage(GetTextMessage().Text);
 
    }

    private Editor GetTextMessage()
    {
      return this.FindByName<Editor>("TextMessage");
    }

  }
}