using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MunicipalServices.Core.DataStructures;
using MunicipalServices.Models;

namespace MunicipalServices.Core.Services
{
    public class ServiceRequestManager
    {
        private readonly ServiceRequestBST _regularRequests;
        private readonly RedBlackTree _priorityRequests;
        private readonly ServiceRequestGraph _relatedRequests;
        private readonly ServiceStatusTree _statusTree;
        private readonly DataManager _dataManager;
        private readonly EmergencyNoticeTree _emergencyNotices = EmergencyNoticeTree.Instance;

        public ServiceRequestManager(DataManager dataManager)
        {
            _dataManager = dataManager;
            _regularRequests = new ServiceRequestBST();
            _priorityRequests = new RedBlackTree();
            _relatedRequests = new ServiceRequestGraph(dataManager);
            _statusTree = new ServiceStatusTree();
        }

        public void ProcessNewRequest(ServiceRequest request)
        {
            // Store in appropriate tree based on priority
            if (request.Priority >= 8)
            {
                _priorityRequests.Insert(request);
                
                // Create emergency notice for high-priority requests
                var notice = new EmergencyNotice
                {
                    Title = $"High Priority Request: {request.Category}",
                    Description = request.Description,
                    RelatedRequestId = request.RequestId
                };
                _emergencyNotices.Insert(notice);
            }
            else
            {
                _regularRequests.Insert(request);
            }

            // Add to graph for relationship tracking
            _relatedRequests.AddRequest(request);

            // Update service status
            UpdateServiceStatus(request);
        }

        public IEnumerable<ServiceRequest> GetRelatedRequests(string requestId)
        {
            return _relatedRequests.GetRelatedRequests(requestId, ServiceRequestGraph.TraversalType.Priority);
        }

        private void UpdateServiceStatus(ServiceRequest request)
        {
            var status = new ServiceStatus
            {
                Service = request.Category,
                Status = DetermineServiceStatus(request)
            };
            _statusTree.Insert(status);
        }

        private string DetermineServiceStatus(ServiceRequest request)
        {
            return request.Priority >= 8 ? "Critical" : "Active";
        }
    }
}
