using MunicipalServices.Models;
using System;
using System.Collections.Generic;

namespace MunicipalServices.Core.DataStructures
{
    /// <summary>
    /// Represents a singleton tree structure for managing emergency notices.
    /// Currently implemented using a List-based storage mechanism.
    /// Future updates will implement a proper Red-Black tree structure.
    /// </summary>
    public class EmergencyNoticeTree
    {
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The singleton instance of the EmergencyNoticeTree.
        /// </summary>
        private static EmergencyNoticeTree _instance;

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The internal list storing emergency notices.
        /// </summary>
        private readonly List<EmergencyNotice> _notices;
        
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the singleton instance of EmergencyNoticeTree.
        /// Creates a new instance if one does not exist.
        /// </summary>
        public static EmergencyNoticeTree Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EmergencyNoticeTree();
                }
                return _instance;
            }
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the EmergencyNoticeTree class.
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        /// <remarks>
        /// Initializes with sample emergency notices for testing purposes.
        /// </remarks>
        private EmergencyNoticeTree()
        {
            _notices = new List<EmergencyNotice>
            {
                new EmergencyNotice 
                { 
                    Title = "Planned Water Interruption",
                    Description = "Maintenance work scheduled for Central District - 15 April, 08:00-16:00",
                    Severity = "Warning"
                },
                new EmergencyNotice 
                { 
                    Title = "Power Outage Alert",
                    Description = "Emergency repairs in Progress - Northern Suburbs affected",
                    Severity = "Critical"
                }
            };
        }

//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all emergency notices stored in the tree.
        /// </summary>
        /// <returns>A list containing all emergency notices.</returns>
        public List<EmergencyNotice> GetAll() => _notices;
        
//-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts a new emergency notice into the tree.
        /// </summary>
        /// <param name="notice">The emergency notice to insert.</param>
        public void Insert(EmergencyNotice notice)
        {
            _notices.Add(notice);
        }
//-------------------------------------------------------------------------------------------------------------
    }
}
//-----------------------------------------------------END-OF-FILE-----------------------------------------------------//
