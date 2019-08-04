clear all; close all; clc;

% double tank cross section areas
A1 = 15;    % cm^2 upper tank
A2 = 50;    % cm^2 lower tank
a1 = 0.3;   % cm^2 upper tank outlet
a2 = 0.2;   % cm^2 lower tank outlet

% operating actuator proportional constant for pump
k = 6.5;    % [cm^3/Vs]

% gravitational constant
g = 981;    % cm/s^2

% controller variables and parameters
I = 0;              % error integral
e = 0; ep = 0;      % current and prior error
de = 0;             % error derivative
dE = 0;             % error derivative (low pass)
de_temp = 0;        % error derivative holder
u = 0;              % control signal
Kp = 1.5;           % proportional
Ki = 0.1;           % integral
Kd = 2;             % derivative
u_max = 7.5;        % control signal constraint
u_min = -7.5 * 0;   % control signal constraint
q = 0.01;           % FIR smoothing parameter

% global attack and simulation settings
T = 180;            % total simulation time
T_atk_start = 60;   % attack start time
dt = 0.1;           % simulation time interval
N = T/dt;           % number of discrete time steps
t = dt*(0:N-1);     % time vector

%% Control signal + Sensor attack
u0 = 3.3;   % normal operating set-point
u_atk = 5;  % attack control signal
run('attacks/actuator_sensor_attack.m')

%% Reference + Sensor attack
r0 = 6;     % normal operating set-point
r_atk = 10;	% attack set-point
run('attacks/reference_sensor_attack.m')

%% Reference + Sensor + Actuator signal attack
r0 = 6;  	% normal operating set-poin
r_atk = 10; % attack set-point
run('attacks/reference_sensor_actuator_attack.m')



