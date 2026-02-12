using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace LocatorsForWebElements.CoreLayer;
internal class DriverWrapper
{
    private const int MaxRetries = 3;
    private readonly IWebDriver _driver;
    private readonly TimeSpan _timeout;

    public DriverWrapper(IWebDriver driver, TimeSpan timeout)
    {
        _driver = driver;
        _timeout = timeout;
    }

    public void GoToUrl(string url) => _driver.Navigate().GoToUrl(url);

    public void Close()
    {
        _driver.Quit();
        _driver.Dispose();
    }

    /// <summary>
    /// Allows clicking an element safely in cases where it can be interrupted by animation or popups.
    /// </summary>
    /// <param name="element">Element we're trying to click.</param>
    public void SafeClick(IWebElement element)
    {
        new Actions(_driver)
            .MoveToElement(element)
            .Click()
            .Build()
            .Perform();
    }

    public void JavascriptClick(IWebElement element)
    {
        _driver.ExecuteJavaScript("arguments[0].click();", element);
    }

    public void Hover(IWebElement element)
    {
        new Actions(_driver)
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

    /// <summary>
    /// Waits for element and returns it based on check action.
    /// </summary>
    /// <param name="by">Locator to find element by.</param>
    /// <param name="parent">Element's parent.</param>
    /// <param name="checkAction">Action to perform on element to decide whether return element or null.</param>
    /// <exception cref="StaleElementReferenceException">Throws if element was stale.</exception>
    /// <exception cref="NoSuchElementException">Throws if element was not found.</exception>
    private IWebElement WaitForElement(
        By by,
        IWebElement? parent = default,
        Func<IWebElement?, IWebElement?>? checkAction = null)
    {
        int retries = 0;
        Type exceptionCaught = typeof(NoSuchElementException);
        while (retries < MaxRetries)
        {
            try
            {
                var wait = new WebDriverWait(_driver, _timeout);
                return wait.Until(driver =>
                {
                    try
                    {
                        return CheckElementValidity(by, parent, checkAction);
                    }
                    catch (StaleElementReferenceException)
                    {
                        exceptionCaught = typeof(StaleElementReferenceException);
                        return null;
                    }
                    catch (NoSuchElementException)
                    {
                        exceptionCaught = typeof(NoSuchElementException);
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                retries++;
            }
        }

        if (exceptionCaught == typeof(NoSuchElementException))
        {
            throw new NoSuchElementException($"Could not find element located by {by} after {MaxRetries} attempts.");
        }
        else
        {
            throw new StaleElementReferenceException($"Element located by {by} remained stale after {MaxRetries} attempts.");
        }
    }

    private ReadOnlyCollection<IWebElement> WaitForElements(
        By by,
        IWebElement? parent = default,
        Func<IWebElement?, IWebElement?>? checkAction = null)
    {
        int retries = 0;
        Type exceptionCaught = typeof(NoSuchElementException);
        while (retries < MaxRetries)
        {
            try
            {
                var wait = new WebDriverWait(_driver, _timeout);
                return wait.Until(driver =>
                {
                    try
                    {
                        return CheckElementsCollectionValidity(by, parent, checkAction);
                    }
                    catch (StaleElementReferenceException)
                    {
                        exceptionCaught = typeof(StaleElementReferenceException);
                        return null;
                    }
                    catch (NoSuchElementException)
                    {
                        exceptionCaught = typeof(NoSuchElementException);
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                retries++;
            }
        }

        if (exceptionCaught == typeof(NoSuchElementException))
        {
            throw new NoSuchElementException($"Could not find elements located by {by} after {MaxRetries} attempts.");
        }
        else
        {
            throw new StaleElementReferenceException($"Elements located by {by} remained stale after {MaxRetries} attempts.");
        }
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
        return parent == null ? _driver.FindElement(by) : parent.FindElement(by);
    }

    private ReadOnlyCollection<IWebElement> FindElements(By by, IWebElement? parent = default)
    {
        return parent == null ? _driver.FindElements(by) : parent.FindElements(by);
    }
}
