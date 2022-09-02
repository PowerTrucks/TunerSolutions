using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TunerSolution.Properties
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)(object)SettingsBase.Synchronized((SettingsBase)(object)new Settings());

		public static Settings Default => defaultInstance;

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0")]
		public int selectedDriver
		{
			get
			{
				return (int)((SettingsBase)this).get_Item("selectedDriver");
			}
			set
			{
				((SettingsBase)this).set_Item("selectedDriver", (object)value);
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0")]
		public int selectedDevice
		{
			get
			{
				return (int)((SettingsBase)this).get_Item("selectedDevice");
			}
			set
			{
				((SettingsBase)this).set_Item("selectedDevice", (object)value);
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string fileName
		{
			get
			{
				return (string)((SettingsBase)this).get_Item("fileName");
			}
			set
			{
				((SettingsBase)this).set_Item("fileName", (object)value);
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0")]
		public int selectedTruck
		{
			get
			{
				return (int)((SettingsBase)this).get_Item("selectedTruck");
			}
			set
			{
				((SettingsBase)this).set_Item("selectedTruck", (object)value);
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0")]
		public int selectedSpecific
		{
			get
			{
				return (int)((SettingsBase)this).get_Item("selectedSpecific");
			}
			set
			{
				((SettingsBase)this).set_Item("selectedSpecific", (object)value);
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string parameterFileName
		{
			get
			{
				return (string)((SettingsBase)this).get_Item("parameterFileName");
			}
			set
			{
				((SettingsBase)this).set_Item("parameterFileName", (object)value);
			}
		}

		public Settings()
			: this()
		{
		}
	}
}
