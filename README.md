# BoardGame

# 벽바둑 (WallGo)

**벽바둑**은 플레이어가 말을 이동시키고 벽을 설치하여 상대의 움직임을 방해하거나 공간을 확보하는 전략 보드게임입니다.  
Unity 기반 클라이언트와 C# 기반 서버로 구성되어 있으며, 실시간 멀티플레이를 지원합니다.

---

## 🎮 게임 개요

- 최대 4명의 플레이어가 번갈아 턴을 진행
- 각 플레이어는 자신의 말을 하나씩 보드 위에 배치
- 말은 상하좌우로 이동 가능
- 이동 후에는 주변에 벽을 설치해 다른 플레이어의 경로를 제한
- 점수는 각 말이 확보한 영역에 따라 결정

---

## 🧩 게임 규칙

| 항목 | 내용 |
|------|------|
| 보드 크기 | 7x7 타일 |
| 턴 진행 | 말 배치 → 이동(최대 2번) → 벽 설치 |
| 이동 조건 | 상하좌우 1칸씩, 벽에 막히면 불가능 |
| 벽 설치 | 상하좌우 방향, 자신의 마지막 이동 말 기준 |
| 벽 제거 | 1게임당 1회, 본인 턴 중 사용 가능 |
| 종료 조건 | 모든 말이 연결되지 않은 단일 영역 확보 시 |