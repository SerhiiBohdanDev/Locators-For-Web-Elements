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
        var careers = topRow.FindElement(careersText);
        driver.Hover(careers);

        var join = topRow.FindElement(joinUs);
        join.Click();
        return this;
    }

}
