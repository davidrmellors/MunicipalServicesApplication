using MunicipalServices.Core.Services;
using MunicipalServices.Models;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
    /// <summary>
    /// Abstract base class for tree data structures that store service requests.
    /// Provides a common interface for different tree implementations like AVL or Red-Black trees.
    /// </summary>
    public abstract class ServiceRequestTree
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the total number of service requests stored in the tree.
        /// </summary>
        public abstract int Count { get; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts a new service request into the tree.
        /// </summary>
        /// <param name="request">The service request to insert.</param>
        public abstract void Insert(ServiceRequest request);

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Finds and returns a service request by its ID.
        /// </summary>
        /// <param name="requestId">The ID of the service request to find.</param>
        /// <returns>The matching service request if found, null otherwise.</returns>
        public abstract ServiceRequest Find(string requestId);

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes a service request from the tree by its ID.
        /// </summary>
        /// <param name="requestId">The ID of the service request to delete.</param>
        public abstract void Delete(string requestId);

//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//