using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CodeGenerator
{
    public class Property
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Value { get; set; }

        public string Comment { get; set; }

        public string Type { get; set; }
    }
}
