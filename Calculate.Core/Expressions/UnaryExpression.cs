using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core.Expressions
{
    public class UnaryExpression : Expression
    {
        private readonly Expression _operand;
        private readonly UnaryOperator _operator;

        public UnaryExpression(Expression operand, UnaryOperator @operator)
        {
            _operand = operand;
            _operator = @operator;
        }

        public Expression Operand
        {
            get { return _operand; }
        }

        public UnaryOperator Operator
        {
            get { return _operator; }
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            Operand.Accept(visitor);
            visitor.Visit(this);
        }
    }
}
