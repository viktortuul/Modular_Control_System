Start ..\HMI\HMI\bin\Debug\HMI.exe canal_controller=127.0.0.1:8111

Start ..\Controller\Controller\bin\Debug\Controller.exe gui_ep=127.0.0.1:8100:8200 plant_ep=127.0.0.1:8300:8400 canal_gui=127.0.0.1:8111 canal_plant=127.0.0.1:8222 controller=PID_normal log=false
Start ..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8400:8300 canal_controller=127.0.0.1:8222 model=dwt:5,0:0,3:5,0:0,2:0,00001 log=false

Start ..\Controller\Controller\bin\Debug\Controller.exe gui_ep=127.0.0.1:8101:8201 plant_ep=127.0.0.1:8301:8401 canal_gui=127.0.0.1:8111 canal_plant=127.0.0.1:8222 controller=PID_plus log=false
Start ..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8401:8301 canal_controller=127.0.0.1:8222 model=dwt:5,0:0,3:5,0:0,2:0,00001 log=false

Start ..\Canal_GUI\Channel_GUI\bin\Debug\Canal_GUI.exe port_receive=8111 bernoulli=90
Start ..\Canal_GUI\Channel_GUI\bin\Debug\Canal_GUI.exe port_receive=8222 markov=80:98


