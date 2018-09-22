%% [Reference & Measurement] time-series attack
r = r0*ones(1, N);                          % normal operating set-point (vector)
atk_start = round((T_atk_start)/dt + 1);    % attack start index
r(atk_start:end) = r_atk;                   % set-point time series

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

textsize = 12;
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
