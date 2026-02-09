using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace LocatorsForWebElements;
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
        this.driver.Quit();
        this.driver.Dispose();
    }

    /// <summary>
    /// Allows clicking an element safely in cases where it can be interrupted by animation or popups
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="element"></param>
    public void SafeClick(IWebElement element)
    {
        new Actions(this.driver)
            .MoveToElement(element)
            .Click()
            .Build()
            .Perform();
    }

    public void JavascriptClick(IWebElement element)
    {
        this.driver.ExecuteJavaScript("arguments[0].click();", element);
    }

    public void Hover(IWebElement element)
    {
        new Actions(this.driver)
                .MoveToElement(element)
                .Perform();
    }

    public void MoveToElement(IWebElement element)
    {
        new Actions(this.driver)
                .MoveToElement(element)
                .Perform();
    }

    public IWebElement WaitForElementToBePresent(By by)
    {
        var wait = new WebDriverWait(this.driver, this.timeout);
        return wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(by);
                if (element != null)
                    return element;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"'NoSuchElementException' is found. By = {by}");
            }

            return null;
        });
    }

    public IWebElement WaitForElementToBeVisible(By by)
    {
        int reties = 0;
        while (reties < MaxRetries)
        {
            try
            {
                var wait = new WebDriverWait(this.driver, this.timeout);
                return wait.Until(driver =>
                {
                    try
                    {
                        var element = driver.FindElement(by);
                        // forces check for stallness?
                        bool _ = element.Displayed;
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
                reties++;
            }
        }
        throw new StaleElementReferenceException($"Element located by {by} remained stale after {MaxRetries} attempts.");
    }

    public IWebElement WaitForElementToBeVisible(By by, IWebElement parent)
    {
        int reties = 0;
        while (reties < MaxRetries)
        {
            try
            {
                var wait = new WebDriverWait(this.driver, this.timeout);
                return wait.Until(driver =>
                {
                    try
                    {
                        var element = parent.FindElement(by);
                        // forces check for stallness?
                        bool _ = element.Displayed;
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
                reties++;
            }
        }
        throw new StaleElementReferenceException($"Element located by {by} remained stale after {MaxRetries} attempts.");
    }

    public IWebElement WaitForElementToBeClickable(By by)
    {
        var wait = new WebDriverWait(this.driver, this.timeout);
        return wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(by);
                if (element != null && element.Enabled)
                    return element;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"'NoSuchElementException' is found. By = {by}");
            }

            return null;
        });
    }
}
