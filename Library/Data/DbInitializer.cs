using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using LibraryData;
using LibraryData.Models;

namespace Library.Data
{
    public class DbInitializer
    {
        public static void Initialize(LibraryContext context)
        {
            context.Database.EnsureCreated();

            if (context.Patrons.Any())
            {
                return;
            }

            //var patrons = new Patron[]
            //{
            //    new Patron {Address = "165, Peace St", DateOfBirth = DateTime.Parse ("1986-07-10"), FirstName = "Jane", HomeLibraryBranchID=1, LibraryCardID = 1, TelephoneNumber= "08033086298"},
            //};

            var librarybranchs = new LibraryBranch[]
            {
                new LibraryBranch {ImageUrl =null/*"/images/branches/1.png"*/, Address="Banire Street", Name = "Ajadi", Telephone = "080330", OpenDate= DateTime.Parse("2004-04-12"), Description="The oldest library branch in Lakeview, the Lake Shore Branch was opened in 1975. Patrons of all ages enjoy the wide selection of literature available at Lake Shore library. The coffee shop is open during library hours of operation." }
            };

            var librarycards = new LibraryCard[]
            {
                new LibraryCard{ Created = DateTime.Parse("2002-03-11"), Fees= 200 }
            };

            var statuses = new Status[]
            {
                new Status{Name = "Checked out", Description="A library asset that has been checked out"}
            };

            var libraryassets = new LibraryAsset[]
            {
               
            };

        }
    }
}
