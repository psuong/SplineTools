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

        /// <summary>
        /// Returns a lookup table of accumulating distances of the spline(s).
        /// </summary>
        /// <param name="segments">How many segments exist within each spline?</param>
        /// <param name="transformData">The position, rotation, and scale which affects the splines position.</param>
        /// <returns>An array of floats that define the entire distance of the spline.</returns>
        float[] GetLengthTable(int segments, Curves.Utility.TransformData transformData);

        /// <summary>
        /// Gets the total length of the spline.
        /// </summary>
        /// <returns>Returns the total length of the spline.</returns>
        float GetTotalSplineDistance(int segments);
    }
}
