# Project Mandates

This file is the primary authority for Gemini CLI operations within this project.

## 1. Language & Communication
*   **Output Language**: Use **Korean** for communication with the user.
*   **Documentation**: Keep **GEMINI.md** and other project documentation in **English** to ensure consistency and avoid encoding issues.
*   **Code Comments**: Use **Korean** for comments in code to help the student developer understand the logic more easily.
*   **Tone**: Maintain a professional, polite, and objective tone.
*   **Educational Support**: Provide high-level mechanism summaries and operational principles for the student developer.

## 2. Workflow
*   **Summary First**: Summarize proposed changes and obtain approval BEFORE executing any file modifications.
*   **Project Memory**: Regularly reference and maintain `Assets/ProjectSummary.md` to keep track of system architecture and progress.

## 3. Technical Standards
*   **Unity Conventions**: Follow standard Unity naming conventions and component-based design.
*   **Architecture Integrity**: Maintain consistency with existing systems (e.g., `PlayerStats` centric stat management, singleton patterns).
*   **Robustness**: Prioritize stability by using null checks and `TryGetComponent` to prevent `NullReferenceException`.
*   **File Integrity**: 
    *   **Strictly Prohibited**: Do not move files or create folders via shell commands or external tools. All structure changes must be done manually by the user within the Unity Editor to prevent loss of `.meta` file references.
    *   **Reporting**: If structural changes are needed, report and request them from the user.
