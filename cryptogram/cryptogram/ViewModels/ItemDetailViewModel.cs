using System;

using cryptogram.Models;

namespace cryptogram.ViewModels
{
  public class ItemDetailViewModel : BaseViewModel
  {
    public Item Item { get; set; }
    public ItemDetailViewModel(Item item = null)
    {
      //Core.Messaging.RecipientPublicKeyBase64 = item.PublicKey; 
      Title = item?.ContactName;
      Item = item;
    }
  }
}
