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
			.cubeDescriptionList[0];
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
					.parseLine("	ROAMING OPERATOR")
					.parseLine("	ROAMING_ZONE")
					.parseLine("	NETWORK ELEMENT")
					.parseLine("Generate")
					.cubeDescriptionList[0];
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
			Assert.AreEqual (5, arrCube.Length);
			Assert.AreEqual ("DMAGGR_SMACS", arrCube [0].counterTableName,"DMAGGR_SMACS");
			Assert.AreEqual ("VI_DMAGGR_SMACS", arrCube [0].olapCubeSource,"VI_DMAGGR_SMACS");
			Assert.AreEqual ("SMACS",arrCube[0].olapCubeName,"SMACS" );
			Assert.AreEqual (4, arrCube[0].measures.Length,string.Join(",",arrCube[0].measures));
			Assert.AreEqual (7, arrCube[0].dimensions.Length,string.Join(",",arrCube[0].dimensions));

			Assert.AreEqual ("COUNT"       , arrCube[0].measures[0]);
			Assert.AreEqual ("TOTAL_VOLUME", arrCube[0].measures[1]);
			Assert.AreEqual ("UPLINK"      , arrCube[0].measures[2]);
			Assert.AreEqual ("DOWNLINK"    , arrCube[0].measures[3]);

			Assert.AreEqual ("START_TIME"      , arrCube[0].dimensions[0]);
			Assert.AreEqual ("SUBSCRIBER_TYPE" , arrCube[0].dimensions[1]);
			Assert.AreEqual ("TRAFFIC_CATEGORY", arrCube[0].dimensions[2]);
			Assert.AreEqual ("ROAMING_OPERATOR", arrCube[0].dimensions[3]);
			Assert.AreEqual ("ROAMING_ZONE"    , arrCube[0].dimensions[4]);
			Assert.AreEqual ("NETWORK_ELEMENT" , arrCube[0].dimensions[5]);
			Assert.AreEqual ("BSCS_OUT"        , arrCube[0].dimensions[6]);	


			Assert.AreEqual ("DMAGGR_UMVPABXRTX", arrCube [1].counterTableName,"DMAGGR_UMVPABXRTX");
			Assert.AreEqual ("VI_UMVPABXRTX", arrCube [1].olapCubeSource,"VI_UMVPABXRTX");
			Assert.AreEqual ("UMVPABXRTX",arrCube[1].olapCubeName,"UMVPABXRTX" );
			Assert.AreEqual (3, arrCube[1].measures.Length,string.Join(",",arrCube[1].measures));
			Assert.AreEqual (9, arrCube[1].dimensions.Length,string.Join(",",arrCube[1].dimensions));

			streamReader.Close();
		}
	}
}

