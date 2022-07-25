using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CodeGenerator
{
    public class ComponentConfig
    {
        public string Name { get; set; }

        public string Template { get; set; }

        public string Ending { get; set; }

        public IList<string> Parents { get; set; }

        public IList<Property> PropertyList { get; set; }

    }
}
