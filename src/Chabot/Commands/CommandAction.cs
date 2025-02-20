namespace Chabot.Commands;

internal delegate Task CommandAction<TUpdate>(
    CommandsBase<TUpdate> instance,
    UpdateContext<TUpdate> updateContext);