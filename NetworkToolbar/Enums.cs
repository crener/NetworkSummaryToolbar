namespace NetworkToolbar
{
    public enum RenderingMode
    {
        /// <summary>
        /// Direct data rendered into the control
        /// </summary>
        Direct,
        /// <summary>
        /// Render as thicker information
        /// </summary>
        Thick,
        /// <summary>
        /// Take the average network traffic over a few seconds and render that
        /// </summary>
        Average
    }
}