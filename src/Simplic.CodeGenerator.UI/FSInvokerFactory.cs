using Simplic.UIDataTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CodeGenerator.UI
{
    public class FSInvokerFactory : ITemplateLoaderFactory
    {
        public ITemplateLoader Create()
        {
            return new FileSystemTemplateLoader(@"C:\Users\patyk\source\repos\simplic-code-generator\src\Simplic.CodeGenerator.UI");
        }
    }
}
