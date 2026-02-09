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

    public bool ContainsLanguageInLastSearchResult(string[] language)
    {
        var container = driver.WaitForElementToBeVisible(resultsContainer);
        var lastResult = driver.WaitForElementToBeVisible(lastElement, container);
        if (HasLanguageInTitle(lastResult, language) ||
            HasLanguageInShortDescription(lastResult, language) ||
            HasLanguageInFullDescription(lastResult, language))
        {
            return true;
        }
        return false;
    }

    private bool HasLanguageInTitle(IWebElement parent, string[] language)
    {
        var title = parent.FindElement(jobCardTitle);
        for (int i = 0; i < language.Length; i++)
        {
            if (ContainsText(title.Text, language[i]))
            {
                return true;
            }
        }
        return false;
    }

    private bool HasLanguageInShortDescription(IWebElement parent, string[] language)
    {
        var shortDescription = parent.FindElement(shortJobDescription);
        for (int i = 0; i < language.Length; i++)
        {
            if (ContainsText(shortDescription.Text, language[i]))
            {
                return true;
            }
        }
        return false;
    }

    private bool HasLanguageInFullDescription(IWebElement parent, string[] language)
    {
        var container = parent.FindElement(fullDescriptionContainer);
        var sentences = container.FindElements(descriptionSentences);

        foreach (var sentence in sentences)
        {
            // using this inseat of property because sentences are hidden
            var text = sentence.GetText();
            if (text == null)
            {
                continue;
            }

            for (int i = 0; i < language.Length; i++)
            {
                if (ContainsText(text, language[i]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void EnterText(By locator, string text)
    {
        driver.WaitForElementToBeClickable(locator)
            .SendKeys(text);
    }

    private static bool ContainsText(string text, string target) => text.Contains(target, StringComparison.InvariantCulture);
}
