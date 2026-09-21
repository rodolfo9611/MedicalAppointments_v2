using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Nuget.Persistence.Common;

public static class Filter
{
    public static Expression<Func<TModel, bool>> FromStringExpression<TModel>(string query, string parameter = "x")
    {
        try
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TModel), parameter);
            return (Expression<Func<TModel, bool>>)DynamicExpressionParser.ParseLambda(new ParameterExpression[1] { parameterExpression }, null, query);
        }
        catch
        {
            throw new ValidationException("filter expression invalid");
        }
    }
}
