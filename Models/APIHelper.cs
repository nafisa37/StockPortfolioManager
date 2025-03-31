using Newtonsoft.Json;

public class APIHelper
{
    private readonly IHttpClientFactory _httpClientFactory;
    public APIHelper(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<(string Symbol, string CompanyName, decimal LastSalePrice)> GetStockInfoFromApi(string ticker)
    {

        string apiUrl = $"https://yahoo-finance15.p.rapidapi.com/api/v1/markets/quote?ticker={ticker}&type=STOCKS";
        string apiKey = Environment.GetEnvironmentVariable("API_Key");
        Console.WriteLine($"API Key: {apiKey}"); 
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        
        requestMessage.Headers.Add("X-RapidAPI-Key", apiKey);
        requestMessage.Headers.Add("X-RapidAPI-Host", "yahoo-finance15.p.rapidapi.com");

        var client = _httpClientFactory.CreateClient();
        HttpResponseMessage response = await client.SendAsync(requestMessage);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseContent) || responseContent.TrimStart().StartsWith("<"))
            {
                Console.WriteLine("Not getting the correct JSON");
            }
            //Console.WriteLine($"API Response: {responseContent}");
            try{
                var stockData = JsonConvert.DeserializeObject<StockResponse>(responseContent);
                var stockInfo = stockData?.Body?.PrimaryData;
                
                if (stockInfo != null)
                {
                    string priceString = stockInfo.LastSalePrice.Replace("$", "").Trim();
                    decimal lastSalePrice = decimal.TryParse(priceString, out var price) ? price : 0m;
                    return (stockData.Body.Symbol, stockData.Body.CompanyName, lastSalePrice);
                }
                else
                {
                    Console.WriteLine($"Error fetching data: {response.StatusCode}");
                    return ("", "", 0m); //returns empty tuple if API call fails
                }
            }
            catch (Exception ex){
                Console.WriteLine("Invalid Stock Ticker");
                throw new InvalidTickerException ();
            }
        }
        else
        {
            Console.WriteLine($"Error fetching stock data: {response.StatusCode}");
            return ("", "", 0m); //returns empty tuple if API call fails
        }
    }

}
