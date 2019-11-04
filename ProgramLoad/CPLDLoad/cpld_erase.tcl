# Microsemi Tcl Script
# flashpro
# Date: Fri Sep 20 15:55:36 2019
# Directory C:\Users\trogers\Desktop\VLS_SW
# File C:\Users\trogers\Desktop\VLS_SW\cpld_erase.tcl


open_project -project {C:/MFG_527/program_load/cpld_load/PowerBoard_V12.pro} -connect_programmers 1 
set_programming_action -action {ERASE} 
run_selected_actions 