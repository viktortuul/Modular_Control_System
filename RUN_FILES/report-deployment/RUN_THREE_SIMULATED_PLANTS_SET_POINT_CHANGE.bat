Start ..\..\HMI\HMI\bin\Debug\HMI.exe channel_controller=127.0.0.1:8111 config_file=config_hmi.json

Start ..\..\Controller\Controller\bin\Debug\Controller.exe hmi_ep=127.0.0.1:8100:8200 plant_ep=127.0.0.1:8300:8400 channel_hmi=127.0.0.1:8111 channel_plant=127.0.0.1:8222 config_file=config_setpoint_1.json
Start ..\..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8400:8300 channel_controller=127.0.0.1:8222 config_file=config_setpoint_1.json

Start ..\..\Controller\Controller\bin\Debug\Controller.exe hmi_ep=127.0.0.1:8101:8201 plant_ep=127.0.0.1:8301:8401 channel_hmi=127.0.0.1:8111 channel_plant=127.0.0.1:8222 config_file=config_setpoint_2.json
Start ..\..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8401:8301 channel_controller=127.0.0.1:8222 config_file=config_setpoint_2.json

Start ..\..\Controller\Controller\bin\Debug\Controller.exe hmi_ep=127.0.0.1:8102:8202 plant_ep=127.0.0.1:8302:8402 channel_hmi=127.0.0.1:8111 channel_plant=127.0.0.1:8222 config_file=config_setpoint_3.json
Start ..\..\Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8402:8302 channel_controller=127.0.0.1:8222 config_file=config_setpoint_3.json

Start ..\..\Channel_GUI\Channel_GUI\bin\Debug\Channel_GUI.exe port_receive=8111 bernoulli=10
Start ..\..\Channel_GUI\Channel_GUI\bin\Debug\Channel_GUI.exe port_receive=8222 markov=10:90
