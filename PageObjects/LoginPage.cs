using Microsoft.Playwright;

namespace webUiFrame.PageObjects;

public class LoginPage
{
    private readonly IPage _page;
    private readonly ILocator _usernameInput;
    private readonly ILocator _passwordInput;
    private readonly ILocator _loginButton;
    private readonly ILocator _errorMessage;

    public LoginPage(IPage page)
    {
        _page = page;
        _usernameInput = page.Locator("[data-test=\"username\"]");
        _passwordInput = page.Locator("[data-test=\"password\"]");
        _loginButton = page.Locator("[data-test=\"login-button\"]");
        _errorMessage = page.Locator("[data-test=\"error\"]");
    }

    public async Task GotoAsync()
    {
        await _page.GotoAsync("https://www.saucedemo.com/");
    }

    public async Task LoginAsync(string username, string password)
    {
        await _usernameInput.FillAsync(username);
        await _passwordInput.FillAsync(password);
        await _loginButton.ClickAsync();
    }

    public async Task<string?> GetErrorMessageAsync()
    {
        if (await _errorMessage.IsVisibleAsync())
        {
            return await _errorMessage.InnerTextAsync();
        }
        return null;
    }
}
