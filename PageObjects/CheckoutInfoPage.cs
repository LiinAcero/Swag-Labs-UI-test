using Microsoft.Playwright;

namespace webUiFrame.PageObjects;

public class CheckoutInfoPage
{
    private readonly IPage _page;
    private readonly ILocator _firstNameInput;
    private readonly ILocator _lastNameInput;
    private readonly ILocator _postalCodeInput;
    private readonly ILocator _continueButton;
    private readonly ILocator _cancelButton;
    private readonly ILocator _errorMessage;

    public CheckoutInfoPage(IPage page)
    {
        _page = page;
        _firstNameInput = page.Locator("[data-test=\"firstName\"]");
        _lastNameInput = page.Locator("[data-test=\"lastName\"]");
        _postalCodeInput = page.Locator("[data-test=\"postalCode\"]");
        _continueButton = page.Locator("[data-test=\"continue\"]");
        _cancelButton = page.Locator("[data-test=\"cancel\"]");
        _errorMessage = page.Locator("[data-test=\"error\"]");
    }

    public async Task EnterInfoAsync(string firstName, string lastName, string postalCode)
    {
        await _firstNameInput.FillAsync(firstName);
        await _lastNameInput.FillAsync(lastName);
        await _postalCodeInput.FillAsync(postalCode);
    }

    public async Task ClickContinueAsync() => await _continueButton.ClickAsync();

    public async Task ClickCancelAsync() => await _cancelButton.ClickAsync();

    public async Task<string?> GetErrorMessageAsync()
    {
        if (await _errorMessage.IsVisibleAsync())
        {
            return await _errorMessage.InnerTextAsync();
        }
        return null;
    }
}
