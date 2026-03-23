using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dreamine.MVVM.Core
{
    /// <summary>
    /// 📦 Dreamine 전용 DI 컨테이너 클래스입니다.
    /// 타입별 팩토리 등록, 싱글턴 등록, 자동 등록, 생성자 기반 DI 등을 지원합니다.
    /// </summary>
    public static partial class DMContainer
    {
        private static readonly Dictionary<Type, Func<object>> _map = new();
        private static readonly Dictionary<Type, object> _singletonCache = new();

        /// <summary>
        /// 주어진 타입 T에 대한 팩토리 함수를 등록합니다.
        /// </summary>
        /// <typeparam name="T">등록할 클래스 타입</typeparam>
        /// <param name="factory">생성 함수</param>
        public static void Register<T>(Func<T> factory) where T : class
            => _map[typeof(T)] = () => factory();

        /// <summary>
        /// 주어진 싱글턴 인스턴스를 타입 T로 등록합니다.
        /// </summary>
        /// <typeparam name="T">등록할 클래스 타입</typeparam>
        /// <param name="instance">싱글턴 인스턴스</param>
        public static void RegisterSingleton<T>(T instance) where T : class
            => _map[typeof(T)] = () => instance;

        /// <summary>
        /// 타입 T의 인스턴스를 Resolve합니다.
        /// 등록된 팩토리를 통해 생성하며, 없을 경우 예외를 발생시킵니다.
        /// </summary>
        /// <typeparam name="T">해결할 타입</typeparam>
        /// <returns>생성된 인스턴스</returns>
        public static T Resolve<T>() where T : class
        {
            if (_map.TryGetValue(typeof(T), out var factory))
            {
                return (T)factory();
            }

            throw new InvalidOperationException($"[{typeof(T).Name}] 등록되지 않음.");
        }

        /// <summary>
        /// 주어진 타입 인스턴스를 Resolve합니다.
        /// 등록되지 않은 경우 생성자 기반 자동 생성도 시도합니다.
        /// </summary>
        /// <param name="type">타겟 타입</param>
        /// <returns>생성된 인스턴스</returns>
        public static object Resolve(Type type)
        {
            if (_map.TryGetValue(type, out var factory))
            {
                return factory();
            }

            // Fallback: 자동 생성 시도 (등록 안됐지만 생성 가능한 경우)
            if (!type.IsAbstract && type.IsClass)
            {
                ConstructorInfo? ctor = GetPreferredConstructor(type);
                if (ctor != null)
                {
                    object instance = CreateInstance(type, ctor);

                    // 선택: 한번 생성되면 캐시할 수도 있음
                    _map[type] = () => instance;

                    return instance;
                }
            }

            throw new InvalidOperationException($"[{type.FullName}] 등록되지 않았고, 생성도 불가능합니다.");
        }

        /// <summary>
        /// 주어진 어셈블리 및 현재 AppDomain 내 어셈블리에서
        /// Model, Event, Manager, ViewModel, View 타입들을 자동 등록합니다.
        /// </summary>
        /// <param name="rootAssembly">우선 처리할 주 어셈블리</param>
        public static void AutoRegisterAll(Assembly rootAssembly)
        {
            foreach (Assembly assembly in GetCandidateAssemblies(rootAssembly))
            {
                RegisterAssemblyTypes(assembly);
            }
        }

        /// <summary>
        /// 자동 등록 대상 어셈블리 목록을 반환합니다.
        /// rootAssembly를 우선 순위로 포함하고, 현재 AppDomain에 로드된 유효한 어셈블리만 대상으로 합니다.
        /// </summary>
        /// <param name="rootAssembly">우선 처리할 주 어셈블리</param>
        /// <returns>자동 등록 대상 어셈블리 목록</returns>
        private static IEnumerable<Assembly> GetCandidateAssemblies(Assembly rootAssembly)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.FullName))
                .Prepend(rootAssembly)
                .Distinct();
        }

        /// <summary>
        /// 지정된 어셈블리의 타입들을 순회하며 자동 등록을 시도합니다.
        /// </summary>
        /// <param name="assembly">검사할 어셈블리</param>
        private static void RegisterAssemblyTypes(Assembly assembly)
        {
            foreach (Type type in SafeGetTypes(assembly))
            {
                TryRegisterType(type);
            }
        }

        /// <summary>
        /// 지정된 타입이 자동 등록 대상이면 팩토리를 등록합니다.
        /// </summary>
        /// <param name="type">등록 후보 타입</param>
        private static void TryRegisterType(Type type)
        {
            if (!IsRegistrableConcreteType(type))
            {
                return;
            }

            if (!IsAutoRegisterTarget(type))
            {
                return;
            }

            if (_map.ContainsKey(type))
            {
                return;
            }

            Func<object>? factory = CreateFactory(type);
            if (factory is null)
            {
                return;
            }

            _map[type] = factory;
        }

        /// <summary>
        /// 자동 등록 가능한 일반 클래스인지 확인합니다.
        /// </summary>
        /// <param name="type">검사할 타입</param>
        /// <returns>등록 가능한 일반 클래스이면 true</returns>
        private static bool IsRegistrableConcreteType(Type type)
        {
            return type.IsClass && !type.IsAbstract && !type.IsGenericType;
        }

        /// <summary>
        /// 타입명이 자동 등록 규칙에 해당하는지 확인합니다.
        /// </summary>
        /// <param name="type">검사할 타입</param>
        /// <returns>자동 등록 대상이면 true</returns>
        private static bool IsAutoRegisterTarget(Type type)
        {
            string name = type.Name;

            return name.EndsWith("Model", StringComparison.Ordinal) ||
                   name.EndsWith("Event", StringComparison.Ordinal) ||
                   name.EndsWith("Manager", StringComparison.Ordinal) ||
                   name.EndsWith("ViewModel", StringComparison.Ordinal) ||
                   name.Contains(".xaml.ViewModel", StringComparison.Ordinal) ||
                   name.Contains(".xaml.Model", StringComparison.Ordinal) ||
                   name.Contains(".xaml.Event", StringComparison.Ordinal);
        }

        /// <summary>
        /// 지정된 타입에 대한 팩토리를 생성합니다.
        /// 생성 가능한 생성자가 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="type">팩토리를 만들 대상 타입</param>
        /// <returns>생성 팩토리 또는 null</returns>
        private static Func<object>? CreateFactory(Type type)
        {
            ConstructorInfo? ctor = GetPreferredConstructor(type);
            if (ctor is null)
            {
                return type.GetConstructor(Type.EmptyTypes) is null
                    ? null
                    : () => Activator.CreateInstance(type)!;
            }

            return () => GetOrCreateSingleton(type, ctor);
        }

        /// <summary>
        /// 생성자 매개변수가 가장 많은 생성자를 우선 생성자로 선택합니다.
        /// </summary>
        /// <param name="type">대상 타입</param>
        /// <returns>선택된 생성자 또는 null</returns>
        private static ConstructorInfo? GetPreferredConstructor(Type type)
        {
            return type.GetConstructors()
                .OrderByDescending(constructor => constructor.GetParameters().Length)
                .FirstOrDefault();
        }

        /// <summary>
        /// 싱글턴 캐시에서 인스턴스를 가져오거나, 없으면 생성 후 캐시합니다.
        /// </summary>
        /// <param name="type">생성 대상 타입</param>
        /// <param name="ctor">사용할 생성자</param>
        /// <returns>생성되었거나 캐시된 인스턴스</returns>
        private static object GetOrCreateSingleton(Type type, ConstructorInfo ctor)
        {
            if (_singletonCache.TryGetValue(type, out object? cached))
            {
                return cached;
            }

            object instance = CreateInstance(type, ctor);
            _singletonCache[type] = instance;

            return instance;
        }

        /// <summary>
        /// 지정된 생성자와 Resolve 결과를 이용해 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="type">생성할 타입</param>
        /// <param name="ctor">사용할 생성자</param>
        /// <returns>생성된 인스턴스</returns>
        private static object CreateInstance(Type type, ConstructorInfo ctor)
        {
            object[] args = ctor.GetParameters()
                .Select(parameter => Resolve(parameter.ParameterType))
                .ToArray();

            return Activator.CreateInstance(type, args)!;
        }

        /// <summary>
        /// 어셈블리 로드 오류 발생 시에도 가능한 타입을 반환하는 안전한 GetTypes
        /// </summary>
        /// <param name="assembly">대상 어셈블리</param>
        /// <returns>유효한 타입 목록</returns>
        private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(type => type != null)!;
            }
        }
    }
}