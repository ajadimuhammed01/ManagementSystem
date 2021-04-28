using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface ICheckout
    {
        void Add(Checkout newCheckOut);

        IEnumerable<Checkout> GetAll();
        IEnumerable<Hold> GetCurrentHolds(int id);

        Checkout GetById(int checkoutId);
        Checkout GetLatestCheckout(int id);
        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId);
        void PlaceHold(int assetId, int libraryCardId);
        void MarkLost(int assetId);
        void MarkFound(int assetId);
        DateTime GetCurrentHoldPlaced(int id);

        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
       
        string GetCurrentCheckoutPatron(int assetId);
        string GetCurrentHoldPatron(int id);
        
       
        bool IsCheckedOut(int id);

      
    }
}
