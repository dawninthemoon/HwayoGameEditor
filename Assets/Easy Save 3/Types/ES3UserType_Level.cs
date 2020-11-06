using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("<LevelID>k__BackingField", "<LevelName>k__BackingField")]
	public class ES3UserType_Level : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Level() : base(typeof(CustomTilemap.Level)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CustomTilemap.Level)obj;
			
			writer.WritePrivateField("<LevelID>k__BackingField", instance);
			writer.WritePrivateField("<LevelName>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CustomTilemap.Level)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "<LevelID>k__BackingField":
					reader.SetPrivateField("<LevelID>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "<LevelName>k__BackingField":
					reader.SetPrivateField("<LevelName>k__BackingField", reader.Read<System.String>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CustomTilemap.Level();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_LevelArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_LevelArray() : base(typeof(CustomTilemap.Level[]), ES3UserType_Level.Instance)
		{
			Instance = this;
		}
	}
}