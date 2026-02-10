using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium;

namespace LocatorsForWebElements.BusinessLayer.Pages;
internal class MainPage
{

    private const string PageUrl = "https://www.epam.com/";
    private readonly DriverWrapper driver;

    private readonly By topNavRow = By.ClassName("top-navigation__row");
    private readonly By careersText = By.LinkText("Careers");
    private readonly By joinUs = By.PartialLinkText("Join our Team");
    private readonly By magnifyingGlass = By.CssSelector("button[class='header-search__button header__icon']");
    private readonly By searchField = By.Name("q");
    private readonly By findButton = By.XPath("//form//child::button");
    private readonly By searchResult = By.ClassName("search-results__title-link");

    public MainPage(DriverWrapper driver)
    {
        this.driver = driver;
    }

    public MainPage Open()
    {
        driver.GoToUrl(PageUrl);
        return this;
    }

    public MainPage ClickJoinUs()
    {
        var topRow = driver.WaitForElementToBePresent(topNavRow);
        var careers = driver.WaitForElementToBeClickable(careersText, topRow);
        driver.Hover(careers);

        driver
            .WaitForElementToBeClickable(joinUs, topRow)
            .Click();
        return this;
    }

    public MainPage ClickMagnifyingGlass()
    {
        driver
            .WaitForElementToBeClickable(magnifyingGlass)
            .Click();
        return this;
    }

    public MainPage EnterSearchTerm(string text)
    {
        driver
            .WaitForElementToBeClickable(searchField)
            .SendKeys(text);
        return this;
    }

    public MainPage ClickFind()
    {
        driver
            .WaitForElementToBeClickable(findButton)
            .Click();
        return this;
    }

    public List<string> GetSearchResultTitles()
    {
        var results = new List<string>();
        var elements = driver.WaitForElementsCollectionToBeClickable(searchResult);
        foreach (var element in elements)
        {
            results.Add(element.Text);
        }

        return results;
    }
}
