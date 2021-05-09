using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Processors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImageProcessorAttribute : Attribute
    {
        public string Name { get; private set; }
        public ImageProcessorAttribute(string name)
        {
            Name = name;
        }
    }
}
