Start HMI\HMI\bin\Debug\HMI.exe 127.0.0.1 8111

Start Controller\Controller\bin\Debug\Controller.exe 127.0.0.1 8100 8200 127.0.0.1 8300 8400 127.0.0.1 8111 127.0.0.1 8222 PID_normal log_false
Start Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe 127.0.0.1 8400 8300 dwt 127.0.0.1 8222 10 0,3 20 0,2

Start Controller\Controller\bin\Debug\Controller.exe 127.0.0.1 8101 8201 127.0.0.1 8301 8401 127.0.0.1 8111 127.0.0.1 8222 PID_plus log_false
Start Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe 127.0.0.1 8401 8301 dwt 127.0.0.1 8222 10 0,3 10 0,2

Start Canal_GUI\Canal_GUI\bin\Debug\Canal_GUI.exe 8111 0
Start Canal_GUI\Canal_GUI\bin\Debug\Canal_GUI.exe 8222 0

