using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SaaS.Inventory.Domain.Common
{
    public interface IMustHaveTenant
    {
        Guid TenantId { get; }
    }
}
