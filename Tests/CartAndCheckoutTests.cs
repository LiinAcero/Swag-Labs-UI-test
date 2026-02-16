using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using webUiFrame.PageObjects;

namespace webUiFrame.Tests;

[TestFixture]
public class CartAndCheckoutTests : PageTest
{
    private LoginPage _loginPage;
    private InventoryPage _inventoryPage;
    private ItemDetailsPage _itemDetailsPage;
    private CartPage _cartPage;
    private CheckoutInfoPage _checkoutInfoPage;
    private CheckoutOverviewPage _checkoutOverviewPage;
    private CheckoutCompletePage _checkoutCompletePage;

    [SetUp]
    public async Task Setup()
    {
        _loginPage = new LoginPage(Page);
        _inventoryPage = new InventoryPage(Page);
        _itemDetailsPage = new ItemDetailsPage(Page);
        _cartPage = new CartPage(Page);
        _checkoutInfoPage = new CheckoutInfoPage(Page);
        _checkoutOverviewPage = new CheckoutOverviewPage(Page);
        _checkoutCompletePage = new CheckoutCompletePage(Page);

        await _loginPage.GotoAsync();
        await _loginPage.LoginAsync("standard_user", "secret_sauce");
    }

    [Test]
    public async Task VerifyAddToCartButtonFunctionality()
    {
        int itemCount = await _inventoryPage.GetItemCountAsync();
        for (int i = 0; i < itemCount; i++)
        {
            Assert.That(await _inventoryPage.GetAddToCartButtonTextAsync(i), Is.EqualTo("Add to cart"));
            await _inventoryPage.AddItemToCartAsync(i);
            Assert.That(await _inventoryPage.GetAddToCartButtonTextAsync(i), Is.EqualTo("Remove"));
            await _inventoryPage.RemoveItemFromCartAsync(i);
            Assert.That(await _inventoryPage.GetAddToCartButtonTextAsync(i), Is.EqualTo("Add to cart"));
            
            await _inventoryPage.ClickItemNameAsync(i);
            Assert.That(await _itemDetailsPage.GetAddToCartButtonTextAsync(), Is.EqualTo("Add to cart"));
            await _itemDetailsPage.AddToCartAsync();
            Assert.That(await _itemDetailsPage.GetAddToCartButtonTextAsync(), Is.EqualTo("Remove"));
            await _itemDetailsPage.AddToCartAsync();
            Assert.That(await _itemDetailsPage.GetAddToCartButtonTextAsync(), Is.EqualTo("Add to cart"));
            await _itemDetailsPage.ClickBackToProductsAsync();
        }
    }

    [Test]
    public async Task VerifyCartDisplayAndRemoval()
    {
        string name0 = await _inventoryPage.GetItemNameAsync(0);
        await _inventoryPage.AddItemToCartAsync(0);
        await _inventoryPage.ClickCartAsync();

        Assert.That(await _cartPage.GetItemCountAsync(), Is.EqualTo(1));
        Assert.That(await _cartPage.GetItemNameAsync(0), Is.EqualTo(name0));
        Assert.That(await _cartPage.GetItemQuantityAsync(0), Is.EqualTo("1"));
        
        await _cartPage.RemoveItemAsync(0);
        Assert.That(await _cartPage.GetItemCountAsync(), Is.EqualTo(0));

        await _cartPage.ClickContinueShoppingAsync();
        await _inventoryPage.AddItemToCartAsync(0);
        await _inventoryPage.ClickCartAsync();
        Assert.That(await _cartPage.GetItemCountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task VerifyCheckoutProcess()
    {
        await _inventoryPage.AddItemToCartAsync(0);
        await _inventoryPage.AddItemToCartAsync(1);
        double price0 = (await _inventoryPage.GetAllItemPricesAsync())[0];
        double price1 = (await _inventoryPage.GetAllItemPricesAsync())[1];
        
        await _inventoryPage.ClickCartAsync();
        await _cartPage.ClickCheckoutAsync();

        await _checkoutInfoPage.ClickContinueAsync();
        Assert.That(await _checkoutInfoPage.GetErrorMessageAsync(), Is.EqualTo("Error: First Name is required"));

        await _checkoutInfoPage.EnterInfoAsync("John", "", "");
        await _checkoutInfoPage.ClickContinueAsync();
        Assert.That(await _checkoutInfoPage.GetErrorMessageAsync(), Is.EqualTo("Error: Last Name is required"));

        await _checkoutInfoPage.EnterInfoAsync("John", "Doe", "");
        await _checkoutInfoPage.ClickContinueAsync();
        Assert.That(await _checkoutInfoPage.GetErrorMessageAsync(), Is.EqualTo("Error: Postal Code is required"));

        await _checkoutInfoPage.EnterInfoAsync("John", "Doe", "12345");
        await _checkoutInfoPage.ClickContinueAsync();

        Assert.That(await _checkoutOverviewPage.GetItemCountAsync(), Is.EqualTo(2));
        string subtotalText = await _checkoutOverviewPage.GetSubtotalAsync();
        Assert.That(subtotalText, Does.Contain((price0 + price1).ToString("0.00")));

        await _checkoutOverviewPage.ClickFinishAsync();
        Assert.That(await _checkoutCompletePage.GetHeaderTextAsync(), Is.EqualTo("Thank you for your order!"));
        
        await _checkoutCompletePage.ClickBackHomeAsync();
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("inventory.html"));
    }

    [Test]
    public async Task VerifyCancelButtons()
    {
        await _inventoryPage.ClickCartAsync();
        await _cartPage.ClickCheckoutAsync();
        await _checkoutInfoPage.ClickCancelAsync();
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("cart.html"));

        await _cartPage.ClickCheckoutAsync();
        await _checkoutInfoPage.EnterInfoAsync("John", "Doe", "12345");
        await _checkoutInfoPage.ClickContinueAsync();
        await _checkoutOverviewPage.ClickCancelAsync();
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("inventory.html"));
    }
}
