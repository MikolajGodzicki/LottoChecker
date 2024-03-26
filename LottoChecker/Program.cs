namespace LottoChecker {
    internal class Program {
        static void Main(string[] args) {
            Scraper scraper = new Scraper("https://www.lotto.pl/lotto/wyniki-i-wygrane");
            IEnumerable<Lottery> LotteriesLotto = scraper.GetLotteries(LotteryType.Lotto);
            IEnumerable<Lottery> LotteriesLottoPlus = scraper.GetLotteries(LotteryType.LottoPlus);

            Console.WriteLine("Typ:\n" +
                "1. Lotto\n" +
                "2. LottoPlus\n" +
                "3. Lotto + LottoPlus\n" +
                "Podaj typ kuponu, który chcesz sprawdzić: ");

            string type = Console.ReadLine();

            string? line;
            do {
                Console.WriteLine("Wpisz liczby z losowania po spacji (np: 1 2 30 34 47 49)");
                line = Console.ReadLine();

                if (line is null) {
                    return;
                }
            } while (line.Split(' ').Count() != 6);
            Console.WriteLine();

            IEnumerable<int> numbers = line.Split(' ').Select(e => Convert.ToInt32(e));

            switch(type) {
                case "1":
                    WriteWindow(LotteriesLotto, numbers, "Lotto: ");
                    break;
                case "2":
                    WriteWindow(LotteriesLottoPlus, numbers, "LottoPlus: ");
                    break;
                case "3":
                    WriteWindow(LotteriesLotto, numbers, "Lotto: ");
                    Console.WriteLine();
                    WriteWindow(LotteriesLottoPlus, numbers, "LottoPlus: ");
                    break;
            }
        }

        static void WriteWindow(IEnumerable<Lottery> lotteries, IEnumerable<int> numbers, string label) {
            Console.WriteLine(label);
            foreach (Lottery lottery in lotteries) {
                Console.Write(lottery.Date.ToString("g") + " ");
                foreach (int number in numbers) {
                    bool contains = lottery.Numbers.Contains(number);
                    if (contains) {
                        Console.ForegroundColor = ConsoleColor.Green;
                    } else {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(number);
                    Console.ResetColor();
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}
