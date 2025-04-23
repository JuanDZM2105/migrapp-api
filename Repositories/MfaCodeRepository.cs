using migrapp_api.Data;
using migrapp_api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace migrapp_api.Repositories
{
    public class MfaCodeRepository : IMfaCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public MfaCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveCodeAsync(string email, string code)
        {
            var existing = await _context.UserMfaCodes.FirstOrDefaultAsync(c => c.Email == email);
            if (existing != null) _context.UserMfaCodes.Remove(existing);

            var entity = new UserMfaCode
            {
                Email = email,
                Code = code,
                Expiration = DateTime.UtcNow.AddMinutes(5)
            };

            await _context.UserMfaCodes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            var record = await _context.UserMfaCodes.FirstOrDefaultAsync(c => c.Email == email && c.Code == code);
            return record != null && record.Expiration > DateTime.UtcNow;
        }
    }
}
