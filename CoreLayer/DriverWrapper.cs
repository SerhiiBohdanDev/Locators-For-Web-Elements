using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

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

    public void MoveToElement(IWebElement element)
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

    private IWebElement WaitForElement(By by, IWebElement? parent = default, Func<IWebElement, IWebElement>? checkAction = null)
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
                        IWebElement element = FindElement(by, parent);
                        if (checkAction != null)
                        {
                            return checkAction.Invoke(element);
                        }

                        return element;
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

    private static IWebElement GetVisibleElement(IWebElement element)
    {
        // forces check for stallness
        // without it last element in the list of found jobs would throw StaleElementReferenceException after all retries
        bool _ = element.Displayed;
        return element;
    }

    private static IWebElement GetClickableElement(IWebElement element)
    {
        if (element != null && element.Displayed && element.Enabled)
        {
            return element;
        }

        return null;
    }

    private IWebElement FindElement(By by, IWebElement? parent = default)
    {
        return parent == null ? driver.FindElement(by) : parent.FindElement(by);
    }
}
