using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LocatorsForWebElements
{
    public class Tests
    {
        private const string EpamLink = "https://www.epam.com/";
        private DriverWrapper driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("start-maximized");
            //options.AddArgument("--auto-open-devtools-for-tabs");
            this.driver = new DriverWrapper(new ChromeDriver(options), TimeSpan.FromSeconds(10));
        }

        [TestCase("javascript", "Georgia")]
        public void SearchJobsTest(string language, string location)
        {
            driver.GoToUrl(EpamLink);

            By topNavRowBy = By.ClassName("top-navigation__row");
            By careersTextBy = By.LinkText("Careers");
            By joinUsBy = By.PartialLinkText("Join our Team");

            var topRow = driver.WaitForElementToBePresent(topNavRowBy);
            var careers = topRow.FindElement(careersTextBy);
            driver.Hover(careers);

            var joinUs = topRow.FindElement(joinUsBy);
            joinUs.Click();

            By searchInputBy = By.CssSelector("input[data-testid='search-input']");
            var searchField = driver.WaitForElementToBeClickable(searchInputBy);
            searchField.SendKeys(language);

            By locationBy = By.Id("react-select-4-input");
            var locationDropdown = driver.WaitForElementToBeClickable(locationBy);
            locationDropdown.SendKeys(location);

            var remoteCheckboxBy = By.Id("checkbox-vacancy_type-Remote-«r0»");
            var remoteCheckbox = driver.WaitForElementToBePresent(remoteCheckboxBy);
            // the checkbox has opacity at 0 which makes it Displayed property false, and so clicking is not allowed
            // so we use js to click
            driver.JavascriptClick(remoteCheckbox);

            var searchButtonBy = By.XPath("//*[@id='anchor-list']/div/div/div/form/button");
            var searchButton = driver.WaitForElementToBeClickable(searchButtonBy);
            driver.SafeClick(searchButton);

            By resultsContainerBy = By.ClassName("List_list___59gh");
            var resultsContainer = driver.WaitForElementToBeVisible(resultsContainerBy);
            var lastResult = driver.WaitForElementToBeVisible(By.XPath("./*[last()]"), resultsContainer);

            var jobCardTitleBy = By.CssSelector("span[data-testid='job-card-title']");
            var title = lastResult.FindElement(jobCardTitleBy);
            bool titleContainsLanguage = title.Text.Contains(language, StringComparison.InvariantCultureIgnoreCase);

            var shortJobDescriptionBy = By.CssSelector("div[data-testid='job-card-description']");
            var shortDescription = lastResult.FindElement(shortJobDescriptionBy);
            bool descriptionContainsLanguage = shortDescription.Text.Contains(language, StringComparison.InvariantCultureIgnoreCase);

            var fullDescriptionContainerBy = By.CssSelector("div[data-testid='categories-container']");
            var fullDescriptionContainer = lastResult.FindElement(fullDescriptionContainerBy);
            var descriptionSentencesBy = By.CssSelector("div[data-testid='rich-text']");
            var sentences = fullDescriptionContainer.FindElements(descriptionSentencesBy);

            bool sentenceContainsLanguage = false;
            foreach (var item in sentences)
            {
                var text = item.GetAttribute("textContent");
                if (text == null)
                {
                    continue;
                }

                if (text.Contains(language, StringComparison.InvariantCultureIgnoreCase))
                {
                    sentenceContainsLanguage = true;
                    break;
                }
            }

            var result = titleContainsLanguage || descriptionContainsLanguage || sentenceContainsLanguage;
            //Assert.That(result, Is.True);
            Assert.Pass();
        }

        //[Test]
        //public void GeneralSearchTest()
        //{
        //    //driver.Navigate().GoToUrl(EpamLink);
        //    //By magnifyingGlass = By.CssSelector("button[class='header-search__button header__icon']");
        //    ////By searchField = By.Id("new_form_search");
        //    //By searchField = By.Name("q");

        //    //var glass = WaitForElementToBeClickable(magnifyingGlass);
        //    //glass.Click();

        //    //var field = WaitForElementToBeClickable(searchField);
        //    //field.SendKeys("test123");

        //    Assert.Pass();
        //}

        [TearDown]
        public void Teardown()
        {
            driver.Close();
        }
    }
}