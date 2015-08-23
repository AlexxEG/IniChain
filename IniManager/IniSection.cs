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
        /// Gets the number of properties in the section.
        /// </summary>
        public int Count
        {
            get
            {
                return this.properties.Count;
            }
        }
        /// <summary>
        /// Gets or sets the section name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

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
        public IniSection Add(IniProperty property)
        {
            string key = property.Key;
            IniType type = property.Type;

            /* Generate a key for certain types, this fixes the problem 
             * with duplicate key for non-property lines. */
            switch (type)
            {
                case IniType.Comment:
                case IniType.EmptyLine:
                case IniType.Invalid:
                    // Empty key means we have to generate a random one
                    if (string.IsNullOrEmpty(key))
                        key = GenerateKey(type);

                    break;
                case IniType.Property:
                default:
                    key = property.Key;
                    break;
            }

            // Apply new key and section
            property.Key = key;
            property.Section = this.Name;

            this.properties.Add(key, property);

            return this;
        }

        /// <summary>
        /// Adds an IniProperty to the end of the properties list.
        /// </summary>
        /// <param name="key">The key of the property.</param>
        /// <param name="value">The value of the property.</param>
        public IniSection Add(string key, string value)
        {
            this.Add(new IniProperty(this.Name, key, value));

            return this;
        }

        /// <summary>
        /// Adds an IniProperty to the end of the properties list.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="value">The value of the property.</param>
        public IniSection Add(IniType type, string value)
        {
            if (type == IniType.Property)
            {
                // Properties can't have empty key. 
                throw new ArgumentException("INI property can't have empty key.");
            }

            this.Add(new IniProperty(this.Name, type, value));

            return this;
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
        /// Inserts comment at the bottom of the property list.
        /// </summary>
        /// <param name="comment">The comment text.</param>
        public IniSection InsertComment(string comment)
        {
            this.Add(new IniProperty(this.Name, IniType.Comment, "; " + comment));

            return this;
        }

        /// <summary>
        /// Inserts comment into property list at the given index.
        /// </summary>
        /// <param name="index">The index to insert comment at.</param>
        /// <param name="comment">The comment text.</param>
        public IniSection InsertComment(int index, string comment)
        {
            this.properties.Insert(index, GenerateKey(IniType.Comment), "; " + comment);

            return this;
        }

        /// <summary>
        /// Inserts a empty line at the bottom of the property list.
        /// </summary>
        public IniSection InsertEmptyLine()
        {
            this.Add(new IniProperty(this.Name, IniType.EmptyLine, string.Empty));

            return this;
        }

        /// <summary>
        /// Inserts a empty line into property list at the given index.
        /// </summary>
        /// <param name="index">The index to insert empty line at.</param>
        public IniSection InsertEmptyLine(int index)
        {
            this.properties.Insert(index, GenerateKey(IniType.EmptyLine), string.Empty);

            return this;
        }

        /// <summary>
        /// Removes the IniProperty with the given key from the properties list.
        /// </summary>
        /// <param name="key">The key of the IniProperty to remove from the properties list.</param>
        public IniSection Remove(string key)
        {
            this.properties.Remove(key);

            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the properties list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Generates and returns a random key by combining the type name + a random number.
        /// </summary>
        private string GenerateKey(IniType type)
        {
            if (type == IniType.Property)
                throw new ArgumentException("Can't generate key for properties.", "type");

            string key = string.Empty;
            int attempts = 0;

            // Keep generating a new key until it finds a unused one.
            // This shouldn't add much delay, if any, unless the INI file is absolutely MASSIVE.
            do
            {
                key = type.ToString() + random.Next(int.MaxValue);

                if ((attempts++) >= int.MaxValue)
                {
                    // There is literally no keys left
                    throw new Exception("No random key available.");
                }
            } while (this.properties.Contains(key));

            return key;
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
