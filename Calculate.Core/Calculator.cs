using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculate.Core.Expressions;

namespace Calculate.Core
{
    public sealed class Calculator
    {
        private readonly History _history = new History();

        public Calculator()
        {

        }

        public History History
        {
            get { return _history; }
        }

        public Result Calculate(string input)
        {
            Result r = null;
            try
            {
                var e = Parser.Parse(input);
                r = new Result(input, Evaluator.Run(e));
            }
            catch (Exception ex)
            {
                r = new Result(input, ex);
            }
            _history.Add(r);
            return r;
        }
    }

    public sealed class History : ObservableCollection<Result>
    {

    }
}