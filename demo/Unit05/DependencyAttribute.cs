using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace demo.Unit05
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DependencyAttribute : Attribute
    {
    }
}
