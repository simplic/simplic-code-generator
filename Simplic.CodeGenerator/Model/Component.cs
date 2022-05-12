using System;
using System.Collections.Generic;

namespace Simplic.CodeGenerator
{
    public abstract class Component
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public IList<string> Parent { get; set; }

        public IList<Component> Children { get; set; }
    }
}
