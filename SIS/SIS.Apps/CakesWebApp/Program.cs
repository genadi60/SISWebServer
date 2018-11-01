using System;
using System.Text;
using CakesWebApp.Data;
using Microsoft.EntityFrameworkCore;
using SIS.MvcFramework;

namespace CakesWebApp
{
    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            InitializeDatabase();

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
