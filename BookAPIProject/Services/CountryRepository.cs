﻿using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class CountryRepository : ICountryRepository
    {
        BookDbContext _countryContext;

        public CountryRepository(BookDbContext countryContext)
        {
            _countryContext = countryContext;
        }



        public ICollection<Country> GetCountries()
        {
            return _countryContext.Countries.OrderBy(c => c.Name).ToList();
        }

        public Country GetCountry(int countryId)
        {
            return _countryContext.Countries.FirstOrDefault(c => c.Id == countryId);
        }

        public Country GetCountryOfAnAuthor(int authorId)
        {
            //return _countryContext.Authors.FirstOrDefault(a => a.Id == authorId)?.Country;
            return _countryContext.Authors.Where(a => a.Id == authorId).Select(c => c.Country).FirstOrDefault();
        }

        public ICollection<Author> GetAuthorsFromACountry(int countryId)
        {
            return (ICollection<Author>)_countryContext.Countries.Where(c => c.Id == countryId).Select(a => a.Authors).ToList();
            //return _countryContext.Authors.Where(c => c.Country.Id == countryId).ToList();
        }

        public bool CountryExists(int countryId)
        {
            return _countryContext.Countries.Any(c => c.Id == countryId);
        }
    }
}
