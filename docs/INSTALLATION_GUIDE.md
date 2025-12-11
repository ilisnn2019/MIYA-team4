[← Back to Main README](../README.md)

# 🔧 Detailed Installation Guide

This document provides a step-by-step walkthrough for setting up the **MIYA VR** project from scratch.

---

## Step 1: Clone & Project Setup

1.  **Clone the Repository**
    - Copy the HTTPS link from GitHub (`Code > Copy HTTPS link`).
    - Run the following command in your terminal:
      ```bash
      git clone [https://github.com/ilisnn2019/MIYA-team4.git](https://github.com/ilisnn2019/MIYA-team4.git)
      ```
    <img width="500" alt="그림1" src="https://github.com/user-attachments/assets/42544105-eb1f-4290-8fb3-db4ec0b76161" />

2.  **Add to Unity Hub**
    - Launch Unity Hub and click **Add**.
    - Select the root folder of the cloned `MIYA-team4` project.
    <br>
    <img width="500" alt="그림2" src="https://github.com/user-attachments/assets/892bdb7b-d8e1-40c3-9e72-f26de49effac" />
    <br>
    <img width="800" alt="그림3" src="https://github.com/user-attachments/assets/53b16703-52ba-49ee-aa29-2cae0879d1e3" />

3.  **Check Unity Version**
    - Ensure you are using the correct Unity version (Recommended: **6000.0.58f2**).
    - If prompted, click `Install Editor` to download the matching version.
    <img width="500" alt="그림4" src="https://github.com/user-attachments/assets/dabaad00-dd28-4db8-8d59-e2f3831c0fab" />

---

## Step 2: Environment Configuration

Once the project is open, configure the Editor settings.

### 1. Verify Packages
- Go to `Window > Package Manager`.
- Ensure the following packages are installed/imported:
    - **OpenAI-Unity**
    - **MetaXR** & **Voice SDK**
    - **InputSystem**
<img width="250" alt="그림5" src="https://github.com/user-attachments/assets/477f5caf-c711-4cc3-91c4-1b982c79f9b7" />

### 2. XR Plug-in Management
- Go to `Edit > Project Settings > XR Plug-in Management`.
- Check the box for your target device (e.g., **Oculus** for Quest, **OpenXR** for general headsets).
<img width="250" alt="그림6" src="https://github.com/user-attachments/assets/2979fdf2-e4f4-4b75-9f1d-a5a968c45dec" />
<img width="700" alt="그림7" src="https://github.com/user-attachments/assets/ac1bfeb0-14a0-4abd-9632-7bb77a8102d2" />

### 3. Build Settings
- Go to `File > Build Settings`.
- Select your target platform:
    - **Android:** For Standalone VR (Meta Quest).
    - **PC, Mac & Linux:** For PCVR (Link/AirLink).
<div style="display:flex; gap:10px;">
<img width="250" alt="그림8" src="https://github.com/user-attachments/assets/a2c95574-535b-4ab3-a2f3-af40dca4a814" />
<img width="250" alt="그림9" src="https://github.com/user-attachments/assets/b2dfd8c1-a09a-4475-8707-d9f5d043890c" />
</div>

---

## Step 3: Critical Pre-requisites

⚠️ **Important:** Failure to configure these will result in runtime errors.

1.  **Resource Structure**
    - **Do NOT rename** critical folders under `Assets/Resources`.
    - Ensure all assets (meshes, textures, data) are correctly placed as references in scripts rely on these specific paths.
    <img width="900" alt="그림10" src="https://github.com/user-attachments/assets/c1535233-9c59-4c34-9b6c-cc99022ea1eb" />

2.  **API Key Setup**
    - You **must** register your OpenAI API Key for the voice interaction (STT/LLM) to work.
    - *(Refer to the separate API Key Setup section if available, or input via the Inspector).*

##### 🔑 OpenAI Key Setup

1. Create `AccessKeySO` ScriptableObject
<br><img width="400" height="500" alt="그림12" src="https://github.com/user-attachments/assets/70ea784a-cb27-4ff0-91b1-822e1a46fba1" /><br>
2. Enter OpenAI API Key in Inspector
<br><img width="800" height="200" alt="그림13" src="https://github.com/user-attachments/assets/1f458c6c-eaea-4348-85d8-28cc3ebfccba" /><br>
3. Connect SO to relevant managers (e.g., `ChatManager`)
<br><img width="800" height="60" alt="그림14" src="https://github.com/user-attachments/assets/7bcc19ab-5636-48d1-adc7-916c33e6a776" /><br>

---

## Step 4: First Launch

1.  Open the sample scene: `Assets/Scenes/Experiment.unity`.
2.  Ensure your VR headset is connected and Microphone permissions are granted.
3.  Press **Play (▶)** in the Unity Editor.<br>
    <img width="141" height="197" alt="image" src="https://github.com/user-attachments/assets/6dece5e7-ba70-4d17-8b22-b8e030f22b7b" />
