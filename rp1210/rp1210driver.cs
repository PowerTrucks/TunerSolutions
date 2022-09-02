using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;

namespace rp1210
{
	public class rp1210driver
	{
		public class PeriodicMessage : IDisposable
		{
			public enum PeriodicMessageType
			{
				J1939,
				J1587
			}

			public delegate void txJ1939Data(J1939Message msgToSend);

			public delegate void txJ1587Data(J1587Message msgToSend);

			private bool disposed;

			private J1939Message _j1939Message;

			private J1587Message _j1587Message;

			private System.Timers.Timer _timeKeeper;

			private PeriodicMessageType _MessageType;

			private ElapsedEventHandler _EventHandler;

			public J1939Message j1939Message => _j1939Message;

			public J1587Message j1587Message => _j1587Message;

			public PeriodicMessageType MessageType => _MessageType;

			public txJ1939Data SendJ1939Data { get; set; }

			public txJ1587Data SendJ1587Data { get; set; }

			public PeriodicMessage(J1939Message msg, double interval, txJ1939Data txDelegate)
			{
				_MessageType = PeriodicMessageType.J1939;
				_timeKeeper = new System.Timers.Timer(interval);
				SendJ1939Data = txDelegate;
				_j1939Message = msg;
				_EventHandler = _timeKeeper_Elapsed;
				_timeKeeper.Elapsed += _EventHandler;
				_timeKeeper.Enabled = true;
			}

			public PeriodicMessage(J1587Message msg, double interval, txJ1587Data txDelegate)
			{
				_MessageType = PeriodicMessageType.J1587;
				_timeKeeper = new System.Timers.Timer(interval);
				SendJ1587Data = txDelegate;
				_j1587Message = msg;
				_EventHandler = _timeKeeper_Elapsed;
				_timeKeeper.Elapsed += _EventHandler;
				_timeKeeper.Enabled = true;
			}

			private void _timeKeeper_Elapsed(object sender, ElapsedEventArgs e)
			{
				if (_MessageType == PeriodicMessageType.J1939)
				{
					if (_j1939Message != null && SendJ1939Data != null)
					{
						SendJ1939Data(_j1939Message);
					}
				}
				else if (_MessageType == PeriodicMessageType.J1587 && _j1587Message != null && SendJ1587Data != null)
				{
					SendJ1587Data(_j1587Message);
				}
			}

			public void Dispose()
			{
				if (!disposed)
				{
					_timeKeeper.Enabled = false;
					_timeKeeper.Elapsed -= _EventHandler;
				}
				disposed = true;
			}

			~PeriodicMessage()
			{
				Dispose();
			}
		}

		public class DataRecievedArgs : EventArgs
		{
			public bool J1939 { get; set; }

			public bool J1587 { get; set; }

			public J1939Message RecievedJ1939Message { get; set; }

			public J1587Message RecievedJ1587Message { get; set; }
		}

		public delegate void DataRecievedHandler(object sender, DataRecievedArgs e);

		private RP121032 J1939inst;

		private RP121032 J1587inst;

		private RP1210BDriverInfo driverInfo;

		private DeviceInfo deviceInfo;

		private Thread DataPoller;

		public List<J1939Message> J1939MessageFilter;

		public List<J1587Message> J1587MessageFilter;

		private List<string> _DriverList;

		private string _SelectedDriver;

		private List<string> _DeviceList;

		private string _SelectedDevice;

		public bool J1939Connected { get; set; }

		public bool J1587Connected { get; set; }

		public List<PeriodicMessage> PeriodicMessages { get; set; }

		public List<string> DriverList => _DriverList;

		public string SelectedDriver
		{
			get
			{
				return _SelectedDriver;
			}
			set
			{
				_SelectedDriver = value;
				_DeviceList.Clear();
				driverInfo = RP121032.LoadDeviceParameters(Environment.GetEnvironmentVariable("SystemRoot") + "\\" + _SelectedDriver + ".ini");
				foreach (DeviceInfo rP1210Device in driverInfo.RP1210Devices)
				{
					_DeviceList.Add(rP1210Device.DeviceName);
				}
			}
		}

		public List<string> DeviceList => _DeviceList;

		public string SelectedDevice
		{
			get
			{
				return _SelectedDevice;
			}
			set
			{
				_SelectedDevice = value;
				deviceInfo = driverInfo.RP1210Devices.Find((DeviceInfo x) => x.DeviceName == _SelectedDevice);
			}
		}

		public event DataRecievedHandler J1939DataRecieved;

		public rp1210driver()
		{
			_DeviceList = new List<string>();
			_DriverList = RP121032.ScanForDrivers();
			SelectedDriver = _DriverList[0];
			PeriodicMessages = new List<PeriodicMessage>();
			J1939MessageFilter = new List<J1939Message>();
		}

		private void J1939AddressClaim()
		{
			byte[] array = new byte[8] { 0, 0, 32, 37, 0, 129, 0, 0 };
			byte[] array2 = new byte[array.Length + 2];
			array2[0] = 0;
			array2[1] = array[0];
			array2[2] = array[1];
			array2[3] = array[2];
			array2[4] = array[3];
			array2[5] = array[4];
			array2[6] = array[5];
			array2[7] = array[6];
			array2[8] = array[7];
			array2[9] = 0;
			J1939inst.RP1210_SendCommand(RP1210_Commands.RP1210_Protect_J1939_Address, array2, (short)array2.Length);
		}

