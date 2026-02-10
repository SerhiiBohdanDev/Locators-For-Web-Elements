using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace LocatorsForWebElements.CoreLayer;
internal class DriverWrapper
{
    private const int MaxRetries = 3;
    private readonly IWebDriver driver;
    private readonly TimeSpan timeout;

    public DriverWrapper(IWebDriver driver, TimeSpan timeout)
    {
        this.driver = driver;
        this.timeout = timeout;
    }

    public void GoToUrl(string url) => driver.Navigate().GoToUrl(url);
    public void Close()
    {
        driver.Quit();
        driver.Dispose();
    }

    /// <summary>
    /// Allows clicking an element safely in cases where it can be interrupted by animation or popups
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="element"></param>
    public void SafeClick(IWebElement element)
    {
        new Actions(driver)
            .MoveToElement(element)
            .Click()
            .Build()
            .Perform();
    }

    public void JavascriptClick(IWebElement element)
    {
        driver.ExecuteJavaScript("arguments[0].click();", element);
    }

    public void Hover(IWebElement element)
    {
        new Actions(driver)
                .MoveToElement(element)
                .Perform();
    }

    public IWebElement WaitForElementToBePresent(By by, IWebElement? parent = default)
    {
        return WaitForElement(by, parent);
    }

    public IWebElement WaitForElementToBeVisible(By by, IWebElement? parent = default)
    {
        return WaitForElement(by, parent, GetVisibleElement);
    }

    public IWebElement WaitForElementToBeClickable(By by, IWebElement? parent = default)
    {
        return WaitForElement(by, parent, GetClickableElement);
    }

    public ReadOnlyCollection<IWebElement> WaitForElementsCollectionToBeClickable(By by, IWebElement? parent = default)
    {
        return WaitForElements(by, parent, GetClickableElement);
    }

    /// <summary>
    /// Waits for element and returns it or null based on check action
    /// </summary>
    /// <param name="by"></param>
    /// <param name="parent"></param>
    /// <param name="checkAction">Action to perform on element to decide whether return element or null</param>
    /// <returns></returns>
    /// <exception cref="StaleElementReferenceException"></exception>
    private IWebElement WaitForElement(
        By by, 
        IWebElement? parent = default, 
        Func<IWebElement?, IWebElement?>? checkAction = null)
    {
        int retries = 0;
        while (retries < MaxRetries)
        {
            try
            {
                var wait = new WebDriverWait(driver, timeout);
                return wait.Until(driver =>
                {
                    try
                    {
                        return CheckElementValidity(by, parent, checkAction);
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                retries++;
            }
        }
        throw new StaleElementReferenceException($"Element located by {by} remained stale after {MaxRetries} attempts.");
    }

    private ReadOnlyCollection<IWebElement> WaitForElements(
        By by, 
        IWebElement? parent = default,
        Func<IWebElement?, IWebElement?>? checkAction = null)
    {
        int retries = 0;
        while (retries < MaxRetries)
        {
            try
            {
                var wait = new WebDriverWait(driver, timeout);
                return wait.Until(driver =>
                {
                    try
                    {
                        return CheckElementsCollectionValidity(by, parent, checkAction);
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                retries++;
            }
        }
        throw new StaleElementReferenceException($"Element located by {by} remained stale after {MaxRetries} attempts.");
    }

    private static IWebElement? GetVisibleElement(IWebElement? element)
    {
        if (element != null)
        {
            // forces check for stallness
            // without it last element in the list of found jobs would throw StaleElementReferenceException after all retries
            bool _ = element.Displayed;
        }
        
        return element;
    }

    private static IWebElement? GetClickableElement(IWebElement? element)
    {
        if (element != null && element.Displayed && element.Enabled)
        {
            return element;
        }

        return null;
    }

    private IWebElement? CheckElementValidity(
        By by,
        IWebElement? parent = default,
        Func<IWebElement?, IWebElement?>? checkAction = null)
    {
        IWebElement element = FindElement(by, parent);
        if (checkAction != null)
        {
            return checkAction.Invoke(element);
        }

        return element;
    }

    private ReadOnlyCollection<IWebElement>? CheckElementsCollectionValidity(
        By by,
        IWebElement? parent = default, 
        Func<IWebElement?, IWebElement?>? checkAction = null)
    {
        var elements = FindElements(by, parent);
        if (elements != null && elements.Count > 0)
        {
            if (checkAction != null)
            {
                foreach (var item in elements)
                {
                    // will throw StaleElementReferenceException if element doesn't fit condition, which will be caught later
                    checkAction.Invoke(item);
                }
            }
            return elements;
        }

        return null;
    }

    private IWebElement FindElement(By by, IWebElement? parent = default)
    {
        return parent == null ? driver.FindElement(by) : parent.FindElement(by);
    }

    private ReadOnlyCollection<IWebElement> FindElements(By by, IWebElement? parent = default)
    {
        return parent == null ? driver.FindElements(by) : parent.FindElements(by);
    }
}
