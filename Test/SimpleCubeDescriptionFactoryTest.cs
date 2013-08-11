using NUnit.Framework;
using System;
using System.IO;

namespace CubeGenerator
{
	[TestFixture()]
	public class SimpleCubeDescriptionFactoryTest
	{
		[Test()]
		public void CreationTestCase ()
		{
			//given
			SimpleCubeDescriptionFactory cf = new SimpleCubeDescriptionFactory ();
			//when
			SimpleCubeDescription scd =
			cf.setCounterTableName ("COUNTER_TABLE_NAME")
			.setOlapCubeName ("OLAP_CUBE_NAME")
			.setOlapCubeSource ("OLAP_CUBE_SOURCE")
			.addDimension("DIM_1")
			.addDimension("DIM_2")
			.addOther("DIM_3")
			.addMeasure("MEASURE_1")
			.addMeasure("MEASURE_2")
			.addMeasure("MEASURE_3")
			.addOther("MEASURE_4")
			.create ("")
			.get();
			//then
			Assert.AreEqual ("COUNTER_TABLE_NAME", scd.counterTableName);
			Assert.AreEqual ("OLAP_CUBE_NAME", scd.olapCubeName);
			Assert.AreEqual ("OLAP_CUBE_SOURCE", scd.olapCubeSource);
			Assert.AreEqual (3, scd.dimensions.Length);
			Assert.AreEqual (4, scd.measures.Length);

		}
		[Test()]
		public void ParseLineTestCase()
		{
			//given
			SimpleCubeDescriptionFactory cf = new SimpleCubeDescriptionFactory ();
			//when
			SimpleCubeDescription scd =
				cf
					.parseLine ("Counter table name:  DMAGGR_SMACS")
					.parseLine ("OLAP cube source: VI_DMAGGR_SMACS  ")
					.parseLine ("OLAP cube Name:  \t SMACS")
					.parseLine ("Measures COUNT")
					.parseLine("\tTOTAL_VOLUME")
					.parseLine("\tUPLINK")
					.parseLine("\tDOWNLINK")
					.parseLine("Dimensions	START_TIME")
					.parseLine("SUBSCRIBER_TYPE")
					.parseLine("	TRAFFIC_CATEGORY")
					.parseLine("	ROAMING_OPERATOR")
					.parseLine("	ROAMING_ZONE")
					.parseLine("	NETWORK_ELEMENT")
					.parseLine("Generate")
					.get();
			//then
			Assert.AreEqual ("DMAGGR_SMACS", scd.counterTableName);
			Assert.AreEqual ("SMACS", scd.olapCubeName);
			Assert.AreEqual ("VI_DMAGGR_SMACS", scd.olapCubeSource);
			Assert.AreEqual (4, scd.measures.Length,string.Join(",",scd.measures));
			Assert.AreEqual (5, scd.dimensions.Length,string.Join(",",scd.dimensions));
			Assert.IsTrue( Array.Exists( scd.dimensions, delegate(string s){ return s.Equals("NETWORK_ELEMENT");}));
			//System.Console.WriteLine ();
		}
		[Test()]
		public void ParseFileTestCase()
		{
			//given
			String filePath = "CubeDescriptor.txt";
			StreamReader streamReader = new StreamReader(filePath);
			SimpleCubeDescriptionFactory cf = new SimpleCubeDescriptionFactory ();



			//when
			while (!streamReader.EndOfStream) 
			{
				string text = streamReader.ReadLine ();
				cf.parseLine (text);
			}
			//	streamReader.ReadToEnd();

			//then

			streamReader.Close();
		}
		[Test()]
		public void ParseFileAllTestCase()
		{
			//given
			String filePath = "CubeDescriptor.txt";
			StreamReader streamReader = new StreamReader(filePath);
			SimpleCubeDescriptionFactory cf = new SimpleCubeDescriptionFactory ();
			//when
			SimpleCubeDescription [] arrCube = cf.parseStream (streamReader);

			//then

			streamReader.Close();
		}
	}
}

