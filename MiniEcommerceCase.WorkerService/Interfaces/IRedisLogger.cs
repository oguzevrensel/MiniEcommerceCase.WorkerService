using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniEcommerceCase.WorkerService.Interfaces
{
    public interface IRedisLogger
    {
        Task LogProcessedAsync(Guid orderId);
    }
}
