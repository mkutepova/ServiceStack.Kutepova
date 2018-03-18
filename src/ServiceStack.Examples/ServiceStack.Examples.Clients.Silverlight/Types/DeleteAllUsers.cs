﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ServiceStack.Examples.ServiceInterface.Types
{
	/// <summary>
	/// Use Plain old DataContract's Define your 'Service Interface'
	/// </summary>
	[DataContract]
	public class DeleteAllUsers
	{
	}

	[DataContract]
	public class DeleteAllUsersResponse
	{
		public DeleteAllUsersResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

		[DataMember]
		public long UserId { get; set; }

		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}
}