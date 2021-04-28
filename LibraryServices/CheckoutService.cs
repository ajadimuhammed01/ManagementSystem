using LibraryData;
using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {

        private LibraryContext _context;

        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Checkout newCheckOut)
        {
            _context.Add(newCheckOut);
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;

            var item = _context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);

            _context.Update(item);

            //remove any existing checkouts on the item
            // RemoveExistingCheckouts(assetId);
            var checkout = _context.Checkouts
               .Include(c => c.libraryAsset)
               .Include(c => c.LibraryCard)
               .FirstOrDefault(a => a.libraryAsset.Id == assetId);
            if (checkout != null) _context.Remove(checkout);



            //close any existing checkut history
            // CloseExistingCheckoutsHistory(assetId, now);

            var history = _context.CheckoutHistories
                .Include(h => h.libraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h =>
                    h.libraryAsset.Id == assetId
                    && h.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }

            //look  for exisiing hold on the item
            var currentHolds = _context.Holds
                .Include(h => h.libraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.libraryAsset.Id == assetId);

            //if there are holds, checkout the item too the librarycard with the earliest
           if(currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
                return;
            }

            //otherwise, update the item status to available
            //UpdateAssetStatus(assetId, "Available");
            //_context.SaveChanges();

            // otherwise, set item status to available
            item.Status = _context.Statuses.FirstOrDefault(a => a.Name == "Available");

            _context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault();

            var card = earliestHold.LibraryCard;
            _context.Remove(earliestHold);
            _context.SaveChanges();
        }

        public void CheckOutItem(int assetId, int LibraryCardId)
        {
               if(IsCheckedOut(assetId))
            {
                return;
            }

            var item = _context.LibraryAssets
                .Include(a=> a.Status)
                .FirstOrDefault(a => a.Id == assetId);

           // _context.Update(item);

            item.Status = _context.Statuses
            .FirstOrDefault(a => a.Name == "Checked out");

           // UpdateAssetStatus(assetId, "Checked Out");

            var libraryCard = _context.LibraryCards
                .Include(card => card.Checkout)
                .FirstOrDefault(card => card.Id == LibraryCardId);

            var now = DateTime.Now;

            var checkout = new Checkout
            {
                libraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Untill = GetDefaultCheckOutTime(now)

            };
            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                libraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public bool IsCheckedOut(int assetId)
        {
            return _context.Checkouts
                .Where(co => co.libraryAsset.Id == assetId)
                .Any();
        }

        private DateTime GetDefaultCheckOutTime(DateTime now)
        {
            return now.AddDays(30);
        }


       

        private void CheckoutToEarliest(int assetId, IQueryable<Hold>currentHolds)

        {
            var earliestHold = currentHolds.OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault();

            var card = earliestHold.LibraryCard;
            _context.Remove(earliestHold);
            _context.SaveChanges();
            CheckOutItem(assetId, card.Id); 
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll().FirstOrDefault(Checkout => Checkout.Id == checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(h => h.libraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.libraryAsset.Id == id);
        }

        public string GetCurrentHoldPatron(int id)
        {
            var hold = _context.Holds
                .Include(h => h.libraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.Id == id);

            //var cardId = hold?.LibraryCard.Id;

            var cardId = hold
               .Include(a => a.LibraryCard)
               .Select(a => a.LibraryCard.Id)
               .FirstOrDefault();

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            //return patron?.FirstName + " " + patron.LastName;
            return patron?.FullName;

        }



        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return
                _context.Holds
                .Include(h => h.libraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId)
                .HoldPlaced;
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.libraryAsset)
                 
                .Where(h => h.libraryAsset.Id == id);
        }

        public void MarkFound(int id)
        {
            var now = DateTime.Now;

            var item = _context.LibraryAssets
                .First(a => a.Id == id);

            _context.Update(item);
            item.Status = _context.Statuses.FirstOrDefault(a => a.Name == "Available");


            var checkout = _context.Checkouts
                .FirstOrDefault(a => a.libraryAsset.Id == id);
            if (checkout != null) _context.Remove(checkout);

            //RemoveExistingCheckouts(assetId);

            var history = _context.CheckoutHistories
                .FirstOrDefault(h => h.libraryAsset.Id == id
                                && h.CheckedIn == null);

            if(history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }

            _context.SaveChanges();

            //CloseExistingCheckoutsHistory(assetId, now);

           
            //close any existing checkout history
           
        }

        private void UpdateAssetStatus(int assetId, string newStatus)
        {

            var item = _context.LibraryAssets
                .Include(h => h.Status)
               .FirstOrDefault(a => a.Id == assetId);
            _context.Update(item);


            item.Status = _context.Statuses.FirstOrDefault(Status => Status.Name == newStatus);

           // UpdateAssetStatus(assetId, "Available");
        }

        private void CloseExistingCheckoutsHistory(int assetId, DateTime now)
        {
            var history = _context.CheckoutHistories
               .FirstOrDefault(h => h.libraryAsset.Id == assetId
               && h.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }

            _context.SaveChanges(); 
        }

        private void RemoveExistingCheckouts(int assetId)
        {
            var checkout = _context.Checkouts
               .FirstOrDefault(co => co.libraryAsset.Id == assetId);
            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public Checkout GetLatestCheckout (int assetId)
        {
            return _context.Checkouts
                .Where(c => c.libraryAsset.Id == assetId)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }

        public void MarkLost(int assetId)
        {

            var now = DateTime.Now;

            UpdateAssetStatus(assetId, "Lost");

            var checkout = _context.Checkouts.FirstOrDefault(co => co.libraryAsset.Id == assetId);

            if(checkout != null)
            {
                _context.Remove(checkout);
            }

            
            var history = _context.CheckoutHistories
                .FirstOrDefault(h => h.libraryAsset.Id == assetId
                && h.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
            _context.SaveChanges();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .Include(a => a.Status)
                .FirstOrDefault(a => a.Id == assetId);

            var card = _context.LibraryCards
                .FirstOrDefault(a => a.Id == libraryCardId);

            _context.Update(asset);

            if(asset.Status.Name == "Available")
            {
                //UpdateAssetStatus(assetId, "On Hold");
                asset.Status = _context.Statuses.FirstOrDefault(a => a.Name == "On Hold");

            }
            var hold = new Hold
            {
                HoldPlaced = now,
                libraryAsset = asset,
                LibraryCard = card
            };
            _context.Add(hold);
            _context.SaveChanges();
        }

        public DateTime GetCurrentHoldPlced(int holdId)
        {
            return
                _context.Holds
                .Include(h => h.libraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId)
                .HoldPlaced;

        }

       public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            if (checkout == null)
            {
                return "Not checked out.";
            }

            var cardId = checkout.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
                

        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(co => co.libraryAsset)
                .Include(co => co.LibraryCard)
                .FirstOrDefault(co => co.libraryAsset.Id == assetId);
        }

     
    }
}
