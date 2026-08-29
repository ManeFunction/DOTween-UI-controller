# DOTween UI controller

A neat Unity controller component to set up UI `DOTween` animations without any code. `DOTween Pro` is not required, but it is supported!

The package is supported starting from **Unity 6.0.81** (6000.1.81f1)

## Features

- Seven pre-configured tweens for all essential UI animation operations: **Move** (X/Y), **Scale** (X/Y), **Rotate** (Z only), **Color** (for Images and other MaskableGraphic components), and **Fade** (for the alpha property of CanvasGroup).
- A powerful inspector built with the `UI Toolkit`, designed to optimize editor rendering performance.
- Lazy initialization of required components to streamline the workflow and enhance efficiency.
- Flexible Unity lifecycle integration, allowing tweens to pause on `OnDisable()`, for example, or to be reused and replayed on `OnEnable()`.
- Separate tweens for X and Y axes in move and scale animations, providing finer control.
- Looping support via Sequences, enabling comprehensive control over delay configurations.
- Easing options with support for both custom curves and built-in mathematical easing functions.
- An autoplay feature with optional delays, reducing the need for coding — ideal for animators.
- Full programmatic control is also available for developers who prefer working directly in code.
- The ability to replay animations directly from the editor for testing and fine-tuning without relaunching your game (available only in `Play mode` due to `DOTween` limitations).

## Look

![](https://raw.githubusercontent.com/wiki/ManeFunction/DOTween-UI-controller/main.png)

## Installation

I recommend installing this package with the `OpenUPM` CLI. It keeps dependencies and updates easy to manage. If you cannot use `OpenUPM`, download the package and place it anywhere in your Unity project.

Setting up `OpenUPM` for the first time takes a few minutes, but it is worth it. `OpenUPM` is the usual registry for open-source Unity packages and works with Unity’s Package Manager (dependency resolution and updates included).

On Windows, I recommend `Git Bash` (`MINGW`) for CLI work: it is a Unix-like shell, and it is often already installed.

![](https://raw.githubusercontent.com/wiki/ManeFunction/DOTween-UI-controller/asmdef.png)

1. Prepare your `DOTween` package for use by other tools:
   - Open `Tools -> Demigiant -> DOTween Utility Panel`.
   - Click `Create ASMDEF...`. (If you see a `Remove ASMDEF...` button, do nothing and skip to the next step.)
   - If you see compilation errors afterward, add the `DOTween` asmdef to the References of your own asmdef
     for any modules that use `DOTween`.
1. Install `OpenUPM` (skip this if you already have it):
   - If you do not have `npm`, install [Node.js](https://nodejs.org) (or on macOS: `brew install node`).
   - In a terminal, run: `npm install -g openupm-cli`.
   - You can then install packages from the `OpenUPM` registry with no extra Unity setup.
1. Install this package:
   - Open a terminal in your Unity project folder: `cd /path/to/your/project`.
   - Run: `openupm add com.manefunction.dotween-ui-controller`.
   - Switch back to Unity and wait for the package to finish importing.

## Usage

Open the `Add Component` menu on a UI object, go to `DOTween -> UI Controller`, set up, and you are good.

## Disclaimer

This controller is made to work with [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676), a tweening library created and copyrighted by [Daniele Giardini](http://blog.demigiant.com).  
`DOTween` is not created, owned, or maintained by me, and all rights to `DOTween` belong to its respective author.
For more information about `DOTween`, including licensing and terms of use, please visit the [official website](http://dotween.demigiant.com/).

## Repository info

This repo follows the [Conventional Commits](https://www.conventionalcommits.org/) specification (though not from the start).

[![GitHub Sponsors](https://img.shields.io/github/sponsors/ManeFunction?label=Sponsor&logo=GitHubSponsors&style=flat)](https://github.com/sponsors/ManeFunction)
[![openupm](https://img.shields.io/npm/v/com.manefunction.dotween-ui-controller?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.manefunction.dotween-ui-controller/)
[![openupm](https://img.shields.io/badge/dynamic/json?color=brightgreen&label=downloads&query=%24.downloads&suffix=%2Fmonth&url=https%3A%2F%2Fpackage.openupm.com%2Fdownloads%2Fpoint%2Flast-month%2Fcom.manefunction.dotween-ui-controller)](https://openupm.com/packages/com.manefunction.dotween-ui-controller/)
