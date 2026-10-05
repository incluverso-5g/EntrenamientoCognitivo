# Entrenamiento Cognitivo

Unity application for immersive cognitive training in Virtual Reality, developed as part of the **Incluverso 5G** project.

The application provides a set of VR-based cognitive training activities designed to support cognitive and functional training for people with intellectual disabilities.

The repository contains the Unity project and application builds, but **does not include the assets from the supermarket and cafeteria environments**.

## Repository

https://github.com/incluverso-5g/EntrenamientoCognitivo

## Requirements

- Unity **2022.3.29f1**
- Android Build Support
- VR-compatible Android device
- VIVE Wave SDK packages
- Unity XR Interaction Toolkit

The Unity project uses the following main packages:

- VIVE Wave Essence
- VIVE Wave Native
- VIVE Wave XR SDK
- Unity XR Interaction Toolkit 2.5.4
- Universal Render Pipeline 14.0.11
- TextMeshPro
- Unity Timeline

The exact package versions and dependencies are specified in `Packages/manifest.json`.

## Project structure

```text
EntrenamientoCognitivo/
├── Assets/
├── Packages/
├── ProjectSettings/
├── aplicacion/
├── SD/
├── Cognitivo.apk
├── Cognitivo_noLogs.apk
├── LICENSE
└── ...
```

`Assets/` contains the Unity project assets and scripts.

`Packages/` contains the Unity package dependencies.

`ProjectSettings/` contains the Unity project configuration.

`aplicacion/` contains additional application-related files.

`SD/` contains supporting project files.

The repository also provides two Android application builds:

- `Cognitivo.apk` – application build with logging.
- `Cognitivo_noLogs.apk` – application build without logging.

## Opening the project

Clone the repository:

```bash
git clone https://github.com/incluverso-5g/EntrenamientoCognitivo.git
cd EntrenamientoCognitivo
```

Open the project with **Unity 2022.3.29f1**.

Unity should automatically resolve the packages specified in `Packages/manifest.json`.

> The project should be opened with the specified Unity version to avoid compatibility issues.

## Missing assets

The repository intentionally does not include the assets corresponding to the **supermarket and cafeteria environments**.

Consequently, opening the project from a clean clone may result in missing assets or incomplete scenes.

These assets are not redistributed in this repository.

## Android application

Pre-built Android applications are provided in the repository:

```text
Cognitivo.apk
Cognitivo_noLogs.apk
```

The first build includes the application's logging functionality, while the second build is intended for use without logging.

The APKs can be installed on a compatible Android-based VR device for testing.

## Cognitive training application

The application implements immersive cognitive-training activities in VR. The training environment includes scenarios based on everyday activities, including:

- Cafeteria tasks
- Supermarket tasks
- Cognitive training activities
- Interaction with virtual objects
- Task progression and feedback

The supermarket and cafeteria environments are part of the application, but their original assets are not included in this repository.

## Data and logging

The application includes logging functionality for recording information generated during training sessions.

Depending on the application build and configuration, logs can be generated during execution and used for subsequent analysis.

The repository does **not** distribute participant study data.

Any data collected from participants should be handled according to the applicable ethical, privacy and data-protection requirements.

## Relation to the Incluverso 5G project

This application was developed within the **Incluverso 5G** project, which explores the use of Extended Reality technologies for accessible training and intervention.

The cognitive-training application forms part of the project's work on immersive cognitive and functional training for people with intellectual disabilities.

## License

This project is distributed under the **MIT License**.

See [`LICENSE`](LICENSE) for the complete license text.
