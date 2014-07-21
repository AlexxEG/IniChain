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

        public string Name { get; set; }

        public IniProperty this[int index]
        {
            get { return (IniProperty)properties[index]; }
        }

        public IniSection(string name)
        {
            this.Name = name;
        }

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

        public void Add(string section, string key, string value)
        {
            this.Add(new IniProperty(section, key, value));
        }

        public void Add(string section, IniType type, string value)
        {
            if (type == IniType.Property)
            {
                /* Properties can't have empty key. */
                throw new ArgumentException("INI property can't have empty key.");
            }

            this.Add(new IniProperty(section, type, value));
        }

        public bool Contains(string key)
        {
            return this.properties.Contains(key);
        }

        public int Count { get { return this.properties.Count; } }

        public IniProperty Get(string key)
        {
            return (IniProperty)this.properties[key];
        }

        /// <summary>
        /// Returns a read-only collection of properties.
        /// </summary>
        /// <returns></returns>
        public ICollection<IniProperty> GetAll()
        {
            IniProperty[] properties = new IniProperty[this.properties.Count];

            this.properties.Values.CopyTo(properties, 0);

            return Array.AsReadOnly<IniProperty>(properties);
        }

        public void Remove(string key)
        {
            this.properties.Remove(key);
        }

        public IEnumerator GetEnumerator()
        {
            return new IniListEnumerator(this);
        }

        private class IniListEnumerator : IEnumerator
        {
            private int position = -1;
            private IniSection l;

            public IniListEnumerator(IniSection l)
            {
                this.l = l;
            }

            // The IEnumerator interface requires a MoveNext method. 
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

            // The IEnumerator interface requires a Reset method. 
            public void Reset()
            {
                position = -1;
            }

            /// The IEnumerator interface requires a Current method. 
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
