using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_OnClick", "m_Colors", "m_TargetGraphic", "m_CurrentIndex")]
	public class ES3UserType_Button : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Button() : base(typeof(UnityEngine.UI.Button)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.UI.Button)obj;
			
			writer.WritePrivateField("m_OnClick", instance);
			writer.WritePrivateField("m_Colors", instance);
			writer.WritePrivateFieldByRef("m_TargetGraphic", instance);
			writer.WritePrivateField("m_CurrentIndex", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.UI.Button)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_OnClick":
					reader.SetPrivateField("m_OnClick", reader.Read<UnityEngine.UI.Button.ButtonClickedEvent>(), instance);
					break;
					case "m_Colors":
					reader.SetPrivateField("m_Colors", reader.Read<UnityEngine.UI.ColorBlock>(), instance);
					break;
					case "m_TargetGraphic":
					reader.SetPrivateField("m_TargetGraphic", reader.Read<UnityEngine.UI.Graphic>(), instance);
					break;
					case "m_CurrentIndex":
					reader.SetPrivateField("m_CurrentIndex", reader.Read<System.Int32>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_ButtonArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ButtonArray() : base(typeof(UnityEngine.UI.Button[]), ES3UserType_Button.Instance)
		{
			Instance = this;
		}
	}
}