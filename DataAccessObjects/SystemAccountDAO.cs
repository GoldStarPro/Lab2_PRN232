using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class SystemAccountDAO
    {
        private static SystemAccountDAO? instance = null;
        public static SystemAccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SystemAccountDAO();
                }
                return instance;
            }
        }

        private SystemAccountDAO()
        {
            // Private constructor to prevent instantiation
        }

        public async Task<SystemAccount> Login(string email, string password)
        {
            try
            {
                using (var context = new CosmeticsDbContext())
                {
                    var account = await context.SystemAccounts
                        .FirstOrDefaultAsync(a => a.EmailAddress == email && a.AccountPassword == password);

                    if (account == null)
                    {
                        throw new Exception("Invalid email or password.");
                    }

                    return account;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while logging in.", ex);
            }
        }
    }
}
