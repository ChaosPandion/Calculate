using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core.Expressions
{
    [DebuggerStepThrough]
    public abstract class Expression
    {
        public abstract void Accept(IExpressionVisitor visitor);
    }
}