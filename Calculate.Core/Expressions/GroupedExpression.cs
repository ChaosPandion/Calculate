using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core.Expressions
{
    [DebuggerStepThrough]
    public class GroupedExpression : Expression
    {        
        private readonly Expression _operand;

        public GroupedExpression(Expression operand)
        {
            Debug.Assert(operand != null);
            _operand = operand;
        }

        public Expression Operand
        {
            get { return _operand; }
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            Operand.Accept(visitor);
        }

        public override string ToString()
        {
            return "(" + Operand + "}";
        }
    }
}