using MunicipalServices.Models;

namespace MunicipalServices.Core.DataStructures
{
    /// <summary>
    /// Represents a node in a Red-Black tree structure for storing emergency notices.
    /// This class is used as part of the EmergencyNoticeTree implementation.
    /// </summary>
    public class EmergencyNoticeNode
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the emergency notice data stored in this node.
        /// </summary>
        public EmergencyNotice Data { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the left child node in the Red-Black tree.
        /// </summary>
        public EmergencyNoticeNode Left { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the right child node in the Red-Black tree.
        /// </summary>
        public EmergencyNoticeNode Right { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether this node is red (true) or black (false).
        /// Used for maintaining Red-Black tree properties and balancing.
        /// </summary>
        public bool IsRed { get; set; }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the EmergencyNoticeNode class.
        /// </summary>
        /// <param name="data">The emergency notice data to store in this node.</param>
        /// <remarks>
        /// New nodes are always created as red nodes, following Red-Black tree insertion rules.
        /// The color may be changed during tree balancing operations.
        /// </remarks>
        public EmergencyNoticeNode(EmergencyNotice data)
        {
            Data = data;
            IsRed = true;
        }
//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//