n_pass = 0;
n_drop = 0;
n_switches = 0;
n_on = 0;
dp = 0.20;
pd = 0.80;

state = 1;
for i = 1:10000000
    x = rand(1);
    if (state == 1 && x < pd)
        state = 0;
        n_switches = n_switches + 1;
    elseif (state == 0 && x < dp)
        state = 1;
    end
    
    if (state == 1)
        n_pass = n_pass + 1;
    elseif (state == 0)
        n_drop = n_drop + 1;
    end
end

n_drop
n_on
n_switches
avg_drop_time = 0.1*n_drop/(n_switches/2)
avg_on_time = 0.1*n_pass/(n_switches/2)