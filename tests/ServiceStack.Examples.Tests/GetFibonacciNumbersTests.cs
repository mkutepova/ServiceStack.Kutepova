﻿using NUnit.Framework;
using ServiceStack.Examples.ServiceInterface;
using ServiceStack.Examples.ServiceModel;
using ExampleConfig = ServiceStack.Examples.ServiceInterface.ExampleConfig;

namespace ServiceStack.Examples.Tests
{
	[TestFixture]
	public class GetFibonacciNumbersTests
		: TestHostBase
	{
		[Test]
		public void GetFibonacciNumbers_Test()
		{
			var request = new GetFibonacciNumbers { Take = 5 };

			var handler = new GetFibonacciNumbersService(
				new ExampleConfig { DefaultFibonacciLimit = 10 });

			var response = handler.Any(request);

			Assert.That(response.Results.Count, Is.EqualTo(request.Take));
			Assert.That(response.Results, Is.EqualTo(new[] { 1, 2, 3, 5, 8 }));
		}
	}
}