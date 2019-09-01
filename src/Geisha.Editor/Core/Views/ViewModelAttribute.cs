using System;

namespace Geisha.Editor.Core.Views
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelAttribute : Attribute
    {
        public Type ViewModelType { get; }

        public ViewModelAttribute(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }
    }
}