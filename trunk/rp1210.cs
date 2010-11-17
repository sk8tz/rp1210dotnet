using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace rp1210
{
    /// <summary>
    ///  RP1210A  RP1210_SendCommand Defines
    /// </summary>
    enum RP1210_Commands
    {
        RP1210_Reset_Device                      = 0,
        RP1210_Set_All_Filters_States_to_Pass    = 3,
        RP1210_Set_Message_Filtering_For_J1939   = 4,
        RP1210_Set_Message_Filtering_For_CAN     = 5,
        RP1210_Set_Message_Filtering_For_J1708   = 7,
        RP1210_Generic_Driver_Command            = 14,
        RP1210_Set_J1708_Mode                    = 15,
        RP1210_Echo_Transmitted_Messages         = 16,
        RP1210_Set_All_Filters_States_to_Discard = 17,
        RP1210_Set_Message_Receive               = 18,
        RP1210_Protect_J1939_Address             = 19
    }

    /// <summary>
    /// RP1210A Return Definitions
    /// </summary>
    enum RP1210_Returns
    {
        NO_ERRORS                           =   0,
        ERR_DLL_NOT_INITIALIZED             = 128,
        ERR_INVALID_CLIENT_ID               = 129,
        ERR_CLIENT_ALREADY_CONNECTED        = 130,
        ERR_CLIENT_AREA_FULL                = 131,
        ERR_FREE_MEMORY                     = 132,
        ERR_NOT_ENOUGH_MEMORY               = 133,
        ERR_INVALID_DEVICE                  = 134,
        ERR_DEVICE_IN_USE                   = 135,
        ERR_INVALID_PROTOCOL                = 136,
        ERR_TX_QUEUE_FULL                   = 137,
        ERR_TX_QUEUE_CORRUPT                = 138,
        ERR_RX_QUEUE_FULL                   = 139,
        ERR_RX_QUEUE_CORRUPT                = 140,
        ERR_MESSAGE_TOO_LONG                = 141,
        ERR_HARDWARE_NOT_RESPONDING         = 142,
        ERR_COMMAND_NOT_SUPPORTED           = 143,
        ERR_INVALID_COMMAND                 = 144,
        ERR_TXMESSAGE_STATUS                = 145,
        ERR_ADDRESS_CLAIM_FAILED            = 146,
        ERR_CANNOT_SET_PRIORITY             = 147,
        ERR_CLIENT_DISCONNECTED             = 148,
        ERR_CONNECT_NOT_ALLOWED             = 149,
        ERR_CHANGE_MODE_FAILED              = 150,
        ERR_BUS_OFF                         = 151,
        ERR_COULD_NOT_TX_ADDRESS_CLAIMED    = 152,
        ERR_ADDRESS_LOST                    = 153,
        ERR_CODE_NOT_FOUND                  = 154,
        ERR_BLOCK_NOT_ALLOWED               = 155,
        ERR_MULTIPLE_CLIENTS_CONNECTED      = 156,
        ERR_ADDRESS_NEVER_CLAIMED           = 157,
        ERR_WINDOW_HANDLE_REQUIRED          = 158,
        ERR_MESSAGE_NOT_SENT                = 159,
        ERR_MAX_NOTIFY_EXCEEDED             = 160,
        ERR_MAX_FILTERS_EXCEEDED            = 161,
        ERR_HARDWARE_STATUS_CHANGE          = 162,
        ERR_INI_FILE_NOT_IN_WIN_DIR         = 202,
        ERR_INI_SECTION_NOT_FOUND           = 204,
        ERR_INI_KEY_NOT_FOUND               = 205,
        ERR_INVALID_KEY_STRING              = 206,
        ERR_DEVICE_NOT_SUPPORTED            = 207,
        ERR_INVALID_PORT_PARAM              = 208,
        ERR_COMMAND_TIMED_OUT               = 213,
        ERR_OS_NOT_SUPPORTED                = 220,
        ERR_COMMAND_QUEUE_IS_FULL           = 222,
        ERR_CANNOT_SET_CAN_BAUDRATE         = 224,
        ERR_CANNOT_CLAIM_BROADCAST_ADDRESS  = 225,
        ERR_OUT_OF_ADDRESS_RESOURCES        = 226,
        ERR_ADDRESS_RELEASE_FAILED          = 227,
        ERR_COMM_DEVICE_IN_USE              = 230,
        DG_DLL_NOT_FOUND                    = 254,
        DG_BUSY_SENDING                     = 255
    }

    public class RP121032 : IDisposable
    {
        #region RP1210A Constants
        //--------------------------------------------------------------
        // RP1210A Constants
        //--------------------------------------------------------------
        public const string RP1210_INI = "rp121032.ini";    //if any devices are installed this file should be in the system root

        public const byte BLOCKING_IO = 1;          // For Blocking calls to send/read.
        public const byte NON_BLOCKING_IO = 0;          // For Non-Blocking calls to send/read.

        public const byte CONVERTED_MODE = 1;          // RP1210Mode="Converted"
        public const byte RAW_MODE = 0;          // RP1210Mode="Raw"

        public const uint MAX_J1708_MESSAGE_LENGTH = 508;        // Maximum size a J1708 message can be (+1)
        public const uint MAX_J1939_MESSAGE_LENGTH = 1796;       // Maximum size a J1939 message can be (+1)
        public const uint MAX_J1850_MESSAGE_LENGTH = 508;        // Maximum size a J1850 message can be
        public const uint CAN_DATA_SIZE = 8;          // Maximum data size a CAN message can be

        public const byte ECHO_OFF = 0x00;       // EchoMode
        public const byte ECHO_ON = 0x01;       // EchoMode

        public const byte RECEIVE_ON = 0x01;       // Set Message Receive
        public const byte RECEIVE_OFF = 0x00;       // Set Message Receive

        public const uint FILTER_PGN = 0x00000001; // Setting of J1939 filters
        public const uint FILTER_PRIORITY = 0x00000002; // Setting of J1939 filters
        public const uint FILTER_SOURCE = 0x00000004; // Setting of J1939 filters
        public const uint FILTER_DESTINATION = 0x00000008; // Setting of J1939 filters

        public const byte SILENT_J1939_CLAIM = 0x00;       // Claim J1939 Address
        public const byte PASS_J1939_CLAIM_MESSAGES = 0x01;       // Claim J1939 Address

        public const byte STANDARD_CAN = 0x00;       // Filters
        public const byte EXTENDED_CAN = 0x01;       // Filters

        #endregion

        #region RP1210 External References
        //--------------------------------------------------------------
        // TMC RP1210A Defined Function Prototypes
        //--------------------------------------------------------------

        const string RP1210_CLIENT_CONNECT = "RP1210_ClientConnect";
        const string RP1210_CLIENT_DISCONNECT = "RP1210_ClientDisconnect";
        const string RP1210_GET_ERROR_MSG = "RP1210_GetErrorMsg";
        const string RP1210_GET_HARDWARE_STATUS = "RP1210_GetHardwareStatus";
        const string RP1210_READ_DETAILED_VERSION = "RP1210_ReadDetailedVersion";
        const string RP1210_READ_MESSAGE = "RP1210_ReadMessage";
        const string RP1210_READ_VERSION = "RP1210_ReadVersion";
        const string RP1210_SEND_COMMAND = "RP1210_SendCommand";
        const string RP1210_SEND_MESSAGE = "RP1210_SendMessage";

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate short RP1210ClientConnect(int hwndClient, short nDeviceId, StringBuilder fpchProtocol, long lSendBuffer, long lReceiveBuffer, short nIsAppPacketizingIncomingMsgs);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate short RP1210ClientDisconnect(short nClientID);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate short RP1210SendMessage(short nClientID, StringBuilder fpchClientMessage, short nMessageSize, short nNotifyStatusOnTx, short nBlockOnSend);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate short RP1210ReadMessage(short nClientID, StringBuilder fpchAPIMessage, short nBufferSize, short nBlockOnSend);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate short RP1210SendCommand(short nCommandNumber, short nClientID, StringBuilder fpchClientCommand, short nMessageSize);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void RP1210ReadVersion(StringBuilder fpchDLLMajorVersion, StringBuilder fpchDLLMinorVersion, StringBuilder fpchAPIMajorVersion, StringBuilder fpchAPIMinorVersion);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate short RP1210ReadDetailedVersion(short nClientID, StringBuilder fpchAPIVersionInfo, StringBuilder fpchDLLVersionInfo, StringBuilder fpchFWVersionInfo);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate short RP1210GetHardwareStatus(short nClientID, StringBuilder fpchClientInfo, short nInfoSize, short nBlockOnRequest);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate short RP1210GetErrorMsg(short err_code, StringBuilder fpchMessage, short nClientID);

        #endregion

        #region Local Variables
        private bool disposed = false; // Track whether Dispose has been called.
        private class Win32API
        {
            [DllImport("kernel32.dll")]
            public static extern IntPtr LoadLibrary(string dllToLoad);

            [DllImport("kernel32.dll")]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

            [DllImport("kernel32.dll")]
            public static extern bool FreeLibrary(IntPtr hModule);
        }
        private IntPtr pDLL;
        private IntPtr fpRP1210_ClientConnect;
        private IntPtr fpRP1210_ClientDisconnect;
        private IntPtr fpRP1210_SendMessage;
        private IntPtr fpRP1210_ReadMessage;
        private IntPtr fpRP1210_SendCommand;
        private IntPtr fpRP1210_ReadVersion;
        private IntPtr fpRP1210_ReadDetailedVersion;
        private IntPtr fpRP1210_GetHardwareStatus;
        private IntPtr fpRP1210_GetErrorMsg;

        private RP1210ClientConnect pRP1210_ClientConnect;
        private RP1210ClientDisconnect pRP1210_ClientDisconnect;
        private RP1210SendMessage pRP1210_SendMessage;
        private RP1210ReadMessage pRP1210_ReadMessage;
        private RP1210SendCommand pRP1210_SendCommand;
        private RP1210ReadVersion pRP1210_ReadVersion;
        private RP1210ReadDetailedVersion pRP1210_ReadDetailedVersion;
        private RP1210GetHardwareStatus pRP1210_GetHardwareStatus;
        private RP1210GetErrorMsg pRP1210_GetErrorMsg;

        #endregion

        #region Constructor
        public RP121032(string PathToRP1210DLL)
        {
            pDLL = Win32API.LoadLibrary(PathToRP1210DLL);
            //if (pDll == IntPtr.Zero)   // Error Handling

            fpRP1210_ClientConnect = Win32API.GetProcAddress(pDLL, RP1210_CLIENT_CONNECT);
            fpRP1210_ClientDisconnect = Win32API.GetProcAddress(pDLL, RP1210_CLIENT_DISCONNECT);
            fpRP1210_SendMessage = Win32API.GetProcAddress(pDLL, RP1210_SEND_MESSAGE);
            fpRP1210_ReadMessage = Win32API.GetProcAddress(pDLL, RP1210_READ_MESSAGE);
            fpRP1210_SendCommand = Win32API.GetProcAddress(pDLL, RP1210_SEND_COMMAND);
            fpRP1210_ReadVersion = Win32API.GetProcAddress(pDLL, RP1210_READ_VERSION);
            fpRP1210_ReadDetailedVersion = Win32API.GetProcAddress(pDLL, RP1210_READ_DETAILED_VERSION);
            fpRP1210_GetHardwareStatus = Win32API.GetProcAddress(pDLL, RP1210_GET_HARDWARE_STATUS);
            fpRP1210_GetErrorMsg = Win32API.GetProcAddress(pDLL, RP1210_GET_ERROR_MSG);

            pRP1210_ClientConnect = (RP1210ClientConnect)Marshal.GetDelegateForFunctionPointer(fpRP1210_ClientConnect, typeof(RP1210ClientConnect));
            pRP1210_ClientDisconnect = (RP1210ClientDisconnect)Marshal.GetDelegateForFunctionPointer(fpRP1210_ClientDisconnect, typeof(RP1210ClientDisconnect));
            pRP1210_SendMessage = (RP1210SendMessage)Marshal.GetDelegateForFunctionPointer(fpRP1210_SendMessage, typeof(RP1210SendMessage));
            pRP1210_ReadMessage = (RP1210ReadMessage)Marshal.GetDelegateForFunctionPointer(fpRP1210_ReadMessage, typeof(RP1210ReadMessage));
            pRP1210_SendCommand = (RP1210SendCommand)Marshal.GetDelegateForFunctionPointer(fpRP1210_SendCommand, typeof(RP1210SendCommand));
            pRP1210_ReadVersion = (RP1210ReadVersion)Marshal.GetDelegateForFunctionPointer(fpRP1210_ReadVersion, typeof(RP1210ReadVersion));
            pRP1210_ReadDetailedVersion = (RP1210ReadDetailedVersion)Marshal.GetDelegateForFunctionPointer(fpRP1210_ReadDetailedVersion, typeof(RP1210ReadDetailedVersion));
            pRP1210_GetHardwareStatus = (RP1210GetHardwareStatus)Marshal.GetDelegateForFunctionPointer(fpRP1210_GetHardwareStatus, typeof(RP1210GetHardwareStatus));
            pRP1210_GetErrorMsg = (RP1210GetErrorMsg)Marshal.GetDelegateForFunctionPointer(fpRP1210_GetErrorMsg, typeof(RP1210GetErrorMsg));
        }
        #endregion

        #region RP1210 Library Functions

        /// <summary>
        /// Establishes a logical client connection with the TMC RP1210 API DLL
        /// </summary>
        /// <param name="hwndClient"></param>
        /// <param name="nDeviceId"></param>
        /// <param name="fpchProtocol"></param>
        /// <param name="lSendBuffer"></param>
        /// <param name="lReceiveBuffer"></param>
        /// <param name="nIsAppPacketizingIncomingMsgs"></param>
        /// <returns>nClientID</returns>
        public short RP1210_ClientConnect(int hwndClient, short nDeviceId, StringBuilder fpchProtocol, long lSendBuffer, long lReceiveBuffer, short nIsAppPacketizingIncomingMsgs)
        {
            return pRP1210_ClientConnect(hwndClient, nDeviceId, fpchProtocol, lSendBuffer, lReceiveBuffer, nIsAppPacketizingIncomingMsgs);
        }

        /// <summary>
        /// Disconnects the logical client application from the TMC RP1210 API DLL
        /// </summary>
        /// <param name="nClientID"></param>
        /// <returns></returns>
        public short RP1210_ClientDisconnect(short nClientID)
        {
            return pRP1210_ClientDisconnect(nClientID);
        }

        /// <summary>
        /// Sends a message to the API DLL
        /// </summary>
        /// <param name="nClientID"></param>
        /// <param name="fpchClientMessage"></param>
        /// <param name="nMessageSize"></param>
        /// <param name="nNotifyStatusOnTx"></param>
        /// <param name="nBlockOnSend"></param>
        /// <returns></returns>
        public short RP1210_SendMessage(short nClientID, StringBuilder fpchClientMessage, short nMessageSize, short nNotifyStatusOnTx, short nBlockOnSend)
        {
            return pRP1210_SendMessage(nClientID, fpchClientMessage, nMessageSize, nNotifyStatusOnTx, nBlockOnSend);
        }

        /// <summary>
        /// Reads a message from the API DLL.
        /// </summary>
        /// <param name="nClientID"></param>
        /// <param name="fpchAPIMessage"></param>
        /// <param name="nBufferSize"></param>
        /// <param name="nBlockOnSend"></param>
        /// <returns></returns>
        public short RP1210_ReadMessage(short nClientID, StringBuilder fpchAPIMessage, short nBufferSize, short nBlockOnSend)
        {
            return pRP1210_ReadMessage(nClientID, fpchAPIMessage, nBufferSize, nBlockOnSend);
        }

        /// <summary>
        /// Sends a command to the API DLL for things like filtering, etc
        /// </summary>
        /// <param name="nCommandNumber"></param>
        /// <param name="nClientID"></param>
        /// <param name="fpchClientCommand"></param>
        /// <param name="nMessageSize"></param>
        /// <returns></returns>
        public short RP1210_SendCommand(short nCommandNumber, short nClientID, StringBuilder fpchClientCommand, short nMessageSize)
        {
            return pRP1210_SendCommand(nCommandNumber, nClientID, fpchClientCommand, nMessageSize);
        }

        /// <summary>
        /// Reads version information from the API, about the API.
        /// </summary>
        /// <param name="fpchDLLMajorVersion"></param>
        /// <param name="fpchDLLMinorVersion"></param>
        /// <param name="fpchAPIMajorVersion"></param>
        /// <param name="fpchAPIMinorVersion"></param>
        public void RP1210_ReadVersion(StringBuilder fpchDLLMajorVersion, StringBuilder fpchDLLMinorVersion, StringBuilder fpchAPIMajorVersion, StringBuilder fpchAPIMinorVersion)
        {
            pRP1210_ReadVersion(fpchDLLMajorVersion, fpchDLLMinorVersion, fpchAPIMajorVersion, fpchAPIMinorVersion);
        }

        /// <summary>
        /// Newer more comprehensive version information command.  This command is now preferred over the RP1210_ReadVersion call
        /// </summary>
        /// <param name="nClientID"></param>
        /// <param name="fpchAPIVersionInfo"></param>
        /// <param name="fpchDLLVersionInfo"></param>
        /// <param name="fpchFWVersionInfo"></param>
        /// <returns></returns>
        public short RP1210_ReadDetailedVersion(short nClientID, StringBuilder fpchAPIVersionInfo, StringBuilder fpchDLLVersionInfo, StringBuilder fpchFWVersionInfo)
        {
            return pRP1210_ReadDetailedVersion(nClientID, fpchAPIVersionInfo, fpchDLLVersionInfo, fpchFWVersionInfo);
        }

        /// <summary>
        /// Returns information state of the connection and data link
        /// </summary>
        /// <param name="nClientID"></param>
        /// <param name="fpchClientInfo"></param>
        /// <param name="nInfoSize"></param>
        /// <param name="nBlockOnRequest"></param>
        /// <returns></returns>
        public short RP1210_GetHardwareStatus(short nClientID, StringBuilder fpchClientInfo, short nInfoSize, short nBlockOnRequest)
        {
            return pRP1210_GetHardwareStatus(nClientID, fpchClientInfo, nInfoSize, nBlockOnRequest);
        }

        /// <summary>
        /// Translates an RP1210 error code into a textual description of the error
        /// </summary>
        /// <param name="err_code"></param>
        /// <param name="fpchMessage"></param>
        /// <param name="nClientID"></param>
        /// <returns></returns>
        public short RP1210_GetErrorMsg(short err_code, StringBuilder fpchMessage, short nClientID)
        {
            return pRP1210_GetErrorMsg(err_code, fpchMessage, nClientID);
        }
        #endregion

        /// <summary>
        /// Looks for installed devices that support the RP1210 API. According to TMC RP1210 the file rp121032.ini should
        /// exist in the %SystemRoot% directory with a list of devices. Those device names should match a cooresponding dll
        /// in the system32 directory. There may also exist an ini file using the same device name.
        /// </summary>
        /// <returns>List&lt;string&gt; of device names that are able to service the RP1210 API</returns>
        public static List<string> ScanForDrivers()
        {
            List<string> returnList;
            string temp = Environment.GetEnvironmentVariable("SystemRoot");
            temp += "\\" + RP1210_INI;
            try
            {
                IniParser parser = new IniParser(temp);
                string temp2 = parser.GetSetting("RP1210Support", "APIImplementations");
                returnList = new List<string>(temp2.Split(new char[] { ',' }));
                return returnList;
            }
            catch
            {
                return null;
            }
        }

        #region Garbage Collection
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // Release unmanaged resources.
                bool result = Win32API.FreeLibrary(pDLL);
                if (!result)   //Problem in closing and releasing the resources.
                {
                    //throw
                }
                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.
            }
            disposed = true;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~RP121032()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose();
        }
        #endregion
    }
}