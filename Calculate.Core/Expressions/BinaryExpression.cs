using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core.Expressions
{
    [DebuggerStepThrough]
    public class BinaryExpression : Expression
    {
        private readonly Expression _leftOperand;
        private readonly Expression _rightOperand;
        private readonly BinaryOperator _operator;

        public BinaryExpression(Expression leftOperand, Expression rightOperand, BinaryOperator @operator)
        {
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
            _operator = @operator;
        }

        public Expression LeftOperand
        {
            get { return _leftOperand; }
        }

        public Expression RightOperand
        {
            get { return _rightOperand; }
        }

        public BinaryOperator Operator
        {
            get { return _operator; }
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            LeftOperand.Accept(visitor);
            RightOperand.Accept(visitor);
            visitor.Visit(this);
        }

        public override string ToString()
        {
            switch (Operator)
            {
                case BinaryOperator.Addition:
                    return LeftOperand + "+" + RightOperand;
                case BinaryOperator.Subtraction:
                    return LeftOperand + "-" + RightOperand;
                case BinaryOperator.Multiplication:
                    return LeftOperand + "*" + RightOperand;
                case BinaryOperator.Division:
                    return LeftOperand + "/" + RightOperand;
                default:
                    return "";
            }
        }
    }
}