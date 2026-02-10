using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium;

namespace LocatorsForWebElements.BusinessLayer.Pages;
internal class SearchJobsPage
{
    private readonly DriverWrapper driver;

    public readonly By searchInput = By.CssSelector("input[data-testid='search-input']");
    public readonly By locationDropdown = By.Id("react-select-4-input");
    public readonly By remoteCheckbox = By.Id("checkbox-vacancy_type-Remote-«r0»");
    public readonly By searchButton = By.XPath("//*[@id='anchor-list']/div/div/div/form/button");
    public readonly By resultsContainer = By.ClassName("List_list___59gh");
    public readonly By jobCardTitle = By.CssSelector("span[data-testid='job-card-title']");
    public readonly By shortJobDescription = By.CssSelector("div[data-testid='job-card-description']");
    public readonly By fullDescriptionContainer = By.CssSelector("div[data-testid='categories-container']");
    public readonly By descriptionSentences = By.CssSelector("div[data-testid='rich-text']");
    public readonly By lastElement = By.XPath("./*[last()]");

    public SearchJobsPage(DriverWrapper driver)
    {
        this.driver = driver;
    }

    public SearchJobsPage EnterLanguage(string[] language)
    {
        EnterText(searchInput, language[0]);
        return this;
    }

    public SearchJobsPage EnterLocation(string location)
    {
        EnterText(locationDropdown, location);
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

    private void EnterText(By locator, string text)
    {
        driver.WaitForElementToBeClickable(locator)
            .SendKeys(text);
    }
}
