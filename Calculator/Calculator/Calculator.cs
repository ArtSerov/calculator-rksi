using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    enum Operation { Add, Div, Sub, Mul, Pow}

    delegate void CalculatorDidUpdateOutput(Calculator sender, double value, int precision);

    class Calculator
    {
        double? left = 0;
        double? right = null;
        Operation? currentOp = null;
        bool decimalPoint = false;
        int precision = 0;

        public event CalculatorDidUpdateOutput DidUpdateValue;
        public event EventHandler<string> InputError;
        public event EventHandler<string> CalculationError;

        public void AddDigit(double digit)
        {
            
            if (left.HasValue && Math.Log10(left.Value) > 10)
            {
                InputError?.Invoke(this, "Input overflow");
                return;
            }

            if (!decimalPoint)
            {
                left = (left ?? 0) * 10 + digit;
            }
            else
            {
                precision += 1;
                if (precision == 1)
                {
                    left = left + 0.1 * digit;
                }
                else
                {
                    string znach1 = left.ToString()+digit.ToString();
                    var znach2 = double.Parse(znach1);
                    left = znach2;
                }

            }
            DidUpdateValue?.Invoke(this, left.Value, precision);
        }

        public void AddDecimalPoint()
        {
            decimalPoint = true;
        }

        public void AddOperation(Operation op)
        {
            if (left.HasValue && currentOp.HasValue)
            {
                Compute();
                precision = 0;
                decimalPoint = false;
            }
            if (!right.HasValue)
            {
                right = left;
                left = 0;
                precision = 0;
                decimalPoint = false;
                DidUpdateValue.Invoke(this, left.Value, precision);
            }
            currentOp = op;
        }

        public void Compute()
        {
            switch (currentOp)
            {
                case Operation.Add:
                    right = left + right;
                    left = null;
                    precision = 0;
                    decimalPoint = false;
                    break;
                case Operation.Sub:
                    right = right - left;
                    left = null;
                    precision = 0;
                    decimalPoint = false;
                    break;
                case Operation.Mul:
                    right = left * right;
                    left = null;
                    precision = 0;
                    decimalPoint = false;
                    break;
                case Operation.Div:
                    if (left == 0)
                    {
                        CalculationError?.Invoke(this, "Division by 0!");
                        return;
                    }
                    right = right / left;
                    left = null;
                    precision = 0;
                    decimalPoint = false;
                    break;
                case Operation.Pow:
                    right = Math.Pow((double)right,(double)left);
                    left = null;
                    precision = 0;
                    decimalPoint = false;
                    break;
                default:
                    currentOp = null;
                    precision = 0;
                    decimalPoint = false;
                    return;
            }

            
            DidUpdateValue?.Invoke(this, right.Value, precision);
        }

        public void Clear()
        {
            left = 0;            
        }
        public void Reset()
        {
            left = 0;
            right = 0; 
        }
        public void Sqrt()
        {
            left = Math.Sqrt((double)left);
            DidUpdateValue?.Invoke(this, left.Value, precision);
        }
        public void Interest()
        {
            left = left / 100;
            DidUpdateValue?.Invoke(this, left.Value, precision);
        }
        public void Sign()
        {
            left = left * (-1);
            DidUpdateValue?.Invoke(this, left.Value, precision);
        }
        public void Back()
        {
            
            if (left != null)
            {
                
                var znach1 = left.ToString();
                if (znach1.Length == 0) return;
                if (znach1.Length == 1)
                {
                    left = 0;
                    return;
                } 
                int z1 = znach1.Length - 1;
                znach1 = znach1.Remove(z1);
                left = double.Parse(znach1);
            }
            else
            {
                var znach1 = right.ToString();
                if (znach1.Length == 0) return;
                if (znach1.Length == 1)
                {
                    right = 0;
                    return;
                }
                int z1 = znach1.Length - 1;
                znach1 = znach1.Remove(z1);
                right = double.Parse(znach1);
            }
        }

    }
}
