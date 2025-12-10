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
<img width="640" height="300" alt="Image" src="https://github.com/user-attachments/assets/0a1f70ad-9e9d-4e17-a0e8-2af9b3cc890c" />
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
|                 | Meta Quest 2 / 3 | Vive | Varjo |
|-----------------|:----------------:|:----:|:-----:|
| **Demo Scene** |        ✅        |  ✅  |   ✅  |
| **Miya Pipeline**|        ✅        |   -  |   ✅  |

---

## 🛠️ How to Use

### ⚙️ MIYA Pipeline Usage
- Multi-pass command analysis: Context Awareness → Intent Recognition → Entity Extraction → Clarification.
- Supports extensive voice commands: create, rotate, move, scale, color/texture change, selection, deletion, set weight/time, and more.
- Designed for robust operation in collaborative and multimodal (voice+gesture) VR settings.

#### 🔧 Unity Editor Setup

1.  **Clone & Open**
    ```bash
    git clone [https://github.com/ilisnn2019/MIYA-team4.git](https://github.com/ilisnn2019/MIYA-team4.git)
    ```
    - Add the project to Unity Hub and open with **Unity 6000.0.58f2**.

2.  **Configuration**
    - **Platform:** Switch to `Android` (Quest) or `PC, Mac & Linux` in Build Settings.
    - **XR Plugin:** Enable **Oculus** (or OpenXR) in `Project Settings > XR Plug-in Management`.
    - **API Key:** **(Essential)** Configure your OpenAI API Key to enable voice features.

3.  **Launch**
    - Open `Assets/Scenes/ExampleMIYA.unity` and press **Play**.

> 📖 **Need a Step-by-Step Guide?**
>
> If you are setting this up for the first time or need detailed configuration instructions,<br>
> please read the **[📄 Full Installation Guide](docs/INSTALLATION_GUIDE.md)**.

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

For issues or bug reports, visit [GitHub Issues page](https://github.com/ilisnn2019/MIYA-team4/issues)

---

## ⚖️ License
This project is licensed under the MIT License.
