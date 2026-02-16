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
}
