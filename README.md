# 🛡️ SafeWalkAR
### AI-Based AR Navigation Service for Children's Safe School Commute

<p align="center">
<img src="images/cover.png" width="100%">
</p>

<p align="center">

![Unity](https://img.shields.io/badge/Unity-2022.3-black?logo=unity)
![Swift](https://img.shields.io/badge/Swift-5-orange?logo=swift)
![Python](https://img.shields.io/badge/Python-3.10-blue?logo=python)
![Flask](https://img.shields.io/badge/Flask-black?logo=flask)
![AWS](https://img.shields.io/badge/AWS-EC2-orange?logo=amazonaws)
![AR Foundation](https://img.shields.io/badge/AR-Foundation-blue)
![ARKit](https://img.shields.io/badge/ARKit-iOS-lightgrey)

</p>

---

# 📖 Overview

SafeWalkAR는 AI 기반 위험도 분석과 AR 내비게이션을 결합하여 어린이가 **스스로 안전한 등하굣길을 이동할 수 있도록 지원하는 서비스**입니다.

기존 어린이 안전 서비스는 보호자가 아이의 위치를 확인하거나 위험 상황을 알리는 기능에 집중되어 있습니다.

SafeWalkAR는 한 단계 더 나아가, **AI가 분석한 안전 경로를 실제 도로 위에 AR 코인 형태로 시각화**하여 아이가 게임처럼 즐겁게 길을 따라갈 수 있도록 설계했습니다.

본 프로젝트는 2025학년도 캡스톤디자인 프로젝트로 진행되었으며,

- AI 위험도 분석
- 안전 경로 생성
- AWS 기반 API 서버
- Mobile Navigation
- Unity AR Navigation

까지 하나의 서비스로 구현하였습니다.

---

# 🎥 Demo

## 💡 Idea Introduction

https://youtu.be/zockLh4R2Lk

프로젝트의 기획 배경과 서비스 아이디어를 소개하는 영상입니다.

---

## 🚀 Final Demonstration

https://youtu.be/lKd-h1pzmws

실제 구현된 SafeWalkAR 서비스 시연 영상입니다.

---

# 🚸 Background

<p align="center">
<img src="images/problem.png" width="100%">
</p>

최근 맞벌이 가정 증가와 초등학생의 자율 등하교 증가로 인해 어린이 보행 안전 문제가 지속적으로 발생하고 있습니다.

기존 서비스들은 위치 공유 또는 보호자 중심의 모니터링 기능은 제공하지만,

> **아이 스스로 안전한 길을 선택하고 따라갈 수 있도록 돕는 서비스는 부족했습니다.**

SafeWalkAR는 이러한 문제를 해결하기 위해

- AI 기반 위험도 분석
- 안전 경로 추천
- AR 길 안내

를 하나의 서비스로 통합하였습니다.

---

# ✨ Key Features

✅ AI 기반 위험도 분석

도로별 위험도를 계산하여 가장 안전한 경로 생성

---

✅ Mobile Navigation

생성된 안전 경로를 모바일 지도에서 직관적으로 확인

---

✅ AR Navigation

실제 도로 위에 AR 코인을 생성하여 아이가 자연스럽게 길을 따라 이동

---

✅ Gamification

코인 수집 요소를 추가하여 아이들의 자발적인 참여 유도

---

✅ Cloud API

AWS EC2 + Flask 기반 서버를 통해 AI 분석 결과를 모바일 앱으로 전달

---

# 🔄 Service Flow

<p align="center">
<img src="images/flow.png" width="100%">
</p>

SafeWalkAR는 다음과 같은 순서로 동작합니다.

1. 출발지와 목적지를 입력
2. AI가 위험도를 분석하여 안전 경로 생성
3. Mobile App에서 안전 경로 확인
4. Unity AR Navigation 실행
5. 현실 공간에 AR 코인 생성
6. 코인을 따라 목적지까지 이동
