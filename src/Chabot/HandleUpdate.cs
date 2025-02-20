namespace Chabot;

public delegate Task HandleUpdate<TUpdate>(UpdateContext<TUpdate> context);