		public void J1939Connect()
		{
			bool flag = false;
			if (J1939Connected)
			{
				if (J1939inst != null)
				{
					J1939inst = null;
				}
				J1939Connected = false;
				return;
			}
			J1939inst = new RP121032(_SelectedDriver);
			try
			{
				J1939inst.RP1210_ClientConnect(deviceInfo.DeviceId, new StringBuilder("J1939"), 0, 0, 0);
				DataPoller = new Thread(new ThreadStart(PollingDriver));
				DataPoller.IsBackground = true;
				DataPoller.Start();
				try
				{
					J1939inst.RP1210_SendCommand(RP1210_Commands.RP1210_Set_All_Filters_States_to_Pass, null, 0);
					try
					{
						J1939AddressClaim();
					}
					catch (Exception ex)
					{
						flag = true;
						throw new Exception(ex.Message);
					}
				}
				catch (Exception ex2)
				{
					flag = true;
					throw new Exception(ex2.Message);
				}
			}
			catch (Exception)
			{
				flag = true;
			}
			if (!flag)
			{
				J1939Connected = true;
			}
		}

		public void J1587Connect()
		{
			bool flag = false;
			if (J1939Connected)
			{
				if (J1587inst != null)
				{
					J1587inst = null;
				}
				J1939Connected = false;
				return;
			}
			J1587inst = new RP121032(_SelectedDevice);
			try
			{
				J1587inst.RP1210_ClientConnect(deviceInfo.DeviceId, new StringBuilder("J1708"), 0, 0, 0);
			}
			catch (Exception)
			{
				flag = true;
			}
			try
			{
				J1587inst.RP1210_SendCommand(RP1210_Commands.RP1210_Set_All_Filters_States_to_Pass, null, 0);
			}
			catch (Exception)
			{
				flag = true;
			}
			if (!flag)
			{
				J1587Connected = true;
			}
		}

		public void J1939Disconnect()
		{
			if (J1939inst != null)
			{
				J1939Connected = false;
				J1939inst.RP1210_ClientDisconnect();
				J1939inst.Dispose();
				J1939inst = null;
			}
		}

		public void J1587Disconnect()
		{
			if (J1587inst != null)
			{
				J1587Connected = false;
				J1587inst.RP1210_ClientDisconnect();
				J1587inst.Dispose();
				J1587inst = null;
			}
		}

		public void Close()
		{
			try
			{
				foreach (PeriodicMessage periodicMessage in PeriodicMessages)
				{
					periodicMessage.Dispose();
				}
				PeriodicMessages.Clear();
				DataPoller.Abort();
				J1939Disconnect();
				J1587Disconnect();
			}
			catch
			{
			}
		}

		public void SendPeriodicMessage(J1939Message msgToSend, double interval)
		{
			PeriodicMessage item = new PeriodicMessage(msgToSend, interval, SendData);
			PeriodicMessages.Add(item);
		}

		public void SendPeriodicMessage(J1587Message msgToSend, double interval)
		{
			PeriodicMessage item = new PeriodicMessage(msgToSend, interval, SendData);
			PeriodicMessages.Add(item);
		}

		public void SendData(J1939Message msgToSend)
		{
			if (J1939inst != null)
			{
				try
				{
					byte[] array = RP121032.EncodeJ1939Message(msgToSend);
					_ = msgToSend.Priority;
					_ = msgToSend.PGN;
					_ = msgToSend.SourceAddress;
					J1939inst.RP1210_SendMessage(array, (short)array.Length, 0, 0);
				}
				catch (Exception)
				{
				}
			}
		}

		public void SendData(J1587Message msgToSend)
		{
			if (J1587inst != null)
			{
				try
				{
					byte[] array = msgToSend.ToArray();
					J1939inst.RP1210_SendMessage(array, (short)array.Length, 0, 0);
				}
				catch (Exception)
				{
				}
			}
		}

		private void PollingDriver()
		{
			while (true)
			{
				if (J1939inst != null)
				{
					byte[] array = J1939inst.RP1210_ReadMessage(0);
					if (array.Length <= 1)
					{
						continue;
					}
					DataRecievedArgs dataRecievedArgs = new DataRecievedArgs();
					dataRecievedArgs.J1939 = true;
					J1939Message message = RP121032.DecodeJ1939Message(array);
					if (J1939MessageFilter.Count != 0)
					{
						if (J1939MessageFilter.Find((J1939Message x) => x.PGN == message.PGN) != null)
						{
							dataRecievedArgs.RecievedJ1939Message = message;
						}
					}
					else
					{
						dataRecievedArgs.RecievedJ1939Message = message;
					}
					if (this.J1939DataRecieved != null && dataRecievedArgs.RecievedJ1939Message != null)
					{
						this.J1939DataRecieved(this, dataRecievedArgs);
					}
				}
				else if (J1587inst == null)
				{
					break;
				}
			}
		}
	}
}
