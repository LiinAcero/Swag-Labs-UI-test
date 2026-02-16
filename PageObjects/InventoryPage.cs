using Microsoft.Playwright;

namespace webUiFrame.PageObjects;

public class InventoryPage
{
    private readonly IPage _page;
    private readonly ILocator _inventoryItems;
    private readonly ILocator _menuButton;
    private readonly ILocator _menuList;
    private readonly ILocator _sortDropdown;
    private readonly ILocator _inventoryItemNames;
    private readonly ILocator _inventoryItemPrices;

    public InventoryPage(IPage page)
    {
        _page = page;
        _inventoryItems = page.Locator(".inventory_item");
        _menuButton = page.Locator("#react-burger-menu-btn");
        _menuList = page.Locator(".bm-item-list");
        _sortDropdown = page.Locator("[data-test=\"product-sort-container\"]");
        _inventoryItemNames = page.Locator(".inventory_item_name");
        _inventoryItemPrices = page.Locator(".inventory_item_price");
    }

    public async Task OpenMenuAsync()
    {
        await _menuButton.ClickAsync();
        // Wait for the menu to be visible
        await _menuList.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }

    public async Task<bool> IsMenuVisibleAsync()
    {
        return await _menuList.IsVisibleAsync();
    }

    public async Task<IReadOnlyList<string>> GetMenuItemsAsync()
    {
        var items = await _menuList.Locator("a").AllInnerTextsAsync();
        return items;
    }

    public async Task SelectSortOptionAsync(string optionValue)
    {
        await _sortDropdown.SelectOptionAsync(optionValue);
    }

    public async Task<IReadOnlyList<string>> GetAllItemNamesAsync()
    {
        return await _inventoryItemNames.AllInnerTextsAsync();
    }

    public async Task<IReadOnlyList<double>> GetAllItemPricesAsync()
    {
        var priceTexts = await _inventoryItemPrices.AllInnerTextsAsync();
        return priceTexts.Select(p => double.Parse(p.Replace("$", ""))).ToList();
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
