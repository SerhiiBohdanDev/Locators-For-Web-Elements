using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace LocatorsForWebElements.CoreLayer;
internal static class IWebElementExtensions
{
    public static ReadOnlyCollection<IWebElement> GetAllDirectChildren(this IWebElement element)
    {
        return element.FindElements(By.XPath("*"));
    }

    // allows getting text on elements that are invisible
    public static string? GetText(this IWebElement element)
    {
        return element.GetAttribute("textContent");
    }
}
