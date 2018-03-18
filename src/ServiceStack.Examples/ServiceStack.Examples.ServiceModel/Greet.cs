﻿using System.Runtime.Serialization;

namespace ServiceStack.Examples.ServiceModel
{
    [DataContract(Namespace = ExampleConfig.DefaultNamespace)]
    public class Greet
	{
        [DataMember]
        public string Name { get; set; }
	}

    [DataContract(Namespace = ExampleConfig.DefaultNamespace)]
    public class GreetResponse
	{
        [DataMember]
        public string Result { get; set; }
	}
}