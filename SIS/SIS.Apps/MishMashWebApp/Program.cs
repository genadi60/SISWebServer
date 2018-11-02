using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MishMashWebApp.Data;
using SIS.MvcFramework;

namespace MishMashWebApp
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
            using (var db = new MishMashDbContext())
            {
                db.Database.Migrate();
            }
        }
    }
}