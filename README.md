# Control Board Test Fixture (MFG-00527)
## Description
This program is designed to perform a functional test of control boards. The test will program the board, perform the test, and update the software.
The test results are stored on the SD card of the device and in a remote database.


## Dependencies
To run this program the following libraries must be installed
* MccDaq.dll

To run this program the following programs must be installed
* Instacal (MccDaq standalone program, this is needed to properly setup the required GPIO module)
* Microsemi Flashpro (used to flash the CPLD firmware)
* Texas Instruments Uniflash (Used to install the Hercules software)
