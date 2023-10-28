using System;
using System.Collections.Generic;

public class LaggedFibonacci
{
    // Новое случайное число является тем, которое было сгенерировано 7 раз назад,
    // плюс случайное число, сгенерированное 10 раз назад    
    private const int K = 10; 
    private const int J = 7; 
    private const int M = 2147483647;  // 2^31 - 1 = maxint
    private const int WarmingUp = 1000;
    private readonly List<int> _history = null;
    private readonly object _lock = new object();

    public int Seed { get; }

    public LaggedFibonacci(int seed)
    {
        Seed = seed;  
        _history = new List<int>();
        
        for (var i = 0; i < K + 1; ++i)
        {
            _history.Add(seed);
        }

        if (seed % 2 == 0)
        {
            _history[0] = 11;
        }

        for (var i = 0; i < WarmingUp; ++i) 
        {
            Next4Bytes();
        }
    } 
		
    public byte[] Next4Bytes()
    {
        lock (_lock)
        {           
            var left = _history[0] % M; 
            var right = _history[K - J] % M; 
            var sum = left + right; 
            var seed = (sum % M); 
            _history.Insert(K + 1, seed); 
            _history.RemoveAt(0); 
            var float4Bytes = ( 1.0f * seed ) / M;
            return BitConverter.GetBytes( float4Bytes );
        }
    }

    public int NextInteger()
    {
        return BitConverter.ToInt32(Next4Bytes(), 0);
    }
    
    public float NextFloat()
    {
        return BitConverter.ToSingle(Next4Bytes(), 0);
    }
}