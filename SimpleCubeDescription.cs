using System;

namespace CubeGenerator
{

	public class SimpleCubeDescription
	{
		public  string counterTableName ;
		public  string olapCubeSource   ;
		public  string olapCubeName     ;
		public  string[] measures       ;
		public  string[] dimensions     ;

		public SimpleCubeDescription ()
		{
		}
	}
}

