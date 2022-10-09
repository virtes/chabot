using System.Linq.Expressions;
using System.Reflection;
using Chabot.Message;
using Chabot.User;

namespace Chabot.Command.Implementation;

public class CommandMessageActionBuilder<TMessage, TUser, TUserId>
    : ICommandMessageActionBuilder<TMessage, TUser, TUserId>
    where TMessage : IMessage
    where TUser : IUser<TUserId>
{
    private readonly IEnumerable<ICommandParameterValueResolverFactory<TMessage, TUser, TUserId>>
        _parameterValueResolverFactories;

    public CommandMessageActionBuilder(
        IEnumerable<ICommandParameterValueResolverFactory<TMessage, TUser, TUserId>>
            parameterValueResolverFactories)
    {
        _parameterValueResolverFactories = parameterValueResolverFactories;
    }

    public Func<CommandGroupBase<TMessage, TUser, TUserId>, MessageContext<TMessage, TUser, TUserId>, Task>
        BuildInvokeCommand(Type type, MethodInfo method)
    {
        var instanceParameter = Expression.Parameter(typeof(CommandGroupBase<TMessage, TUser, TUserId>), "instance");
        var convertToConcreteInstance = Expression.Convert(instanceParameter, type);

        var messageContextParameter = Expression.Parameter(typeof(MessageContext<TMessage, TUser, TUserId>), "messageContext");

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
                .GetMethod(nameof(ICommandParameterValueResolver<TMessage, TUser, TUserId>.ResolveParameterValue))!;

            var callResolveParameterValue = Expression.Call(
                Expression.Constant(valueResolver), resolveParameterValueMethod,
                Expression.Constant(parameterInfo),
                messageContextParameter);
            var convertObjectParameterValue = Expression.Convert(
                callResolveParameterValue, parameterInfo.ParameterType);

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

        var block = Expression.Block(variables, expressions);

        return Expression
            .Lambda<Func<CommandGroupBase<TMessage, TUser, TUserId>, MessageContext<TMessage, TUser, TUserId>, Task>>(
                block, instanceParameter, messageContextParameter)
            .Compile();
    }

    private ICommandParameterValueResolver<TMessage, TUser, TUserId>?
        GetParameterValueResolver(ParameterInfo parameterInfo)
    {
        foreach (var valueResolverFactory in _parameterValueResolverFactories)
        {
            var valueResolver = valueResolverFactory.CreateValueResolver(parameterInfo);
            if (valueResolver is null)
                continue;

            return valueResolver;
        }

        return null;
    }
}