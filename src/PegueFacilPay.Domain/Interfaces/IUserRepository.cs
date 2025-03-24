using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PegueFacilPay.Domain.Entities;

namespace PegueFacilPay.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByDocumentAsync(string document);
        Task<IEnumerable<User>> GetAllAsync();
        Task<bool> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByDocumentAsync(string document);
    }
} 