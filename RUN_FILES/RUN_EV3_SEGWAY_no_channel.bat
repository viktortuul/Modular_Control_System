Start ..\HMI\HMI\bin\Debug\HMI.exe pid_params=5,0:10,0:0,8
Start ..\Controller\Controller\bin\Debug\Controller.exe gui_ep=127.0.0.1:8100:8200 plant_ep=127.0.0.1:8300:8400 controller=PID_STANDARD control_range=-100:100 log=false
Start ..\EV3\EV3\bin\Debug\EV3.exe controller_ep=127.0.0.1:8400:8300 physical_process_type=SEGWAY com_port=COM7 log=false


