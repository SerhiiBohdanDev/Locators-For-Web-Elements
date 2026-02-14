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
        return WaitForElement<IWebElement>(by, () =>
        {
            return CheckElementValidity(FindElement(by, parent));
        });
    }

    public ReadOnlyCollection<IWebElement> WaitForElementsCollectionToBePresent(By by, IWebElement? parent = default)
    {
        return WaitForElement<ReadOnlyCollection<IWebElement>>(by, () =>
        {
            // using ! because it will either return collection of elements or throw exception
            return CheckElementsCollectionValidity(FindElements(by, parent))!;
        });
    }

    public IWebElement WaitForElementToBeVisible(By by, IWebElement? parent = default)
    {
        return WaitForElement<IWebElement>(by, () =>
        {
            return CheckElementValidity(FindElement(by, parent), GetVisibleElement);
        });
    }

    public IWebElement WaitForElementToBeClickable(By by, IWebElement? parent = default)
    {
        return WaitForElement<IWebElement>(by, () =>
        {
            return CheckElementValidity(FindElement(by, parent), GetClickableElement);
        });
    }

    public ReadOnlyCollection<IWebElement> WaitForElementsCollectionToBeClickable(By by, IWebElement? parent = default)
    {
        return WaitForElement<ReadOnlyCollection<IWebElement>>(by, () =>
        {
            // using ! because it will either return collection of elements or throw exception
            return CheckElementsCollectionValidity(FindElements(by, parent), GetClickableElement)!;
        });
    }

    private static IWebElement GetVisibleElement(IWebElement element)
    {
        // accessing element's property forces check for stallness
        // is not null because when finding it NoSuchElementException would be thrown by driver
        // without it last element in the list of found jobs would throw StaleElementReferenceException after all retries
        bool _ = element!.Displayed;
        return element;
    }

    private static IWebElement GetClickableElement(IWebElement element)
    {
        // accessing element's property forces check for stallness
        // is not null because when finding it NoSuchElementException would be thrown by driver
        bool _ = element.Displayed && element.Enabled;
        return element;
    }

    private static IWebElement CheckElementValidity(
        IWebElement element,
        Func<IWebElement, IWebElement>? checkAction = null)
    {
        if (checkAction != null)
        {
            return checkAction.Invoke(element);
        }

        return element;
    }

    private static ReadOnlyCollection<IWebElement>? CheckElementsCollectionValidity(
        ReadOnlyCollection<IWebElement> elements,
        Func<IWebElement, IWebElement>? checkAction = null)
    {
        if (elements.Count == 0)
        {
            return null;
        }

        if (checkAction != null)
        {
            foreach (var item in elements)
            {
                checkAction.Invoke(item);
            }
        }

        return elements;
    }

    /// <summary>
    /// A wrapper method to allow finding one IWebElement or ReadOnlyCollection of type IWebElement.
    /// </summary>
    /// <typeparam name="T">IWebElement or ReadOnlyCollection of type IWebElement.</typeparam>
    /// <param name="by">Locator to reference in exception.</param>
    /// <param name="validityCheckAction">Action to perform on element/elements to check if they fit the condition.</param>
    /// <returns>One IWebElement or ReadOnlyCollection of type IWebElement.</returns>
    /// <exception cref="ArgumentNullException">Thrown if locator is null.</exception>
    /// <exception cref="NoSuchElementException">Thrown if element was not found when looking for a single element.</exception>
    /// <exception cref="StaleElementReferenceException">Thrown if an element or collection were stale.</exception>
    private T WaitForElement<T>(By by, Func<T> validityCheckAction)
    {
        ArgumentNullException.ThrowIfNull(by);

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
                        return validityCheckAction.Invoke();
                    }
                    catch (StaleElementReferenceException)
                    {
                        exceptionCaught = typeof(StaleElementReferenceException);
                        return default;
                    }
                    catch (NoSuchElementException)
                    {
                        exceptionCaught = typeof(NoSuchElementException);
                        return default;
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

    private IWebElement FindElement(By by, IWebElement? parent = default)
    {
        return parent == null ? _driver.FindElement(by) : parent.FindElement(by);
    }

    private ReadOnlyCollection<IWebElement> FindElements(By by, IWebElement? parent = default)
    {
        return parent == null ? _driver.FindElements(by) : parent.FindElements(by);
    }
}
