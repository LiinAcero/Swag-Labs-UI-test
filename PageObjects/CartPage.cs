using Microsoft.Playwright;

namespace webUiFrame.PageObjects;

public class CartPage
{
    private readonly IPage _page;
    private readonly ILocator _cartItems;
    private readonly ILocator _continueShoppingButton;
    private readonly ILocator _checkoutButton;

    public CartPage(IPage page)
    {
        _page = page;
        _cartItems = page.Locator(".cart_item");
        _continueShoppingButton = page.Locator("[data-test=\"continue-shopping\"]");
        _checkoutButton = page.Locator("[data-test=\"checkout\"]");
    }

    public async Task<int> GetItemCountAsync() => await _cartItems.CountAsync();

    public async Task<string> GetItemNameAsync(int index) => await _cartItems.Nth(index).Locator(".inventory_item_name").InnerTextAsync();

    public async Task<string> GetItemDescriptionAsync(int index) => await _cartItems.Nth(index).Locator(".inventory_item_desc").InnerTextAsync();

    public async Task<string> GetItemQuantityAsync(int index) => await _cartItems.Nth(index).Locator(".cart_quantity").InnerTextAsync();

    public async Task RemoveItemAsync(int index) => await _cartItems.Nth(index).Locator("button:has-text(\"Remove\")").ClickAsync();

    public async Task ClickContinueShoppingAsync() => await _continueShoppingButton.ClickAsync();

    public async Task ClickCheckoutAsync() => await _checkoutButton.ClickAsync();
}
