using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly APIHelper _APIHelper;

     public AccountController(AppDbContext context, IHttpClientFactory httpClientFactory, APIHelper APIHelper)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _APIHelper = APIHelper;
    }
    
    public IActionResult Index()
    {
        return View(_context.UserAccounts.ToList());
    }
    public IActionResult Registration()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Registration(RegistrationViewModel model)
    {
        if (ModelState.IsValid)
        {
            //Console.WriteLine("Checking if ModelState is valid...");
            UserAccount account = new UserAccount();
            account.Username = model.Username;
            account.Email = model.Email;
            // account.FirstName = model.FirstName;
            // account.LastName = model.LastName;
            account.Password = model.Password;

            try
            {
                Console.WriteLine($"Email: {model.Email}, Username: {model.Username}, Password: {model.Password}");
                _context.UserAccounts.Add(account);
                _context.SaveChanges(); //save data in database

                ModelState.Clear();
                ViewBag.Message = $"{account.Username} registered successfully!".Trim();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Username/Email already taken. Please try another one.");
                return View(model);
            }

            return View();
        }
        return View(model);
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.UserAccounts.Where(x => (x.Username == model.UsernameOrEmail || x.Email == model.UsernameOrEmail) && x.Password == model.Password).FirstOrDefault();
            if (user != null)
            {
                //Success, create cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("Username", user.Username),
                    //new Claim("Name", user.FirstName),
                    new Claim(ClaimTypes.Role, "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                foreach (var claim in claimsIdentity.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)); 
                
                return RedirectToAction("Dashboard");
            }
            else{
                ModelState.AddModelError("", "Username/Email or Password incorrect.");
            }
        }
        return View(model);
    }
    
    public IActionResult LogOut()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Dashboard()
    {
        var username = User.FindFirst("Username")?.Value;

        //get user from database
        var user = _context.UserAccounts.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        // get user's portfolio info
        var portfolio = _context.UserPortfolio
            .Where(up => up.UserId == user.UserId)  //filter by user ID
            .Include(up => up.Stock)
            .ToList();

        //calculate portfolio value and pass to view
        decimal totalPortfolioValue = portfolio.Sum(up => up.PortfolioValue);


        ViewBag.Cash = user.Cash;
        ViewBag.Username = user.Username;
        ViewBag.TotalPortfolioValue = totalPortfolioValue;

        ViewBag.Error = TempData["Error"];

        return View(portfolio);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> BuyStock(string ticker, int shares)
    {
        Console.WriteLine("Entering Buy Stock");

        if (shares <= 0)
        {
            TempData["Error"] = "Invalid number of shares";
            return RedirectToAction("Dashboard");
        }

        var username = User.FindFirst("Username")?.Value;
        var user = _context.UserAccounts.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        Console.WriteLine($"Ticker before calling Helper function: {ticker}");
        Console.WriteLine($"Shares before calling Helper function: {shares}");
        var (symbol, companyName, currentPrice) = await _APIHelper.GetStockInfoFromApi(ticker);
        // var symbol = "FMX";
        // var companyName = "Fomento Economico Mexicano S.A.B. de C.V. Common Stock";
        // decimal currentPrice = 96.61m;
        Console.WriteLine($"Stock info - Symbol: {symbol}, Company: {companyName}, Current Price: {currentPrice}");

        if (currentPrice <= 0)
        {
            TempData["Error"] = "Invalid stock price. Please try again";
            return RedirectToAction("Dashboard");
            
        }

        decimal totalCost = shares * currentPrice;

        if (user.Cash < totalCost)
        {
            TempData["Error"] = "Insufficient funds to buy stock.";
            return RedirectToAction("Dashboard");
        }

        // see if stock is in database
        var stock = _context.Stocks.FirstOrDefault(s => s.TickerSymbol == symbol);
        //Console.WriteLine($"Stock found: {stock.TickerSymbol}");
        if (stock == null)
        {
            //add new stock to database
            Console.WriteLine("ENTERING ADDING NEW STOCK CODE");
            stock = new Stock { TickerSymbol = symbol, CompanyName = companyName, CurrentPrice = currentPrice };
            _context.Stocks.Add(stock);
            _context.SaveChanges();
        }
        else
        {
            //update stock price in database to latest
            Console.WriteLine("UPDATING STOCK PRICE IN DATABASE");
            stock.CurrentPrice = currentPrice;
            _context.Stocks.Update(stock);
        }

        // check if user already owns this stock
        var portfolio = _context.UserPortfolio.FirstOrDefault(up => up.UserId == user.UserId && up.StockId == stock.StockId);

        if (portfolio != null)
        {
            portfolio.SharesOwned += shares;
        }
        else
        {
            // adds new stock to user portfolio
            portfolio = new UserPortfolio { UserId = user.UserId, StockId = stock.StockId, SharesOwned = shares };
            _context.UserPortfolio.Add(portfolio);
        }

        user.Cash -= totalCost;

        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SellStock(string ticker, int shares)
    {
        if (shares <= 0)
        {
            TempData["Error"] = "Invalid number of shares";
            return RedirectToAction("Dashboard");
        }

        var username = User.FindFirst("Username")?.Value;
        var user = _context.UserAccounts.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var (symbol, companyName, currentPrice) = await _APIHelper.GetStockInfoFromApi(ticker);

        var stock = _context.Stocks.FirstOrDefault(s => s.TickerSymbol == symbol);

        if (stock == null)
        {
            TempData["Error"] = "Not enough shares to sell";
            return RedirectToAction("Dashboard");
        }
        // else{
        //     Console.WriteLine($"Stock found: {stock.TickerSymbol}");
        // }

        var portfolio = _context.UserPortfolio.FirstOrDefault(up => up.UserId == user.UserId && up.StockId == stock.StockId);

        if (portfolio == null || portfolio.SharesOwned < shares)
        {
            TempData["Error"] = "Not enough shares to sell";
            return RedirectToAction("Dashboard");
        }

        decimal totalEarnings = shares * currentPrice;

        portfolio.SharesOwned -= shares;

        if (portfolio.SharesOwned == 0)
        {
            _context.UserPortfolio.Remove(portfolio);
        }

        user.Cash += totalEarnings;

        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> GetStockResults(string ticker)
    {
        Console.WriteLine("ENTERING GETTING STOCK RESULTS");
        if (string.IsNullOrWhiteSpace(ticker))
        {
            TempData["Error"] = "Please provide a valid stock ticker";
            return RedirectToAction("Dashboard");
        }

        try
        {
            var (symbol, companyName, lastSalePrice) = await _APIHelper.GetStockInfoFromApi(ticker);
            Console.WriteLine("BOGUS TOKEN");
            Console.WriteLine($"TOKEN: {symbol}");
            if (!string.IsNullOrEmpty(symbol))
            {
                TempData["CompanyName"] = companyName;
                TempData["LastSalePrice"] = lastSalePrice.ToString("F2"); //decimal formatting 2 places, no $
                TempData["TickerSymbol"] = symbol;

                return RedirectToAction("Dashboard");
            }
            else //if bogus token
            {   Console.WriteLine("BOGUS TOKEN 2");
                TempData["Error"] = "Invalid stock ticker. Please check your input.";
                return RedirectToAction("Dashboard");
            }
        }
        catch (InvalidTickerException ex){
            TempData["Error"] = "Invalid stock ticker. Please check your input.";
            return RedirectToAction("Dashboard");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error fetching stock data: {ex.Message}");
        }

        return RedirectToAction("Dashboard");
    }

}