namespace System.Ini
{
    public class IniProperty
    {
        /// <summary>
        /// Gets the property key.
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// Gets the property section.
        /// </summary>
        public string Section { get; internal set; }
        /// <summary>
        /// Gets the property type.
        /// </summary>
        public IniType Type { get; private set; }
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the System.Ini.IniProperty class.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        /// <param name="value">The value of the property.</param>
        public IniProperty(string section, string key, string value)
        {
            this.Key = key;
            this.Section = section;
            this.Type = IniType.Property;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the System.Ini.IniProperty class.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="value">The value of the property.</param>
        public IniProperty(string section, IniType type, string value)
        {
            this.Key = null;
            this.Section = section;
            this.Type = type;
            this.Value = value;
        }
    }
}
