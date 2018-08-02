using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cryptogram.Models;

[assembly: Xamarin.Forms.Dependency(typeof(cryptogram.Services.MockDataStore))]
namespace cryptogram.Services
{
  public class MockDataStore : IDataStore<Item>
  {
    List<Item> items;

    public MockDataStore()
    {
      items = new List<Item>();

      System.Security.Cryptography.RSACryptoServiceProvider RSA = new System.Security.Cryptography.RSACryptoServiceProvider();
      string PK = Convert.ToBase64String(RSA.ExportCspBlob(false));
      System.Diagnostics.Debug.Print(PK);

      var mockItems = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Mario", PublicKey=PK },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Luigi", PublicKey=PK },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Gennaro", PublicKey=PK },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Peppone", PublicKey=PK },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Andrea", PublicKey = "BgIAAACkAABSU0ExAAQAAAEAAQCFZy1gQ+ks9C+jKZ8n5ypEgYWXLZWFfyXpbsmOURNuvSGenCqOUB7DAtzWQlooKDSyq8K+KUW0SS9ks4IJwpAzfEWkX4aJ6pgMzUgb4LJiaoGUNONWTDDM+UYpgCA2C5jLcV/PDhVgDZexIfMAZsg9AuRIEwK6zQW9d/yCIH50uw==" },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Bruno", PublicKey  = "BgIAAACkAABSU0ExAAQAAAEAAQCBVjsSHlUACoyB57m7jFiCGoxE9vY4Lg+CM4s156B/8M4ldPaUt27/7yRn4llcvq3nOlDPpde6dkCx4t7fVPkkHNSlVY6LDBG2YCoe03fP2275Y7T9u3TM4NleD8uthdSk/sW4YrpMVPoGXTnCXCKSgxK0i/AuSO+4vrqwqUb73A==" },
            };
      items = mockItems.OrderBy(o => o.ContactName).ToList();

      //foreach (var item in mockItems)
      //{
      //  items.Add(item);
      //}
    }

    public async Task<bool> AddItemAsync(Item item)
    {
      items.Add(item);

      return await Task.FromResult(true);
    }

    public async Task<bool> UpdateItemAsync(Item item)
    {
      var _item = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
      items.Remove(_item);
      items.Add(item);

      return await Task.FromResult(true);
    }

    public async Task<bool> DeleteItemAsync(string id)
    {
      var _item = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
      items.Remove(_item);

      return await Task.FromResult(true);
    }

    public async Task<Item> GetItemAsync(string id)
    {
      return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
    }

    public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
    {
      return await Task.FromResult(items);
    }
  }
}