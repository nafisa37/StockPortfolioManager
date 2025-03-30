public class Stock
{
    public int StockId { get; set; }
    public string TickerSymbol { get; set; }
    public string CompanyName { get; set; }
    public decimal CurrentPrice { get; set; }
    
    //not stored in database
    public decimal TotalValue(int numberOfShares) => numberOfShares * CurrentPrice;
}
