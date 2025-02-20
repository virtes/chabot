using Chabot.EntityFrameworkCore.State;
using Chabot.State;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Chabot.EntityFrameworkCore;

public static class ChabotStateBuilderExtensions
{
    public static ChabotStateBuilder UseEntityFrameworkStorage<TDbContext>(this ChabotStateBuilder builder,
        Action<ISerializableStateStorageBuilder<string>> configureBuilder)
        where TDbContext : DbContext, IChabotStateStorageDbContext
    {
        var efStateStorageBuilder = new EntityFrameworkStateStorageBuilder(builder);
        configureBuilder(efStateStorageBuilder);

        efStateStorageBuilder.Register<TDbContext>();

        return builder;
    }
}