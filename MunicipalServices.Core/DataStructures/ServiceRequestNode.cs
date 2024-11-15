using MunicipalServices.Models;

namespace MunicipalServices.Core.DataStructures
{
    public class ServiceRequestNode
    {
        public ServiceRequest Data { get; set; }
        public ServiceRequestNode Left { get; set; }
        public ServiceRequestNode Right { get; set; }
        public int Height { get; set; }
        public bool IsRed { get; set; }

        public ServiceRequestNode(ServiceRequest data)
        {
            Data = data;
            Height = 1;
            IsRed = true;
        }
    }
}
