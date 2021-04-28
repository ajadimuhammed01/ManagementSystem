using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {

        private LibraryContext _context;

        public LibraryAssetService(LibraryContext context) 
        {
            _context = context;
        }

        public async Task Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            await _context.SaveChangesAsync();
        }

        public async Task Create(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            
           await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
             var asset = GetById(id);
            _context.Remove(asset);
            await _context.SaveChangesAsync(); 
        }
        public async Task Update(int id, string Title, int Year, decimal Cost, string imageUrl)
        {
            var item =  GetById(id);
            item.Title = Title;
            item.Year = Year;
            item.Cost = Cost;
            item.ImageUrl = imageUrl;

            _context.Update(item);
            await _context.SaveChangesAsync();




        }
        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location);
        }

        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>()
                .Where(asset => asset.Id == id).Any();

            var isVideo = _context.LibraryAssets.OfType<Video>()
                .Where(asset => asset.Id == id).Any();

            return isBook ?
                _context.Books.FirstOrDefault(book => book.Id == id).Author :
                _context.Videos.FirstOrDefault(video => video.Id == id).Director
                ?? "Unknown";

        }

        public LibraryAsset GetById(int id)
        {
            return
                GetAll()
                  .FirstOrDefault(asset => asset.Id == id);
                
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id).Location;
            //return _context.LibraryAssets.FirstOrDefault(asset => asset.Id == id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {

                return _context.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }

            else return "";

           
        }

        public string GetIsbn(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {

                return _context.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }

            else return "";
        }

        public string GetTitle(int id)
        {
            return _context.LibraryAssets.FirstOrDefault(a => a.Id == id).Title;
        }

        public string GetType(int id)
        {
            var book = _context.LibraryAssets.OfType<Book>().Where(b => b.Id == id);

            return book.Any() ? "Book" : "Video";
        }

        //public IEnumerable<LibraryAsset> ILibraryAsset.GetAll()
        //{
        //    return _context.LibraryAssets
        //        .Include
        //}
    }
}
