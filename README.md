<p align="left">
  <a href="https://github.com/yeongco"><img src="https://img.shields.io/badge/GitHub-LEE YEONG GYO-blue?style=flat-square"/></a>
  <a href="mailto:yeong000327@gmail.com"><img src="https://img.shields.io/badge/yeong000327@gmail.com-blue?style=flat-square"/></a>
</p>
<p align="left">
  <a href="https://github.com/ilisnn2019"><img src="https://img.shields.io/badge/GitHub-KOH SANG JIN-black?style=flat-square"/></a>
  <a href="mailto:gsko1115@konkuk.ac.kr"><img src="https://img.shields.io/badge/gsko1115@konkuk.ac.kr-black?style=flat-square"/></a>
</p>
<p align="left">
  <a href="https://github.com/mjk02123"><img src="https://img.shields.io/badge/GitHub-KIM MIN JEONG-yellow?style=flat-square"/></a>
  <a href="mailto:k64837611@gmail.com"><img src="https://img.shields.io/badge/k64837611@gmail.com-yellow?style=flat-square"/></a>
</p>

# MIYA : VR Voice Interface

[<img width="640" height="360" alt="Image" src="https://github.com/user-attachments/assets/f10e0991-487b-44e0-9c63-2c5ef44fbda8" />](https://youtu.be/uab5hOoRDZ0)<br>
☝️Demo Video

---

### 💡 WHAT
MIYA is a natural language voice interface designed for Unity VR environments. Users interact exclusively via voice, leveraging Large Language Models (LLM) and Speech-To-Text (STT) technology to create,transform and manipulate VR objects through speech-driven commands.
**Key Proposition:** MIYA proposes the full potential of a voice-only interface in immersive VR. You can control all scenes and manage objects without the need for keyboard, controller, or gesture input—just voice.

### 🎯 WHY
- Maximizes efficiency, immersion, and convenience with real-time voice command in VR.
- User-friendly: Natural speech replaces complex control schemes.
- Shows that voice input alone supports precise and intuitive manipulation, establishing a new standard for hands-free VR interaction.

### 🧪 **Tested on**
* MetaQuest3 / Varjo XR-4
* Unity 6000.0.58f2
* OpenAI API integration (wit.ai, Whisper STT supported)

### 🎧 **Support HMD**
|                 | Meta Quest | Vive | Varjo |
|-----------------|:----:|:-------:|:-----:|
| **Demo Scene**  |   ✅   |  ✅   |   ✅  |
| **Miya Pipeline**|  ✅   |   -   |   ✅  |

---

## 🛠️ How to Use

### ⚙️ MIYA Pipeline Usage
- Multi-pass command analysis: Context Awareness → Intent Recognition → Entity Extraction → Clarification.
- Supports extensive voice commands: create, rotate, move, scale, color/texture change, selection, deletion, set weight/time, and more.
- Designed for robust operation in collaborative and multimodal (voice+gesture) VR settings.

#### 🔧 Unity Editor Setup

**Step-by-step guide to launch MIYA VR Voice Interface from GitHub with Unity Hub:**

1. **Clone the Repository**
    <br><img width="500" height="373" alt="Image" src="https://github.com/user-attachments/assets/3ae8ba3b-b41b-45fd-980d-bf5bafa9de48" /><br>
   - Go to the GitHub page and click `Code > Copy HTTPS link`.
   - Open a terminal and run:
     ```
     git clone https://github.com/ilisnn2019/MIYA-team4.git
     ```
   - Or, use GitHub Desktop or download ZIP and extract.

2. **Open in Unity Hub**
    <br><img width="500" height="333" alt="Image" src="https://github.com/user-attachments/assets/ee976fd3-cb63-4029-aad9-59c8c1023f89" /><br>
    <br><img width="900" height="90" alt="Image" src="https://github.com/user-attachments/assets/8d7d729e-7568-4d9a-b23f-ebf1d88c54a7" /><br>
   - Launch Unity Hub and click `Add`.
   - Select the root folder of the downloaded MIYA-team4 project.
   - In your Unity Hub “Projects” list, select the MIYA-team4 project.

3. **Set Correct Unity Version**
    <br><img width="500" height="280" alt="Image" src="https://github.com/user-attachments/assets/e7551b76-398a-42bc-8808-df9f82035c74" /><br>
   - If prompted, select/reinstall the recommended Unity version (e.g., 6000.0.58f2).  
   - If not installed, click `Install Editor` in Unity Hub and choose the correct version.

4. **Install Required Packages**
    <br><img width="250" height="350" alt="Image" src="https://github.com/user-attachments/assets/1dba2fe7-270c-4d35-9022-0655f5ee7059" /><br>
   - Open the project in Unity Editor.
   - Go to `Window > Package Manager`.
   - Confirm these packages are present. If not, install/import:
     - OpenAI-Unity (custom or from manifest.json)
     - MetaXR
     - Voice SDK
     - InputSystem & relevant XR packages

5. **XR Plugin**
    <br><img width="250" height="510" alt="Image" src="https://github.com/user-attachments/assets/0f84d6dc-1cc7-46e5-a3cd-96e416a21028" /><br>
    <br><img width="800" height="350" alt="Image" src="https://github.com/user-attachments/assets/f3a64f30-e51b-41a0-a90b-15d7ad6a77aa" /><br>
   - Open `Edit > Project Settings > XR Plug-in Management`
   - Enable for intended HMD (e.g., Oculus, OpenXR).

6. **Build Settings**
    <br><img width="250" height="300" alt="Image" src="https://github.com/user-attachments/assets/8c74a7e2-54f0-4636-b9a1-439f873c38d6" />
    <img width="250" height="470" alt="Image" src="https://github.com/user-attachments/assets/25545d59-4cb0-4665-9e9e-53eeb90c3f9d" /><br>
   - Open `File > Build Settings`.
   - Select your platform: `PC, Mac & Linux` or `Android` for standalone/Quest.

6. **Resource Structure**
    <br><img width="1000" height="250" alt="Image" src="https://github.com/user-attachments/assets/1c4d1556-886d-46ca-8b27-4fea28e3cb78" /><br>
   - Ensure all assets (objects, meshes, textures, materials, data) are in correct folders, especially under `Assets/Resources`.
   - Do not change critical resource folder/file names to avoid missing references in code/scripts.
     
7. **API Key Registration**
   - Follow “OpenAI Key Setup” steps (see section below) to enable voice commands.
   - Without API keys, LLM/STT features won't function.
    
8. **First Launch**
    <br><img width="500" height="450" alt="Image" src="https://github.com/user-attachments/assets/91cdeccb-677e-4259-870f-eb664d402b9d" /><br>
   - Open a sample scene: `Assets/Scenes/ExampleMIYA.unity`
   - Click `Play` (▶) in Unity Editor to test the environment and MIYA pipeline.
   - Set up microphone permissions and VR device connection as necessary.



**Design Principle:**  
MIYA is implemented as a voice-first interface. Gesture and controller input are optional and can be configured if desired, but the core system requires only a working microphone and a VR-ready Unity Editor!

#### 🛠 Inspector Configuration

- Create/assign ScriptableObjects (e.g. `AccessKeySO`) to core managers (`STTModule`, `Chat Manager`).
- Check prefab/data connections in sample scenes (`VR_NLP`, `FirstPerson_NLP`).
- Verify that the prompt is functioning correctly and properly configured.
- Test wake-word and accurate entity parsing for best results.
- `Project configuration` helps you can effectively utilize the pipeline.

#### ✏️ How to Add New Commands

1. Define commands in `Assets/Resources/prompt.txt` (name/params).
2. Implement matching C# methods in `Assets/Scripts/CME/CommandExecutor.cs`.
3. Ensure name & parameters match for correct auto-mapping.
4. In the project configuration, create and apply the modified prompt.
5. Test in scene and validate schema-driven execution.

#### 🔑 OpenAI Key Setup

1. Create `AccessKeySO` ScriptableObject
<br><img width="400" height="500" alt="Image" src="https://github.com/user-attachments/assets/6aa028dc-4ca8-4f31-8bcb-c736594811ff" /><br>
2. Enter OpenAI API Key in Inspector
<br><img width="800" height="200" alt="Image" src="https://github.com/user-attachments/assets/1c4627ea-0b6c-4c7f-9645-80904926c4bb" /><br>
3. Connect SO to relevant managers (e.g., `ChatManager`)
<br><img width="800" height="60" alt="Image" src="https://github.com/user-attachments/assets/bbcdc1fd-2128-48a3-a57c-3de9ff11e268" /><br>


#### 🎨 Customization

- Add or edit object types and textures that can be handled within the scene. Bundle `EntitySpec` (object types) and `TextureSpec` (textures) into their respective `Pack`s, and explicitly specify them within the prompt to ensure proper usage in the scene.
- You can also customize conversational prompts and scenario flows—schema-driven for easy extension.

---

### 🎮 Demo Scene Usage (Demo Scene APK)

- The Demo scene will be distributed as an APK.
- Supported commands (create/move/rotate/color/texture/scale etc) will be documented in release notes.
- Real APK usage includes wake-word detection, entity-driven commands, rapid real-time feedback.
- Details and updates (supported devices, command guide) to follow with each release.

---

## 📋 Sample Command
In preparation

---

## 🐛 Known Issues
- Some commands may not work or may produce errors
- Differences may arise per environment/HMD
- Changing resource paths or names can break command execution
- Current limitations: Some feature coverage non-exhaustive; planned future work includes multi-language support and more gesture fusion

For issues or bug reports, visit [GitHub Issues page](https://github.com/yeongco/MIYA-team4/issues)

---

## ⚖️ License
This project is licensed under the MIT License.
