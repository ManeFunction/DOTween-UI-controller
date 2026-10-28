# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [2.2.0] - 2026-10-28
### Added
- Added dependency to [ManeTools-Unity](https://github.com/ManeFunction/ManeTools-Unity.git).
- `DOTweenUIController.GetRequiredComponent(TweenType)` returns the component a tween slot animates.

### Changed
- `OptionalSizeField` replaced with universal `LimitedIntField` from the dependent package.
- Tween blocks in the inspector use the `FeatureBlock` control from the dependent package, the same one `FeatureToggle` builds.
- Moved context "Add component" menu to `Mane Tools -> UI -> DOTween UI Controller`.
- A tween slot whose component is missing (`CanvasGroup` for Fade, a `MaskableGraphic` for Color) is skipped with a warning when tweens are created, instead of throwing.

### Fixed
- With Autoplay off, finished tweens were auto-killed but kept in the controller, so `Play`, `PlayBackwards`, `Restart` and the inspector buttons worked only once. Code-driven tweens are no longer auto-killed, and killed tweens are rebuilt on the next call.
- With one-shot tweens (Autoplay on, Restart On Enable off), a looping slot outlived the others, so `Play` and `Restart` replayed only that slot. If any tween was killed, the rest are now rewound and disposed, and every slot is rebuilt.
- Calling `Play` before `Start` (for example right after `Instantiate`) made `Start` create a second set of tweens that fought the first one.
- With Delay Between Loops set, Autoplay off, Unscaled Time and auto-kill were ignored, because they were applied to the tween nested in the loop sequence.
- Rotate animated the world rotation, although it is documented as local Z, so under a rotated parent the element ended at a different angle than set. It now animates the local rotation, like Move and Scale work in the parent's space.
- When a tween's component was missing, the inspector switched that tween off just by being opened, without Undo and without marking the object dirty, so the change could be lost or saved by accident. It now only shows the warning and locks the toggle; the setting is kept, and the tween is skipped at runtime until the component is added.
- Pause On Disable resumed tweens on enable that were never played or were paused from code. Only the tweens it paused are resumed now.


## [2.1.0] - 2026-09-02
### Added
- Added summary docs for all public methods and classes.

## [2.0.0] - 2026-09-02
Adapted package to `ManeTools` package family (see my [GitHub](https://github.com/ManeFunction) for more useful Unity tools).

### Changed
- BREAKING CHANGE: Namespace was changed to `Mane.Unity.DOTween`.
- Moved context "Add component" menu to `Mane Tools -> Components -> DOTween UI Controller`.
- Set up asmdef's with new namespaces and removed auto-referencing from the Editor.
- `Loop Count` field now renders "None" instead of "0".

## [1.4.1] - 2026-09-01
### Changed
- Update minimum `uGUI` dependency version to `2.0.0` (merge of TMPro).

## [1.4.0] - 2026-08-29
### Added
- Added custom 'Add Component' context menu (`DOTween -> UI Controller`).

### Changed
- A bit refactoring and some lagacy code clean up.

## [1.3.1] - 2026-08-22
### Changed
- Upgraded editor UI with built-in Unity tool for UI verification.

## [1.3.0] - 2026-08-21
### Added
- Added RectTransform checks for all transform related tweens.
- Added "Add" button to all warnings that indicates missing components.
- `Loop Count` field now renders an explicit "Infinity" label instead of just "-1" indicator.

### Changed
- Removed editor fixes that was applied for the earlier versions of `UI Toolkit` (Unity 2022.2).

## [1.2.3] - 2026-08-18
### Fixed
- Updated `README.md`.

## [1.2.2] - 2026-08-13
### Fixed
- Updated `CHANGELOG.md`.

## [1.2.0] - 2026-08-13
### Removed
- Removed support for Unity versions older than the latest patch of the earliest Unity 6 LTS (6.0.81).

### Fixed
- Fixed tooltips for the runtime editor buttons and updated some icons.

## [1.1.0] - 2025-04-02
### Added
- Added PlayBackwards() method (and Editor button).
- Added RestartBackwards() method (and Editor button).
- Added Rewind() method (and Editor button).
- Added UndoAndDispose() method.

### Changed
- Changed the look of the Editor buttons. They are now more compact and less intrusive. Added tooltips.

### Fixed
- Fixed an issue where you could not change animation parameters in Play mode. Added a special button to recreate tweens.

## [1.0.1] - 2025-06-01
### Fixed
- Fixed missing `com.unity.ugui` dependency in the package manifest. Not critical, but it's better to have it.

## [1.0.0] - 2025-06-01
### Added
- Added installation instructions to the `README.md`.

## [1.0.0-preview.1] - 2024-12-05
### Added
- Added support for `Unity 2021.2+` (`2022+` was the minimum before, and I personally do not recommend using `2021.1` due to editor problems in that period).

### Removed
- Removed bold highlighting of the Ease toggle. 

### Fixed
- Fixed incorrect behaviour of the animation block in the inspector on `Unity 2022.2-`.
- Fixed differences in properties layout before and after `Unity 2022.2`.
- Fixed versioning in the package.
- Fixed `CHANGELOG.md` formatting.

## [0.9.1-preview] - 2024-11-25
### Fixed
- Changed the `DOTween` dependency from GUID to string linking, as it is more flexible.

## [0.9.0-preview] - 2024-11-25
### Added
- Initial release of the tool.
