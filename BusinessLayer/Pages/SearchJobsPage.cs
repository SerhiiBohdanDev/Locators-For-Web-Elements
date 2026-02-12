using System.Collections.ObjectModel;
using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium;

namespace LocatorsForWebElements.BusinessLayer.Pages;
internal class SearchJobsPage
{
    private readonly DriverWrapper _driver;

    private readonly By _form = By.XPath("//*[@id=\"anchor-list\"]//child::form");
    private readonly By _keywordSearchField = By.CssSelector("input[data-testid='search-input']");
    private readonly By _locationDropdown = By.XPath("//input[contains(@class, 'dropdown__input')]");
    private readonly By _remoteCheckbox = By.Id("checkbox-vacancy_type-Remote-«r0»");
    private readonly By _searchButton = By.XPath("//*[@id='anchor-list']//child::button[@type='submit']");
    private readonly By _resultsContainer = By.ClassName("List_list___59gh");
    private readonly By _jobCardTitle = By.CssSelector("span[data-testid='job-card-title']");
    private readonly By _shortJobDescription = By.CssSelector("div[data-testid='job-card-description']");
    private readonly By _fullDescriptionContainer = By.CssSelector("div[data-testid='categories-container']");
    private readonly By _descriptionSentences = By.CssSelector("div[data-testid='rich-text']");
    private readonly By _lastElement = By.XPath("./*[last()]");

    public SearchJobsPage(DriverWrapper driver)
    {
        _driver = driver;
    }

    public SearchJobsPage EnterLanguage(string[] language)
    {
        var formContainer = _driver.WaitForElementToBePresent(_form);
        var element = _driver.WaitForElementToBeClickable(_keywordSearchField, formContainer);
        EnterText(element, language[0]);
        return this;
    }

    public SearchJobsPage EnterLocation(string location)
    {
        var formContainer = _driver.WaitForElementToBePresent(_form);
        var element = _driver.WaitForElementToBeClickable(_locationDropdown, formContainer);
        EnterText(element, location, true);
        return this;
    }

    public SearchJobsPage ClickRemoteCheckbox()
    {
        var checkbox = _driver.WaitForElementToBePresent(_remoteCheckbox);

        // the checkbox has opacity at 0 which makes it Displayed property false, and so clicking is not allowed
        // so we use js to click
        _driver.JavascriptClick(checkbox);
        return this;
    }

    public SearchJobsPage ClickSearch()
    {
        var search = _driver.WaitForElementToBeClickable(_searchButton);
        _driver.SafeClick(search);
        return this;
    }

    public List<string> GetJobInformation()
    {
        var results = new List<string>();
        var container = _driver.WaitForElementToBeVisible(_resultsContainer);
        var lastResult = _driver.WaitForElementToBeVisible(_lastElement, container);
        var title = lastResult.FindElement(_jobCardTitle);
        results.Add(title.Text);

        var shortDescription = lastResult.FindElement(_shortJobDescription);
        results.Add(shortDescription.Text);

        IWebElement fullDescription = lastResult.FindElement(_fullDescriptionContainer);
        ReadOnlyCollection<IWebElement> sentences = fullDescription.FindElements(_descriptionSentences);
        for (int i = 0; i < sentences.Count; i++)
        {
            string? text = sentences[i].GetText();
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
