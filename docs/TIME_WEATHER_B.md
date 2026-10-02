# Clock and weather — Person B

Implemented against the unchanged ITimeOfDay and IWeatherService contracts. B decides state; A displays sun/moon/sky, rain/fog, lightning/audio and wave rendering.

## Defaults and controls

One game day lasts 24 real minutes; start at day 1, 08:00. Daylight is 06:00 inclusive to 18:00 exclusive. Midnight increments DayNumber and emits NewDay for every crossed day. Both components use scaled Unity delta time, so a later game pause with Time.timeScale=0 stops them. Debug pause also stops these two services without stopping physics.

Weather starts clear, wind along world +X at 6 m/s. An independent seeded random stream chooses a different weather type every 180–360 real seconds. Continuous values blend over 30 seconds; direction takes the shortest angular path and stays normalized. Manual weather commands disable automatic selection until `weather auto`.

| Command | Expected result |
|---|---|
| `time 20` | Clock reports night at 20:00; A's sky will react when integrated |
| `time 23.9`, then `speed 60` | Midnight arrives quickly; DayNumber increments |
| `speed 1` | Return clock to ordinary speed |
| `weather storm` | Rain, storm intensity, wind and wave scale blend toward storm |
| `ocean sine` | Mock hull rocking reflects the weather's WaveScale |
| `weather clear` | Smooth return toward clear; interrupted transitions start from current values |
| `weather auto` | Resume scheduled automatic choices |
| `pause`, `resume` | Stop/restart clock and weather; buoyancy keeps running |
| `services` | Registry includes IOceanSurface, ITimeOfDay and IWeatherService |

## Setup and integration

The existing sandbox builder now creates B_TimeOfDay and B_WeatherLogic, plus B_TimeOfDay.asset and B_Weather.asset. Generate the sandbox after importing the updated source. Existing settings assets are preserved. Use the Inspector to tune timing and all five profiles; exactly one profile per WeatherType is required.

In Main_World, add one TimeOfDayService and one WeatherService with their settings assigned before enabling the objects. They register on enable and unregister on disable. A reads ITimeOfDay during presentation updates; subscribe/unsubscribe to WeatherService Changed and LightningStrike using IWeatherService. Weather Type identifies the destination immediately, while numerical properties blend. Treat those numerical properties as the source for effects.

LightningStrike requests presentation only. It does not spawn damage, lights or audio. The lightning timer runs when storm intensity reaches the configured threshold; default intervals are 8–20 seconds. Clock speed changes affect only the clock, not the weather schedule.

SetHour/Restore are debug/state restoration operations; they do not synthesize collector/quest/day-change events. Advance emits NewDay for elapsed gameplay days. There is no save-system integration yet and no promise that random weather schedules survive save/load; that belongs to the saving milestone.

## Verification and cost

The actual TimeOfDayClock source passed nine standalone .NET checks. All nine Unity EditMode tests and two PlayMode tests passed, including time/weather integration in the generated scene and an instant-weather regression. Interactive visuals and integration with A remain pending.

Ordinary frames update a few numbers without allocating arrays or touching shared random state. Profile lookup scans five entries only when selecting weather. The sandbox debug panel formats text and allocates for readability; it is development-only.
