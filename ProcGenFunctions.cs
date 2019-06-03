// Copyright(c) 2019 Nuno Fachada
// Distributed under the MIT License(See accompanying file LICENSE or copy 
// at http://opensource.org/licenses/MIT)
using System;

namespace Funcs4ProcGen
{
    /// <summary>
    /// Static class containing some useful functions for procedural generation.
    /// </summary>
    public static class ProcGenFunctions
    {

        /// <summary>
        /// Linear function
        /// </summary>
        /// <param name="x">Input variable x</param>
        /// <param name="m">Slope</param>
        /// <param name="b">Intercept value at yy</param>
        /// <returns>The y output variable</returns>
        public static double Linear(double x, double m, double b)
        {
            return m * x + b;
        }

        /// <summary>
        /// Piecewise linear function
        /// </summary>
        /// <param name="x">Input variable x</param>
        /// <param name="m">Slope</param>
        /// <param name="b">Intercept value at yy</param>
        /// <param name="l">Value of x which separates the sloped and constant
        /// parts of the function.</param>
        /// <returns>The y output variable</returns>
        public static double PiecewiseLinear(double x, double m, double b, double l)
        {
            return x < l ? m * x + b : m * l + b;
        }

        /// <summary>
        /// Logistic function
        /// </summary>
        /// <param name="x">Input variable x</param>
        /// <param name="L">The curve's maximum value</param>
        /// <param name="x0">The x-value of the sigmoid's midpoint</param>
        /// <param name="k">The steepness of the curve</param>
        /// <returns>The y output variable</returns>
        public static double Logistic(double x, double L, double x0, double k)
        {
            return L / (1 + Math.Exp(-k * (x - x0)));
        }

        /// <summary>
        /// Logarithmic function
        /// </summary>
        /// <param name="x">Input variable x</param>
        /// <param name="a">Scale modifier</param>
        /// <param name="d">Base modifier</param>
        /// <returns>The y output variable</returns>
        public static double Log(double x, double a, double d)
        {
            return a * Math.Log(d * x);
        }
    }
}
