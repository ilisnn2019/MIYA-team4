[← Back to Main README](../README.md)

# 🎮 MIYA Command Showcase

This document details the functional logic behind MIYA's voice interactions.
All demonstrations include subtitles indicating the voice commands used.

---

## 1. Attribute & Transformation
Direct manipulation of object properties and transforms using natural language.

| Feature | Preview | Functional Description |
|:---:|:---:|:---|
| **Instantiation**<br>(Create) | <img src="images/01.create.gif" width="300" /> | **Prefab Spawning**<br>Identifies the requested object type from the entity database and instantiates it in the scene in front of the user. |
| **Material**<br>(Color) | <img src="images/02.color.gif" width="300" /> | **Shader Property Modification**<br>Parses color keywords and applies the corresponding RGB values to the target object's material. |
| **Translation**<br>(Move) | <img src="images/03.move.gif" width="300" /> | **View-Relative Movement**<br>Moves the object based on the user's current viewpoint (e.g., "Right" translates along the camera's local X-axis). |
| **Rotation**<br>(Rotate) | <img src="images/04.rotate.gif" width="300" /> | **Transform Rotation**<br>Applies Euler angle rotation to the object. Supports specific degrees (e.g., 30°) or general directions. |
| **Scaling**<br>(Size) | <img src="images/05.scale.gif" width="300" /> | **Local Scale Adjustment**<br>Modifies the object's local scale factor. Handles multiplication (double, triple) and descriptive resizing. |
| **Texture Mapping** | <img src="images/06.texture.gif" width="300" /> | **Texture Swapping**<br>Replaces the main texture of the material with a requested texture type (e.g., Wood, Metal, Pattern). |
| **Physics Property**<br>(Weight) | <img src="images/07.weight.gif" width="300" /> | **RigidBody Manipulation**<br>Accesses the Unity Physics engine to adjust the `Mass` property of the target object interactively. |
| **Layout**<br>(Sort) | <img src="images/08.sort.gif" width="300" /> | **Procedural Arrangement**<br>Calculates positions for multiple objects to arrange them in specific formations (Linear, Circular, Grid). |

---

## 2. Handling Ambiguous Expressions
Commands that resolve vague or implicit references using gaze, history, and attributes.

| Feature | Preview | Functional Description |
|:---:|:---:|:---|
| **Deictic Reference**<br>(Gaze) | <img src="images/09.this.gif" width="300" /> | **Gaze-Driven Selection**<br>Resolves demonstrative pronouns like "This" or "That" by raycasting from the user's HMD to identify the focused object. |
| **Anaphoric Reference**<br>(History) | <img src="images/10.prev.gif" width="300" /> | **Contextual Memory**<br>Resolves references to previous interactions (e.g., "Do the same to that one") by recalling the last executed action parameters. |
| **Fuzzy Quantifier**<br>(Relative) | <img src="images/11.relative.gif" width="300" /> | **Non-Deterministic Value Processing**<br>Interprets abstract modifiers like "Slightly", "A little", or "Huge" into calculated numerical coefficients. |
| **Implicit Targeting**<br>(Attribute Search) | <img src="images/12.unclear.gif" width="300" /> | **Attribute-Based Filtering**<br>Identifies a target not by name, but by searching the scene for objects matching a specific property (e.g., "The green one"). |

---

## 3. Complex Sequence
Handling multi-intent instructions within a single utterance.

| Feature | Preview | Functional Description |
|:---:|:---:|:---|
| **Sequential Logic** | <img src="images/13.sequence.gif" width="300" /> | **Multi-Intent Parsing**<br>Decomposes a compound sentence into a queue of atomic actions and executes them in the correct logical order. |