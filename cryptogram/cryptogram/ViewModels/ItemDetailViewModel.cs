namespace cryptogram.ViewModels
{
  public class ItemDetailViewModel : BaseViewModel
  {
    public CryptogramLibrary.Messaging.Contact Item { get; set; }
    public ItemDetailViewModel(CryptogramLibrary.Messaging.Contact item = null)
    {
      Title = item?.Name;
      Item = item;
    }
  }
}
