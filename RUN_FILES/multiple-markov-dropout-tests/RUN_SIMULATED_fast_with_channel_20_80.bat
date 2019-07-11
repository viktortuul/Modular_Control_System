REM Start ..\HMI\HMI\bin\Debug\HMI.exe channel_controller=127.0.0.1:8111 pid_params=1,0:0,3:0,0
Start ..\..\HMI\HMI\bin\Debug\HMI.exe channel_controller=127.0.0.1:8111 pid_params=1,0:0,3:0,0 plant_animation=dwt

Start ..\..\Controller\Controller\bin\Debug\Controller.exe hmi_ep=127.0.0.1:8100:8200 plant_ep=127.0.0.1:8300:8400 channel_hmi=127.0.0.1:8111 channel_plant=127.0.0.1:8222 controller=PID_STANDARD control_range=0:7,5 log=false
Start ..\..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8400:8300 channel_controller=127.0.0.1:8222 model=dwt:5,0:0,3:5,0:0,2:0,0001 log=true log_file_name=standard_20_80

Start ..\..\Controller\Controller\bin\Debug\Controller.exe hmi_ep=127.0.0.1:8101:8201 plant_ep=127.0.0.1:8301:8401 channel_hmi=127.0.0.1:8111 channel_plant=127.0.0.1:8222 controller=PID_PLUS control_range=0:7,5 log=false
Start ..\..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8401:8301 channel_controller=127.0.0.1:8222 model=dwt:5,0:0,3:5,0:0,2:0,0001 log=true log_file_name=plus_20_80

Start ..\..\Controller\Controller\bin\Debug\Controller.exe hmi_ep=127.0.0.1:8102:8202 plant_ep=127.0.0.1:8302:8402 channel_hmi=127.0.0.1:8111 channel_plant=127.0.0.1:8222 controller=PID_SUPPRESS control_range=0:7,5 log=false
Start ..\..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8402:8302 channel_controller=127.0.0.1:8222 model=dwt:5,0:0,3:5,0:0,2:0,0001 log=true log_file_name=suppress_20_80

Start ..\..\Channel_GUI\Channel_GUI\bin\Debug\Channel_GUI.exe port_receive=8111 bernoulli=10
Start ..\..\Channel_GUI\Channel_GUI\bin\Debug\Channel_GUI.exe port_receive=8222 markov=20:80
