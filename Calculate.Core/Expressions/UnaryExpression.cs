using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core.Expressions
{
    [DebuggerStepThrough]
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

        public override string ToString()
        {
            switch (Operator)
            {
                case UnaryOperator.Plus:
                    return "+" + Operand;
                case UnaryOperator.Minus: 
                    return "-" + Operand;
                default:
                    return "";
            }
        }
    }
}
