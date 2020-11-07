using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_cellSize", "<Tag>k__BackingField", "_points", "_visual", "<LayerName>k__BackingField", "<LayerID>k__BackingField")]
	public class ES3UserType_CollisionLayer : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CollisionLayer() : base(typeof(CustomTilemap.CollisionLayer)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CustomTilemap.CollisionLayer)obj;
			
			writer.WritePrivateField("_cellSize", instance);
			writer.WritePrivateField("<Tag>k__BackingField", instance);
			writer.WritePrivateField("_points", instance);
			writer.WritePrivateFieldByRef("_visual", instance);
			writer.WritePrivateField("<LayerName>k__BackingField", instance);
			writer.WritePrivateField("<LayerID>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CustomTilemap.CollisionLayer)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_cellSize":
					reader.SetPrivateField("_cellSize", reader.Read<System.Single>(), instance);
					break;
					case "<Tag>k__BackingField":
					reader.SetPrivateField("<Tag>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "_points":
					reader.SetPrivateField("_points", reader.Read<System.Collections.Generic.List<UnityEngine.Vector2>>(), instance);
					break;
					case "_visual":
					reader.SetPrivateField("_visual", reader.Read<CustomTilemap.CollisionVisual>(), instance);
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
			var instance = new CustomTilemap.CollisionLayer();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_CollisionLayerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CollisionLayerArray() : base(typeof(CustomTilemap.CollisionLayer[]), ES3UserType_CollisionLayer.Instance)
		{
			Instance = this;
		}
	}
}