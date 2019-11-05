# ECO-2375: VOCSN Configuration File Update Program (MFG-00536)
## Description
This program is designed to perform the VOCSN configuration file update needed to update the control boards from PRT-00875-024 to PRT-00875-025. This adds an entry to the configuration file that determines how the VOCSN software handles blower controls.



## Installation
* Run the MFG_00536_A.msi file
* Run the icalsetup.exe file
* Open InstaCal from the start menu with the GPIO controller installed
* Close InstaCal
* Open the dhcpsrv.exe file. Hit "Install", hit "Start"
* Run MFG_00536_A.exe


## Dependencies
To run this program the following libraries must be installed
* MccDaq.dll

To run this program the following extra programs must be installed
* Instacal (MccDaq standalone program, this is needed to properly setup the required GPIO module) The program must be run once with the GPIO module connected to create the necessary CB.CFG file in C:\Program Data\Measurement Computing\CB.CFG
* DHCPSRV (DHCP Server program used )
