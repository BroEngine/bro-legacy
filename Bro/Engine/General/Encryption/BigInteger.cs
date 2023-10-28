using System;

namespace Bro
{
    /* Author 2002 Chew Keong TAN, MIT License */
    public class BigInteger
    {
        private const int MaxLength = 1000;
        private readonly uint[] _data = null;
        private int _dataLength; 

        private BigInteger()
        {
            _data = new uint[MaxLength];
            _dataLength = 1;
        }

        public BigInteger(long value)
        {
            _data = new uint[MaxLength];
            var tempVal = value;

            _dataLength = 0;
            while (value != 0 && _dataLength < MaxLength)
            {
                _data[_dataLength] = (uint) (value & 0xFFFFFFFF);
                value >>= 32;
                _dataLength++;
            }

            if (tempVal > 0) // overflow check for +ve value
            {
                if (value != 0 || (_data[MaxLength - 1] & 0x80000000) != 0)
                {
                    throw (new ArithmeticException("Positive overflow in constructor."));
                }
            }
            else if (tempVal < 0) // underflow check for -ve value
            {
                if (value != -1 || (_data[_dataLength - 1] & 0x80000000) == 0)
                {
                    throw (new ArithmeticException("Negative underflow in constructor."));
                }
            }

            if (_dataLength == 0)
            {
                _dataLength = 1;
            }
        }
        
        public BigInteger(BigInteger bi)
        {
            _data = new uint[MaxLength];
            _dataLength = bi._dataLength;

            for (var i = 0; i < _dataLength; i++)
            {
                _data[i] = bi._data[i];
            }
        }

        public BigInteger(uint[] inData)
        {
            _dataLength = inData.Length;

            if (_dataLength > MaxLength)
            {
                throw (new ArithmeticException("Byte overflow in constructor."));
            }

            _data = new uint[MaxLength];

            for (int i = _dataLength - 1, j = 0; i >= 0; i--, j++)
            {
                _data[j] = inData[i];
            }

            while (_dataLength > 1 && _data[_dataLength - 1] == 0)
            {
                _dataLength--;
            }
        }


        public static implicit operator BigInteger(long value)
        {
            return (new BigInteger(value));
        }

        // ReSharper disable once FunctionRecursiveOnAllPaths
        public static implicit operator BigInteger(ulong value)
        {
            return (new BigInteger(value));
        }

        public static implicit operator BigInteger(int value)
        {
            return (new BigInteger((long) value));
        }

        public static implicit operator BigInteger(uint value)
        {
            return (new BigInteger((ulong) value));
        }
        
