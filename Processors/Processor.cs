using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ImageProcessor.Processors
{
    public abstract class Processor
    {
        public abstract Image ProcessImage(Image source);

        public override string ToString()
        {
            var attr = GetType().GetCustomAttribute<ImageProcessorAttribute>();
            return attr.Name;
        }
    }
}
