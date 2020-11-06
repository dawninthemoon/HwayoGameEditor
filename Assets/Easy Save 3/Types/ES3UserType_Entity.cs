using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("<EntityName>k__BackingField", "<TextureIndex>k__BackingField", "<EntityColor>k__BackingField", "<EntityID>k__BackingField", "<FieldSequence>k__BackingField", "_fields")]
	public class ES3UserType_Entity : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Entity() : base(typeof(CustomTilemap.Entity)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CustomTilemap.Entity)obj;
			
			writer.WritePrivateField("<EntityName>k__BackingField", instance);
			writer.WritePrivateField("<TextureIndex>k__BackingField", instance);
			writer.WritePrivateField("<EntityColor>k__BackingField", instance);
			writer.WritePrivateField("<EntityID>k__BackingField", instance);
			writer.WritePrivateField("<FieldSequence>k__BackingField", instance);
			writer.WritePrivateField("_fields", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CustomTilemap.Entity)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "<EntityName>k__BackingField":
					reader.SetPrivateField("<EntityName>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "<TextureIndex>k__BackingField":
					reader.SetPrivateField("<TextureIndex>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "<EntityColor>k__BackingField":
					reader.SetPrivateField("<EntityColor>k__BackingField", reader.Read<UnityEngine.Color>(), instance);
					break;
					case "<EntityID>k__BackingField":
					reader.SetPrivateField("<EntityID>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "<FieldSequence>k__BackingField":
					reader.SetPrivateField("<FieldSequence>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "_fields":
					reader.SetPrivateField("_fields", reader.Read<System.Collections.Generic.List<System.String>>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CustomTilemap.Entity();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_EntityArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_EntityArray() : base(typeof(CustomTilemap.Entity[]), ES3UserType_Entity.Instance)
		{
			Instance = this;
		}
	}
}