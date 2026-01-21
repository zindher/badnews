using System;
using System.Collections.Generic;
using System.Linq;

namespace BadNews.Services;

/// <summary>
/// Servicio para manejo de zonas horarias de México
/// Mapeo directo: Estado → Zona Horaria
/// </summary>
public interface ITimezoneService
{
    string GetTimezoneByState(string state);
    TimeSpan GetTimezoneOffset(string timezone);
    (string time, int dayOffset, string dayLabel) ConvertTimeToTimezone(string timeString, string targetTimezone);
    bool IsValidCallTime(string timeString);
    TimezoneInfo GetTimezoneInfo(string timezone);
}

public class TimezoneService : ITimezoneService
{
    private readonly Dictionary<string, TimezoneInfo> _timezones = new()
    {
        {
            "CENTRO", new TimezoneInfo
            {
                Name = "Centro (UTC-6)",
                Offset = -6,
                States = new[] { "Aguascalientes", "Ciudad de México", "Hidalgo", "Morelos", "Puebla", "Querétaro", "San Luis Potosí", "Tamaulipas", "Tlaxcala", "Veracruz", "Oaxaca", "Nuevo León" }
            }
        },
        {
            "MONTANA", new TimezoneInfo
            {
                Name = "Montaña (UTC-7)",
                Offset = -7,
                States = new[] { "Chihuahua Oeste", "Durango" }
            }
        },
        {
            "PACIFICO", new TimezoneInfo
            {
                Name = "Pacífico (UTC-7)",
                Offset = -7,
                States = new[] { "Nayarit", "Sinaloa", "Jalisco", "Michoacán", "Guanajuato", "Zacatecas", "Coahuila Oeste", "Chihuahua Este", "Guerrero", "Colima" }
            }
        },
        {
            "NOROESTE", new TimezoneInfo
            {
                Name = "Noroeste (UTC-8)",
                Offset = -8,
                States = new[] { "Baja California", "Sonora" }
            }
        },
        {
            "QUINTANA_ROO", new TimezoneInfo
            {
                Name = "Quintana Roo (UTC-5)",
                Offset = -5,
                States = new[] { "Quintana Roo", "Yucatán", "Campeche", "Tabasco" }
            }
        }
    };

    // Mapeo directo: Estado → Zona Horaria
    private readonly Dictionary<string, string> _statesToTimezone = new()
    {
        // Centro (UTC-6)
        { "Aguascalientes", "CENTRO" },
        { "Ciudad de México", "CENTRO" },
        { "Hidalgo", "CENTRO" },
        { "Morelos", "CENTRO" },
        { "Puebla", "CENTRO" },
        { "Querétaro", "CENTRO" },
        { "San Luis Potosí", "CENTRO" },
        { "Tamaulipas", "CENTRO" },
        { "Tlaxcala", "CENTRO" },
        { "Veracruz", "CENTRO" },
        { "Oaxaca", "CENTRO" },
        { "Nuevo León", "CENTRO" },

        // Montaña (UTC-7)
        { "Chihuahua Oeste", "MONTANA" },
        { "Durango", "MONTANA" },

        // Pacífico (UTC-7)
        { "Nayarit", "PACIFICO" },
        { "Sinaloa", "PACIFICO" },
        { "Jalisco", "PACIFICO" },
        { "Michoacán", "PACIFICO" },
        { "Guanajuato", "PACIFICO" },
        { "Zacatecas", "PACIFICO" },
        { "Coahuila Oeste", "PACIFICO" },
        { "Chihuahua Este", "PACIFICO" },
        { "Guerrero", "PACIFICO" },
        { "Colima", "PACIFICO" },

        // Noroeste (UTC-8)
        { "Baja California", "NOROESTE" },
        { "Sonora", "NOROESTE" },

        // Quintana Roo (UTC-5)
        { "Quintana Roo", "QUINTANA_ROO" },
        { "Yucatán", "QUINTANA_ROO" },
        { "Campeche", "QUINTANA_ROO" },
        { "Tabasco", "QUINTANA_ROO" }
    };

    /// <summary>
    /// Obtiene la zona horaria basada en el estado
    /// </summary>
    public string GetTimezoneByState(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return "CENTRO";

        return _statesToTimezone.ContainsKey(state) ? _statesToTimezone[state] : "CENTRO";
    }

    /// <summary>
    /// Calcula el offset (diferencia) entre Aguascalientes y otra zona
    /// </summary>
    public TimeSpan GetTimezoneOffset(string timezone)
    {
        if (!_timezones.ContainsKey(timezone))
            return TimeSpan.Zero;

        var tz = _timezones[timezone];
        const int aguascalientesOffset = -6; // UTC-6
        var offsetHours = tz.Offset - aguascalientesOffset;

        return TimeSpan.FromHours(offsetHours);
    }

    /// <summary>
    /// Convierte una hora de Aguascalientes a otra zona horaria
    /// </summary>
    public (string time, int dayOffset, string dayLabel) ConvertTimeToTimezone(string timeString, string targetTimezone)
    {
        if (!TimeSpan.TryParse(timeString, out var time))
            return ("00:00", 0, "");

        var offset = GetTimezoneOffset(targetTimezone);
        var newTime = time.Add(offset);

        int dayOffset = 0;
        if (newTime.TotalHours < 0)
        {
            newTime = newTime.Add(TimeSpan.FromHours(24));
            dayOffset = -1;
        }
        else if (newTime.TotalHours >= 24)
        {
            newTime = newTime.Subtract(TimeSpan.FromHours(24));
            dayOffset = 1;
        }

        var formattedTime = newTime.ToString(@"hh\:mm");
        var dayLabel = dayOffset == -1 ? "(día anterior)" : dayOffset == 1 ? "(día siguiente)" : "";

        return (formattedTime, dayOffset, dayLabel);
    }

    /// <summary>
    /// Valida que el horario sea válido (no después de las 21:00)
    /// </summary>
    public bool IsValidCallTime(string timeString)
    {
        if (!TimeSpan.TryParse(timeString, out var time))
            return false;

        var maxTime = TimeSpan.FromHours(21); // 9 PM
        return time <= maxTime;
    }

    /// <summary>
    /// Obtiene la información de una zona horaria
    /// </summary>
    public TimezoneInfo GetTimezoneInfo(string timezone)
    {
        return _timezones.ContainsKey(timezone) ? _timezones[timezone] : _timezones["CENTRO"];
    }
}

public class TimezoneInfo
{
    public string Name { get; set; } = null!;
    public int Offset { get; set; }
    public string[] States { get; set; } = Array.Empty<string>();
}
