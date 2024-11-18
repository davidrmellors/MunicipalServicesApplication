using MunicipalServices.Models;

namespace MunicipalServices.Core.DataStructures
{
    /// <summary>
    /// Represents a node in a self-balancing tree structure for storing service requests.
    /// Can be used in both AVL trees (using Height property) and Red-Black trees (using IsRed property).
    /// </summary>
    public class ServiceRequestNode
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The service request data stored in this node.
        /// </summary>
        public ServiceRequest Data { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Reference to the left child node.
        /// </summary>
        public ServiceRequestNode Left { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Reference to the right child node.
        /// </summary>
        public ServiceRequestNode Right { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The height of this node in an AVL tree, used for balancing.
        /// </summary>
        public int Height { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Color flag used in Red-Black tree implementation.
        /// True indicates a red node, false indicates a black node.
        /// </summary>
        public bool IsRed { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the ServiceRequestNode class.
        /// </summary>
        /// <param name="data">The service request to store in this node.</param>
        public ServiceRequestNode(ServiceRequest data)
        {
            Data = data;
            Height = 1;
            IsRed = true;
        }
//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//