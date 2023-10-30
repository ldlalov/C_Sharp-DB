using Microsoft.EntityFrameworkCore;
using P02_FootballBetting.Data;

public class StartUp
{
    private static void Main(string[] args)
    {
        var context = new FootballBettingContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}