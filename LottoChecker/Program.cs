// Wsystkie komentarze są sugestami
// Błędów nie widzę żadnych, sam w sobie kod też wygląda naprawdę dobrze

namespace LottoChecker {
    internal class Program {

        // Osobiście wydzielił bym kod zawartyw tej klasie do oddzielnej, nowej klasy
        // ale jest to szczegół

        static void Main(string[] args) {
            Scraper scraper = new Scraper("https://www.lotto.pl/lotto/wyniki-i-wygrane");
            IEnumerable<Lottery> LotteriesLotto = scraper.GetLotteries(LotteryType.Lotto);
            IEnumerable<Lottery> LotteriesLottoPlus = scraper.GetLotteries(LotteryType.LottoPlus);


            // Wydzieliłbym do oddzielnej metody
            Console.WriteLine("Typ:\n" +
                "1. Lotto\n" +
                "2. LottoPlus\n" +
                "3. Lotto + LottoPlus\n" +
                "Podaj typ kuponu, który chcesz sprawdzić: ");


            // Wydzieliłbym do oddzielnej metody
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

            // Zapisał bym tą operację w nowej metodzie, o nazwie która mówi nam co ma na celu ta operacja
            // dzięki temu już po samej nazwie metody wiadomo było by co tutaj się dzieje
            // teraz, też nie jest to trudne do ustalenia, ale jednak trzeba nieco rozkminić ten fragment kodu
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

        // Metoda ogólnie jest ok, nie jest dużo
        //
        // Jeśli miałbym coś poprawić to:
        // W tej metodzie jest mimo wszystko trochę logiki
        // Można by ją podzielić na conajmniej 2 metody
        // 1 - Pierwsza sprawdzała by poprawność liczb i przypisywała im kolory np. po przez nową klasę lub słownik lub innym sposobem
        // 2 - Druga metoda odpowiadała by za samo wyświetlanie danych z konsoli
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
