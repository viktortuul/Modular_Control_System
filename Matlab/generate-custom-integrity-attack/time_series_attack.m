clear all; close all; clc;
% plot settings
textsize = 12;

% Cross section areas
A1 = 15;   % cm^2 upper tank
A2 = 50;   % cm^2 lower tank
a1 = 0.3;   % cm^2 upper tank
a2 = 0.2;   % cm^2 lower tank

% operating actuator proportional constant for pump
k = 6.5; % [cm^3/Vs]

% Acceleration of gravity
g = 981;        % cm/s^2

%% [Control & Measurement] time-series attack
T = 180;                % total simulation time
T_atk_start = 60;       % attack start time
dt = 0.1;               % simulation time interval
N = T/dt;               % number of discrete time steps
t = dt*(0:N-1);         % time axis

% attack parameters
u0 = 3.3;               % steady state control signal
u = u0*ones(1, N);      % control signal time series

% control signal attack
u_atk = 5;                                  % attack control signal
atk_start = round((T_atk_start)/dt + 1);    % attack start index
u(atk_start:end) = u_atk;                   % attack control signal time series

% simulate plant
x0 = [3; 5];                              % initial state vector
y = SimulatePlant(N, dt, a1, a2, x0, g, A1, A2, k, u);

% extract attack time series
y_ref = y(2,atk_start);                     % state value before attack initialized
y_atk = -(y(2,atk_start:end) - y_ref);      % output the attack - "compensation"
        
figure
subplot(2,1,1)
xlim([50 T])
hold on
yyaxis left; ylim([4 15])
plot(t,y(2,:), 'b', 'LineWidth', 2)
plot([t(atk_start) t(end)], [y_ref y_ref], '--b', 'LineWidth', 2)
yyaxis right; ylim([1 5])
plot(t,u, 'r', 'LineWidth', 2)
lgd = legend('y', '$\tilde{y}_{desired}$', '$\tilde{u}$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Plant simulation')
xlabel('Time [s]')

subplot(2,1,2)
xlim([50 T])
hold on
plot(t(atk_start:end), y_atk, 'b', 'LineWidth', 2)
plot(t,u - u0, 'r', 'LineWidth', 2)
lgd = legend('$a_y$', '$a_u$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Integrity attack')
xlabel('Time [s]')


%% [Reference & Measurement] time-series attack
T = 180;                 % total simulation time
T_atk_start = 60;        % attack start time
dt = 0.1;                % simulation time interval
N = T/dt;                % number of discrete time steps
t = dt*(0:N-1);          % time axis

% steady state reference signal
r0 = 6;                 % steady state control signal
r = r0*ones(1, N);      % control signal time series

% reference signal attack
r_atk = 10;                                 % attack control signal
atk_start = round((T_atk_start)/dt + 1);    % attack start index
r(atk_start:end) = r_atk;                   % attack control signal time series

% controller variables and parameters
I = 0;          % error integral
e = 0; ep = 0;  % current and prior error
de = 0;         % error derivative
dE = 0;         % error derivative (low pass)
de_temp = 0;    % error derivative holder
u = 0;          % control signal
Kp = 1.5;       % proportional
Ki = 0.1;       % integral
Kd = 2;         % derivative
u_max = 7.5;
u_min = -7.5 * 0;
q = 0.01;       % FIR smoothing parameter

variables = struct('I', I, 'e', e, 'ep', ep, 'de', de, 'dE', dE, 'de_temp', de_temp, 'u', u);
parameters = struct('Kp', Kp, 'Ki', Ki, 'Kd', Kd, 'u_min', u_min, 'u_max', u_max, 'q', q);

x0 = [3; 6];    % initial state vector
y = x0;
% simulate process
for i = 1:N-1
    y_temp = SimulatePlant(1, dt, a1, a2, y(1:2,i), g, A1, A2, k, variables.u);
    y(:,i+1) = y_temp;
    variables = ComputeControlSignal(r(i), y(2,i+1), dt, variables, parameters);
end

% extract attack time series
y_0 = y(2,atk_start);                   % state value before attack initialized
y_atk = -(y(2,atk_start:end) - y_0);    % output the sensor attack - "compensation"
        
