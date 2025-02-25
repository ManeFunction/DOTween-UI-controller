# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [1.1.0] - 2025-04-02
### Added
- Added PlayBackwards() method (and Editor button).
- Added RestartBackwards() method (and Editor button).
- Added Rewind() method (and Editor button).
- Added UndoAndDispose() method.

### Changed
- Changed view of Editor buttons. Become more compact and less intrusive. Tooltips added.

### Fixed
- Fixed an issue that you could not change animation parameters in the Play mode. Added a special button to recreate tweens.

## [1.0.1] - 2025-06-01
### Fixed
- Fixed missing `com.unity.ugui` dependency in the package manifest. Not critical, but it's better to have it.

## [1.0.0] - 2025-06-01
### Added
- Added installation instructions to the `README.md`.

## [1.0.0-preview.1] - 2024-12-05
### Added
- Added support for `Unity 2021.2+` (`2022+` was minimal before, and I personally do not recommend using of `2021.1` due to the editor problems in that period).

### Removed
- Removed bold highlighting of Ease toggle. 

### Fixed
- Fixed wrong behaviour of animation block in inspector on `Unity 2022.2-`.
- Fixed differences in properties layout before and after `Unity 2022.2`.
- Fixed versioning in the package.
- Fixed `CHANGELOG.md` formatting.

## [0.9.1-preview] - 2024-11-25
### Fixed
- Replaced `DOTween` dependency from GUID to string linking, as it is more flexible.

## [0.9.0-preview] - 2024-11-25
### Added
- Initial release of the tool.
