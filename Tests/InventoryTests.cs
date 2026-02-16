using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using webUiFrame.PageObjects;

namespace webUiFrame.Tests;

[TestFixture]
public class InventoryTests : PageTest
{
    private LoginPage _loginPage;
    private InventoryPage _inventoryPage;
    private ItemDetailsPage _itemDetailsPage;

    [SetUp]
    public async Task Setup()
    {
        _loginPage = new LoginPage(Page);
        _inventoryPage = new InventoryPage(Page);
        _itemDetailsPage = new ItemDetailsPage(Page);
        
        await _loginPage.GotoAsync();
        await _loginPage.LoginAsync("standard_user", "secret_sauce");
    }

    [Test]
    public async Task VerifyAllItemsDisplayedTest()
    {
        int itemCount = await _inventoryPage.GetItemCountAsync();
        Assert.That(itemCount, Is.GreaterThan(0), "No items found on inventory page.");

        List<string> issues = new List<string>();

        for (int i = 0; i < itemCount; i++)
        {
            string itemName = await _inventoryPage.GetItemNameAsync(i);
            TestContext.WriteLine($"Checking item: {itemName}");

            await _inventoryPage.ClickItemNameAsync(i);

            if (!await _itemDetailsPage.IsNameVisibleAsync()) issues.Add($"{itemName}: Name not visible");
            if (!await _itemDetailsPage.IsDescriptionVisibleAsync()) issues.Add($"{itemName}: Description not visible");
            if (!await _itemDetailsPage.IsPriceVisibleAsync()) issues.Add($"{itemName}: Price not visible");
            if (!await _itemDetailsPage.IsImageVisibleAsync()) issues.Add($"{itemName}: Image not visible");

            string? altText = await _itemDetailsPage.GetImageAltAsync();
            if (string.IsNullOrWhiteSpace(altText))
            {
                issues.Add($"{itemName}: Image missing audio descriptor (alt text)");
            }
            else
            {
                TestContext.WriteLine($"{itemName} alt text: {altText}");
            }

            string nameFont = await _itemDetailsPage.GetNameFontFamilyAsync();
            string descFont = await _itemDetailsPage.GetDescriptionFontFamilyAsync();
            string priceFont = await _itemDetailsPage.GetPriceFontFamilyAsync();
            TestContext.WriteLine($"{itemName} fonts: Name={nameFont}, Description={descFont}, Price={priceFont}");

            await _itemDetailsPage.ClickBackToProductsAsync();
        }

        if (issues.Any())
        {
            Assert.Fail("Issues found:\n" + string.Join("\n", issues));
        }
    }

    [Test]
    public async Task VerifySidebarMenuTest()
    {
        await _inventoryPage.OpenMenuAsync();
        
        Assert.That(await _inventoryPage.IsMenuVisibleAsync(), Is.True, "Sidebar menu should be visible after clicking the menu button.");
        
        var menuItems = await _inventoryPage.GetMenuItemsAsync();
        var expectedItems = new[] { "All Items", "About", "Logout", "Reset App State" };
        
        Assert.That(menuItems, Is.EquivalentTo(expectedItems), "Sidebar menu should contain all expected items.");
    }

    [Test]
    public async Task VerifySidebarLogoutTest()
    {
        await _inventoryPage.OpenMenuAsync();
        await _inventoryPage.ClickLogoutAsync();
        
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("https://www.saucedemo.com/"));
        var loginButton = Page.Locator("[data-test=\"login-button\"]");
        await Expect(loginButton).ToBeVisibleAsync();
    }

    [Test]
    public async Task VerifySidebarAboutTest()
    {
        await _inventoryPage.OpenMenuAsync();
        await _inventoryPage.ClickAboutAsync();
        
        Assert.That(Page.Url, Does.Contain("saucelabs.com"));
    }

    [Test]
    public async Task VerifySidebarAllItemsTest()
    {
        await _inventoryPage.ClickItemNameAsync(0);
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("inventory-item.html"));
        
        await _inventoryPage.OpenMenuAsync();
        await _inventoryPage.ClickAllItemsAsync();
        
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("inventory.html"));
    }

    [Test]
    public async Task VerifySidebarResetAppStateTest()
    {
        await _inventoryPage.AddItemToCartAsync(0);
        var badgeText = await _inventoryPage.GetCartBadgeTextAsync();
        Assert.That(badgeText, Is.EqualTo("1"), "Cart should have 1 item.");

        await _inventoryPage.OpenMenuAsync();
        await _inventoryPage.ResetAppStateAsync();
        
        var badgeTextAfter = await _inventoryPage.GetCartBadgeTextAsync();
        Assert.That(badgeTextAfter, Is.Null, "Cart should be empty after reset.");
    }

    [Test]
    public async Task VerifyAllItemsAccessibilityAndAvailabilityTest()
    {
        int itemCount = await _inventoryPage.GetItemCountAsync();
        Assert.That(itemCount, Is.GreaterThan(0));

        for (int i = 0; i < itemCount; i++)
        {
            var item = Page.Locator(".inventory_item").Nth(i);
            
            var addToCartButton = item.Locator("button:has-text(\"Add to cart\")");
            await Expect(addToCartButton).ToBeVisibleAsync();
            await Expect(addToCartButton).ToBeEnabledAsync();

            var nameLink = item.Locator(".inventory_item_name");
            await Expect(nameLink).ToBeVisibleAsync();
            var tagName = await nameLink.EvaluateAsync<string>("el => el.tagName");
            
            var img = item.Locator("img.inventory_item_img");
            string? alt = await img.GetAttributeAsync("alt");
            Assert.That(alt, Is.Not.Null.And.Not.Empty, $"Item {i} image missing alt text");
        }
    }

    [Test]
    [TestCase("az", "Name (A to Z)")]
    [TestCase("za", "Name (Z to A)")]
    [TestCase("lohi", "Price (low to high)")]
    [TestCase("hilo", "Price (high to low)")]
    public async Task VerifySortingTest(string sortOption, string description)
    {
        TestContext.WriteLine($"Testing sort option: {description} ({sortOption})");
        await _inventoryPage.SelectSortOptionAsync(sortOption);

        if (sortOption == "az" || sortOption == "za")
        {
            var names = await _inventoryPage.GetAllItemNamesAsync();
            var sortedNames = sortOption == "az" ? names.OrderBy(n => n).ToList() : names.OrderByDescending(n => n).ToList();
            Assert.That(names, Is.EqualTo(sortedNames), $"Items should be sorted by name {sortOption}.");
        }
        else if (sortOption == "lohi" || sortOption == "hilo")
        {
            var prices = await _inventoryPage.GetAllItemPricesAsync();
            var sortedPrices = sortOption == "lohi" ? prices.OrderBy(p => p).ToList() : prices.OrderByDescending(p => p).ToList();
            Assert.That(prices, Is.EqualTo(sortedPrices), $"Items should be sorted by price {sortOption}.");
        }
    }
}
