using Simplic.UI.MVC;
using System;
using System.Collections.Generic;
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
        private Component searchComponent;
        private StringBuilder projectInterface = new StringBuilder();
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
                var newComponent = ConfigView.ConfigViewModels.FirstOrDefault().Component;
                GenerateCode(newComponent);

                var mainPath = Savepath + "\\" + searchComponent.Namespace + "\\" + searchComponent.Name + searchComponent.Config.Ending;
                var mainProject = File.ReadAllText(mainPath);
                mainProject = mainProject.Replace("..Interfaces..", projectInterface.ToString());
                File.WriteAllText(mainPath, mainProject);
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

                (var path, var generateText) = ReplaceText(child, folderName);
                File.WriteAllText(path, generateText);
            }

            if(component.Config.Ending == ".sln")
            {
                (var path, var generateText) = ReplaceText(component, folderName);
                File.WriteAllText(path, generateText);
            }
        }

        private void GenerateFolder(string folderName)
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
        }

        private string GetTemplatePath(string template)
        {
            return Directory.GetFiles(@"..\..\..\Simplic.CodeGenerator\Templates", template+".txt", SearchOption.AllDirectories).FirstOrDefault();
        }

        private (string,string) ReplaceText(Component component, string folderName)
        {
            string templatePath = GetTemplatePath(component.Config.Template);
            string readText = File.ReadAllText(templatePath);
            var properties = new StringBuilder();
            var path = "";
            var componentFile = component.Name + component.Config.Ending;

            switch (component.Config.Ending)
            {
                case ".cs":
                    (readText, properties) = ReplaceClass(component, readText);
                    path = folderName + "\\" + componentFile;
                    break;

                case ".xaml.cs":
                    (readText, properties) = ReplaceClass(component, readText);
                    path = folderName + "\\" + component.Name + component.Config.Ending;
                    break;

                case ".csproj":
                    (readText, properties) = ReplaceProject(component, readText);
                    path = folderName + "\\" + component.Name + "\\" + componentFile;
                    break;

                case ".sln":
                    (readText, properties) = ReplaceSolution(component, readText);
                    path = folderName + "\\" + componentFile;
                    break;

            }
            readText = readText.Replace("..Guid..", component.Guid.ToString());
            readText = readText.Replace("..Name..", component.Name);
            readText = readText.Replace("..Properties..", properties.ToString());
            return (path, readText);
        }

        private (string, StringBuilder) ReplaceClass(Component component, string readText)
        {
            var properties = new StringBuilder();
            var relatedClasses = new List<string>();
            SearchComponent("ProjectMain", null);

            if(component.Config.RelatedClass != null)
            {
                foreach (var relatedClass in component.Config.RelatedClass)
                    relatedClasses.Add(File.ReadAllText(GetTemplatePath(relatedClass.Template)));
            }
            
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
                {
                    readText = readText.Replace(textHolder, childProperty.Value);
                    if (!relatedClasses.Any())
                        continue;

                    for (int i = 0; i < relatedClasses.Count(); i++)
                    {
                        //Interface von UI falscher Namespace noch fixen!.
                        if (textHolder == "..NamespaceName..")
                            continue;

                        relatedClasses[i] = relatedClasses[i].Replace(textHolder, childProperty.Value);
                    }
                        
                }    
            }
            if(!relatedClasses.Any())
                return (readText, properties);

            var j = 0;
            var interfaceFolder = Savepath + "\\" + searchComponent.Namespace + "\\Interface";
            GenerateFolder(interfaceFolder);
            foreach (var relatedClass in component.Config.RelatedClass)
            {
                relatedClasses[j] = relatedClasses[j].Replace("..Guid..", component.Guid.ToString());
                relatedClasses[j] = relatedClasses[j].Replace("..Name..", component.Name);
                var path = "";
                if(relatedClass.Name == "Interface")
                {
                    relatedClasses[j] = relatedClasses[j].Replace("..NamespaceName..", searchComponent.Name);
                    var fileName = relatedClass.Prefix + component.Name + relatedClass.Ending;
                    path = interfaceFolder + "\\" + fileName;
                    projectInterface.Append(' ', 4).Append($"<Compile Include=\"Interface\\{fileName}\" />").AppendLine();
                }
                else
                {
                    var interfaceNamespace = component.Properties.Where(x => x.Name == "NamespaceName").FirstOrDefault().Value;
                    relatedClasses[j] = relatedClasses[j].Replace("..NamespaceName..", interfaceNamespace);
                    path = Savepath + "\\" + component.Namespace + "\\" + relatedClass.Prefix + component.Name + relatedClass.Ending;
                }
                    

                File.WriteAllText(path, relatedClasses[j]);
                j++;
            }
            
            return (readText, properties);
        }

        private (string, StringBuilder) ReplaceProject(Component component, string readText)
        {
            var properties = new StringBuilder();

            foreach(var child in component.Children)
            {
                if (child.Config.RelatedClass != null && child.Config.RelatedClass.Where(x=> x.Name == "WindowInterface").Any())
                {
                    readText = readText.Replace("..Window..", child.Name + ".xaml");
                    foreach (var child2 in child.Config.RelatedClass.Where(x => x.Prefix == "I"))
                        properties.Append(' ', 4).Append($"<Compile Include=\"I{child.Name}{child2.Ending}\" />");
                }
                else
                {
                    properties.Append(' ', 4).Append($"<Compile Include=\"{child.Name}{child.Config.Ending}\" />");

                    if (component.Children.Count > 1)
                        properties.AppendLine();
                }  
                readText = ReplaceProjectReference(child, readText);
            }
            CreateAssembly(component);

            return (readText, properties);
        }

        private (string, StringBuilder) ReplaceSolution(Component component, string readText)
        {
            var properties = new StringBuilder();
            var globalSection = new StringBuilder();
            var projectSection = new StringBuilder();

            foreach(var child in component.Children)
            {
                properties.Append($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{child.Name}\", \"{child.Name}" +
                $"\\{child.Name}{child.Config.Ending}\", \"{{{child.Guid}}}\"").AppendLine("EndProject").AppendLine();

                globalSection.Append(' ', 8).Append($"{{{child.Guid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU").AppendLine();
                globalSection.Append(' ', 8).Append($"{{{child.Guid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU").AppendLine();
                globalSection.Append(' ', 8).Append($"{{{child.Guid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU").AppendLine();
                globalSection.Append(' ', 8).Append($"{{{child.Guid}}}.Release|Any CPU.Build.0 = Release|Any CPU").AppendLine();

                if(child.Config.Ending != ".csproj")
                {
                    //ENV Folder etc.
                }
            }

            readText = readText.Replace("..GlobalSection..", globalSection.ToString());

            return (readText, properties);
        }

        private string ReplaceProjectReference(Component component, string readText)
        {
            string[] projects = { "Model", "Repository", "Service" };
            foreach (var project in projects)
            {
                var property = component.Properties.Where(x => x.Name == project).FirstOrDefault();
                if (property == null)
                    continue;

                var searchItem = property.Value;

                SearchComponent(searchItem, null);
                if (searchComponent == null)
                    continue;

                var path = searchComponent.Name + $"\\{searchComponent.Name}{searchComponent.Config.Ending}";
                readText = readText.Replace($"..{project}Path..", path);
                readText = readText.Replace($"..{project}Guid..", searchComponent.Guid.ToString());
                readText = readText.Replace($"..{project}Name..", searchComponent.Name);
            }
            
            return readText;
        }

        private void SearchComponent(string searchItem, Component component)
        {
            var child = component;
            if (component == null)
                child = ConfigView.ConfigViewModels.FirstOrDefault().Component;

            if (child.Config.Name == searchItem)
                searchComponent = child;

            foreach(var child2 in child.Children)
            {
                if (child2.Name == searchItem)
                    searchComponent = child;

                if (child2.Config.Name == searchItem)
                    searchComponent = child2;

                if (child2.Children.Any())
                    SearchComponent(searchItem, child2);
            }

        }

        private void CreateAssembly(Component component)
        {
            var assembly = File.ReadAllText(GetTemplatePath("AssemblyInfo"));
            var assemblyFolder = Savepath + "\\" + component.Namespace + "\\Properties";
            GenerateFolder(assemblyFolder);

            assembly = assembly.Replace("..Name..", component.Name);
            assembly = assembly.Replace("..Guid..", component.Guid.ToString());

            var savePath = assemblyFolder + "\\AssemblyInfo.cs";
            File.WriteAllText(savePath, assembly);
        }
    }
}
