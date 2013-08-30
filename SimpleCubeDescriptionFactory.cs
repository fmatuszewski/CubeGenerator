using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CubeGenerator
{
	delegate SimpleCubeDescriptionFactory ParserAction(string value);
	public delegate void ChangeEventHandler(SimpleCubeDescription description, String action,String change);

	public class SimpleCubeDescriptionFactory
	{
		private SimpleCubeDescription cubeDescriptionPrototype;
		public List<SimpleCubeDescription> cubeDescriptionList;
		private List<String> measureList;
		private List<String> dimensionList;
		private Dictionary<Regex, ParserAction> parseCommand;
		private Dictionary<State, ParserAction>    contextCommand;
		enum    State{New,Property,Measure,Dimension};
		private State activeState;

		public event ChangeEventHandler Changed;

		public SimpleCubeDescriptionFactory()
		{
			measureList = new List<String> ();
			dimensionList = new List<String> ();
			cubeDescriptionList = new List<SimpleCubeDescription>();
			cubeDescriptionPrototype = new SimpleCubeDescription();

			parseCommand = new Dictionary<Regex, ParserAction>()
			{
				{new Regex(@"Counter table name:\s+(?<word>\w+)\s?",RegexOptions.IgnoreCase),this.setCounterTableName},
				{new Regex(@"OLAP cube source:\s+(?<word>\w+)\s?"  ,RegexOptions.IgnoreCase),this.setOlapCubeSource},
				{new Regex(@"OLAP cube Name:\s+(?<word>\w+)\s?"    ,RegexOptions.IgnoreCase),this.setOlapCubeName},
				{new Regex(@"Measures\s+(?<word>[\w ]+)\s?"        ,RegexOptions.IgnoreCase),this.addMeasure},
				{new Regex(@"Dimensions\s+(?<word>[\w ]+)\s?"      ,RegexOptions.IgnoreCase),this.addDimension},
				{new Regex(@"Generate\s?"                          ,RegexOptions.IgnoreCase),this.create},
				{new Regex(@"\s+(?<word>[\w ]+)\s?"                ,RegexOptions.IgnoreCase),this.addOther}


			};
			contextCommand = new Dictionary<State, ParserAction>()
			{
				{State.New,      this.skip},
				{State.Property, this.skip},
				{State.Measure,  this.addMeasure},
				{State.Dimension,this.addDimension}
			};


		}

		private String extractString( Object gr )
		{
			if (gr is Group[]) 
			{

				return "";
			}
			if (gr is String) {
				return (String)gr;
			}
			return "";
		}
		public SimpleCubeDescriptionFactory parseLine(String line)
		{
			foreach( KeyValuePair<Regex,ParserAction> pair in parseCommand )
			{
				MatchCollection matches = pair.Key.Matches(line);
				if (matches.Count > 0)
				{
					pair.Value(matches[0].Groups["word"].Value.Trim().ToUpper().Replace(' ','_'));
					break;
				}
			}
			return this;
		}


		public void isNull()
		{
			if( cubeDescriptionPrototype == null )
				cubeDescriptionPrototype = new SimpleCubeDescription();
		}

		protected virtual void OnChange(String action,String value)
		{
			if (Changed != null)
				Changed (this.cubeDescriptionPrototype, action, value);

		}
		public SimpleCubeDescriptionFactory setCounterTableName(String name)
		{
			isNull();
			activeState = State.Property;
			cubeDescriptionPrototype.counterTableName = name;
			OnChange ("counterTableName", name);
			return this;
		}
		public SimpleCubeDescriptionFactory setOlapCubeSource(String name)
		{
			isNull();
			activeState = State.Property;
			cubeDescriptionPrototype.olapCubeSource = name;
			OnChange ("olapCubeSource", name);
			return this;
		}
		public SimpleCubeDescriptionFactory setOlapCubeName(String name)
		{
			isNull();
			activeState = State.Property;
			cubeDescriptionPrototype.olapCubeName = name;
			OnChange("olapCubeName",name);
			return this;
		}
		public SimpleCubeDescriptionFactory addMeasure(String value)
		{
			isNull();
			activeState = State.Measure;
			measureList.Add(value);
			OnChange ("addMeasure", value);
			return this;
		}
		public SimpleCubeDescriptionFactory addDimension(String value)
		{
			isNull();
			activeState = State.Dimension;
			dimensionList.Add(value);
			OnChange ("addDimension", value);
			return this;
		}
		public SimpleCubeDescriptionFactory addOther(String value)
		{
			contextCommand[activeState](value);
			return this;
		}

		public SimpleCubeDescriptionFactory create(String value){
			isNull();
			cubeDescriptionPrototype.measures   = measureList.ToArray();
			cubeDescriptionPrototype.dimensions = dimensionList.ToArray();
			cubeDescriptionList.Add(cubeDescriptionPrototype);
			cubeDescriptionPrototype = new SimpleCubeDescription();
			measureList.Clear();
			dimensionList.Clear();

			OnChange ("create", value);
			return this;
		}
		public SimpleCubeDescriptionFactory skip(String value)
		{

			return this;
		}

		public SimpleCubeDescription[] parseStream(StreamReader streamReader)
		{

			while(!streamReader.EndOfStream){
				string line = streamReader.ReadLine ();
				parseLine (line);
			}
			return cubeDescriptionList.ToArray ();
		}
	}
}

