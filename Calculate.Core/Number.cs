using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Calculate.Core
{
    public struct Number : IEquatable<Number>
    {
        private readonly BigInteger _mantisa;
        private readonly int _exponent;

        private Number(BigInteger mantisa, int exponent)
        {
            _mantisa = mantisa;
            _exponent = exponent;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 43;
                hash += _mantisa.GetHashCode() * 17;
                hash += _exponent.GetHashCode() * 17;
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Number && Equals((Number)obj);
        }

        public bool Equals(Number other)
        {
            return this._exponent == other._exponent 
                && this._mantisa == other._mantisa;
        }

        public override string ToString()
        {
            return _mantisa + "e" + _exponent;
        }

        public static Number Pow(Number @base, int exponent)
        {
            var tmp = (Number)1m;
            while (Math.Abs(exponent) > 100)
            {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= Math.Pow(@base, diff);
                exponent -= diff;
            }
            return tmp * Math.Pow(@base, exponent);
        }

        private static Number Add(Number left, Number right)
        {
            return left._exponent > right._exponent
                ? new Number(AlignExponent(left, right) + right._mantisa, right._exponent)
                : new Number(AlignExponent(right, left) + left._mantisa, left._exponent);
        }

        private static BigInteger AlignExponent(Number value, Number reference)
        {
            return value._mantisa * BigInteger.Pow(10, value._exponent - reference._exponent);
        }

        public static implicit operator Number(decimal value)
        {
            var mantissa = (BigInteger)value;
            var exponent = 0;
            decimal scaleFactor = 1;
            while ((decimal)mantissa != value * scaleFactor)
            {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = (BigInteger)(value * scaleFactor);
            }
            return new Number(mantissa, exponent);
        }

        public static implicit operator decimal(Number value)
        {
            return (decimal)value._mantisa * (decimal)Math.Pow(10, value._exponent);
        }

        public static implicit operator Number(double value)
        {
            var mantissa = (BigInteger)value;
            var exponent = 0;
            double scaleFactor = 1;
            while (Math.Abs(value * scaleFactor - (double)mantissa) > 0)
            {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = (BigInteger)(value * scaleFactor);
            }
            return new Number(mantissa, exponent);
        }

        public static implicit operator double(Number value)
        {
            return (double)value._mantisa * (double)Math.Pow(10, value._exponent);
        }

        public static Number operator +(Number value)
        {
            return value;
        }

        public static Number operator -(Number value)
        {
            return new Number(value._mantisa * -1, value._exponent);
        }

        public static Number operator +(Number left, Number right)
        {
            return Add(left, right);
        }

        public static Number operator -(Number left, Number right)
        {
            return Add(left, -right);
        }

        public static Number operator *(Number left, Number right)
        {
            return new Number(left._mantisa * right._mantisa, left._exponent + right._exponent);
        }

        public static Number operator /(Number dividend, Number divisor)
        {
            return new Number(dividend._mantisa / divisor._mantisa, dividend._exponent - divisor._exponent);
        }
    }
}