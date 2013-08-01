using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core
{
    public sealed class Result
    {
        private readonly string _expression;
        private readonly Number _value;
        private readonly Exception _exception;

        public Result(string expression, Number value)
        {
            _expression = expression;
            _value = value;
        }

        public Result(string expression, Exception exception)
        {
            _expression = expression;
            _exception = exception;
        }

        public string Expression
        {
            get { return _expression; }
        }

        public Number Value
        {
            get { return _value; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}