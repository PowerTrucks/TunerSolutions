using System;
using System.Collections;
using System.IO;

namespace rp1210
{
	public class IniParser
	{
		private struct SectionPair
		{
			public string Section;

			public string Key;
		}

		private Hashtable keyPairs = new Hashtable();

		private string iniFilePath;

		public IniParser(string iniPath)
		{
			TextReader textReader = null;
			string text = null;
			string text2 = null;
			string[] array = null;
			iniFilePath = iniPath;
			if (File.Exists(iniPath))
			{
				try
				{
					textReader = new StreamReader(iniPath);
					SectionPair sectionPair = default(SectionPair);
					for (text = textReader.ReadLine(); text != null; text = textReader.ReadLine())
					{
						text = text.Trim().ToUpper();
						if (text != "")
						{
							if (text.StartsWith("[") && text.EndsWith("]"))
							{
								text2 = text.Substring(1, text.Length - 2);
							}
							else if (!text.StartsWith("#") & !text.StartsWith(";"))
							{
								array = text.Split(new char[1] { '=' }, 2);
								string value = null;
								if (text2 == null)
								{
									text2 = "ROOT";
								}
								sectionPair.Section = text2;
								sectionPair.Key = array[0];
								if (array.Length > 1)
								{
									value = array[1];
								}
								keyPairs.Add(sectionPair, value);
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					textReader?.Close();
				}
				return;
			}
			throw new FileNotFoundException("Unable to locate " + iniPath);
		}

		public string GetSetting(string sectionName, string settingName)
		{
			SectionPair sectionPair = default(SectionPair);
			sectionPair.Section = sectionName.ToUpper();
			sectionPair.Key = settingName.ToUpper();
			return (string)keyPairs[sectionPair];
		}

		public string[] EnumSection(string sectionName)
		{
			ArrayList arrayList = new ArrayList();
			foreach (SectionPair key in keyPairs.Keys)
			{
				if (key.Section == sectionName.ToUpper())
				{
					arrayList.Add(key.Key);
				}
			}
			return (string[])arrayList.ToArray(typeof(string));
		}

		public void AddSetting(string sectionName, string settingName, string settingValue)
		{
			SectionPair sectionPair = default(SectionPair);
			sectionPair.Section = sectionName.ToUpper();
			sectionPair.Key = settingName.ToUpper();
			if (keyPairs.ContainsKey(sectionPair))
			{
				keyPairs.Remove(sectionPair);
			}
			keyPairs.Add(sectionPair, settingValue);
		}

		public void AddSetting(string sectionName, string settingName)
		{
			AddSetting(sectionName, settingName, null);
		}

		public void DeleteSetting(string sectionName, string settingName)
		{
			SectionPair sectionPair = default(SectionPair);
			sectionPair.Section = sectionName.ToUpper();
			sectionPair.Key = settingName.ToUpper();
			if (keyPairs.ContainsKey(sectionPair))
			{
				keyPairs.Remove(sectionPair);
			}
		}

		public void SaveSettings(string newFilePath)
		{
			ArrayList arrayList = new ArrayList();
			string text = "";
			string text2 = "";
			foreach (SectionPair key in keyPairs.Keys)
			{
				if (!arrayList.Contains(key.Section))
				{
					arrayList.Add(key.Section);
				}
			}
			foreach (string item in arrayList)
			{
				text2 = text2 + "[" + item + "]\r\n";
				foreach (SectionPair key2 in keyPairs.Keys)
				{
					if (key2.Section == item)
					{
						text = (string)keyPairs[key2];
						if (text != null)
						{
							text = "=" + text;
						}
						text2 = text2 + key2.Key + text + "\r\n";
					}
				}
				text2 += "\r\n";
			}
			try
			{
				StreamWriter streamWriter = new StreamWriter(newFilePath);
				streamWriter.Write(text2);
				streamWriter.Close();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void SaveSettings()
		{
			SaveSettings(iniFilePath);
		}
	}
}
