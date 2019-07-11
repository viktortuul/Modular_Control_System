clc; clear; close all;

rise_time = [0.32 0.31 0.31 0.31 0.33 0.35;
             0.32 0.31 0.31 0.32 0.33 0.35;
             1.05 0.75 0.36 0.37 0.35 0.35]
         
overshoot = [27.28 13.08 15.28 8.03 8.33 6.77;
             34.81 19.37 11.88 9.50 9.18 6.68;
             12.68 5.86 10.93 9.94 9.11 7.68]
         
settling_time = [0 0 5.74 3.45 2.65 1.43;
                16.66 8.93 4.45 2.62 2.20 1.45;
                4.47 3.99 2.64 2.07 2.02 1.43]

rise_time = fliplr(rise_time);
overshoot = fliplr(overshoot);
settling_time = fliplr(settling_time);

% VIZUALIZE

figure
subplot(3,1,1)
hold on
plot((1:length(rise_time(1,:))), rise_time(1,:), 'r-o', 'LineWidth', 1)
plot((1:length(rise_time(1,:))), rise_time(2,:), 'g-*', 'LineWidth', 1)
plot((1:length(rise_time(1,:))), rise_time(3,:), 'b-v', 'LineWidth', 1)
legend('PIDstandard', 'PIDplus', 'PIDsuppress')
%xlabel('Drop-out setting', 'FontSize', 10)
ylabel('Rise time [s]', 'FontSize', 10)
title('Rise time')
ylim([0 1.2])
xticks(1:1:7)
xticklabels({'100-0','20-80', '15-85', '10-90', '7-93', '5-95'})
set(gcf,'color','w');
grid on
box on
  
%figure
subplot(3,1,2)
hold on
plot((1:length(overshoot(1,:))), overshoot(1,:), 'r-o', 'LineWidth', 1)
plot((1:length(overshoot(1,:))), overshoot(2,:), 'g-*', 'LineWidth', 1)
plot((1:length(overshoot(1,:))), overshoot(3,:), 'b-v', 'LineWidth', 1)
legend('PIDstandard', 'PIDplus', 'PIDsuppress')
legend('PIDstandard', 'PIDplus', 'PIDsuppress')
%xlabel('Drop-out setting', 'FontSize', 10)
ylabel('Overshoot [%]', 'FontSize', 10)
title('Overshoot')
ylim([0 40])
xticks(1:1:7)
xticklabels({'100-0','20-80', '15-85', '10-90', '7-93', '5-95'})
set(gcf,'color','w');
grid on
box on

%figure
subplot(3,1,3)
hold on
plot((1:4), settling_time(1,1:4), 'r-o', 'LineWidth', 1)
plot((1:length(settling_time(2,:))), settling_time(2,:), 'g-*', 'LineWidth', 1)
plot((1:length(settling_time(3,:))), settling_time(3,:), 'b-v', 'LineWidth', 1)
legend('PIDstandard', 'PIDplus', 'PIDsuppress')
legend('PIDstandard', 'PIDplus', 'PIDsuppress')
xlabel('Drop-out setting', 'FontSize', 10)
ylabel('Settling time [s]', 'FontSize', 10)
title('Settling time')
ylim([0 18])
xticks(1:1:7)
xticklabels({'100-0','20-80', '15-85', '10-90', '7-93', '5-95'})
set(gcf,'color','w');
grid on
box on


figure
hold on
plot((1:4), settling_time(1,1:4), 'r-o', 'LineWidth', 1)
plot((1:length(settling_time(2,:))), settling_time(2,:), 'g-*', 'LineWidth', 1)
plot((1:length(settling_time(3,:))), settling_time(3,:), 'b-v', 'LineWidth', 1)
legend('PIDstandard', 'PIDplus', 'PIDsuppress')
legend('PIDstandard', 'PIDplus', 'PIDsuppress')
xlabel('Drop-out setting', 'FontSize', 10)
ylabel('Settling time [s]', 'FontSize', 10)
title('Settling time')
ylim([0 18])
xticks(1:1:7)
xticklabels({'100-0','20-80', '15-85', '10-90', '7-93', '5-95'})
set(gcf,'color','w');
grid on
box on

% 5_95	Rise time	Overshoot	Settling time	
% PIDstandard	0,32	27,28	-	5,000
% PIDplus	0,32	34,81	16,66	1,000
% PIDsuppress	1,05	12,68	4,47	0,000
% 							
% 7_93	Rise time	Overshoot	Settling time	
% PIDstandard	0,31	13,08	-	5,000
% PIDplus	0,31	19,37	8,93	0,000
% PIDsuppress	0,75	5,86	3,99	0,000
% 				
% 10_90	Rise time	Overshoot	Settling time	
% PIDstandard	0,31	15,28	5,74	1,000
% PIDplus	0,31	11,88	4,45	0,000
% PIDsuppress	0,36	10,93	2,64	0,000
% 				
% 15_85	Rise time	Overshoot	Settling time	
% PIDstandard	0,31	8,03	3,29	0,000
% PIDplus	0,32	9,50	2,95	0,000
% PIDsuppress	0,39	9,94	1,80	0,000
% 				
% 20_80	Rise time	Overshoot	Settling time	
% PIDstandard	0,33	8,33	2,65	0,000
% PIDplus	0,33	9,18	2,20	0,000
% PIDsuppress	0,35	9,11	2,02	0,000
% 
% 100_0	Rise time	Overshoot	Settling time	No settling
% PIDstandard	0,35	6,77	1,43	0,000
% PIDplus	0,35	6,68	1,45	0
% PIDsuppress	0,35	6,68	1,43	0
% 				