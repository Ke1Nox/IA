using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interfaz base para todos los Steering Behaviours.
/// Cada behaviour recibe la velocidad actual y devuelve la nueva direccion deseada.
/// </summary>
public interface ISteering
{
    Vector3 GetDir(Vector3 currentSpeed);
}

/// <summary>
/// Utilidades de aleatoriedad para IA.
/// </summary>
public class MyRandom
{
    /// <summary>
    /// Roulette Wheel Selection: selecciona un elemento al azar
    /// segun su peso relativo dentro del diccionario.
    /// Cuanto mayor el peso, mayor la probabilidad de ser elegido.
    /// Ejemplo: { "Patrol": 60f, "Idle": 30f, "RunAway": 10f }
    /// </summary>
    public static T RouletteWheelSelection<T>(Dictionary<T, float> elements)
    {
        // Sumamos todas las probabilidades para normalizarlas
        float totalChance = 0;
        foreach (var elem in elements.Values)
            totalChance += elem;

        // Elegimos un valor aleatorio entre 0 y la suma total
        float randomValue = Random.Range(0f, totalChance);

        // Recorremos restando pesos hasta encontrar el elegido
        foreach (var elem in elements)
        {
            randomValue -= elem.Value;
            // CORRECTO: chequeamos randomValue (no totalChance como en el original)
            if (randomValue <= 0)
                return elem.Key;
        }

        // Fallback por precision flotante: devolvemos el ultimo elemento
        T last = default;
        foreach (var elem in elements)
            last = elem.Key;
        return last;
    }
}
