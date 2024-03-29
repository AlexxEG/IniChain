﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;

namespace IniChain
{
    public class Ini
    {
        private const string SectionHeader = "HEADER";

        OrderedDictionary p_Sections = new OrderedDictionary();

        /// <summary>
        /// Gets the amount of properties across all sections.
        /// </summary>
        public int CountProperties
        {
            get
            {
                int count = 0;

                foreach (DictionaryEntry entry in p_Sections)
                {
                    count += (entry.Value as IniSection).Count;
                }

                return count;
            }
        }
        /// <summary>
        /// Gets the amount of sections.
        /// </summary>
        public int CountSections
        {
            get { return p_Sections.Count; }
        }
        /// <summary>
        /// Gets the filename of the current configuration file.
        /// </summary>
        public string Filename { get; private set; }
        /// <summary>
        /// Gets the always present Header section.
        /// </summary>
        public IniSection HeaderSection
        {
            get
            {
                return (IniSection)p_Sections[SectionHeader];
            }
        }
        /// <summary>
        /// Gets or sets if default value is returned when value is empty.
        /// </summary>
        [DefaultValue(false)]
        public bool ReturnDefaultIfEmpty { get; set; }
        /// <summary>
        /// Gets the sections of the configuration file with properties as read-only.
        /// </summary>
        public OrderedDictionary Sections
        {
            get { return p_Sections.AsReadOnly(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniChain.Ini"/> class. Does not load until Load method is called.
        /// </summary>
        /// <param name="filename">The complete file path to write and read configuration file.</param>
        public Ini(string filename)
        {
            this.Filename = filename;

            p_Sections.Add(SectionHeader, new IniSection(SectionHeader));
        }

        /// <summary>
        /// Backs up the configuration file.
        /// </summary>
        /// <param name="file">Path to save backup file to.</param>
        /// <param name="applyChanges">True if changes should be written, false to just copy the configuration file.</param>
        public void Backup(string file, bool applyChanges)
        {
            if (applyChanges)
            {
                this.Save(file);
            }
            else
            {
                if (!File.Exists(this.Filename))
                    throw new FileNotFoundException("INI file not found", this.Filename);

                File.Copy(this.Filename, file);
            }
        }

        /// <summary>
        /// Determine whether a section exists.
        /// </summary>
        /// <param name="section">The section to find.</param>
        public bool Contains(string section)
        {
            return this.p_Sections.Contains(section);
        }

        /// <summary>
        /// Determine whether a property within a given section exists.
        /// </summary>
        /// <param name="section">The section to find.</param>
        /// <param name="key">The key of the property to find.</param>
        public bool Contains(string section, string key)
        {
            return this.GetSection(section).Contains(key);
        }

        /// <summary>
        /// Deletes property from section in configuration file.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        public void Delete(string section, string key)
        {
            IniSection properties = this.GetSection(section);

            if (properties == null)
                return;

            // If the key is the last property, might as well delete section. 
            if (properties.Contains(key) && properties.Count == 1)
            {
                p_Sections.Remove(section);
            }
            else
            {
                properties.Remove(key);
            }
        }

        /// <summary>
        /// Deletes section in configuration file.
        /// </summary>
        /// <param name="section">The section to delete.</param>
        public void DeleteSection(string section)
        {
            if (p_Sections.Contains(section))
            {
                p_Sections.Remove(section);
            }
        }

        /// <summary>
        /// Returns the property object, or null if it couldn't be found.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        public IniProperty Get(string section, string key)
        {
            return (p_Sections[section] as IniSection).Get(key);
        }

        /// <summary>
        /// Returns the property value.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        /// <param name="defaultValue">The value to return if the property couldn't be found.</param>
        public object GetValue(string section, string key, object defaultValue)
        {
            if (!p_Sections.Contains(section) ||
                !this.GetSection(section).Contains(key))
            {
                return defaultValue;
            }

            string value = this.GetSection(section).Get(key).Value;

            return this.ReturnDefaultIfEmpty && string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>
        /// Returns the property boolean value.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        /// <param name="defaultValue">The value to return if the property couldn't be found.</param>
        public bool GetBoolean(string section, string key, bool defaultValue)
        {
            return bool.Parse(this.GetValue(section, key, defaultValue).ToString());
        }

        /// <summary>
        /// Returns the property integer value.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        /// <param name="defaultValue">The value to return if the property couldn't be found.</param>
        public int GetInteger(string section, string key, int defaultValue)
        {
            return int.Parse(this.GetValue(section, key, defaultValue).ToString());
        }

        /// <summary>
        /// Returns the property long value.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        /// <param name="defaultValue">The value to return if the property couldn't be found.</param>
        public long GetLong(string section, string key, long defaultValue)
        {
            return long.Parse(this.GetValue(section, key, defaultValue).ToString());
        }

        /// <summary>
        /// Returns the properties from the given section.
        /// </summary>
        /// <param name="section">Section to return properties from.</param>
        public IniSection GetSection(string section)
        {
            if (!p_Sections.Contains(section))
                p_Sections.Add(section, new IniSection(section));

            return (IniSection)p_Sections[section];
        }

        /// <summary>
        /// Returns a read-only list of all section names.
        /// </summary>
        public ICollection<string> GetSectionNames()
        {
            string[] sections = new string[this.p_Sections.Count];

            this.p_Sections.Keys.CopyTo(sections, 0);

            return Array.AsReadOnly<string>(sections);
        }

        /// <summary>
        /// Returns a read-only list of all sections.
        /// </summary>
        public ICollection<IniSection> GetSections()
        {
            IniSection[] sections = new IniSection[this.p_Sections.Count];

            this.p_Sections.Values.CopyTo(sections, 0);

            return Array.AsReadOnly<IniSection>(sections);
        }

        /// <summary>
        /// Returns the property string value.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        /// <param name="defaultValue">The value to return if the property couldn't be found.</param>
        public string GetString(string section, string key, string defaultValue)
        {
            return this.GetValue(section, key, defaultValue).ToString();
        }

        /// <summary>
        /// Loads the properties from the configuration file.
        /// </summary>
        public void Load()
        {
            if (!File.Exists(this.Filename))
                return;

            this.p_Sections.Clear();

            using (var reader = new StreamReader(this.Filename))
            {
                string key = string.Empty;
                string line;
                int lineCount = 0;
                IniSection section = this.GetSection(SectionHeader);

                while ((line = reader.ReadLine()) != null)
                {
                    lineCount++;
                    line = line.Trim();

                    // Empty line. 
                    if (string.IsNullOrEmpty(line))
                    {
                        key = IniType.EmptyLine.ToString() + lineCount;

                        // Use key above if available, otherwise generate random
                        if (section.Contains(key))
                            section.Add(new IniProperty(section.Name, IniType.EmptyLine, line));
                        else
                            section.Add(new IniProperty(section.Name, IniType.EmptyLine, key, line));
                    }
                    else
                    {
                        char firstChar = char.Parse(line.Substring(0, 1));

                        switch (firstChar)
                        {
                            case ';':
                            case '#': // Comment line
                                key = IniType.Comment.ToString() + lineCount;

                                // Use key above if available, otherwise generate random
                                if (section.Contains(key))
                                    section.Add(new IniProperty(section.Name, IniType.Comment, line));
                                else
                                    section.Add(new IniProperty(section.Name, IniType.Comment, key, line));

                                break;
                            case '[': // Section line
                                if (!line.EndsWith("]"))
                                    // Line doesn't end with a closing bracket. 
                                    goto default;
                                else
                                {
                                    int start = line.IndexOf('[') + 1;
                                    int length = line.IndexOf(']') - line.IndexOf('[') - 1;

                                    // Trim white space from section name
                                    string name = line.Substring(start, length).Trim();

                                    if (p_Sections.Contains(name))
                                    {
                                        section = (IniSection)p_Sections[name];
                                    }
                                    else
                                    {
                                        section = new IniSection(name);

                                        p_Sections.Add(name, section);
                                    }
                                }
                                break;
                            default:
                                // Valid property line
                                if (line.Contains("=") && section.Name != SectionHeader)
                                {
                                    string[] split = line.Split('=');

                                    // Trim key name and value
                                    string value = split[1].Trim();
                                    key = split[0].Trim();

                                    if (section.Contains(split[0]))
                                        throw new Exception(string.Format("Section '{0}' already contains property '{1}'. Value: {2}", section.Name, key, value));

                                    section.Add(new IniProperty(section.Name, key, value));
                                }
                                else // Invalid line
                                {
                                    key = IniType.Invalid.ToString() + lineCount;

                                    // Use key above if available, otherwise generate random
                                    if (section.Contains(key))
                                        section.Add(new IniProperty(section.Name, IniType.Invalid, line));
                                    else
                                        section.Add(new IniProperty(section.Name, IniType.Invalid, key, line));
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds or sets the value of the property.
        /// </summary>
        /// <param name="section">The section of the property.</param>
        /// <param name="key">The key of the property.</param>
        /// <param name="value">The property value.</param>
        public void Put(string section, string key, string value)
        {
            IniSection sect = null;

            if (p_Sections.Contains(section))
            {
                sect = this.GetSection(section);
            }
            else
            {
                if (p_Sections.Count > 0)
                {
                    IniSection lastSect = (IniSection)p_Sections[p_Sections.Count - 1];

                    // Insert empy line? 
                    if (!string.IsNullOrEmpty(lastSect[lastSect.Count - 1].Value))
                    {
                        lastSect.Add(new IniProperty(section, IniType.EmptyLine, ""));
                    }
                }

                sect = new IniSection(section);

                p_Sections.Add(section, sect);
            }

            if (sect.Contains(key))
            {
                sect.Get(key).Value = value;
            }
            else
            {
                sect.Add(new IniProperty(section, key, value));
            }
        }

        /// <summary>
        /// Saves the properties to the configuration file.
        /// </summary>
        public void Save()
        {
            this.Save(this.Filename);
        }

        /// <summary>
        /// Saves the properties to the given file.
        /// </summary>
        /// <param name="file">The file to save the properties to.</param>
        private void Save(string file)
        {
            using (var writer = new StreamWriter(file))
            {
                foreach (DictionaryEntry pair in p_Sections)
                {
                    if ((string)pair.Key != SectionHeader)
                        writer.WriteLine("[{0}]", pair.Key);

                    foreach (IniProperty property in (pair.Value as IniSection).GetAll())
                    {
                        // Respect comment/empty/invalid lines. 
                        if (property.Type != IniType.Property)
                        {
                            // Automatically add the comment character if there is none for comments
                            if (property.Type == IniType.Comment)
                            {
                                if (!property.Value.StartsWith(";") && !property.Value.StartsWith("#"))
                                {
                                    property.Value = "; " + property.Value;
                                }
                            }

                            writer.WriteLine(property.Value);
                        }
                        else
                        {
                            writer.WriteLine("{0}={1}", property.Key, property.Value);
                        }
                    }
                }
            }
        }
    }
}
