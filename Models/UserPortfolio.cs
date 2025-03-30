using System.ComponentModel.DataAnnotations.Schema;

public class UserPortfolio
{
    public int PortfolioId { get; set; }  // primary Key
    public int UserId { get; set; }  // foreign key to UserAccounts
    public int StockId { get; set; }  // foreign key to Stocks
    public int SharesOwned { get; set; }

    // navigation properties
    public virtual UserAccount UserAccount { get; set; }
    [ForeignKey("StockId")]
    public virtual Stock Stock { get; set; }

    public decimal PortfolioValue => SharesOwned * Stock.CurrentPrice;
}
