using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CodeGenerator
{
    public class ComponentConfig
    {
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public string Template { get; set; }

        public IList<ComponentConfig> Parents { get; set; }

        public IList<Property> properties { get; set; }

    }
}
