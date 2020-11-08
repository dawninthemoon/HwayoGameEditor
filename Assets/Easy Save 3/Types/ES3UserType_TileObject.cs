using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("x", "y", "textureIndex")]
	public class ES3UserType_TileObject : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TileObject() : base(typeof(CustomTilemap.TileLayer.TileObject)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CustomTilemap.TileLayer.TileObject)obj;
			
			writer.WriteProperty("x", instance.x, ES3Type_int.Instance);
			writer.WriteProperty("y", instance.y, ES3Type_int.Instance);
			writer.WriteProperty("textureIndex", instance.textureIndex, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CustomTilemap.TileLayer.TileObject)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "x":
						instance.x = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "y":
						instance.y = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "textureIndex":
						instance.textureIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CustomTilemap.TileLayer.TileObject();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_TileObjectArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TileObjectArray() : base(typeof(CustomTilemap.TileLayer.TileObject[]), ES3UserType_TileObject.Instance)
		{
			Instance = this;
		}
	}
}