using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Repository;

public class BugPermissionRepository(ApplicationDbContext context) : IBugPermissionRepository
{
    private readonly ApplicationDbContext _context = context;

    public IUnitOfWork UnitOfWork
    {
        get
        {
            return _context;
        }
    }

    public async Task<List<BugPermission>> GetAllAsync()
    {
        return await _context.BugPermissions.ToListAsync();
    }
}
