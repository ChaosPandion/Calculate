﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core.Expressions
{
    [DebuggerStepThrough]
    public class ConstantExpression : Expression
    {
        private readonly object _value;

        public ConstantExpression(object value)
        {
            _value = value;
        }

        public object Value
        {
            get { return _value; }
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return (Value ?? "").ToString();
        }
    }
}