# DOTween UI controller
Neat Unity controller component to set up UI **DOTween** animations without any code. **DOTween Pro** is not required, but supported!

# Features
1. Seven pre-configured tweens for all essential UI animation operations: **Move** (X/Y), **Scale** (X/Y), **Rotate** (Z only), **Color** (for images), and **Fade** (adjusting alpha for CanvasGroups).
1. A powerful inspector built with the UI Toolkit, designed to optimize editor rendering performance.
1. Lazy initialization of required components to streamline the workflow and enhance efficiency.
1. Flexible Unity lifecycle integration, allowing tweens to, for example, pause on OnDisable() or be reused and replayed on OnEnable().
1. Separate tweens for X and Y axes in move and scale animations, providing finer control.
1. Looping support via Sequences, enabling comprehensive control over delay configurations.
1. Easing options with support for both custom curves and build-in mathematical easing functions.
1. An autoplay feature with optional delays, reducing the need for coding—ideal for animators.
1. Full programmatic control is also available for developers who prefer working directly in code.
1. The ability to replay animations directly from the editor for testing and fine-tuning without needing to relaunch your game (available only in Play mode due to DOTween limitations).

# Look
![](https://raw.githubusercontent.com/wiki/ManeFunction/DOTween-UI-controller/main.png)

# Installation
// TODO

# Disclaimer
This controller is made to work with [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676), a tweening library created and copyrighted by [Daniele Giardini](http://blog.demigiant.com). DOTween is not created, owned, or maintained by me, and all rights to DOTween belong to its respective author. For more information about DOTween, including licensing and terms of use, please visit the [official website](http://dotween.demigiant.com/).
