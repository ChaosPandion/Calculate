using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core.Expressions
{
    public interface IExpressionVisitor
    {
        void Visit(ConstantExpression e);
        void Visit(UnaryExpression e);
        void Visit(BinaryExpression e);
    }
}