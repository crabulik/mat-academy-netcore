using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatOrderingService.Services.Generator.Impl
{
    public class GuidGenerator : IGuidGenerator
    {
        private Guid Id = Guid.NewGuid();

        public Guid Generate()
        {
            return Id;
        }
    }
}
