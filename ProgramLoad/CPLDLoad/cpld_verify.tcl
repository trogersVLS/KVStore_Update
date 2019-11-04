# Microsemi Tcl Script
# flashpro
# Date: Fri Sep 20 15:55:20 2019
# Directory C:\Users\trogers\Desktop\VLS_SW



open_project -project {PowerBoard_V12.pro} -connect_programmers 1 
set_programming_action -action {VERIFY} 
run_selected_actions 
