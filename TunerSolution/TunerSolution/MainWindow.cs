using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using rp1210;
using TunerSolution.Properties;
using Vehicle_Applications;

namespace TunerSolution
{
	public class MainWindow : Window, IComponentConnector
	{
		private bool bConnected;

		private RP1210BDriverInfo driverInfo;

		private bool SniffStarted;

		private Flasher Vehicle;

		private BackgroundWorker SniffWorker;

		internal Button btnQuery;

		internal ComboBox cmbDriverList;

		internal ComboBox cmbDeviceList;

		internal TextBox txtStatus;

		internal ProgressBar prgBar1;

		internal ComboBox cmbSpecificVehicle;

		internal ComboBox cmbTruckList;

		internal TextBox txtBoxFileName;

		internal Button btnFlashFile;

		internal Button btnLoadFile;

		internal Button btSniff;

		internal TextBox txtBoxRestore;

		internal Button btnWriteRestore;

		internal Button btnReadRestore;

		internal Button btnLoadParamFile;

		internal RadioButton RadioFullFlash;

		internal RadioButton RadioCalibration;

		internal CheckBox CheckBoxJ1708;

		internal CheckBox CheckBoxJ1939;

		internal Button btSave;

		internal Button btnReadFile;

		internal TextBox txtBoxStartAddr;

		internal TextBox txtBoxLength;

		internal RadioButton radio250k;

		internal RadioButton radio500k;

		private bool _contentLoaded;

		public MainWindow()
			: this()
		{
			InitializeComponent();
		}

		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			((RangeBase)prgBar1).set_Value((double)e.ProgressPercentage);
			txtStatus.set_Text(e.UserState!.ToString());
		}

