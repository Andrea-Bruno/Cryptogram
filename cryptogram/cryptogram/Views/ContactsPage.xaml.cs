using cryptogram.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ContactsPage
  {
    public static ContactsPage Istance;
    public ContactsPage()
    {
      InitializeComponent();
      Istance = this;

      //this.Focused += delegate
      //          {
      //            Update();
      //          };

    }

    private CryptogramLibrary.Messaging.Contact _lastItemSelected;

		private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
		{
			//var item = args.SelectedItem as Item;
			//if (item == null)
			//	return;

			//if (LastItemSelected == item)
			//	await Navigation.PushAsync(new ChatRoom(new ItemDetailViewModel(item)));
			//else
			//	LastItemSelected = item;

			//Manually deselect item.
		 //ItemsListView.SelectedItem = null;
		}

		private async void OnItemTapped(object sender, ItemTappedEventArgs args)
    {
      var item = args.Item as CryptogramLibrary.Messaging.Contact;
      if (_lastItemSelected == item)
        await Navigation.PushAsync(new ChatRoom(new ItemDetailViewModel(Clone2Contect(item))));
      _lastItemSelected = item;
    }

    private async void AddItem_Clicked(object sender, EventArgs e)
    {
      await Navigation.PushAsync(new EditItemPage());
    }

    private async void Edit_Clicked(object sender, EventArgs e)
    {
      var item = (CryptogramLibrary.Messaging.Contact)ItemsListView.SelectedItem;
      if (item != null)
        await Navigation.PushAsync(new EditItemPage() { Contact = Clone2Contect(item) });
    }

    protected override void OnAppearing()
    {
      base.OnAppearing();
      _lastItemSelected = null;
      //Device.BeginInvokeOnMainThread(delegate
      //{
      //  ItemsListView.ItemsSource = Core.Messaging.GetContacts();
      //});
      Find_Unfocused(null, null);
      PopulateList(CryptogramLibrary.Messaging.GetContacts());
    }

    private static readonly Dictionary<CryptogramLibrary.Messaging.Contact, CryptogramLibrary.Messaging.Contact> List = new Dictionary<CryptogramLibrary.Messaging.Contact, CryptogramLibrary.Messaging.Contact>();
    //static public System.Collections.ObjectModel.ObservableCollection<Core.Messaging.Contact> List = new System.Collections.ObjectModel.ObservableCollection<Core.Messaging.Contact>();
    public void PopulateList(CryptogramLibrary.Messaging.Contact[] contacts)
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
      var toRemove = new List<CryptogramLibrary.Messaging.Contact>();
      foreach (var I in List)
      {
        if (!contacts.Contains(I.Value))
          toRemove.Add(I.Key);
      }
      toRemove.ForEach(x => { List.Remove(x); });

      foreach (var x in contacts)
      {
        if (!List.Values.Contains(x))
          List.Add((CryptogramLibrary.Messaging.Contact)x.Clone(), x);
      }
      ItemsListView.ItemsSource = null; // Se non lo annullo e lo reimposto non funziona su Android

      var values = List.Keys;
      var sorted = values.OrderBy(o => o.Name).ToList();
      ItemsListView.ItemsSource = sorted;
      ItemsListView.EndRefresh();
    }

    private CryptogramLibrary.Messaging.Contact Clone2Contect(CryptogramLibrary.Messaging.Contact cloneContact)
    {
      return List[cloneContact];
      //return List.ToArray().First(x => x.Value == CloneContact).Key;
    }

    private Color? _watermark;
    private void Find_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (_watermark != null) return;
      var txt = Find.Text.ToLower();
      var contacts = CryptogramLibrary.Messaging.GetContacts();
      PopulateList(contacts.ToList().FindAll(x => x.Name.ToLower().Contains(txt)).ToArray());
    }

    private void Find_Focused(object sender, FocusEventArgs e)
    {
      if (_watermark == null) return;
      Find.TextColor = (Color)_watermark;
      Find.Text = "";
      _watermark = null;
    }

    private void Find_Unfocused(object sender, FocusEventArgs e)
    {
      if (!string.IsNullOrEmpty(Find.Text)) return;
      _watermark = Find.TextColor;
      Find.TextColor = Color.FromRgba(128, 128, 128, 128);
      Find.Text = cryptogram.Resources.Dictionary.Search;
    }
  }
}