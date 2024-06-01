using LottoAPI;

namespace LottoChecker {
    internal class Program {
        static void Main(string[] args) {
            using (Scraper scraper = new Scraper("https://www.lotto.pl/lotto/wyniki-i-wygrane")) {


                Console.WriteLine("Typ:\n" +
                    "1. Lotto\n" +
                    "2. LottoPlus\n" +
                    "3. Lotto + LottoPlus\n" +
                    "Podaj typ kuponu, który chcesz sprawdzić: ");

                string type = Console.ReadLine();

                Console.WriteLine("Podaj datę od której i do której szukać (np: 2024-02-20 2024-06-01):");

                string dates = Console.ReadLine();
                IEnumerable<DateTime> convertedDates = dates.Split(' ').Select(DateTime.Parse).ToArray();

                string? line;
                do {
                    Console.WriteLine("Wpisz liczby z losowania po spacji (np: 1 2 30 34 47 49): ");
                    line = Console.ReadLine();

                    if (line is null) {
                        return;
                    }
                } while (line.Split(' ').Count() != 6);
                Console.WriteLine();

                IEnumerable<int> numbers = line.Split(' ').Select(e => Convert.ToInt32(e));

                IEnumerable<Lottery> LotteriesLotto;
                IEnumerable<Lottery> LotteriesLottoPlus;

                switch (type) {
                    case "1":
                        LotteriesLotto = scraper.GetLotteries(LotteryType.Lotto, convertedDates);
                        WriteWindow(LotteriesLotto, numbers, "Lotto: ");
                        break;
                    case "2":
                        LotteriesLottoPlus = scraper.GetLotteries(LotteryType.LottoPlus, convertedDates);
                        WriteWindow(LotteriesLottoPlus, numbers, "LottoPlus: ");
                        break;
                    case "3":
                        LotteriesLotto = scraper.GetLotteries(LotteryType.Lotto, convertedDates);
                        LotteriesLottoPlus = scraper.GetLotteries(LotteryType.LottoPlus, convertedDates);
                        WriteWindow(LotteriesLotto, numbers, "Lotto: ");
                        Console.WriteLine();
                        WriteWindow(LotteriesLottoPlus, numbers, "LottoPlus: ");
                        break;
                }

                Console.WriteLine("Zakończono wyszukwanie");
                Console.ReadLine();
            } 
        }


        static void WriteWindow(IEnumerable<Lottery> lotteries, IEnumerable<int> numbers, string label) {
            Console.WriteLine(label);
            foreach (Lottery lottery in lotteries) {
                Console.Write(lottery.Date.ToString("g") + " ");
                int correctCount = 0;
                foreach (int number in numbers) {
                    bool contains = lottery.Numbers.Contains(number);
                    if (contains) {
                        Console.ForegroundColor = ConsoleColor.Green;
                        correctCount++;
                    } else {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(number);
                    Console.ResetColor();
                    Console.Write(" ");
                }
                Console.Write($"Trafione: {correctCount}");

                Console.WriteLine();
            }
        }
    }
}
