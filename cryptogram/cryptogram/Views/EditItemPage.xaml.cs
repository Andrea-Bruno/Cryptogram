using System;
using Xamarin.Forms.Xaml;
namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class EditItemPage
  {
    private bool _edit;
    private CryptogramLibrary.Messaging.Contact _contact;
    public CryptogramLibrary.Messaging.Contact Contact
    {
      get => _contact;
      set
      {
        if (value == null)
        {
          Name.Text = "";
          PublicKey.Text = "";
        }
        else
        {
          Name.Text = value.Name;
          PublicKey.Text = value.PublicKey;
        }
        _contact = value;
        _edit = true;
      }
    }

    public EditItemPage()
    {
      InitializeComponent();
      _contact = new CryptogramLibrary.Messaging.Contact();
      BindingContext = this;
    }

    private new void Disappearing(object sender, EventArgs e)
    {
      //Save the contact
      if (Contact == null) return;
      if (_edit)
      {
        if (Contact.Name != Name.Text)
          Contact.Name = Name.Text;
        if (Contact.PublicKey != PublicKey.Text)
          Contact.PublicKey = PublicKey.Text;
        {
          Contact.Save();
          //ContactsPage.Istance.Update();
        }
      }
      else
      {
        Contact.Name = Name.Text;
        Contact.PublicKey = PublicKey.Text;
        CryptogramLibrary.Messaging.AddContact(Contact);
      }
    }

    private async void Remove_Clicked(object sender, EventArgs e)
    {
      CryptogramLibrary.Messaging.RemoveContact(Contact);
      Contact = null;
      await Navigation.PopAsync();//Exit
    }

    private void Share_Clicked(object sender, EventArgs e)
    {
      CryptogramLibrary.Functions.ShareText(PublicKey.Text);
    }
  }
}