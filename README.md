A C# project to read and write INI files. Mostly developed for personal use.

Features
========
- Key value pairs stored in dictionaries divided in sections
- Attempts to not touch any lines that isn't a key value pair. For example empty lines, comments, invalid lines
- String operations together. Example:
```c#
ini.GetSection("General") // GetSection creates missing sections automatically
    .InsertComment("General settings")
    .Add("Width", 1)
    .Add("Height", 1)
    .InsertEmptyLine();
```
- Backup either local or unsaved INI. For example to backup the old INI before saving the new one