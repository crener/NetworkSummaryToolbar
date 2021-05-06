namespace NetworkToolbar
{
    public enum RenderingMode
    {
        /// <summary>
        /// Use multiple averaging methods to produce a good looking graph
        /// </summary>
        Smart,
        /// <summary>
        /// Direct data rendered into the control
        /// </summary>
        Direct,
        /// <summary>
        /// Render as thicker information
        /// </summary>
        Thick,
        /// <summary>
        /// Take the average network traffic over a few seconds
        /// </summary>
        Average,
        /// <summary>
        /// Take the moving average network traffic over a few seconds
        /// </summary>
        AverageMoving,
    }
}