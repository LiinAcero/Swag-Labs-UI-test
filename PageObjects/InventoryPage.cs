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
    private readonly ILocator _allItemsLink;
    private readonly ILocator _aboutLink;
    private readonly ILocator _logoutLink;
    private readonly ILocator _resetAppStateLink;
    private readonly ILocator _cartBadge;
    private readonly ILocator _cartLink;

    public InventoryPage(IPage page)
    {
        _page = page;
        _inventoryItems = page.Locator(".inventory_item");
        _menuButton = page.Locator("#react-burger-menu-btn");
        _menuList = page.Locator(".bm-item-list");
        _sortDropdown = page.Locator("[data-test=\"product-sort-container\"]");
        _inventoryItemNames = page.Locator(".inventory_item_name");
        _inventoryItemPrices = page.Locator(".inventory_item_price");
        _allItemsLink = page.Locator("#inventory_sidebar_link");
        _aboutLink = page.Locator("#about_sidebar_link");
        _logoutLink = page.Locator("#logout_sidebar_link");
        _resetAppStateLink = page.Locator("#reset_sidebar_link");
        _cartBadge = page.Locator(".shopping_cart_badge");
        _cartLink = page.Locator(".shopping_cart_link");
    }

    public async Task OpenMenuAsync()
    {
        await _menuButton.ClickAsync();
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

    public async Task ClickAllItemsAsync() => await _allItemsLink.ClickAsync();
    public async Task ClickAboutAsync() => await _aboutLink.ClickAsync();
    public async Task ClickLogoutAsync() => await _logoutLink.ClickAsync();
    public async Task ResetAppStateAsync() => await _resetAppStateLink.ClickAsync();

    public async Task AddItemToCartAsync(int index)
    {
        await _inventoryItems.Nth(index).Locator("button:has-text(\"Add to cart\")").ClickAsync();
    }

    public async Task RemoveItemFromCartAsync(int index)
    {
        await _inventoryItems.Nth(index).Locator("button:has-text(\"Remove\")").ClickAsync();
    }

    public async Task<string> GetAddToCartButtonTextAsync(int index)
    {
        return await _inventoryItems.Nth(index).Locator(".btn_inventory").InnerTextAsync();
    }

    public async Task ClickCartAsync() => await _cartLink.ClickAsync();

    public async Task<string?> GetCartBadgeTextAsync()
    {
        if (await _cartBadge.IsVisibleAsync())
        {
            return await _cartBadge.InnerTextAsync();
        }
        return null;
    }
}
