using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculate.Core.Expressions;

namespace Calculate.Core
{
    public class Evaluator : IExpressionVisitor
    {
        private readonly Stack<Number> _results = new Stack<Number>();

        private Evaluator()
        {

        }

        public static Number Run(Expression expression)
        {
            var evaluator = new Evaluator();
            expression.Accept(evaluator);
            return evaluator.Complete();
        }

        public Number Complete()
        {
            return _results.Pop();
        }

        public void Visit(ConstantExpression e)
        {
            _results.Push((Number)e.Value);
        }

        public void Visit(UnaryExpression e)
        {
            var o = _results.Pop();
            switch (e.Operator)
            {
                case UnaryOperator.Minus:
                    o *= (Number)(-1m);
                    break;
                case UnaryOperator.Plus:
                default:
                    break;
            }
            _results.Push(o);
        }

        public void Visit(BinaryExpression e)
        {
            var result = (Number)0m;
            var right = _results.Pop();
            var left = _results.Pop();
            switch (e.Operator)
            {
                case BinaryOperator.Addition:
                    result = left + right;
                    break;
                case BinaryOperator.Subtraction:
                    result = left - right;
                    break;
                case BinaryOperator.Multiplication:
                    result = left * right;
                    break;
                case BinaryOperator.Division:
                    result = left / right;
                    break;
                default:
                    break;
            }
            _results.Push(result);
        }
    }
}