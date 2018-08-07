using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using cryptogram.Models;
using cryptogram.Views;
using cryptogram.ViewModels;

namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ItemsPage : ContentPage
  {
    ItemsViewModel viewModel;

    public ItemsPage()
    {
      InitializeComponent();

      BindingContext = viewModel = new ItemsViewModel();
    }

    private Item LastItemSelected;
    async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
      //var item = args.SelectedItem as Item;
      //if (item == null)
      //  return;

      //if (LastItemSelected == item)
      //  await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));
      //else
      //  LastItemSelected = item;

      // Manually deselect item.
      //ItemsListView.SelectedItem = null;
    }
    async void OnItemTapped(object sender, ItemTappedEventArgs args)
    {
      var item = args.Item as Item;
      if (LastItemSelected == item)
        await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));
      LastItemSelected = item;
    }

    async void AddItem_Clicked(object sender, EventArgs e)
    {
      await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
    }

    async void RemoveItem_Clicked(object sender, EventArgs e)
    {
      Item item = (Item)ItemsListView.SelectedItem;
      if (item != null)
        MessagingCenter.Send(this, "DeleteItem", item);
      //await Navigation.PopModalAsync();
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();

      if (viewModel.Items.Count == 0)
        viewModel.LoadItemsCommand.Execute(null);
    }
  }
}