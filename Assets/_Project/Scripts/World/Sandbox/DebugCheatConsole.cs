using System;
using System.Globalization;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using TitansOfTheSea.Ship;
using UnityEngine;

namespace TitansOfTheSea.World.Sandbox
{
    /// Sandbox-only development panel; no gameplay input or production UI ownership.
    public sealed class DebugCheatConsole : MonoBehaviour
    {
        [SerializeField] private FlatOcean _ocean;
        [SerializeField] private ShipBuoyancy[] _ships;
        private string _command = "help";
        private string _result = "Commands: help, reset, ocean flat, ocean sine, services";
        private bool Allowed => Application.isEditor || Debug.isDebugBuild;
        public void Configure(FlatOcean ocean, ShipBuoyancy[] ships) { _ocean = ocean; _ships = ships; }
        private void OnGUI()
        {
            if (!Allowed) return;
            GUILayout.BeginArea(new Rect(12, 12, 570, 270), GUI.skin.box);
            GUILayout.Label("PERSON B — PHYSICS SANDBOX / DEVELOPMENT ONLY");
            _command = GUILayout.TextField(_command);
            if (GUILayout.Button("Run command")) Execute(_command);
            GUILayout.Label(_result);
            if (Services.TryGet<ITimeOfDay>(out var time))
                GUILayout.Label($"Day {time.DayNumber} | Hour {time.Hour:F2} | Night: {time.IsNight}");
            if (Services.TryGet<IWeatherService>(out var weather))
            {
                var snapshot = weather.Current;
                GUILayout.Label($"{snapshot.Type} | Wind {snapshot.WindSpeed:F1} m/s | Rain {snapshot.Rain:F2} | Waves {snapshot.WaveScale:F2}");
            }
            GUILayout.EndArea();
        }
        public void Execute(string command)
        {
            if (!Allowed) return;
            string text = (command ?? "").Trim().ToLowerInvariant();
            // Parsing allocates only when an explicit debug command is submitted.
            string[] parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                if (parts.Length == 2 && parts[0] == "time" && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float hour))
                {
                    if (Services.Get<ITimeOfDay>() is TimeOfDayService clock) { clock.SetHour(hour); _result = "Set hour: " + hour; }
                    return;
                }
                if (parts.Length == 2 && parts[0] == "speed" && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float speed))
                {
                    if (Services.Get<ITimeOfDay>() is TimeOfDayService clock) { clock.Speed = speed; _result = "Clock speed: " + speed; }
                    return;
                }
                if (parts.Length == 2 && parts[0] == "weather")
                {
                    if (Services.Get<IWeatherService>() is WeatherService weather)
                    {
                        if (parts[1] == "auto") { weather.Automatic = true; _result = "Automatic weather enabled."; return; }
                        if (!Enum.TryParse(parts[1], true, out WeatherType type) || !Enum.IsDefined(typeof(WeatherType), type))
                            throw new ArgumentException("Use clear, cloudy, fog, rain or storm.");
                        weather.Automatic = false; weather.ForceWeather(type);
                        _result = "Weather blending toward " + type + "; automatic changes disabled.";
                    }
                    return;
                }
                if (text == "pause" || text == "resume")
                {
                    bool paused = text == "pause";
                    if (Services.Get<ITimeOfDay>() is TimeOfDayService clock) clock.Paused = paused;
                    if (Services.Get<IWeatherService>() is WeatherService weather) weather.Paused = paused;
                    _result = paused ? "Clock/weather paused; physics still runs." : "Clock/weather resumed.";
                    return;
                }
            }
            catch (Exception error) { _result = error.Message; return; }
            switch (text)
            {
                case "reset":
                    if (_ships != null) foreach (var ship in _ships) if (ship != null) ship.ResetPose();
                    _result = "Reset test hulls.";
                    break;
                case "ocean flat":
                case "ocean sine":
                    if (_ocean == null) { _result = "No mock ocean configured."; break; }
                    _ocean.Waves = command.Trim().ToLowerInvariant() == "ocean sine";
                    _result = "Mock wave mode: " + _ocean.Waves;
                    break;
                case "services": _result = Services.Describe(); break;
                case "help": _result = "reset | ocean flat/sine | services | time 20 | speed 60 | weather clear/cloudy/fog/rain/storm/auto | pause | resume"; break;
                default: _result = "Unknown command. Run help."; break;
            }
        }
    }
}
