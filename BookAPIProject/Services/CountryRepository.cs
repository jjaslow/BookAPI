using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class CountryRepository : ICountryRepository
    {
        BookDbContext _context;

        public CountryRepository(BookDbContext countryContext)
        {
            _context = countryContext;
        }


        //////////////


        public ICollection<Country> GetCountries()
        {
            return _context.Countries.OrderBy(c => c.Name).ToList();
        }

        public Country GetCountry(int countryId)
        {
            return _context.Countries.FirstOrDefault(c => c.Id == countryId);
        }

        public Country GetCountryOfAnAuthor(int authorId)
        {
            return _context.Authors.Where(a => a.Id == authorId).Select(c => c.Country).FirstOrDefault();
        }

        public ICollection<Author> GetAuthorsFromACountry(int countryId)
        {
            return _context.Authors.Where(c => c.Id == countryId).ToList();
        }

        public bool CountryExists(int countryId)
        {
            return _context.Countries.Any(c => c.Id == countryId);
        }

        public bool IsDuplicateCountryName(int countryID, string countryName)
        {
            bool countryNameExists = _context.Countries.Any(c => c.Name == countryName);
            if (!countryNameExists)
                return false;
            else
            {
                Country existingCountry = _context.Countries.FirstOrDefault(c => c.Name.Trim().ToUpper() == countryName.Trim().ToUpper());
                return existingCountry.Id != countryID;
            }
        }



        ////////


        public bool CreateCountry(Country country)
        {
            _context.Add(country);
            return Save();
        }

        public bool UpdateCountry(Country country)
        {
            _context.Update(country);
            return Save();
        }

        public bool DeleteCountry(Country country)
        {
            _context.Remove(country);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved >= 0;
        }

    }
}
