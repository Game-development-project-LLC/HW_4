# Bowling Game – Weekly Assignment

A small 3D bowling game built in Unity to practice using the physics engine and basic game architecture.

---

## 1. How to Run the Game

1. Open the project in **Unity** (2022/6000+ or compatible).
2. Open the scene:

   `Assets/Scenes/BowlingScene.unity`

3. Press **Play** in the Unity editor.

Controls in Play mode:

- **Spacebar** – throw the ball forward along the lane.
- Optional: rotate the player/camera to aim (if enabled in the scene).

The UI at the top shows:
- `Pins: X` – number of pins still standing.  
- `Roll: 1/2` or `Roll: 2/2` – current roll in the frame.  
- A message with simple instructions and feedback.

Buttons:
- **Restart** – reloads the scene and starts a new frame.
- **Next Level** – starts a new round / next scene (if configured).

---

## 2. Class Relationships (Short Overview)

- **GameManager**  
  - Singleton that holds the main game state: list of pins, current roll, game state enum, and references to UI texts.  
  - Decides when the player is allowed to shoot, counts how many pins fell after each roll, and updates the UI.

- **PlayerController**  
  - Handles player input and ball spawning.  
  - On **SPACE** it asks `GameManager.CanShoot()`.  
  - If allowed, it instantiates the `Ball` prefab at `BallSpawnPoint`, adds an impulse force with `Rigidbody.AddForce`, and then calls `GameManager.OnPlayerShot()`.

- **EnemyBase** (one bowling pin)  
  - Attached to each pin (with `Rigidbody` + `CapsuleCollider`).  
  - Stores initial position / rotation / up-direction of the pin.  
  - In `Update()` it checks the angle between the current up vector and the initial one; if the angle is larger than a threshold, the pin is marked as **down**.  
  - Exposes methods `ResetPin()` and `RemoveFromLane()` so that `GameManager` can reset or hide pins.

- **BallEndTrigger**  
  - Attached to an invisible trigger at the end of the lane.  
  - When a `Ball` enters the trigger it calls `GameManager.OnBallStopped()` so the manager can evaluate the roll.

Relationships in short:

- `GameManager` aggregates a list of `EnemyBase` and polls them to know which pins fell.  
- `PlayerController` and `BallEndTrigger` both **depend on** `GameManager` and notify it about player actions and ball state.  
- `EnemyBase` does not know the manager directly; it only tracks its own physical state.

---

## 3. Design Choices / Architecture Notes

- **Single Responsibility**  
  - `PlayerController` handles only input and ball launching.  
  - `GameManager` contains all the game rules and UI updates.  
  - `EnemyBase` is responsible only for detecting whether a pin has fallen and for resetting itself.

- **Physics-based gameplay**  
  - Both ball and pins use Unity’s physics (`Rigidbody` + colliders).  
  - The game does not animate pins manually; their motion is a result of real collisions and forces.

- **Prefab-based setup**  
  - `Ball` and `BowlingPin` are prefabs.  
  - All pins in the lane are instances of a single prefab under `PinsParent`, which makes it easy to tweak physics or visuals in one place.

- **Decoupled visuals and logic**  
  - The imported 3D bowling pin model is a child of the `BowlingPin` root object.  
  - The root holds the collider, rigidbody and `EnemyBase` script, so we can change the model/texture without touching the game logic.

