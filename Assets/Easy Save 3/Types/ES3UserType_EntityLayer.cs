using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_entityModel", "_entityEditWindow", "_entityVisual", "_gridArray", "<LayerName>k__BackingField", "<LayerID>k__BackingField")]
	public class ES3UserType_EntityLayer : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_EntityLayer() : base(typeof(CustomTilemap.EntityLayer)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CustomTilemap.EntityLayer)obj;
			
			writer.WritePrivateFieldByRef("_entityModel", instance);
			writer.WritePrivateFieldByRef("_entityEditWindow", instance);
			writer.WritePrivateFieldByRef("_entityVisual", instance);
			writer.WritePrivateField("_gridArray", instance);
			writer.WritePrivateField("<LayerName>k__BackingField", instance);
			writer.WritePrivateField("<LayerID>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CustomTilemap.EntityLayer)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_entityModel":
					reader.SetPrivateField("_entityModel", reader.Read<CustomTilemap.EntityModel>(), instance);
					break;
					case "_entityEditWindow":
					reader.SetPrivateField("_entityEditWindow", reader.Read<EntityEditWindow>(), instance);
					break;
					case "_entityVisual":
					reader.SetPrivateField("_entityVisual", reader.Read<CustomTilemap.EntityVisual>(), instance);
					break;
					case "_gridArray":
					reader.SetPrivateField("_gridArray", reader.Read<CustomTilemap.EntityLayer.EntityObject[,]>(), instance);
					break;
					case "<LayerName>k__BackingField":
					reader.SetPrivateField("<LayerName>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "<LayerID>k__BackingField":
					reader.SetPrivateField("<LayerID>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CustomTilemap.EntityLayer();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_EntityLayerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_EntityLayerArray() : base(typeof(CustomTilemap.EntityLayer[]), ES3UserType_EntityLayer.Instance)
		{
			Instance = this;
		}
	}
}