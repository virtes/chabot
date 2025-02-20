using Microsoft.EntityFrameworkCore;

namespace Chabot.EntityFrameworkCore.State;

public interface IChabotStateStorageDbContext
{
    public DbSet<Entities.State> States { get; set; }
}