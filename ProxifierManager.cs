using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public class ProxifierManager
    {
        public static Dictionary<Type, Proxifier> Proxiers { get; private set; }
            = new Dictionary<Type, Proxifier>();

        public static void AddProxifier(Proxifier optProxifier)
        {
            foreach (Type type in optProxifier.GetProxyTypes())
            {
                SetProxifier(type, optProxifier);
            }
        }

        public static void SetProxifier(Type type, Proxifier optProxier)
        {
            if (Proxiers.ContainsKey(type))
            {
                Proxiers[type] = optProxier;
            }
            else
            {
                Proxiers.Add(type, optProxier);
            }
        }

        public static Proxifier GetProxifier(Type type)
        {
            if (Proxiers.ContainsKey(type))
            {
                return Proxiers[type];
            }
            else
            {
                return null;
            }
        }
    }
}
