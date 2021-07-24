using ImageProcessor.Processors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImageProcessor
{
    public abstract class Proxifier
    {
        public abstract IEnumerable<Type> GetProxyTypes();
        public abstract StackPanel GetPanel(Window parent, OptionAttribute attribute, FieldInfo opt, Processor p, Action updater);
    }

    public abstract class MonotypedProxifier<T> : Proxifier
    {
        public override IEnumerable<Type> GetProxyTypes()
        {
            return new Type[] { typeof(T) };
        }
    }

    public class SimpleProxifier<T> : MonotypedProxifier<T>
    {
        private Func<OptionAttribute, FieldInfo, Processor, Action, StackPanel> panelGenerator;

        public SimpleProxifier(Func<OptionAttribute, FieldInfo, Processor, Action, StackPanel> func)
        {
            panelGenerator = func;
        }

        public override StackPanel GetPanel(Window parent, OptionAttribute attribute, FieldInfo opt, Processor p, Action updater)
        {
            return panelGenerator(attribute, opt, p, updater);
        }
    }
}
