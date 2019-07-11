clc; clear; close all;

for filedir=1:3
    % read all files that fulfills the regex below
    Files=dir('log_states_standard*');
    if (filedir == 1)
        Files=dir('log_states_standard*');
    elseif (filedir == 2)
        Files=dir('log_states_plus*');
    elseif (filedir == 3)
        Files=dir('log_states_suppres*');
    end
 
    N_files = 0;
    N_settling = 0;
    T_rise_all = [];
    Overshoot_all = [];
    T_settling_all = [];

    for i=1:length(Files)
        file_name = Files(i).name;

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
            Y = [Y; y];
            U = [U; u];
            tline = fgetl(fid);       
        end
        fclose(fid);

        if (length(Y) > 100)
            N_files = N_files + 1;
        else
            break
        end

        % RISE TIME
        T_start = 1;
        T_end = 1;
        for i=1:length(Y)
            if Y(i) > 6 + 6*0.1
                T_start = i;
                break
            end
        end
        for i=1:length(Y)
            if Y(i) > 6 + 6*0.9
                T_end = i;
                break
            end
        end
        T_rise = (T_end - T_start)/100;
        T_rise_all = [T_rise_all T_rise];

        % PEAK
        T_peak = 1;
        [M,T_peak] = max(Y(T_end:T_end+100));
        T_peak = T_peak + T_end - 1;
        Overshoot = 100*(Y(T_peak) - 12)/12;
        Overshoot_all = [Overshoot_all Overshoot];

        % SETTLING TIME
        treshold = 0.02;
        T_settling_temp = 1;
        for i=1:length(Y)
            if abs(Y(i) - 12) > 12*treshold
                T_settling_temp = i;
            end
        end
        
        if (T_settling_temp > length(Y)-50)
            T_settling_temp = 1;   
        else
            N_settling = N_settling + 1;
            T_settling = (T_settling_temp - T_start)/100;
            T_settling_all = [T_settling_all T_settling];
        end

        % VIZUALIZE
        figure
        hold on
        plot((1:length(Y))/100, Y, 'b', 'LineWidth', 1.5)
        plot((1:length(U))/100, U, 'r', 'LineWidth', 1.5)

        plot([1 length(Y)/100], [12 12], 'k--', 'LineWidth', 1)
        plot([1 length(Y)/100], [6 6], 'k--', 'LineWidth', 1)

        % plot some key-points
        %plot(T_start/100, Y(T_start), 'ok','LineWidth', 1.5)
        %plot(T_end/100, Y(T_end), 'ok','LineWidth', 1.5)
        %plot(T_peak/100, Y(T_peak), 'ok','LineWidth', 1.5)
        %plot(T_settling_temp/100, Y(T_settling_temp), 'ok','LineWidth', 2)

        legend('Process value y', 'Realized control value u')
        xlabel('Time[t]', 'FontSize', 10)
        ylabel('Magnitude', 'FontSize', 10)
        xlim([2 length(Y)/100])
        ylim([0 max(Y)])
        set(gcf,'color','w');
        box on
    end

     
    if (filedir == 1)
        disp('log_states_standard*')
    elseif (filedir == 2)
        disp('log_states_plus*');
    elseif (filedir == 3)
        disp('log_states_suppres*');
    end
    
    % average statistics
    avg_rise = sum(T_rise_all)/N_files
    avg_overshoot = sum(Overshoot_all)/N_files
    avg_settling = sum(T_settling_all)/N_settling
    no_settling = N_files - N_settling
end