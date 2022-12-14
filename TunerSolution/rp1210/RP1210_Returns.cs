namespace rp1210
{
	public enum RP1210_Returns
	{
		NO_ERRORS = 0,
		ERR_DLL_NOT_INITIALIZED = 0x80,
		ERR_INVALID_CLIENT_ID = 129,
		ERR_CLIENT_ALREADY_CONNECTED = 130,
		ERR_CLIENT_AREA_FULL = 131,
		ERR_FREE_MEMORY = 132,
		ERR_NOT_ENOUGH_MEMORY = 133,
		ERR_INVALID_DEVICE = 134,
		ERR_DEVICE_IN_USE = 135,
		ERR_INVALID_PROTOCOL = 136,
		ERR_TX_QUEUE_FULL = 137,
		ERR_TX_QUEUE_CORRUPT = 138,
		ERR_RX_QUEUE_FULL = 139,
		ERR_RX_QUEUE_CORRUPT = 140,
		ERR_MESSAGE_TOO_LONG = 141,
		ERR_HARDWARE_NOT_RESPONDING = 142,
		ERR_COMMAND_NOT_SUPPORTED = 143,
		ERR_INVALID_COMMAND = 144,
		ERR_TXMESSAGE_STATUS = 145,
		ERR_ADDRESS_CLAIM_FAILED = 146,
		ERR_CANNOT_SET_PRIORITY = 147,
		ERR_CLIENT_DISCONNECTED = 148,
		ERR_CONNECT_NOT_ALLOWED = 149,
		ERR_CHANGE_MODE_FAILED = 150,
		ERR_BUS_OFF = 151,
		ERR_COULD_NOT_TX_ADDRESS_CLAIMED = 152,
		ERR_ADDRESS_LOST = 153,
		ERR_CODE_NOT_FOUND = 154,
		ERR_BLOCK_NOT_ALLOWED = 155,
		ERR_MULTIPLE_CLIENTS_CONNECTED = 156,
		ERR_ADDRESS_NEVER_CLAIMED = 157,
		ERR_WINDOW_HANDLE_REQUIRED = 158,
		ERR_MESSAGE_NOT_SENT = 159,
		ERR_MAX_NOTIFY_EXCEEDED = 160,
		ERR_MAX_FILTERS_EXCEEDED = 161,
		ERR_HARDWARE_STATUS_CHANGE = 162,
		ERR_INI_FILE_NOT_IN_WIN_DIR = 202,
		ERR_INI_SECTION_NOT_FOUND = 204,
		ERR_INI_KEY_NOT_FOUND = 205,
		ERR_INVALID_KEY_STRING = 206,
		ERR_DEVICE_NOT_SUPPORTED = 207,
		ERR_INVALID_PORT_PARAM = 208,
		ERR_COMMAND_TIMED_OUT = 213,
		ERR_OS_NOT_SUPPORTED = 220,
		ERR_COMMAND_QUEUE_IS_FULL = 222,
		ERR_CANNOT_SET_CAN_BAUDRATE = 224,
		ERR_CANNOT_CLAIM_BROADCAST_ADDRESS = 225,
		ERR_OUT_OF_ADDRESS_RESOURCES = 226,
		ERR_ADDRESS_RELEASE_FAILED = 227,
		ERR_COMM_DEVICE_IN_USE = 230,
		DG_DLL_NOT_FOUND = 254,
		DG_BUSY_SENDING = 0xFF,
		ERR_DATA_LINK_CONFLICT = 441,
		ERR_ADAPTER_NOT_RESPONDING = 453,
		ERR_CAN_BAUD_SET_NONSTANDARD = 454,
		ERR_MULTIPLE_CONNECTIONS_NOT_ALLOWED_NOW = 455,
		ERR_J1708_BAUD_SET_NONSTANDARD = 456,
		ERR_J1939_BAUD_SET_NONSTANDARD = 457,
		ERR_ISO15765_BAUD_SET_NONSTANDARD = 458
	}
}
