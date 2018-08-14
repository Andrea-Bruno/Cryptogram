namespace cryptogram.ViewModels
{
  public class ItemDetailViewModel : BaseViewModel
  {
    public Core.Messaging.Contact Item { get; set; }
    public ItemDetailViewModel(Core.Messaging.Contact item = null)
    {
      //Core.Messaging.RecipientPublicKeyBase64 = item.PublicKey; 
      Title = item?.Name;
      Item = item;
    }
  }
}
