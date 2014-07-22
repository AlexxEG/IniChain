using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Ini
{
    public class IniSection : IEnumerable
    {
        Random random = new Random();
        /* Store properties in a OrderedDictionary to keep the order, and faster lookup.
         * Key is string type, Value is IniProperty type. */
        OrderedDictionary properties = new OrderedDictionary();

        /// <summary>
        /// Gets or sets the section name.
        /// </summary>
        public string Name { get; set; }

        public IniProperty this[int index]
        {
            get { return (IniProperty)properties[index]; }
        }

        /// <summary>
        /// Initializes a new instance of the System.Ini.IniSection class.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        public IniSection(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Adds an IniProperty to the end of the properties list.
        /// </summary>
        /// <param name="property">The IniProperty to be added to the end of the properties list.</param>
        public void Add(IniProperty property)
        {
            string key = "";
            IniType type = property.Type;
            int attempts = 0;

            /* Generate a key for certain types, this fixes the problem 
             * with duplicate key for non-property lines. */
            switch (type)
            {
                case IniType.Comment:
                case IniType.EmptyLine:
                case IniType.Invalid:
                    /* Keep generating a new key until it finds a unused one.
                     * This shouldn't add much delay, if any, unless the INI file is absolutely MASSIVE. */
                    do
                    {
                        key = type.ToString() + random.Next(int.MaxValue);

                        if ((attempts++) >= int.MaxValue)
                        {
                            /* There's literally no keys left. */
                            throw new Exception("No random keys left.");
                        }
                    }
                    while (this.properties.Contains(key));
                    break;
                case IniType.Property:
                default:
                    key = property.Key;
                    break;
            }

            this.properties.Add(key, property);
        }

        /// <summary>
        /// Adds an IniProperty to the end of the properties list.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void Add(string section, string key, string value)
        {
            this.Add(new IniProperty(section, key, value));
        }

        /// <summary>
        /// Adds an IniProperty to the end of the properties list.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void Add(string section, IniType type, string value)
        {
            if (type == IniType.Property)
            {
                /* Properties can't have empty key. */
                throw new ArgumentException("INI property can't have empty key.");
            }

            this.Add(new IniProperty(section, type, value));
        }

        /// <summary>
        /// Determine whether a property with the given key exists.
        /// </summary>
        /// <param name="key">The key of the property to find.</param>
        public bool Contains(string key)
        {
            return this.properties.Contains(key);
        }

        /// <summary>
        /// Gets the number of properties in the section.
        /// </summary>
        public int Count { get { return this.properties.Count; } }

        /// <summary>
        /// Returns the IniProperty with the given key.
        /// </summary>
        /// <param name="key">The key of the property to find.</param>
        public IniProperty Get(string key)
        {
            return (IniProperty)this.properties[key];
        }

        /// <summary>
        /// Returns a read-only collection of properties.
        /// </summary>
        public ICollection<IniProperty> GetAll()
        {
            IniProperty[] properties = new IniProperty[this.properties.Count];

            this.properties.Values.CopyTo(properties, 0);

            return Array.AsReadOnly<IniProperty>(properties);
        }

        /// <summary>
        /// Removes the IniProperty with the given key from the properties list.
        /// </summary>
        /// <param name="key">The key of the IniProperty to remove from the properties list.</param>
        public void Remove(string key)
        {
            this.properties.Remove(key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the properties list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        private class Enumerator : IEnumerator
        {
            private int position = -1;
            private IniSection l;

            public Enumerator(IniSection l)
            {
                this.l = l;
            }

            public bool MoveNext()
            {
                if (position < l.properties.Count - 1)
                {
                    position++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                position = -1;
            }

            public object Current
            {
                get
                {
                    return l.properties[position];
                }
            }
        }
    }
}
