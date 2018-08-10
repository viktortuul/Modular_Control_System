function variables_ = ComputeControlSignal(r, y, dt, variables, parameters)

    I = variables.I ;
    e = variables.e;
    ep = variables.ep;
    de = variables.de;
    de_temp = variables.de_temp;
    u = variables.u;    
    u_min = parameters.u_min;
    u_max = parameters.u_max;
    Kp = parameters.Kp;
    Ki = parameters.Ki;
    Kd = parameters.Kd;

    % calculte error
    e = r - y;

    % integrator with anti wind-up (only add intergral action if the control signal is not saturated)
    if (u > u_min && u < u_max) 
        I = I + dt * e;
    else
        I = I + dt * e;
    end
    

    % derivator (with low pass)
    de = 1 / (dt + 1) * y - de_temp;
    de_temp = (y - de) / (dt + 1);

    % control signal
    u = Kp * e + Ki * I - Kd * de;
    
    ep = e; % update prior error

    % saturation
    if (u > u_max) 
        u = u_max;
    end
    if (u < u_min) 
        u = u_min;
    end
    
    variables_.I = I;
    variables_.e = e;
    variables_.ep = ep;
    variables_.de = de;
    variables_.de_temp = de_temp;
    variables_.u = u;

end