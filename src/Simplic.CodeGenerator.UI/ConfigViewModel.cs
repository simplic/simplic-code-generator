using Newtonsoft.Json;
using Simplic.UI.MVC;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Simplic.CodeGenerator.UI
{
    public class ConfigViewModel
    {
        private readonly IList<ComponentConfig> componentConfigList;
        private readonly ComponentStructure componentStructure;
        private ConfigViewModel parent;
        private string componentName;
        private ComponentConfig componentConfig;
        private ICommand addNewComponent;
        private ConfigViewModel codeGeneratorViewModel;
        private Component component;


        public ConfigViewModel(Component component, ConfigViewModel parent)
        {
            this.parent = parent;
            this.component = component;
            componentName = component.Config.Name;
            componentConfigList = new List<ComponentConfig>();
            var fileName = @"..\..\..\Simplic.CodeGenerator\Config\ComponentStructure.json";
            var jsonString = File.ReadAllText(fileName);
            componentStructure = JsonConvert.DeserializeObject<ComponentStructure>(jsonString);

            foreach(var componentStruc in componentStructure.ComponentStructureDict)
            {
                componentStruc.Value.Name = componentStruc.Key;
                if(componentStruc.Value.Parents.Contains(componentName)) 
                    componentConfigList.Add(componentStruc.Value);
            }             

            addNewComponent = new RelayCommand((e) =>
            {
                if(e == null) 
                    return;

                var newComponent = new Component() { Config = (ComponentConfig)e };
                codeGeneratorViewModel = new ConfigViewModel(newComponent, this);
                codeGeneratorViewModel.ComponentConfig = (ComponentConfig)e;
                component.Children.Add(codeGeneratorViewModel.component);
                ConfigViewModels.Add(codeGeneratorViewModel);
                
                parent = codeGeneratorViewModel.parent;
                
            });
        }

        public ObservableCollection<ConfigViewModel> ConfigViewModels { get; set; } = new ObservableCollection<ConfigViewModel>();

        public IList<ComponentConfig> ComponentConfigList => componentConfigList;

        public string ComponentName
        {
            get => this.componentName;
            set => componentName = value;
        }

        public Component Component
        {
            get => this.component;
            set => component = value;
        }

        public ComponentConfig ComponentConfig
        {
            get => this.componentConfig;
            set => this.componentConfig = value;
        }

        public ICommand AddNewComponent
        {
            get { return this.addNewComponent; }

            set => this.addNewComponent = value;
        }

        public Visibility PropertyVisible => (Component.Config.Properties.Any()) ? Visibility.Visible : Visibility.Collapsed;

        public Visibility AddButtonVisible => (ComponentConfigList.Any()) ? Visibility.Visible : Visibility.Collapsed; 
    }
}