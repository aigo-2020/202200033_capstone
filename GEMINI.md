# Project Mandates

This file is the primary authority for Gemini CLI operations within this project.

## 1. Language & Communication
*   **Output Language**: All responses MUST be in **Korean**.
*   **Code Comments**: Use **Korean** for comments in any new or modified code.
*   **Tone**: Maintain a professional, polite (honorifics), and objective tone.
*   **Educational Support**: Provide high-level mechanism summaries and operational principles for the student developer.

## 2. Workflow
*   **Summary First**: Summarize proposed changes and obtain approval BEFORE executing any file modifications.
*   **Project Memory**: Regularly reference and maintain `Assets/ProjectSummary.md` to keep track of system architecture and progress.

## 3. Technical Standards
*   **Unity Conventions**: Follow standard Unity naming conventions and component-based design.
*   **Architecture Integrity**: Maintain consistency with existing systems (e.g., `PlayerStats` centric stat management, singleton patterns).
*   **Robustness**: Prioritize stability by using null checks and `TryGetComponent` to prevent `NullReferenceException`.
*   **File Integrity**: 
    *   **절대 엄금**: 셸 명령어나 외부 툴을 통한 파일 이동 및 폴더 생성을 금지합니다. 유니티 `.meta` 파일 참조 유실 방지를 위해 모든 파일 구조 변경은 유니티 에디터 내에서 사용자가 직접 수행해야 합니다.
    *   **보고**: 구조 변경이 필요할 경우 반드시 사용자에게 보고하고 요청합니다.
