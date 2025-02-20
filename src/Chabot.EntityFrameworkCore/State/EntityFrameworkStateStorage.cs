using Chabot.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chabot.EntityFrameworkCore.State;

internal class EntityFrameworkStateStorage<TDbContext> : IStateStorage<string>
    where TDbContext : DbContext, IChabotStateStorageDbContext
{
    private readonly TDbContext _dbContext;
    private readonly ILogger<EntityFrameworkStateStorage<TDbContext>> _logger;

    public EntityFrameworkStateStorage(TDbContext dbContext,
        ILogger<EntityFrameworkStateStorage<TDbContext>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async ValueTask<SerializedState<string>?> GetState(string stateKey, string stateTargetType)
    {
        var state = await _dbContext.States
            .FirstOrDefaultAsync(s => s.Key == stateKey && s.TargetType == stateTargetType);

        if (state is null)
            return null;

        return new SerializedState<string>(
            Id: state.Id,
            TypeKey: state.TypeKey,
            Value: state.Value,
            CreatedAtUtc: state.CreatedAtUtc);
    }

    public async ValueTask SetState(string stateKey, string stateTargetType,
        SerializedState<string>? serializedState)
    {
        if (serializedState is null)
        {
            var affectedRows = await _dbContext.States
                .Where(s => s.Key == stateKey && s.TargetType == stateTargetType)
                .ExecuteDeleteAsync();

            _logger.LogDebug("State {StateKey} {StateTargetType} deleted ({AffectedRows})",
                stateKey, stateTargetType, affectedRows);
        }
        else
        {
            var updateAffectedRows = await _dbContext.States
                .Where(s => s.Key == stateKey && s.TargetType == stateTargetType)
                .ExecuteUpdateAsync(c => c
                    .SetProperty(s => s.Id, serializedState.Id)
                    .SetProperty(s => s.TypeKey, serializedState.TypeKey)
                    .SetProperty(s => s.Value, serializedState.Value)
                    .SetProperty(s => s.CreatedAtUtc, serializedState.CreatedAtUtc));

            if (updateAffectedRows > 0)
            {
                _logger.LogDebug("State {StateKey} {StateTargetType} updated ({AffectedRows})",
                    stateKey, stateTargetType, updateAffectedRows);
                return;
            }

            var state = new Entities.State
            {
                Key = stateKey,
                TargetType = stateTargetType,
                Id = serializedState.Id,
                CreatedAtUtc = serializedState.CreatedAtUtc,
                TypeKey = serializedState.TypeKey,
                Value = serializedState.Value
            };
            _dbContext.States.Add(state);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("State {StateKey} {StateTargetType} added", stateKey, stateTargetType);
        }
    }
}