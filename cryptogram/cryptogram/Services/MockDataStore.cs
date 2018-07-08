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
      var mockItems = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Mario", PublicKey="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Luigi", PublicKey="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Gennaro", PublicKey="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Peppone", PublicKey="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Andrea", PublicKey="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), ContactName = "Bruno", PublicKey="This is an item description." },
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