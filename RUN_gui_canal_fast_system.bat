Start HMI\HMI\bin\Debug\HMI.exe 127.0.0.1 8111

REM Start Controller\Controller\bin\Debug\Controller.exe 127.0.0.1 8100 8200 127.0.0.1 8300 8400 127.0.0.1 8111 127.0.0.1 8222 
Start Controller\Controller\bin\Debug\Controller.exe gui_ep=127.0.0.1:8100:8200 plant_ep=127.0.0.1:8300:8400 canal_gui=127.0.0.1:8111 canal_plant=127.0.0.1:8222 controller=PID_normal log=log_false

REM Start Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe 127.0.0.1 8400 8300 dwt 127.0.0.1 8222 5 0,3 5 0,2 0,00001
Start Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8400:8300 canal_controller=127.0.0.1:8222 model=dwt:5,0:0,3:5,0:0,2:0,00001

REM Start Controller\Controller\bin\Debug\Controller.exe 127.0.0.1 8101 8201 127.0.0.1 8301 8401 127.0.0.1 8111 127.0.0.1 8222 PID_plus log_false
Start Controller\Controller\bin\Debug\Controller.exe gui_ep=127.0.0.1:8101:8201 plant_ep=127.0.0.1:8301:8401 canal_gui=127.0.0.1:8111 canal_plant=127.0.0.1:8222 controller=PID_plus log=log_false

REM Start Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe 127.0.0.1 8401 8301 dwt 127.0.0.1 8222 5 0,3 5 0,2 0,00001
Start Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8401:8301 canal_controller=127.0.0.1:8222 model=dwt:5,0:0,3:5,0:0,2:0,00001

Start Canal_GUI\Canal_GUI\bin\Debug\Canal_GUI.exe 8111 99 80
Start Canal_GUI\Canal_GUI\bin\Debug\Canal_GUI.exe 8222 99 80


