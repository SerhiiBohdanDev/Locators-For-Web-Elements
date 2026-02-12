using LocatorsForWebElements.BusinessLayer.Models;
using LocatorsForWebElements.BusinessLayer.Pages;
using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium.Chrome;

namespace LocatorsForWebElements.TestLayer
{
    internal class Tests
    {
        private DriverWrapper _driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("start-maximized");
            _driver = new DriverWrapper(new ChromeDriver(options), TimeSpan.FromSeconds(3));
        }

        [TestCaseSource(nameof(JobsSearchData))]
        public void SearchJobsTest(JobSearchModel model)
        {
            new MainPage(_driver)
                .Open()
                .ClickJoinUs();

            var searchPage = new SearchJobsPage(_driver)
                .EnterLanguage(model.Language)
                .EnterLocation(model.Location)
                .ClickRemoteCheckbox()
                .ClickSearch();

            var jobInformation = searchPage.GetJobInformation();
            var isInformationContainsLanguage = false;
            for (int i = 0; i < jobInformation.Count; i++)
            {
                for (int k = 0; k < model.Language.Length; k++)
                {
                    if (ContainsText(jobInformation[i], model.Language[k]))
                    {
                        isInformationContainsLanguage = true;
                        break;
                    }
                }

                if (isInformationContainsLanguage)
                {
                    break;
                }
            }

            Assert.That(isInformationContainsLanguage, Is.True);
        }

        [TestCase("BLOCKCHAIN")]
        [TestCase("Cloud")]
        [TestCase("Automation")]
        public void GeneralSearchTest(string term)
        {
            var mainPage = new MainPage(_driver)
                .Open()
                .ClickMagnifyingGlass()
                .EnterSearchTerm(term)
                .ClickFind();

            var result = mainPage.GetSearchResultTitles();
            foreach (var item in result.Where(item => !item.Contains(term, StringComparison.InvariantCultureIgnoreCase)))
            {
                Assert.Fail($"'{item}' does NOT contain '{term}'");
            }

            Assert.Pass($"All link titles contain term '{term}'");
        }

        [TearDown]
        public void Teardown()
        {
            _driver.Close();
        }

        private static IEnumerable<TestCaseData> JobsSearchData()
        {
            var cases = new JobSearchModel[]
            {
                new () { Language = new string[] { "JavaScript", "JS", "Javascript" }, Location = "Georgia" },
                new () { Language = new string[] { "C#", "c#" }, Location = "Georgia" },
                new () { Language = new string[] { "Python", "python" }, Location = "Georgia" },
                new () { Language = new string[] { "JavaScript", "JS", "Javascript" }, Location = "Belgium" },
                new () { Language = new string[] { "C#", "c#" }, Location = "Belgium" },
                new () { Language = new string[] { "Python", "python" }, Location = "Belgium" },
                new () { Language = new string[] { "JavaScript", "JS", "Javascript" }, Location = "Armenia" },
                new () { Language = new string[] { "C#", "c#" }, Location = "Armenia" },
                new () { Language = new string[] { "Python", "python" }, Location = "Armenia" },
            };

            foreach (var model in cases)
            {
                yield return new TestCaseData(model)
                    .SetName($"SearchJobsTest(\"{model.Language[0]}\", \"{model.Location}\")");
            }
        }

        private static bool ContainsText(string text, string target) => text.Contains(target, StringComparison.InvariantCulture);
    }
}
