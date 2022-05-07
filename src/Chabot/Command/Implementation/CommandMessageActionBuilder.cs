using System.Linq.Expressions;
using System.Reflection;
using Chabot.Message;
using Chabot.State;
using Chabot.User;

namespace Chabot.Command.Implementation;

public class CommandMessageActionBuilder : ICommandMessageActionBuilder
{
    public Func<CommandGroupBase<TMessage, TUser, TUserId>, IState?, Task> BuildInvokeCommand
        <TMessage, TUser, TUserId>(Type type, MethodInfo method, Type? stateType) 
        where TMessage : IMessage
        where TUser : IUser<TUserId>
    {
        var instanceParameter = Expression.Parameter(typeof(CommandGroupBase<TMessage, TUser, TUserId>), "instance");
        var convertToConcreteInstance = Expression.Convert(instanceParameter, type);

        var stateParameter = Expression.Parameter(typeof(IState), "state");

        var variables = new List<ParameterExpression>();
        var expressions = new List<Expression>();

        foreach (var parameterInfo in method.GetParameters())
        {
            var variable = Expression.Variable(parameterInfo.ParameterType, parameterInfo.Name);
            variables.Add(variable);

            expressions.Add(parameterInfo.ParameterType == stateType
                ? Expression.Assign(variable, Expression.Convert(stateParameter, stateType))
                : Expression.Assign(variable, Expression.Default(parameterInfo.ParameterType)));
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
            .Lambda<Func<CommandGroupBase<TMessage, TUser, TUserId>, IState?, Task>>(block, instanceParameter, stateParameter)
            .Compile();
    }
}