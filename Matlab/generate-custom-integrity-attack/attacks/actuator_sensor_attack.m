addpath('helpers')

%% [Control & Measurement] time-series attack
u = u0*ones(1, N);                          % control signal time series
atk_start = round((T_atk_start)/dt + 1);    % attack start index
u(atk_start:end) = u_atk;                   % control signal time series

% simulate plant
x0 = [3; 5];                              % initial state vector
y = SimulatePlant(N, dt, a1, a2, x0, g, A1, A2, k, u);

% extract attack time series
y_ref = y(2,atk_start);                     % state value before attack initialized
y_atk = -(y(2,atk_start:end) - y_ref);      % output the attack - "compensation"

textsize = 12;
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