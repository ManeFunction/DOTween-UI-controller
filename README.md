# DOTween UI controller
Neat Unity controller component to set up UI `DOTween` animations without any code. `DOTween Pro` is not required, but supported!

# Features
1. Seven pre-configured tweens for all essential UI animation operations: **Move** (X/Y), **Scale** (X/Y), 
**Rotate** (Z only), **Color** (for Images and other MaskableGraphic), and **Fade** 
(for alpha property of CanvasGroup).
1. A powerful inspector built with the `UI Toolkit`, designed to optimize editor rendering performance.
1. Lazy initialization of required components to streamline the workflow and enhance efficiency.
1. Flexible Unity lifecycle integration, allowing tweens to, for example, pause on `OnDisable()` 
or be reused and replayed on `OnEnable()`.
1. Separate tweens for X and Y axes in move and scale animations, providing finer control.
1. Looping support via Sequences, enabling comprehensive control over delay configurations.
1. Easing options with support for both custom curves and build-in mathematical easing functions.
1. An autoplay feature with optional delays, reducing the need for coding — ideal for animators.
1. Full programmatic control is also available for developers who prefer working directly in code.
1. The ability to replay animations directly from the editor for testing and fine-tuning without 
relaunching your game (available only in `Play mode` due to `DOTween` limitations).

# Look
![](https://raw.githubusercontent.com/wiki/ManeFunction/DOTween-UI-controller/main.png)

# Installation
I recommend using the `OpenUPM` package manager to install this package because it’s easy to manage dependencies 
and updates that way. However, if you prefer not to use `OpenUPM` or are unable to do so, you can simply follow 
the first step below and then place the downloaded package anywhere in your project.

I understand that it might seem a bit complicated to install `OpenUPM` for the first time, but it’s worth it.
`OpenUPM` is the best place to find and manage open-source Unity packages, providing a seamless experience 
with package management, such as dependency resolution and updates, right from Unity’s `Package Manager`.

For CL operations on Windows, I recommend using `Git Bash` (`MINGW`) because it provides a Unix-like environment 
and there’s a good chance it’s already installed on your machine.

1. Prepare your `DOTween` package to be used by other tools:
   - Open `Tools -> Demigiant -> DOTween Utility Panel`.
   - Click `Create ASMDEF...`. (If you see a `Remove ASMDEF...` button - do nothing, skip to the next step).
   - If you see compilation errors afterward, add the `DOTween` asmdef to the References of your own asmdef 
   for any modules that use `DOTween`.
1. Install `OpenUPM` on your machine (if you don’t have it already):
   - If you don’t have `npm` installed, install `Node.js` from the [official website](https://nodejs.org) 
   (or use [Homebrew](https://brew.sh) on macOS: `brew install node`).
   - Open a command line interface (CLI) of your choice and run: `npm install -g openupm-cli`.
   - Now you can install packages from the `OpenUPM` registry without any additional steps.
1. Install the package via `OpenUPM`:
   - Open a CLI, navigate to your Unity project folder: `cd /path/to/your/project`.
   - Run: `openupm add com.manefunction.dotween-ui-controller`.
   - Switch back to `Unity` and wait for the package to finish installing.

# Disclaimer
This controller is made to work with [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676), a tweening library created and copyrighted by [Daniele Giardini](http://blog.demigiant.com).  
`DOTween` is not created, owned, or maintained by me, and all rights to `DOTween` belong to its respective author.  
For more information about `DOTween`, including licensing and terms of use, please visit the [official website](http://dotween.demigiant.com/).
