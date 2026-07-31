using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoSaaS.Application.Common.Exceptions
{
     public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"La entidad \"{name}\" con el identificador ({key}) no fue encontrada.")
        {
        }
    }

}