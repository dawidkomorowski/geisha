using System;

namespace Geisha.Editor.Core.ViewModels.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DependsOnPropertyAttribute : Attribute
    {
        public string PropertyName { get; }

        public DependsOnPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}