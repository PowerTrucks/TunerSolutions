namespace rp1210
{
	public enum RP1210_Commands
	{
		RP1210_Reset_Device = 0,
		RP1210_Set_All_Filters_States_to_Pass = 3,
		RP1210_Set_Message_Filtering_For_J1939 = 4,
		RP1210_Set_Message_Filtering_For_CAN = 5,
		RP1210_Set_Message_Filtering_For_J1708 = 7,
		RP1210_Set_Message_Filtering_For_J1850 = 8,
		RP1210_Set_Message_Filtering_For_ISO15765 = 9,
		RP1210_Generic_Driver_Command = 14,
		RP1210_Set_J1708_Mode = 0xF,
		RP1210_Echo_Transmitted_Messages = 0x10,
		RP1210_Set_All_Filters_States_to_Discard = 17,
		RP1210_Set_Message_Receive = 18,
		RP1210_Protect_J1939_Address = 19,
		RP1210_Set_Broadcast_For_J1708 = 20,
		RP1210_Set_Broadcast_For_CAN = 21,
		RP1210_Set_Broadcast_For_J1939 = 22,
		RP1210_Set_Broadcast_For_J1850 = 23,
		RP1210_Set_J1708_Filter_Type = 24,
		RP1210_Set_J1939_Filter_Type = 25,
		RP1210_Set_CAN_Filter_Type = 26,
		RP1210_Set_J1939_Interpacket_Time = 27,
		RP1210_SetMaxErrorMsgSize = 28,
		RP1210_Disallow_Further_Connections = 29,
		RP1210_Set_J1850_Filter_Type = 30,
		RP1210_Release_J1939_Address = 0x1F,
		RP1210_Set_ISO15765_Filter_Type = 0x20,
		RP1210_Set_Broadcast_For_ISO15765 = 33,
		RP1210_Set_ISO15765_Flow_Control = 34,
		RP1210_Clear_ISO15765_Flow_Control = 35,
		RP1210_Set_ISO15765_Link_Type = 36,
		RP1210_Set_J1939_Baud = 37,
		RP1210_Set_ISO15765_Baud = 38,
		RP1210_Set_BlockTimeout = 215,
		RP1210_Set_J1708_Baud = 305
	}
}