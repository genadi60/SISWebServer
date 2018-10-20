namespace CakesWebApp
{
    using System;
    using System.Text;
    using Microsoft.EntityFrameworkCore;

    using Data;
    using SIS.MvcFramework;

    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            ////InitializeDatabase();

            WebHost.Start(new Startup());
        }

        private static void InitializeDatabase()
        {
            using (var db = new CakesDbContext())
            {
                db.Database.Migrate();
            }
        }
    }
}
