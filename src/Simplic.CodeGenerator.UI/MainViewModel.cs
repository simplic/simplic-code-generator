using Simplic.UI.MVC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Simplic.CodeGenerator.UI
{
    public class MainViewModel
    {
        private ConfigViewModel codeGeneratorViewModel;
        private ComponentConfig componentConfig;
        private ICommand addNewComponent;
        private ObservableCollection<ConfigViewModel> configViews;
        private string parent;
        public MainViewModel()
        {
            parent = "none";
            configViews = new ObservableCollection<ConfigViewModel>();  
            componentConfig = new ComponentConfig() { Name = "none" };
            codeGeneratorViewModel = new ConfigViewModel(componentConfig, configViews, parent);

            addNewComponent = new RelayCommand((e) =>
            {
                if (e == null)
                    return;

                componentConfig = (ComponentConfig)e;
                codeGeneratorViewModel = new ConfigViewModel(componentConfig, configViews, parent);
            });
        }

        public ConfigViewModel ConfigView => codeGeneratorViewModel;

        public string Parent => parent;

        public ICommand AddNewComponent
        {
            get { return this.addNewComponent; }

            set => this.addNewComponent = value;
        }

    }
}
