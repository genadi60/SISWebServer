namespace MishMashWebApp
{
    ////using Microsoft.EntityFrameworkCore;
    using System;
    using System.Text;
    
    ////using Data;
    using SIS.MvcFramework;

    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            ////InitializeDatabase();

            WebHost.Start(new Startup());
        }

        ////private static void InitializeDatabase()
        ////{
        ////    using (var db = new MishMashDbContext())
        ////    {
        ////        db.Database.Migrate();
        ////    }
        ////}
    }
}