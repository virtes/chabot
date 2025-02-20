using System.Linq.Expressions;
using System.Reflection;

namespace Chabot.Commands;

internal class CommandActionBuilder<TUpdate> : ICommandActionBuilder<TUpdate>
{
    private readonly ICommandParameterValueResolverFactory<TUpdate>[] _valueResolverFactories;

    public CommandActionBuilder(
        IEnumerable<ICommandParameterValueResolverFactory<TUpdate>> valueResolverFactories)
    {
        _valueResolverFactories = valueResolverFactories.ToArray();
    }

    public CommandAction<TUpdate> BuildCommandAction(Type type, MethodInfo method)
    {
        var instanceParameter = Expression.Parameter(typeof(CommandsBase<TUpdate>), "instance");
        var convertToConcreteInstance = Expression.Convert(instanceParameter, type);

        var updateContextParameter = Expression.Parameter(typeof(UpdateContext<TUpdate>), "updateContext");

        var variables = new List<ParameterExpression>();
        var expressions = new List<Expression>();

        foreach (var parameterInfo in method.GetParameters())
        {
            var valueResolver = GetParameterValueResolver(parameterInfo);

            var parameterValueVariable = Expression.Variable(parameterInfo.ParameterType, parameterInfo.Name);
            variables.Add(parameterValueVariable);

            if (valueResolver is null)
            {
                var assignDefaultValue = Expression.Assign(
                    parameterValueVariable, Expression.Default(parameterInfo.ParameterType));
                expressions.Add(assignDefaultValue);

                continue;
            }

            var resolveParameterValueMethod = valueResolver
                .GetType()
                .GetMethod(nameof(ICommandParameterValueResolver<TUpdate>.ResolveParameterValue))!;

            var resolveParameterValueTask = Expression.Call(
                instance: Expression.Constant(valueResolver),
                method: resolveParameterValueMethod,
                arg0: Expression.Constant(parameterInfo),
                arg1: updateContextParameter);

            // Expression API doesn't support await
            var getAwaiterMethod = typeof(ValueTask<object?>).GetMethod("GetAwaiter")!;
            var resolveParameterValueTaskGetAwaiter = Expression.Call(resolveParameterValueTask, getAwaiterMethod);

            var getResultMethod = resolveParameterValueTaskGetAwaiter.Type.GetMethod("GetResult")!;
            var resolveParameterValue = Expression.Call(resolveParameterValueTaskGetAwaiter, getResultMethod);

            var convertObjectParameterValue = Expression.Convert(
                resolveParameterValue, parameterInfo.ParameterType);

            var assignParameterValue = Expression.Assign(parameterValueVariable, convertObjectParameterValue);
            expressions.Add(assignParameterValue);
        }

        expressions.Add(Expression.Call(convertToConcreteInstance, method, variables));

        if (method.ReturnType != typeof(Task))
        {
            var returnTarget = Expression.Label(typeof(Task));
            var returnLabel = Expression.Label(returnTarget, Expression.Constant(Task.CompletedTask));
            expressions.Add(returnLabel);
        }

        var methodBody = Expression.Block(variables, expressions);

        return Expression
            .Lambda<CommandAction<TUpdate>>(methodBody, instanceParameter, updateContextParameter)
            .Compile();
    }

    private ICommandParameterValueResolver<TUpdate>? GetParameterValueResolver(ParameterInfo parameterInfo)
    {
        foreach (var valueResolverFactory in _valueResolverFactories)
        {
            if (valueResolverFactory.TryCreate(parameterInfo, out var valueResolver))
            {
                return valueResolver;
            }
        }

        return null;
    }
}