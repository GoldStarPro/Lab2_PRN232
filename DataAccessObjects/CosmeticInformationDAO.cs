using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class CosmeticInformationDAO
    {
        private static CosmeticInformationDAO? _instance = null;
        public static CosmeticInformationDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CosmeticInformationDAO();
                }
                return _instance;
            }
        }

        private CosmeticInformationDAO()
        {
            // Private constructor to prevent instantiation
        }

        public async Task<List<CosmeticInformation>> GetAllCosmetic()
        {
            try
            {
                using (var context = new CosmeticsDbContext())
                {
                    return await context.CosmeticInformations
                        .Include(c => c.Category)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving cosmetic information.", ex);
            }
        }

        public async Task<List<CosmeticCategory>> GetAllCategories()
        {
            try
            {
                using (var context = new CosmeticsDbContext())
                {
                    return await context.CosmeticCategories.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving cosmetic categories.", ex);
            }
        }

        public async Task<CosmeticInformation> AddCosmeticInformation(CosmeticInformation cosmeticInformation)
        {
            try
            {
                using (var context = new CosmeticsDbContext())
                {
                    var categoryObject = await context.CosmeticCategories.FirstOrDefaultAsync(c => c.CategoryId.Equals(cosmeticInformation.CategoryId));
                    if (categoryObject == null)
                    {
                        throw new Exception("Category is not found");
                    }

                    cosmeticInformation.CosmeticId = GenerateId();
                    await context.CosmeticInformations.AddAsync(cosmeticInformation);
                    await context.SaveChangesAsync();
                    return cosmeticInformation;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding cosmetic information.", ex);
            }
        }

        public async Task<CosmeticInformation> GetById(string id)
        {
            try
            {
                using (var context = new CosmeticsDbContext())
                {
                    var cosmeticInformation = await context.CosmeticInformations
                        .Include(c => c.Category)
                        .FirstOrDefaultAsync(c => c.CosmeticId == id);
                    if (cosmeticInformation == null)
                    {
                        throw new Exception("Cosmetic information not found.");
                    }
                    return cosmeticInformation;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving cosmetic information by ID.", ex);
            }
        }

        public async Task<CosmeticInformation> Update(CosmeticInformation cosmeticInformation)
        {
            try
            {
                using (var context = new CosmeticsDbContext())
                {
                    var existingCosmetic = await context.CosmeticInformations
                        .Include(c => c.Category)
                        .FirstOrDefaultAsync(c => c.CosmeticId == cosmeticInformation.CosmeticId);
                    if (existingCosmetic == null)
                    {
                        throw new Exception("Cosmetic information not found.");
                    }

                    var cate = await context.CosmeticCategories.FirstOrDefaultAsync(c => c.CategoryId.Equals(cosmeticInformation.CategoryId));
                    if (cate == null)
                    {
                        throw new Exception("Cate not found");
                    }

                    existingCosmetic.CosmeticName = cosmeticInformation.CosmeticName;
                    existingCosmetic.SkinType = cosmeticInformation.SkinType;
                    existingCosmetic.ExpirationDate = cosmeticInformation.ExpirationDate;
                    existingCosmetic.CosmeticSize = cosmeticInformation.CosmeticSize;
                    existingCosmetic.DollarPrice = cosmeticInformation.DollarPrice;
                    existingCosmetic.CategoryId = cosmeticInformation.CategoryId;
                    await context.SaveChangesAsync();
                    return existingCosmetic;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating cosmetic information.", ex);
            }
        }

        public async Task<CosmeticInformation> Delete(string id)
        {
            try
            {
                using (var context = new CosmeticsDbContext())
                {
                    var cosmeticInformation = await context.CosmeticInformations
                        .Include(c => c.Category)
                        .FirstOrDefaultAsync(c => c.CosmeticId.Equals(id));
                    if (cosmeticInformation == null)
                    {
                        throw new Exception("Cosmetic information not found.");
                    }
                    context.CosmeticInformations.Remove(cosmeticInformation);
                    await context.SaveChangesAsync();
                    return cosmeticInformation;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting cosmetic information.", ex);
            }
        }
        private string GenerateId()
        {
            var random = new Random();
            var id = random.Next(100000, 999999);
            return "PL" + id.ToString();
        }

    }
}
