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
        private ComponentConfig componentConfig2;
        private ICommand addNewComponent;
        private ConfigViewModel codeGeneratorViewModel;
        private ObservableCollection<ConfigViewModel> configViews;


        public ConfigViewModel(ComponentConfig componentConfig, ObservableCollection<ConfigViewModel> configViews, string parent)
        {
            this.configViews = configViews;
            this.parent = parent;
            componentName = componentConfig.Name;
            componentConfigList = new List<ComponentConfig>();
            var fileName = @"C:\Users\patyk\source\repos\simplic-code-generator\src\Simplic.CodeGenerator\Config\ComponentStructure.json";
            var jsonString = File.ReadAllText(fileName);
            componentStructure = JsonConvert.DeserializeObject<ComponentStructure>(jsonString);
            componentConfig2 = componentConfig;

            foreach(var component in componentStructure.ComponentStructureDict)
            {
                component.Value.Name = component.Key;
                if(component.Value.Parents.Contains(componentName)) 
                    componentConfigList.Add(component.Value);
            }

            if(configViews.Count == 0 && componentName != "none")
                this.configViews.Add(this);

            addNewComponent = new RelayCommand((e) =>
            {
                if(e == null) 
                    return;

                componentConfig = (ComponentConfig)e;
                codeGeneratorViewModel = new ConfigViewModel(componentConfig, this.configViews, componentName);

                var lastParent = this.configViews.Where(x => x.parent == codeGeneratorViewModel.Parent 
                || x.componentName == codeGeneratorViewModel.Parent).LastOrDefault();

                if (lastParent == null)
                {
                    this.configViews.Add(codeGeneratorViewModel);
                    return;
                }
                    
                var newIndex = this.configViews.IndexOf(lastParent) + 1;
                this.configViews.Insert(newIndex, codeGeneratorViewModel);
            });

        }
        public IList<ComponentConfig> ComponentConfigList => componentConfigList;

        public ComponentStructure ComponentStructure => componentStructure;

        public ObservableCollection<ConfigViewModel> ConfigViews => configViews;

        public string Parent => this.parent;

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

        public ComponentConfig ComponentConfig2
        {
            get => this.componentConfig2;
            set => this.componentConfig2 = value;
        }

        public ICommand AddNewComponent
        {
            get { return this.addNewComponent; }

            set => this.addNewComponent = value;
        }

    }
}