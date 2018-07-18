clear all;
clc;
close all;

% attack time series synthesizer
s = tf('s');

% system parameters 
% a2  [cm^2]     - effectiv outlet area of lower tank
% a1  [cm^2]     - effectiv outlet area of upper tank
% h20 [cm]       - operating level of lower tank
% h10 [cm]       - operating level of upper tank
% u20 [V]        - operating voltage of pump
% k2  [cm^3/Vs]  - operating actuator proportional constant for right pump

% Magnus Åkerblad 2000-01-20
% This file is changed by Jonas Wijk 2002-12-27, to work with the new watertanks.

% Cross section areas
A1 = 15;   % cm^2
A2 = 30;   % cm^2

a1 = 0.16 * 2.5;   % cm^2
a2 = 0.16 * 2;   % cm^2

% operating level
h10 = 2.5; % [cm]       - operating level of upper left tank
h20 = 4; % [cm]       - operating level of lower left tank

% operating actuator proportional constant for pump
k = 6.5; % [cm^3/Vs]

% Sensor proportional constant
kc = 1;       % V/cm

% Acceleration of gravity
g = 981;        % cm/s^2

% Time constants
T1 = A1/a1*sqrt(2*h10/g);   % s
T2 = A2/a2*sqrt(2*h20/g);   % s

% System matrices
A = [-1/T1      0;   
     A1/(A2*T1) -1/T2];   
B = [k/A1;
     0];  
C = [kc 0;
     0  kc];
D = zeros(2,1);

% Create state-space model
sys = ss(A,B,C,D)

% % controller
% K = 1;
% C = K*(1+s/0.2)/s; % PI-controller, cancel slow process pole ("lambda tuning")
% C = ss(C);
% 
% Go = C * sys_tf
% GC = feedback(sys, 1);
% 
opt = stepDataOptions('InputOffset',0, 'StepAmplitude',5);
[y,t] = step(sys,opt);
figure
plot(t,y)


%% [Control & Measurement] time-series attack
T = 30;                 % total simulation time
T_atk_start = 0;        % attack start time
dt = 0.1;               % simulation time interval
N = T/dt;               % number of discrete time steps
t = dt*(0:N-1);         % time axis

u = [4.4*ones(1, N)];   % steady state control signal

% attack
atk_start = round((T_atk_start)/dt + 1);    % defines in which step attack starts
u(atk_start:end) = 5; % attack control signal

x = [2.6;4]; % initial state vector

% dynamical system
for i = 1:N  
    q_out1 = a1 * sqrt(2 * x(1) * g);
    q_out2 = a2 * sqrt(2 * x(2) * g);
    x(1) = x(1) + dt * (1 / A1) * (k * u(i) - q_out1);
    x(2) = x(2) + dt * (1 / A2) * (q_out1 - q_out2);  
    y_(1:2,i) = x;
end

% extract attack time series
x2_ref = y_(2,atk_start);                   % state value before attack
atk_x2 = -(y_(2,atk_start:end) - x2_ref);   % output the attack - "compensation"
         
figure
hold on
plot(t,y_)
plot(t(atk_start:end),x2_ref + atk_x2)
xlabel('t')
