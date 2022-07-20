using Simplic.UI.MVC;
using System.Linq;
using System.Windows.Input;

namespace Simplic.CodeGenerator.UI
{
    public class MainViewModel
    {
        private ConfigViewModel codeGeneratorViewModel;
        private ComponentConfig componentConfig;
        private ICommand addNewComponent;
        private Component component;
        public MainViewModel()
        {
            component = new Component();
           
            componentConfig = new ComponentConfig() { Name = "none" };
            component.Config = componentConfig;

            codeGeneratorViewModel = new ConfigViewModel(component, null);
            ConfigView = codeGeneratorViewModel;

            addNewComponent = new RelayCommand((e) =>
            {
                if (e == null)
                    return;

                component = new Component();
                component.Config = (ComponentConfig)e;
                if (!ConfigView.ConfigViewModels.Any())
                    ConfigView.ConfigViewModels.Add(new ConfigViewModel(component, null));
            });
        }

        public ConfigViewModel ConfigView { get; set; }

        public ICommand AddNewComponent
        {
            get { return this.addNewComponent; }

            set => this.addNewComponent = value;
        }

    }
}
