using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Models
{
    public class UserRequestHistory
    {
        public string UserId { get; set; }
        public List<string> RequestIds { get; set; } = new List<string>();

        public void AddRequest(string requestId)
        {
            RequestIds.Add(requestId);
        }
    }
}
