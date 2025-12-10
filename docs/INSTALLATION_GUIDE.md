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
    <img width="500" alt="Clone Repo" src="https://github.com/user-attachments/assets/3ae8ba3b-b41b-45fd-980d-bf5bafa9de48" />

2.  **Add to Unity Hub**
    - Launch Unity Hub and click **Add**.
    - Select the root folder of the cloned `MIYA-team4` project.
    <br>
    <img width="500" alt="Unity Hub Add" src="https://github.com/user-attachments/assets/ee976fd3-cb63-4029-aad9-59c8c1023f89" />
    <br>
    <img width="800" alt="Project List" src="https://github.com/user-attachments/assets/8d7d729e-7568-4d9a-b23f-ebf1d88c54a7" />

3.  **Check Unity Version**
    - Ensure you are using the correct Unity version (Recommended: **6000.0.58f2**).
    - If prompted, click `Install Editor` to download the matching version.
    <img width="500" alt="Unity Version" src="https://github.com/user-attachments/assets/e7551b76-398a-42bc-8808-df9f82035c74" />

---

## Step 2: Environment Configuration

Once the project is open, configure the Editor settings.

### 1. Verify Packages
- Go to `Window > Package Manager`.
- Ensure the following packages are installed/imported:
    - **OpenAI-Unity**
    - **MetaXR** & **Voice SDK**
    - **InputSystem**
<img width="250" alt="Package Manager" src="https://github.com/user-attachments/assets/1dba2fe7-270c-4d35-9022-0655f5ee7059" />

### 2. XR Plug-in Management
- Go to `Edit > Project Settings > XR Plug-in Management`.
- Check the box for your target device (e.g., **Oculus** for Quest, **OpenXR** for general headsets).
<img width="250" alt="XR Menu" src="https://github.com/user-attachments/assets/0f84d6dc-1cc7-46e5-a3cd-96e416a21028" />
<img width="700" alt="XR Settings" src="https://github.com/user-attachments/assets/f3a64f30-e51b-41a0-a90b-15d7ad6a77aa" />

### 3. Build Settings
- Go to `File > Build Settings`.
- Select your target platform:
    - **Android:** For Standalone VR (Meta Quest).
    - **PC, Mac & Linux:** For PCVR (Link/AirLink).
<div style="display:flex; gap:10px;">
<img width="250" alt="Build Menu" src="https://github.com/user-attachments/assets/8c74a7e2-54f0-4636-b9a1-439f873c38d6" />
<img width="250" alt="Platform Selection" src="https://github.com/user-attachments/assets/25545d59-4cb0-4665-9e9e-53eeb90c3f9d" />
</div>

---

## Step 3: Critical Pre-requisites

⚠️ **Important:** Failure to configure these will result in runtime errors.

1.  **Resource Structure**
    - **Do NOT rename** critical folders under `Assets/Resources`.
    - Ensure all assets (meshes, textures, data) are correctly placed as references in scripts rely on these specific paths.
    <img width="900" alt="Resources" src="https://github.com/user-attachments/assets/1c4d1556-886d-46ca-8b27-4fea28e3cb78" />

2.  **API Key Setup**
    - You **must** register your OpenAI API Key for the voice interaction (STT/LLM) to work.
    - *(Refer to the separate API Key Setup section if available, or input via the Inspector).*

##### 🔑 OpenAI Key Setup

1. Create `AccessKeySO` ScriptableObject
<br><img width="400" height="500" alt="Image" src="https://github.com/user-attachments/assets/6aa028dc-4ca8-4f31-8bcb-c736594811ff" /><br>
2. Enter OpenAI API Key in Inspector
<br><img width="800" height="200" alt="Image" src="https://github.com/user-attachments/assets/1c4627ea-0b6c-4c7f-9645-80904926c4bb" /><br>
3. Connect SO to relevant managers (e.g., `ChatManager`)
<br><img width="800" height="60" alt="Image" src="https://github.com/user-attachments/assets/bbcdc1fd-2128-48a3-a57c-3de9ff11e268" /><br>

---

## Step 4: First Launch

1.  Open the sample scene: `Assets/Scenes/ExampleMIYA.unity`.
2.  Ensure your VR headset is connected and Microphone permissions are granted.
3.  Press **Play (▶)** in the Unity Editor.
<img width="500" alt="Play Mode" src="https://github.com/user-attachments/assets/91cdeccb-677e-4259-870f-eb664d402b9d" />