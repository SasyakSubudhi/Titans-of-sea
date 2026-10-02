namespace TitansOfTheSea.Contracts
{
    public enum WeatherType { Clear, Cloudy, Fog, Rain, Storm }
    public struct WeatherSnapshot
    {
        public WeatherType Type;
        public UnityEngine.Vector2 WindDirection;   // normalised, world XZ
        public float WindSpeed;                     // m/s, 0..30
        public float Rain;                          // 0..1
        public float Fog;                           // 0..1
        public float StormIntensity;                // 0..1 (controls wave size + lightning)
        public float WaveScale;                     // 0.3..2.5 multiplier for wave height
    }
    public interface IWeatherService
    {
        WeatherSnapshot Current { get; }
        event System.Action<WeatherSnapshot> Changed;   // smooth transitions: Current updates every frame
        event System.Action LightningStrike;            // A plays flash + thunder
    }
}