figure
subplot(2,1,1)
xlim([50 T]); ylim([2 12])
hold on
plot(t,r, 'r', 'LineWidth', 2)
plot(t,y(2,:), 'b', 'LineWidth', 2)
plot([t(atk_start) t(end)],[y_0 y_0], '--b', 'LineWidth', 2)
lgd = legend('$\tilde{r}$', 'y', '$y_{ss}$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Plant response')
xlabel('Time [s]')

subplot(2,1,2)
xlim([50 T]); ylim([-6 6])
hold on
plot(t,r - r0, 'r', 'LineWidth', 2)
plot(t(atk_start:end), y_atk, 'b', 'LineWidth', 2)
lgd = legend('$a_r$', '$a_y$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Integrity attack')
xlabel('Time [s]')

%% [Reference & Measurement & Actuator] time-series attack
T = 180;                 % total simulation time
T_atk_start = 60;        % attack start time
dt = 0.1;                % simulation time interval
N = T/dt;                % number of discrete time steps
t = dt*(0:N-1);          % time axis

% steady state reference signal
r0 = 6;                 % steady state control signal
r = r0*ones(1, N);      % control signal time series

% reference signal attack
r_atk = 10;                                 % attack control signal
atk_start = round((T_atk_start)/dt + 1);    % attack start index
r(atk_start:end) = r_atk;                   % attack control signal time series

% controller variables and parameters
I = 0;          % error integral
e = 0; ep = 0;  % current and prior error
de = 0;         % error derivative
dE = 0;         % error derivative (low pass)
de_temp = 0;    % error derivative holder
u = 0;          % control signal
Kp = 1.5;       % proportional
Ki = 0.1;       % integral
Kd = 2;         % derivative
u_max = 7.5;
u_min = -7.5 * 0;
q = 0.01;       % FIR smoothing parameter

variables = struct('I', I, 'e', e, 'ep', ep, 'de', de, 'dE', dE, 'de_temp', de_temp, 'u', u);
parameters = struct('Kp', Kp, 'Ki', Ki, 'Kd', Kd, 'u_min', u_min, 'u_max', u_max, 'q', q);

x0 = [3; 6];    % initial state vector
y = x0;
u = 0;
% simulate process
for i = 1:N-1
    y_temp = SimulatePlant(1, dt, a1, a2, y(1:2,i), g, A1, A2, k, variables.u);
    y(:,i+1) = y_temp;
    variables = ComputeControlSignal(r(i), y(2,i+1), dt, variables, parameters);
    u(i+1) = variables.u;
end

% extract attack time series
y_0 = y(2,atk_start);                   % state value before attack initialized
u_0 = u(atk_start);                     % control signal value before attack initialized
y_atk = -(y(2,atk_start:end) - y_0);    % output the sensor attack - "compensation"
u_atk = -(u(atk_start:end) - u_0);      % output the coontrol attack - "compensation"
        
figure
subplot(2,1,1)
xlim([50 T]); ylim([2 12])
hold on
plot(t,r, 'r', 'LineWidth', 2)
plot(t,y(2,:), 'b', 'LineWidth', 2)
plot(t, u, 'Color', [0.9 0.9 0], 'LineWidth', 2)
plot([t(atk_start) t(end)],[y_0 y_0], '--b', 'LineWidth', 2)
lgd = legend('$\tilde{r}$', 'y', 'u', '$y_{ss}$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Plant response')
xlabel('Time [s]')

subplot(2,1,2)
xlim([50 T]); ylim([-6 6])
hold on
plot(t,r - r0, 'r', 'LineWidth', 2)
plot(t(atk_start:end), y_atk, 'b', 'LineWidth', 2)
plot(t(atk_start:end), u_atk, 'Color', [0.9 0.9 0], 'LineWidth', 2)
lgd = legend('$a_r$', '$a_y$', '$a_u$');
set(lgd,'Interpreter','latex');
set(lgd,'FontSize',textsize);
title('Integrity attack')
xlabel('Time [s]')
u_atk = -(u(atk_start + 1:end) - u_0);      % output the coontrol attack - "compensation"