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
        public string Section { get; private set; }
        /// <summary>
        /// Gets the property type.
        /// </summary>
        public IniType Type { get; private set; }
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public object Value { get; set; }

        public IniProperty(string section, string key, object value)
        {
            this.Key = key;
            this.Section = section;
            this.Type = IniType.Property;
            this.Value = value;
        }

        public IniProperty(string section, IniType type, string value)
        {
            this.Key = null;
            this.Section = section;
            this.Type = type;
            this.Value = value;
        }
    }
}
