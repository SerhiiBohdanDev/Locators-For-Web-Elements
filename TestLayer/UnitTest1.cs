using LocatorsForWebElements.BusinessLayer.Models;
using LocatorsForWebElements.BusinessLayer.Pages;
using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium.Chrome;

namespace LocatorsForWebElements.TestLayer
{
    internal class Tests
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

        [TestCaseSource(nameof(JobsSearchData))]
        public void SearchJobsTest(JobSearchModel model)
        {
            new MainPage(this.driver)
                .Open()
                .ClickJoinUs();

            var searchPage = new SearchJobsPage(this.driver)
                .EnterLanguage(model.Language)
                .EnterLocation(model.Location)
                .ClickRemoteCheckbox()
                .ClickSearch();

            var result = searchPage.ContainsLanguageInLastSearchResult(model.Language);
            Assert.That(result, Is.True);
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

        private static IEnumerable<JobSearchModel> JobsSearchData()
        {
            yield return new JobSearchModel() { Language = new string[] { "JavaScript", "JS", "Javascript" }, Location = "Georgia" };
            yield return new JobSearchModel() { Language = new string[] { "C#", "c#" }, Location = "Georgia" };
            yield return new JobSearchModel() { Language = new string[] { "Python", "python" }, Location = "Georgia" };
            yield return new JobSearchModel() { Language = new string[] { "JavaScript", "JS", "Javascript" }, Location = "Belgium" };
            yield return new JobSearchModel() { Language = new string[] { "C#", "c#" }, Location = "Belgium" };
            yield return new JobSearchModel() { Language = new string[] { "Python", "python" }, Location = "Belgium" };
        }
    }
}