clc; clear; close all;

for i=1:3
    file_name = '';
    if (i == 1)
        file_name = 'log_states_standard_5_95_noise.txt'
    elseif (i == 2)
         file_name = 'log_states_plus_5_95_noise.txt'
    elseif (i == 3)
         file_name = 'log_states_suppress_5_95_noise.txt'
    end
    
    % READ FILES
    Y = [];
    U = [];
    fid = fopen(file_name, 'r');

    tline = fgetl(fid);
    iter = 0;
    while ischar(tline)

        %disp(tline);
        C = strsplit(tline,':');
        c1 = strrep(string(C(1)), ',', '.');
        c2 = strrep(string(C(2)), ',', '.');
        y = str2double(c1);
        u = str2double(c2);
        
        iter = iter + 1;
        if (i == 1)
            Y = [Y; y];
            U = [U; u];
        elseif (i == 2 && iter > 1) %140
            Y = [Y; y];
            U = [U; u];     
        elseif (i == 3 && iter > 1) %140
            Y = [Y; y];
            U = [U; u];
        end
       
        tline = fgetl(fid);       
    end
    fclose(fid);
    
    % VIZUALIZE
    figure
    hold on
    plot((1:length(Y))/100, Y, 'b', 'LineWidth', 1.5)
    plot((1:length(U))/100, U, 'r', 'LineWidth', 1.5)
    
    %plot([1 19], [12 12], 'k--', 'LineWidth', 1)
    plot([1 19], [6 6], 'k--', 'LineWidth', 1)
    plot([1 19], [12 12], 'k--', 'LineWidth', 1)
    plot([5.0 5.0], [0 18], 'k--', 'LineWidth', 1)
    
    leg = legend('Process value y', 'Realized control value u')
    leg.ItemTokenSize = [10,10];

    if (i == 1)
        title('(A)')
    elseif (i == 2)
         title('(B)')
    elseif (i == 3)
        title('(C)')
    end
    xlabel('Time [sec]', 'FontSize', 10)
    ylabel('Magnitude', 'FontSize', 10)
    xlim([1 18]) %15
    ylim([0 18]) %15
    set(gcf,'color','w');
    box on
end
