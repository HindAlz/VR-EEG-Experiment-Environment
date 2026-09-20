# VR EEG Experiment Environment

**A Unity VR environment for planned EEG studies of cognitive workload, with progressively more demanding tasks and interaction event markers.**

Participants move from interaction practice and a stillness period to guided exploration, reading and comparison, and multi step object manipulation tasks. The environment is intended to support EEG collection across these different task conditions.

The research aim is to examine how EEG activity varies with task demands and participant actions. The difficulty progression is a design intention, differences in experienced workload remain to be evaluated through the planned study.

## Task design

- **Interaction practice:** teleportation, poking buttons, and rotating objects with two hands.
- **Stillness period:** a two minute segment asking the participant to remain still.
- **Audio-guided progression:** spoken prompts and a button controlled transition.
- **Guided gallery exploration:** move between statues, read descriptions, and pick up objects.
- **Comparison and judgment:** read about paintings and sculptures, then select preferred designs and stories.
- **Multi-step tasks:** find and place a sphere, straighten selected paintings, and sort paint tubes onto shelves by color.

## Scene guide

Experiment scenes are organized under **[`Assets/FINAL SCENES/`](Assets/FINAL%20SCENES/)**.

| Scene | Purpose |
| --- | --- |
| [`START/SEG 0.5.1.unity`](Assets/FINAL%20SCENES/START/SEG%200.5.1.unity) | Teleportation and button-poking practice; the first enabled scene in the saved build settings. |
| [`SEG 0.5/SEG 0.5.2.unity`](Assets/FINAL%20SCENES/SEG%200.5/SEG%200.5.2.unity) | Two handed rotation practice and further navigation instructions. |
| [`Segment 0/SEG 0.unity`](Assets/FINAL%20SCENES/Segment%200/SEG%200.unity) | Additional room scene included in the build list. |
| [`Segment 1/SEG 1.unity`](Assets/FINAL%20SCENES/Segment%201/SEG%201.unity) | Stillness period, configured for 120 seconds before advancing. |
| [`Segment 2/SEG 2.unity`](Assets/FINAL%20SCENES/Segment%202/SEG%202.unity) | Audio sequence, repeated prompt, and proceed button. |
| [`Segment 3/SEG 3.unity`](Assets/FINAL%20SCENES/Segment%203/SEG%203.unity) | Statue exploration, reading, and object handling. |
| [`Segment 4/SEG 4.unity`](Assets/FINAL%20SCENES/Segment%204/SEG%204.unity) | Gallery reading and preference-selection tasks. |
| [`Segment 5/SEG 5.unity`](Assets/FINAL%20SCENES/Segment%205/SEG%205.unity) | Sphere placement, painting alignment, and paint-tube sorting. |

An additional [`SEG 0.5.unity`](Assets/FINAL%20SCENES/SEG%200.5/SEG%200.5.unity) practice scene exists. 

## Interaction markers

The custom scripts emit **`EM:`-prefixed messages through Unity's `Debug.Log`**. These mark instrumented events such as:

- Segment starts and ends.
- Audio playback and proceed-button presses.
- Movement onset and arrival at a statue or exit.
- Opening and closing reading panels.
- Grabbing, releasing, placing, and aligning objects.
- Review submissions and task completion.

Examples from the code:

```text
EM: SEG 1 START
EM: SEG 1 END
EM: Playing audio 1
EM: User starts moving
EM: placed sphere
EM: Locked painting
EM: task 3 done
```

These are currently **Unity log markers**.

## Technology

| Component | Version or implementation |
| --- | --- |
| Engine | **Unity 6000.2.7f2** |
| Language | C# |
| Meta XR Core and Interaction SDK OVR | 78.0.0 |
| XR Interaction Toolkit | 3.2.1 |
| OpenXR Plugin | 1.15.1 |
| XR Hands | 1.6.1 |
| Universal Render Pipeline | 17.2.0 |
| Event instrumentation | Unity log messages prefixed with `EM:` |

The editor version is recorded in [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt), and package versions are listed in [`Packages/manifest.json`](Packages/manifest.json).

## Open and explore

1. Install **Unity Hub** and **Unity 6000.2.7f2**.
2. Clone the repository:

   ```bash
   git clone https://github.com/HindAlz/VR-EEG-Experiment-Environment.git
   ```

3. Add the cloned folder to Unity Hub. Select the root containing `Assets`, `Packages`, and `ProjectSettings`.
4. Open it with the matching editor and allow package resolution and asset import to finish.
5. Open **`Assets/FINAL SCENES/START/SEG 0.5.1.unity`** as the entry scene.
6. Configure compatible VR hardware for the project's Meta XR/OpenXR setup and follow the in-scene practice instructions.
7. During Editor testing, filter the **Console** for `EM:` to inspect the instrumented events.

For a device build, review the active build profile's scene list against [`EditorBuildSettings.asset`](ProjectSettings/EditorBuildSettings.asset), including any practice-retry scenes needed by the selected flow.

## Code navigation

| File | Role |
| --- | --- |
| [`GrabEM.cs`](Assets/GrabEM.cs) | Meta Interaction SDK grab/release events and task progression. |
| [`GrabEMGrabbable.cs`](Assets/GrabEMGrabbable.cs) | Grab/release instrumentation for `OVRGrabbable`. |
| [`ReadPanel.cs`](Assets/ReadPanel.cs) and [`ReadText.cs`](Assets/ReadText.cs) | Reading-panel interaction markers. |
| [`movementEM.cs`](Assets/FINAL%20SCENES/Segment%203/movementEM.cs) | First movement detection after an initial delay. |
| [`readInstruct.cs`](Assets/readInstruct.cs) and [`ChoiceSelect.cs`](Assets/ChoiceSelect.cs) | Gallery reading progress and review submissions. |
| [`snap.cs`](Assets/snap.cs) | Painting manipulation, alignment, and locking. |
| [`paint.cs`](Assets/paint.cs), [`shelf.cs`](Assets/shelf.cs), and [`manager.cs`](Assets/manager.cs) | Paint-tube handling, shelf membership, and sorting-task completion. |

## Demo
Full Video Demo: https://drive.google.com/file/d/1hkzsy-d9wjkD30KLICL7cAY1SKWJLX18/view?usp=sharing
<img width="1819" height="1069" alt="Screenshot 2025-11-18 073222" src="https://github.com/user-attachments/assets/01e8600c-4d1e-44e5-a2af-f1efbd456ece" />
<img width="1910" height="1129" alt="Screenshot 2025-11-18 072827" src="https://github.com/user-attachments/assets/5592d426-2cb7-45e7-ad03-688186847602" />
<img width="1691" height="1132" alt="Screenshot 2025-11-18 072158" src="https://github.com/user-attachments/assets/17c1117f-9a4e-41f0-914b-af110748acd3" />
<img width="1900" height="1069" alt="Screenshot 2025-11-18 072945" src="https://github.com/user-attachments/assets/9163de52-ea6d-4d79-9435-dcf9ea286c6c" />

