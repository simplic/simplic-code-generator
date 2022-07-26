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
    public class ConfigViewModel : ExtendableViewModelBase
    {
        private readonly IList<ComponentConfig> componentConfigList;
        private ComponentStructure componentStructures;
        private ConfigViewModel parent;
        private string componentConfigName;
        private ComponentConfig componentConfig;
        private ICommand addNewComponent;
        private ConfigViewModel codeGeneratorViewModel;
        private Component component;
        private string fileName;
        private string jsonString;


        public ConfigViewModel(Component component, ConfigViewModel parent)
        {
            this.parent = parent;
            this.component = component;
            componentConfigName = component.Config.Name;
            componentConfigList = new List<ComponentConfig>();

            SetComponentList();

            addNewComponent = new RelayCommand((e) =>
            {
                if (e == null)
                    return;

                var newComponentConfig = (ComponentConfig)e;

                if (newComponentConfig.Name == componentConfigName)
                    return;

                SetComponentList();

                var componentPropertys = componentConfigList.Where(x => x.Name == newComponentConfig.Name).FirstOrDefault().PropertyList;

                //Fails if adds a new parent component to the Tree in step 3.
                if (component.Namespace == "" || (component.Namespace.EndsWith("\\src") && !component.Namespace.EndsWith(component.Name)))
                    component.Namespace += (component.Config.Ending != ".sln") ? $"\\{component.Name}" : $"\\{component.Name}\\src";

                var newComponent = new Component()
                {
                    Config = newComponentConfig,
                    Properties = componentPropertys,
                    Namespace = component.Namespace
                };

                codeGeneratorViewModel = new ConfigViewModel(newComponent, this);
                codeGeneratorViewModel.ComponentConfig = newComponentConfig;
                component.Children.Add(codeGeneratorViewModel.component);
                ConfigViewModels.Add(codeGeneratorViewModel);

                parent = codeGeneratorViewModel.parent;

            });
        }

        public ObservableCollection<ConfigViewModel> ConfigViewModels { get; set; } = new ObservableCollection<ConfigViewModel>();

        public IList<ComponentConfig> ComponentConfigList => componentConfigList;

        public string ComponentName
        {
            get => this.componentConfigName;
            set => componentConfigName = value;
        }

        public Component Component
        {
            get => this.component;
            set => component = value;
        }

        public ComponentConfig ComponentConfig
        {
            get => this.componentConfig;
            set
            {
                PropertySetter(value, (v) => this.componentConfig = v);
                RaisePropertyChanged(nameof(AddButtonName));
            }
        }

        public ICommand AddNewComponentCommand
        {
            get { return this.addNewComponent; }

            set => this.addNewComponent = value;
        }


        public Visibility PropertyVisible => (Component.Config.PropertyList.Any()) ? Visibility.Visible : Visibility.Collapsed;

        public Visibility AddButtonVisible => (ComponentConfigList.Any()) ? Visibility.Visible : Visibility.Collapsed;

        public string AddButtonName => $"Add - {(ComponentConfig != null ? ComponentConfig.Name : null)}";

        private void SetComponentList()
        {
            fileName = @"..\..\..\Simplic.CodeGenerator\Config\ComponentStructure.json";
            jsonString = File.ReadAllText(fileName);
            componentStructures = JsonConvert.DeserializeObject<ComponentStructure>(jsonString);
            componentConfigList.Clear();

            foreach (var componentStructure in componentStructures.ComponentStructureDict)
            {
                componentStructure.Value.Name = componentStructure.Key;
                if (componentStructure.Value.Parents.Contains(componentConfigName))
                    componentConfigList.Add(componentStructure.Value);
            }

        }
    }
}