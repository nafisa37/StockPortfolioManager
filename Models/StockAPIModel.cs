public class StockResponse
{
    public Meta Meta { get; set; }
    public Body Body { get; set; }
}

public class Meta
{
    public string Version { get; set; }
    public int Status { get; set; }
    public string Copywrite { get; set; }
}

public class Body
{
    public string Symbol { get; set; }
    public string CompanyName { get; set; }
    public string StockType { get; set; }
    public string Exchange { get; set; }
    public PrimaryData PrimaryData { get; set; }
}

public class PrimaryData
{
    public string LastSalePrice { get; set; }
    public string NetChange { get; set; }
    public string PercentageChange { get; set; }
    public string DeltaIndicator { get; set; }
    public string LastTradeTimestamp { get; set; }
    public bool IsRealTime { get; set; }
}
