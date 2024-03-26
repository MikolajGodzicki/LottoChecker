namespace LottoChecker {
    internal class Lottery {
        public DateTime Date { get; set; }
        public int LotteryID { get; set; }
        public IEnumerable<int> Numbers {  get; set; }

        public override string ToString() => $"{LotteryID}. {Date}: {string.Join(' ', Numbers)}";
    }
}
