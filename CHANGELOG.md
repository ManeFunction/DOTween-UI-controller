# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [1.3.1] - 2026-08-22
### Changed
- Upgraded editor UI with built-in Unity tool for UI verification.

## [1.3.0] - 2026-08-21
### Added
- Added RectTransform checks for all transform related tweens.
- Added "Add" button to all warnings that indicates missing components.
- Added Loop Count field now renders an explicit "infinity" label instead of just "-1" indicator.

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
