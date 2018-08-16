using cryptogram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ContactsPage : ContentPage
  {
    static public ContactsPage Istance;
    public ContactsPage()
    {
      InitializeComponent();
      Istance = this;

      //this.Focused += delegate
      //          {
      //            Update();
      //          };

    }

    private Core.Messaging.Contact LastItemSelected;
    async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
    {
      //var item = args.SelectedItem as Item;
      //if (item == null)
      //  return;

      //if (LastItemSelected == item)
      //  await Navigation.PushAsync(new ChatRoom(new ItemDetailViewModel(item)));
      //else
      //  LastItemSelected = item;

      // Manually deselect item.
      //ItemsListView.SelectedItem = null;
    }
    async void OnItemTapped(object sender, ItemTappedEventArgs args)
    {
      var item = args.Item as Core.Messaging.Contact;
      if (LastItemSelected == item)
        await Navigation.PushAsync(new ChatRoom(new ItemDetailViewModel(item)));
      LastItemSelected = item;
    }

    async void AddItem_Clicked(object sender, EventArgs e)
    {
      await Navigation.PushAsync(new EditItemPage());
    }

    async void Edit_Clicked(object sender, EventArgs e)
    {
      Core.Messaging.Contact item = (Core.Messaging.Contact)ItemsListView.SelectedItem;
      if (item != null)
        await Navigation.PushAsync(new EditItemPage() { Contact = item });
    }

    private Color? Watermark;
    protected override void OnAppearing()
    {
      base.OnAppearing();
      LastItemSelected = null;
      if (string.IsNullOrEmpty(Find.Text))
      {
        Watermark = Find.TextColor;
        Find.TextColor = Color.FromRgba(128, 128, 128, 128);
        Find.Text = cryptogram.Resources.Dictionary.Search;
      };
      //Device.BeginInvokeOnMainThread(delegate
      //{
      //  ItemsListView.ItemsSource = Core.Messaging.GetContacts();
      //});
      PopulateList(Core.Messaging.GetContacts());
    }

    static Dictionary<Core.Messaging.Contact, Core.Messaging.Contact> ListC = new Dictionary<Core.Messaging.Contact, Core.Messaging.Contact>();
    static public System.Collections.ObjectModel.ObservableCollection<Core.Messaging.Contact> List = new System.Collections.ObjectModel.ObservableCollection<Core.Messaging.Contact>();
    public void PopulateList(Core.Messaging.Contact[] Contacts)
    {
      ItemsListView.SelectedItem = null;

      //ItemsListView.BeginRefresh();
      //ListC.Clear();
      //foreach (var X in Contacts)
      //{
      //  ListC.Add(X, (Core.Messaging.Contact)X.Clone()); //Se non clono non funziona
      //}
      //ItemsListView.EndRefresh();
      //ItemsListView.ItemsSource = null; //Se Non lo tolgo e rimetto non funziona;
      //ItemsListView.ItemsSource = ListC.Values;
      //return;

      ItemsListView.BeginRefresh();
      var ToRemove = new List<Core.Messaging.Contact>();
      foreach (var X in ListC.Keys)
      {
        if (!Contacts.Contains(X))
          ToRemove.Add(X);
      }
      ToRemove.ForEach(X => { ListC.Remove(X); });

      foreach (var X in Contacts)
      {
        if (!ListC.Keys.Contains(X))
          ListC.Add(X, (Core.Messaging.Contact)X.Clone());
      }
      ItemsListView.ItemsSource = null; // Se non lo annullo e lo reimposto non funziona su Android

      var Values = ListC.Values;
      var Sorted = Values.OrderBy(o => o.Name).ToList();
      ItemsListView.ItemsSource = Sorted;
      ItemsListView.EndRefresh();
    }

    private void Find_TextChanged(object sender, TextChangedEventArgs e)
    {
      var txt = Find.Text.ToLower();
      var Contacts = Core.Messaging.GetContacts();
      var Findes = Contacts.ToList().FindAll(X => X.Name.ToLower().Contains(txt));
      PopulateList(Findes.ToArray());
    }

    private void Find_Focused(object sender, FocusEventArgs e)
    {
      if (Watermark != null)
      {
        Find.TextColor = (Color)Watermark;
        Watermark = null;
        Find.Text = "";
      }
    }
  }
}