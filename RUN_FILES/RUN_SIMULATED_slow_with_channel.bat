Start ..\HMI\HMI\bin\Debug\HMI.exe channel_controller=127.0.0.1:8111 pid_params=0,5:0,05:2,0

REM Start ..\Controller\Controller\bin\Debug\Controller.exe gui_ep=127.0.0.1:8100:8200 plant_ep=127.0.0.1:8300:8400 channel_gui=127.0.0.1:8111 channel_plant=127.0.0.1:8222 controller=PID_normal log=false
REM Start ..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8400:8300 channel_controller=127.0.0.1:8222 model=dwt:20,0:0,3:20,0:0,2:0,0001 log=false

Start ..\Controller\Controller\bin\Debug\Controller.exe gui_ep=127.0.0.1:8101:8201 plant_ep=127.0.0.1:8301:8401 channel_gui=127.0.0.1:8111 channel_plant=127.0.0.1:8222 controller=PID_plus log=false
Start ..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8401:8301 channel_controller=127.0.0.1:8222 model=dwt:20,0:0,3:20,0:0,2:0,0001 log=false

Start ..\Channel_GUI\Channel_GUI\bin\Debug\Channel_GUI.exe port_receive=8111 bernoulli=90
Start ..\Channel_GUI\Channel_GUI\bin\Debug\Channel_GUI.exe port_receive=8222 markov=80:98


