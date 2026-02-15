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

    // only using this approach to satisfy task requirement,
    // the commented out _fundButton should be used
    private readonly By _form = By.TagName("form");
    private readonly By _findButton = By.XPath("//button[contains(@class, 'large-gradient-button') and contains(@class, 'custom-search-button')]");

    //private readonly By _findButton = By.XPath("//form//child::button");
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
        var topRow = _driver.FindElement(_topNavRow);
        var careers = _driver.FindClickableElement(_careersText, topRow);
        _driver.Hover(careers);

        _driver
            .FindClickableElement(_joinUs, topRow)
            .Click();
        return this;
    }

    public MainPage ClickMagnifyingGlass()
    {
        _driver
            .FindClickableElement(_magnifyingGlass)
            .Click();
        return this;
    }

    public MainPage EnterSearchTerm(string text)
    {
        _driver
            .FindClickableElement(_searchField)
            .SendKeys(text);
        return this;
    }

    public MainPage ClickFind()
    {
        // this approach is only here to satisfy requirements to include all By methods
        // remove after
        var form = _driver.FindElement(_form);
        _driver
            .FindClickableElement(_findButton, form)
            .Click();

        /*_driver
            .FindClickableElement(_findButton)
            .Click();
        */
        return this;
    }

    public List<string> GetSearchResultTitles()
    {
        var results = new List<string>();
        var elements = _driver.FindClickableElements(_searchResult);
        foreach (var element in elements)
        {
            results.Add(element.Text);
        }

        return results;
    }
}
