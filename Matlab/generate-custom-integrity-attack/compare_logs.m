clc; clear; close all;

% READ FILES
LOG_s = [];
LOG_r = [];
fid_s = fopen('log_sent.txt');
fid_r = fopen('log_received.txt');

tline_s = fgetl(fid_s);
tline_r = fgetl(fid_r);
while ischar(tline_s)
    %disp(tline_s)
    LOG_s = [LOG_s; string(tline_s)];
    tline_s = fgetl(fid_s);
end
fclose(fid_s);

while ischar(tline_r)
    %disp(tline_r)
    LOG_r = [LOG_r; string(tline_r)];
    tline_r = fgetl(fid_r);
end
fclose(fid_r);

% FIND FIRST AND LAST MATCHING PACKAGES
flag = 0;
idx_align_s = 0;
idx_align_r = 0;
for i = 1:length(LOG_s)
    for j = 1:length(LOG_r)
        if LOG_s(i) == LOG_r(j)
            if (flag == 0)
                idx_align_s = i
                idx_align_r = j
                flag = 1   
            else 
                idx_last_s = i;
                idx_last_r = j;
            end          
        end
    end
end

% FIND MATCH / MIS-MATCH
scan_len = max([length(LOG_s) - idx_align_s], [length(LOG_r) - idx_align_r])
match_status = zeros(1,scan_len);
pointer = idx_align_r
stop = 0
for i = 1:scan_len
    if ((idx_align_s + i - 1 > idx_last_s) || (pointer > idx_last_r))
        stop = i - 1
        break
    end
    
    if LOG_s(idx_align_s + i - 1) == LOG_r(pointer)
        disp('MATCH')
        pointer = pointer + 1;
        match_status(i) = 1;
    else
        disp('MIS-MATCH')
    end
end

n_match = length(find(match_status(1:stop) == 1))
n_mismatch = length(find(match_status(1:stop) == 0))
p_pass = n_match/(n_match + n_mismatch)

% VIZUALIZE
figure
hold on
plot(1:stop, match_status(1:stop), 'LineWidth', 2.5)
xlabel('Package number (sender)', 'FontSize', 12)
ylabel('Package status', 'FontSize', 12)
set(gcf,'color','w');
box on
