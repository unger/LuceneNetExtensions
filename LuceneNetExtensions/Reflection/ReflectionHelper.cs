namespace LuceneNetExtensions.Reflection
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class ReflectionHelper
    {
        public static PropertyInfo GetPropertyInfo<TMapping, TReturn>(Expression<Func<TMapping, TReturn>> expression)
        {
            return GetPropertyInfo(expression.Body);
        }

        private static PropertyInfo GetPropertyInfo(Expression expression)
        {
            var memberExpression = GetMemberExpression(expression);

            if (memberExpression.Member.MemberType == MemberTypes.Property)
            {
                return memberExpression.Member as PropertyInfo;
            }

            return null;
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            MemberExpression memberExpression = null;
            if (expression.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", "expression");
            }

            return memberExpression;
        }
    }
}
