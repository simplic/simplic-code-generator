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

        public string Template { get; set; }

        public IList<Component> Children { get; set; }

        public IList<Property> Properties { get; set; }
    }

    public static class TestClass { 
        public static void Test()
        {
            var solution = new Component();
            solution.Name = "simplic-furhpark";
            solution.ComponentType = "Solution";
            solution.Children = new List<Component>();

            var projectModel = new Component();
            projectModel.Name = "Simplic.Fuhrpark";
            projectModel.ComponentType = "ProjectMain";

            var modelClass = new Component();
            modelClass.Name = "Fahrzeug";
            modelClass.Properties = new List<Property>();

            var modelProperty = new Property();
            modelProperty.Name = "Guid";
            modelProperty.Value = "Guid";
            modelProperty.Comment = "Unique id";

            var modelProperty2 = new Property();
            modelProperty2.Name = "Name";
            modelProperty2.Value = "string";
            modelProperty2.Comment = "Name property";

            modelClass.Properties.Add(modelProperty);
            modelClass.Properties.Add(modelProperty2);
            projectModel.Children.Add(modelClass);
            solution.Children.Add(projectModel);



        }
    
    }

}
