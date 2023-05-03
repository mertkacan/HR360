using ApplicationCore.DTO;
using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Ardalis.Specification;
using AutoMapper;
using Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Areas.Identity.Data;

namespace ApplicationCore.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _repository;
        private readonly IMapper _mapper;

        public CompanyService(IRepository<Company> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task CreateCompanyAsync(AppCompanyDTO appCompanyDTO)
        {
            // tüm mappingleri kontrol edecez
            var company = _mapper.Map<AppCompanyDTO, Company>(appCompanyDTO);


            if (appCompanyDTO.CompanyName != null)
            {
                var companyName = appCompanyDTO.CompanyName.Trim().ToLower();

                Dictionary<char, char> degisimTablosu = new Dictionary<char, char>()
                  {
                      {'ç', 'c'},
                      {'ğ', 'g'},
                      {'ı', 'i'},
                      {'ö', 'o'},
                      {'ş', 's'},
                      {'ü', 'u'}
                   };

                string orjinalMail = companyName + "@info.com";
                string yeniMail = "";

                foreach (char harf in orjinalMail)  
                {
                    if (degisimTablosu.ContainsKey(harf))
                        yeniMail += degisimTablosu[harf];

                    else
                        yeniMail += harf;
                }

                string trimmedEmail = yeniMail.Replace(" ", string.Empty);

                company.CompanyMail = trimmedEmail;

            }

            await _repository.AddAsync(company);
        }

        public async Task<Company> FirstOrDefaultAsync(Specification<Company> specification)
        {
            var company = await _repository.FirstOrDefaultAsync(specification);

            return company!;
        }

        public async Task<List<Company>> GetAllCompanyAsync()
        {
            var companies = await _repository.ListAsync();

            return companies.ToList();
        }

        public async Task<Company> GetCompanyByEmailAsync(string sirketEmail)
        {
            var specGetCompanyByEmail = new GetCompanyByEmailSpecification(sirketEmail);

            var company = await _repository.FirstOrDefaultAsync(specGetCompanyByEmail);

            return company!;
        }

        public async Task<Company> GetCompanyByIdAsync(Guid? id)
        {
            var company = await _repository.GetByIdAsync(id);
            return company!;
        }


        public async Task UpdateCompanyAsync(AppCompanyDTO appCompanyDTO)
        {
            var specGetCompanyById = new GetCompanyByIdSpecification(appCompanyDTO.Id);

            var company = await _repository.FirstOrDefaultAsync(specGetCompanyById);

            company!.Logo = appCompanyDTO.Logo;
            // Map the properties from company to appCompanyDTO
            _mapper.Map(company, appCompanyDTO);
            await _repository.UpdateAsync(company!);
        }


    }
}
