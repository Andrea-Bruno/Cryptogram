using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using cryptogram.Views;
using cryptogram.Resources;

namespace cryptogram.ViewModels
{
  public class ItemsViewModel : BaseViewModel
  {
    public ObservableCollection<Core.Messaging.Contact> Items { get; set; }
    public Command LoadItemsCommand { get; set; }

    public ItemsViewModel()
    {
      Title = Dictionary.Contacts;
      Items = new ObservableCollection<Core.Messaging.Contact>();
      LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

      MessagingCenter.Subscribe<NewItemPage, Core.Messaging.Contact>(this, "AddItem", async (obj, item) =>
      {
        var _item = item as Core.Messaging.Contact;
      if  (Core.Messaging.AddContact(_item))
          Items.Add(_item);
      });

      MessagingCenter.Subscribe<ItemsPage, Core.Messaging.Contact>(this, "DeleteItem", async (obj, item) =>
      {
        var _item = item as Core.Messaging.Contact;
        Items.Remove(_item);
        Core.Messaging.RemoveContact(_item.PublicKey);
      });

      MessagingCenter.Subscribe<ItemDetailPage, Core.Messaging.Contact>(this, "DeleteItem", async (obj, item) =>
      {
        var _item = item as Core.Messaging.Contact;
        Items.Remove(_item);
        Core.Messaging.RemoveContact(_item.PublicKey);
      });

      MessagingCenter.Subscribe<ItemDetailPage, Core.Messaging.Contact>(this, "UpdateItem", async (obj, item) =>
      {
        var _item = item as Core.Messaging.Contact;
        var Contact = new Core.Messaging.Contact() { Name = _item.Name, PublicKey = _item.PublicKey };
        Items.Add(Contact);
        Core.Messaging.AddContact(Contact);
        Items.Remove(_item);
        Core.Messaging.RemoveContact(_item.PublicKey);
      });


    }

    async Task ExecuteLoadItemsCommand()
    {
      if (IsBusy)
        return;

      IsBusy = true;

      try
      {
        Items.Clear();
        var items = Core.Messaging.GetContacts();
        foreach (var item in items)
        {
          Items.Add(item);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
      }
      finally
      {
        IsBusy = false;
      }
    }
  }
}