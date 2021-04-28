using System;

namespace LibraryData.Models
{
    public class Patron
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string TelephoneNumber { get; set; }

        public string FullName
        {
            get { return FirstName + ", " + LastName; }
        }
       // public int LibraryCardID { get; set; }
        //public int HomeLibraryBranchID { get; set; }


        public virtual LibraryCard LibraryCard { get; set; }
        public virtual LibraryBranch HomeLibraryBranch { get; set; }


       

    }
}
