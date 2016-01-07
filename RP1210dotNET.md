# Introduction #

This wrapper is designed to allow easy access to the RP1210a & b API as installed by vendors of automotive and industrial network interface devices such the Dearborn Group, Nexiq, Vector, and Kvaser.


# Details #

The core of this library are the API functions:

RP1210\_ClientConnect - Establishes a logical client connection with the API DLL.

RP1210\_ClientDisconnect - Disconnects the logical client application from the API DLL.

RP1210\_SendCommand -Sends a command to the API DLL for things like filtering, etc.

RP1210\_SendMessage - Sends a message to the API DLL.

RP1210\_ReadMessage - Reads a message from the API DLL.

RP1210\_ReadVersion - Reads version information from the API, about the API.

RP1210\_ReadDetailedVersion - Newer more comprehensive version information command.  This command is now preferred over the RP1210\_ReadVersion call.

RP1210\_GetErrorMsg - Translates an RP1210 error code into a textual description of the error.

RP1210\_GetHardwareStatus - Returns information state of the connection and data link.