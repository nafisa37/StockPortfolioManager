using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<UserAccount> UserAccounts {get; set;}
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<UserPortfolio> UserPortfolio { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserAccount>().ToTable("UserAccounts");
        modelBuilder.Entity<UserAccount>().Property(u => u.UserId).HasColumnName("UserID");
        modelBuilder.Entity<UserAccount>().Property(u => u.Username).HasColumnName("Username");
        modelBuilder.Entity<UserAccount>().Property(u => u.Email).HasColumnName("Email");
        modelBuilder.Entity<UserAccount>().Property(u => u.Password).HasColumnName("Password");
        modelBuilder.Entity<UserAccount>().Property(u => u.Cash)
            .HasColumnName("Cash")
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Stock>().ToTable("Stocks");
        modelBuilder.Entity<Stock>().Property(s => s.StockId).HasColumnName("StockID");
        modelBuilder.Entity<Stock>().Property(s => s.TickerSymbol).HasColumnName("TickerSymbol");
        modelBuilder.Entity<Stock>().Property(s => s.CompanyName).HasColumnName("CompanyName");
        modelBuilder.Entity<Stock>().Property(s => s.CurrentPrice)
            .HasColumnName("CurrentPrice")
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<UserPortfolio>().ToTable("UserPortfolio");
        modelBuilder.Entity<UserPortfolio>().Property(up => up.PortfolioId).HasColumnName("PortfolioID");
        modelBuilder.Entity<UserPortfolio>().Property(up => up.UserId).HasColumnName("UserID");
        modelBuilder.Entity<UserPortfolio>().Property(up => up.StockId).HasColumnName("StockID");
        modelBuilder.Entity<UserPortfolio>().Property(up => up.SharesOwned).HasColumnName("SharesOwned");

        modelBuilder.Entity<UserPortfolio>().HasKey(up => up.PortfolioId);  

        modelBuilder.Entity<UserPortfolio>()
            .HasOne(up => up.UserAccount)
            .WithMany(u => u.Portfolios)
            .HasForeignKey(up => up.UserId);

        modelBuilder.Entity<UserPortfolio>()
            .HasOne(up => up.Stock)
            .WithMany()
            .HasForeignKey(up => up.StockId);
    }

}