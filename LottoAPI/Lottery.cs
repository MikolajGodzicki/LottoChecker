namespace LottoAPI {
    public class Lottery {
        public DateTime Date { get; set; }
        public int LotteryID { get; set; }
        public IEnumerable<int> Numbers { get; set; } = default!;

        public override string ToString() => $"{LotteryID}. {Date}: {string.Join(' ', Numbers)}";
    }
}
