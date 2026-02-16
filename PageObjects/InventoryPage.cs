using Microsoft.Playwright;

namespace webUiFrame.PageObjects;

public class InventoryPage
{
    private readonly IPage _page;
    private readonly ILocator _inventoryItems;

    public InventoryPage(IPage page)
    {
        _page = page;
        _inventoryItems = page.Locator(".inventory_item");
    }

    public async Task<int> GetItemCountAsync()
    {
        return await _inventoryItems.CountAsync();
    }

    public async Task ClickItemNameAsync(int index)
    {
        await _inventoryItems.Nth(index).Locator(".inventory_item_name").ClickAsync();
    }
    
    public async Task<string> GetItemNameAsync(int index)
    {
        return await _inventoryItems.Nth(index).Locator(".inventory_item_name").InnerTextAsync();
    }
}
