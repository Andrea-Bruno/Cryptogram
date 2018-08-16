using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace cryptogram.Views
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class EditItemPage : ContentPage
  {
    private Boolean Edit = false;
    private Core.Messaging.Contact _Contact;
    public Core.Messaging.Contact Contact
    {
      get { return _Contact; }
      set
      {
        Name.Text = value.Name;
        PublicKey.Text = value.PublicKey;
        _Contact = value;
        Edit = true;

      }
    }

    public EditItemPage()
    {
      InitializeComponent();
      _Contact = new Core.Messaging.Contact();
      BindingContext = this;
    }
    async void Disappearing(object sender, EventArgs e)
    {
      //Save the contact
      if (Contact != null)
      {
        if (Edit)
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
          Core.Messaging.AddContact(Contact);
        }

      }
    }

    async private void Exit_Clicked(object sender, EventArgs e)
    {
      await Navigation.PopModalAsync();//Exit
    }

    async private void Remove_Clicked(object sender, EventArgs e)
    {
      Core.Messaging.RemoveContact(Contact);
      Contact = null;
      await Navigation.PopModalAsync();//Exit
    }

    private void Share_Clicked(object sender, EventArgs e)
    {
      Core.Functions.ShareText(PublicKey.Text);
    }
  }
}