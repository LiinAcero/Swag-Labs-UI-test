using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using webUiFrame.PageObjects;

namespace webUiFrame.Tests;

[TestFixture]
public class LoginTests : PageTest
{
    private LoginPage _loginPage;

    [SetUp]
    public async Task Setup()
    {
        _loginPage = new LoginPage(Page);
        await _loginPage.GotoAsync();
    }

    [Test]
    public async Task SuccessfulLoginTest()
    {
        await _loginPage.LoginAsync("standard_user", "secret_sauce");
        
        // After successful login, we expect to be redirected to the inventory page
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("inventory.html"));
        
        var inventoryContainer = Page.Locator(".inventory_container");
        await Expect(inventoryContainer).ToBeVisibleAsync();
    }

    [Test]
    public async Task LockedOutUserLoginTest()
    {
        await _loginPage.LoginAsync("locked_out_user", "secret_sauce");
        
        var error = await _loginPage.GetErrorMessageAsync();
        Assert.That(error, Does.Contain("Epic sadface: Sorry, this user has been locked out."));
    }

    [Test]
    [TestCase("invalid_user", "secret_sauce", "Epic sadface: Username and password do not match any user in this service")]
    [TestCase("standard_user", "wrong_password", "Epic sadface: Username and password do not match any user in this service")]
    [TestCase("", "", "Epic sadface: Username is required")]
    [TestCase("standard_user", "", "Epic sadface: Password is required")]
    public async Task InvalidLoginTests(string username, string password, string expectedErrorMessage)
    {
        await _loginPage.LoginAsync(username, password);
        
        var error = await _loginPage.GetErrorMessageAsync();
        Assert.That(error, Is.EqualTo(expectedErrorMessage));
    }
}
