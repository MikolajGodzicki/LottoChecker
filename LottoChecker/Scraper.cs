using HtmlAgilityPack;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LottoChecker {
    internal class Scraper {
        private HtmlDocument document;
        private string url;
        public Scraper(string url) {
            this.url = url;
            document = GetDocument(this.url);
        }
        private HtmlDocument GetDocument(string url) {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            return doc;
        }

        public IEnumerable<Lottery> GetLotteries(LotteryType lotteryType) {
            HtmlDocument doc = GetDocument(url);
            IList<HtmlNode> gameNodes = doc.DocumentNode.QuerySelectorAll(".game-main-box-overbox");

            foreach (HtmlNode node in gameNodes) {
                int lotteryID = Convert.ToInt32(node.QuerySelector(".result-item__number").InnerHtml.Trim());

                string date = node.QuerySelector(".sg__desc-title").InnerHtml.Trim();
                DateTime dateConverted = ConvertStringToDate(date);

                string classType = GetClassType(lotteryType);
                IList<HtmlNode> resultLottoNodes = GetResultNodes(node, classType);
                IEnumerable<int> resultsConverted = ConvertResultsToInt(GetResult(resultLottoNodes));

                yield return new Lottery() {
                    LotteryID = lotteryID,
                    Date = dateConverted,
                    Numbers = resultsConverted.ToList()
                };

                // Lotto -> query: .Lotto
                // Lotto Plus -> query: .LottoPlus
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

        private string GetClassType(LotteryType lotteryType) => $".{lotteryType}";

        private IList<HtmlNode> GetResultNodes(HtmlNode node, string type) => node.QuerySelectorAll(type).QuerySelectorAll(".scoreline-item");

        private List<string> GetResult(IList<HtmlNode> resultNodes) => resultNodes.Select(e => e.InnerHtml.Trim()).ToList();

        public IEnumerable<int> ConvertResultsToInt(IEnumerable<string> results) => results.Select(e => Convert.ToInt32(e));
    }
}
