using Microsoft.Playwright;

namespace webUiFrame.PageObjects;

public class ItemDetailsPage
{
    private readonly IPage _page;
    private readonly ILocator _itemName;
    private readonly ILocator _itemDescription;
    private readonly ILocator _itemPrice;
    private readonly ILocator _itemImage;
    private readonly ILocator _backToProductsButton;

    public ItemDetailsPage(IPage page)
    {
        _page = page;
        _itemName = page.Locator(".inventory_details_name");
        _itemDescription = page.Locator(".inventory_details_desc");
        _itemPrice = page.Locator(".inventory_details_price");
        _itemImage = page.Locator(".inventory_details_img");
        _backToProductsButton = page.Locator("[data-test=\"back-to-products\"]");
    }

    public async Task<string> GetNameAsync() => await _itemName.InnerTextAsync();
    public async Task<string> GetDescriptionAsync() => await _itemDescription.InnerTextAsync();
    public async Task<string> GetPriceAsync() => await _itemPrice.InnerTextAsync();
    public async Task<string?> GetImageAltAsync() => await _itemImage.GetAttributeAsync("alt");
    
    public async Task<bool> IsNameVisibleAsync() => await _itemName.IsVisibleAsync();
    public async Task<bool> IsDescriptionVisibleAsync() => await _itemDescription.IsVisibleAsync();
    public async Task<bool> IsPriceVisibleAsync() => await _itemPrice.IsVisibleAsync();
    public async Task<bool> IsImageVisibleAsync() => await _itemImage.IsVisibleAsync();

    public async Task<string> GetNameFontFamilyAsync() => await _itemName.EvaluateAsync<string>("el => window.getComputedStyle(el).fontFamily");
    public async Task<string> GetDescriptionFontFamilyAsync() => await _itemDescription.EvaluateAsync<string>("el => window.getComputedStyle(el).fontFamily");
    public async Task<string> GetPriceFontFamilyAsync() => await _itemPrice.EvaluateAsync<string>("el => window.getComputedStyle(el).fontFamily");

    public async Task ClickBackToProductsAsync()
    {
        await _backToProductsButton.ClickAsync();
    }
}
