% double water tank
function y = SimulatePlant(N, dt, a1, a2, x, g, A1, A2, k, u)
    for i = 1:N  
        q_out1 = a1 * sqrt(2 * x(1) * g);
        q_out2 = a2 * sqrt(2 * x(2) * g);
        x(1) = x(1) + dt * (1 / A1) * (k * u(i) - q_out1);
        x(2) = x(2) + dt * (1 / A2) * (q_out1 - q_out2);  
        y(1:2,i) = x;
    end
end

