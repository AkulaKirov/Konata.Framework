using Konata.Framework.Sdk.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework.Sdk.Extensions
{
    public interface IExtension
    {
        public string PackageName { get; }

        public bool OnLoad();
        public bool OnEnable();
        public void OnDisable();
    }
}
