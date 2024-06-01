using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LottoAPI {
    public class Scraper : IDisposable {
        private IWebDriver driver;
        private string url;

        public Scraper(string url) {
            this.url = url;
            Init();
        }

        private void Init() {
            var options = new FirefoxOptions();
            options.AddArgument("-headless");
            driver = new FirefoxDriver(options);
            driver.Navigate().GoToUrl(url);
        }

        public IEnumerable<Lottery> GetLotteries(LotteryType lotteryType, IEnumerable<DateTime> dates) {
            ReadOnlyCollection<IWebElement> gameNodes = default!;

            bool ContinueLoading = true;
            int gameNodesRequiredCount = 10;

            while (ContinueLoading) {
                IWebElement moreResult = driver.FindElement(By.ClassName("more-results"));

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", moreResult);

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", moreResult);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => {
                    IList<IWebElement> gameNodes = driver.FindElements(By.ClassName("game-main-box-overbox"));
                    return gameNodes.Count > gameNodesRequiredCount;
                });

                gameNodes = FindElementsByClassName("game-main-box-overbox");

                foreach (var node in gameNodes) {
                    string scrapedDate = GetTrimmedInnerHtmlFromNodeByClassName(node, "sg__desc-title");
                    DateTime date = ConvertStringToDate(scrapedDate);

                    if (date < dates.First()) {
                        ContinueLoading = false;
                        break;
                    }
                }

                gameNodesRequiredCount += 10;
            }

            return ExtractLotteries(gameNodes, lotteryType, dates);
        }

        private IEnumerable<Lottery> ExtractLotteries(IEnumerable<IWebElement> gameNodes, LotteryType lotteryType, IEnumerable<DateTime> dates) {
            foreach (var node in gameNodes) {
                string scrapedLotteryID = GetTrimmedInnerHtmlFromNodeByClassName(node, "result-item__number");
                int lotteryID = Convert.ToInt32(scrapedLotteryID);

                string scrapedDate = GetTrimmedInnerHtmlFromNodeByClassName(node, "sg__desc-title");
                DateTime date = ConvertStringToDate(scrapedDate);

                string classType = GetClassType(lotteryType);
                ReadOnlyCollection<IWebElement> resultLottoNodes = GetResultNodes(node, classType);
                IEnumerable<int> resultsConverted = ConvertResultsToInt(GetResult(resultLottoNodes));

                if (date > dates.Last()) {
                    continue;
                }

                if (date < dates.First()) {
                    break;
                }

                yield return new Lottery() {
                    LotteryID = lotteryID,
                    Date = date,
                    Numbers = resultsConverted.ToList()
                };

                // Lotto -> classType: Lotto
                // Lotto Plus -> classType: LottoPlus
            }
        }

        private DateTime ConvertStringToDate(string date) {
            string regexPattern = @"([A-Za-z]+), (\d{2}.\d{2}.\d{4}), godz. (\d{2}:\d{2})";
            Match match = Regex.Match(date, regexPattern);

            string _date = match.Groups[2].Value;
            string _hours = match.Groups[3].Value;

            string _dateFormat = "dd.MM.yyyy, HH:mm";
            string _dateAndHours = $"{_date}, {_hours}";

            DateTime dateConverted = DateTime.ParseExact(_dateAndHours, _dateFormat, CultureInfo.InvariantCulture);
            return dateConverted;
        }

        private string GetClassType(LotteryType lotteryType) => $"{lotteryType}";
        
        private ReadOnlyCollection<IWebElement> GetResultNodes(IWebElement node, string type) => node.FindElement(By.ClassName(type)).FindElements(By.ClassName("scoreline-item"));

        private List<string> GetResult(ReadOnlyCollection<IWebElement> resultNodes) => resultNodes.Select(GetTrimmedInnerHtml).ToList();
        
        private ReadOnlyCollection<IWebElement> FindElementsByClassName(string className) {
            return driver.FindElements(By.ClassName(className));
        }

        private IWebElement FindElementFromNodeByClassName(IWebElement node, string className) {
            return node.FindElement(By.ClassName(className));
        }

        private string GetTrimmedInnerHtml(IWebElement node) {
            return node.GetAttribute("innerHTML").Trim();
        }

        private string GetTrimmedInnerHtmlFromNodeByClassName(IWebElement node, string className) {
            return GetTrimmedInnerHtml(FindElementFromNodeByClassName(node, className));
        }

        public IEnumerable<int> ConvertResultsToInt(IEnumerable<string> results) => results.Select(e => Convert.ToInt32(e));

        public void Dispose() {
            driver.Quit();
        }
    }
}
