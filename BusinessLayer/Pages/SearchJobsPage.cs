using System.Collections.ObjectModel;
using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium;

namespace LocatorsForWebElements.BusinessLayer.Pages;

internal class SearchJobsPage
{
    private readonly DriverWrapper _driver;

    private readonly By _keywordSearchField = By.XPath("//*[@id='anchor-list']//form//input[@data-testid='search-input']");
    private readonly By _locationDropdown = By.XPath("//*[@id='anchor-list']//form//input[contains(@class, 'dropdown__input')]");
    private readonly By _remoteCheckbox = By.Id("checkbox-vacancy_type-Remote-«r0»");
    private readonly By _searchButton = By.XPath("//*[@id='anchor-list']//child::button[@type='submit']");
    private readonly By _resultsContainer = By.ClassName("List_list___59gh");
    private readonly By _jobCardTitle = By.CssSelector("span[data-testid='job-card-title']");
    private readonly By _shortJobDescription = By.CssSelector("div[data-testid='job-card-description']");
    private readonly By _descriptionSentences = By.CssSelector("div[data-testid='rich-text']");
    private readonly By _lastElement = By.XPath("./*[last()]");

    public SearchJobsPage(DriverWrapper driver)
    {
        _driver = driver;
    }

    public SearchJobsPage EnterLanguage(string[] language)
    {
        var element = _driver.FindClickableElement(_keywordSearchField);
        EnterText(element, language[0]);
        return this;
    }

    public SearchJobsPage EnterLocation(string location)
    {
        var element = _driver.FindClickableElement(_locationDropdown);
        EnterText(element, location, true);
        return this;
    }

    public SearchJobsPage ClickRemoteCheckbox()
    {
        var checkbox = _driver.FindElement(_remoteCheckbox);

        // the checkbox has opacity at 0 which makes it Displayed property false, and so clicking is not allowed
        // so we use js to click
        _driver.JavascriptClick(checkbox);
        return this;
    }

    public SearchJobsPage ClickSearch()
    {
        var search = _driver.FindClickableElement(_searchButton);
        _driver.SafeClick(search);
        return this;
    }

    public List<string> GetJobInformation()
    {
        var results = new List<string>();
        var container = _driver.FindDisplayedElement(_resultsContainer);
        var lastResult = _driver.FindDisplayedElement(_lastElement, container);
        var title = _driver.FindElement(_jobCardTitle, lastResult);
        results.Add(title.Text);

        var shortDescription = _driver.FindElement(_shortJobDescription, lastResult);
        results.Add(shortDescription.Text);

        ReadOnlyCollection<IWebElement> sentences = _driver.FindElements(_descriptionSentences, lastResult);
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
