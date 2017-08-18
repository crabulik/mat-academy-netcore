using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatOrderingService.Services.Generator
{
    public interface IGuidGenerator
    {
        Guid Generate();
    }
}
