<!--!
\file README_KO.md
\brief Dreamine.MVVM.Core - 경량 의존성 주입 및 인프라 모듈.
\details Dreamine MVVM 프레임워크를 위한 코어 컨테이너 및 공통 인프라 서비스를 제공합니다.
\author Dreamine
\date 2026-03-15
\version 1.0.0
-->

# Dreamine.MVVM.Core

**Dreamine.MVVM.Core**는 Dreamine MVVM 프레임워크의 **경량 인프라 코어 모듈**입니다.

이 패키지는 명시적인 아키텍처, 생성자 기반 객체 해석, 낮은 복잡도의 애플리케이션 구성을 지원하기 위한 최소 런타임 기반을 제공합니다.

의도적으로 작고 집중된 역할만 담당합니다.

[➡️ English Version](README.md)

---

## 이 라이브러리가 제공하는 것

현재 Dreamine.MVVM.Core는 다음 역할에 집중합니다.

- 경량 의존성 등록
- 생성자 기반 객체 해석
- 싱글턴 및 팩토리 기반 등록
- Dreamine MVVM 모듈을 위한 최소 공통 인프라 제공

---

## 주요 기능

- **DMContainer** 경량 의존성 주입 컨테이너
- 명시적 등록 모델
- 생성자 기반 객체 해석
- 단순한 싱글턴 캐싱
- 가벼운 프레임워크 구조

---

## 요구사항

- **.NET**: `net8.0`

---

## 설치

### 옵션 A) 프로젝트 참조

```xml
<ItemGroup>
  <ProjectReference Include="..\Dreamine.MVVM.Core\Dreamine.MVVM.Core.csproj" />
</ItemGroup>
```

### 옵션 B) NuGet

향후 릴리스에서 NuGet 패키징이 제공될 예정입니다.

---

## 프로젝트 구조

```
Dreamine.MVVM.Core
└── DMContainer.cs
```

---

## 빠른 시작

### 서비스 등록

```csharp
DMContainer.Register<IMyService>(() => new MyService());
```

### 서비스 해석

```csharp
IMyService service = DMContainer.Resolve<IMyService>();
```

### 싱글턴 등록

```csharp
var service = new MyService();
DMContainer.RegisterSingleton<IMyService>(service);
```

---

## 컴포넌트 설명

### DMContainer

`DMContainer`는 이 패키지의 핵심 인프라 컴포넌트입니다.

주요 역할:

- 팩토리 델리게이트 등록
- 싱글턴 인스턴스 등록
- 생성자 의존성 해석
- 지원 대상 타입 자동 등록

예시:

```csharp
DMContainer.Register<IMessageService>(() => new MessageService());

var messageService = DMContainer.Resolve<IMessageService>();
```

---

## 설계 목표

Dreamine.MVVM.Core는 다음을 우선합니다.

- 숨겨진 마법보다 명시적 동작
- 낮은 의존성 표면적
- 예측 가능한 생성자 기반 구성
- 상위 모듈을 위한 단순한 확장 지점

---

## 관련 모듈

일반적으로 다음 Dreamine 패키지와 함께 구성됩니다.

- `Dreamine.MVVM.Interfaces`
- `Dreamine.MVVM.ViewModels`
- `Dreamine.MVVM.Locators`
- `Dreamine.MVVM.Wpf`

---

## License

MIT License
