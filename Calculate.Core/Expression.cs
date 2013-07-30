using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core
{
    abstract class Expression
    {
        public abstract decimal Evaluate();

        public static Expression Parse(string input)
        {
            return Parser.Parse(input);
        }
    }

    class ConstantExpression : Expression
    {
        private readonly decimal _value;

        public ConstantExpression(decimal value)
        {
            _value = value;
        }

        public decimal Value
        {
            get { return _value; }
        }

        public override decimal Evaluate()
        {
            return _value;
        }
    }

    class UnaryExpression : Expression
    {
        private readonly Expression _operand;
        private readonly Func<decimal, decimal> _computation;

        public UnaryExpression(Expression operand, Func<decimal, decimal> computation)
        {
            _operand = operand;
            _computation = computation;
        }

        public override decimal Evaluate()
        {
            return _computation(_operand.Evaluate());
        }
    }

    class BinaryExpression : Expression
    {
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private readonly Func<decimal, decimal, decimal> _computation;

        public BinaryExpression(Expression leftOperand, Expression rightOperand, Func<decimal, decimal, decimal> computation)
        {
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
            _computation = computation;
        }

        public Expression LeftOperand
        {
            get { return _leftOperand; }
        }

        public Expression RightOperand
        {
            get { return _rightOperand; }
        }

        public override decimal Evaluate()
        {
            return _computation(_leftOperand.Evaluate(), _rightOperand.Evaluate());
        }
    }
}