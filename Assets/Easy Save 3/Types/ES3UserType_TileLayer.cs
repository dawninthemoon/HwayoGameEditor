using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("<DropdownValue>k__BackingField", "<TilesetName>k__BackingField", "_tilesetVisual", "_gridArray", "<LayerName>k__BackingField", "<LayerID>k__BackingField")]
	public class ES3UserType_TileLayer : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TileLayer() : base(typeof(CustomTilemap.TileLayer)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CustomTilemap.TileLayer)obj;
			
			writer.WritePrivateField("<DropdownValue>k__BackingField", instance);
			writer.WritePrivateField("<TilesetName>k__BackingField", instance);
			writer.WritePrivateFieldByRef("_tilesetVisual", instance);
			writer.WritePrivateField("_gridArray", instance);
			writer.WritePrivateField("<LayerName>k__BackingField", instance);
			writer.WritePrivateField("<LayerID>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CustomTilemap.TileLayer)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "<DropdownValue>k__BackingField":
					reader.SetPrivateField("<DropdownValue>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "<TilesetName>k__BackingField":
					reader.SetPrivateField("<TilesetName>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "_tilesetVisual":
					reader.SetPrivateField("_tilesetVisual", reader.Read<CustomTilemap.TilesetVisual>(), instance);
					break;
					case "_gridArray":
					reader.SetPrivateField("_gridArray", reader.Read<CustomTilemap.TileLayer.TileObject[,]>(), instance);
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
			var instance = new CustomTilemap.TileLayer();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_TileLayerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TileLayerArray() : base(typeof(CustomTilemap.TileLayer[]), ES3UserType_TileLayer.Instance)
		{
			Instance = this;
		}
	}
}