using Microsoft.Playwright;

namespace webUiFrame.PageObjects;

public class CheckoutOverviewPage
{
    private readonly IPage _page;
    private readonly ILocator _cartItems;
    private readonly ILocator _subtotalLabel;
    private readonly ILocator _taxLabel;
    private readonly ILocator _totalLabel;
    private readonly ILocator _finishButton;
    private readonly ILocator _cancelButton;

    public CheckoutOverviewPage(IPage page)
    {
        _page = page;
        _cartItems = page.Locator(".cart_item");
        _subtotalLabel = page.Locator(".summary_subtotal_label");
        _taxLabel = page.Locator(".summary_tax_label");
        _totalLabel = page.Locator(".summary_total_label");
        _finishButton = page.Locator("[data-test=\"finish\"]");
        _cancelButton = page.Locator("[data-test=\"cancel\"]");
    }

    public async Task<int> GetItemCountAsync() => await _cartItems.CountAsync();

    public async Task<string> GetItemNameAsync(int index) => await _cartItems.Nth(index).Locator(".inventory_item_name").InnerTextAsync();

    public async Task<string> GetSubtotalAsync() => await _subtotalLabel.InnerTextAsync();

    public async Task<string> GetTaxAsync() => await _taxLabel.InnerTextAsync();

    public async Task<string> GetTotalAsync() => await _totalLabel.InnerTextAsync();

    public async Task ClickFinishAsync() => await _finishButton.ClickAsync();

    public async Task ClickCancelAsync() => await _cancelButton.ClickAsync();
}
