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
        await Navigation.PushAsync(new ChatRoom(new ItemDetailViewModel(Clone2Contect(item))));
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
        await Navigation.PushAsync(new EditItemPage() { Contact = Clone2Contect(item) });
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();
      LastItemSelected = null;
      //Device.BeginInvokeOnMainThread(delegate
      //{
      //  ItemsListView.ItemsSource = Core.Messaging.GetContacts();
      //});
      Find_Unfocused(null, null);
      PopulateList(Core.Messaging.GetContacts());
    }

    static Dictionary<Core.Messaging.Contact, Core.Messaging.Contact> List = new Dictionary<Core.Messaging.Contact, Core.Messaging.Contact>();
    //static public System.Collections.ObjectModel.ObservableCollection<Core.Messaging.Contact> List = new System.Collections.ObjectModel.ObservableCollection<Core.Messaging.Contact>();
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
      foreach (var I in List)
      {
        if (!Contacts.Contains(I.Value))
          ToRemove.Add(I.Key);
      }
      ToRemove.ForEach(X => { List.Remove(X); });

      foreach (var X in Contacts)
      {
        if (!List.Values.Contains(X))
          List.Add((Core.Messaging.Contact)X.Clone(), X);
      }
      ItemsListView.ItemsSource = null; // Se non lo annullo e lo reimposto non funziona su Android

      var Values = List.Keys;
      var Sorted = Values.OrderBy(o => o.Name).ToList();
      ItemsListView.ItemsSource = Sorted;
      ItemsListView.EndRefresh();
    }

    private Core.Messaging.Contact Clone2Contect(Core.Messaging.Contact CloneContact)
    {
      return List[CloneContact];
      //return List.ToArray().First(x => x.Value == CloneContact).Key;
    }

    private Color? Watermark;
    private void Find_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (Watermark == null)
      {
        var txt = Find.Text.ToLower();
        var Contacts = Core.Messaging.GetContacts();
        var Findes = Contacts.ToList().FindAll(X => X.Name.ToLower().Contains(txt));
        PopulateList(Findes.ToArray());
      }
    }

    private void Find_Focused(object sender, FocusEventArgs e)
    {
      if (Watermark != null)
      {
        Find.TextColor = (Color)Watermark;
        Find.Text = "";
        Watermark = null;
      }
    }

    private void Find_Unfocused(object sender, FocusEventArgs e)
    {
      if (string.IsNullOrEmpty(Find.Text))
      {
        Watermark = Find.TextColor;
        Find.TextColor = Color.FromRgba(128, 128, 128, 128);
        Find.Text = cryptogram.Resources.Dictionary.Search;
      }
    }
  }
}