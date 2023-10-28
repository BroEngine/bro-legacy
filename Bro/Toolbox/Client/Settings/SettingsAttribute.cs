using System;
namespace Bro.Toolbox.Client
{
    public class SettingsAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Path;

        public SettingsAttribute(string name, string path = "")
        {
            Name = name;
            Path = path;
        }
    }
}
