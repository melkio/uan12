using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace demo.Unit04
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EnableSingletonAttribute : Attribute
    {
        public String PropertyName { get; private set; }

        public EnableSingletonAttribute(String propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
