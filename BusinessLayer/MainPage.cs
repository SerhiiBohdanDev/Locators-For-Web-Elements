using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium;

namespace LocatorsForWebElements.BusinessLayer;
internal class MainPage
{

    public const string Url = "https://www.epam.com/";
    private readonly DriverWrapper driver;

    public readonly By topNavRow = By.ClassName("top-navigation__row");
    public readonly By careersText = By.LinkText("Careers");
    public readonly By joinUs = By.PartialLinkText("Join our Team");

    public MainPage(DriverWrapper driver)
    {
        this.driver = driver;
    }

    public MainPage Open()
    {
        driver.GoToUrl(Url);
        return this;
    }

    public MainPage ClickJoinUs()
    {
        var topRow = driver.WaitForElementToBePresent(this.topNavRow);
        var careers = topRow.FindElement(this.careersText);
        driver.Hover(careers);

        var join = topRow.FindElement(this.joinUs);
        join.Click();
        return this;
    }

}
