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


[<img width="640" height="360" alt="그림15" src="https://github.com/user-attachments/assets/682add49-93b2-47b3-8f10-934e903e1c56" />](https://youtu.be/mma5uJsDFiM?si=Fx0bUf5QaVs4gaaJ)<br>
☝️Demo Video

---

### 💡 WHAT
<img width="640" height="300" alt="Image" src="https://github.com/user-attachments/assets/0a1f70ad-9e9d-4e17-a0e8-2af9b3cc890c" /><br>
- MIYA is a natural language voice interface designed for Unity VR environments. Users interact exclusively via voice, leveraging Large Language Models (LLM) and Speech-To-Text (STT) technology to create, transform and manipulate VR objects through speech-driven commands.<br>
- **Key Proposition:** MIYA proposes the full potential of a voice-only interface in immersive VR. You can control all scenes and manage objects without the need for keyboard, controller, or gesture input—just voice.

### 🎯 WHY
- Maximizes efficiency, immersion, and convenience with real-time voice command in VR.
- User-friendly: Natural speech replaces complex control schemes.
- Shows that voice input alone supports precise and intuitive manipulation, establishing a new standard for hands-free VR interaction.

### 🚀 Performance
We compared MIYA with existing voice interface baselines to validate its efficiency and accuracy.

| Metric | Previous Work | **MIYA (Ours)** | Improvement |
| :--- | :---: | :---: | :---: |
| **STT Accuracy** | 96.71 ± 0.05% | **98.6%** | 🔼 **+1.89%** |
| **TTC (Latency)**| 21.0s | **2.69s** | ⚡ **87% Faster** |

> **Result:** MIYA achieves near-perfect command recognition while drastically reducing execution time, enabling a seamless real-time VR experience.

### 🧪 **Tested on**
* MetaQuest2/3 / Varjo XR-4
* Unity 6000.0.58f2
* OpenAI API integration (wit.ai, Whisper STT supported)

### 🎧 **Support HMD**
|                 | Meta Quest 2 / 3 | Vive | Varjo |
|-----------------|:----------------:|:----:|:-----:|
| **Demo Scene** |        ✅        |  -  |   -  |
| **Miya Pipeline**|        ✅        |   ✅  |   ✅  |

---

## 🛠️ How to Use

### ⚙️ MIYA Pipeline Usage
- Multi-pass command analysis: Context Awareness → Intent Recognition → Entity Extraction → Clarification.
- Supports extensive voice commands: create, rotate, move, scale, color/texture change, selection, deletion, set weight/time, and more.
- Designed for robust operation in collaborative and multimodal (voice+gesture) VR settings.
> 📖 **Need a Step-by-Step Guide?** <br>
> If you are setting this up for the first time, please read the **[Full Installation Guide](docs/INSTALLATION_GUIDE.md)**.

#### 🔧 Unity Editor Setup

1.  **Clone & Open**
    ```bash
    git clone https://github.com/ilisnn2019/MIYA-team4.git
    ```
    - Add the project to Unity Hub and open with **Unity 6000.0.58f2**.

2.  **Configuration**
    - **Platform:** Switch to `Android` (Quest) or `PC, Mac & Linux` in Build Settings.
    - **XR Plugin:** Enable **Oculus** (or OpenXR) in `Project Settings > XR Plug-in Management`.
    - **API Key:** **(Essential)** Configure your OpenAI API Key to enable voice features.
    - 🔑 **OpenAI Key Setup**
      1. Create `AccessKeySO` ScriptableObject
      <br><img width="400" height="500" alt="그림12" src="https://github.com/user-attachments/assets/70ea784a-cb27-4ff0-91b1-822e1a46fba1" /><br>
      2. Enter OpenAI API Key in Inspector
      <br><img width="800" height="200" alt="그림13" src="https://github.com/user-attachments/assets/1f458c6c-eaea-4348-85d8-28cc3ebfccba" /><br>
      3. Connect SO to relevant managers (e.g., `ChatManager`)
      <br><img width="800" height="60" alt="그림14" src="https://github.com/user-attachments/assets/7bcc19ab-5636-48d1-adc7-916c33e6a776" /><br>

3.  **Launch**
    - Open `Assets/Scenes/Experiment.unity` and press **Play**.

#### 🛠 Inspector Configuration

- Create/assign ScriptableObjects (e.g. `AccessKeySO`) to core managers (`STTModule`, `Chat Manager`).
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
> **⚠️ Compatibility Note**
> While this demo is compatible with **Meta Quest 2**, we **strongly recommend using Meta Quest 3 or higher** for a smoother and optimal experience.

You can experience the MIYA demo directly on your Meta Quest device without setting up the Unity Editor.

- **Download:** Get the latest `MIYA_DEMO.apk` file from the **[Releases](../../releases)** page.
- **Install:** Sideload the APK onto your Meta Quest 2 or 3 using tools like **SideQuest** or **Meta Quest Developer Hub (MQDH)**.
- **Run:** Once installed, find the app in your library under **Unknown Sources** and launch it to test voice commands immediately.

---

## 📋 Sample Command

Experience the core capabilities of MIYA through these voice commands.

| **Create Object** | **Change Color** |
|:---:|:---:|
| <img src="docs/images/01.create.gif" width="200" /> | <img src="docs/images/02.color.gif" width="200" /> |
| *"Create a cube"* | *"Make cube red"* |

| **Move Object** | **Rotate Object** |
|:---:|:---:|
| <img src="docs/images/03.move.gif" width="200" /> | <img src="docs/images/04.rotate.gif" width="200" /> |
| *"Move cube right"* | *"Rotate cube 30 degrees"* |

> 🚀 **Explore All 13 Commands**<br>
> Check out the full gallery in the **[Command Showcase](docs/COMMANDS.md)**.

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
