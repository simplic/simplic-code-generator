using Newtonsoft.Json;
using Simplic.Framework.UI;
using Simplic.UI.MVC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Simplic.CodeGenerator.UI
{
    public class ConfigViewModel
    {
        private readonly IList<ComponentConfig> componentConfigList;
        private readonly ComponentStructure componentStructure;
        private string parent;
        private string componentName;
        private ComponentConfig componentConfig;
        private ICommand addNewComponent;
        private ConfigViewModel codeGeneratorViewModel;
        private ObservableCollection<ConfigViewModel> configViews;


        public ConfigViewModel(ComponentConfig componentConfig, ObservableCollection<ConfigViewModel> configViews)
        {
            this.configViews = configViews;
            this.parent = componentConfig.Name;
            componentName = componentConfig.Name;
            componentConfigList = new List<ComponentConfig>();
            var fileName = @"C:\Users\patyk\source\repos\simplic-code-generator\src\Simplic.CodeGenerator\Config\ComponentStructure.json";
            var jsonString = File.ReadAllText(fileName);
            componentStructure = JsonConvert.DeserializeObject<ComponentStructure>(jsonString);

            foreach(var component in componentStructure.ComponentStructureDict)
            {
                component.Value.Name = component.Key;
                if(component.Value.Parents.Contains(this.parent)) 
                    componentConfigList.Add(component.Value);
            }

            if(configViews.Count == 0 && componentName != "none")
                this.configViews.Add(this);

            addNewComponent = new RelayCommand((e) =>
            {
                if(e == null) 
                    return;

                componentConfig = (ComponentConfig)e;
                codeGeneratorViewModel = new ConfigViewModel(componentConfig, this.configViews);
                this.configViews.Add(codeGeneratorViewModel);
            });

        }
        public IList<ComponentConfig> ComponentConfigList => componentConfigList;

        public ComponentStructure ComponentStructure => componentStructure;

        public ObservableCollection<ConfigViewModel> ConfigViews => configViews;

        public string ComponentName
        {
            get => this.componentName;
            set => componentName = value;
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

    }
}