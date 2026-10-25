# VR Climate Simulator

A virtual reality application in which the player lives a short day in a
virtual apartment and, at each step, takes a small everyday decision with an
environmental impact: lighting the room, sorting waste, choosing a breakfast,
handling the heating, commuting and preparing dinner.

The choices feed a single score that drives the state of the virtual world
(weather, background music and a radio bulletin), so that the consequences of a
behaviour are experienced rather than only described. Every relevant action is
written to a time-stamped CSV log, which makes the application a testbed for
behavioural experiments.

Authors: Alessandro Tomaiuolo, Gianluca Di Giacomo

## The day

The session starts with a short introductory video (a flood) and a name entry
on a virtual keyboard. Then the player goes through six tasks, in order:

| Task | Options | Points |
|---|---|---|
| Light | open the window shutters or turn on the lamp | +10 / -10 |
| Recycling | sort 15 pieces of waste into the right bins | +10 / -10 |
| Breakfast | healthy breakfast or snacks | +10 / -10 |
| Thermostat | turn it off or leave it on | +10 / -10 |
| Go to work | touch the door button (loading screen) | 0 |
| Dinner | order a pizza or cook a burger | +10 / +10 |

The total score is mapped to five world states (good, neutral-good, neutral,
neutral-bad, bad), which change the weather, the music and the radio bulletin.

## Requirements

- Windows 64-bit PC
- An OpenXR-compatible VR headset, with its OpenXR runtime set as active
- To open the project: Unity **6000.3.9f1** (Universal Render Pipeline,
  XR Interaction Toolkit 3.4, OpenXR 1.16)

## Run the project

1. Clone the repository and open the folder with Unity 6000.3.9f1.
2. Open the scene `Assets/Scenes/House Scene.unity`.
3. Connect the headset and press Play, or use the XR Device Simulator to test
   without a headset.

## Controls

- Thumbstick: move / snap turn
- Grip button: grab objects, press switches, turn knobs
- Trigger button: use (e.g. squeeze a bottle or an orange); with the ray
  interactor, press keys on the name keyboard
- Touch buttons and panels with the hand (poke)

## Customising the experience

The experience is data-driven and most parameters can be changed without code:

- **Task order:** the ordered list of `TaskData` assets in the `TaskManager`
- **Score:** `pointsPerChoice`, `goodThreshold` and `badThreshold` in the `TaskManager`
- **Intro video:** assign or remove the clip in the `IntroVideoPlayer`
- **Feedback channels:** wire or unwire weather, music and radio in the
  `TaskManager` events
- **Effort:** e.g. `minSnackItemsToEat` in the `BreakfastTask`

## Data log

Every session is appended to `Logs/results_log.csv`, a semicolon-separated file
created next to the executable (or in the project root in the Editor). Each row
has the columns: user, timestamp, task, action, details, task time (s), score.
For recycling, every single throw is logged with the object, the bin and
whether it was correct. The column names and the text values are currently in
Italian.

## Documentation

The project report is in [`docs/`](docs/) (LaTeX source `docs/main.tex`). It
describes the architecture, the implementation and a set of studies that can
be run with the application.

## Credits

- Background music generated with [MusicFX](https://musicfx.net/)
- Radio scripts written with Gemini; radio voices generated with ElevenLabs
