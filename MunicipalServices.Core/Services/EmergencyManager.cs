using MunicipalServices.Core.DataStructures;
using MunicipalServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServices.Core.Services
{
    public class EmergencyManager
    {
        private readonly EmergencyNoticeTree _noticeTree = EmergencyNoticeTree.Instance;
        private readonly ServiceRequestGraph _relatedIssues;

        public EmergencyManager(ServiceRequestGraph relatedIssues)
        {
            _relatedIssues = relatedIssues;
        }

        public void PublishEmergencyNotice(EmergencyNotice notice)
        {
            _noticeTree.Insert(notice);

            // Find related service requests
            var relatedRequests = _relatedIssues.GetRelatedRequests(
                notice.RelatedRequestId,
                ServiceRequestGraph.TraversalType.Priority
            );

            // Notify affected areas
            foreach (var request in relatedRequests.Where(r => !string.IsNullOrEmpty(r.UserId)))
            {
                NotificationService.Send(request.UserId, notice);
            }
        }
    }
}
