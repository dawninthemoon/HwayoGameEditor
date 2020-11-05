using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_x", "_y", "_textureIndex")]
	public class ES3UserType_TileObject : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TileObject() : base(typeof(CustomTilemap.TileLayer.TileObject)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CustomTilemap.TileLayer.TileObject)obj;
			
			writer.WritePrivateField("_x", instance);
			writer.WritePrivateField("_y", instance);
			writer.WritePrivateField("_textureIndex", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CustomTilemap.TileLayer.TileObject)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_x":
					reader.SetPrivateField("_x", reader.Read<System.Int32>(), instance);
					break;
					case "_y":
					reader.SetPrivateField("_y", reader.Read<System.Int32>(), instance);
					break;
					case "_textureIndex":
					reader.SetPrivateField("_textureIndex", reader.Read<System.Int32>(), instance);
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