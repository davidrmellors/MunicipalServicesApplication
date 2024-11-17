using MunicipalServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Core.DataStructures
{
    public class EmergencyNoticeNode
    {
        public EmergencyNotice Data { get; set; }
        public EmergencyNoticeNode Left { get; set; }
        public EmergencyNoticeNode Right { get; set; }
        public bool IsRed { get; set; }

        public EmergencyNoticeNode(EmergencyNotice data)
        {
            Data = data;
            IsRed = true;
        }
    }
}
