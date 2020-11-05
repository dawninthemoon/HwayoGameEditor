using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_x", "_y", "_textureIndex", "<EntityName>k__BackingField", "<EntityID>k__BackingField", "_fields")]
	public class ES3UserType_EntityObject : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_EntityObject() : base(typeof(CustomTilemap.EntityLayer.EntityObject)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CustomTilemap.EntityLayer.EntityObject)obj;
			
			writer.WritePrivateField("_x", instance);
			writer.WritePrivateField("_y", instance);
			writer.WritePrivateField("_textureIndex", instance);
			writer.WritePrivateField("<EntityName>k__BackingField", instance);
			writer.WritePrivateField("<EntityID>k__BackingField", instance);
			writer.WritePrivateField("_fields", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CustomTilemap.EntityLayer.EntityObject)obj;
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
					case "<EntityName>k__BackingField":
					reader.SetPrivateField("<EntityName>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "<EntityID>k__BackingField":
					reader.SetPrivateField("<EntityID>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "_fields":
					reader.SetPrivateField("_fields", reader.Read<System.Collections.Generic.Dictionary<System.String, System.String>>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CustomTilemap.EntityLayer.EntityObject();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_EntityObjectArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_EntityObjectArray() : base(typeof(CustomTilemap.EntityLayer.EntityObject[]), ES3UserType_EntityObject.Instance)
		{
			Instance = this;
		}
	}
}