using Microsoft.Playwright;

namespace webUiFrame.PageObjects;

public class CheckoutCompletePage
{
    private readonly IPage _page;
    private readonly ILocator _completeHeader;
    private readonly ILocator _backHomeButton;

    public CheckoutCompletePage(IPage page)
    {
        _page = page;
        _completeHeader = page.Locator(".complete-header");
        _backHomeButton = page.Locator("[data-test=\"back-to-products\"]");
    }

    public async Task<string> GetHeaderTextAsync() => await _completeHeader.InnerTextAsync();

    public async Task ClickBackHomeAsync() => await _backHomeButton.ClickAsync();
}
