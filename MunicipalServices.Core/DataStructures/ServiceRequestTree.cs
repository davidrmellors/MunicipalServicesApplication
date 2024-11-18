using MunicipalServices.Core.Services;
using MunicipalServices.Models;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
    public abstract class ServiceRequestTree
    {
        public abstract int Count { get; }
        public abstract void Insert(ServiceRequest request);
        public abstract ServiceRequest Find(string requestId);
        public abstract void Delete(string requestId);
    }
}
