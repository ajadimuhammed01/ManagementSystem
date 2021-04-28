using System;

namespace LibraryData.Models
{
    public  class Hold
    {

        public int Id { get; set; }

        public virtual LibraryAsset libraryAsset { get; set; }
        
        public virtual LibraryCard LibraryCard { get; set; }

        public DateTime HoldPlaced { get; set; }
    }
}
