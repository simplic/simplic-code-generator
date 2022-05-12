using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CodeGenerator
{
    public class ComponentMongoRepository : Component
    {
        public string FilterName { get; set; }

        public string CollectionName { get; set; }
    }
}
