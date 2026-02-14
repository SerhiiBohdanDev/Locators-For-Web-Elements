using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium;

namespace LocatorsForWebElements.BusinessLayer.Pages;

internal class MainPage
{
    private const string PageUrl = "https://www.epam.com/";
    private readonly DriverWrapper _driver;

    private readonly By _topNavRow = By.ClassName("top-navigation__row");
    private readonly By _careersText = By.LinkText("Careers");
    private readonly By _joinUs = By.PartialLinkText("Join our Team");
    private readonly By _magnifyingGlass = By.CssSelector("button[class='header-search__button header__icon']");
    private readonly By _searchField = By.Name("q");
    private readonly By _findButton = By.XPath("//form//child::button");
    private readonly By _searchResult = By.ClassName("search-results__title-link");

    public MainPage(DriverWrapper driver)
    {
        _driver = driver;
    }

    public MainPage Open()
    {
        _driver.GoToUrl(PageUrl);
        return this;
    }

    public MainPage ClickJoinUs()
    {
        var topRow = _driver.WaitForElementToBePresent(_topNavRow);
        var careers = _driver.WaitForElementToBeClickable(_careersText, topRow);
        _driver.Hover(careers);

        _driver
            .WaitForElementToBeClickable(_joinUs, topRow)
            .Click();
        return this;
    }

    public MainPage ClickMagnifyingGlass()
    {
        _driver
            .WaitForElementToBeClickable(_magnifyingGlass)
            .Click();
        return this;
    }

    public MainPage EnterSearchTerm(string text)
    {
        _driver
            .WaitForElementToBeClickable(_searchField)
            .SendKeys(text);
        return this;
    }

    public MainPage ClickFind()
    {
        _driver
            .WaitForElementToBeClickable(_findButton)
            .Click();
        return this;
    }

    public List<string> GetSearchResultTitles()
    {
        var results = new List<string>();
        var elements = _driver.WaitForElementsCollectionToBeClickable(_searchResult);
        foreach (var element in elements)
        {
            results.Add(element.Text);
        }

        return results;
    }
}
