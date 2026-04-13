using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SaaS.Inventory.Application.Interfaces
{
    public interface ITenantProvider
    {
        Guid GetTenantId();
    }
}
