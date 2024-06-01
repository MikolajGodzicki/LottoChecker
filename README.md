# LottoChecker

LottoChecker is Console Application used for Web Scraping Lotto Page to get lottery winning numbers between chosen dates.
I created API which includes scraper using Selenium and Firefox Driver.

## Usage and implementation

There is whole new class (Scraper) to Web scrape and get data from page.

I Implemented LotteryType enum used to define which type you want to check
```csharp
internal enum LotteryType {
    Lotto,
    LottoPlus
}
```

You need to pass this type in GetLotteries method and dates between you want to search [2024-01-01, 2024-02-02].
```csharp
IEnumerable<Lottery> GetLotteries(LotteryType lotteryType, IEnumerable<DateTime> dates)
```

It returns lottery Model
```csharp
internal class Lottery {
    public DateTime Date { get; set; }
    public int LotteryID { get; set; }
    public IEnumerable<int> Numbers {  get; set; }

    public override string ToString() => $"{LotteryID}. {Date}: {string.Join(' ', Numbers)}";
}
```
