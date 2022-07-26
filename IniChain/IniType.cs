namespace IniChain
{
    /// <summary>
    /// Specifies the type of INI line.
    /// </summary>
    public enum IniType
    {
        /// <summary>
        /// A INI comment line.
        /// </summary>
        Comment,
        /// <summary>
        /// A empty line.
        /// </summary>
        EmptyLine,
        /// <summary>
        /// A invalid INI line.
        /// </summary>
        Invalid,
        /// <summary>
        /// A INI property line.
        /// </summary>
        Property
    }
}
