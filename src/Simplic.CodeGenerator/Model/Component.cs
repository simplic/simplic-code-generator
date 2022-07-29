using System;
using System.Collections.Generic;

namespace Simplic.CodeGenerator
{
    public class Component
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Comment { get; set; }

        public bool IsActive { get; set; }

        public string Namespace { get; set; } = "";

        public ComponentConfig Config { get; set; }

        public IList<Component> Children { get; set; } = new List<Component>();

        public IList<Property> Properties { get; set; } = new List<Property>();

    }
}