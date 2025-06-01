using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMER.Features.Interfaces
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class IgnoreToolgunGUI : Attribute
    {
        // Пустой класс, используется только для маркировки
    }
}
