# ResXR Research Template

A comprehensive Unity XR template designed specifically for Meta Quest research experiments. Build VR research applications with built-in support for hand tracking, eye tracking, face expression tracking, comprehensive data collection, and more.

> **⚠️ Note**: This project is still under construction. For inquiries, help, or support, please contact: **resxr.toolkit@gmail.com**

## 🎯 Overview

ResXR is a complete framework for building VR research applications on Meta Quest headsets. It provides:

- **Hand Tracking & Gesture Recognition** - Full hand tracking with pinch detection
- **Eye Tracking & Gaze Analysis** - Real-time eye gaze tracking and focused object detection
- **Face Expression Tracking** - Face expression weights and validity tracking
- **Comprehensive Data Collection** - Automatic CSV export of all tracking data
- **Scene Management** - Additive scene loading with smooth transitions

## 🧠 Core Philosophy: "Clear Box" Design

Unlike other solutions that provide black-box classes, ResXR is designed as a **transparent, clear box** where researchers **own and modify every part of their experiment**. The template provides structure, base classes, and examples, but you are expected to copy, modify, and customize the code for your specific research needs. Everything is open and transparent - you understand exactly how your experiment runs under the hood.

## 📋 Prerequisites

- **Unity 2021.3 or later**
- **Meta Quest SDK (OVR)** - See [Installation](#-installation) section below
- **Meta Quest headset** (Quest 2, Quest Pro, Quest 3)
- **Basic knowledge** of Unity and C#

## 🚀 Quick Start

### 1. Create a New Repository from Template

1. Click the **"Use this template"** button on GitHub
2. Create a new repository with your desired name
3. Clone your new repository locally:

```bash
git clone <your-repository-url>
cd <your-repository-name>
```

### 2. Open in Unity

1. Open Unity Hub
2. Click "Add" and select the cloned project folder
3. Unity will detect the project and open it

### 3. Install Meta SDK Packages

When you first open the project, an installation checker dialog will appear if packages are missing. Follow the prompts to install the required Meta SDK packages.

**Required packages (version 78.0.0):**
- `com.meta.xr.mrutilitykit@78.0.0`
- `com.meta.xr.sdk.audio@78.0.0`
- `com.meta.xr.sdk.core@78.0.0`
- `com.meta.xr.sdk.haptics@78.0.0`
- `com.meta.xr.sdk.interaction.ovr@78.0.0`
- `com.meta.xr.sdk.platform@78.0.0`
- `com.meta.xr.sdk.voice@78.0.0`
- `com.meta.xr.simulator@78.0.0`

See the [Installation](#-installation) section for detailed instructions.

### 4. Create Your First Experiment

1. Navigate to `Assets/Project Folder/`
2. Duplicate `New ResXRScene [Duplicate].unity`
3. Rename it to your experiment name
4. Add it to Build Settings (after Base Scene)
5. **Important**: The Base Scene must be opened **with** your experiment scene additively. The Base Scene contains the player (`ResXRPlayer`) and data manager (`ResXRDataManager_V2`) which run continuously throughout your experiment, even when scenes are changed. Your experiment scene will be loaded additively on top of the Base Scene.
6. Modify the `SceneReferencer` and flow management (SessionManager, RoundManager, TrialManager) scripts to add your experiment references
7. Build and run!

## 📦 Installation

### Automatic Installation Checker

When you first open the project, an installation checker will automatically detect missing Meta SDK packages and guide you through installation:

1. Click "Open Package Manager" in the dialog
2. Click the '+' button > "Add package by name"
3. Add each package with version 78.0.0
4. Repeat for all 8 packages

You can also manually trigger the checker via: `Tools > ResXR > Check Meta SDK Installation`

### Manual Installation

**Via Package Manager:**
1. Open `Window > Package Manager`
2. Click the '+' button > "Add package by name"
3. Enter each package: `com.meta.xr.sdk.core@78.0.0`
4. Click "Add" and repeat for all packages

**Via manifest.json:**
1. Open `Packages/manifest.json`
2. Add the package entries to the `dependencies` section (see full list above)
3. Unity will automatically download and install

### Verification

After installation:
- All packages should appear in Package Manager with version 78.0.0
- No compilation errors related to OVR types
- Installation checker shows no warnings

## 📁 Project Structure

```
Assets/ResXR/
├── Base Scene/              # Core persistent scene and systems
│   ├── ResXRPlayer/        # Player controller, hand/eye/face tracking
│   ├── ResXRDataManager_V2/# Data collection and export system
│   ├── SceneManagement/    # Scene loading and transitions
│   └── ResXR_RoomCalibrator/# Room-scale calibration
├── Flow Management/        # Session/Round/Trial flow control
├── Utilities/              # Helper scripts and utilities
│   ├── EditorUtilities/    # Editor tools and installation checker
│   ├── General Scripts/    # Singleton, utilities, extensions
├── Detectors/              # Interaction detection system
├── Demo Experiments/       # Example implementations
│   ├── Binary Choice/      # Two-choice decision experiment
│   ├── Maze/               # Navigation experiment
│   └── Museum/             # Art viewing experiment
└── Meta components/        # Meta-specific integrations
```

## ✨ Key Features

### Data Collection
- **Automatic Continuous Data**: Head, hands, eyes, body, face tracking at 50Hz
- **Custom Event Logging**: Create custom data classes for experiment-specific events
- **CSV Export**: All data exported to organized CSV files
- **Metadata**: Automatic session metadata generation


### ResXRPlayer API - Easy Access to Tracking Components
While every VR app has tracking, ResXR provides a **simple, unified API** through `ResXRPlayer` singleton that gives you easy access to all tracking components without digging through OVR internals:

- **Hand Tracking**: `ResXRPlayer.Instance.HandLeft/HandRight` - Direct access to hand tracking, pinch detection, and finger colliders
- **Eye Tracking**: `ResXRPlayer.Instance.FocusedObject`, `EyeGazeHitPosition` - Simple properties for gaze tracking and focused object detection
- **Face Tracking**: `ResXRPlayer.Instance.OVRFace` - Direct access to face expression weights and validity
- **Body Tracking**: Body joint positions and calibration
- **Player Transforms**: `PlayerHead`, `RightHand`, `LeftHand` - Easy access to player transforms
- **Input Managers**: `ControllersInputManager`, `PinchingInputManager` - Unified input handling

See `ResXRPlayer.cs` for the complete API. Access everything through `ResXRPlayer.Instance` - no need to find OVR components manually!

### Scene Management
- **Additive Loading**: Experiment scenes are loaded additively on top of the Base Scene, which must remain open throughout your experiment
- **Persistent Base Scene**: The Base Scene contains the player (`ResXRPlayer`) and data manager (`ResXRDataManager_V2`) that run continuously, even when switching between experiment scenes
- **Smooth Transitions**: Automatic fade effects during scene changes
- **Player Repositioning**: Automatic player positioning per scene

### Interaction Systems
- **Pinching**: Hand-based pinch interaction with priority-based selection
- **Controllers**: Quest controller input with haptic feedback
- **Touch**: Collider-based touch detection

### Flow Management
- **Hierarchical Structure**: Session → Round → Trial organization
- **Use Base Classes Directly**: Use `SessionManager`, `RoundManager`, and `TrialManager` as-is (demo experiments duplicate them with prefixes for convenience, but it's not required)
- **Clear Ownership**: You own and modify your experiment code

## 📚 Documentation

- **[Full Component Documentation](ResXR_Template_Documentation.md)** - Comprehensive guide to all components
- **[Data Manager Documentation](Assets/ResXR/Base%20Scene/ResXRDataManager_V2/Doc/ResXRDataManager_V2_README.txt)** - Data collection system details
- **Demo Experiments** - Working examples in `Assets/ResXR/Demo Experiments/`

## 🎓 Learning Resources

### Demo Experiments

The template includes three complete demo experiments:

1. **Binary Choice** - Two-choice decision-making experiment
2. **Maze** - Navigation experiment with coin collection
3. **Museum** - Art viewing experiment with gaze tracking

Each demo shows:
- Flow Management structure (Session/Round/Trial)
- Custom data logging
- Interaction patterns
- Scene organization

### Getting Started Guide

1. **Read the Documentation** - Start with `ResXR_Template_Documentation.md`
2. **Explore Demo Experiments** - See working examples in `Assets/ResXR/Demo Experiments/`
3. **Duplicate the Template Scene** - Use `Assets/Project Folder/New ResXRScene [Duplicate].unity`
4. **Modify Directly** - Own your experiment code - modify scripts directly
5. **Build Your Experiment** - Add your research logic to the template structure

## 🔧 Usage Example

```csharp
// Access player instance
ResXRPlayer player = ResXRPlayer.Instance;

// Fade to black
await player.FadeViewToColor(Color.black, 1.0f);

// Check if player is looking at something
if (player.FocusedObject != null)
{
    Debug.Log($"Looking at: {player.FocusedObject.name}");
}

// Wait for pinch gesture
await player.PinchingInputManager.WaitForHoldAndRelease(HandType.Right, 1.0f);

// Log custom event
ResXRDataManager_V2.Instance.LogChoice(trialNum, option1, option2, choice, reactionTime);

// Switch scenes
await ResXRSceneManager.Instance.SwitchActiveScene("NextExperimentScene");
```

## 🏗️ Building Your Experiment

### Step 1: Create Your Scene
1. Duplicate `Assets/Project Folder/New ResXRScene [Duplicate].unity`
2. Rename to your experiment name
3. Add to Build Settings (after Base Scene)
4. **Scene Architecture**: The Base Scene and your experiment scene must be opened **additively** together. The Base Scene contains:
   - `ResXRPlayer` - Player controller with hand/eye/face tracking
   - `ResXRDataManager_V2` - Data collection system
   - Other core systems that persist throughout your experiment
   
   These systems run continuously and remain active even when you switch between experiment scenes. Your experiment scene is loaded additively on top of the Base Scene, allowing you to change experiment content while keeping the player and data collection systems running.

### Step 2: Modify SceneReferencer
Open `SceneReferencer.cs` and add your experiment references:

```csharp
public class SceneReferencer : ResXRSingleton<SceneReferencer>
{
    [Header("My Experiment Objects")]
    public GameObject stimulus;
    public InstructionsPanel instructions;
    public Transform targetPosition;
    
    [Header("Configuration")]
    public float trialDuration = 10f;
}
```

### Step 3: Set Up Flow Management
Use the base classes from `Assets/ResXR/Flow Management/` directly - no need to copy them. The demo experiments duplicate them with new names (e.g., `Maze_SessionManager`) for organizational convenience, but you can use `SessionManager`, `RoundManager`, and `TrialManager` as-is:

```csharp
// In your experiment scene, use SessionManager directly
// Add it to a GameObject and configure in the Inspector
// The base classes are designed to be used directly
```

### Step 4: Add Your Research Logic
- Use the Flow Management classes (`SessionManager`, `RoundManager`, `TrialManager`) directly in your scene
- Configure them in the Inspector with your rounds and trials
- Add custom data classes for logging
- Implement your experiment-specific logic in the provided methods (StartSession, EndSession, BetweenRoundsFlow, etc.)

## 🤝 Contributing

This is a research template. Contributions, improvements, and feedback are welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

ResXR builds upon the early work of the [TAUXR Research Template](https://github.com/TAU-XR/TAUXR-Research-Template), developed by the [TAU-XR Studio](https://github.com/TAU-XR).

We would like to thank the original contributors and developers for their work on the initial template, which helped shape the early direction of this project.

## 📞 Support

For questions, issues, or inquiries:
- **Email**: resxr.toolkit@gmail.com
- Check the [Full Documentation](ResXR_Template_Documentation.md)
- Review the Demo Experiments for examples
- Examine the source code (it's all transparent!)

## 🎯 Best Practices

1. **Own Your Code** - Use Flow Management base classes directly (SessionManager, RoundManager, TrialManager) - no need to copy them
2. **Understand the System** - Read the code to understand how it works
3. **Use Demo Experiments** - Learn from working examples
4. **Log Everything** - Use ResXRDataManager_V2 for all experiment data
5. **Follow Flow Hierarchy** - Use Session → Round → Trial structure
6. **Keep It Transparent** - All code is open - understand and customize

---

**Remember**: ResXR is a "clear box" template. You own and modify your experiment code. Everything is transparent and open for you to understand and customize.