		private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error == null)
			{
				((RangeBase)prgBar1).set_Value(100.0);
				txtStatus.set_Text(e.Result!.ToString());
				((ContentControl)btnQuery).set_Content((object)"Disconnect");
				bConnected = true;
			}
			else
			{
				((RangeBase)prgBar1).set_Value(0.0);
				txtStatus.set_Text("Error: " + e.Error!.Message);
				bConnected = false;
				((ContentControl)btnQuery).set_Content((object)"Query");
			}
		}

		private void QueryCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			if (e.Error == null)
			{
				((RangeBase)prgBar1).set_Value(100.0);
				txtStatus.set_Text(e.Result!.ToString());
				((ContentControl)btnQuery).set_Content((object)"Disconnect");
				if (((Selector)cmbTruckList).get_SelectedItem().ToString() == "Paccar")
				{
					((UIElement)btnReadFile).set_IsEnabled(true);
					if (Vehicle.ecmModel.StartsWith("PC4__1408"))
					{
						txtBoxStartAddr.set_Text("240000");
						txtBoxLength.set_Text("2FF000");
					}
					else
					{
						txtBoxStartAddr.set_Text("200000");
						txtBoxLength.set_Text("2FF000");
					}
					if (Vehicle.ecmModel.StartsWith("PC4_A14"))
					{
						MessageBox.Show("This ECM Uses the New Style Bootloader, Contact Support");
					}
				}
				bConnected = true;
			}
			else
			{
				((RangeBase)prgBar1).set_Value(0.0);
				txtStatus.set_Text("Error: " + e.Error!.Message);
				bConnected = false;
				((ContentControl)btnQuery).set_Content((object)"Query");
			}
		}

		private void SniffProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			((RangeBase)prgBar1).set_Value((double)e.ProgressPercentage);
			TextBox obj = txtStatus;
			obj.set_Text(obj.get_Text() + e.UserState!.ToString() + Environment.NewLine);
		}

		private void SniffWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error == null)
			{
				((RangeBase)prgBar1).set_Value(100.0);
				((ContentControl)btSniff).set_Content((object)"Sniffer");
			}
			else
			{
				((RangeBase)prgBar1).set_Value(0.0);
				txtStatus.set_Text("Error: " + e.Error!.Message);
			}
		}

		private void Grid_Loaded(object sender, RoutedEventArgs e)
		{
			List<string> itemsSource = RP121032.ScanForDrivers();
			((ItemsControl)cmbDriverList).set_ItemsSource((IEnumerable)itemsSource);
			((Selector)cmbDriverList).set_SelectedIndex(Settings.Default.selectedDriver);
			((Selector)cmbDeviceList).set_SelectedIndex(Settings.Default.selectedDevice);
			((Selector)cmbTruckList).set_SelectedIndex(Settings.Default.selectedTruck);
			txtBoxFileName.set_Text(Settings.Default.fileName);
			txtBoxRestore.set_Text(Settings.Default.parameterFileName);
		}

		private void btnLoadFile_Click_1(object sender, RoutedEventArgs e)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OpenFileDialog val = new OpenFileDialog();
			((FileDialog)val).set_Filter("Tuning Files | *.bin");
			if (((CommonDialog)val).ShowDialog() == true)
			{
				txtBoxFileName.set_Text(((FileDialog)val).get_FileName());
				Settings.Default.fileName = ((FileDialog)val).get_FileName();
				((SettingsBase)Settings.Default).Save();
				if (bConnected)
				{
					((UIElement)btnFlashFile).set_IsEnabled(true);
				}
			}
		}

		private void cmbTruckList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch (((Selector)cmbTruckList).get_SelectedItem().ToString())
			{
			case "Maxxforce":
				((ItemsControl)cmbSpecificVehicle).get_Items().Clear();
				foreach (MaxxModuleType value in Enum.GetValues(typeof(MaxxModuleType)))
				{
					((ItemsControl)cmbSpecificVehicle).get_Items().Add((object)value);
				}
				((UIElement)cmbSpecificVehicle).set_IsEnabled(true);
				((Selector)cmbSpecificVehicle).set_SelectedIndex(Settings.Default.selectedSpecific);
				return;
			case "DetroitMCM":
				((ItemsControl)cmbSpecificVehicle).get_Items().Clear();
				foreach (DetroitMCMModuleType value2 in Enum.GetValues(typeof(DetroitMCMModuleType)))
				{
					((ItemsControl)cmbSpecificVehicle).get_Items().Add((object)value2);
				}
				((UIElement)cmbSpecificVehicle).set_IsEnabled(true);
				((Selector)cmbSpecificVehicle).set_SelectedIndex(Settings.Default.selectedSpecific);
				return;
			case "DetroitACM":
				((ItemsControl)cmbSpecificVehicle).get_Items().Clear();
				foreach (DetroitMCMModuleType value3 in Enum.GetValues(typeof(DetroitMCMModuleType)))
				{
					((ItemsControl)cmbSpecificVehicle).get_Items().Add((object)value3);
				}
				((UIElement)cmbSpecificVehicle).set_IsEnabled(true);
				((Selector)cmbSpecificVehicle).set_SelectedIndex(Settings.Default.selectedSpecific);
				return;
			}
			try
			{
				if (((UIElement)cmbSpecificVehicle).get_IsEnabled())
				{
					((ItemsControl)cmbSpecificVehicle).get_Items().Clear();
					((UIElement)cmbSpecificVehicle).set_IsEnabled(false);
				}
			}
			catch (Exception)
			{
			}
		}

		private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!bConnected)
			{
				driverInfo = RP121032.LoadDeviceParameters(Environment.GetEnvironmentVariable("SystemRoot") + "\\" + ((Selector)cmbDriverList).get_SelectedItem().ToString() + ".ini");
				((ItemsControl)cmbDeviceList).set_ItemsSource((IEnumerable)driverInfo.RP1210Devices);
			}
		}

		private void btSniff_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.selectedDriver = ((Selector)cmbDriverList).get_SelectedIndex();
			Settings.Default.selectedDevice = ((Selector)cmbDeviceList).get_SelectedIndex();
			Settings.Default.selectedTruck = ((Selector)cmbTruckList).get_SelectedIndex();
			Settings.Default.selectedSpecific = ((Selector)cmbSpecificVehicle).get_SelectedIndex();
			((SettingsBase)Settings.Default).Save();
			try
			{
				((ContentControl)btSniff).set_Content((object)"Stop");
				if (!SniffStarted)
				{
					if (Vehicle == null)
					{
						Vehicle = new Flasher(new RP121032(((Selector)cmbDriverList).get_SelectedItem().ToString()), new RP121032(((Selector)cmbDriverList).get_SelectedItem().ToString()), new RP121032(((Selector)cmbDriverList).get_SelectedItem().ToString()));
					}
					DeviceInfo selectedDevice = (DeviceInfo)((Selector)cmbDeviceList).get_SelectedValue();
					try
					{
						txtStatus.Clear();
						SniffStarted = true;
						SniffWorker = new BackgroundWorker();
						SniffWorker.WorkerReportsProgress = true;
						SniffWorker.WorkerSupportsCancellation = true;
						SniffWorker.DoWork += new DoWorkEventHandler(Vehicle.Sniffer);
						SniffWorker.ProgressChanged += new ProgressChangedEventHandler(SniffProgressChanged);
						SniffWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SniffWorkerCompleted);
						SnifferArgs argument = new SnifferArgs(selectedDevice, ((ToggleButton)CheckBoxJ1708).get_IsChecked().Value, ((ToggleButton)CheckBoxJ1939).get_IsChecked().Value);
						SniffWorker.RunWorkerAsync(argument);
					}
					catch (Exception ex)
					{
						txtStatus.set_Text(ex.Message);
					}
				}
				else
				{
					SniffStarted = false;
					SniffWorker.CancelAsync();
					SniffWorker.Dispose();
				}
			}
			catch (Exception ex2)
			{
				txtStatus.set_Text(ex2.Message);
			}
		}

		private void btnQuery_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.selectedDriver = ((Selector)cmbDriverList).get_SelectedIndex();
			Settings.Default.selectedDevice = ((Selector)cmbDeviceList).get_SelectedIndex();
			Settings.Default.selectedTruck = ((Selector)cmbTruckList).get_SelectedIndex();
			Settings.Default.selectedSpecific = ((Selector)cmbSpecificVehicle).get_SelectedIndex();
			((SettingsBase)Settings.Default).Save();
			txtStatus.set_Text("Connecting....");
			if (bConnected)
			{
				if (Vehicle.J1708inst != null)
				{
					Vehicle.J1708inst = null;
				}
				if (Vehicle.J1939inst != null)
				{
					Vehicle.J1939inst = null;
				}
				if (Vehicle.ISO15765inst != null)
				{
					Vehicle.ISO15765inst = null;
				}
				((ContentControl)btnQuery).set_Content((object)"Connect");
				bConnected = false;
				return;
			}
			Vehicle = new Flasher(new RP121032(((Selector)cmbDriverList).get_SelectedItem().ToString()), new RP121032(((Selector)cmbDriverList).get_SelectedItem().ToString()), new RP121032(((Selector)cmbDriverList).get_SelectedItem().ToString()));
			DeviceInfo deviceInfo = (DeviceInfo)((Selector)cmbDeviceList).get_SelectedValue();
			if (((ToggleButton)radio250k).get_IsChecked() == true)
			{
				deviceInfo.SelectedRate = BaudRate.Can250;
			}
			else
			{
				deviceInfo.SelectedRate = BaudRate.Can500;
			}
			try
			{
				switch (((Selector)cmbTruckList).get_SelectedItem().ToString())
				{
				case "Paccar":
				{
					BackgroundWorker backgroundWorker5 = new BackgroundWorker();
					backgroundWorker5.WorkerReportsProgress = true;
					backgroundWorker5.DoWork += new DoWorkEventHandler(Vehicle.PaccarQuery);
					backgroundWorker5.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker5.RunWorkerCompleted += new RunWorkerCompletedEventHandler(QueryCompleted);
					backgroundWorker5.RunWorkerAsync(deviceInfo);
					break;
				}
				case "DetroitACM":
				{
					BackgroundWorker backgroundWorker4 = new BackgroundWorker();
					backgroundWorker4.WorkerReportsProgress = true;
					backgroundWorker4.DoWork += new DoWorkEventHandler(Vehicle.DetroitACMQuery);
					backgroundWorker4.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker4.RunWorkerCompleted += new RunWorkerCompletedEventHandler(QueryCompleted);
					backgroundWorker4.RunWorkerAsync(deviceInfo);
					break;
				}
				case "DetroitMCM":
				{
					BackgroundWorker backgroundWorker3 = new BackgroundWorker();
					backgroundWorker3.WorkerReportsProgress = true;
					backgroundWorker3.DoWork += new DoWorkEventHandler(Vehicle.DetroitMCMQuery);
					backgroundWorker3.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker3.RunWorkerCompleted += new RunWorkerCompletedEventHandler(QueryCompleted);
					backgroundWorker3.RunWorkerAsync(new DetroitMCMAsyncArgs((DetroitMCMModuleType)((Selector)cmbSpecificVehicle).get_SelectedItem(), null, deviceInfo, ((ToggleButton)RadioFullFlash).get_IsChecked().Value));
					break;
				}
				case "Maxxforce":
				{
					BackgroundWorker backgroundWorker2 = new BackgroundWorker();
					backgroundWorker2.WorkerReportsProgress = true;
					backgroundWorker2.DoWork += new DoWorkEventHandler(Vehicle.MaxxforceQuery);
					backgroundWorker2.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(QueryCompleted);
					backgroundWorker2.RunWorkerAsync(new MaxxAsyncArgs((MaxxModuleType)((Selector)cmbSpecificVehicle).get_SelectedItem(), null, deviceInfo, fullFlash: false));
					break;
				}
				case "CaterpillarAG":
				{
					BackgroundWorker backgroundWorker = new BackgroundWorker();
					backgroundWorker.WorkerReportsProgress = true;
					backgroundWorker.DoWork += new DoWorkEventHandler(Vehicle.CatAGQuery);
					backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(QueryCompleted);
					backgroundWorker.RunWorkerAsync(deviceInfo);
					break;
				}
				default:
					throw new Exception("Incompatible Truck Selected");
				}
			}
			catch (Exception ex)
			{
				txtStatus.set_Text(ex.Message);
			}
			if (txtBoxFileName.get_Text() != "")
			{
				((UIElement)btnFlashFile).set_IsEnabled(true);
			}
			if (txtBoxRestore.get_Text() != "")
			{
				((UIElement)btnWriteRestore).set_IsEnabled(true);
			}
			((UIElement)btnReadRestore).set_IsEnabled(true);
			((UIElement)btSniff).set_IsEnabled(false);
		}

		private void btnFlashFile_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				switch (((Selector)cmbTruckList).get_SelectedItem().ToString())
				{
				case "Paccar":
				{
					BackgroundWorker backgroundWorker4 = new BackgroundWorker();
					backgroundWorker4.WorkerReportsProgress = true;
					backgroundWorker4.DoWork += new DoWorkEventHandler(Vehicle.PaccarFlash);
					backgroundWorker4.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker4.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
					backgroundWorker4.RunWorkerAsync(new Flasher.AddressRange(Convert.ToUInt32(txtBoxStartAddr.get_Text(), 16), Convert.ToUInt32(txtBoxLength.get_Text(), 16), txtBoxFileName.get_Text()));
					break;
				}
				case "DetroitACM":
				{
					BackgroundWorker backgroundWorker3 = new BackgroundWorker();
					backgroundWorker3.WorkerReportsProgress = true;
					backgroundWorker3.DoWork += new DoWorkEventHandler(Vehicle.DetroitACMFlash);
					backgroundWorker3.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker3.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
					backgroundWorker3.RunWorkerAsync(new DetroitMCMAsyncArgs((DetroitMCMModuleType)((Selector)cmbSpecificVehicle).get_SelectedItem(), txtBoxFileName.get_Text(), null, ((ToggleButton)RadioFullFlash).get_IsChecked().Value));
					break;
				}
				case "DetroitMCM":
				{
					BackgroundWorker backgroundWorker2 = new BackgroundWorker();
					backgroundWorker2.WorkerReportsProgress = true;
					backgroundWorker2.DoWork += new DoWorkEventHandler(Vehicle.DetroitMCMFlash);
					backgroundWorker2.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
					backgroundWorker2.RunWorkerAsync(new DetroitMCMAsyncArgs((DetroitMCMModuleType)((Selector)cmbSpecificVehicle).get_SelectedItem(), txtBoxFileName.get_Text(), null, ((ToggleButton)RadioFullFlash).get_IsChecked().Value));
					break;
				}
				case "Maxxforce":
				{
					BackgroundWorker backgroundWorker = new BackgroundWorker();
					backgroundWorker.WorkerReportsProgress = true;
					backgroundWorker.DoWork += new DoWorkEventHandler(Vehicle.MaxxforceFlash);
					backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
					backgroundWorker.RunWorkerAsync(new MaxxAsyncArgs((MaxxModuleType)((Selector)cmbSpecificVehicle).get_SelectedItem(), txtBoxFileName.get_Text(), null, ((ToggleButton)RadioFullFlash).get_IsChecked().Value));
					break;
				}
				default:
					throw new Exception("Incompatible Truck Selected");
				}
			}
			catch (Exception ex)
			{
				txtStatus.set_Text(ex.Message);
			}
		}

		private void btnLoadParamFile_Click(object sender, RoutedEventArgs e)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OpenFileDialog val = new OpenFileDialog();
			((FileDialog)val).set_Filter("Restore Files|*.txt");
			if (((CommonDialog)val).ShowDialog() == true)
			{
				txtBoxRestore.set_Text(((FileDialog)val).get_FileName());
				Settings.Default.parameterFileName = ((FileDialog)val).get_FileName();
				((SettingsBase)Settings.Default).Save();
				if (bConnected)
				{
					((UIElement)btnWriteRestore).set_IsEnabled(true);
				}
			}
		}

		private void btnReadRestore_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string text = ((Selector)cmbTruckList).get_SelectedItem().ToString();
				if (text == "Maxxforce")
				{
					BackgroundWorker backgroundWorker = new BackgroundWorker();
					backgroundWorker.WorkerReportsProgress = true;
					backgroundWorker.DoWork += new DoWorkEventHandler(Vehicle.MaxxforceReadRestore);
					backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
					backgroundWorker.RunWorkerAsync(new MaxxAsyncArgs((MaxxModuleType)((Selector)cmbSpecificVehicle).get_SelectedItem(), txtBoxRestore.get_Text(), null, fullFlash: false));
					return;
				}
				throw new Exception("Vehicle Not Implemented");
			}
			catch (Exception ex)
			{
				txtStatus.set_Text(ex.Message);
			}
		}

		private void btnWriteRestore_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string text = ((Selector)cmbTruckList).get_SelectedItem().ToString();
				if (text == "Maxxforce")
				{
					BackgroundWorker backgroundWorker = new BackgroundWorker();
					backgroundWorker.WorkerReportsProgress = true;
					backgroundWorker.DoWork += new DoWorkEventHandler(Vehicle.MaxxforceWriteRestore);
					backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
					backgroundWorker.RunWorkerAsync(new MaxxAsyncArgs((MaxxModuleType)((Selector)cmbSpecificVehicle).get_SelectedItem(), txtBoxRestore.get_Text(), null, fullFlash: false));
					return;
				}
				throw new Exception("Vehicle Not Implemented");
			}
			catch (Exception ex)
			{
				txtStatus.set_Text(ex.Message);
			}
		}

		private void btSave_Click(object sender, RoutedEventArgs e)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			SaveFileDialog val = new SaveFileDialog();
			((FileDialog)val).set_Filter("Sniff Files|*.txt");
			if (((CommonDialog)val).ShowDialog() == true)
			{
				File.WriteAllText(((FileDialog)val).get_FileName(), txtStatus.get_Text());
			}
		}

		private void BtnReadFile_Click(object sender, RoutedEventArgs e)
		{
			((UIElement)btnReadFile).set_IsEnabled(false);
			try
			{
				string text = ((Selector)cmbTruckList).get_SelectedItem().ToString();
				if (text == "Paccar")
				{
					BackgroundWorker backgroundWorker = new BackgroundWorker();
					backgroundWorker.WorkerReportsProgress = true;
					backgroundWorker.DoWork += new DoWorkEventHandler(Vehicle.PaccarRead);
					backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
					backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
					backgroundWorker.RunWorkerAsync(new Flasher.AddressRange(Convert.ToUInt32(txtBoxStartAddr.get_Text(), 16), Convert.ToUInt32(txtBoxLength.get_Text(), 16), txtBoxFileName.get_Text()));
					return;
				}
				throw new Exception("No Read Available");
			}
			catch (Exception ex)
			{
				txtStatus.set_Text(ex.Message);
			}
		}

		private void BtDetDel_Click(object sender, RoutedEventArgs e)
		{
			((UIElement)btnReadFile).set_IsEnabled(false);
			try
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.WorkerReportsProgress = true;
				backgroundWorker.DoWork += new DoWorkEventHandler(Vehicle.DetroitDelete);
				backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
				backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
				backgroundWorker.RunWorkerAsync();
			}
			catch (Exception ex)
			{
				txtStatus.set_Text(ex.Message);
			}
		}

		private void CmbDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		private void CmbSpecificVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		public string RandomString(int size, bool lowerCase)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Random random = new Random();
			for (int i = 0; i < size; i++)
			{
				char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * random.NextDouble() + 65.0)));
				stringBuilder.Append(value);
			}
			if (lowerCase)
			{
				return stringBuilder.ToString().ToLower();
			}
			return stringBuilder.ToString();
		}

		private void Window_Initialized(object sender, EventArgs e)
		{
			string text = RandomString(10, lowerCase: false);
			string text2 = Interaction.InputBox(text, "Password", text, -1, -1);
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.ASCII.GetBytes(text);
			string text3 = BitConverter.ToString(mD.ComputeHash(bytes)).Replace("-", string.Empty);
			if (text2 != text3)
			{
				((Window)this).Close();
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri uri = new Uri("/TunerSolution;component/mainwindow.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Expected O, but got Unknown
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected O, but got Unknown
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Expected O, but got Unknown
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Expected O, but got Unknown
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Expected O, but got Unknown
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Expected O, but got Unknown
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Expected O, but got Unknown
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Expected O, but got Unknown
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Expected O, but got Unknown
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Expected O, but got Unknown
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Expected O, but got Unknown
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Expected O, but got Unknown
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Expected O, but got Unknown
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Expected O, but got Unknown
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Expected O, but got Unknown
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Expected O, but got Unknown
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Expected O, but got Unknown
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Expected O, but got Unknown
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Expected O, but got Unknown
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Expected O, but got Unknown
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Expected O, but got Unknown
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Expected O, but got Unknown
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Expected O, but got Unknown
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Expected O, but got Unknown
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Expected O, but got Unknown
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Expected O, but got Unknown
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Expected O, but got Unknown
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Expected O, but got Unknown
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Expected O, but got Unknown
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Expected O, but got Unknown
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Expected O, but got Unknown
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Expected O, but got Unknown
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Expected O, but got Unknown
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Expected O, but got Unknown
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Expected O, but got Unknown
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Expected O, but got Unknown
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				((FrameworkElement)(MainWindow)target).add_Loaded(new RoutedEventHandler(Window_Loaded));
				((FrameworkElement)(MainWindow)target).add_Initialized((EventHandler)Window_Initialized);
				break;
			case 2:
				((FrameworkElement)(Grid)target).add_Loaded(new RoutedEventHandler(Grid_Loaded));
				break;
			case 3:
				btnQuery = (Button)target;
				((ButtonBase)btnQuery).add_Click(new RoutedEventHandler(btnQuery_Click));
				break;
			case 4:
				cmbDriverList = (ComboBox)target;
				((Selector)cmbDriverList).add_SelectionChanged(new SelectionChangedEventHandler(comboBox1_SelectionChanged));
				break;
			case 5:
				cmbDeviceList = (ComboBox)target;
				((Selector)cmbDeviceList).add_SelectionChanged(new SelectionChangedEventHandler(CmbDeviceList_SelectionChanged));
				break;
			case 6:
				txtStatus = (TextBox)target;
				break;
			case 7:
				prgBar1 = (ProgressBar)target;
				break;
			case 8:
				cmbSpecificVehicle = (ComboBox)target;
				((Selector)cmbSpecificVehicle).add_SelectionChanged(new SelectionChangedEventHandler(CmbSpecificVehicle_SelectionChanged));
				break;
			case 9:
				cmbTruckList = (ComboBox)target;
				((Selector)cmbTruckList).add_SelectionChanged(new SelectionChangedEventHandler(cmbTruckList_SelectionChanged));
				break;
			case 10:
				txtBoxFileName = (TextBox)target;
				break;
			case 11:
				btnFlashFile = (Button)target;
				((ButtonBase)btnFlashFile).add_Click(new RoutedEventHandler(btnFlashFile_Click));
				break;
			case 12:
				btnLoadFile = (Button)target;
				((ButtonBase)btnLoadFile).add_Click(new RoutedEventHandler(btnLoadFile_Click_1));
				break;
			case 13:
				btSniff = (Button)target;
				((ButtonBase)btSniff).add_Click(new RoutedEventHandler(btSniff_Click));
				break;
			case 14:
				txtBoxRestore = (TextBox)target;
				break;
			case 15:
				btnWriteRestore = (Button)target;
				((ButtonBase)btnWriteRestore).add_Click(new RoutedEventHandler(btnWriteRestore_Click));
				break;
			case 16:
				btnReadRestore = (Button)target;
				((ButtonBase)btnReadRestore).add_Click(new RoutedEventHandler(btnReadRestore_Click));
				break;
			case 17:
				btnLoadParamFile = (Button)target;
				((ButtonBase)btnLoadParamFile).add_Click(new RoutedEventHandler(btnLoadParamFile_Click));
				break;
			case 18:
				RadioFullFlash = (RadioButton)target;
				break;
			case 19:
				RadioCalibration = (RadioButton)target;
				break;
			case 20:
				CheckBoxJ1708 = (CheckBox)target;
				break;
			case 21:
				CheckBoxJ1939 = (CheckBox)target;
				break;
			case 22:
				btSave = (Button)target;
				((ButtonBase)btSave).add_Click(new RoutedEventHandler(btSave_Click));
				break;
			case 23:
				btnReadFile = (Button)target;
				((ButtonBase)btnReadFile).add_Click(new RoutedEventHandler(BtnReadFile_Click));
				break;
			case 24:
				txtBoxStartAddr = (TextBox)target;
				break;
			case 25:
				txtBoxLength = (TextBox)target;
				break;
			case 26:
				radio250k = (RadioButton)target;
				break;
			case 27:
				radio500k = (RadioButton)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
