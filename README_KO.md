<!--!
\file README_ko.md
\brief Dreamine.MVVM.Core - WPF 애플리케이션을 위한 경량 MVVM 인프라.
\details 아키텍처 / 설치 / 퀵스타트 / 컴포넌트 설명.
\author Dreamine
\date 2026-03-08
\version 1.0.0
-->

# Dreamine.MVVM.Core

**Dreamine.MVVM.Core**는 .NET 기반 WPF 애플리케이션을 위한 **경량 MVVM 프레임워크 코어 모듈**입니다.

MVVM 구현에 필요한 핵심 요소만 제공하며 **명확한 구조와 개발자 제어권**을 유지하도록 설계되었습니다.

[➡️ English Version](README.md)

---

## 이 라이브러리가 해결하는 문제

기존 MVVM 프레임워크들은 다음과 같은 문제를 가지는 경우가 많습니다.

- 복잡한 설정
- 과도한 DI 프레임워크 의존
- 숨겨진 View ↔ ViewModel 연결
- 많은 반복 코드

Dreamine.MVVM.Core는 이러한 문제를 해결하기 위해 **최소한의 MVVM 코어 구조**를 제공합니다.

---

## 주요 기능

- **ViewModelBase** : MVVM 속성 변경 알림 지원
- **RelayCommand** : ICommand 구현
- **DMContainer** : 경량 DI 컨테이너
- **DreamineAppBuilder** : 애플리케이션 초기화
- 명시적 MVVM 구조
- 최소 보일러플레이트 코드

---

## 요구사항

- **.NET** : `net8.0-windows`
- **UI Framework** : WPF

---

## 설치

### 옵션 A) 프로젝트 참조

```xml
<ItemGroup>
  <ProjectReference Include="..\Dreamine.MVVM.Core\Dreamine.MVVM.Core.csproj" />
</ItemGroup>
```

### 옵션 B) NuGet

향후 NuGet 패키지로 배포될 예정입니다.

---

## 프로젝트 구조

```
Dreamine.MVVM.Core
├── ViewModelBase.cs
├── RelayCommand.cs
├── DMContainer.cs
└── DreamineAppBuilder.cs
```

---

## 아키텍처

```
Application
│
├── DreamineAppBuilder
│
├── DMContainer
│
├── View
│    ↔ ViewModelBase
│
└── RelayCommand
```

---

## 퀵스타트

### 1) Dreamine 초기화

```csharp
DreamineAppBuilder.Initialize(
    Assembly.GetExecutingAssembly());
```

---

### 2) ViewModel 작성

```csharp
public class MainViewModel : ViewModelBase
{
    private string _title;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}
```

---

### 3) Command 추가

```csharp
public RelayCommand SaveCommand { get; }

public MainViewModel()
{
    SaveCommand = new RelayCommand(Save);
}

private void Save()
{
    Console.WriteLine("Saved");
}
```

---

## 컴포넌트 설명

### ViewModelBase

모든 ViewModel의 기본 클래스입니다.

- INotifyPropertyChanged 구현
- SetProperty 제공

---

### RelayCommand

UI 이벤트 처리를 위한 ICommand 구현 클래스입니다.

---

### DMContainer

Dreamine에서 사용하는 경량 DI 컨테이너입니다.

```csharp
DMContainer.Register<IMyService>(() => new MyService());
var service = DMContainer.Resolve<IMyService>();
```

---

### DreamineAppBuilder

Dreamine MVVM 환경을 초기화합니다.

- DI 컨테이너 초기화
- ViewModel 검색
- 애플리케이션 부트스트랩

---

## 프레임워크 비교

| 프레임워크 | 복잡도 | 코드량 | 제어권 |
|-------------|--------|--------|--------|
| Prism | 높음 | 높음 | 중간 |
| MVVMLight | 중간 | 중간 | 중간 |
| CommunityToolkit | 중간 | 중간 | 중간 |
| ReactiveUI | 매우 높음 | 높음 | 낮음 |
| Dreamine | 낮음 | 낮음 | 높음 |

Dreamine은 **명확한 구조와 개발자 제어권**을 중요하게 생각합니다.

---

## 로드맵

향후 Dreamine 모듈:

```
Dreamine.Container
Dreamine.MVVM.Generator
Dreamine.Hybrid
Dreamine.Threading
Dreamine.Actuator
```

---

## License

MIT License