        public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
        {
            var result = new BigInteger
            {
                _dataLength = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength
            };

            long carry = 0;
            for (var i = 0; i < result._dataLength; i++)
            {
                var sum = (long) bi1._data[i] + (long) bi2._data[i] + carry;
                carry = sum >> 32;
                result._data[i] = (uint) (sum & 0xFFFFFFFF);
            }

            if (carry != 0 && result._dataLength < MaxLength)
            {
                result._data[result._dataLength] = (uint) (carry);
                result._dataLength++;
            }

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
            {
                result._dataLength--;
            }
            
            var lastPos = MaxLength - 1;
            if ((bi1._data[lastPos] & 0x80000000) == (bi2._data[lastPos] & 0x80000000) && (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return result;
        }
        
        public static BigInteger operator ++(BigInteger bi1)
        {
            var result = new BigInteger(bi1);

            long val, carry = 1;
            var index = 0;

            while (carry != 0 && index < MaxLength)
            {
                val = (long) (result._data[index]);
                val++;

                result._data[index] = (uint) (val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if (index > result._dataLength)
            {
                result._dataLength = index;
            }
            else
            {
                while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                {
                    result._dataLength--;
                }
            }

            // overflow check
            int lastPos = MaxLength - 1;

            // overflow if initial value was +ve but ++ caused a sign
            // change to negative.

            if ((bi1._data[lastPos] & 0x80000000) == 0 &&
                (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException("Overflow in ++."));
            }

            return result;
        }


        //***********************************************************************
        // Overloading of subtraction operator
        //***********************************************************************

        public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            result._dataLength = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

            long carryIn = 0;
            for (int i = 0; i < result._dataLength; i++)
            {
                long diff;

                diff = (long) bi1._data[i] - (long) bi2._data[i] - carryIn;
                result._data[i] = (uint) (diff & 0xFFFFFFFF);

                if (diff < 0)
                    carryIn = 1;
                else
                    carryIn = 0;
            }

            // roll over to negative
            if (carryIn != 0)
            {
                for (int i = result._dataLength; i < MaxLength; i++)
                    result._data[i] = 0xFFFFFFFF;
                result._dataLength = MaxLength;
            }

            // fixed in v1.03 to give correct datalength for a - (-b)
            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            // overflow check

            int lastPos = MaxLength - 1;
            if ((bi1._data[lastPos] & 0x80000000) != (bi2._data[lastPos] & 0x80000000) &&
                (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return result;
        }


        //***********************************************************************
        // Overloading of the unary -- operator
        //***********************************************************************

        public static BigInteger operator --(BigInteger bi1)
        {
            BigInteger result = new BigInteger(bi1);

            long val;
            bool carryIn = true;
            int index = 0;

            while (carryIn && index < MaxLength)
            {
                val = (long) (result._data[index]);
                val--;

                result._data[index] = (uint) (val & 0xFFFFFFFF);

                if (val >= 0)
                    carryIn = false;

                index++;
            }

            if (index > result._dataLength)
                result._dataLength = index;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            // overflow check
            int lastPos = MaxLength - 1;

            // overflow if initial value was -ve but -- caused a sign
            // change to positive.

            if ((bi1._data[lastPos] & 0x80000000) != 0 &&
                (result._data[lastPos] & 0x80000000) != (bi1._data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException("Underflow in --."));
            }

            return result;
        }


        //***********************************************************************
        // Overloading of multiplication operator
        //***********************************************************************

        public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
        {
            int lastPos = MaxLength - 1;
            bool bi1Neg = false, bi2Neg = false;

            // take the absolute value of the inputs
            try
            {
                if ((bi1._data[lastPos] & 0x80000000) != 0) // bi1 negative
                {
                    bi1Neg = true;
                    bi1 = -bi1;
                }

                if ((bi2._data[lastPos] & 0x80000000) != 0) // bi2 negative
                {
                    bi2Neg = true;
                    bi2 = -bi2;
                }
            }
            catch (Exception)
            {
            }

            BigInteger result = new BigInteger();

            // multiply the absolute values
            //                try
            {
                for (int i = 0; i < bi1._dataLength; i++)
                {
                    if (bi1._data[i] == 0) continue;

                    ulong mcarry = 0;
                    for (int j = 0, k = i; j < bi2._dataLength; j++, k++)
                    {
                        // k = i + j
                        ulong val = ((ulong) bi1._data[i] * (ulong) bi2._data[j]) +
                                    (ulong) result._data[k] + mcarry;

                        result._data[k] = (uint) (val & 0xFFFFFFFF);
                        mcarry = (val >> 32);
                    }

                    if (mcarry != 0)
                        result._data[i + bi2._dataLength] = (uint) mcarry;
                }
            }
            //                catch(Exception ex)
            //                {
            //                        throw(new ArithmeticException("Multiplication overflow."));
            //                }


            result._dataLength = bi1._dataLength + bi2._dataLength;
            if (result._dataLength > MaxLength)
                result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            // overflow check (result is -ve)
            if ((result._data[lastPos] & 0x80000000) != 0)
            {
                if (bi1Neg != bi2Neg && result._data[lastPos] == 0x80000000) // different sign
                {
                    // handle the special case where multiplication produces
                    // a max negative number in 2's complement.

                    if (result._dataLength == 1)
                        return result;
                    else
                    {
                        bool isMaxNeg = true;
                        for (int i = 0; i < result._dataLength - 1 && isMaxNeg; i++)
                        {
                            if (result._data[i] != 0)
                                isMaxNeg = false;
                        }

                        if (isMaxNeg)
                            return result;
                    }
                }

                throw (new ArithmeticException("Multiplication overflow."));
            }

            // if input has different signs, then result is -ve
            if (bi1Neg != bi2Neg)
                return -result;

            return result;
        }


        //***********************************************************************
        // Overloading of unary << operators
        //***********************************************************************

        public static BigInteger operator <<(BigInteger bi1, int shiftVal)
        {
            BigInteger result = new BigInteger(bi1);
            result._dataLength = shiftLeft(result._data, shiftVal);

            return result;
        }


        // least significant bits at lower part of buffer

        private static int shiftLeft(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                    shiftAmount = count;

                //Console.WriteLine("shiftAmount = {0}", shiftAmount);

                ulong carry = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    ulong val = ((ulong) buffer[i]) << shiftAmount;
                    val |= carry;

                    buffer[i] = (uint) (val & 0xFFFFFFFF);
                    carry = val >> 32;
                }

                if (carry != 0)
                {
                    if (bufLen + 1 <= buffer.Length)
                    {
                        buffer[bufLen] = (uint) carry;
                        bufLen++;
                    }
                }

                count -= shiftAmount;
            }

            return bufLen;
        }


        //***********************************************************************
        // Overloading of unary >> operators
        //***********************************************************************

        public static BigInteger operator >>(BigInteger bi1, int shiftVal)
        {
            BigInteger result = new BigInteger(bi1);
            result._dataLength = shiftRight(result._data, shiftVal);


            if ((bi1._data[MaxLength - 1] & 0x80000000) != 0) // negative
            {
                for (int i = MaxLength - 1; i >= result._dataLength; i--)
                    result._data[i] = 0xFFFFFFFF;

                uint mask = 0x80000000;
                for (int i = 0; i < 32; i++)
                {
                    if ((result._data[result._dataLength - 1] & mask) != 0)
                        break;

                    result._data[result._dataLength - 1] |= mask;
                    mask >>= 1;
                }

                result._dataLength = MaxLength;
            }

            return result;
        }


        private static int shiftRight(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int invShift = 0;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            //Console.WriteLine("bufLen = " + bufLen + " buffer.Length = " + buffer.Length);

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                    invShift = 32 - shiftAmount;
                }

                //Console.WriteLine("shiftAmount = {0}", shiftAmount);

                ulong carry = 0;
                for (int i = bufLen - 1; i >= 0; i--)
                {
                    ulong val = ((ulong) buffer[i]) >> shiftAmount;
                    val |= carry;

                    carry = ((ulong) buffer[i]) << invShift;
                    buffer[i] = (uint) (val);
                }

                count -= shiftAmount;
            }

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            return bufLen;
        }


        //***********************************************************************
        // Overloading of the NOT operator (1's complement)
        //***********************************************************************

        public static BigInteger operator ~(BigInteger bi1)
        {
            BigInteger result = new BigInteger(bi1);

            for (int i = 0; i < MaxLength; i++)
                result._data[i] = (uint) (~(bi1._data[i]));

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            return result;
        }


        //***********************************************************************
        // Overloading of the NEGATE operator (2's complement)
        //***********************************************************************

        public static BigInteger operator -(BigInteger bi1)
        {
            // handle neg of zero separately since it'll cause an overflow
            // if we proceed.

            if (bi1._dataLength == 1 && bi1._data[0] == 0)
                return (new BigInteger());

            BigInteger result = new BigInteger(bi1);

            // 1's complement
            for (int i = 0; i < MaxLength; i++)
                result._data[i] = (uint) (~(bi1._data[i]));

            // add one to result of 1's complement
            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < MaxLength)
            {
                val = (long) (result._data[index]);
                val++;

                result._data[index] = (uint) (val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if ((bi1._data[MaxLength - 1] & 0x80000000) == (result._data[MaxLength - 1] & 0x80000000))
                throw (new ArithmeticException("Overflow in negation.\n"));

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;
            return result;
        }


        //***********************************************************************
        // Overloading of equality operator
        //***********************************************************************

        public static bool operator ==(BigInteger bi1, BigInteger bi2)
        {
            if (object.ReferenceEquals(bi1, null))
            {
                return object.ReferenceEquals(bi2, null);
            }

            return bi1.Equals(bi2);
        }


        public static bool operator !=(BigInteger bi1, BigInteger bi2)
        {
            if (object.ReferenceEquals(bi1, null))
            {
                return !object.ReferenceEquals(bi2, null);
            }

            return !(bi1.Equals(bi2));
        }


        public override bool Equals(object o)
        {
            BigInteger bi = (BigInteger) o;

            if (bi == null)
            {
                return false;
            }

            if (this._dataLength != bi._dataLength)
                return false;

            for (int i = 0; i < this._dataLength; i++)
            {
                if (this._data[i] != bi._data[i])
                    return false;
            }

            return true;
        }


        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }


        //***********************************************************************
        // Overloading of inequality operator
        //***********************************************************************

        public static bool operator >(BigInteger bi1, BigInteger bi2)
        {
            int pos = MaxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1._data[pos] & 0x80000000) != 0 && (bi2._data[pos] & 0x80000000) == 0)
                return false;

