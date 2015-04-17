using System;
using System.Windows;

//<Summary> Extends double type to provide few extensions. </summary>
//<ref>http://code.msdn.microsoft.com/Taking-Advantage-of-Large-c1aa44d7</ref>
namespace AdysTech.FeatherLite.Extensions
{
  public static class MathExtensions
  {
    public const double Epsilon = 0.001;
    public static bool IsCloseEnoughTo(this double d1, double d2)
    {
      return (Math.Abs(d1 - d2) < Epsilon);
    }

    public static bool IsCloseEnoughOrSmallerThan(this double d1, double d2)
    {
      return d1 < (d2 + Epsilon);
    }

    public static double NudgeToClosestPoint(this double currentValue, int nudgeValue)
    {
      var newValue = currentValue * 10 / nudgeValue;
      newValue = Math.Floor(newValue + Epsilon);
      return newValue / 10 * nudgeValue;
    }

    public static double GetHypotenuse(this Size rect)
    {
        return Math.Sqrt (Math.Pow (rect.Width, 2) + Math.Pow (rect.Height, 2));
    }

  }
}
