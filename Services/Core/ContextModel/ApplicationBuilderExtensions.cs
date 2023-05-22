using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;


//dotnet ef migrations add db1 --context myDBContext --output-dir Migrations

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
    {
        using (IServiceScope scope = app.ApplicationServices.CreateScope())
        using (myDBContext context = scope.ServiceProvider.GetRequiredService<myDBContext>())
        {
            try
            {
                //apply migrations
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            //seed data
            SeedData(context);
        }

        return app;
    }

    private static void SeedData(myDBContext dataContext)
    {
        SeedDataCMS(dataContext);
    }

    static void SeedDataCMS(myDBContext dataContext)
    {

        var lang2 = dataContext.Lang.Add(new Lang() { Name = "Ýngilizce", Code = "EN", CreaDate = DateTime.Now, CreaUser = 1 });
        var lang1 = dataContext.Lang.Add(new Lang() { Name = "Türkçe", Code = "TR", CreaDate = DateTime.Now, CreaUser = 1 });
        var lang3 = dataContext.Lang.Add(new Lang() { Name = "Almanca", Code = "DE", CreaDate = DateTime.Now, CreaUser = 1 });
        var lang4 = dataContext.Lang.Add(new Lang() { Name = "Rusça", Code = "RU", CreaDate = DateTime.Now, CreaUser = 1 });
        var lang5 = dataContext.Lang.Add(new Lang() { Name = "Fransýzca", Code = "FR", CreaDate = DateTime.Now, CreaUser = 1 });

        dataContext.SaveChanges();




        var user1 = dataContext.User.Add(new User()
        {
            Name = "admin",
            UserName = "admin",
            Pass = "123",
            CreaDate = DateTime.Now,
            CreaUser = 1,
        });
        dataContext.SaveChanges();


        var siteConfig = dataContext.SiteConfig.Add(new SiteConfig()
        {
            Title = "Site",
            BaseUrl = "http://coupon.hybro.systems/",
            layoutUrlBase = "http://couponcms.hybro.systems/",
            layoutUrl = "http://couponcms.hybro.systems/",
            ImageUrl = "http://couponcms.hybro.systems/",
            JokerPass = "123_*1",
            StartPage = "Base",
            StartAction = "Index",
            layoutId = "",
            version = "1",
            CreaUser = 1,
            CreaDate = DateTime.Now,
        });

        dataContext.SaveChanges();


    }
}
