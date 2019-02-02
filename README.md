# Modular_Control_System
This project is conducted at the department of automatic control at KTH, Stockholm. The project is to develop a simulation environment for a modular and networked control system. The purpose of the simulation environment is to realistically study attack and defence mechanisms, both well known and to develop new ones. Several modules make up the whole system and are each described in sections below. In total there are four modules: HMI, Control, Plant, and Channel.

An overview of the modular control system is presented below, where two communication channels enable communication between the modules. The system can be scaled up as an arbitrary number of plants can be controlled in parallel from one single HMI, illustrated in the figure.

![mcs_basic_overview](https://user-images.githubusercontent.com/25713113/52147441-03474480-2667-11e9-9cef-200f0cba1618.png)

## Modules
### HMI
### Control
### Plant
### Canal

## Executing modules
The modules are executed through the command line. Examples on how to execute the modules are presented below.

### With Channel module
#### HMI
When the channel module is used, the IP address and port of the Channel module between the HMI and Control module must be specified, e.g.
```
Start HMI\HMI\bin\Debug\HMI.exe 127.0.0.1 8111
```
The IP:port pair of the control module(s) are specified in the GUI.

#### Control
When executing the Control module, the IP:port pairs for both Channel modules must be specified, e.g. `canal_gui=127.0.0.1:8111` and `canal_gui=127.0.0.1:8222`.

Furthermore the end-points of the HMI and plant modules must be specified, e.g. `gui_ep=127.0.0.1:8100:8200` and `plant_ep=127.0.0.1:8300:8400`, where the second port entry dentoes to receiving port on the Control module.

The controller type and logging flag must also be specified, e.g. `controller=PID_normal` and `log=log_false`

Below is an example of how the Controller module can be executed.
```
Start Controller\Controller\bin\Debug\Controller.exe gui_ep=127.0.0.1:8100:8200 plant_ep=127.0.0.1:8300:8400 canal_gui=127.0.0.1:8111 canal_plant=127.0.0.1:8222 controller=PID_normal log=log_false
```

#### Plant
The Plant module requires three arguments in total when using the Channel module. Firstly, the Control module end-point must be specified, along with the Channel module IP:port pair, e.g. `controller_ep=127.0.0.1:8400:8300` and `canal_controller=127.0.0.1:8222` respectively. Furthermore, the parameters for the simulated physical model must be specified, e.g. `model=dwt:5,0:0,3:5,0:0,2:0,00001`.

Below is an example of how the Plant module can be executed.
```Start Model_GUI\Model_GUI\bin\Debug\Model_GUI.exe controller_ep=127.0.0.1:8400:8300 canal_controller=127.0.0.1:8222 model=dwt:5,0:0,3:5,0:0,2:0,00001```

#### Channel
When executing the Channel module the receiving port must be specified. Optionally, drop-out parameters for the Markov model can be specified.

Below is an example of how the Channel module can be executed.
```Start Canal_GUI\Canal_GUI\bin\Debug\Canal_GUI.exe 8111 99 80```
