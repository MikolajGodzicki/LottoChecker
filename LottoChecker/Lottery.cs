namespace LottoChecker {
    internal class Lottery {
        public DateTime Date { get; set; }
        public int LotteryID { get; set; }
        // Tutaj można być by zainicjować pole lub dać default!
        // public IEnumerable<int> Numbers {  get; set; } = default!;
        // Choć nie jest to błąd
        public IEnumerable<int> Numbers {  get; set; }

        public override string ToString() => $"{LotteryID}. {Date}: {string.Join(' ', Numbers)}";
    }
}
