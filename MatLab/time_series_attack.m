clear all;
clc;
close all;

textsize = 12;

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

% plotting
% opt = stepDataOptions('InputOffset',0, 'StepAmplitude',5);
% [y,t] = step(sys,opt);
% figure
% plot(t,y)


%% [Control & Measurement] time-series attack
T = 30;                 % total simulation time
T_atk_start = 5;        % attack start time
dt = 0.1;               % simulation time interval
N = T/dt;               % number of discrete time steps
t = dt*(0:N-1);         % time axis

% attack parameters
u0 = 4.4;               % steady state control signal
u = u0*ones(1, N);      % control signal time series

% control signal attack
u_atk = 5;                                  % attack control signal
atk_start = round((T_atk_start)/dt + 1);    % attack start index
u(atk_start:end) = u_atk;                   % attack control signal time series

% simulate plant
x0 = [2.6; 4];                              % initial state vector
y = SimulatePlant(N, dt, a1, a2, x0, g, A1, A2, k, u);

% extract attack time series
y_ref = y(2,atk_start);                     % state value before attack initialized
y_atk = -(y(2,atk_start:end) - y_ref);      % output the attack - "compensation"
        

figure
subplot(2,1,1)
hold on
yyaxis left; ylim([3.5 6.5])
plot(t,y(2,:), 'b', 'LineWidth', 2)
plot([t(atk_start) t(end)], [y_ref y_ref], '--b', 'LineWidth', 2)
yyaxis right; ylim([3 6])
plot(t,u, 'r')
lgd = legend('y', '$\tilde{y}_{desired}$', '$\tilde{u}$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Plant simulation')
xlabel('Time [s]')

subplot(2,1,2)
hold on
plot(t(atk_start:end), y_atk, 'b', 'LineWidth', 2)
plot(t,u - u0, 'r', 'LineWidth', 2)
lgd = legend('$a_y$', '$a_u$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Integrity attack')
xlabel('Time [s]')


%% [reference & Measurement] time-series attack
clear y

T = 120;                 % total simulation time
T_atk_start = 60;        % attack start time
dt = 0.1;               % simulation time interval
N = T/dt;               % number of discrete time steps
t = dt*(0:N-1);         % time axis

% attack parameters
r0 = 3;               % steady state control signal
r = r0*ones(1, N);      % control signal time series

% reference signal attack
r_atk = 4;                                  % attack control signal
atk_start = round((T_atk_start)/dt + 1);    % attack start index
r(atk_start:end) = r_atk;                   % attack control signal time series

% controller variables and parameters
I = 0;          % error integral
e = 0; ep = 0;  % current and prior error
de = 0;         % error derivative
de_temp = 0;    % error derivative holder
u = 0;          % control signal
Kp = 1;             % proportional
Ki = 0.2;             % integral
Kd = 0;             % derivative
u_max = 7.5;
u_min = -7.5 * 0;

variables = struct('I', I, 'e', e, 'ep', ep, 'de', de, 'de_temp', de_temp, 'u', u);
parameters = struct('Kp', Kp, 'Ki', Ki, 'Kd', Kd, 'u_min', u_min, 'u_max', u_max);

x0 = [0; 0];                              % initial state vector
y = x0;
for i = 1:N-1
    y_temp = SimulatePlant(1, dt, a1, a2, y(1:2,i), g, A1, A2, k, variables.u);
    y(:,i+1) = y_temp;
    variables = ComputeControlSignal(r(i), y(2,i+1), dt, variables, parameters);
end

% extract attack time series
y_ref = y(2,atk_start);                     % state value before attack initialized
y_atk = -(y(2,atk_start:end) - y_ref);      % output the attack - "compensation"
        
figure
subplot(2,1,1)
xlim([20 120]); ylim([2 5])
hold on
plot(t,y(2,:), 'b', 'LineWidth', 2)
plot([t(atk_start) t(end)], [y_ref y_ref], '--b', 'LineWidth', 2)
plot(t,r, 'r')
lgd = legend('y', '$\tilde{y}_{desired}$', '$\tilde{r}$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Plant simulation')
xlabel('Time [s]')

subplot(2,1,2)
xlim([20 120]); ylim([-2 2])
hold on
plot(t(atk_start:end), y_atk, 'b', 'LineWidth', 2)
plot(t,r - r0, 'r', 'LineWidth', 2)
lgd = legend('$a_y$', '$a_r$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Integrity attack')
xlabel('Time [s]')
