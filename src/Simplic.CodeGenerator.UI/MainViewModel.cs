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
            string folderName = Savepath + "\\" + component.Namespace;
            GenerateFolder(folderName);

            foreach (var child in component.Children)
            {
                if (child.Children.Any())
                    GenerateCode(child);

                if (child.Config.Ending == null)
                    continue;

                var path = folderName + "\\" + child.Name + child.Config.Ending;
                var generateText = ReplaceText(child);
                File.WriteAllText(path, generateText);
            }
        }

        private void GenerateFolder(string folderName)
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
        }

        private string GetTemplatePath(Component component)
        {
            string templatePath = "";
            if (component.Config.Ending == ".cs")
                return templatePath = $"..\\..\\..\\Simplic.CodeGenerator\\Templates\\Classes\\{component.Config.Template}.txt";
            else
                return templatePath = $"..\\..\\..\\Simplic.CodeGenerator\\Templates\\{component.Config.Template}.txt";
        }

        private string ReplaceText(Component component)
        {
            string templatePath = GetTemplatePath(component);
            string readText = File.ReadAllText(templatePath);
            var properties = new StringBuilder();

            foreach (var childProperty in component.Properties)
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
            readText = readText.Replace("..Name..", component.Name);
            readText = readText.Replace("..Properties..", properties.ToString());
            return readText;
        }

    }
}
