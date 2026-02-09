using LocatorsForWebElements.BusinessLayer;
using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium.Chrome;

namespace LocatorsForWebElements.TestLayer
{
    public class Tests
    {
        private DriverWrapper driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("start-maximized");
            //options.AddArgument("--auto-open-devtools-for-tabs");
            driver = new DriverWrapper(new ChromeDriver(options), TimeSpan.FromSeconds(10));
        }

        [TestCase("javascript", "Georgia")]
        public void SearchJobsTest(string language, string location)
        {
            new MainPage(this.driver)
                .Open()
                .ClickJoinUs();

            var searchPage = new SearchJobsPage(this.driver)
                .EnterLanguage(language)
                .EnterLocation(location)
                .ClickRemoteCheckbox()
                .ClickSearch();

            var result = searchPage.ContainsLanguageInLastSearchResult(language);
            Assert.That(result, Is.True);
            //Assert.Pass();
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