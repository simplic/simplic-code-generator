using System;
using Simplic.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CodeGenerator
{
    public interface ICodeGeneratorRepository : IRepositoryBase<Guid, CodeGenerator>
    {
    }
}
