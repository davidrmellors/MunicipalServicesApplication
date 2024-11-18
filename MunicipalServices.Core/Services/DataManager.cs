using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServices.Core.DataStructures;
using MunicipalServices.Core.Monitoring;

namespace MunicipalServices.Core.Services
{
    public class DataManager
    {
        private static readonly object lockObject = new object();
        private static DataManager instance;

        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new DataManager();
                        }
                    }
                }
                return instance;
            }
        }

        private DataManager() { }
    }
}