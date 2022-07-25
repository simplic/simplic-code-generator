using Simplic.UI.MVC;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Simplic.CodeGenerator.UI
{
    public class MainViewModel : ExtendableViewModelBase
    {
        private ConfigViewModel codeGeneratorViewModel;
        private ComponentConfig componentConfig;
        private ICommand addNewComponent;
        private ICommand selectSavepath;
        private string savePath;
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

            selectSavepath = new RelayCommand((e) =>
            {
                var folderDialog = new FolderBrowserDialog();
                folderDialog.ShowDialog();
                if (!string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    savePath = folderDialog.SelectedPath;
                    RaisePropertyChanged(nameof(Savepath));
                }
            });
        }

        public ConfigViewModel ConfigView { get; set; }

        public string Savepath
        {
            get { return savePath; }
            set => savePath = value; 
        }

        public ICommand AddNewComponent
        {
            get { return this.addNewComponent; }

            set => this.addNewComponent = value;
        }
        public ICommand SelectSavepath
        {
            get { return this.selectSavepath; }

            set => this.selectSavepath = value;
        }

    }
}
