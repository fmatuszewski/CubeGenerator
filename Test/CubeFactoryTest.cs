using NUnit.Framework;
using System;

namespace CubeGenerator
{
	[TestFixture()]
	public class CubeFactoryTest
	{
		[Test()]
		public void TestCase ()
		{
		}

		[Test()]
		public void ConnectDatabaseTestCase ()
		{
			//given
			CubeFactory cf = new CubeFactory ("LocalHost", "msolap", "MyOlap");
			//when

			//then
			Assert.IsTrue (cf.IsConnected (), "AnalysisServices not connected");
		}

	}
}

