using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium;

namespace LocatorsForWebElements.BusinessLayer.Pages;
internal class SearchJobsPage
{
    private readonly DriverWrapper driver;

    private readonly By form = By.XPath("//*[@id=\"anchor-list\"]//child::form");
    private readonly By keywordSearchField = By.CssSelector("input[data-testid='search-input']");
    private readonly By locationDropdown = By.XPath("//input[contains(@class, 'Dropdown_defaultOption__pvL_3') and contains(@class, 'ym-disable-keys') and contains(@class, 'dropdown__input')]");
    private readonly By remoteCheckbox = By.Id("checkbox-vacancy_type-Remote-«r0»");
    private readonly By searchButton = By.XPath("//*[@id='anchor-list']//child::button[@type='submit']");
    private readonly By resultsContainer = By.ClassName("List_list___59gh");
    private readonly By jobCardTitle = By.CssSelector("span[data-testid='job-card-title']");
    private readonly By shortJobDescription = By.CssSelector("div[data-testid='job-card-description']");
    private readonly By fullDescriptionContainer = By.CssSelector("div[data-testid='categories-container']");
    private readonly By descriptionSentences = By.CssSelector("div[data-testid='rich-text']");
    private readonly By lastElement = By.XPath("./*[last()]");

    public SearchJobsPage(DriverWrapper driver)
    {
        this.driver = driver;
    }

    public SearchJobsPage EnterLanguage(string[] language)
    {
        var formContainer = driver.WaitForElementToBePresent(form);
        var element = driver.WaitForElementToBeClickable(keywordSearchField, formContainer);
        EnterText(element, language[0]);
        return this;
    }

    public SearchJobsPage EnterLocation(string location)
    {
        var formContainer = driver.WaitForElementToBePresent(form);
        var element = driver.WaitForElementToBeClickable(locationDropdown, formContainer);
        EnterText(element, location, true);
        return this;
    }

    public SearchJobsPage ClickRemoteCheckbox()
    {
        var checkbox = driver.WaitForElementToBePresent(remoteCheckbox);
        // the checkbox has opacity at 0 which makes it Displayed property false, and so clicking is not allowed
        // so we use js to click
        driver.JavascriptClick(checkbox);
        return this;
    }

    public SearchJobsPage ClickSearch()
    {
        var search = driver.WaitForElementToBeClickable(searchButton);
        driver.SafeClick(search);
        return this;
    }

    public List<string> GetJobInformation()
    {
        var results = new List<string>();
        var container = driver.WaitForElementToBeVisible(resultsContainer);
        var lastResult = driver.WaitForElementToBeVisible(lastElement, container);
        var title = lastResult.FindElement(jobCardTitle);
        results.Add(title.Text);

        var shortDescription = lastResult.FindElement(shortJobDescription);
        results.Add(shortDescription.Text);

        var fullDescription = lastResult.FindElement(fullDescriptionContainer);
        var sentences = fullDescription.FindElements(descriptionSentences);
        for (int i = 0; i < sentences.Count; i++)
        {
            var text = sentences[i].GetText();
            if (text != null)
            {
                results.Add(text);
            }
        }

        return results;
    }

    private static void EnterText(IWebElement element, string text, bool pressEnter = false)
    {
        element.SendKeys(text);
        // in order to correctly select location have to press enter
        if (pressEnter)
        {
            element.SendKeys(Keys.Enter);
        }
    }
}
