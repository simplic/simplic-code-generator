using Simplic.UI.MVC;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
        private ICommand generateCode;
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

            generateCode = new RelayCommand((e) =>
            {
                var test = ConfigView.ConfigViewModels;
                var test2 = test.FirstOrDefault();
                var newComponent = test2.Component;
                GenerateCode(newComponent);
            });
        }

        public ConfigViewModel ConfigView { get; set; }

        public string Savepath
        {
            get { return savePath; }
            set => savePath = value; 
        }

        public ICommand AddNewComponentCommand
        {
            get { return this.addNewComponent; }

            set => this.addNewComponent = value;
        }

        public ICommand SelectSavePathCommand
        {
            get { return this.selectSavepath; }

            set => this.selectSavepath = value;
        }

        public ICommand GenerateCodeCommand
        {
            get { return this.generateCode; }

            set => this.generateCode = value;
        }

        private void GenerateCode(Component component)
        {
            foreach(var child in component.Children)
            {
                if (child.Children.Any())
                    GenerateCode(child);

                string path = "";

                if (child.Config.Ending == null)
                    continue;

                if(child.Config.Ending == ".cs")
                    path = $"..\\..\\..\\Simplic.CodeGenerator\\Templates\\Classes\\{child.Config.Template}";
                else
                    path = $"..\\..\\..\\Simplic.CodeGenerator\\Templates\\{child.Config.Template}";

                path += ".txt";
                string readText = File.ReadAllText(path);
                var properties = new StringBuilder();

                foreach (var childProperty in child.Properties)
                {
                    var textHolder = ".." + childProperty.Name + "..";
                    if (childProperty.Type == null)
                    {
                        properties.AppendLine().Append(' ', 8).AppendLine("/// <summary>");
                        properties.Append(' ', 8).AppendLine($"/// {childProperty.Comment}");
                        properties.Append(' ', 8).AppendLine("/// </summary>");
                        properties.Append(' ', 8).Append($"public {childProperty.Value} {childProperty.Name} ").AppendLine("{ get; set; }");
                    }
                    else
                        readText = readText.Replace(textHolder, childProperty.Value);

                }
                readText = readText.Replace("..Name..", child.Name);
                readText = readText.Replace("..Properties..", properties.ToString());
                var newPath = Savepath + "\\" + child.Config.Template + child.Config.Ending;
                File.WriteAllText(newPath, readText);


            }
        }

    }
}
