using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CodeGenerator
{
    /// <summary>
    /// Wird immer gebraucht 
    /// </summary>
    public class CodeGenerator
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        
        public string Name { get; set; }
        
        public DateTime CreateDatetime { get; set; } = DateTime.Now;

        public byte[] Data { get; set; }

    }
}