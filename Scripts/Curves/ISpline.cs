namespace Curves {

    public interface ISpline {

        /// <summary>
        /// Returns the number of splines within the curve.
        /// </summary>
        int SplineCount { get; }
        
        /// <summary>
        /// Returns a lookup table of accumulating distances of the spline(s).
        /// </summary>
        /// <param name="segments">How many segments exist within each spline?</param>
        /// <returns>An array of floats that define the entire distance of the spline.</returns>
        float[] GetLengthTable(int segments);
    }
}