            // bi1 is positive, bi2 is negative
            else if ((bi1._data[pos] & 0x80000000) == 0 && (bi2._data[pos] & 0x80000000) != 0)
                return true;

            // same sign
            int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;
            for (pos = len - 1; pos >= 0 && bi1._data[pos] == bi2._data[pos]; pos--) ;

            if (pos >= 0)
            {
                if (bi1._data[pos] > bi2._data[pos])
                    return true;
                return false;
            }

            return false;
        }


        public static bool operator <(BigInteger bi1, BigInteger bi2)
        {
            int pos = MaxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1._data[pos] & 0x80000000) != 0 && (bi2._data[pos] & 0x80000000) == 0)
                return true;

            // bi1 is positive, bi2 is negative
            else if ((bi1._data[pos] & 0x80000000) == 0 && (bi2._data[pos] & 0x80000000) != 0)
                return false;

            // same sign
            int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;
            for (pos = len - 1; pos >= 0 && bi1._data[pos] == bi2._data[pos]; pos--) ;

            if (pos >= 0)
            {
                if (bi1._data[pos] < bi2._data[pos])
                    return true;
                return false;
            }

            return false;
        }


        public static bool operator >=(BigInteger bi1, BigInteger bi2)
        {
            return (bi1 == bi2 || bi1 > bi2);
        }


        public static bool operator <=(BigInteger bi1, BigInteger bi2)
        {
            return (bi1 == bi2 || bi1 < bi2);
        }


        //***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has more than 1 digit.
        //
        // Algorithm taken from [1]
        //***********************************************************************

        private static void multiByteDivide(BigInteger bi1, BigInteger bi2,
            BigInteger outQuotient, BigInteger outRemainder)
        {
            uint[] result = new uint[MaxLength];

            int remainderLen = bi1._dataLength + 1;
            uint[] remainder = new uint[remainderLen];

            uint mask = 0x80000000;
            uint val = bi2._data[bi2._dataLength - 1];
            int shift = 0, resultPos = 0;

            while (mask != 0 && (val & mask) == 0)
            {
                shift++;
                mask >>= 1;
            }

            //Console.WriteLine("shift = {0}", shift);
            //Console.WriteLine("Before bi1 Len = {0}, bi2 Len = {1}", bi1.dataLength, bi2.dataLength);

            for (int i = 0; i < bi1._dataLength; i++)
                remainder[i] = bi1._data[i];
            shiftLeft(remainder, shift);
            bi2 = bi2 << shift;

            /*
            Console.WriteLine("bi1 Len = {0}, bi2 Len = {1}", bi1.dataLength, bi2.dataLength);
            Console.WriteLine("dividend = " + bi1 + "\ndivisor = " + bi2);
            for(int q = remainderLen - 1; q >= 0; q--)
                    Console.Write("{0:x2}", remainder[q]);
            Console.WriteLine();
            */

            int j = remainderLen - bi2._dataLength;
            int pos = remainderLen - 1;

            ulong firstDivisorByte = bi2._data[bi2._dataLength - 1];
            ulong secondDivisorByte = bi2._data[bi2._dataLength - 2];

            int divisorLen = bi2._dataLength + 1;
            uint[] dividendPart = new uint[divisorLen];

            while (j > 0)
            {
                ulong dividend = ((ulong) remainder[pos] << 32) + (ulong) remainder[pos - 1];
                //Console.WriteLine("dividend = {0}", dividend);

                ulong q_hat = dividend / firstDivisorByte;
                ulong r_hat = dividend % firstDivisorByte;

                //Console.WriteLine("q_hat = {0:X}, r_hat = {1:X}", q_hat, r_hat);

                bool done = false;
                while (!done)
                {
                    done = true;

                    if (q_hat == 0x100000000 ||
                        (q_hat * secondDivisorByte) > ((r_hat << 32) + remainder[pos - 2]))
                    {
                        q_hat--;
                        r_hat += firstDivisorByte;

                        if (r_hat < 0x100000000)
                            done = false;
                    }
                }

                for (int h = 0; h < divisorLen; h++)
                    dividendPart[h] = remainder[pos - h];

                BigInteger kk = new BigInteger(dividendPart);
                BigInteger ss = bi2 * (long) q_hat;

                //Console.WriteLine("ss before = " + ss);
                while (ss > kk)
                {
                    q_hat--;
                    ss -= bi2;
                    //Console.WriteLine(ss);
                }

                BigInteger yy = kk - ss;

                //Console.WriteLine("ss = " + ss);
                //Console.WriteLine("kk = " + kk);
                //Console.WriteLine("yy = " + yy);

                for (int h = 0; h < divisorLen; h++)
                    remainder[pos - h] = yy._data[bi2._dataLength - h];

                /*
                Console.WriteLine("dividend = ");
                for(int q = remainderLen - 1; q >= 0; q--)
                        Console.Write("{0:x2}", remainder[q]);
                Console.WriteLine("\n************ q_hat = {0:X}\n", q_hat);
                */

                result[resultPos++] = (uint) q_hat;

                pos--;
                j--;
            }

            outQuotient._dataLength = resultPos;
            int y = 0;
            for (int x = outQuotient._dataLength - 1; x >= 0; x--, y++)
                outQuotient._data[y] = result[x];
            for (; y < MaxLength; y++)
                outQuotient._data[y] = 0;

            while (outQuotient._dataLength > 1 && outQuotient._data[outQuotient._dataLength - 1] == 0)
                outQuotient._dataLength--;

            if (outQuotient._dataLength == 0)
                outQuotient._dataLength = 1;

            outRemainder._dataLength = shiftRight(remainder, shift);

            for (y = 0; y < outRemainder._dataLength; y++)
                outRemainder._data[y] = remainder[y];
            for (; y < MaxLength; y++)
                outRemainder._data[y] = 0;
        }


        //***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has only 1 digit.
        //***********************************************************************

        private static void singleByteDivide(BigInteger bi1, BigInteger bi2,
            BigInteger outQuotient, BigInteger outRemainder)
        {
            uint[] result = new uint[MaxLength];
            int resultPos = 0;

            // copy dividend to reminder
            for (int i = 0; i < MaxLength; i++)
                outRemainder._data[i] = bi1._data[i];
            outRemainder._dataLength = bi1._dataLength;

            while (outRemainder._dataLength > 1 && outRemainder._data[outRemainder._dataLength - 1] == 0)
                outRemainder._dataLength--;

            ulong divisor = (ulong) bi2._data[0];
            int pos = outRemainder._dataLength - 1;
            ulong dividend = (ulong) outRemainder._data[pos];

            //Console.WriteLine("divisor = " + divisor + " dividend = " + dividend);
            //Console.WriteLine("divisor = " + bi2 + "\ndividend = " + bi1);

            if (dividend >= divisor)
            {
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint) quotient;

                outRemainder._data[pos] = (uint) (dividend % divisor);
            }

            pos--;

            while (pos >= 0)
            {
                //Console.WriteLine(pos);

                dividend = ((ulong) outRemainder._data[pos + 1] << 32) + (ulong) outRemainder._data[pos];
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint) quotient;

                outRemainder._data[pos + 1] = 0;
                outRemainder._data[pos--] = (uint) (dividend % divisor);
                
            }

            outQuotient._dataLength = resultPos;
            int j = 0;
            for (int i = outQuotient._dataLength - 1; i >= 0; i--, j++)
                outQuotient._data[j] = result[i];
            for (; j < MaxLength; j++)
                outQuotient._data[j] = 0;

            while (outQuotient._dataLength > 1 && outQuotient._data[outQuotient._dataLength - 1] == 0)
                outQuotient._dataLength--;

            if (outQuotient._dataLength == 0)
                outQuotient._dataLength = 1;

            while (outRemainder._dataLength > 1 && outRemainder._data[outRemainder._dataLength - 1] == 0)
                outRemainder._dataLength--;
        }


        //***********************************************************************
        // Overloading of division operator
        //***********************************************************************

        public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
        {
            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger();

            int lastPos = MaxLength - 1;
            bool divisorNeg = false, dividendNeg = false;

            if ((bi1._data[lastPos] & 0x80000000) != 0) // bi1 negative
            {
                bi1 = -bi1;
                dividendNeg = true;
            }

            if ((bi2._data[lastPos] & 0x80000000) != 0) // bi2 negative
            {
                bi2 = -bi2;
                divisorNeg = true;
            }

            if (bi1 < bi2)
            {
                return quotient;
            }

            else
            {
                if (bi2._dataLength == 1)
                    singleByteDivide(bi1, bi2, quotient, remainder);
                else
                    multiByteDivide(bi1, bi2, quotient, remainder);

                if (dividendNeg != divisorNeg)
                    return -quotient;

                return quotient;
            }
        }


        //***********************************************************************
        // Overloading of modulus operator
        //***********************************************************************

        public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
        {
            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger(bi1);

            int lastPos = MaxLength - 1;
            bool dividendNeg = false;

            if ((bi1._data[lastPos] & 0x80000000) != 0) // bi1 negative
            {
                bi1 = -bi1;
                dividendNeg = true;
            }

            if ((bi2._data[lastPos] & 0x80000000) != 0) // bi2 negative
                bi2 = -bi2;

            if (bi1 < bi2)
            {
                return remainder;
            }

            else
            {
                if (bi2._dataLength == 1)
                    singleByteDivide(bi1, bi2, quotient, remainder);
                else
                    multiByteDivide(bi1, bi2, quotient, remainder);

                if (dividendNeg)
                    return -remainder;

                return remainder;
            }
        }


        //***********************************************************************
        // Overloading of bitwise AND operator
        //***********************************************************************

        public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = (uint) (bi1._data[i] & bi2._data[i]);
                result._data[i] = sum;
            }

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            return result;
        }


        //***********************************************************************
        // Overloading of bitwise OR operator
        //***********************************************************************

        public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = (uint) (bi1._data[i] | bi2._data[i]);
                result._data[i] = sum;
            }

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            return result;
        }


        //***********************************************************************
        // Overloading of bitwise XOR operator
        //***********************************************************************

        public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
        {
            BigInteger result = new BigInteger();

            int len = (bi1._dataLength > bi2._dataLength) ? bi1._dataLength : bi2._dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = (uint) (bi1._data[i] ^ bi2._data[i]);
                result._data[i] = sum;
            }

            result._dataLength = MaxLength;

            while (result._dataLength > 1 && result._data[result._dataLength - 1] == 0)
                result._dataLength--;

            return result;
        }

        //***********************************************************************
        // Returns a string representing the BigInteger in base 10.
        //***********************************************************************

        public override string ToString()
        {
            return ToString(10);
        }


        //***********************************************************************
        // Returns a string representing the BigInteger in sign-and-magnitude
        // format in the specified radix.
        //
        // Example
        // -------
        // If the value of BigInteger is -255 in base 10, then
        // ToString(16) returns "-FF"
        //
        //***********************************************************************

        public string ToString(int radix)
        {
            if (radix < 2 || radix > 36)
                throw (new ArgumentException("Radix must be >= 2 and <= 36"));

            string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = "";

            BigInteger a = this;

            bool negative = false;
            if ((a._data[MaxLength - 1] & 0x80000000) != 0)
            {
                negative = true;
                try
                {
                    a = -a;
                }
                catch (Exception)
                {
                }
            }

            BigInteger quotient = new BigInteger();
            BigInteger remainder = new BigInteger();
            BigInteger biRadix = new BigInteger(radix);

            if (a._dataLength == 1 && a._data[0] == 0)
                result = "0";
            else
            {
                while (a._dataLength > 1 || (a._dataLength == 1 && a._data[0] != 0))
                {
                    singleByteDivide(a, biRadix, quotient, remainder);

                    if (remainder._data[0] < 10)
                        result = remainder._data[0] + result;
                    else
                        result = charSet[(int) remainder._data[0] - 10] + result;

                    a = quotient;
                }

                if (negative)
                    result = "-" + result;
            }

            return result;
        }


        //***********************************************************************
        // Returns the lowest 4 bytes of the BigInteger as an int.
        //***********************************************************************

        public int IntValue()
        {
            return (int) _data[0];
        }


        //***********************************************************************
        // Returns the lowest 8 bytes of the BigInteger as a long.
        //***********************************************************************

        public long LongValue()
        {
            long val = 0;

            val = (long) _data[0];
            try
            {
                // exception if maxLength = 1
                val |= (long) _data[1] << 32;
            }
            catch (Exception)
            {
                if ((_data[0] & 0x80000000) != 0) // negative
                    val = (int) _data[0];
            }

            return val;
        }
    }
}