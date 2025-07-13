# Guitar Learning Game

**An educational game that teaches guitar fundamentals through interactive gameplay and real-time pitch detection.**

[![Unity](https://img.shields.io/badge/Unity-2021.3+-black.svg?style=flat&logo=unity)](https://unity3d.com/)
[![Python](https://img.shields.io/badge/Python-3.x-blue.svg?style=flat&logo=python)](https://www.python.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## Overview

This project is an **educational game** developed to teach guitar basics through an interactive digital platform. By combining game development with advanced pitch detection algorithms, it creates an engaging learning experience that bridges the gap between traditional music education and modern gaming.

The game uses **real-time audio analysis** to provide immediate feedback on guitar notes played by users, making the learning process both effective and enjoyable.

---

## Features

- **Real-time Pitch Detection**: Utilizes the refined YIN algorithm for accurate note recognition
- **Interactive Gameplay**: Explore a game world while learning guitar fundamentals
- **Music Theory Integration**: Learn musical notes, frequencies, and their relationships
- **Immediate Feedback**: Get instant results on your guitar playing accuracy
- **Visual Learning**: See frequency graphs and note representations in real-time
- **Progressive Difficulty**: Start with single notes and advance as you improve

---

## Technical Architecture

### Core Components

#### Python Scripts
- **`Suyash_YIN.py`**: Implements the YIN pitch detection algorithm, adapted to compute average frequency within specific time brackets
- **`Audio_processing.py`**: Handles audio format conversion and standardization to WAV format

#### Unity C# Scripts
- **`Audio_Recorder.cs`**: Manages microphone input and audio recording within Unity
- **`CallYin.cs`**: Creates interface between Unity (C#) and Python scripts
- **`Note_Played.cs`**: Identifies and displays the note played by the user
- **`Verify_Note.cs`**: Compares played notes against target notes for feedback
- **`Player_Movement.cs`**: Handles player navigation and collision detection
- **`Camera_Follow.cs`**: Ensures smooth camera movement following the player
- **`StartPopUp.cs`**: Manages initial game interactions and tutorials

### Algorithm Details

The **YIN algorithm** was specifically refined for this educational context:
- Focuses on computing average frequency over 5-second time windows
- Provides stable and consistent pitch measurements
- Optimized for real-time feedback in learning scenarios
- Handles harmonic detection and filtering

---

## How It Works

1. **Player Interaction**: Navigate the game world and interact with NPCs
2. **Musical Challenges**: NPCs present guitar notes to play
3. **Audio Recording**: Use Unity's microphone to record your guitar playing
4. **Pitch Analysis**: The YIN algorithm analyzes the recorded audio
5. **Feedback**: Receive immediate feedback on accuracy
6. **Progress**: Advance through increasingly complex musical challenges

---

## Setup and Requirements

### Prerequisites
- Unity Game Engine
- Python 3.x with NumPy, SciPy, and PyDub libraries
- Microphone for audio input
- Guitar (electric or acoustic)

### Project Setup
1. Clone the repository to your local machine
2. Open the project folder in Unity
3. Ensure Python dependencies are installed: `numpy`, `scipy`, `pydub`
4. Configure Unity's microphone permissions in project settings
5. Audio files must be in WAV format for proper processing
6. Run the game through Unity Editor or build for target platform

---

## Educational Approach

The game introduces fundamental music theory concepts:
- Musical notes (A, A#, B, C, C#, D, D#, E, F, F#, G, G#)
- Note frequencies and octaves
- Pitch recognition and accuracy
- Real-time performance feedback

---

## Learning Outcomes

Through gameplay, users will:
- Understand the relationship between notes and frequencies
- Develop pitch recognition skills
- Learn to play accurate single notes on guitar
- Build a foundation for more advanced guitar techniques
- Gain confidence through immediate feedback

---

## Research Background

This project was developed as part of **undergraduate research at FLAME University**, exploring the intersection of:
- Game-based learning
- Music education technology
- Real-time audio processing
- Interactive educational design

---

## Limitations and Future Scope

### Current Limitations
- Optimized for single-note detection (chord recognition not yet implemented)
- Requires quiet environment for optimal accuracy
- Limited to basic guitar techniques
- Dependency on external Python scripts for audio processing

### Future Enhancements
- Chord recognition and progression analysis
- Expanded game world with additional musical challenges
- Adaptive difficulty based on player performance
- Support for multiple instruments
- Advanced music theory modules
- Multiplayer learning modes
- Integrated audio processing within Unity

---

## Technical Challenges and Solutions

The project addressed several key challenges:
- **Harmonic Detection**: The YIN algorithm was refined to handle guitar harmonics that could interfere with fundamental frequency detection
- **Real-time Processing**: Optimized audio processing pipeline to provide immediate feedback without noticeable latency
- **Cross-platform Communication**: Developed interface between Unity (C#) and Python scripts for seamless audio analysis

---

## Acknowledgments

- **Thesis Advisor**: Professor Prajish Prasad, Computer Science, FLAME University
- **Defense Committee**: Professor Kaushik Gopalan & Professor Prajish Prasad
- **YIN Algorithm**: Based on De Cheveigné & Kawahara (2002), "YIN, a fundamental frequency estimator for speech and music"
- **Unity Assets**: Utilized free educational assets from Unity Asset Store

---

## References

- De Cheveigné, A., & Kawahara, H. (2002). YIN, a fundamental frequency estimator for speech and music. *The Journal of the Acoustical Society of America*, 111(4), 1917-1930.
- Guyot, Patrice. (2018), Fast Python implementation of the Yin algorithm (Version v1.1.1). Zenodo.
- Unity documentation for microphone input and audio processing

---

## Author

**Suyash Aditya Lal**  
FLAME University, Computer Science Honors  
Project Type: Undergraduate Thesis Project

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.