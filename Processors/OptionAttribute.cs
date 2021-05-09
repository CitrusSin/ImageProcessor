using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Processors
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionAttribute : Attribute
    {
        public string Name { get; private set; }
        public object Maximum { get; set; }
        public object Minimum { get; set; }

        public OptionAttribute(string name)
        {
            Name = name;
        }
    }
}
