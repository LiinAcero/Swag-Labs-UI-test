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

            // Verify visibility
            if (!await _itemDetailsPage.IsNameVisibleAsync()) issues.Add($"{itemName}: Name not visible");
            if (!await _itemDetailsPage.IsDescriptionVisibleAsync()) issues.Add($"{itemName}: Description not visible");
            if (!await _itemDetailsPage.IsPriceVisibleAsync()) issues.Add($"{itemName}: Price not visible");
            if (!await _itemDetailsPage.IsImageVisibleAsync()) issues.Add($"{itemName}: Image not visible");

            // Verify audio descriptor (alt text)
            string? altText = await _itemDetailsPage.GetImageAltAsync();
            if (string.IsNullOrWhiteSpace(altText))
            {
                issues.Add($"{itemName}: Image missing audio descriptor (alt text)");
            }
            else
            {
                TestContext.WriteLine($"{itemName} alt text: {altText}");
            }

            // Verify font
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
