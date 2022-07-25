using System;
using System.Collections.Generic;

namespace Simplic.CodeGenerator
{
    public class Component
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string ComponentType { get; set; }

        public string Name { get; set; }

        public string FileExtension { get; set; }

        public string Comment { get; set; }

        public bool IsActive { get; set; }

        public string NameSpace { get; set; }

        public ComponentConfig Config { get; set; }

        public IList<Component> Children { get; set; } = new List<Component>();

    }
}