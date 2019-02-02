# Modular_Control_System
This project is conducted at the department of automatic control at KTH, Stockholm. The project is to develop a simulation environment for a modular and networked control system. The purpose of the simulation environment is to realistically study attack and defence mechanisms, both well known and to develop new ones. Several modules make up the whole system and are each described in sections below. In total there are four modules: HMI (Human Machine Interface), Control, Plant, and Channel.

An overview of the modular control system is presented below, where two communication channels enable communication between the modules. The system can be scaled up as an arbitrary number of plants and can be controlled in parallel from one single HMI, illustrated in the figure.

![mcs_basic_overview](https://user-images.githubusercontent.com/25713113/52147441-03474480-2667-11e9-9cef-200f0cba1618.png)

## Modules
### HMI
With the HMI module the user can 1) connect to new control modules, 2) tune parameters such as the reference set-point and controller parameters, 3) select which control module to monitor. In addition, details of each controller module is presented in a tree view as well as an animation of the process. The chart shows all variables and parameters associated with the control of the plant, that is, the reference set-point, control signal, state measurements, and state estimates using the Kalman filter.

Furthermore, residual and security metric graphs indicates how much the actual state measurements deviate from the estimated one. The deviation is used to quantify the probability of an attack occurring.

### Control
The control module contains PID-controllers which produces the control signals which manipulates the simulated physical process. The inputs to the control module are the reference set-point **r** and controller parameters **K_P**, **K_I**, **K_D** from the HMI module, along with the measurement signal **y** from the plant. The outputs from the control module consists of the control signal **u** to the plant module, along with the same control signal **u** and received measurement signal **y** to the HMI module. The communication from the control module to the HMI module is only for monitoring purposes. Two types of PID controllers are available in the control module: a standard PID controller, and a PIDPlus controller.

### Plant
Two plant models are included in the plant module; a single-output single-input (SISO) double water tank system, and a multi-input multi-output (MIMO) quad water tank system. In the double water tank case water is pumped into the upper tank which flows into the lower one and then exits the system. In the case of the quad water tank, two pumps pump water into the upper tanks which induce coupling between the inputs and outputs.

### Canal
The main purpose of the channel module is to enable the execution of integrity attacks and drop-out on packages send between the HMI- and control module as well as between the control- and plant module. However it is possible to run the modules without the channel module, but in that case integrity attacks are not possible, and the packages are directly sent between the modules.

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
