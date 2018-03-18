﻿using System.Runtime.Serialization;
using ServiceStack.Examples.ServiceModel.Types;

namespace ServiceStack.Examples.ServiceModel
{
    [DataContract(Namespace = ExampleConfig.DefaultNamespace)]
    public class StoreLogs
	{
        [DataMember]
        public ArrayOfLogger Loggers { get; set; }
	}

    [DataContract(Namespace = ExampleConfig.DefaultNamespace)]
    public class StoreLogsResponse
	{
		public StoreLogsResponse()
		{
			this.ResponseStatus = new ResponseStatus();

			this.ExistingLogs = new ArrayOfLogger();
		}

        [DataMember]
        public ArrayOfLogger ExistingLogs { get; set; }

        [DataMember]
        public ResponseStatus ResponseStatus { get; set; }
	}
}