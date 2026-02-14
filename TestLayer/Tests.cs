using System.Text;
using LocatorsForWebElements.BusinessLayer.Models;
using LocatorsForWebElements.BusinessLayer.Pages;
using LocatorsForWebElements.CoreLayer;
using OpenQA.Selenium.Chrome;

namespace LocatorsForWebElements.TestLayer
{
    internal class Tests
    {
        private const string JobDescriptionMissingKeywordMessage = "Job description is missing the following keyword(s):";
        private const string TitleMissingSearchTermMessage = "Following titles are missing the following search term:";
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

            if (!isInformationContainsLanguage)
            {
                var builder = new StringBuilder();
                string keywords = string.Join(',', model.Language);

                builder.AppendLine($"{JobDescriptionMissingKeywordMessage} [{keywords}]");
                builder.AppendLine();
                for (int i = 0; i < jobInformation.Count; i++)
                {
                    builder.AppendLine(jobInformation[i]);
                }

                LogInformation(builder.ToString());
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

            List<string> titles = mainPage.GetSearchResultTitles();
            var allTitlesContainTerm = true;
            List<string> titlesThatDoNoContainTerm = [];
            foreach (var title in titles)
            {
                if (!title.Contains(term, StringComparison.InvariantCultureIgnoreCase))
                {
                    allTitlesContainTerm = false;
                    titlesThatDoNoContainTerm.Add(title);
                }
            }

            if (!allTitlesContainTerm)
            {
                var builder = new StringBuilder();
                builder.AppendLine($"{TitleMissingSearchTermMessage} [{term}]");
                builder.AppendLine();
                for (int i = 0; i < titlesThatDoNoContainTerm.Count; i++)
                {
                    builder.AppendLine(titlesThatDoNoContainTerm[i]);
                }

                LogInformation(builder.ToString());
            }

            Assert.That(allTitlesContainTerm, Is.True);
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
                new() { Language = ["JavaScript", "JS", "Javascript"], Location = "Georgia" },
                new() { Language = ["C#", "c#"], Location = "Georgia" },
                new() { Language = ["Python", "python"], Location = "Georgia" },
                new() { Language = ["JavaScript", "JS", "Javascript"], Location = "Belgium" },
                new() { Language = ["C#", "c#"], Location = "Belgium" },
                new() { Language = ["Python", "python"], Location = "Belgium" },
                new() { Language = ["JavaScript", "JS", "Javascript"], Location = "Armenia" },
                new() { Language = ["C#", "c#"], Location = "Armenia" },
                new() { Language = ["Python", "python"], Location = "Armenia" },
            };

            foreach (var model in cases)
            {
                yield return new TestCaseData(model)
                    .SetName($"SearchJobsTest(\"{model.Language[0]}\", \"{model.Location}\")");
            }
        }

        private static bool ContainsText(string text, string target) => text.Contains(target, StringComparison.InvariantCulture);

        private static void LogInformation(string text)
        {
            Console.WriteLine(text);
        }
    }
}
